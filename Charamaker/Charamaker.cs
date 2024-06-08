using Charamaker3;
using Rectangle = Charamaker3.Rectangle;
using Charamaker3.CharaModel;
using Charamaker3.Inputs;
using Charamaker3.ParameterFile;
namespace Test
{

    public partial class Charamaker : Form
    {
        Display display;
        World w = new World();
        Camera cam;
        KeyMouse<IButton> km=new KeyMouse<IButton>();
        System.Drawing.Size BaseSize = new Size(1600, 900);

        NameInput<IButton> inp ;

        Texture SLP, TXYP;
        public Charamaker()
        {
            InitializeComponent();

            FP.l.seting(textsn:new List<string> {@"texts\text" });

            inp = new NameInput<IButton>(km);
            inp.Bind("CamSlide",new IButton(MouseButtons.XButton1));
            inp.Bind("SelSlide", new IButton(MouseButtons.XButton2));

            ClientSize = BaseSize;

            display = new Display(this,1f);

            

            FileMan.setupTextureLoader(display);
            FileMan.SoundSetUP();


            cam=display.makeCamera(new ColorC(0,0.8f,0.9f,1));
            cam.watchRect.add(w);
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

                SLP = Texture.make(9999999,1,new KeyValuePair<string, string> ("def","redbit" ));
                TXYP = Texture.make(99999999, 1, new KeyValuePair<string, string>("def", "bluebit"));


                SLP.add(new Entity());
                TXYP.add(new Entity());


           
                SLP.e.add(w);
                TXYP.e.add(w);
            }
            {
                var c=Entity.ToLoadEntity(DataSaver.loadFromPath(@".\character\yoshino",ext:".ctc"));

                c.settxy(0, 0);

                c.add(w);

                Select(c);

            }

           

            var text=new Text(10, new ColorC(0, 0, 0, 1), "ザ・カバ・チャン"
               , new FontC(16, 16 * 20, 16 * 15, isBold: false,alignment:FontC.alignment.left));
            text.add(cam.watchRect);

            text.updated += (aa, bb) =>
            {
                text.text=inp.Replace(FP.l.GT("CamSlide"))+"\n"+ inp.Replace(FP.l.GT("SelSlide"));
            };

            w.classifyed += (aa, bb) => 
            {
                if (bb.getcompos<Character>().Count>0) 
                {
                    w.addEdic("character",bb,0);
                }
            };
            {
                zoomUD_ValueChanged(null, null);
            }
        }

        private void ticked(object sender, EventArgs e)
        {
            km.setpointer(cam,this);

            {
                sel.setPoints(SLP, TXYP, (float)PointB.Value / 100f);
            }

            w.update(1);

            display.draw(1);

            moveCamera();
            moveSelected();
            selectCharacter();


            FileMan.SoundUpdate(1);
            //SEのテスト
            if (km.ok(new IButton(MouseButtons.Left), itype.down)) 
            {

                var sc =SoundComponent.MakeSE(FileMan.SoundEffect,@"TB\jett", 1);
                sc.add(w.staticEntity);

                //FileMan.SoundEffect.playoto(@"TB\jett");
            }
            //BGMのテスト
            if (km.ok(new IButton(MouseButtons.Left), itype.down))
            {

                var newbgm= SoundComponent.MakeBGM(FileMan.BGM, @"BGM\inami2", 0.5f, 30, 30);
                  var lis = w.staticEntity.getcompos<SoundComponent>(SoundComponent.BGMname);
                newbgm.updated += (aa, bb) =>
                {
                    var len = Mathf.abs((sel.c.e.gettxy() - cam.watchRect.gettxy()).length/cam.watchRect.bigs);

                    len = Mathf.max(1 -len, 0);
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
                //FileMan.BGM.playoto(@"inami2");
            }
            //BGMのテスト
            if (km.ok(new IButton(MouseButtons.Right), itype.down))
            {
                var newbgm = SoundComponent.MakeBGM(FileMan.BGM, @"BGM\tim", 0.5f, 30, 30);
                var lis = w.staticEntity.getcompos<SoundComponent>(SoundComponent.BGMname);
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


            km.topre();
            if (BaseC != null) 
            {
                BaseC.e.settxy(sel.c.e.gettxy(),BaseC.e.w*2,-BaseC.e.h*-2);
            }
            
        }
        void selectCharacter()
        {
            if (km.ok(new IButton(MouseButtons.Left), itype.down))
            {
                sel.selectbyPoint(km.x, km.y);
                if (sel.e == null)
                {
                    Select();
                    var lis = w.getEdic("character");
                    lis.Remove(sel.c.e);
                    foreach (var a in lis)
                    {
                        var rec = new Charamaker3.Shapes.Rectangle();
                        rec.setto(a);
                        if (rec.onhani(km.x, km.y)&&a!=BaseC.e)
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


        FXY startcursorC=new FXY(0,0),startCameraFXY=new FXY(0,0);
        void moveCamera() 
        {
            if (inp.ok("CamSlide", itype.ing))
            {
                if (inp.ok("CamSlide", itype.down))
                {
                    startcursorC = new FXY(KeyMouse<IButton>.raw.x, KeyMouse<IButton>.raw.y);
                    startCameraFXY = cam.watchRect.gettxy();
                }
                if (inp.ok("CamSlide", itype.ing))
                {
                    var d = new FXY(KeyMouse<IButton>.raw.x, KeyMouse<IButton>.raw.y) - startcursorC;
                    cam.watchRect.settxy(startCameraFXY
                        , (d.x + 0.5f) * cam.e.w, (d.y + 0.5f) * cam.e.h);

                }
            }

        }
        void moveSelected()
        {
            if (inp.ok("SelSlide", itype.ing))
            {
                sel.c.e.settxy(km.x,km.y);
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
        public Character BaseC=null;

        private void zoomUD_ValueChanged(object sender, EventArgs e)
        {
            var txy = cam.watchRect.gettxy();
            cam.watchRect.w = display.width/(float)zoomUD.Value;
            cam.watchRect.h = display.height / (float)zoomUD.Value;

            cam.watchRect.settxy(txy);
            {
                TXYP.e.w = cam.watchRect.w / 100;
                TXYP.e.h = cam.watchRect.w / 100;

                TXYP.e.tx = TXYP.e.w / 2;
                TXYP.e.ty = TXYP.e.w / 2;
            }
        }
        void setDSB(string s) 
        {
            var cursour=DSB.SelectionStart;
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
            DSB.SelectionStart=cursour;
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
            var path=FileMan.dialog("character",".ctc");
            sel.c.e.ToSave().saveToPath(path,".ctc");
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
                var d = DataSaver.loadFromPath(@".\character\" + textB.Text, ext: ".ctc");
                var newe = Entity.ToLoadEntity(d);
                if (newe.getcompos<Character>().Count > 0)
                {
                    newe.add(w);
                    Select(newe); ;
                }
                else
                {
                    messageB.Text = textB.Text + Environment.NewLine + "は存在しないか、キャラクターのファイルではない！！！";
                }
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
            display.ShotThisScreen();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            km.down(new IButton(e.Button));
        }
        private void Resized(object sender, EventArgs e)
        {

            int sum = this.ClientSize.Width + this.ClientSize.Height;
            var size = BaseSize;
            if (size.Width != 0)
                this.ClientSize = new System.Drawing.Size(sum * size.Width / (size.Width + size.Height), sum * size.Height / (size.Width + size.Height));

        }
    }
}