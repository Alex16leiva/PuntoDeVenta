namespace CrossCutting.Logging
{
    public interface ILoggerFactory
    {
        ILogger Create();
    }
}
