using Charamaker3;
using Charamaker3.Inputs;
using Charamaker3.Shapes;
using Charamaker3.Hitboxs;
using Rectangle = Charamaker3.Shapes.Rectangle;
using Charamaker3.ParameterFile;

namespace Sample
{
    public partial class SampleForm : Form
    {
        System.Drawing.Size BaseSize = new Size(1600, 900);
        Display display;
        KeyMouse km = new KeyMouse();
        NameInput input ;
        SceneContainer SC;
        public SampleForm()
        {
            InitializeComponent();

            ClientSize = BaseSize;
            display = new Display(this,1);


            FP.l.seting(textsn: new List<string> {  });

            FileMan.SetDefaultDisplay(display);
            FileMan.SoundSetUP();

            input = new NameInput(km);
            SC = new SceneContainer(display,input);
            new SampleScene(SC).Start();


            SC.input.Bind("Tap", new IButton(MouseButtons.Left));
            SC.input.Bind("Tap2", new IButton(MouseButtons.Right));

            SC.input.Bind("Slide", new IButton(MouseButtons.XButton1));
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ticked(object sender, EventArgs e)
        {
            km.setpointer(this);

            SC.nowScene.Update(1);

            km.topre();

        }



        private void KeyDowned(object sender, KeyEventArgs e)
        {
            km.down(new IButton(e.KeyCode));
        }

        private void KeyUped(object sender, KeyEventArgs e)
        {
            km.up(new IButton(e.KeyCode));
        }

        private void MouseDowned(object sender, MouseEventArgs e)
        {
            km.down(new IButton(e.Button));
        }

        private void MouseUped(object sender, MouseEventArgs e)
        {
            km.up(new IButton(e.Button));
        }
    }

    public class SampleScene :Scene
    {
        public SampleScene(SceneContainer sc) : base(sc) 
        {
            for (float i = 0; i < 1; i += 0.1f)
            {
                //後ろのものほどフェードするエフェクト
                var col=new ColorC(cam.col);
                col.opa = 0.1f;
                new DRectangle(i,col).add(cam.watchRect);
            }
        }


        Entity? drawing = null;
        FXY precursour;
        protected override void onUpdate(float cl)
        {
            base.onUpdate(cl);

            if ((sc.input.ok("Tap", itype.down)||sc.input.ok("Tap2", itype.down))&&drawing==null)
            {
                var FXY = GetCursourPoint();
                drawing = new Entity();
                drawing.x = FXY.x;
                drawing.y = FXY.y;
                drawing.w = 100;
                drawing.h = 100;

                DRectangle d ;
                if(1==1)
                {
                    var hai = new Haikei(FileMan.whrandhani(1.0f), FileMan.whrandhani(1.0f), cam);
                    hai.add(drawing);
                    hai.SousaiZahyou();//これがないとずれる 
                    d= new DRectangle((hai.px * hai.py), new ColorC(1, 0, 0
                    , (1 - hai.px * hai.py) * 0.0f + 1.0f));
                }
                else 
                {
                    d = new DRectangle(10, new ColorC(1, 0, 0, 1));
                }
                
                d.add(drawing);
                
                var hit=new Hitbox(new Rectangle(0,0),new List<int>(),new List<int>());
                hit.add(drawing);

                {

                    var a = new Entity();
                    new DRectangle(10,new ColorC(1,1,1,0.25f)).add(a);
                    a.add(wol);

                    var b = new Entity();
                    new DRectangle(10, new ColorC(0, 0, 1, 0.25f)).add(b);
                    b.add(wol);

                    hit.freeEvent += (aa, bb) =>
                    {
                        a.setto(hit.HitShape);
                        b.setto(hit.preHitShape);
                       // Debug.WriteLine(hit.HitShape.gettxy() + "  AA " + hit.preHitShape.gettxy());
                    };
                }

                var comp=new Component(-1, "kasanariiro");
                {
                    comp.updated += (aa, bb) => 
                    {
                        var hits=comp.e.getHits();

                        if (hits.Count > 0)
                        {
                            d.col.r = 0;
                        }
                        else 
                        {
                            d.col.r = 1;
                        }
                    };
                }
                comp.add(drawing);
                
                drawing.add(wol);

            }
            if ((sc.input.ok("Tap", itype.ing)|| sc.input.ok("Tap2", itype.ing))&&drawing!=null)
            {
                var FXY = GetCursourPoint();
                drawing.w = FXY.x - drawing.x;
                drawing.h = FXY.y - drawing.y;

            }
            if (sc.input.ok("Tap2", itype.up)&&drawing!=null) 
            {
                var p=new PhysicsComp(1, 0.01f, 0.9f, 0.1f, 0, 0, 1, 0);
                p.onHansya += (aa, bb) => 
                {
                  //  Debug.WriteLine(p.vx+" :vxy: "+p.vy);
                };
                p.add(drawing);
                drawing = null;
            }
            if (sc.input.ok("Tap", itype.up) && drawing != null)
            {

                new PhysicsComp(PhysicsComp.MW, 0, 0f, 0f, 0, 0, 0, 0).add(drawing);
                drawing = null;
            }

            if (sc.input.ok("Slide", itype.down))
            {
                precursour = new FXY(sc.input.input.x,sc.input.input.y);
            }
            if (sc.input.ok("Slide", itype.ing))
            {
                var now = new FXY(sc.input.input.x, sc.input.input.y);
                cam.watchRect.x += (now-precursour).x*cam.watchRect.w;
                cam.watchRect.y += (now - precursour).y * cam.watchRect.h;

                precursour = now;
            }
        }

    }
}