using FluentValidation;
using Nop.Plugin.Ralfeus.Agent.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Ralfeus.Agent.Validators
{
    public class AgentOrderValidator : BaseNopValidator<AgentOrderModel>
    {
        public AgentOrderValidator(ILocalizationService localizationService)
        {
            RuleFor(m => m.ProductUrl)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Ralfeus.Agent.ProductUrlRequired"));
        }
    }
}