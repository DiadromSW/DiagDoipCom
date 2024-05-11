namespace DiagCom.Doip.Messages
{
    public class DoIpCommon
    {
        public const int DoIpPortNumber = 13400;

        public const int HeaderLength = 8; // 1 + 1 + 2 + 4

        public const byte DoIpVersion = 0x02;
        public const byte DoIpIdentificationVersion = 0x02; // 0xFF: default value for vehicle identification request messages

        public const int VinLength = 17;

        public const int IdLength = 6;

        public enum PayloadType : ushort
        {
            GenericNack = 0x0000,
            VehicleIdentificationRequest = 0x0001,
            VehicleIdentificationRequestWithEid = 0x0002,
            VehicleIdentificationRequestWithVin = 0x0003,
            VehicleAnnouncementMessage = 0x0004,
            RoutingActivationRequest = 0x0005,
            RoutingActivationResponse = 0x0006,
            DiagnosticMessage = 0x8001,
            DiagnosticAck = 0x8002,
            DiagnosticNack = 0x8003,
        }

        public enum RoutingActivationType : byte
        {
            Default = 0x00,
            WwhObd = 0x01,
            CentralSecurity = 0xE0,
            RAREQC_LOCAL = 0xE2, // Activate routing in local mode
            RAREQC_REMOTE = 0xE3, // Activate routing in remote mode

        }

        public enum RoutingActivationResponseCode : byte
        {
            UnknownSourceAddress = 0x00,
            NoSocketAvailable = 0x01,
            DifferentSourceAddress = 0x02,
            SocketAlreadyActive = 0x03,
            MissingAuthentication = 0x04,
            RejectedConfirmation = 0x05,
            UnsupportedActivationType = 0x06,
            SuccessfullyActivated = 0x10,
            RARESPC_BUSY = 0xE1, // Another client (internal or external) is currently using the vehciel diagnostic system
            RARESPC_WRONG_MODE = 0xE2  // The activation type does not match the current DoIP mode of the vehicle (Local; Remote)
        }

        public enum SocketStates : byte
        {
            Ss_Unconnected = 0x00,
            Ss_Connecting = 0x01,
            Ss_Connected = 0x02,
            Ss_Listening = 0x03

        }
    }
}
