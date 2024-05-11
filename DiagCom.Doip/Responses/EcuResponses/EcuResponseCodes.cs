namespace DiagCom.Doip.Responses.EcuResponses
{
    public enum EcuResponseCodes : byte
    {
        /// <summary>
        /// This response code indicates that the negative response could not be parsed.
        /// </summary>
        Undefined,

        /// <summary>
        /// This response code indicates that the requested action has been rejected by the server.
        /// </summary>
        GeneralReject = 0x10,

        /// <summary>
        /// This response code indicates that the requested action will not be taken because the
        /// server does not support the requested service.
        /// </summary>
        ServiceNotSupported = 0x11,

        /// <summary>
        /// The sub-function parameter in the request message is not supported.
        /// </summary>
        SubFunctionNotSupported = 0x12,

        /// <summary>
        /// The length of the message is wrong.
        /// </summary>
        IncorrectMessageLengthOrInvalidFormat = 0x13,

        /// <summary>
        /// The length of the response is wrong.
        /// </summary>
        ResponseTooLong = 0x14,

        /// <summary>
        /// The server is temporarily too busy to perform the requested operation.
        /// In this circumstance, the client shall perform repetition of the
        /// “identical request message” or “another request message”. The repetition of the
        /// request shall be delayed by a time specified in the respective implementation documents.
        /// </summary>
        BusyRepeatRequest = 0x21,

        /// <summary>
        /// The requested action will not be taken because the server prerequisite conditions are not met.
        /// </summary>
        ConditionsNotCorrect = 0x22,

        /// <summary>
        /// The requested action will not be taken because the server expects a different sequence of request messages
        /// or message to that sent by the client.
        /// This may occur when sequence-sensitive requests are issued in the wrong order.
        /// </summary>
        RequestSequenceError = 0x24,

        /// <summary>
        /// The server has received the request but the requested action could not be performed by the server, 
        /// as a subnet component which is necessary to supply the requested information 
        /// did not respond within the specified time.
        /// </summary>
        NoResponseFromSubnetComponent = 0x25,

        /// <summary>
        /// The requested action will not be taken because a failure condition,
        /// identified by a DTC (with at least one DTC status bit for TestFailed,
        /// Pending, Confirmed or TestFailedSinceLastClear set to 1), has occurred and that
        /// this failure condition prevents the server from performing the requested action.
        /// </summary>
        FailurePreventsExecutionOfRequestedAction = 0x26,

        /// <summary>
        /// The requested action will not be taken because the server has detected that the request message 
        /// contains a parameter which attempts to substitute a value beyond its range of 
        /// authority (e.g. attempting to substitute a data byte of 111 when the data is only defined to 100),
        /// or which attempts to access a dataIdentifier/routineIdentifer that is not supported or not supported in active session.
        /// </summary>
        RequestOutOfRange = 0x31,

        /// <summary>
        /// The server is not in an unlocked state.
        /// </summary>
        SecurityAccessDenied = 0x33,

        /// <summary>
        /// The server has not given security access because
        /// the key sent by the client did not match with the key in the server’s memory.
        /// This counts as an attempt to gain security.
        /// </summary>
        InvalidKey = 0x35,

        /// <summary>
        /// The requested action will not be taken because the
        /// client has unsuccessfully attempted to gain security access more times than the
        /// server’s security strategy will allow.
        /// </summary>
        ExceedNumberOfAttempts = 0x36,

        /// <summary>
        /// The requested action will not be taken because the
        /// client’s latest attempt to gain security access was initiated before the server’s
        /// required timeout period had elapsed.
        /// </summary>
        RequiredTimeDelayNotExpired = 0x37,

        /// <summary>
        /// An attempt to upload/download to a server’s
        /// memory cannot be accomplished due to fault conditions.
        /// </summary>
        UploadDownloadNotAccepted = 0x70,

        /// <summary>
        /// data transfer operation was halted due to a fault.
        /// The active transferData sequence shall be aborted.
        /// </summary>
        TransferDataSuspended = 0x71,

        /// <summary>
        /// The server detected an error when erasing or programming a memory location 
        /// in the permanent memory device (e.g. Flash Memory).
        /// </summary>
        GeneralProgrammingFailure = 0x72,

        /// <summary>
        /// The server detected an error in the sequence of blockSequenceCounter values.
        /// </summary>
        WrongBlockSequenceCounter = 0x73,

        /// <summary>
        /// The request message was received correctly, and that all parameters in the request 
        /// message were valid, but the action to be performed is not yet completed and 
        /// the server is not yet ready to receive another request.
        /// </summary>
        ResponsePending = 0x78,

        /// <summary>
        /// The requested action will not be taken because the
        /// server does not support the requested sub-function in the session currently active.
        /// </summary>
        SubFunctionNotSupportedInActiveSession = 0x7E,

        /// <summary>
        /// The requested action will not be taken because the
        /// server does not support the requested service in the session currently active.
        /// </summary>
        ServiceNotSupportedInActiveSession = 0x7F,

        /// <summary>
        /// Engine revelatons per minit is too high. 
        /// </summary>
        RpmTooHigh = 0x81,

        /// <summary>
        /// Engine revelatons per minit is too low. 
        /// </summary>
        RpmTooLow = 0x82,

        /// <summary>
        /// Engine is running. 
        /// </summary>
        EngineIsRunning = 0x83,

        /// <summary>
        /// Engine is not running. 
        /// </summary>
        EngineIsNotRunning = 0x84,

        /// <summary>
        /// Engine has not been running for the required time. 
        /// </summary>
        EngineRunTimeTooLow = 0x85,

        /// <summary>
        /// Temperature is too high. 
        /// </summary>
        TemperatureTooHigh = 0x86,

        /// <summary>
        /// Temperature is too low. 
        /// </summary>
        TemperatureTooLow = 0x87,

        /// <summary>
        /// Vehicle speed is too high. 
        /// </summary>
        VehicleSpeedTooHigh = 0x88,

        /// <summary>
        /// Vehicle speed is too low. 
        /// </summary>
        VehicleSpeedTooLow = 0x89,

        /// <summary>
        /// Throttle/Pedal applied. 
        /// </summary>
        ThrottlePedalTooHigh = 0x8A,

        /// <summary>
        /// Throttle/Pedal not applied. 
        /// </summary>
        ThrottlePedalTooLow = 0x8B,

        /// <summary>
        /// Transmission in wrong position, not in N. 
        /// </summary>
        TransmissionRangeNotInNeutral = 0x8C,

        /// <summary>
        /// Transmission in wrong position, not in R/D mode. 
        /// </summary>
        TransmissionRangeNotInGear = 0x8D,

        /// <summary>
        /// Brakes not applied. 
        /// </summary>
        BrakeSwitchNotClosed = 0x8F,

        /// <summary>
        /// Shift lever not in P position. 
        /// </summary>
        ShiftLeverNotInPark = 0x90,

        /// <summary>
        /// Transmission locked in gear. 
        /// </summary>
        TorqueConverterClutchLocked = 0x91,

        /// <summary>
        /// System voltage not in range, too high voltage. 
        /// </summary>
        VoltageTooHigh = 0x92,

        /// <summary>
        /// System voltage not in range, too low voltage. 
        /// </summary>
        VoltageTooLow = 0x93
    }
}
