using System;
using System.Text;
using System.Windows.Forms;

namespace UIAutoLite.Logs
{
    public class Logger : ILogger
    {
        private Label label;
        private ListBox listBox;
        private StringBuilder sb;
        private bool isRecord = false;

        public Logger(Label label, ListBox listBox)
        {
            this.label = label;
            this.listBox = listBox;
        }

        public string Record { get; private set; }

        public void BeginRecord()
        {
            isRecord = true;
            sb = new StringBuilder();
        }

        public void EndRecord()
        {
            isRecord = false;
            Record = sb.ToString();
        }

        public void Write(string message)
        {
            Write(LogMode.Both, message);
        }

        public void Write(string format, params object[] args)
        {
            Write(LogMode.Both, format, args);
        }

        public void Write(LogMode mode, string message)
        {
            switch (mode)
            {
                case LogMode.Both:
                    WriteStatus(message);
                    WriteListBox(message);
                    break;
                case LogMode.OnlyStatus:
                    WriteStatus(message);
                    break;
                case LogMode.OnlyListBox:
                    WriteListBox(message);
                    break;
                default:
                    break;
            }
        }

        public void Write(LogMode mode, string format, params object[] args)
        {
            var message = string.Format(format, args);
            Write(mode, message);
        }

        private void WriteStatus(string message)
        {
            if (label != null)
            {
                label.Text = message;
            }
        }

        private void WriteListBox(string message)
        {
            if (listBox != null)
            {
                var index = 0;
                var messages = message.Split(Environment.NewLine.ToCharArray());
                foreach (var item in messages)
                {
                    var str = item;
                    if (!string.IsNullOrEmpty(str))
                    {
                        if ((index++) == 0)
                            str = string.Format("{0:HH:mm:ss} {1}", DateTime.Now, str);
                        listBox.Items.Add(str);
                    }
                }
            }
            if (isRecord)
            {
                sb.AppendLine(message);
            }
        }
    }
}
