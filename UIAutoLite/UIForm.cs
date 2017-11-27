using System;
using System.Windows.Automation;
using UIAutoLite.Logs;

namespace UIAutoLite
{
    public class UIForm : UIElement
    {
        private UIForm(ILogger log, AutomationElement element) : base(log, element) { }

        public static UIForm FindFormByAi(ILogger log, string control)
        {
            var winHandle = (IntPtr)AutomationElement.RootElement.Current.NativeWindowHandle;
            var element = ElementHelper.FindElement(winHandle, control);
            return new UIForm(log, element);
        }

        public static UIForm FindFormByCondition(ILogger log, Condition condition)
        {
            var element = ElementHelper.FindElement(AutomationElement.RootElement, TreeScope.Children, condition);
            return new UIForm(log, element);
        }

        public static UIForm FindFormById(ILogger log, string id)
        {
            var condition = new PropertyCondition(AutomationElement.AutomationIdProperty, id);
            return FindFormByCondition(log, condition);
        }

        public static UIForm FindFormByName(ILogger log, string name)
        {
            var condition = new PropertyCondition(AutomationElement.NameProperty, name);
            return FindFormByCondition(log, condition);
        }
    }
}
