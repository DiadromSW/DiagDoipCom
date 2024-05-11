using System.ComponentModel.DataAnnotations;

namespace DiagCom.RestApi.Models.Dtcs
{
    /// <summary>
    /// ReadClearDTCs payload 
    /// </summary>
    public class ReadDtcPayload
    {
        /// <summary>
        /// Vehicle Idenification Number
        /// </summary>
        /// <example>LVX00000000000001</example>
        [Required]
        public string Vin { get; set; }
        /// <summary>
        /// List of Ecu addresses
        /// </summary>
        /// <example>["1001", "1261"]</example>
        [Required]
        public string[] Ecus { get; set; }
        /// <summary>
        /// Flag if clear Dtc operation will execute before read Dtc operation
        /// </summary>
        public bool? Erase { get; set; } = false;
    }
}
