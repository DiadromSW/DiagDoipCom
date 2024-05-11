using DiagCom.RestApi.Models;
using DiagCom.RestApi.Models.DiagnosticSequences;
using DiagCom.RestApi.Validators.Static;
using FluentValidation;

namespace DiagCom.RestApi.Validators
{
    public class DiagnosticSequencePayloadValidator : AbstractValidator<DiagnosticSequencePayload>
    {
        public DiagnosticSequencePayloadValidator()
        {
            RuleFor(x => x.Vin).Must(StaticValidator.ValidateVinNumber).WithMessage(ErrorMessages.InvalidVinLength);
        }
    }
}
