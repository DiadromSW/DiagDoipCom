using Utilities;

namespace DiagCom.Uds.Model.Dtcs
{
    public class DtcStatusAndRecord
    {
        private const int _statusPosition = 3;
        // high byte middle byte low byte dtc status 
        public byte[] Dtc { get; }
        public DtcStatus DtcStatus { get; }
        public DtcStatusAndRecord(byte[] bytes)
        {
            Dtc = bytes.Take(3).ToArray();
            DtcStatus = new DtcStatus((new byte[] { bytes[_statusPosition] }).ToBits());
        }

        public override bool Equals(object other)
        {
            return Equals(other as DtcStatusAndRecord);
        }

        public bool Equals(DtcStatusAndRecord other)
        {
            if (Dtc.Length != other.Dtc.Length)
            {
                return false;
            }

            for (var i = 0; i < Dtc.Length; i++)
            {
                if (Dtc[i] != other.Dtc[i])
                {
                    return false;
                }
            }

            return DtcStatus.Equals(other.DtcStatus);
        }

        public override int GetHashCode()
        {
            return 1;
        }
    }
}

