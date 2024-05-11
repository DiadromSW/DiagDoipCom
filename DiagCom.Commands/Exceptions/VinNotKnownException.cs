namespace DiagCom.Commands.Exceptions
{
    public class VinNotKnownException : Exception
    {
        public VinNotKnownException(string vin, string vinOverride)
            : base(CreateMessage(vin, vinOverride))
        {
        }

        private static string CreateMessage(string vin, string vinOverride)
        {
            var vinText = "";

            if (!string.IsNullOrEmpty(vin))
            {
                vinText = $" with VIN \"{vin}\"";
                if (vinOverride != vin)
                {
                    vinText += $" or VIN \"{vinOverride}\"";
                }
            }

            return $"Vehicle{vinText} not known.";
        }
    }
}
