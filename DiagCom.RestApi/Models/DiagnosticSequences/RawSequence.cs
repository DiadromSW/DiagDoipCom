using System.ComponentModel.DataAnnotations;

namespace DiagCom.RestApi.Models.DiagnosticSequences
{
    /// <summary>
    /// Payload object for RawDiagnosticSequenceAsync
    /// </summary>
    public class RawSequence
    {
        /// <summary>
        /// Vehicle Identification Number
        /// </summary>
        /// <example>LVX00000000000001</example>
        [Required]
        public string Vin { get; set; }

        /// <summary>
        /// Ecu address given as a hexadecimal string.
        /// </summary>
        /// <example>"1001"</example>
        [Required]
        public string EcuAddress { get; set; }

        /// <summary>
        /// Sequence of payload. Service given as a hexadecimal string.
        /// First byte the UDS service. Then any service specific data.
        /// </summary>
        /// <example>"190220"</example>
        [Required]
        public string Request { get; set; }
    }
}