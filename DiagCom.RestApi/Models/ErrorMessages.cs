namespace DiagCom.RestApi.Models
{
    public static class ErrorMessages
    {
        public const string InvalidVinLength = "Invalid Length On Vin Length Should Be 17 characters long.";
        public const string InvalidCurrentVin = "Invalid current Vin format. Just 00000000000000000 is allowed to override.";
        public const string EmptyEcus = "Your List is empty, Please add some ecus";
        public const string InvalidEcuFormat = "One or more ecus is not a valid ecu, Please check name";
        public const string InvalidCorrelationId = @"Correlation id can't be empty or ""string"", Please check id ";
        public const string InvalidLogLevel = "Incorrect log level please use either Trace, Debug, Info, Warn, Error or Fatal";
        public const string MustBeDigits = "Incorrect value, value must be digits";
        public const string MustBeGuidId = "Incorrect value, value must be a guid Identifier";
        public const string CannotBeEmpty = "Field can not be empty";
        public const string DirectoryNotFound = "No logs were found for provided VIN, please make sure logging have been started or that the VIN is correct";
        public const string NoErrorLogsFound = "No error logs were found";
        public const string IsValidBool = "Must be true or false";

    }
}
