using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Deposit.Models
{
    public class ConfigurationModel  : BaseNopModel, ILocalizedModel<ConfigurationModel.ConfigurationLocalizedModel>
    {
        #region Nested class

        public partial class ConfigurationLocalizedModel : ILocalizedModelLocal
        {
            public int LanguageId { get; set; }

            [AllowHtml]
            [NopResourceDisplayName("Plugins.Payment.Deposit.DescriptionText")]
            public string DescriptionText { get; set; }
        }

        #endregion

        [AllowHtml]
        [NopResourceDisplayName("Plugins.Payment.Deposit.DescriptionText")]
        public string DescriptionText { get; set; }

        public IList<ConfigurationLocalizedModel> Locales { get; set; }
    }
}