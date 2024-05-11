using DiagCom.Doip.Responses;
using DiagCom.Uds.Model;

namespace DiagCom.Uds
{
    public class ReadDtcResponse : IResponse
    {
        public ReadDtcResponse(Ecu ecu)
        {
            Ecu = ecu;
        }
        public Ecu Ecu { get; internal set; }
    }
}