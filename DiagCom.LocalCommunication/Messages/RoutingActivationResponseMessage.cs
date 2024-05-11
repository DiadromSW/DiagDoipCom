using DiagCom.Doip.Exceptions;
using DiagCom.Doip.Messages;
using Utilities;
using static DiagCom.Doip.Messages.DoIpCommon;

namespace DiagCom.LocalCommunication.Messages
{
    public class RoutingActivationResponseMessage
    {
        private const int PayloadLengthWithoutOem = 9;
        private const int PayloadLengthWithOem = 13;

        public const int MinLength = HeaderLength + PayloadLengthWithoutOem;
        public const int MaxLength = HeaderLength + PayloadLengthWithOem;

        public ushort TesterAddress { get; }
        public ushort DoipEntityAddress { get; }
        public RoutingActivationResponseCode ResponseCode { get; }
        public uint ReservedIso { get; }
        public uint? ReservedOem { get; } = null;

        public RoutingActivationResponseMessage(byte[] bytes)
        {
            if (bytes.Length < MinLength)
            {
                throw new CommunicationException("Received message to short.");
            }

            var header = new Header(bytes);

            if (header.Version != DoIpVersion)
            {
                throw new CommunicationException("Failed to deserialize payload. Unexpected protocol version.");
            }

            if (header.PayloadType != PayloadType.RoutingActivationResponse)
            {
                throw new CommunicationException("Failed to deserialize payload. Unexpected payload type.");
            }

            int pos = HeaderLength;

            TesterAddress = bytes[pos..(pos + 2)].ToUShort();
            pos += 2;

            DoipEntityAddress = bytes[pos..(pos + 2)].ToUShort();
            pos += 2;

            ResponseCode = (RoutingActivationResponseCode)bytes[pos];
            pos += 1;

            ReservedIso = bytes[pos..(pos + 4)].ToUInt();
            pos += 4;

            if (bytes.Length == MaxLength)
            {
                ReservedOem = bytes[pos..(pos + 4)].ToUInt();
            }
        }
    }
}
