using Utilities;

namespace DiagCom.RestApi.Validators.Static
{
    class StaticValidator
    {

        public static bool ValidateLogLevel(string level)
        {
            switch (level)
            {
                case "Trace":
                    return true;
                case "Debug":
                    return true;
                case "Info":
                    return true;
                case "Warn":
                    return true;
                case "Error":
                    return true;
                case "Fatal":
                    return true;
                default:
                    return false;
            }
        }
        public static bool ValidateVinNumber(string vin)
        {
            if (vin == null || vin.Length != 17)
            {
                return false;
            }
            else
                return true;

        }

        public static bool ValidEcusFormat(string[] ecus)
        {
            foreach (var Ecu in ecus)
            {
                if (!ValidEcuAddressFormat(Ecu))
                    return false;
            }
            return true;
        }
        public static bool ValidEcuAddressFormat(string ecuAddress)
        {
            if (ecuAddress.Length != 4 || !ecuAddress.CheckValidHexFormat())
            {
                return false;
            }
            return true;
        }
        public static bool ValidateCurrentVinNumber(string vin)
        {
            if (vin == null || vin.Length != 17 || !vin.All(x => x == '0'))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static bool DirectoryPathExist(string directory)
        {
            if (!Directory.Exists(directory))
                return false;

            return true;

        }
    }
}
