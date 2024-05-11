using DiagCom.Uds.Model.Dtcs;

namespace DiagCom.Uds.Model
{
    public class Ecu
    {
        public List<DtcStatusAndRecord> Dtcs { get; set; } = new();
        public bool Responding { get; set; }
        public string EcuShortName { get; set; } //not sure if this is correct should be CEM
        public ushort EcuAddress { get; set; }

        public List<string> Error { get; set; } = new List<string>();

        public Ecu(ushort ecuAddress)
        {
            EcuAddress = ecuAddress;
            Responding = false;
        }

        public void GetDtcs(byte[] resultBytes)
        {
            try
            {
                var responseType = resultBytes[1];
                var readDtcBytes = resultBytes.Skip(2).ToArray();
                if (responseType == 0x02)
                {
                    for (var i = 1; i < readDtcBytes.Length - 3; i += 4)
                        Dtcs.Add(new DtcStatusAndRecord(readDtcBytes.Skip(i).Take(4).ToArray()));
                }
            }
            catch (Exception ex)
            {
                Error.Add($"Error on parsing dtc:{ex.Message}");
            }
        }
    }
}
