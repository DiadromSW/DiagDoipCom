using DiagCom.RestApi.Models;
using DiagCom.RestApi.Models.Dtcs;
using DiagCom.RestApi.Validators.Static;
using FluentValidation;

namespace DiagCom.RestApi.Validators
{
    class ReadDtcPayloadValidator : AbstractValidator<ReadDtcPayload>
    {
        public ReadDtcPayloadValidator()
        {
            RuleFor(x => x.Vin).Must(StaticValidator.ValidateVinNumber).WithMessage(ErrorMessages.InvalidVinLength);
            RuleFor(x => x.Ecus.Length)
                .GreaterThanOrEqualTo(1).WithMessage(ErrorMessages.EmptyEcus);
            RuleFor(x => x.Ecus)
                .Must(StaticValidator.ValidEcusFormat).WithMessage(ErrorMessages.InvalidEcuFormat);
        }
    }
}
