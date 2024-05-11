using System.ComponentModel.DataAnnotations;

namespace DiagCom.RestApi.Models.Logg
{
    /// <summary>
    /// Input for start/stop logging.
    /// </summary>
    public class LogPayload
    {
        /// <summary>
        /// Vehicle Identification Number
        /// </summary>
        /// <example>LVX00000000000001</example>
        public string Vin { get; set; }

        /// <summary>
        /// Parameter to either start or stop logging with values ("Start","Stop")
        /// </summary>
        /// <example>Start</example>
        [Required]
        public string Activity { get; set; }


        /// <summary>
        /// Optional parameter (default Info) to set different LogLevel. The LogLevel avaible are: "Trace", "Debug", "Info", "Warn", "Error" and "Fatal"
        /// </summary>
        /// <example>Info</example>
        public string LogLevels { get; set; } = "Info";



    }
}
