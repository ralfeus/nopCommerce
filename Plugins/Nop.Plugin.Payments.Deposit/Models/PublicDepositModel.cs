﻿using System.Web.Mvc;
using Nop.Web.Framework;

namespace Nop.Plugin.Payments.Deposit.Models
{
    public class PublicDepositModel
    {
        [NopResourceDisplayName("Payment.Deposit.DepositAmount")]
        [AllowHtml]
        public string DepositAmount { get; set; }
        [AllowHtml]
        public string DepositCurrencyCode { get; set; }
        [AllowHtml]
        public string ChangeCurrencyConfirmationMessage { get; set; }
    }
}