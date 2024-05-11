namespace DiagCom.RestApi.Models.Dtcs
{
    /// <summary>
    /// Input for GetDtcStatus
    /// </summary>
    public class GetDtcExtendedDataPayload
    {
        /// <summary>
        /// Vehicle Identification Number
        /// </summary>
        /// <example>LVX00000000000001</example>
        public string Vin { get; set; }
        /// <summary>
        /// Diagnostic Trouble Code Identifier
        /// </summary>
        /// <example>ABCDEF</example>
        public string DtcId { get; set; }
        /// <summary>
        /// Ecu Adress
        /// </summary>
        /// <example>1A01</example>
        public string EcuAddress { get; set; }
    }
}
