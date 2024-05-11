namespace DiagCom.Commands.Coordination
{
    public interface IParsedResult
    {
        public bool ParsedOk { get; set; }
        public string Value { get; set; }
    }
}