namespace DiagCom.Doip.Messages
{
    public interface IMessage
    {
        ushort SourceAddress { get; }
        ushort TargetAddress { get; }
        byte[] Data { get; set; }
        int Length { get; }
    }
}