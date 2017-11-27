using UIAutoLite.Logs;

namespace UIAutoLite
{
    public abstract class UICase
    {
        public string Title { get; set; }

        public abstract void Execute(ILogger log);

        public virtual string Validate()
        {
            return string.Empty;
        }
    }
}
