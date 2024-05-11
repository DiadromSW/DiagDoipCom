namespace DiagCom.RestApi.Models.Dtcs
{
    /// <summary>
    /// Diagnostic fault codes status
    /// </summary>
    public class DtcStatusViewResponse
    {
        /// <summary>
        /// Diagnostic fauld code Identifier
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Diagnostic fauld code name
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Diagnostic fauld code value
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="id"></param>
        public DtcStatusViewResponse(string name, string value, string id)
        {
            Name = name;
            Value = value;
            Id = id;
        }
    }
}
