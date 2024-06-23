using Charamaker3;
using Charamaker3.Inputs;

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

            FileMan.setupTextureLoader(display);
            FileMan.SoundSetUP();

            input = new NameInput(km);
            SC = new SceneContainer(display,input);
            new SampleScene(SC).Start();


            SC.input.Bind("Tap", new IButton(MouseButtons.Left));

            SC.input.Bind("Slide", new IButton(MouseButtons.Right));
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


        Entity drawing = null;
        FXY precursour;
        protected override void onUpdate(float cl)
        {
            base.onUpdate(cl);

            if (sc.input.ok("Tap", itype.down))
            {
                var FXY = GetCursourPoint();
                drawing = new Entity();
                drawing.x = FXY.x;
                drawing.y = FXY.y;
                var hai = new Haikei(FileMan.whrandhani(1.0f), FileMan.whrandhani(1.0f),cam);
                hai.add(drawing);
                hai.SousaiZahyou();//これがないとずれる 

                
                new DRectangle((hai.px * hai.py) , new ColorC(1, 0, 0
                    , (1-hai.px * hai.py)*0.0f + 1.0f)).add(drawing);
                
                drawing.add(wol);

            }
            if (sc.input.ok("Tap", itype.ing))
            {
                var FXY = GetCursourPoint();
                drawing.w = FXY.x - drawing.x;
                drawing.h = FXY.y - drawing.y;

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