using Microsoft.Deployment.WindowsInstaller;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace CustomActions
{
    /// <summary>
    /// Class with actions performed during installation of DiagCom service.
    /// NOTE: Build this class after any changes in this class
    /// </summary>
    public class CustomActions
    {
        private const string _certFileName = "DiagComClient.pfx";
        private const string _certPassword = "pF7EN9p/X5ldO4UH+";


        [CustomAction]
        public static ActionResult CreateSelfSignedCertificate(Session session)
        {
            var progFile86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            if(string.IsNullOrEmpty(progFile86))
            {
                return ActionResult.NotExecuted;
            }
               
            string certDirectory = Path.Combine(progFile86, "DiagCom\\Certs\\");

            if (!Directory.Exists(certDirectory))
                Directory.CreateDirectory(certDirectory);

            string fullPath = Path.Combine(certDirectory, _certFileName);
            //If Cert exists in path delete file
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            //Add authority headers to avoid the need for trusting the source
            SubjectAlternativeNameBuilder sanBuilder = new SubjectAlternativeNameBuilder();
            sanBuilder.AddIpAddress(IPAddress.Loopback);
            sanBuilder.AddIpAddress(IPAddress.IPv6Loopback);
            sanBuilder.AddDnsName("localhost");

            //Create Request and certificate with persisted key
            var request = new CertificateRequest(new X500DistinguishedName("cn=DiagComClient"), RSA.Create(), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(sanBuilder.Build());
            var certificate = request.CreateSelfSigned(DateTime.Now, DateTimeOffset.Now.AddYears(100));
            X509Certificate2 certWithPersistedKey = new X509Certificate2(certificate.Export(X509ContentType.Pkcs12, _certPassword),
               _certPassword,
                X509KeyStorageFlags.PersistKeySet);

            //Create pfx file
            File.WriteAllBytes(fullPath, certificate.Export(X509ContentType.Pfx, _certPassword));

            //Trust the certificate if it is not already trusted

            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            DeleteCerts(store);
            store.Add(certWithPersistedKey);
            store.Close();

            return ActionResult.Success;
        }
        [CustomAction]
        public static ActionResult RemoveSelfSignedCertificate(Session session)
        {
            string certDirectory = Path.Combine(Environment.GetEnvironmentVariable("ProgramFiles(x86)"), "DiagCom\\Certs\\");

            if (!Directory.Exists(certDirectory))
                Directory.CreateDirectory(certDirectory);

            string fullPath = Path.Combine(certDirectory, _certFileName);
            //If Cert exists in path delete file
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            X509Store store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            DeleteCerts(store);
            store.Close();

            return ActionResult.Success;
        }

        private static void DeleteCerts(X509Store store)
        {
            var certificates = store.Certificates.Find(X509FindType.FindBySubjectName, "DiagComClient", false);
            // if cert exists in store delete
            if (certificates.Count > 0)
            {
                store.RemoveRange(certificates);
            }
        }



    }
}
