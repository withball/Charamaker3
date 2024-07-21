using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Charamaker3;
using Charamaker3.CharaModel;
using Component = Charamaker3.Component;
namespace Charamaker
{
    public partial class AnimeEditor : Form
    {
        Charamaker charamaker;
        FileInfo? preinfo = null;
        World w;
        Camera cam;
        bool started = false;

        float timer = 0;
        float maxtime = 1;
        AnimeLoader anm;
        public AnimeEditor(Charamaker c)
        {
            this.charamaker = c;
            InitializeComponent();
            cam = charamaker.display.makeCamera(new ColorC(0, 0, 0, 0));
            newWorld();
        }
        void newWorld() 
        {

            cam.watchRect.remove();
            w = new World();
            charamaker.display.removeCamera(cam);
            cam = charamaker.display.makeCamera(new ColorC(0, 0, 0, 0));
            cam.watchRect.add(w);
        }

        private void AnimeEditor_Load(object sender, EventArgs e)
        {
            SetText(new DataSaver());
        }
        public void SetText(DataSaver d)
        {
            SetText(d.getData().Replace("\n", Environment.NewLine));
        }
        public void SetText(string d)
        {
            MessageBox.Text = "";
            foreach (var a in Debug.downMess()) 
            {
                MessageBox.Text += a + Environment.NewLine;
            }
            MessageBox.Text += Environment.NewLine + "******************************" + Environment.NewLine;
            MessageBox.Text += d.Replace("\n", Environment.NewLine);
            MessageBox.Text += Environment.NewLine + "******************************" + Environment.NewLine;
            MessageBox.Text += "[F:Funcname:xxx]{[arg0]{}[arg0]{}[arg0]{}[arg0]{}} でMotionMaker系の関数を使える" + Environment.NewLine;
            {
                var res = "";
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
                        //returns ="__ANIM__"なら書き込む
                        if (a.Element("returns") != null && a.Element("returns").Value == "__ANIM__")
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
                MessageBox.Text += res;
            }


            MessageBox.Text += new AnimeLoader(new DataSaver()).ToSave().getData().Replace("\n", Environment.NewLine);

        }

        private void tiked(object sender, EventArgs e)
        {
            {
                var path = @".\animation\" + this.loadBox.Text + ".anm";

                if (!File.Exists(path))
                {
                    anm = new AnimeLoader(new DataSaver());
                }
                else
                {

                    var info = new FileInfo(path);
                    if (preinfo == null || DateTime.Compare(info.LastWriteTime, preinfo.LastWriteTime) > 0)
                    {
                        var anmD = DataSaver.loadFromPath(path, true, "");
                        anm = new AnimeLoader(anmD);
                        preinfo = info;
                        anmDChanged();
                        timer = TimeBar.Value / 10f;
                        w.update(timer);
                    }
                }

            }
            if (timer < maxtime && started)
            {
                var ntimer = Mathf.min(timer + 1 * ((float)SpeedUd.Value), maxtime);
                w.update(ntimer - timer);
                timer = ntimer;
                this.TimeBar.Value = (int)(timer * 10);
                if (timer == maxtime)
                {
                    started = false;
                }
            }
            timeLabel.Text = timer + " / " + maxtime;
            if (started) 
            {
                timeLabel.Text += " is playing";
            }
            w.update(0);
        }
        void anmDChanged()
        {

            newWorld();
            var d=anm.MakeAnime(w.staticEntity);
            maxtime = Mathf.max(Mathf.abs(d.unpackDataF("maxtime")),1);
            {
                TimeBar.Maximum = (int)(maxtime * 10);
            }
            Debug.WriteLine("animchange!");
        }

        private void shown(object sender, EventArgs e)
        {

        }

        private void close(object sender, FormClosedEventArgs e)
        {
            charamaker.display.removeCamera(cam);
        }

        private void TimeBar_Scroll(object sender, EventArgs e)
        {
            if (!started)
            {
                anmDChanged();
                timer = TimeBar.Value / 10f;
                w.update(timer);
            }
        }

        private void PlayB_Click(object sender, EventArgs e)
        {
            if (!started )
            {
                if (timer < maxtime)
                {
                    started = true;
                }
                else
                {
                    anmDChanged();
                    started = true;
                    timer = 0;
                }
            }
            else 
            {
                started = false;
            }
        }

        private void MessageBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void CheckB_Click(object sender, EventArgs e)
        {
            if (1 == 0)
            {
                string text = "";
                var sp = anm.Check().Split('\n');
                for (int i = 0; i < sp.Length; i++)
                {
                    int cou = 0;
                    while (cou < sp[i].Length && sp[i][cou++] == '.')
                    {

                    }
                    text += cou.ToString() + " : " + sp[i].Substring(cou) + Environment.NewLine;

                }
                SetText(text);
            }
            else 
            {
                var text = "";
                foreach (var a in anm.Check2()) 
                {
                    text += "["+a+"]" + Environment.NewLine;
                }
               // text += anm.Check3();
                SetText(text);
            }
        }
    }
}
