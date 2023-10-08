namespace Laba5
{
    public class Provider : ILoggerProvider
    {
        string path;

        public Provider(string path)
        {
            this.path = path;
        }
        public ILogger CreateLogger(string name)
        {
            return new Filelogger(path);
        }

        public void Dispose() { }
    }
}
