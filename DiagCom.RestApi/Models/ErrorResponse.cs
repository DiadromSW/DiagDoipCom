namespace DiagCom.RestApi.Models
{
    /// <summary>
    /// Represents an error that occured, along with a possible status, and a message explaining the error that occurred
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Http status code
        /// </summary>
        public int Status { get; set; } = 0;
        /// <summary>
        /// Explanation of the problem(s) that occured
        /// </summary>
        /// <example>Error on establishing a UDP connection, please check if vehicle is connected</example>
        public List<string> Message { get; set; } = new List<string>();

        /// <summary>
        /// Error code used as exception identifier
        /// </summary>
        public string ErrorCode { get; set; } = "";
    }
}
