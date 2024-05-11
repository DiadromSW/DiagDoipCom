namespace DiagCom.RestApi.Models.Logg
{
    /// <summary>
    /// Parameter to either start or stop logging with values ("Start","Stop")
    /// </summary>
    /// <example>Start</example>
    public enum LogOption
    {
        Start,
        Stop
    }

    /// <summary>
    /// Optional parameter to set different LogLevel. The LogLevel avaible are: Trace, Debug, Info, Warn, Error and Fatal
    /// </summary>
    /// <example>Info</example>
    public enum LogLevels
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error,
        Fatal
    }
}
