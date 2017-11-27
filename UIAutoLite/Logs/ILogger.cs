namespace UIAutoLite.Logs
{
    public interface ILogger
    {
        string Record { get; }
        void BeginRecord();
        void EndRecord();
        void Write(string message);
        void Write(string format, params object[] args);
        void Write(LogMode mode, string message);
        void Write(LogMode mode, string format, params object[] args);
    }
}
