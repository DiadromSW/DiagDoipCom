namespace DiagCom.Doip.Responses
{
    public class ErrorResponse : IResponse
    {
        readonly Exception _ex;

        public ErrorResponse(Exception ex)
        {
            _ex = ex;
        }
        public Exception Ex
        {
            get
            {
                return _ex;
            }
        }
    }
}