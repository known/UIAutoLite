using System;
using System.Runtime.InteropServices;
using System.Windows.Automation;

namespace UIAutoLite
{
    class ElementHelper
    {
        [DllImport("AutoItX3.dll", EntryPoint = "AU3_ControlGetHandle", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr AU3_ControlGetHandle32(IntPtr winHandle, [MarshalAs(UnmanagedType.LPWStr)] string control);
        [DllImport("AutoItX3_x64.dll", EntryPoint = "AU3_ControlGetHandle", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr AU3_ControlGetHandle64(IntPtr winHandle, [MarshalAs(UnmanagedType.LPWStr)] string control);

        internal static AutomationElement FindElement(IntPtr winHandle, string control)
        {
            if (Marshal.SizeOf(IntPtr.Zero) == 4)
            {
                var hwnd = AU3_ControlGetHandle32(winHandle, control);
                return AutomationElement.FromHandle(hwnd);
            }
            else
            {
                var hwnd = AU3_ControlGetHandle64(winHandle, control);
                return AutomationElement.FromHandle(hwnd);
            }
        }

        internal static AutomationElement FindElement(AutomationElement parent, TreeScope scope, Condition condition)
        {
            return FindElement(parent, scope, condition, null);
        }

        internal static AutomationElement FindElement(AutomationElement parent, TreeScope scope, Condition condition, int? timeout)
        {
            AutomationElement element = null;
            if (parent != null && condition != null)
            {
                var endTime = DateTime.Now.AddMilliseconds(timeout ?? 60000);
                while (true)
                {
                    element = parent.FindFirst(scope, condition);
                    if (element != null || endTime < DateTime.Now)
                    {
                        break;
                    }
                    System.Threading.Thread.Sleep(1000);
                }
            }

            return element;
        }

        internal static AutomationElement FindElement(AutomationElement root, string property)
        {
            if (root == null || string.IsNullOrEmpty(property))
                return null;

            if (property.Contains(","))
            {
                var index = property.IndexOf(',');
                if (index > 0)
                {
                    var parent = FindElement(root, property.Substring(0, index));
                    return FindElement(parent, property.Substring(index + 1));
                }
            }

            PropertyCondition condition = null;
            var properties = property.Split(':');
            if (properties.Length == 1)
                condition = new PropertyCondition(AutomationElement.AutomationIdProperty, properties[0]);
            else
            {
                switch (properties[0])
                {
                    case "Name":
                        condition = new PropertyCondition(AutomationElement.NameProperty, properties[1]);
                        break;
                    case "ControlType":
                        condition = new PropertyCondition(AutomationElement.ControlTypeProperty, properties[1]);
                        break;
                }
            }

            if (condition != null)
                return root.FindFirst(TreeScope.Children, condition);

            return null;
        }
    }
}
