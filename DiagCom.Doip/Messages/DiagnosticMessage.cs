using DiagCom.Doip.Exceptions;
using Utilities;

namespace DiagCom.Doip.Messages
{
    public class DiagnosticMessage : IMessage
    {
        private readonly Header _header;
        private const int _payloadLengthWithoutData = 4;
        private const int _addressLength = 2;

        public ushort SourceAddress { get; private set; }
        public ushort TargetAddress { get; private set; }
        public byte[] Data { get; set; }
        public int Length { get; private set; }

        public DiagnosticMessage(ushort sourceAddress, ushort targetAddress, byte serviceIdentifier, params byte[] parameters)
            : this(sourceAddress, targetAddress, Concat(serviceIdentifier, parameters))
        {
        }

        public DiagnosticMessage(ushort sourceAddress, ushort targetAddress, byte[] data)
        {
            SourceAddress = sourceAddress;
            TargetAddress = targetAddress;
            Data = data;

            _header = new Header(DoIpCommon.DoIpVersion, DoIpCommon.PayloadType.DiagnosticMessage, (uint)(_payloadLengthWithoutData + Data.Length));

            Length = DoIpCommon.HeaderLength + _payloadLengthWithoutData + Data.Length;
        }
        public DiagnosticMessage(byte[] bytes)
        {
            _header = new Header(bytes);

            if (bytes.Length < DoIpCommon.HeaderLength + _payloadLengthWithoutData)
            {
                throw new CommunicationException("Received message to short.");
            }


            if (_header.Version != DoIpCommon.DoIpVersion)
            {
                throw new CommunicationException("Failed to deserialize payload. Unexpected protocol version.");
            }

            if (_header.PayloadType != DoIpCommon.PayloadType.DiagnosticMessage)
            {
                throw new CommunicationException("Failed to deserialize payload. Unexpected payload type.");
            }

            int pos = DoIpCommon.HeaderLength;

            SourceAddress = bytes[pos..(pos + 2)].ToUShort();
            pos += 2;
            TargetAddress = bytes[pos..(pos + 2)].ToUShort();
            pos += 2;

            Data = new byte[_header.PayloadLength - _payloadLengthWithoutData];

            Array.Copy(bytes, pos, Data, 0, Data.Length);

            Length = pos + Data.Length;
        }

        public byte[] ToBytes()
        {
            byte[] request = new byte[Length];

            Array.Copy(_header.ToBytes(), 0, request, 0, DoIpCommon.HeaderLength);

            int pos = DoIpCommon.HeaderLength;

            var addresses = AddressesToBytes(SourceAddress, TargetAddress);
            Array.Copy(addresses, 0, request, pos, addresses.Length);
            pos += addresses.Length;
            Array.Copy(Data, 0, request, pos, Data.Length);

            return request;
        }

        private byte[] AddressesToBytes(ushort source, ushort target)
        {
            return source.ToBytes().Concat(target.ToBytes()).ToArray();
        }

        public byte[] ToPayloadBytes()
        {
            var request = new byte[_addressLength * 2 + Data.Length];

            Array.Copy(SourceAddress.ToBytes(), 0, request, 0, _addressLength);
            Array.Copy(TargetAddress.ToBytes(), 0, request, _addressLength, _addressLength);
            Array.Copy(Data, 0, request, _addressLength * 2, Data.Length);

            return request;
        }

        private static byte[] Concat(byte firstByte, byte[] otherBytes)
        {
            return (new[] { firstByte }).Concat(otherBytes).ToArray();
        }
    }
}
