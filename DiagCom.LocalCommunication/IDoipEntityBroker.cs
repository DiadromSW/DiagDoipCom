using DiagCom.Commands.Coordination;

namespace DiagCom.LocalCommunication
{
    public interface IDoipEntityBroker : IVinBroker
    {
        DoipEntity DoipEntity { get; }
        bool IsFoundOnUdp { get; set; }
        bool IsPresent { get; }
        bool IsTcpConnected { get; }

        void Update(DoipEntity doipEntity);
    }
}