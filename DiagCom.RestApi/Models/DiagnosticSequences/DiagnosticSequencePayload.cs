using Newtonsoft.Json.Linq;
using System.ComponentModel.DataAnnotations;

namespace DiagCom.RestApi.Models.DiagnosticSequences
{
    /// <summary>
    /// Payload object for RunDiagnosticSequence 
    /// </summary>
    public class DiagnosticSequencePayload
    {
        /// <summary>
        /// Vehicle Identification Number
        /// </summary>
        /// <example>LVX00000000000001</example>
        [Required]
        public string Vin { get; set; }

        /// <summary>
        /// Indicates sequence to run on vehicle 
        /// </summary>
        /// <example>
        ///{
        ///   "identifier":"d4cd0ad1-5849-4370-9699-f48683d3b623",
        ///   "sequence":[
        ///      {
        ///         "address":"1401",
        ///         "description":"Read Data By Identifier Usage Mode",
        ///         "service":"22",
        ///         "payload":"DD0A",
        ///         "parsing_data":{
        ///           "identifier":"DD0A",
        ///           "description":"Usage Mode",
        ///           "parameters":[
        ///               {
        ///                  "identifier":"DD0A_1",
        ///                  "data_type":"Unsigned",
        ///                  "description":"",
        ///                  "max":"",
        ///                  "min":"",
        ///                  "offset_bits":"0",
        ///                  "total_bits":"8"
        ///               }
        ///            ]
        ///         }
        ///      }
        ///   ]
        ///}
        /// </example>
        [Required]
        public DiagnosticSequence DiagnosticSequence { get; set; }

    }
}