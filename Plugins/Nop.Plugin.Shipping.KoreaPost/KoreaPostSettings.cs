using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.KoreaPost
{
    public class KoreaPostSettings : ISettings
    {
        public class CountrySettings
        {
            public string CountryIsoCode => CountryParamNameValue.Substring(0, 2);
            public string CountryParamNameValue { get; set; }
            public int MaxWeight => Convert.ToInt32(CountryParamNameValue.Split(new[] {'-'})[2]);
        }
        
        /// <summary>
        /// Defines Korea Post service weight unit
        /// </summary>
        public string GatewayWeightUnit { get; set; }
        
        /// <summary>
        /// Defines minimal weight of the parcel
        /// </summary>
        public int MinWeight { get; set; }

        /// <summary>
        /// Defines HTTP params for calculation service. Domestic and International services differ in URL and parameters
        /// </summary>
        public Dictionary<string, KoreaPostHttpParams> HttpParams { get; set; }

        /// <summary>
        /// Defines additional charges related specificly to Korea Post shipping method handling
        /// </summary>
        public decimal AdditionalHandlingCharge { get; set; }
        
        public IList<CountrySettings> CountriesSettings { get; set; }

        public int? GetMaxWeight(string destinationCountry)
        {
            return this.CountriesSettings.FirstOrDefault(country => country.CountryIsoCode.Equals(destinationCountry))
                ?.MaxWeight;
        }
    }

    public class KoreaPostHttpParams
    {
        /// <summary>
        /// Defines URL of the calculation service
        /// </summary>
        public string CalcUrl { get; set; }
        /// <summary>
        /// Defines HTTP param of parcel weight
        /// </summary>
        public string WeightParamName { get; set; }
        
        /// <summary>
        /// Defines HTTP param of destination country
        /// </summary>
        public string CountryParamName { get; set; }

        /// <summary>
        /// Defines HTTP param of destination country 2 symbols ISO code
        /// </summary>
        public string CountryIsoCodeParamName { get; set; }

        /// <summary>
        /// Defines HTTP param of calculated shipping fee (returned by calculation service)
        /// </summary>
        public string ShippingFeeParamName { get; set; }
        
        /// <summary>
        /// Defines HTTP param of insurance amount
        /// </summary>
        public string InsuranceAmountParamName { get; set; }

        /// <summary>
        /// Defines HTTP param of calculated insurance fee (returned by calculation service)
        /// </summary>
        public string InsuranceFeeParamName { get; set; }
         
        /// <summary>
        /// Defines set of HTTP parameters for each possible shipping option
        /// Currently (as of 29.05.2017) following options are possible:
        /// 1. Normal Parcel - land shipping (not for all countries)
        /// 2. Normal Parcel - air shipping
        /// 3. Insured Parcel - land shipping (not for all countries)
        /// 4. Insured Parcel - air shipping
        /// </summary>
        public IList<ShippingOptions> ShippingOptionsParameters { get; set; }   
        
        /// <summary>
        /// Defines HTTP param of number of packages
        /// </summary>
        public string PackagesCountParamName { get; set; }
    }

    public class ShippingOptions
    {
        public string OptionName { get; set; }
        public NameValueCollection HttpParams { get; set; }
    }
}