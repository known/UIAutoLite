using UIAutoLite.Logs;

namespace UIAutoLite.TestCase
{
    public class Win10Calculator : UICase
    {
        public override void Execute(ILogger log)
        {
            var form = UIForm.FindFormByAi(log, "[CLASS:Windows.UI.Core.CoreWindow; INSTANCE:1]");
            form.FindByAi("[CLASS:ApplicationFrameInputSinkWindow; INSTANCE:1]").Click();
        }
    }
}
