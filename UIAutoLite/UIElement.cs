using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Automation;
using UIAutoLite.Logs;

namespace UIAutoLite
{
    public class UIElement
    {
        private ILogger log;
        private AutomationElement element;

        protected UIElement(ILogger log, AutomationElement element)
        {
            this.log = log;
            this.element = element;
        }

        public bool IsExists
        {
            get { return element != null; }
        }

        public string Id
        {
            get { return IsExists ? element.Current.AutomationId : "未识别控件"; }
        }

        public string Name
        {
            get { return IsExists ? element.Current.Name : "未识别控件"; }
        }

        public IntPtr Handle
        {
            get { return IsExists ? (IntPtr)element.Current.NativeWindowHandle : IntPtr.Zero; }
        }

        public ControlType ControlType
        {
            get { return IsExists ? element.Current.ControlType : ControlType.Custom; }
        }

        public bool CheckDialog(string message)
        {
            return CheckDialog(message, null);
        }

        public string GetDialogText(int? timeout)
        {
            if (!IsExists)
                return string.Empty;

            var info = string.Empty;
            var condition = new PropertyCondition(AutomationElement.ClassNameProperty, "#32770");
            var dialog = FindByCondition(condition, timeout);
            if (dialog.IsExists)
            {
                var text = dialog.FindById("65535");
                if (text.IsExists)
                    info = text.Name;

                var button = dialog.FindById("2");
                if (button.IsExists)
                {
                    button.Click(100, false);
                    log.Write("--关闭[{0}]对话框！", info);
                }
            }

            return info;
        }

        public bool CheckDialog(string message, int? timeout)
        {
            var info = GetDialogText(timeout);
            return info == message;
        }

        public UIElement FindByCondition(Condition condition)
        {
            return FindByCondition(condition, null);
        }

        public UIElement FindByCondition(Condition condition, int? timeout)
        {
            var element = ElementHelper.FindElement(this.element, TreeScope.Descendants, condition, timeout);
            return new UIElement(log, element);
        }

        public UIElement FindByAi(string control)
        {
            var element = ElementHelper.FindElement(Handle, control);
            return new UIElement(log, element);
        }

        public UIElement FindById(string id)
        {
            var condition = new PropertyCondition(AutomationElement.AutomationIdProperty, id);
            return FindByCondition(condition);
        }

        public UIElement FindByName(string name)
        {
            var condition = new PropertyCondition(AutomationElement.NameProperty, name);
            return FindByCondition(condition);
        }

        public UIElement FindByClass(string @class)
        {
            var condition = new PropertyCondition(AutomationElement.ClassNameProperty, @class);
            return FindByCondition(condition);
        }

        public UIElement FindByCi(string @class, int instance)
        {
            var classCondition = new PropertyCondition(AutomationElement.ClassNameProperty, @class);
            var instanceCondition = new PropertyCondition(AutomationElement.ClassNameProperty, @class);
            var condition = new AndCondition(classCondition, instanceCondition);
            return FindByCondition(condition);
        }

        public UIElement FindByType(ControlType type)
        {
            var condition = new PropertyCondition(AutomationElement.ControlTypeProperty, type);
            return FindByCondition(condition);
        }

        public List<UIElement> FindAllChildren()
        {
            if (!IsExists)
                return null;

            var controls = new List<UIElement>();
            var items = element.FindAll(TreeScope.Children, Condition.TrueCondition);
            foreach (AutomationElement item in items)
            {
                controls.Add(new UIElement(log, item));
            }
            return controls;
        }

        public List<UIElement> FindAllByType(ControlType type)
        {
            if (!IsExists)
                return null;

            var controls = new List<UIElement>();
            var items = element.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, type));
            foreach (AutomationElement item in items)
            {
                controls.Add(new UIElement(log, item));
            }
            return controls;
        }

        public void SetFocus()
        {
            if (!IsExists)
                return;

            var name = Name;
            try
            {
                element.SetFocus();
            }
            catch
            {
                log.Write("--不能获取{0}焦点。", name);
            }
        }

        public void TypeEnter()
        {
            TypeEnter(null);
        }

        public void TypeEnter(int? wait)
        {
            if (!IsExists)
                return;

            var name = Name;
            var sendKeys = new SendKeys();
            sendKeys.Sendkeys(element, sendKeys.Enter);
            log.Write("--{0}输入回车。", name);
            if (wait.HasValue)
                Thread.Sleep(wait.Value);
        }

        public void TypeValue(string value)
        {
            TypeValue(value, null);
        }

        public void TypeValue(string value, int? wait)
        {
            if (!CheckValue(value))
                return;

            var name = Name;
            var sendKeys = new SendKeys();
            sendKeys.Sendkeys(element, value);
            log.Write("--{0}输入[{1}]。", name, value);
            if (wait.HasValue)
                Thread.Sleep(wait.Value);
        }

        public void TypeValueAndEnter(string value)
        {
            TypeValueAndEnter(value, null);
        }

        public void TypeValueAndEnter(string value, int? wait)
        {
            if (!CheckValue(value))
                return;

            var name = Name;
            var sendKeys = new SendKeys();
            sendKeys.Sendkeys(element, value, sendKeys.Enter);
            log.Write("--{0}输入[{1}]并回车。", name, value);
            if (wait.HasValue)
                Thread.Sleep(wait.Value);
        }

        public T GetPattern<T>(AutomationPattern pattern)
        {
            if (!IsExists || pattern == null)
                return default(T);

            var name = Name;
            object patternObject;
            if (element.TryGetCurrentPattern(pattern, out patternObject))
                return (T)patternObject;

            log.Write("--{0}不支持{1}模式。", name, pattern.ProgrammaticName);
            return default(T);
        }

        public string GetValue()
        {
            var name = Name;
            var value = string.Empty;
            var vp = GetPattern<ValuePattern>(ValuePattern.Pattern);
            if (vp != null)
            {
                value = vp.Current.Value;
                log.Write("--{0}获取值[{1}]。", name, value);
            }
            return value;
        }

        public void SetValue(string value)
        {
            SetValue(value, null);
        }

        public void SetValue(string value, int? wait)
        {
            if (!CheckValue(value))
                return;

            var name = Name;
            var vp = GetPattern<ValuePattern>(ValuePattern.Pattern);
            if (vp != null)
            {
                vp.SetValue(value);
                log.Write("--{0}输入[{1}]。", name, value);
                if (wait.HasValue)
                    Thread.Sleep(wait.Value);
            }
        }

        public void ClearValue()
        {
            ClearValue(null);
        }

        public void ClearValue(int? wait)
        {
            var name = Name;
            var vp = GetPattern<ValuePattern>(ValuePattern.Pattern);
            if (vp != null)
            {
                vp.SetValue("");
                log.Write("--{0}清空值。", name);
                if (wait.HasValue)
                    Thread.Sleep(wait.Value);
            }
        }

        public void SetValueAndEnter(string value)
        {
            SetValueAndEnter(value, null);
        }

        public void SetValueAndEnter(string value, int? wait)
        {
            if (!CheckValue(value))
                return;

            var name = Name;
            var vp = GetPattern<ValuePattern>(ValuePattern.Pattern);
            if (vp != null)
            {
                vp.SetValue(value);
                var sendKeys = new SendKeys();
                System.Windows.Forms.SendKeys.SendWait(sendKeys.Enter);
                System.Windows.Forms.SendKeys.Flush();
                log.Write("--{0}输入[{1}]并回车。", name, value);
                if (wait.HasValue)
                    Thread.Sleep(wait.Value);
            }
        }

        public void SelectItem()
        {
            SelectItem(null);
        }

        public void SelectItem(int? wait)
        {
            if (!IsExists)
                return;

            var name = Name;
            var sip = GetPattern<SelectionItemPattern>(SelectionItemPattern.Pattern);
            if (sip != null)
            {
                sip.Select();
                log.Write("--选择{0}。", name);
                if (wait.HasValue)
                    Thread.Sleep(wait.Value);
            }
        }

        public bool SelectGridItem(int index)
        {
            return SelectGridItem(index, null);
        }

        public bool SelectGridItem(int index, int? wait)
        {
            var rows = FindAllByType(ControlType.DataItem);
            if (rows != null && rows.Count > 0 && index < rows.Count)
            {
                rows[index].SelectItem(wait);
                return true;
            }

            return false;
        }

        public bool SelectGridItem(int row, int column, string text, int? wait)
        {
            var gp = GetPattern<GridPattern>(GridPattern.Pattern);
            if (gp != null)
            {
                var cell = gp.GetItem(row, column);
                if (cell != null && cell.Current.Name == text)
                {
                    var rows = FindAllByType(ControlType.DataItem);
                    if (rows != null && rows.Count > 0 && row < rows.Count)
                    {
                        rows[row].SelectItem(wait);
                        return true;
                    }
                }
            }

            return false;
        }

        public void Click()
        {
            Click(null);
        }

        public void Click(int? wait)
        {
            Click(wait, true);
        }

        public void Click(int? wait, bool addLog)
        {
            if (!IsExists)
                return;

            var name = Name;
            WinApi.Click(element);
            if (addLog)
                log.Write("--点击{0}。", name);

            if (wait.HasValue)
                Thread.Sleep(wait.Value);
        }

        public void Click(int offsetX, int offsetY)
        {
            if (!IsExists)
                return;

            var name = Name;
            WinApi.Click(element, offsetX, offsetY);
            log.Write("--点击{0}。", name);
            Thread.Sleep(500);
        }

        public void RightClick()
        {
            if (!IsExists)
                return;

            var name = Name;
            WinApi.RightClick(element);
            log.Write("--右击{0}。", name);
            Thread.Sleep(500);
        }

        public void ClickContextMenu(int menuOffsetX, int menuOffsetY)
        {
            if (!IsExists)
                return;

            var name = Name;
            WinApi.ClickContextMenu(element, menuOffsetX, menuOffsetY);
            log.Write("--右击{0}选择菜单。", name);
            Thread.Sleep(500);
        }

        private bool CheckValue(string value)
        {
            if (!IsExists)
                return false;

            if (value == null || value.Trim().Length == 0)
            {
                log.Write("--{0}，无数据输入！", Name);
                return false;
            }

            return true;
        }

        class WinApi
        {
            internal static void Click(AutomationElement control)
            {
                var rect = control.Current.BoundingRectangle;
                var incrementX = (int)(rect.Left + rect.Width / 2);
                var incrementY = (int)(rect.Top + rect.Height / 2);

                SetCursorPos(incrementX, incrementY);
                mouse_event(MOUSEEVENTF_LEFTDOWN, incrementX, incrementY, 0, UIntPtr.Zero);
                mouse_event(MOUSEEVENTF_LEFTUP, incrementX, incrementY, 0, UIntPtr.Zero);
            }

            internal static void Click(AutomationElement control, int offsetX, int offsetY)
            {
                SetCursorPos(offsetX, offsetY);
                mouse_event(MOUSEEVENTF_LEFTDOWN, offsetX, offsetY, 0, UIntPtr.Zero);
                mouse_event(MOUSEEVENTF_LEFTUP, offsetX, offsetY, 0, UIntPtr.Zero);
            }

            internal static void RightClick(AutomationElement control)
            {
                var rect = control.Current.BoundingRectangle;
                var incrementX = (int)(rect.Left + rect.Width / 2);
                var incrementY = (int)(rect.Top + rect.Height / 2);

                SetCursorPos(incrementX, incrementY);
                mouse_event(MOUSEEVENTF_RIGHTDOWN, incrementX, incrementY, 0, UIntPtr.Zero);
                mouse_event(MOUSEEVENTF_RIGHTUP, incrementX, incrementY, 0, UIntPtr.Zero);
            }

            internal static void ClickContextMenu(AutomationElement control, int menuOffsetX, int menuOffsetY)
            {
                var rect = control.Current.BoundingRectangle;
                var incrementX = (int)(rect.Left + rect.Width / 2);
                var incrementY = (int)(rect.Top + rect.Height / 2);

                SetCursorPos(incrementX, incrementY);
                mouse_event(MOUSEEVENTF_RIGHTDOWN, incrementX, incrementY, 0, UIntPtr.Zero);
                mouse_event(MOUSEEVENTF_RIGHTUP, incrementX, incrementY, 0, UIntPtr.Zero);
                Thread.Sleep(1000);

                SetCursorPos(incrementX + menuOffsetX, incrementY + menuOffsetY);
                mouse_event(MOUSEEVENTF_LEFTDOWN, incrementX + menuOffsetX, incrementY + menuOffsetY, 0, UIntPtr.Zero);
                mouse_event(MOUSEEVENTF_LEFTUP, incrementX + menuOffsetX, incrementY + menuOffsetY, 0, UIntPtr.Zero);
            }

            const int MOUSEEVENTF_MOVE = 0x0001;
            const int MOUSEEVENTF_LEFTDOWN = 0x0002;
            const int MOUSEEVENTF_LEFTUP = 0x0004;
            const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
            const int MOUSEEVENTF_RIGHTUP = 0x0010;
            const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
            const int MOUSEEVENTF_MIDDLEUP = 0x0040;
            const int MOUSEEVENTF_ABSOLUTE = 0x8000;

            [DllImport("user32.dll")]
            static extern bool SetCursorPos(int x, int y);
            [DllImport("user32.dll")]
            static extern void mouse_event(int mouseEventFlag, int incrementX, int incrementY, uint data, UIntPtr extraInfo);
            [DllImport("user32.dll", EntryPoint = "FindWindow")]
            internal static extern IntPtr FindWindow(string className, string windowName);
        }
    }
}
