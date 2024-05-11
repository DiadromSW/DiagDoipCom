using DiagCom.RestApi.Models.DiagnosticSequences;
using DiagCom.RestApi.Models.Dtcs;
using DiagCom.RestApi.Models.Logg;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DiagCom.RestApi.Validators.Configuration
{
    internal static class ValidatorConfiguration
    {
        public static void AddPayloadValidators(this IServiceCollection services)
        {
            services.AddTransient<IValidator<ReadDtcPayload>, ReadDtcPayloadValidator>();
            services.AddTransient<IValidator<LogPayload>, LogValidator>();
            services.AddTransient<IValidator<DiagnosticSequencePayload>, DiagnosticSequencePayloadValidator>();

        }
    }
}
