using DiagCom.RestApi.Models;
using DiagCom.RestApi.Models.Logg;
using DiagCom.RestApi.Validators.Static;
using FluentValidation;

namespace DiagCom.RestApi.Validators
{
    class StartLoggingValidator : AbstractValidator<StartLoggingPayload>
    {
        public StartLoggingValidator()
        {
            RuleFor(x => x.Vin).Must(StaticValidator.ValidateVinNumber).WithMessage(ErrorMessages.InvalidVinLength);
            RuleFor(x => x.LevelName).Must(StaticValidator.ValidateLogLevel).WithMessage(ErrorMessages.InvalidLogLevel);
        }
    }
}
