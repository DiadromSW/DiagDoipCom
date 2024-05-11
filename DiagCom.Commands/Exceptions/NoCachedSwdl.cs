namespace DiagCom.Commands.Exceptions
{
    [Serializable]
    public class NoCachedSwdl : Exception
    {
        public NoCachedSwdl(string message) : base(message)
        {

        }
    }
}
