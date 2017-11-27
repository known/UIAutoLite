using System;
using System.ComponentModel;
using System.Windows.Forms;
using UIAutoLite.Logs;

namespace UIAutoLite.Runner
{
    [ToolboxItem(false)]
    public class ControlBase : UserControl
    {
        public ControlBase()
        {
            CheckForIllegalCrossThreadCalls = false;
        }

        public MainForm MainForm { get; set; }

        public void RunFormWork()
        {
            RunFormWork(null);
        }

        public void RunFormWork(object argument)
        {
            if (MainForm != null)
            {
                MainForm.Start(argument);
            }
        }

        public virtual void DoFormWork(ILogger log, DoWorkEventArgs e)
        {
            var uic = e.Argument as UICase;
            if (uic == null)
                return;

            try
            {
                uic.Execute(log);
            }
            catch (Exception ex)
            {
                log.Write(LogMode.OnlyListBox, "发生异常：{0}", ex.ToString());
            }

            EndFormWork(log, uic);
        }

        public virtual void EndFormWork() { }
        public virtual void EndFormWork(ILogger log, UICase uic) { }

        protected void ShowMessage(string message)
        {
            MessageBox.Show(message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected void ExecuteAsync(object argument, Action<DoWorkEventArgs> doAction, Action<RunWorkerCompletedEventArgs> completeAction)
        {
            var backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (o, e) =>
            {
                doAction?.Invoke(e);
            };
            backgroundWorker.RunWorkerCompleted += (o, e) =>
            {
                completeAction?.Invoke(e);
            };
            backgroundWorker.RunWorkerAsync(argument);
        }
    }
}
