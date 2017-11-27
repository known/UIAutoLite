using System.ComponentModel;

namespace UIAutoLite.Runner
{
    [ToolboxItem(false)]
    public partial class ErrorControl : ControlBase
    {
        public ErrorControl(string message)
        {
            InitializeComponent();
            labelMessage.Text = message;
        }
    }
}
