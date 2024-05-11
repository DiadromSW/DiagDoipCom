using System.ComponentModel.DataAnnotations;

namespace DiagCom.RestApi.Models.Logg
{
    /// <summary>
    ///Startlogging payload class 
    /// </summary>
    public class StartLoggingPayload
    {
        /// <summary>
        /// Vehicle Idenification Number
        /// </summary>
        /// <example>LVX00000000000001</example>
        [Required]
        public string Vin { get; set; }
        /// <summary>
        /// Optional parameter to set different LogLevel from default ("Info"). Available LogLevels: "Trace", "Debug", "Info", "Warn", "Error", "Fatal"
        /// </summary>
        /// <example>Info</example>
        [Required]
        public string LevelName { get; set; } = "Info";
    }
}
