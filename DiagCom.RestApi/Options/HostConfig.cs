namespace DiagCom.RestApi.Options
{
    public class HostConfig : IHostConfig
    {

        public HostConfig()
        {
            BasePath = Environment.GetEnvironmentVariable("ProgramFiles(x86)");

        }
        public string CertFileName { get; set; } = "DiagComClient.pfx";

        public string BasePath { get; set; }
        public string Url { get; set; }
        public string CertPath { get; set; } = "DiagCom\\Certs\\";
        public string Password { get; set; } = "pF7EN9p/X5ldO4UH+";

        public string GetFullCertPath() => Path.Combine(BasePath, CertPath, CertFileName);
        public int GetPort()
        {
            string portString = Url.Split(":")[2];
            return Int32.Parse(portString);
        }

    }
}
