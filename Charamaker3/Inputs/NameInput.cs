using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Charamaker3.Inputs
{
    /// <summary>
    /// Charamaker3のキー入力。
    /// </summary>
    public class IButton
    {
        public IButton(MouseButtons m)
        {
            Mouse = m;
        }
        public IButton(Keys k)
        {
            Key = k;
        }

        public IButton(IButton i)
        {
            Key = i.Key;
            Mouse = i.Mouse;
        }
        /// <summary>
        /// マウスの状態
        /// </summary>
        protected MouseButtons Mouse = MouseButtons.None;
        /// <summary>
        /// キーボードの状態
        /// </summary>
        protected Keys Key = Keys.None;

        static public bool operator ==(IButton a, IButton b)
        {
            if (a.Mouse != MouseButtons.None)
            {
                return a.Mouse == b.Mouse;
            }


            if (a.Key != Keys.None)
            {
                return a.Key == b.Key;
            }
            return false;
        }

        static public bool operator !=(IButton a, IButton b)
        {
            return !(a == b);
        }
        static public implicit operator string(IButton a)
        {
            return a.ToString();
        }
        public string ToString()
        {
            if (Mouse != MouseButtons.None)
            {
                return Mouse.ToString();
            }

            if (Key != Keys.None)
            {
                return Key.ToString();
            }
            return "None";
        }
        /// <summary>
        /// ここをオーバーライドしなさい。
        /// </summary>
        /// <returns></returns>
        virtual public IButton Clone()
        {
            return new IButton(this);
        }
        public static bool Contains<T>(T a, List<T> list)
        where T : IButton
        {
            foreach (var b in list)
            {
                if (a == b) return true;
            }
            return false;
        }
        public static bool Remove<T>(T a, List<T> list)
        where T : IButton
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (a == list[i])
                {
                    list.RemoveAt(i);
                    return true;
                }
            }
            return false;

        }
    }
    
    /// <summary>
    /// キー入力をstringで管理できる奴
    /// </summary>
    /// <typeparam name="T">Ibuttonを継承して作ってもいい</typeparam>
    public class NameInput
    {
        KeyMouse input;
        List<string> Names;
        List<IButton> Button;
        public NameInput(KeyMouse input) 
        {
            this.input = input;
            Names = new List<string>();
            Button= new List<IButton>(); 
        }
        public void Bind(string Name, IButton button) 
        {
            Names.Add(Name);
            Button.Add((IButton)button.Clone());
        }
        public bool UnBind(string Name) 
        {
            for (int i = 0; i < Names.Count; i++) 
            {
                if (Names[i] == Name) 
                {
                    Names.RemoveAt(i);
                    Button.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public List<string> GetBinds() 
        {
            return new List<string>(Names);
        }
        public List<IButton> GetBindedButtons()
        {
            return new List<IButton>(Button);
        }

        public bool ok(string name,itype itype)
        {
            for (int i = 0; i < Names.Count; i++) 
            {
                if (name == Names[i]) 
                {
                    return input.ok(Button[i],itype);
                }
            }
            Debug.WriteLine(name + " is not binded on this Nameinput");
            return false; 
        }

        /// <summary>
        /// [input:Name]の形式で書かれた文字列を対応したキーの文字列に変換する。
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string Replace(string text) 
        {
            var res = text;
            var tag = "[input:";
            for (int i = 0; i < text.Length-tag.Length; i++)
            {
                if (tag == text.Substring(i, tag.Length))
                {

                    i += tag.Length;
                    int ends = i;
                    while (ends<text.Length)
                    {
                        bool breaker = false;
                        if (text[ends] == ']') 
                        {
                            var name = text.Substring(i, ends - i );
                            var names = GetBinds();
                            var binds = GetBindedButtons();
                            for (int j = 0; j < names.Count(); j++) 
                            {
                                if (name == names[j]) 
                                {
                                    res=res.Replace(tag + name + "]", binds[j].ToString());
                                    breaker = true;
                                    break;
                                }

                            }
                        }
                        if (breaker) break;


                        ends++;
                    }
                }


            }
            return res;
        }
    }
}
