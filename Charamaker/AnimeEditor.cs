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
using Charamaker3.Shapes;
using Rectangle = Charamaker3.Shapes.Rectangle;
using Charamaker3.Hitboxs;
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
        AnimeBlockManager BlockManager=new AnimeBlockManager(new DataSaver());
        AnimTumiki Tumiki=new AnimTumiki();
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

            {
                var e = Entity.make2(0, 0, 10000, 10, 0.5f, 0.5f);
                new DRectangle(-99999999, new ColorC(1, 0, 0, 0.5f)).add(e);
                e.add(w);
            }
            {
                var e = Entity.make2(0, 0, 10, 10000, 0.5f, 0.5f);
                new DRectangle(-9999999, new ColorC(1, 0, 0, 0.5f)).add(e);
                e.add(w);
            }
            // new DRectangle(-99999999,new ColorC(1,0,0,0.5f)).add(cam.c.watchRect);
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
            MessageBox.Text += "StartTime,TargetName,CommandName,AddCl=0:引数,... でモーション系の関数を飛ばせる。" + Environment.NewLine + Environment.NewLine;
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
                        w.update(0);
                        FileMan.SE.stopAll();


                    }
                }

            }
            if (started)
            {
                float cl = ((float)SpeedUd.Value);
                w.update(cl);

                BlockManager.update(w.staticEntity, cl);
                PlayTimeLabel.Text = BlockManager.Time + " / " + maxtime;

                PlayTimeLabel.Text += " is playing";
                TimeBar.Value = (int)Mathf.min((float)(BlockManager.Time) * 2, (float)TimeBar.Maximum);
            }
            else
            {
                timeLabel.Text = StartTimer + " / " + maxtime;
                w.update(0);
                BlockManager.update(w.staticEntity, 0);
                w.update(0);
                FileMan.SE.stopAll();
            }
            Tumiki.UpdateTumiki(BlockManager.Time,charamaker.km.GetCursourPoint(charamaker.cam, true));
            ScrollTimer.OnlyUpdate(1);
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

            var st = charamaker.cam.watchRect.gettxy2(0.1f, 0.75f);
            var ed = charamaker.cam.watchRect.gettxy2(0.5f, 0.89f);
            Tumiki.SetBlockManager(BlockManager, new Rectangle(st.x, st.y, (ed - st).x, (ed - st).y),charamaker.w);


            Debug.WriteLine("animchange!");
        }

        private void shown(object sender, EventArgs e)
        {

        }

        private void close(object sender, FormClosedEventArgs e)
        {
            newWorld();
            charamaker.display.removeCamera(cam);
            charamaker.OnControls();
            Tumiki.Clear();
        }

        /// <summary>
        /// バーをスクロールしてめっちゃ変更かかるとうるさいから
        /// </summary>
        SuperTimer ScrollTimer = new SuperTimer(20,20);
        private void TimeBar_Scroll(object sender, EventArgs e)
        {
            StartTimer = TimeBar.Value / 10f;
            timeLabel.Text = StartTimer + " / " + maxtime;
            if (ScrollTimer.OnlyGet())
            {
                if (!started)
                {
                    anmDChanged();

                    w.update(StartTimer);
                    BlockManager.update(w.staticEntity, StartTimer);
                    w.update(0);
                    FileMan.SE.stopAll();
                }
                else
                {

                }
            }
        }

        private void PlayB_Click(object sender, EventArgs e)
        {
            if (!started )
            {
                PlayB.Text = "Stop";
                {
                    anmDChanged(); 
                    w.update(StartTimer);
                    BlockManager.update(w.staticEntity, StartTimer);
                    w.update(0);
                    FileMan.SE.stopAll();
                    started = true;
                }
            }
            else
            {
                TimeBar.Value = (int)(StartTimer) * 10;
                PlayB.Text = "Start";
                started = false;
            }
        }

        private void MessageBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void CheckB_Click(object sender, EventArgs e)
        {
            if (CheckB.Text == "Check")
            {
                CheckB.Text = "Back";
                charamaker.OffControls();
            }
            else
            {
                CheckB.Text = "Check";
                charamaker.OnControls();
            }

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
    /// <summary>
    /// AnimBlockがどんな感じか示すためのクラス。
    /// </summary>
    class AnimTumiki 
    {
        AnimeBlockManager man = null;
        List<AnimBlock> AddedBlocks=new List<AnimBlock>();
        List<Character> BlockCharas = new List<Character>();
        List<Character> UIs = new List<Character>();
        Character TimeLine = null;

        Rectangle zone = new Rectangle(0,0,1,1);
        float MaxTime = 0;
        float Height = 0.01f;

        int Maxtumi = 0;
        public void Clear() 
        {
            //Clear
            {
                Maxtumi = 0;
                Height = zone.h / zone.w * 0.1f;
                AddedBlocks.Clear();
                foreach (var a in BlockCharas)
                {
                    a.e.remove();
                }
                BlockCharas.Clear();
                if (TimeLine != null)
                {
                    TimeLine.e.remove();
                }
            }
        }
        public void SetBlockManager(AnimeBlockManager man,Rectangle r,World w) 
        {
            this.man = man;
            zone = r;
            Clear();
            MaxTime=man.MaxTime;
            supersort<AnimBlock> sort = new supersort<AnimBlock>();
            foreach (var a in man.Blocks)
            {
                sort.add(a, a.StartTime);
            }
            sort.sort(false);
            foreach (var a in sort.getresult()) 
            {
                AddAnimBlock(a);
            }
            int cou = 0;
            foreach (var a in BlockCharas)
            {
                a.e.x *= zone.w;
                a.e.y *= zone.w;
                a.e.x += zone.x;
                a.e.y += zone.y;
                EntityMove.ScaleChange(10,"",r.w,r.w).addAndRemove(a.e,100);
                DrawableMove.ZDeltaChange(10,"",cou*10,1).addAndRemove(a.e, 100);
                cou++;
                a.e.add(w);

            }
            {
                TimeLine = Character.MakeCharacter("bluebit", 0, 0, 1, 1, 1, 1).getCharacter();
                EntityMove.ScaleChange(10, "", 0.005f, Height*(Maxtumi+3)).addAndRemove(TimeLine.e, 100);
                DrawableMove.BaseColorChange(10, "", 0.5f).addAndRemove(TimeLine.e, 100);


                int moji = 5;
                var Text = Entity.make(0, 0, Height * moji, Height, Height * moji * 0.5f, Height * 1f, 0, "text");
                TimeLine.addJoint(new Joint("TextJoi", 0.5f, 0.0f, TimeLine.e, new List<Entity> { Text }));

                new Text(1, new ColorC(0, 0, 0, 1), "0", new FontC(32, 32 * moji, 32, alignment:FontC.alignment.center)).add(Text);
                new Texture(0.0f, new ColorC(1, 1, 1, 0.5f), new Dictionary<string, string> { { "def", "bluebit" } }).add(Text);

                TimeLine.SetBaseCharacter();


                TimeLine.e.settxy(0, 0,0,0);

                TimeLine.e.x *= zone.w;
                TimeLine.e.y *= zone.w;
                TimeLine.e.x += zone.x;
                TimeLine.e.y += zone.y;

                EntityMove.ScaleChange(10, "", r.w, r.w).addAndRemove(TimeLine.e, 100);
                DrawableMove.ZDeltaChange(10, "", BlockCharas.Count * 10, 1).addAndRemove(TimeLine.e, 100);


                TimeLine.e.add(w);
            }



        }

        public void UpdateTumiki(float time, FXY cursour) 
        {
            var r = new Rectangle(cursour.x - 0.5f, cursour.y - 0.5f, 1, 1);
            int cou = 0;
            
            foreach (var a in BlockCharas) 
            {
                if (a.e.Hits(r, r)&&1==0)
                {
                    DrawableMove.BaseZDeltaChange(10, "", 0, 0).addAndRemove(a.e, 100);
                    DrawableMove.ZDeltaChange(10, "", BlockCharas.Count * 100, 1).addAndRemove(a.e, 100);

                }
                else
                {
                    DrawableMove.BaseZDeltaChange(10, "", 0, 0).addAndRemove(a.e, 100);
                    DrawableMove.ZDeltaChange(10, "", cou * 10, 1).addAndRemove(a.e, 100);

                }
                cou++;
            }
            if(TimeLine!=null)
            {
                TimeLine.e.x = time/MaxTime*zone.w;
                TimeLine.e.x += zone.x;
                TimeLine.getEntity("text").getDrawable<Text>().text = time.ToString();
            }
        }

        protected bool AddAnimBlock(AnimBlock InBlock) 
        {
            if (AddedBlocks.Contains(InBlock) == false)
            {
                AddedBlocks.Add(InBlock);
                float starttimer = InBlock.StartTime / MaxTime;
                float blocktimer = (InBlock.GetBlockTime(man) / MaxTime);

                var c = Character.MakeCharacter("redbit", 0, 0, 1, 0.0f, 1f, 0.5f).getCharacter();
                DrawableMove.BaseColorChange(10, "", 0.5f).addAndRemove(c.e, 100);
                new Hitbox(new Rectangle(0,0,1,1),new List<int>(1),new List<int>()).add(c.e);

                EntityMove.ScaleChange(10, "", blocktimer, Height).addAndRemove(c.e, 100);
                //Debug.WriteLine(c.e.x + " e:r:t " + c.e.y + " == " + c.e.w + " :: " + c.e.h);

                int moji = 20;
                var Text = Entity.make(0, 0, Height * moji, Height, Height * moji * 0.0f, Height * 1f, 0, "text");
                c.addJoint(new Joint("TextJoi", 0.0f, 1f, c.e, new List<Entity> { Text }));

                new Text(1,new ColorC(0,0,0,1), (InBlock.StartTime)+"@ "+InBlock.Gyo+" @"+(InBlock.StartTime+InBlock.GetBlockTime(man)), new FontC(32,32*moji,32)).add(Text);
                new Texture(0.0f, new ColorC(1, 1, 1, 1), new Dictionary<string, string> { { "def", "whitebit" } }).add(Text);

                c.SetBaseCharacter();

                c.e.update(1);

                int tumicou = 0;

                //ブロックの左下の点
                var BP = new Rectangle(starttimer, 0, blocktimer, Height);
                for (int i = 0; i < 10; i++)
                {
                    foreach (var a in BlockCharas)
                    {
                        var BP2 = new Rectangle(a.e.x, a.e.y, a.e.w+0.05f, a.e.h);
                        if (BP.atarun(BP2))
                        {
                            tumicou++;
                            BP.y -= Height;
                        }
                    }
                }
                Maxtumi = (int)Mathf.max(tumicou, Maxtumi);

                c.e.settxy(BP.x, BP.y+BP.h);
                BlockCharas.Add(c);
                return true;
            }
            return false;
        }
    }
}
