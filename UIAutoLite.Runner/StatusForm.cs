using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using UIAutoLite.Logs;

namespace UIAutoLite.Runner
{
    public partial class StatusForm : FormBase
    {
        private BackgroundWorker backgroundWorker;
        private ControlBase control;
        private ListBox lbLog;

        public StatusForm(ControlBase control, ListBox lbLog)
        {
            this.control = control;
            this.lbLog = lbLog;

            InitializeComponent();
            lblMessage.Text = string.Empty;

            CheckForIllegalCrossThreadCalls = false;
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += BackgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
        }

        private void StatusForm_Load(object sender, EventArgs e)
        {
            var x = SystemInformation.WorkingArea.Width - Width;
            var y = SystemInformation.WorkingArea.Height - Height;
            Location = new Point(x, y);
            TopMost = true;
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (control != null)
            {
                var log = new Logger(lblMessage, lbLog);
                control.DoFormWork(log, e);
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            EndWork();
        }

        public void Start(object argument)
        {
            Show();

            if (argument == null)
                backgroundWorker.RunWorkerAsync();
            else
                backgroundWorker.RunWorkerAsync(argument);
        }

        private void EndWork()
        {
            Hide();

            if (control != null && control.MainForm != null)
            {
                control.MainForm.End();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            backgroundWorker.CancelAsync();
            EndWork();
        }
    }
}
