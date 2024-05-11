using DiagCom.Doip.Responses;

namespace DiagCom.Uds.Model
{
    public class ReadBatteryVoltageResponse : IResponse
    {
        readonly int _voltage;
        public ReadBatteryVoltageResponse(uint voltage)
        {
            _voltage = (int)voltage;
        }

        // Voltage in millivolts.
        public int Voltage
        {
            get
            {
                return _voltage;
            }
        }
    }
}
