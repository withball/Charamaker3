using Charamaker3;
using Charamaker3.Inputs;
using Rectangle = Charamaker3.Rectangle;

namespace Sample
{
    public partial class Form1 : Form
    {
        System.Drawing.Size BaseSize = new Size(1600, 900);
        Display display;
        KeyMouse km = new KeyMouse();
        NameInput input ;
        SceneContainer SC;
        public Form1()
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
        }


        Entity drawing = null;
        protected override void onUpdate(float cl)
        {
            base.onUpdate(cl);

            if (sc.input.ok("Tap", itype.down)) 
            {
                var FXY = sc.GetCursourPoint();
                drawing = new Entity();
                drawing.x = FXY.x;
                drawing.y = FXY.y;
                new Rectangle(1,new ColorC(1,0,0,1)).add(drawing);
                drawing.add(wol);
            }

            if (sc.input.ok("Tap", itype.ing))
            {
                var FXY = sc.GetCursourPoint();
                drawing.w = FXY.x- drawing.x;
                drawing.h = FXY.y- drawing.y;

            }

        }

    }
}