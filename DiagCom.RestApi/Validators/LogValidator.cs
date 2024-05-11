using DiagCom.RestApi.Models;
using DiagCom.RestApi.Models.Logg;
using DiagCom.RestApi.Validators.Static;
using FluentValidation;

namespace DiagCom.RestApi.Validators
{
    public class LogValidator : AbstractValidator<LogPayload>
    {
        public LogValidator()
        {
            RuleFor(x => x.Vin).Must(StaticValidator.ValidateVinNumber).WithMessage(ErrorMessages.InvalidVinLength);
            RuleFor(x => x.LogLevels)
                .Must(ValidateEnumLogLevel)
                .WithMessage("Value does not exist, Make sure log level is correct");
            RuleFor(x => x.Activity)
                .Must(ValidateEnumLogOption).WithMessage("Value does not exist, Make sure activity value is correct");


        }
        private bool ValidateEnumLogLevel(string logLevels)
        {

            if (Enum.TryParse<LogLevels>(logLevels, out _))
            {
                return true;
            }
            return false;
        }
        private bool ValidateEnumLogOption(string logOption)
        {

            if (Enum.TryParse<LogOption>(logOption, out _))
            {
                return true;
            }
            return false;
        }

    }
}
