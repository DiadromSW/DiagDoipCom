using DiagCom.Uds.Operations;

namespace DiagCom.Uds
{
    /// <summary>
    /// A running task that starts pauses and stops tester present functional request 
    /// </summary>
    public interface ITesterPresentController : IDisposable
    {
        /// <summary>
        /// If diagOp has service  10 or 11(ecu reset) a tester present start or stop will be required
        /// </summary>
        /// <param name="diagOp"></param>
        /// <returns></returns>
        Task CheckTesterPresent(IDiagnosticOperation diagOp);
    }
}