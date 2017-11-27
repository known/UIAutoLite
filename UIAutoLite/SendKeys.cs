using System;
using System.Text;
using System.Threading;
using System.Windows.Automation;

namespace UIAutoLite
{
    public class SendKeys
    {
        StringBuilder builder = new StringBuilder();

        public string Alt = "%";
        public string ContextMenu = "+{F10}";
        public string Ctrl = "^";
        public string Shift = "+";
        public string Enter = "{Enter}";
        public string Delete = "{Del}";
        public string Save = "^S";
        public string SaveAll = "^+S";
        public string Copy = "^C";
        public string Cut = "^X";
        public string Paste = "^V";
        public string Undo = "^Z";
        public string Redo = "^Y";
        public string Print = "^P";
        public string Help = "{F1}";
        public string New = "^N";

        public string[] Keys { get; set; }

        public void Sendkeys(AutomationElement element, params string[] keys)
        {
            Keys = keys;

            try
            {
                element.SetFocus();
            }
            catch (Exception exception)
            {
                throw new Exception("Cannot set focus to this element.", exception);
            }
            Thread.Sleep(200);
            System.Windows.Forms.SendKeys.Flush();
            if (Keys != null && Keys.Length > 0)
            {
                System.Windows.Forms.SendKeys.SendWait("^A" + Delete);
            }
            var myKeys = "";
            foreach (string str2 in Keys)
            {
                myKeys = myKeys + str2;
            }
            Thread.Sleep(200);
            if ((ContainsUnescapedKey(myKeys, '^') || ContainsUnescapedKey(myKeys, '%')) || ContainsUnescapedKey(myKeys, '+'))
            {
                myKeys = myKeys.ToLower();
            }

            System.Windows.Forms.SendKeys.SendWait(myKeys);
            System.Windows.Forms.SendKeys.Flush();
        }

        public void Sendkeys(AutomationElement element, string key)
        {
            Keys = new string[1];
            Keys[0] = key;

            try
            {
                element.SetFocus();
            }
            catch (Exception exception)
            {
                throw new Exception("Cannot set focus to this element.", exception);
            }
            Thread.Sleep(200);
            System.Windows.Forms.SendKeys.Flush();
            if (Keys.Length > 0)
            {
                System.Windows.Forms.SendKeys.SendWait("^A" + Delete);
            }
            if ((ContainsUnescapedKey(key, '^') || ContainsUnescapedKey(key, '%')) || ContainsUnescapedKey(key, '+'))
            {
                key = key.ToLower();
            }

            System.Windows.Forms.SendKeys.SendWait(key);
            System.Windows.Forms.SendKeys.Flush();
        }

        private bool ContainsUnescapedKey(string keys, char key)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (keys[i] == key)
                {
                    if ((i == 0) || (i == (keys.Length - 1)))
                    {
                        return true;
                    }
                    if ((keys[i - 1] != '{') || (keys[i + 1] != '}'))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private string KeysToString(string[] keys)
        {
            if (keys != null)
            {
                for (int i = 0; i < keys.Length; i++)
                {
                    string str = keys[i];
                    if (str == null)
                    {
                        builder.Append(keys[i]);
                    }
                    int length = keys.Length - 1;
                    switch (str)
                    {
                        case "^":
                            builder.Append("Ctrl");
                            IsEquals(i, length, builder);
                            break;
                        case "+{F10}":
                            builder.Append("Open Context Menu");
                            IsEquals(i, length, builder);
                            break;
                        case "%":
                            builder.Append("Alt");
                            IsEquals(i, length, builder);
                            break;
                        case "+":
                            builder.Append("Shift");
                            IsEquals(i, length, builder);
                            break;
                        case "^S":
                            builder.Append("Save");
                            IsEquals(i, length, builder);
                            break;
                        case "^X":
                            builder.Append("Cut");
                            IsEquals(i, length, builder);
                            break;
                        case "^C":
                            builder.Append("Copy");
                            IsEquals(i, length, builder);
                            break;
                        case "^V":
                            builder.Append("Paste");
                            IsEquals(i, length, builder);
                            break;
                        case "^+S":
                            builder.Append("Save All");
                            IsEquals(i, length, builder);
                            break;
                        case "^P":
                            builder.Append("Print");
                            IsEquals(i, length, builder);
                            break;
                        case "^Z":
                            builder.Append("Undo");
                            IsEquals(i, length, builder);
                            break;
                        case "^Y":
                            builder.Append("Redo");
                            IsEquals(i, length, builder);
                            break;
                        case "^N":
                            builder.Append("New");
                            IsEquals(i, length, builder);
                            break;
                        default:
                            builder.Append(keys[i]);
                            IsEquals(i, length, builder);
                            break;
                    }
                }
            }
            return builder.ToString();
        }

        void IsEquals(int i, int length, StringBuilder builder)
        {
            if (i < length)
            {
                builder.Append("+");
            }
        }

        public override string ToString()
        {
            return string.Format("Sendkeys to input data or operator with keys = '{0}'", KeysToString(Keys));
        }
    }
}
