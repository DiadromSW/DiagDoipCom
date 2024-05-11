using DiagCom.Doip.Messages;

namespace DiagCom.LocalCommunication
{
    public interface IMessageParser
    {
        IMessage? BytesToMessage(byte[] bytes);
    }
}