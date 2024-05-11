namespace DiagCom.RestApi.Options
{

    /// <summary>
    /// Host Config object 
    /// </summary>
    public interface IHostConfig
    {
        /// <summary>
        /// Cors certificate name
        /// </summary>
        string CertFileName { get; set; }

        /// <summary>
        /// Path where the certificate was generated
        /// </summary>
        string BasePath { get; set; }
        /// <summary>
        /// Path where the certificate was generated
        /// </summary>
        string CertPath { get; set; }     

        /// <summary>
        /// Local generated certificate
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Url where the service is hosted
        /// </summary>
        static string Url { get; set; }

        /// <summary>
        /// Full certificate path
        /// </summary>
        /// <returns></returns>
        string GetFullCertPath();

        /// <summary>
        /// Port for the hosted service
        /// </summary>
        /// <returns></returns>
        int GetPort();
    }
}