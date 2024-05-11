using System.Net;

namespace DiagCom.LocalCommunication
{
    public class DoipEntity
    {
        public IPAddress IpAddress { get; init; }
        public string Vin { get; init; }

        public override bool Equals(object other)
        {
            if (other is DoipEntity)
                return Equals((DoipEntity)other);
            return false;
        }

        public bool Equals(DoipEntity other)
        {
            if (other == null)
            {
                return false;
            }

            return IpAddress.Equals(other.IpAddress) && Vin.Equals(other.Vin);
        }

        public override int GetHashCode()
        {
            return IpAddress.GetHashCode();
        }

        public override string ToString()
        {
            return $"VIN: {Vin}, IP: {IpAddress}";
        }
    }
}
