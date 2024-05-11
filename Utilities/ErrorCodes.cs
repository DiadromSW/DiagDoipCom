namespace Utilities
{
    public enum ErrorCodes
    {
        SwdlFailed,
        EcuNegativeResponse,
        Logging,
        MissingLogs,
        MissingLogRule,
        ParserMiddleware,
        VehicleCommunication,
        VbfFormat,
        VbfFile,
        SwdlOngoing,
        SwdlExclusivity,
        VinLength,
        //From J2534Exception
        J2534Proxy,
        DeviceNotConnected,
        DeviceInUse,
        DeviceNotOpen,
        OpenFailed
    }
}
