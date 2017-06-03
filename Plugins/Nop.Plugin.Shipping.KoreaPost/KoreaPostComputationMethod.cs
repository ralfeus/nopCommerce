using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Core.Plugins;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.KoreaPost
{
    public class KoreaPostComputationMethod : BasePlugin, IShippingRateComputationMethod
    {
        private readonly KoreaPostSettings _koreaPostSettings;
        private readonly IShippingService _shippingService;
        private readonly IMeasureService _measureService;
        private readonly ISettingService _settingsService;

        public KoreaPostComputationMethod(KoreaPostSettings koreaPostSettings, IShippingService shippingService, 
            IMeasureService measureService, ISettingService settingsService)
        {
            _koreaPostSettings = koreaPostSettings;
            _shippingService = shippingService;
            _measureService = measureService;
            _settingsService = settingsService;
        }

        public ShippingRateComputationMethodType ShippingRateComputationMethodType => ShippingRateComputationMethodType
            .Realtime;

        public IShipmentTracker ShipmentTracker => null;

        /// <summary>
        ///  Gets available shipping options
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Represents a response of getting shipping rate options</returns>
        public GetShippingOptionResponse GetShippingOptions(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null)
            {
                response.AddError("No shipment items");
                return response;
            }

            if (string.IsNullOrEmpty(getShippingOptionRequest.ZipPostalCodeFrom))
            {
                response.AddError("Shipping origin zip is not set");
                return response;
            }

            if (getShippingOptionRequest.ShippingAddress == null)
            {
                response.AddError("Shipping address is not set");
                return response;
            }

            var country = getShippingOptionRequest.ShippingAddress.Country;
            if (country == null)
            {
                response.AddError("Shipping country is not specified");
                return response;
            }

            if (string.IsNullOrEmpty(getShippingOptionRequest.ShippingAddress.ZipPostalCode))
            {
                response.AddError("Shipping zip (postal code) is not set");
                return response;
            }

            //estimate packaging
            var weight = this.GetWeight(getShippingOptionRequest);
            var destination = country.TwoLetterIsoCode.Equals("KR") ? "Domestic" : "International";

            var maxWeight = this._koreaPostSettings.GetMaxWeight(country.TwoLetterIsoCode);
            if (!maxWeight.HasValue)
            {
                response.AddError($"Shipping country '{country.TwoLetterIsoCode}' wasn't found");
                return response;
            }
            var totalPackages = weight / maxWeight + 1;

            try
            {
                var shippingOptions = RequestShippingOptions(country.TwoLetterIsoCode, weight, totalPackages);

                foreach (var shippingOption in shippingOptions)
                {
                    response.ShippingOptions.Add(shippingOption);
                }
            }
            catch (NopException ex)
            {
                response.AddError(ex.Message);
                return response;
            }
            catch (Exception)
            {
                response.AddError("Australia Post Service is currently unavailable, try again later");
                return response;
            }
            
            foreach (var shippingOption in response.ShippingOptions)
            {
                shippingOption.Rate += this._koreaPostSettings.AdditionalHandlingCharge;
            }
            return response;
        }


        private int GetWeight(GetShippingOptionRequest getShippingOptionRequest)
        {
            var totalWeigth = _shippingService.GetTotalWeight(getShippingOptionRequest);
            var value = Convert.ToInt32(Math.Ceiling(this._measureService.ConvertFromPrimaryMeasureWeight(
                totalWeigth, this._measureService.GetMeasureWeightBySystemKeyword(this._koreaPostSettings.GatewayWeightUnit))));
            return (value < this._koreaPostSettings.MinWeight ? this._koreaPostSettings.MinWeight : value);
        }

        private IList<ShippingOption> RequestShippingOptions(string countryTwoLetterIsoCode, decimal weight, int totalPackages)
        {
            var shippingOptions = new List<ShippingOption>();
            var destination = countryTwoLetterIsoCode.Equals("KR") ? "Domestic" : "International";
            foreach (var option in this._koreaPostSettings.HttpParams[destination].ShippingOptionsParameters)
            {
                
            }
            return shippingOptions;
        }

        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            return null;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "ShippingKoreaPost";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Shipping.KoreaPost.Controllers" }, { "area", null } };
        }

        void IPlugin.Install()
        {
            var settings = new KoreaPostSettings
            {
                AdditionalHandlingCharge = 0,
                GatewayWeightUnit = "grams",
                MinWeight = 1,
                HttpParams =
                {
                    ["International"] = {
                        CalcUrl = "http://service.epost.go.kr/comm/search/cmsc01004.jsp",
                        CountryIsoCodeParamName = "oArrivNationCdFrn1",
                        CountryParamName = "oArrivNationCdFrn",
                        InsuranceAmountParamName = "insrAmount",
                        InsuranceFeeParamName = "oIntlSpclTreatFeeFrn",
                        PackagesCountParamName = "oFrnRegiMailCnt",
                        ShippingFeeParamName = "oIntlRecevBasicPrcFrn",
                        WeightParamName = "prcCalStdWghtFrnRegi",
                        ShippingOptionsParameters =
                        {
                            new ShippingOptions {
                                OptionName = "Normal-Land",
                                HttpParams =
                                {
                                    {"MailDivCd", "2"},
                                    {"oFrnMailKindCdCombo", "21"},
                                    {"oTranspPartyCd", "2"},
                                    {"insChk", ""}
                                }
                            },
                            new ShippingOptions {
                                OptionName = "Normal-Air",
                                HttpParams =
                                {
                                    {"MailDivCd", "2"},
                                    {"oFrnMailKindCdCombo", "21"},
                                    {"oTranspPartyCd", "1"},
                                    {"insChk", ""}
                                }
                            },
                            new ShippingOptions {
                                OptionName = "Insured-Land",
                                HttpParams =
                                {
                                    {"MailDivCd", "2"},
                                    {"oFrnMailKindCdCombo", "22"},
                                    {"oTranspPartyCd", "2"},
                                    {"insChk", "Y"}
                                }
                            },
                            new ShippingOptions {
                                OptionName = "Insured-Air",
                                HttpParams =
                                {
                                    {"MailDivCd", "2"},
                                    {"oFrnMailKindCdCombo", "22"},
                                    {"oTranspPartyCd", "1"},
                                    {"insChk", "Y"}
                                }
                            }
                        }
                    }
                }
            };
            this._settingsService.SaveSetting(settings);
            base.Install();
        }

        void IPlugin.Uninstall()
        {
            this._settingsService.DeleteSetting<KoreaPostSettings>();
            
            base.Uninstall();
        }
    }
}