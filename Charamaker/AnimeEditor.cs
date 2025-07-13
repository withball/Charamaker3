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
        CP<Camera> cam;
        bool started = false;

        float StartTimer = 0;
        float maxtime = 1;
        //AnimeLoader anm;
        AnimeBlockManager BlockManager;
        public AnimeEditor(Charamaker c)
        {
            this.charamaker = c;
            InitializeComponent();
            cam = Component.ToPointer(charamaker.display.makeCamera(new ColorC(0, 0, 0, 0)));
            newWorld();
        }
        void newWorld() 
        {

            cam.c.watchRect.remove();
            foreach (var a in FileMan.SoundEngines) 
            {
                a.stopAll();
            }
            if (w != null)
            {
                foreach (var a in w.getEdic())
                {
                    a.remove();
                }
            }
            w = new World();
            charamaker.display.removeCamera(cam);
            cam = Component.ToPointer(charamaker.display.makeCamera(new ColorC(0, 0, 0, 0)));
            cam.c.watchRect.add(w);
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
            MessageBox.Text += "StartTime,TargetName,CommandName:引数,... でモーション系の関数を飛ばせる。" + Environment.NewLine;
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
            if (started == false)//始まってない場合読み込み
            {
                var Truepath = FileMan.s_rootpath + @"animation/" + this.loadBox.Text + ".anm";
                var path = @"animation/" + this.loadBox.Text;

                if (!File.Exists(Truepath))
                {
                    BlockManager = new AnimeBlockManager(new DataSaver());
                    //anm = new AnimeLoader(new DataSaver());
                }
                else
                {

                    var info = new FileInfo(Truepath);
                    if (preinfo == null || DateTime.Compare(info.LastWriteTime, preinfo.LastWriteTime) > 0)
                    {
                        //var anmD = DataSaver.loadFromPath(path, true, "");

                        var blockD = DataSaver.loadFromPath(path, false, ".anm");
                        //Debug.WriteLine("POI " + blockD.getData());
                        //anm = new AnimeLoader(anmD);
                        BlockManager = new AnimeBlockManager(blockD);
                        //Debug.WriteLine(path+"Was "+BlockManager.ToString());
                        preinfo = info;
                        StartTimer = TimeBar.Value / 10f;

                        anmDChanged();

                        w.update(StartTimer);
                        BlockManager.update(w.staticEntity, StartTimer);

                    }
                }

            }
            if (started)
            {
                float cl = ((float)SpeedUd.Value);
                w.update(cl);

                BlockManager.update(w.staticEntity, cl);
                timeLabel.Text = BlockManager.Time + " / " + maxtime;

                timeLabel.Text += " is playing";
            }
            else
            {
                timeLabel.Text = StartTimer + " / " + maxtime;
                w.update(0);
                BlockManager.update(w.staticEntity, 0);
            }
        }
        void anmDChanged()
        {
            newWorld();
            //var d=anm.MakeAnime(w.staticEntity);

            //maxtime = Mathf.max(Mathf.abs(d.unpackDataF("maxtime")),1);
            BlockManager.Start();
            
            maxtime = BlockManager.MaxTime*4;
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
            //if (!started)
            {
                StartTimer = TimeBar.Value / 10f; 
                anmDChanged();

                w.update(StartTimer);
                BlockManager.update(w.staticEntity, StartTimer);
            }
        }

        private void PlayB_Click(object sender, EventArgs e)
        {
            if (!started )
            {
                PlayB.Text = "Stop";
                {
                    anmDChanged();
                    started = true;
                    StartTimer = 0;
                }
            }
            else
            {
                PlayB.Text = "Start";
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
                /*
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
                */
            }
            else 
            {
                var text = "";
                /*foreach (var a in anm.Check2()) 
                {
                    text += "["+a+"]" + Environment.NewLine;
                }*/

                text = BlockManager.ToString();
               // text += anm.Check3();
                SetText(text);
            }
        }
    }
}
