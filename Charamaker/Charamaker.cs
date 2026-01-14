using Charamaker3;
using Charamaker3.CharaModel;
using Charamaker3.Inputs;
using Charamaker3.ParameterFile;
using Charamaker3.Utils;
namespace Charamaker
{


    public partial class Charamaker : Form
    {
        System.Diagnostics.Stopwatch stopwatch=new System.Diagnostics.Stopwatch();
        public Display display;
        public World w = new World();
        public World w2 = new World();
        public Camera cam;
        public KeyMouse km = new KeyMouse();
        System.Drawing.Size BaseSize = new Size((int)(1600 * 0.85f), (int)(900 * 0.85f));

        NameInput inp;

        Texture SLP, TXYP;
        DataSaver save;

        public CP<Camera> cam2;

        IntervalClocker intervalClocker;

        public void OffControls()
        {
            this.DSB.Hide();
            this.screenshotB.Hide();
            this.texturelabel.Hide();
            this.printoutB.Hide();
            this.ResetTextureB.Hide();
            this.ForceMirrorButton.Hide();
            this.setBaseB.Hide();
            this.messageB.Hide();

        }

        public void OnControls()
        {

            this.DSB.Show();
            this.screenshotB.Show();
            this.texturelabel.Show();
            this.printoutB.Show();
            this.ResetTextureB.Show();
            this.ForceMirrorButton.Show();
            this.setBaseB.Show();
            this.messageB.Show();
            /*
            foreach (var a in this.Controls)
            {
                if (Mathf.isSubClassOf(a.GetType(), typeof(Control)))
                {
                    ((Control)a).Show();
                }
            }
        */
        }

        public Charamaker()
        {
            InitializeComponent();
            intervalClocker = new IntervalClocker(timer1.Interval);
            {
                save = DataSaver.loadFromPath(@"CharamakerSave", false);
                LastLoad = save.unpackDataS("LastLoad", @"yoshino");

                MotionString = save.unpackDataS("MotionString", "");
                LoadAnim = save.unpackDataS("LoadAnim", LoadAnim);
                AnimStartTime = save.unpackDataF("AnimStartTime", AnimStartTime);
                AnimEndTime = save.unpackDataF("AnimEndTime", AnimEndTime);
                Debug.WriteLine("SaveData " + save.getData());
                ChangeRootpath(save.unpackDataS("rootpath", @".\"));
                LoadCharacter(save.unpackDataS("LastLoad", @"yoshino"));
            }

            ClientSize = BaseSize;



            inp = new NameInput(km);
            inp.Bind("CamSlide", new IButton(MouseButtons.XButton1));
            inp.Bind("SelSlide", new IButton(MouseButtons.XButton2));


            display = new Display(this, 1f);

            FileMan.SetDefaultDisplay(display);


            FileMan.SoundSetUP();


            cam = display.makeCamera(new ColorC(0, 0.8f, 0.9f, 1));
            cam.watchRect.add(w);
            /*{
                //カメラテスト
                cam2 = display.makeBitmapCamera(new ColorC(1, 1, 1, 1));
                cam2.watchRect.add(w2);
                cam2.e.add(w);
                new DRectangle(10,new ColorC(1,0,0,0.5f)).add(cam2.watchRect);
                EntityMove.ScaleChange(10, "", 0.5f, 0.5f).addAndRemove(cam2.e, 100);
            }*/
            ResetLoadDatas();
            /*
            if (Directory.Exists(@".\character"))
            {
                string[] filesM = System.IO.Directory.GetFiles(@".\character", "*.ctc", System.IO.SearchOption.AllDirectories);
                for (int i = 0; i < filesM.Count(); i++)
                {
                    Console.WriteLine(filesM[i]);
                    var d=DataSaver.loadFromPath(filesM[i],ext:".ctc");

                    var e=Entity.ToLoadEntity(d);
                    e.getcompos<Character>()[0].SetBaseCharacter();
                    e.ToSave().saveToPath(filesM[i],ext:".ctc");
                }
            }*/

            {

                SLP = Texture.make(9999999, 1, new KeyValuePair<string, string>("def", "redbit"));
                TXYP = Texture.make(99999999, 1, new KeyValuePair<string, string>("def", "bluebit"));


                SLP.add(new Entity());
                TXYP.add(new Entity());



                SLP.e.add(w);
                TXYP.e.add(w);
            }/*
            {
                var c = Entity.ToLoadEntity(DataSaver.loadFromPath(@".\character\yoshino", ext: ".ctc"));

                c.settxy(0, 0);

                c.add(w);

                Select(c);

            }*/

            /*
            {
                var text = new Text(10, new ColorC(0, 0, 0, 1), "ザ・カバ・チャン"
                   , new FontC(16, 16 * 40, 16 * 30, isBold: 0, alignment: FontC.alignment.left
                   , alignmentV: FontC.alignment.right));
                text.add(cam.watchRect);


                text.font.hutiZure = 0.05f;
                text.font.hutiColor = new ColorC(1, 1, 1, 1);
                text.updated += (aa, bb) =>
                {
                    text.text.TextSource = inp.Replace(FP.l.GT("CamSlide")) + "\n" + inp.Replace(FP.l.GT("SelSlide"));
                    //  cam.watchRect.degree += 0.1f * bb;
                };

            }*/

            /*//セリフのテスト
            {
                var se=Serif.MakeSerif(Entity.make2(0, 0, 64, 64), new Charamaker3.Text(1, new ColorC(0, 0, 0, 1), "", new FontC(16, 64, 64, alignment: FontC.alignment.center, alignmentV:FontC.alignment.right)));
                DrawableMove.SetText(0,"Text", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa").add(se);
                se.WakuHaba = 5;
                se.add(w);
                se.Target = cam.watchRect;
            }
            */

            w.classifyed += (aa, bb) =>
            {
                if (bb.getcompos<Character>().Count > 0)
                {
                    w.addEdic("character", bb, 0);
                }
            };
            {
                zoomUD_ValueChanged(null, null);
            }

        }
        private void ticked(object sender, EventArgs e)
        {
            intervalClocker.Stop();
            float cl = intervalClocker.CL;
            // w = new World();

            {
                string selectchara = "Select:";
                if (sel != null && sel.c != null)
                {
                    selectchara += sel.c.e.name;
                }
                System.Diagnostics.Process proc = System.Diagnostics.Process.GetCurrentProcess();
                proc.Refresh();
                this.Text = selectchara + " " + cam.watchRect.gettxy() + " a " + display.TextRenderesNum + " to " 
                    + display.TextRenderesRemoveNum+ $"memory:{proc.WorkingSet64:N0}[]{proc.VirtualMemorySize64:N0}"
                    + " cl:" + intervalClocker.CL + $"({intervalClocker.NowFps})";
            }
            km.setpointer(this);

            {
                sel?.setPoints(SLP, TXYP, (float)PointB.Value / 100f);
            }


            cl *= SpeedBar.Value / (float)SpeedBar.Maximum;

            w.update(cl);
            w2.update(cl);
            display.draw(cl);

            w.AfterDrawUpdate();
            w2.AfterDrawUpdate();
            anmE?.AfterDrawupdate();

            moveCamera();
            moveSelected();
            selectCharacter();


            //SEのテスト
            if (km.ok(new IButton(MouseButtons.Left), itype.down))
            {


                var sc = SoundComponent.MakeSE(FileMan.SE, @"TB\jett", 0.11f);
                var m = EntityMove.MakeMotion();
                m.addmove(sc);
                m.add(w.staticEntity);

                var FXY = km.GetCursourPoint(cam);
                if (1 == 0)
                {
                    var eff = Character.MakeCharacter(@"effects\sunbit", FXY.x, FXY.y, 64);

                    DrawableMove.BaseColorChange(30, "", 0, go: goOption.goAll).add(eff);
                    EntityMove.XYD(30, "", 0, 0, 360 * 35).add(eff);
                    var lifetimer = new LifeTimer(30);
                    lifetimer.add(eff);




                    eff.add(w);
                    //FileMan.SoundEffect.playoto(@"TB\jett");
                }
            }
            //BGMのテスト
            if (km.ok(new IButton(MouseButtons.Right), itype.down))
            {
                var lis = w.staticEntity.getcompos<SoundComponent>(SoundComponent.BGMname);
                if (lis.Count == 0)
                {
                    var newbgm = SoundComponent.MakeBGM(FileMan.BGM, @"BGM\inami2", 0.5f, 30, 30);
                    newbgm.updated += (aa, bb) =>
                    {
                        var len = Mathf.abs((sel.c.e.gettxy() - cam.watchRect.gettxy()).length / cam.watchRect.bigs);

                        len = Mathf.max(1 - len, 0);
                        newbgm.volume = len;
                    };
                    if (lis.Count > 0)
                    {

                        var oldbgm = lis[0];
                        if (oldbgm.sound.path != newbgm.sound.path)
                        {
                            oldbgm.Stop();
                            oldbgm.afters.Clear();
                            oldbgm.afters.Add(newbgm);
                        }
                    }
                    else
                    {
                        newbgm.add(w.staticEntity);
                    }
                }
                else if (lis[0].sound.path == @"BGM\inami2")
                {
                    var newbgm = SoundComponent.MakeBGM(FileMan.BGM, @"BGM\tim", 0.5f, 30, 30);
                    newbgm.updated += (aa, bb) =>
                    {

                        var len = Mathf.abs((sel.c.e.gettxy() - cam.watchRect.gettxy()).length / cam.watchRect.bigs);

                        len = Mathf.max(1 - len, 0);
                        newbgm.volume = len;
                    };
                    if (lis.Count > 0)
                    {

                        var oldbgm = lis[0];
                        if (oldbgm.sound.path != newbgm.sound.path)
                        {
                            oldbgm.Stop();
                            oldbgm.afters.Clear();
                            oldbgm.afters.Add(newbgm);
                        }
                    }
                    else
                    {
                        newbgm.add(w.staticEntity);
                    }
                }
                else
                {
                    lis[0].Stop();
                }
            }


            km.topre();
            if (BaseC != null)
            {
                BaseC.e.settxy(sel.c.e.gettxy(), BaseC.e.w * 3, -BaseC.e.h * -3);
            }
           
        }
        void selectCharacter()
        {
            FXY p = km.GetCursourPoint(cam, true);
            if (km.ok(new IButton(MouseButtons.Left), itype.down))
            {
                sel.selectbyPoint(p.x, p.y);
                if (sel.e == null)
                {
                    Select();
                    var lis = w.getEdic("character");
                    lis.Remove(sel.c.e);
                    foreach (var a in lis)
                    {
                        var rec = new Charamaker3.Shapes.Rectangle(0);
                        rec.setto(a);
                        if (rec.onhani(p.x, p.y) && a != BaseC.e)
                        {
                            Select(a);
                        }
                    }
                }
                else
                {
                    Select();
                }
            }
        }


        FXY startcursorC = new FXY(0, 0), startCameraFXY = new FXY(0, 0);
        void moveCamera()
        {

            FXY p = new FXY(km.x, km.y);
            if (inp.ok("CamSlide", itype.ing))
            {
                if (inp.ok("CamSlide", itype.down))
                {
                    startcursorC = new FXY(p.x, p.y);
                    startCameraFXY = cam.watchRect.gettxy();

                }
                if (inp.ok("CamSlide", itype.ing))
                {
                    var d = new FXY(p.x, p.y) - startcursorC;

                    cam.watchRect.settxy(startCameraFXY
                        , (d.x + 0.5f) * cam.e.w, (d.y + 0.5f) * cam.e.h);

                }
            }

        }
        void moveSelected()
        {

            FXY p = km.GetCursourPoint(cam, true);
            if (inp.ok("SelSlide", itype.ing))
            {
                sel?.c.e.settxy(p.x, p.y);
                Select();
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void domainUpDown1_SelectedItemChanged(object sender, EventArgs e)
        {

        }
        public CharacterSelecter sel;
        public Character BaseC = null;

        private void zoomUD_ValueChanged(object sender, EventArgs e)
        {
            var txy = cam.watchRect.gettxy();
            cam.watchRect.w = display.width / (float)zoomUD.Value;
            cam.watchRect.h = display.height / (float)zoomUD.Value;

            cam.watchRect.settxy(txy);
            {
                TXYP.e.w = cam.watchRect.w / 100;
                TXYP.e.h = cam.watchRect.w / 100;

                TXYP.e.tx = TXYP.e.w / 2;
                TXYP.e.ty = TXYP.e.w / 2;
            }
        }
        /// <summary>
        /// DSB(DataSaverのボックスをセットする)
        /// </summary>
        /// <param name="s"></param>
        void setDSB(string s)
        {
            var cursour = DSB.SelectionStart;
            var splits = new List<string>(s.Split('\n'));
            for (int i = splits.Count - 1; i >= 0; i--)
            {
                if (splits[i].Replace(" ", "").Length == 0)
                {
                    splits.RemoveAt(i);
                }
            }
            string ss = "";
            foreach (var a in splits)
            {
                ss += a + "\n";
            }
            DSB.Text = ss;

            DSB.Text = DSB.Text.Replace("\n", Environment.NewLine);

            //カーソルの位置をもとの所へ
            DSB.SelectionStart = cursour;


        }


        void BaseSet()
        {
            if (BaseC != null)
            {
                BaseC.e.remove();
            }
            if (sel.c != sel.c.BaseCharacter)
            {
                BaseC = sel.c.BaseCharacter;
                BaseC.e.add(w);
            }
            else
            {
                BaseC = null;
            }
        }
        void Select()
        {
            setDSB(sel.getData());
            BaseSet();
            if (sel.e != null)
            {
                var texs = sel.e.getcompos<Texture>();
                if (texs.Count > 0)
                {

                    this.texturelabel.Text = cam.d.texSize(this.texturelabel.Text = texs[0].nowtex).ToString();
                }
            }
        }
        void Select(Entity e)
        {
            Select(new CharacterSelecter(e));
        }
        void Select(CharacterSelecter e)
        {
            sel = e;
            Select();
        }


        private void saveB_Click(object sender, EventArgs e)
        {
            var path = FileMan.dialog("character", ".ctc");
            var temp = FileMan.s_rootpath;//ダイアログなのでルートパスを外す
            FileMan.s_rootpath = "";
            sel.c.e.ToSave().saveToPath(path, ".ctc");
            FileMan.s_rootpath = temp;
        }

        private void loadB_Click(object sender, EventArgs e)
        {
            if (km.ok(new IButton(MouseButtons.Right), itype.ing))
            {
                km.up(new IButton(MouseButtons.Right));
                var d = DataSaver.loadFromPath(@".\character\" + textB.Text, ext: ".c3c");
                var newe = Character.ToloadC2(d).e;
                if (!float.IsNaN(newe.x))
                {
                    newe.add(w);
                    Select(newe); ;
                }
                else
                {
                    messageB.Text = textB.Text + Environment.NewLine + "は存在しないか、キャラクターのファイルではない！！！";
                }
            }
            else
            {
                LoadCharacter(textB.Text);
                //var d = DataSaver.loadFromPath(@".\character\" + textB.Text, ext: ".ctc");

            }
        }

        private void removeB_Click(object sender, EventArgs e)
        {
            if (sel != null && sel.c != null && sel.c.e != null && !sel.c.e.added)
            {
                sel.c.e.add(w);
            }
            else
            {
                sel?.remove();
            }
        }


        private void DSB_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                var news = sel.setData(DSB.Text);

                sel.remove();
                news.add(w);

                messageB.Text = news.getData();

                //news.selectbyOld(sel);
                Select(news);

            }
        }



        public string MotionString = "";
        private void motionB_Click(object sender, EventArgs e)
        {
            /*
            if (km.ok(MouseButtons.Right, itype.ing)||1==1)
            {
                km.up(MouseButtons.Right);
                var m = EntityMove.ToloadM2(DataSaver.loadFromPath(@"motion\"+textB.Text, ext: ".c3m"));
                m.add(sel.c.e);
            }*/
            var mm = new MotionMaker(this);
            mm.Show();
        }

        private void setBaseB_Click(object sender, EventArgs e)
        {
            sel?.c?.SetBaseCharacter();
            BaseSet();
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            km.up(new IButton(e.Button));
        }


        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            km.down(new IButton(e.KeyCode));
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            km.up(new IButton(e.KeyCode));
        }

        private void screenshotB_Click(object sender, EventArgs e)
        {
            display.ShotThisScreen(cam);
        }


        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            km.down(new IButton(e.Button));
            //cam.watchRect.mirror = !cam.watchRect.mirror;
            //cam.watchRect.degree += 30;
        }
        private void Resized(object sender, EventArgs e)
        {

            int sum = this.ClientSize.Width + this.ClientSize.Height;
            var size = BaseSize;
            if (size.Width != 0)
                this.ClientSize = new System.Drawing.Size(sum * size.Width / (size.Width + size.Height), sum * size.Height / (size.Width + size.Height));

        }

        private void DSB_TextChanged(object sender, EventArgs e)
        {

        }

        public AnimeEditor anmE = null;

        public string LoadAnim = "";
        public float AnimStartTime = 0;
        public float AnimEndTime = 10000;
        private void animationB_Click(object sender, EventArgs e)
        {
            if (anmE == null || anmE.Visible == false)
            {
                anmE = new AnimeEditor(this);
                anmE.Show();
            }
        }

        private void printoutB_Click(object sender, EventArgs e)
        {
            if (sel != null && sel.c != null)
            {
                this.messageB.Text = sel.c.e.ToSave().getData().Replace("\n", Environment.NewLine);
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
        string LastLoad = "";
        void LoadCharacter(string load)
        {
            Debug.WriteLine(load + " LOADED");
            FileMan.loadCharacter(load);
            this.textB.Text = load;
            LastLoad = load;
            var newe = FileMan.loadCharacter(textB.Text, true);

            if (newe.getcompos<Character>().Count > 0)
            {
                newe.add(w);
                Select(newe); ;
            }
            else
            {
                messageB.Text = textB.Text + Environment.NewLine + "は存在しないか、キャラクターのファイルではない！！！";
            }

            Save();
        }
        void ChangeRootpath(string newPath)
        {

            FileMan.s_rootpath = rootpathbox.Text;
            this.rootpathbox.Text = newPath;
            Save();
        }

        /// <summary>
        /// バックアップデータをセーブ
        /// </summary>
        public void Save()
        {

            save = new DataSaver();
            save.packAdd("rootpath", FileMan.s_rootpath.Replace("\\", "/"));
            save.packAdd("LastLoad", LastLoad);
            save.packAdd("MotionString", MotionString);

            save.packAdd("LoadAnim", LoadAnim);
            save.packAdd("AnimStartTime", AnimStartTime);
            save.packAdd("AnimEndTime", AnimEndTime);

            var temp = FileMan.s_rootpath;
            FileMan.s_rootpath = "./";
            save.saveToPath(@"CharamakerSave");
            FileMan.s_rootpath = temp;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ChangeRootpath(rootpathbox.Text);
            //            FileMan.rootpath= rootpathbox.Text; 

        }

        private void selectB_Click(object sender, EventArgs e)
        {
            if (sel != null)
            {
                sel.SelectByName(this.textB.Text);
                Select();
            }
        }
        /// <summary>
        /// いろいろデータをリセットする
        /// </summary>
        public void ResetLoadDatas()
        {
            var temp = FileMan.s_rootpath;
            FileMan.s_rootpath = "./";
            FP.l.seting(textsn: new List<string> { @"texts\text.txt" });
            FileMan.s_rootpath = temp;
            FP.SetDefault(FP.l);//呼び出し元のFPを読み込む
            FileMan.LoadedDS.Clear();
            FileMan.LoadedMotion.Clear();
            cam.d.setupTextureLoader();
        }
        private void ResetTextureB_Click(object sender, EventArgs e)
        {
            ResetLoadDatas();
        }

        private void ForceMirrorButton_Click(object sender, EventArgs e)
        {
            if (sel.c != null)
            {

                foreach (var a in sel.c.getTree(""))
                {
                    a.tx = -(a.tx - a.w * 0.5f) + a.w * 0.5f;
                    foreach (var b in sel.c.getJoint(a.name))
                    {
                        b.px = -(b.px - 0.5f) + 0.5f;
                    }
                }
            }
        }

        private void CheckB_Click(object sender, EventArgs e)
        {
            if (CheckB.Text == "Check")
            {
                CheckB.Text = "Back";
                OffControls();
            }
            else
            {
                CheckB.Text = "Check";
                OnControls();
            }
        }
    }
}