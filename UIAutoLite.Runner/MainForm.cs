using System;
using System.Windows.Forms;

namespace UIAutoLite.Runner
{
    public partial class MainForm : FormBase
    {
        private ControlBase control;
        private StatusForm frmStatus;
        private DateTime startTime;

        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            tsslStatus.Text = string.Empty;
            tsslVersion.Text = string.Format("V{0}", Application.ProductVersion);
            treeMenu.ExpandAll();
        }

        private void TreeMenu_AfterSelect(object sender, TreeViewEventArgs e)
        {
            tsslStatus.Text = string.Empty;
            lbLog.Items.Clear();
            panelContent.Controls.Clear();

            if (e.Node.Tag == null)
            {
                control = new ErrorControl(string.Empty);
            }
            else
            {
                var typeName = string.Format("AdiacTool.{0}", e.Node.Tag);
                var type = Type.GetType(typeName);
                if (type == null)
                {
                    control = new ErrorControl(string.Format("找不到{0}类型控件！", typeName));
                }
                else
                {
                    control = Activator.CreateInstance(type) as ControlBase;
                    if (control == null)
                        control = new ErrorControl(string.Format("{0}类型控件必须继承ControlBase！", typeName));
                }
            }

            control.MainForm = this;
            control.Dock = DockStyle.Fill;
            panelContent.Height = control.Height;
            panelContent.Controls.Add(control);
        }

        public void Start()
        {
            Start(null);
        }

        public void Start(object argument)
        {
            startTime = DateTime.Now;
            Hide();
            lbLog.Items.Clear();
            frmStatus = new StatusForm(control, lbLog);
            frmStatus.Start(argument);
        }

        public void End()
        {
            if (control != null)
                control.EndFormWork();
            Show();
            BringToFront();
            var span = DateTime.Now - startTime;
            tsslStatus.Text = string.Format("运行结束，共耗时{0}小时{1}分{2}秒{3}毫秒。", span.Hours, span.Minutes, span.Seconds, span.Milliseconds);
        }
    }
}
