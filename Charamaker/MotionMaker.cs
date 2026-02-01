using Charamaker3;
using Charamaker3.CharaModel;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static Charamaker3.CharaModel.DrawableMove;
using static Charamaker3.CharaModel.EntityMove;
namespace Charamaker
{
    public partial class MotionMaker : Form
    {

        Charamaker cm;
        public MotionMaker(Charamaker cm)
        {
            InitializeComponent();
            this.cm = cm;
            this.ScriptB.Text = setstringtobox(cm.MotionString);
            setmanual();
        }
        string setstringtobox(string s)
        {
            return s.Replace("\n", Environment.NewLine);
        }
        void addmotion(Motion m)
        {
            if (cm.anmE != null)
            {
                cm.anmE.SetText(m.ToSave());
            }
            if (cm.sel.c != null && cm.sel.c.e != null)
            {
                m.add(cm.sel.c.e);
            }
            m.speed = (float)speedUD.Value;
        }
        private void pathB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                var m = FileMan.loadMotion(pathB.Text, true);
                if (m != null && m.Motion != null && m.Script != null && m.Script != "")
                {
                    //setmotionjouhou(m);
                    if (m.Script == "") m.Script = " ";
                    Clipboard.SetText(m.Script);
                    addmotion(m.Motion);

                    messageB.Text = setstringtobox(m.Script);
                    setmanual();
                }
                else
                {
                    if (Directory.Exists(FileMan.s_rootpath + @".\motion\" + pathB.Text))
                    {
                        messageB.Text = "";
                        string[] filesM = System.IO.Directory.GetFiles(FileMan.s_rootpath + @".\motion\" + pathB.Text, "*.ctm", System.IO.SearchOption.AllDirectories);
                        for (int i = 0; i < filesM.Count(); i++)
                        {
                            messageB.Text += filesM[i].Replace(FileMan.s_rootpath + @".\motion\", @"") + Environment.NewLine;

                        }
                    }
                    else
                    {
                        messageB.Text = "";
                        string[] filesC = System.IO.Directory.GetFiles(FileMan.s_rootpath + @".\character\", "*.ctc", System.IO.SearchOption.AllDirectories);
                        for (int i = 0; i < filesC.Count(); i++)
                        {
                            messageB.Text += filesC[i].Replace(FileMan.s_rootpath + @".\character\", @"character\") + Environment.NewLine;

                        }
                    }
                    setmanual();
                }

            }






        }
        /// <summary>
        /// モーションをスクリプトから作る
        /// </summary>
        /// <param name="script">スクリプトか？</param>
        /// <param name="speed">再生速度</param>
        /// <param name="yobidasi">呼び出してくれるクラス</param>
        /// <returns></returns>
        static Motion buildMotion(string script)
        {
            //var res = new Motion();
            //            work.sp = speed;

            script = "using static Charamaker3.CharaModel.DrawableMove;" +
                "using static Charamaker3.CharaModel.EntityMove;" +
                "var res=new Motion();\n" + script + ";\nreturn res;";
            ScriptOptions a = ScriptOptions.Default
            .WithReferences(Assembly.GetEntryAssembly())
            .WithImports("System", "System.Collections.Generic", "Charamaker3"
            , "Charamaker3.CharaModel");

            var Q = CSharpScript.Create(script, options: a);
            var runner = Q.CreateDelegate();
            var run = (Delegate)runner;
            //runner();
            var ret = (Motion)runner().Result;


            return ret;
        }
        private void saveB_Click(object sender, EventArgs e)
        {
            var m = build();
            var path = FileMan.dialog("motion", ".ctm");
            var temp = FileMan.s_rootpath;//ダイアログなのでルートパスを外す
            FileMan.s_rootpath = "";
            FileMan.saveMotion(path, ScriptB.Text, m);
            FileMan.s_rootpath = temp;
        }
        private void ScriptB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                var m = build();
                cm.MotionString = ScriptB.Text.Replace(Environment.NewLine, "\n");
                cm.Save();
                addmotion(m);

            }
            if (e.KeyCode == Keys.F2)
            {
                ScriptB.Font = new Font(ScriptB.Font.FontFamily, ScriptB.Font.Size + 1);
                messageB.Font = new Font(messageB.Font.FontFamily, messageB.Font.Size + 1);
            }
            else if (e.KeyCode == Keys.F1)
            {
                ScriptB.Font = new Font(ScriptB.Font.FontFamily, ScriptB.Font.Size - 1);
                messageB.Font = new Font(messageB.Font.FontFamily, messageB.Font.Size - 1);
            }
        }
        private Motion build()
        {

            /*/移植用
            {
                Debug.WriteLine(" isyoku");
                string[] filesM = System.IO.Directory.GetFiles(FileMan.rootpath+@"\motion\", "*.c3m", System.IO.SearchOption.AllDirectories);
                foreach (var a in filesM)
                {
                    Debug.WriteLine(a + " kyusaisimasu");
                    if (!a.Contains("zfile"))
                    {
                        string file = a;
                        string dir = a.Replace(".c3m",".ctm");
                        var d = DataSaver.loadFromPath(file, false, ".c3m");

                        var script = d.unpackDataS("script");
                        script = script.Replace("work.addmoves", "res.addmove");
                        script = script.Replace("new ", "");
                        script = script.Replace("setumageman", "RotateToLim");
                        script = script.Replace("radtoman", "RotateToLim");
                        script = script.Replace("idouman", "XYD");
                        script = script.Replace("setuidouman", "XYD");
                        script = script.Replace("texchangeman", "ChangeTexture");
                        script = script.Replace("Kopaman", "BaseColorChange");
                        script = script.Replace("zchangeman", "ZChange");
                        script = script.Replace("Kzchangeman", "BaseZChange");
                        script = script.Replace("Kscalechangeman", "scalechange");
                        script = script.Replace("Ktyusinchangeman", "calechange");
                        script = script.Replace("Kdxychangeman", "JointMove");
                        script = script.Replace("zkaitenman", "ZRotate");
                       

                        FileMan.saveMotion(dir, script, new Motion());

                        Debug.WriteLine(a + " kyusaiend");



                    }
                }
                
            }*/
            try
            {
                messageB.Text = "OK! -> ";
                var res = buildMotion(this.ScriptB.Text);

                messageB.Text += res.sumtime.ToString() + " time ";

                setmanual();

                return res;
            }
            catch (Exception e)
            {

                messageB.Text = "DEATH " + Environment.NewLine + e.ToString();

                setmanual();
                return new Motion();

            }
        }

        private void MotionMaker_Load(object sender, EventArgs e)
        {

        }

        private void SRB_Click(object sender, EventArgs e)
        {
            var m = new Motion();
            m.addmove(EntityMove.EResetMove(0, "", false));
            if (cm.sel.c != null && cm.sel.c.e != null)
            {
                m.addAndRemove(cm.sel.c.e, 100);
            }
        }

        private void DRB_Click(object sender, EventArgs e)
        {

            var m = new Motion();
            m.addmove(EntityMove.RotateToBace(0, "", 0, rotatePath.shorts, true));
            if (cm.sel.c != null && cm.sel.c.e != null)
            {
                m.addAndRemove(cm.sel.c.e, 100);
            }
        }

        private void TRB_Click(object sender, EventArgs e)
        {
            var m = new Motion();
            m.addmove(DrawableMove.DResetMove(0, "", true, true, true, false));
            if (cm.sel.c != null && cm.sel.c.e != null)
            {
                m.addAndRemove(cm.sel.c.e, 100);
            }
        }

        private void ReverseB_Click(object sender, EventArgs e)
        {
            var m = new Motion();
            m.addmove(EntityMove.Mirror(0, ""));
            if (cm.sel.c != null && cm.sel.c.e != null)
            {
                m.addAndRemove(cm.sel.c.e, 100);
            }
        }
        void setmanual()
        {
            string res = "res.addmove(XYD(...))ってカンジ" + Environment.NewLine + Environment.NewLine;
            string xml_file_path_ = @".\Charamaker3.xml";

            //xmlを読み込め。
            XDocument xml_ = XDocument.Load(xml_file_path_);

            //ルートタグを変数に。
            XElement root = xml_.Element("doc");


            //ルートタグを変数に。
            XElement members = root.Element("members");

            foreach (XElement a in members.Elements())
            {

                var name = a.Attribute("name").Value.Split(':');
                //メソッドを取得
                if (name[0] == "M")
                {
                    //returns ="__Move__"なら書き込む
                    if (a.Element("returns") != null && a.Element("returns").Value == "__MOVE__")
                    {
                        var dots = name[1].Split('(')[0].Split('.');
                        var TNAME = "";

                        for (int i = dots.Count() - 1; i < dots.Count(); i++)
                        {
                            if (i > dots.Count() - 1) TNAME += ".";
                            TNAME += dots[i];
                        }
                        TNAME += "(";

                        foreach (var b in a.Elements("param"))
                        {
                            TNAME += " " + b.Attribute("name").Value;
                            TNAME += ":" + b.Value + " ,";
                        }

                        TNAME += ")"; TNAME += a.Element("summary").Value;
                        TNAME += Environment.NewLine;

                        res += TNAME;
                    }
                }

            }

            messageB.Text += Environment.NewLine + "**********************************" + Environment.NewLine;
            messageB.Text += res;
        }

        private void ScriptB_TextChanged(object sender, EventArgs e)
        {

        }

        private void speedUD_ValueChanged(object sender, EventArgs e)
        {

        }

        List<Entity> narabeCharas = new List<Entity>();
        List<Entity>narabeTexts= new List<Entity>();

        private void narabeB_Click(object sender, EventArgs e)
        {
            if (pathB.Text != "")
            {

                messageB.Text = "";
                string[] filesM = System.IO.Directory.GetFiles(FileMan.s_rootpath + @".\motion\", pathB.Text + "*.ctm", System.IO.SearchOption.AllDirectories);
                int cou = 0;
                for (int i = 0; i < filesM.Count(); i++)
                {
                    messageB.Text += filesM[i].Replace(FileMan.s_rootpath + @".\motion\", @"") + Environment.NewLine;

                    if (cm.sel.c != null && cm.sel.c.e != null)
                    {
                        //for (int t = 0; t < 3; ++t)
                        {
                            var chara = cm.sel.c.e.clone();
                            EntityMove.EResetMove().addAndRemove(chara, 100);
                            DrawableMove.DResetMove().addAndRemove(chara, 100);
                            narabeCharas.Add(chara);
                            var m = FileMan.loadMotion(filesM[i].Split(@".\motion\")[1], true);
                            if (m.Motion.IsEmpty == false)
                            {
                                if (m != null && m.Motion != null && m.Script != null && m.Script != "")
                                {
                                    m.Motion.add(chara);
                                }
                                chara.name = filesM[i].Split(@".\motion\")[1];


                                float haba = chara.w * ((float)cm.PointB.Value / cm.PointB.Maximum * 4f + 1);
                                chara.add(cm.w);
                                int num = (int)Mathf.max(cm.cam.watchRect.w / (haba), 1);
                                chara.settxy(chara.gettxy(), -haba * (cou % (num) + 0.5f), -chara.h * 1.5f * (cou / (num) + 0.1f));
                                var text = Entity.make2(chara.gettxy2().x, chara.gettxy2(float.NaN, 1).y, haba, haba, 0.5f, 0);

                                new Text(99999, new ColorC(0, 0, 0, 1), chara.name, new FontC(haba / 10, haba, haba, alignment: FontC.alignment.center)).add(text);
                                text.add(cm.w);
                                narabeTexts.Add(text);
                                
                                cou++;
                            }
                        }
                    }
                }
            }
            else 
            {

                foreach (var a in narabeCharas)
                {
                    a.remove();
                }
                narabeCharas.Clear();


                foreach (var a in narabeTexts)
                {
                    a.remove();
                }
                narabeTexts.Clear();
            }

                setmanual();
        }
    }
}
