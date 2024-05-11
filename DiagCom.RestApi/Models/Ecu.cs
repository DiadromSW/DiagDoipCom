using DiagCom.RestApi.Models.Dtcs;
using Utilities;

namespace DiagCom.RestApi.Models
{
    /// <summary>
    /// Payload ecu object
    /// </summary>
    public class Ecu
    {
        /// <summary>
        /// ECU physical address.
        /// Value represent 2bytes as string
        /// </summary>
        public string EcuAddress { get; set; }
        /// <summary>
        /// Short name of ECU.
        /// It is returned just if value is specified in request body
        /// </summary>
        public string Ecu_ShortName { get; set; }
        /// <summary>
        /// A flag that indicates if specified ecu is responding
        /// It is false in case that no negative or positive response is received.
        /// </summary>
        public bool Responding { get; set; }
        /// <summary>
        /// Detected Dtcs for current Ecu
        /// </summary>
        public List<Dtc> Dtcs { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="notTranslatedEcu"></param>
        public Ecu(Uds.Model.Ecu notTranslatedEcu)
        {
            Dtcs = notTranslatedEcu.Dtcs.ConvertAll(x => new Dtc(x));
            Responding = notTranslatedEcu.Responding;
            Ecu_ShortName = notTranslatedEcu.EcuShortName;
            EcuAddress = BitConverter.ToString(notTranslatedEcu.EcuAddress.ToBytes()).Replace("-", "");
        }
    }
}
