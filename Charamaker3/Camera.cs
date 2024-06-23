using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vortice.Direct2D1;
using Vortice.Mathematics;
using System.Drawing;
using System.Numerics;
using Vortice;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.IO.Compression;
using Vortice.DirectWrite;

namespace Charamaker3
{
    #region cameraUtils
    /// <summary>
    /// カメラの動きに対してどのぐらい動くかを変化させる。無理やりね。<br></br>
    /// これによってe.setTxとかもおかしくなる。
    /// </summary>
    public class Haikei :Component
    {
        /// <summary>
        /// カメラが動くとこのエンテティが動く割合。
        /// </summary>
        public float px = 1, py = 1;

        /// <summary>
        /// 背景っぽく動かす
        /// </summary>
        /// <param name="px">x方向のスクロール割合</param>
        /// <param name="py">y方向のスクロール割合</param>
        /// <param name="cam">追従するカメラ。nullでも後で設定すればいい</param>
        public Haikei(float px, float py,Camera cam) 
        {
            this.px = px;
            this.py = py;
            this.cam = cam;
        }
        public Haikei() { }

        public override void copy(Component c)
        {
            var cc = (Haikei)c;
            base.copy(c);
            cc.px = this.px;
            cc.py = this.py;

            cc.cam = this.cam;
        }
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.linechange();
            d.packAdd("px", px);
            d.packAdd("py", py);
            return d;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.px = d.unpackDataF("px");
            this.py = d.unpackDataF("py");
        }
        /// <summary>
        /// 相対位置のカメラ
        /// </summary>
        protected Camera cam=null;
        /// <summary>
        /// 本来の位置
        /// </summary>
        protected float truex, truey;

        /// <summary>
        /// 前回のの位置
        /// </summary>
        protected float prex, prey;
        public override void update(float cl)
        {
            base.update(cl);
            if (onWorld)
            {
                setZahyou(false);
            }
        }
        public override void addtoworld(float cl = 0)
        {
            base.addtoworld(cl);
            setZahyou(true);
           // Debug.WriteLine("Haikei Add to world");
        }
        public override void removetoworld(float cl = 0)
        {
            base.removetoworld(cl);
            e.x = truex;
            e.y = truey;
           // Debug.WriteLine("Haikei REMOVE to world");
        }

        /// <summary>
        /// 追従するカメラをセッティングする
        /// </summary>
        /// <param name="cam"></param>
        public void setCamera(Camera cam) 
        {
            this.cam = cam;

        }
        /// <summary>
        /// カメラを考慮した座標をセットする<br></br>
        /// </summary>
        /// <param name="OnStart">初めて登場した時にtrueにする</param>
        /// <exception cref="Exception"></exception>
         void setZahyou(bool OnStart) 
        {
            if (cam == null)
            {
                throw new Exception("追従するカメラが無いですやん");
            }
            else if (OnStart)
            {
                var center = cam.watchRect.gettxy();
                truex = e.x;
                truey = e.y;
                prex = e.x + center.x * (px - 1);
                prey = e.y + center.y * (py - 1);
                e.x = prex;
                e.y = prey;

            }
            else
            {
                var center = cam.watchRect.gettxy();
                truex += e.x - prex;
                truey += e.y - prey;
                prex = truex + center.x * (px - 1);
                prey = truey + center.y * (py - 1);
                e.x = prex;
                e.y = prey;
            }
        }
        /// <summary>
        /// e.add(world)前に呼びだせば、カメラ座標と一致させることができる(ごめん言葉ではうまく説明できない。)<br></br>
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void SousaiZahyou()
        {
            if (cam == null)
            {
                throw new Exception("追従するカメラが無いですやん");
            }
            else 
            {
                var center = cam.watchRect.gettxy();
       
                e.x = e.x - center.x * (px - 1);
                e.y = e.y - center.y * (py - 1);
                
            }
       
        }
    }
    #endregion
    #region cameraAndDrawing
    public class ColorC
    {
        public float r, g, b, opa;
        public ColorC(float r, float g, float b, float opa)
        {
            this.r = r; this.g = g; this.b = b; this.opa = opa;
        }
        public ColorC(ColorC c)
        {
            this.r = c.r; this.g = c.g; this.b = c.b; this.opa = c.opa;
        }
        public static implicit operator Color4(ColorC d)
        {
            return new Color4(d.r, d.g, d.b, d.opa);
        }

        public static bool operator ==(ColorC a, ColorC b)
        {
            if(ReferenceEquals(a,null)|| ReferenceEquals(b,null))
            {
                return ReferenceEquals(a, null) == ReferenceEquals(b, null);
            }

            return a.r == b.r && a.g == b.g && a.b == b.b && a.opa == b.opa;
        }
        public static bool operator !=(ColorC a, ColorC b)
        {
            if (ReferenceEquals(a,null)|| ReferenceEquals(b,null)) 
            {
                return ReferenceEquals(a, null) != ReferenceEquals(b, null);
            }
            return a.r != b.r || a.g != b.g || a.b != b.b || a.opa != b.opa;
        }

    }
    public abstract class Drawable : Component 
    {
        /// <summary>
        /// 色。大体透明度しか意味ない。
        /// </summary>
        public ColorC col=new ColorC(0,0,0,0);
        //描画の順番
        public float z;
        public Drawable(float z,ColorC col, float time = -1, string name = ""):base(time,name)
        {
            this.z = z;
            this.col = col;
        }

        public Drawable() : base()
        {
        }


        public override void copy(Component c)
        {
            var cc = (Drawable)c;
            base.copy(c);
            cc.z = this.z;
            cc.col = new ColorC(this.col);
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("z", this.z); 
            res.linechange();
            res.packAdd("r", this.col.r);
            res.packAdd("g", this.col.g);
            res.packAdd("b", this.col.b);
            res.packAdd("opa", this.col.opa);
            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.z = d.unpackDataF("z");
            this.col.r = d.unpackDataF("r");
            this.col.g = d.unpackDataF("g");
            this.col.b = d.unpackDataF("b");
            this.col.opa = d.unpackDataF("opa");
        }
        /// <summary>
        /// 描画をする
        /// </summary>
        /// <param name="cam"></param>
        /// <returns>描画したか(カメラ範囲外とかだと描画しない)</returns>
        public bool goDraw(Camera cam) 
        {
            if (col.opa > 0&&onCamera(cam))
            {
                draw(cam);
                return true;
            }
            return false;
        }

        protected abstract void draw(Camera cam);
        
        protected bool onCamera(Camera cam) 
        {
            var s=new Shapes.Rectangle();
            s.setto(cam.watchRect);


            var s2 = new Shapes.Rectangle();
            s2.setto(this.e);
            return s.atarun(s2);
        }
        
        /// <summary>
        /// カメラからの相対座標
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="xy"></param>
        /// <returns></returns>
        protected FXY camsoutai(Camera cam, FXY xy)
        {
            var watch = cam.watchRect;
            var camupleft = new FXY(watch.x, watch.y);
            

            var res = new FXY(xy.x, xy.y) - camupleft;
            res.degree -= watch.degree;
            res.x *= cam.resolx;
            res.y *= cam.resoly;
           
            return res;
        }
        /// <summary>
        /// 四角形の標準トランスフォーム
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        protected Matrix3x2 rectTrans(Camera cam)
        {

            var watch = cam.watchRect;
          

            //  左上
            var upleft = camsoutai(cam,e.gettxy(0, 0));

            //0.5の中心
            var center = camsoutai(cam, e.gettxy(e.w / 2, e.h / 2));
            //回転の合計
            var rad = Mathf.toRadian(-watch.degree + e.degree);
            var a = Matrix3x2.CreateScale(1, 1);

            if (watch.mirror)
            {
                a = Matrix3x2.Multiply(Matrix3x2.CreateScale(-1, 1), a);

                a = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(-Mathf.abs(watch.w) * 1, 0), a);
            }
           // Debug.WriteLine("wacthrect w "+watch.w+" upleft " +upleft.ToString());
            if (watch.w<0)
            {
                a = Matrix3x2.Multiply(Matrix3x2.CreateScale(-1, 1), a);

            }
            if (watch.h<0)
            {
                a = Matrix3x2.Multiply(Matrix3x2.CreateScale(1, -1), a);

            }

            a = Matrix3x2.Multiply(Matrix3x2.CreateRotation((float)rad, new Vector2(upleft.x, upleft.y)), a);


            if (e.mirror)
            {
                a = Matrix3x2.Multiply(new Matrix3x2(-1, 0, 0, 1, 0, 0), a);
                a = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(-(upleft.x) * 2, 0), a);
            }
            if (e.w < 0)
            {
                a = Matrix3x2.Multiply(new Matrix3x2(-1, 0, 0, 1, 0, 0), a);

                a = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(-(upleft.x) * 2, 0), a);
            }
        

            if (e.h < 0)
            {
                a = Matrix3x2.Multiply(new Matrix3x2(1, 0, 0, -1, 0, 0), a);

                a = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(0, -(upleft.y) * 2), a);
            }

          

            return a;
        }

        /// <summary>
        /// 四角形の標準描画範囲
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        protected RawRectF rectRectF(Camera cam)
        {

            var upleft = camsoutai(cam, e.gettxy(0, 0));
            //wも、hも絶対値で四角形を作り、transformの所で反転させて折り合いをつける。

            var w = Mathf.sameSign((camsoutai(cam, e.gettxy(e.w, 0))-upleft).length,e.w*0+1);
            var h = Mathf.sameSign((camsoutai(cam, e.gettxy(0, e.h)) - upleft).length, e.h*0+1);


            return new RectangleF(upleft.x, upleft.y, w, h);
        }


    }
    /// <summary>
    /// 描画可能な四角形
    /// </summary>
    public class DRectangle : Drawable
    {
        public DRectangle(float z,ColorC c, float time = -1,string name="") : base(z,c,time,name) 
        {
        }
        public DRectangle() : base()
        {
        }
        protected override void draw(Camera cam)
        {
            var render=cam.render;
            
            render.Transform =rectTrans(cam);
            var rect = rectRectF(cam);
      
            using (var brh = render.CreateSolidColorBrush(col))
            {
                render.FillRectangle(rect, brh);
                //render.DrawLine(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top/2+rect.Bottom/2),brh,20);
                //render.DrawLine(new PointF(rect.Left, rect.Bottom), new PointF(rect.Right, rect.Top / 2 + rect.Bottom / 2), brh, 20);

                //render.DrawLine(new PointF(rect.Left, rect.Top), new PointF(rect.Left, rect.Bottom), brh, 20);
            }

        }
    }

    public class Texture : Drawable
    {
        /// <summary>
        /// 登録されているテクスチャ―
        /// </summary>
        public Dictionary<string, string> textures=new Dictionary<string, string>();
        /// <summary>
        /// 今選択されているテクスチャの名前
        /// </summary>
        public string texname="def";

        public Texture(float z, ColorC c, Dictionary<string,string> texs,float time = -1, string name = "") : base(z, c, time, name)
        {
            textures = new Dictionary<string, string>(texs);
            if (textures.Keys.Count > 0)
            {
                texname = new List<string>(textures.Keys)[0];
            }
        }
        public Texture() : base() 
        {}

        public override void copy(Component c)
        {
            var cc = (Texture)c;
            base.copy(c);
            cc.texname=this.texname;
            cc.textures = new Dictionary<string, string>(textures);
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("texname",texname);
            res.linechange();
            var d = new DataSaver();
            foreach (var a in textures) 
            {
                d.packAdd(a.Key, a.Value);
                d.linechange();
            }
            d.indent();
            res.packAdd("textures", d);
            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.texname = d.unpackDataS("texname");

            var textures = new Dictionary<string, string>();
            var dd = d.unpackDataD("textures");
            foreach (var a in dd.getAllPacks()) 
            {
                textures.Add(a, dd.unpackDataS(a));
            }
            this.textures = textures;
        }
        static public Texture make(float z,float opa, params KeyValuePair<string,string> [] textures) 
        {
            var res=new Texture(z, new ColorC(1, 1, 1, opa),new Dictionary<string, string>());
            foreach (var a in textures) 
            {
                res.textures.Add(a.Key,a.Value);
            }
            if (textures.Length > 0) 
            {
                res.texname = textures[0].Key;
            }
            return res;
        }


        protected override void draw(Camera cam)
        {
            var render = cam.render;

            ID2D1Bitmap bitmap;
            if (!textures.ContainsKey(texname))
            {
                bitmap = FileMan.ldtex("nothing");
            }
            else
            {
                bitmap = FileMan.ldtex(textures[texname]);
            }
            render.Transform = rectTrans(cam);
            var rect = rectRectF(cam);


            render.DrawBitmap(bitmap
                   ,rect
                   , this.col.opa, BitmapInterpolationMode.Linear
                   , new RawRectF(0, 0, bitmap.Size.Width, bitmap.Size.Height));


        }

    }

    #region Text
    /// <summary>
    /// フォントを扱うクラス
    /// </summary>
    public class FontC 
    {
        public enum alignment 
        {
            /// <summary>
            /// 左っかわ
            /// </summary>
            left,
            /// <summary>
            /// 中央
            /// </summary>
            center,
            /// <summary>
            /// 右っかわ
            /// </summary>
            right,
            /// <summary>
            /// 謎。わからない。
            /// </summary>
            justify
        }

        /// <summary>
        /// フォントの名前
        /// </summary>
        public string fontName= "MS UI Gothic";

        /// <summary>
        /// 斜体
        /// </summary>
        public bool isItaric = false;

        /// <summary>
        /// 太字
        /// </summary>
        public bool isBold=false;
        /// <summary>
        /// フォントサイズ
        /// </summary>
        public float size = 16;

        /// <summary>
        /// 文字を描画できる範囲描画の範囲
        /// </summary>
        public float w, h;

        /// <summary>
        /// テキストの横のアライメント
        /// </summary>
        public alignment ali= alignment.left;


        /// <summary>
        /// テキストの縦のアライメントあんま信用しないで。
        /// </summary>
        public alignment aliV = alignment.left;

        /// <summary>
        /// フォントのコンストラクタ
        /// </summary>
        /// <param name="size">フォントサイズ</param>
        /// <param name="w">文字を書き込める範囲</param>
        /// <param name="h">文字を書き込める範囲</param>
        /// <param name="fontName">フォントの名前</param>
        /// <param name="isItaric">斜体</param>
        /// <param name="isBold">太字</param>
        /// <param name="alignment">テキストの位置</param>
        /// <param name="alignmentV">テキストの縦の位置</param>
        public FontC(float size,float w,float h,string fontName="MS UI Gothic"
            ,bool isItaric=false,bool isBold=false,alignment alignment=alignment.left
            , alignment alignmentV = alignment.left) 
        {
            this.size= size;
            this.fontName = fontName;
            this.isItaric= isItaric;
            this.isBold = isBold;
            this.w = w;
            this.h = h;
            this.ali = alignment;
            this.aliV = alignmentV;
        }
        /// <summary>
        /// カラコン
        /// </summary>
        public FontC() { }
    
        /// <summary>
        /// フォントをコピーする
        /// </summary>
        /// <param name="c">コピー先</param>
        public void copy(FontC c)
        {
            c.size=this.size;
            c.w = this.w;
            c.h = this.h;
            c.fontName=this.fontName;
            c.isItaric=this.isItaric;
            c.isBold =this.isBold;
            c.ali = this.ali;
            c.aliV = this.aliV;
        }

        public DataSaver ToSave() 
        {
            var d = new DataSaver();
            d.packAdd("size", size);
            d.packAdd("w", w);
            d.packAdd("h", h);
            d.linechange();
            d.packAdd("fontName", fontName);
            d.packAdd("isItaric", isItaric);
            d.packAdd("isBold", isBold);
            d.packAdd("alignment", ali);
            d.packAdd("alignmentV", aliV);
            return d;
        }
        public void ToLoad(DataSaver d) 
        {
            this.size = d.unpackDataF("size");
            this.w = d.unpackDataF("w");
            this.h = d.unpackDataF("h");
            this.fontName = d.unpackDataS("fontName");
            this.isItaric = d.unpackDataB("isItaric");
            this.isBold = d.unpackDataB("isBold");
            this.ali = (alignment)d.unpackDataF("alignment");
            this.aliV = (alignment)d.unpackDataF("alignmentV");
        }
        /// <summary>
        /// ちゃんとしたフォントに変える
        /// </summary>
        /// <returns></returns>
        public IDWriteTextFormat ToFont()
        {
            var fa = Vortice.DirectWrite.DWrite.DWriteCreateFactory<IDWriteFactory>();

            Vortice.DirectWrite.FontStyle style=Vortice.DirectWrite.FontStyle.Normal;
            if (isItaric) 
            {
            style = Vortice.DirectWrite.FontStyle.Italic;
            }
            var Weight = FontWeight.Light;
            if (isBold)
            {
                Weight=FontWeight.UltraBold;
            }
            float size = this.size;
            var fom = fa.CreateTextFormat(fontName, Weight, style, size);

            switch (ali)
            {
                case alignment.left:
                    fom.TextAlignment = TextAlignment.Leading;
                    break;
                case alignment.center:
                    fom.TextAlignment = TextAlignment.Center;
                    break;
                case alignment.right:
                    fom.TextAlignment = TextAlignment.Trailing;
                    break;
                case alignment.justify:
                    fom.TextAlignment = TextAlignment.Justified;
                    break;
                default:
                    fom.TextAlignment = TextAlignment.Leading;
                    break;
            }
            return fom;
        }
        static public bool operator ==(FontC a, FontC b)
        {
           
            if (ReferenceEquals(a ,null)|| ReferenceEquals(b ,null)) 
            {
                return ReferenceEquals(a,null) == ReferenceEquals(b,null);
            }

            return a.size == b.size && a.w == b.w && a.h == b.h &&
                a.fontName == b.fontName && a.isItaric == b.isItaric
                && a.isBold == b.isBold;
        }
        static public bool operator !=(FontC a, FontC b)
        {
            if (ReferenceEquals(a,null) || ReferenceEquals(b,null))
            {
                return ReferenceEquals(a,null) != ReferenceEquals(b,null);
            }


            return a.size != b.size && a.w != b.w || a.h != b.h ||
                a.fontName != b.fontName || a.isItaric != b.isItaric
                || a.isBold != b.isBold;
        }
    }

    /// <summary>
    /// bmp上の領域を確保して文字を書くためのクラス
    /// </summary>
    class TextRenderer
    {
        internal ID2D1BitmapRenderTarget render;

        /// <summary>
        /// Rectangleだが、回転は無視される。
        /// </summary>
        public Shapes.Rectangle rendZone;

        bool NoChange = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="render"></param>
        /// <param name="drawto">回転は無しね</param>
        public TextRenderer(Display parent, ID2D1BitmapRenderTarget render,Shapes.Rectangle drawto) 
        {
            this.parent = parent;
            this.render = render;
            rendZone = drawto;

        }
        /// <summary>
        /// こいつを管理している親
        /// </summary>
        readonly Display parent;

        /// <summary>
        /// 確保した場所が変化した場合に呼びだす。
        /// </summary>
        public void Changed()
        {
            NoChange = false;
        }

        /// <summary>
        /// 違いを探して、Nochangeを操る
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="F"></param>
        /// <param name="color"></param>
        /// <returns>変わっていたか</returns>
        private bool CheckChange(string Text, FontC F, ColorC color)
        {
            if (PastText == null || PastText != Text)
            {
                NoChange = false;
                return　true;
            }

            if (PastFont == null || PastFont != F)
            {
                NoChange = false;
                return true;
            }

            if (PastColor == null || PastColor != color)
            {
                NoChange = false;
                return true;
            }
            return false;
        }


        public void Release() 
        {
            parent.ReleaseTextRenderer(this);
        }

        /// <summary>
        /// 新しく作る
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="F"></param>
        /// <param name="color"></param>
        private void SetPast(string Text, FontC F, ColorC color)
        {
            PastText = Text;
            PastFont=new FontC();
            F.copy(PastFont);
            PastColor = new ColorC(color);

        }
        float _bottom=0;//!<文字が書かれた領域の最下端
        /// <summary>
        /// 領域内でも字が書かれた最下端
        /// </summary>
        public float bottom { get { if (rendZone.h == 0) return 0.5f;
                return _bottom/rendZone.h; } }

        private string PastText = null;
        private FontC PastFont = null;
        private ColorC PastColor = null;
        public void Draw(string Text, FontC F, ColorC color)
        {
            if (CheckChange(Text, F, color)) 
            {
                SetPast(Text, F, color);
            }

            if (!NoChange)
            {
              //  Text+= "\n->"+rendZone.gettxy(0, 0) + " :TO: " + rendZone.gettxy(rendZone.w, rendZone.h);
                var raw = (RawRectF)rendZone;
                render.BeginDraw();
                render.Transform = Matrix3x2.CreateTranslation(0, 0);
                float R = 0.99f, G = 0.98f, B = 0.97f;
                {

                    render.PushAxisAlignedClip(rendZone,AntialiasMode.PerPrimitive);
                    render.Clear(new ColorC(R, G, B, 0));
                    render.PopAxisAlignedClip();
                }
                {
                    var slb = render.CreateSolidColorBrush(color);
                    render.DrawText(Text, F.ToFont(), rendZone, slb);
                }
                render.EndDraw();

                if (F.aliV != FontC.alignment.left)
                {
                    var list = Display.GetPixels(render.Bitmap, render);


                    bool breaked = false;
                    for (int y = (int)(rendZone.y + rendZone.h - 1); y >= (int)rendZone.y; y--)
                    {
                        for (int x = (int)rendZone.x; x < (int)rendZone.x + rendZone.w; x++)
                        {
                            if (list[y][x].opa > 0)
                            {
                                _bottom = y - rendZone.y;
                                breaked = true;
                                break;
                            }                            
                        }
                        if (breaked) break;
                    }
                }
                /*
                {
                    var slb = render.CreateSolidColorBrush(new ColorC(0, 0, 0, 1));
                    render.DrawRectangle((RawRectF)rendZone,slb);
                }*/

                NoChange = true;
                parent.Drawed(this);
             }

        }

    }


    public class Text :Drawable
    {
        public string text="";
        public FontC font;
        private TextRenderer _Trender = null;

        internal TextRenderer Trender { get { return _Trender; }

            set {
                    _Trender?.Release();
                _Trender = value;
            }
        }

        public Text(float z, ColorC c, string text,FontC font,float time = -1, string name = "") : base(z, c, time, name)
        {
            this.text = text;
            this.font = font;
        }
        public Text() { }

        protected override void draw(Camera cam)
        {
            if (Trender == null)
            {
                //ここでテンポラリなやつをもらう。
                Trender = cam.d.makeTextRenderer(font.w, font.h);
            }
            else if(Trender.rendZone.w!=font.w && Trender.rendZone.h != font.h)
            {
                Trender = cam.d.makeTextRenderer(font.w, font.h);
            }

            Trender.Draw(text,font,col);

            var render = cam.render;


            float yZure=0;
            switch (font.aliV)//無理やり上下にアライメントする
            {
                case FontC.alignment.left://上揃え。何もしない

                    break;
                case FontC.alignment.center:
                    yZure = e.h/2- (Trender.bottom *e.h);
                    break;
                case FontC.alignment.right:
                    yZure = e.h- (Trender.bottom*e.h) ;
                    break;
                case FontC.alignment.justify:
                    break;
                default:
                    break;
            }
            render.Transform = rectTrans(cam);
            var rect = rectRectF(cam);

            var zure = new FXY(0,yZure);
            zure = camsoutai(cam,zure+cam.watchRect.gettxy(0,0));
            zure.degree += e.degree;
            rect.Left += zure.x;
            rect.Right += zure.x;
            rect.Top += zure.y;
            rect.Bottom += zure.y;
            {
             
            render.DrawBitmap(Trender.render.Bitmap
              , rect
              , this.col.opa, BitmapInterpolationMode.Linear
                , Trender.rendZone);
            }

        }
        ~Text() 
        {
            Trender = null;
        }
    }
    #endregion
    /// <summary>
    /// カメラは現在、コピーSAVE不可
    /// </summary>
    public class Camera : Drawable
    {
        public Entity watchRect;
        /// <summary>
        /// レンダー
        /// </summary>
        public ID2D1RenderTarget render;
        public ID2D1BitmapRenderTarget Brender
            {  get{
                if (!isBitmap) return null;
                return (ID2D1BitmapRenderTarget)render;
            }}
        /// <summary>
        /// このカメラがビットマップか。
        /// </summary>
        readonly protected bool isBitmap;
        /// <summary>
        /// ディスプレイ
        /// </summary>
        public readonly Display d;

        
        /// <summary>
        /// カメラのx解像度。draw関数の中だとこっち
        /// </summary>
        public float resolx 
        {
            get 
            {
                return render.Size.Width/Mathf.abs(watchRect.w);
            }
        }
        /// <summary>
        /// カメラのx解像度。draw関数の中だとこっち
        /// </summary>
        public float resoly
        {
            get { return render.Size.Height / Mathf.abs(watchRect.h); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WatchRect">この人は別途worldに追加した方がいい</param>
        /// <param name="z"></param>
        /// <param name="col"></param>
        /// <param name="render"></param>
        /// <param name="d"></param>
        public Camera(Entity WatchRect,float z,ColorC col,ID2D1RenderTarget render, Display d):base(z,col)
        {
            this.d = d;
            this.render = render;
            isBitmap = render.GetType() == typeof(ID2D1BitmapRenderTarget);
            this.watchRect = WatchRect;

        }

        public override void update(float cl)
        {
            //この中ではcl=0のとき呼び出されないやつとかある。
            base.update(cl);

            //cl=0でも呼び出される
            if (isBitmap)
            {
                render.BeginDraw();
            }
            if (!(!isBitmap && col.opa <= 0))
            {
                render.Clear(col);
            }
            if (watchRect.world != null)
            {

                foreach (var a in watchRect.world.Ddic.getresult())
                {
                    a.goDraw(this);
                }
                render.Transform = Matrix3x2.CreateRotation(0);
            }
            if (isBitmap)
            {
                render.EndDraw();
            }
        }
        ~Camera() 
        {
            d.removeCamera(this);
            if (isBitmap)
            {
                Brender?.Bitmap.Dispose();
                Brender.Dispose();
            }
        }
        override protected void draw(Camera cam) 
        {
            if (isBitmap)
            {
                var render = cam.render;
                render.Transform = rectTrans(cam);
                render.DrawBitmap(Brender.Bitmap, rectRectF(cam), col.opa, BitmapInterpolationMode.Linear, source);
            }
        }
        /// <summary>
        /// ビットマップ全体
        /// </summary>
        /// <returns></returns>
        protected RawRectF source{get{return new RawRectF(0, 0, render.Size.Width, render.Size.Height);}}
    }

    public class Display
    {
        public ID2D1HwndRenderTarget render { get { return _render; } }
        ID2D1HwndRenderTarget _render;

        /// <summary>
        /// スクリーンショットに使うレンダー      
        /// </summary>
        ID2D1BitmapRenderTarget _SCSRender;


        /// <summary>
        /// Textの一次的な描画に使うレンダーに使うレンダー      
        /// </summary>
        ID2D1BitmapRenderTarget _TextRender;

        ID2D1Factory fac;
        public readonly float resol;
        public float width { get { return _render.Size.Width; } }
        public float height { get { return _render.Size.Height; } }

        public Display(ContainerControl f,float resolution) 
        {
            resol = resolution;
            D2D1.D2D1CreateFactory<ID2D1Factory>(Vortice.Direct2D1.FactoryType.SingleThreaded, out fac);
            var renpro = new RenderTargetProperties();
            var hrenpro = new HwndRenderTargetProperties();
            hrenpro.Hwnd = f.Handle;
            var wi = f.ClientSize.Width;
            var hei = f.ClientSize.Height;
            System.Drawing.Size si = new System.Drawing.Size((int)(wi * resolution), (int)(hei * resolution));
            hrenpro.PixelSize = si;
            _render=fac.CreateHwndRenderTarget(renpro, hrenpro);


            var fom = new Vortice.DCommon.PixelFormat();
            _SCSRender = render.CreateCompatibleRenderTarget(si, si
                , fom
                , CompatibleRenderTargetOptions.GdiCompatible);

            var TextRSize= new System.Drawing.Size((int)((wi+hei) * resolution), (int)((wi+hei) * resolution));
            _TextRender = render.CreateCompatibleRenderTarget(TextRSize, TextRSize
                , fom
                , CompatibleRenderTargetOptions.GdiCompatible);
        }

        List<Camera> cameras = new List<Camera>();
        /// <summary>
        /// 画面に直接描画するカメラを作る。Cameraが追加されてるEntityはマジどうでもいい
        /// </summary>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public Camera makeCamera(Entity Watchrect, ColorC backcolor)
        {
            Entity back = Entity.make2(0, 0, render.Size.Width, render.Size.Height);
            Camera res;
            res = new Camera(Watchrect, 0, backcolor, render, this);
            res.add(back);

            cameras.Add(res);
            return res;
        }
        /// <summary>
        /// 画面に直接描画するカメラを作る。Watchrectはあとでworldに追加してね。Cameraが追加されてるEntityはマジどうでもいい
        /// </summary>
        /// <returns></returns>
        public Camera makeCamera( ColorC backcolor)
        {
            Entity Watchrect = Entity.make2(0, 0, render.Size.Width/resol, render.Size.Height / resol);

            return makeCamera(Watchrect,backcolor);
        }
        /// <summary>
        /// Cameraデストラクタでだけ呼び出す。
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool removeCamera(Camera c)
        {
            return cameras.Remove(c);
        }
        public void clearCamera()
        {
            cameras.Clear();
        }
        /// <summary>
        /// カメラを描画(Updateもするよあんま意味ないけど)する
        /// </summary>
        /// <param name="cam">描画するカメラ。このカメラはWorldに所属していない方がいい。(二重にUpdateされちゃうし)<br></br>
        /// また、Cameraはもちろんこのdisplayから創出された画面用のカメラ</param>
        /// <param name="cl">カメラのフレームスピード</param>
        public void draw(Camera cam,float cl=1)
        {
            render.BeginDraw();
            foreach (var a in cameras) 
            {
                a.e.update(cl);
            }
            render.EndDraw();
        }
        /// <summary>
        /// このカメラのスクショをとる
        /// <param name="cam">描画するカメラ。</param>
        /// </summary>
        public void ShotThisScreen(Camera cam)
        {
             var Size = render.PixelSize;
            //サイズの違いでバグる可能性あり！！！丸めてどうにかしたが、画質の値によっては今後もやばいぞ！
            foreach (var a in cameras) 
            {
                if (a.render == _render)
                {
                    a.render = _SCSRender;
                }
            }
            _SCSRender.BeginDraw();
            draw(cam,0);
            _SCSRender.EndDraw();
            foreach (var a in cameras)
            {
                if (a.render == _SCSRender)
                {
                    a.render = render;
                }
            }
            screenShot(_SCSRender);
        }
        /// <summary>
        /// ビットマップを->扱いやすい形に変える
        /// </summary>
        /// <param name="image"></param>
        /// <param name="renderTarget"></param>
        /// <returns></returns>
        static public List<List<ColorC>> GetPixels(ID2D1Bitmap image, ID2D1RenderTarget renderTarget)
        {
            var deviceContext2d = renderTarget.QueryInterface<ID2D1DeviceContext>();
            var bitmapProperties = new BitmapProperties1();
            bitmapProperties.BitmapOptions = BitmapOptions.CannotDraw | BitmapOptions.CpuRead;
            bitmapProperties.PixelFormat = image.PixelFormat;


            var bitmap1 = deviceContext2d.CreateBitmap(image.PixelSize, renderTarget.NativePointer
                , sizeof(int), ref bitmapProperties);

           

            bitmap1.CopyFromBitmap(renderTarget.CreateSharedBitmap(image
                , new BitmapProperties(image.PixelFormat)));
            var map = bitmap1.Map(MapOptions.Read);
            var size = image.PixelSize.Width * image.PixelSize.Height * 4;
            byte[] bytes = new byte[size];
            Marshal.Copy(map.Bits, bytes, 0, size);
            bitmap1.Unmap();

            bitmap1.Release();

            //bitmap1.Dispose();
            //deviceContext2d.Dispose();
            var res = new List<List<ColorC>>();
            for (int y = 0; y < image.PixelSize.Height; y++)
            {
                res.Add(new List<ColorC>());
                for (int x = 0; x < image.PixelSize.Width; x++)
                {
                    var position = (y * image.PixelSize.Width + x) * 4;
                    res[y].Add(
                        new ColorC(bytes[position + 2]/255f, bytes[position + 1]/255f
                        , bytes[position + 0]/255f, bytes[position + 3]/255f));

                }
            }
            return res;
        }

        /// <summary>
        /// 画面のスクリーンショットを取る。effectcharaがぶれるバグあり！
        /// </summary>
        /// <param name="h">保存する表示マン</param>
        /// <param name="format">保存フォーマット</param>
        /// <param name="addname">追加で付ける名前</param>
        static public void screenShot(ID2D1BitmapRenderTarget bt, string format = "png", string addname = "")
        {
            /*
            var bt = h.render.CreateCompatibleRenderTarget(size, CompatibleRenderTargetOptions.None);
            //  var bt = new ID2D1BitmapRenderTarget(bm.NativePointer);


            h.hyoji2(bt, 0, true, true, false, false, true);
            */

            string dir = @".\shots\";

            if (Directory.Exists(dir))
            {
            }
            else
            {
                Directory.CreateDirectory(dir);
            }

            
            var pxs = GetPixels(bt.Bitmap, bt);

            var size = new Size(pxs[0].Count, pxs.Count);

            var save = new Bitmap(size.Width, size.Height);

            //Console.WriteLine(size.Width + " a:ga:la " + size.Height);
            //  Console.WriteLine(bt.Size.Width + " a:ga:la " + bt.Size.Height);
            for (int y = 0; y < size.Height; y++)
            {
                for (int x = 0; x < size.Width; x++)
                {
                    //  Console.WriteLine(pxs[y][x].A+" al:skfa :");
                    save.SetPixel(x, y, System.Drawing.Color.FromArgb((int)((pxs[y][x].opa)*255)
                        , (int)(pxs[y][x].r*255), (int)(pxs[y][x].g*255), (int)(pxs[y][x].b*255))
                        );
                }
            }
            string name = addname + DateTime.Now.ToString() + "." + format;

            name = name.Replace("/", "_");
            name = name.Replace(" ", "_");
            name = name.Replace(":", "_");
            Debug.WriteLine(dir + name+"SHOT");
            save.Save(dir + name);


        }


        List<TextRenderer> textRenderers = new List<TextRenderer>();
        internal TextRenderer makeTextRenderer(float w,float h) 
        {
            //右下から順に確保していく。
            w = Mathf.ceil(w);
            h = Mathf.ceil(h);

            List<FXY> Points = new List<FXY>();
            Points.Add(new FXY(_TextRender.Bitmap.Size.Width-1, _TextRender.Bitmap.Size.Height - 1));
            foreach (var a in textRenderers) 
            {
                //上に追加
                {
                    var np = a.rendZone.gettxy(a.rendZone.w, 0);
                    if (np.x > 0 && np.y>0)
                    {
                        bool ok = true;
                        foreach (var b in textRenderers)
                        {
                            if (b.rendZone.onhani(np.x, np.y - 0.5f))
                            {
                                ok = false;
                                break;
                            }
                        }
                        if (ok)
                        {
                            Points.Add(np);
                        }
                    }
                }
                //左に追加
                {
                    var np = a.rendZone.gettxy(0, a.rendZone.h);
                    if (np.x > 0 && np.y > 0)
                    {
                        bool ok = true;
                        foreach (var b in textRenderers)
                        {
                            if (b.rendZone.onhani(np.x - 0.5f, np.y))
                            {
                                ok = false;
                                break;
                            }
                        }
                        if (ok)
                        {
                            Points.Add(np);
                        }
                    }
                }
            }

            List<Shapes.Rectangle> rects = new List<Shapes.Rectangle>();
            foreach (var a in Points) 
            {
                float maxx=0, maxy=0;
                foreach (var b in textRenderers) 
                {
                    if (b.rendZone.onhani(a.x - 0.5f, a.y - 0.5f
                        , a.x - _TextRender.Size.Width, a.y)) 
                    {
                        maxx = Mathf.max(b.rendZone.gettxy(b.rendZone.w,0).x,maxx);
                    }
                }
                foreach (var b in textRenderers)
                {
                    if (b.rendZone.onhani(a.x - 0.5f, a.y - 0.5f
                        , a.x , a.y - _TextRender.Size.Height))
                    {
                        maxy = Mathf.max(b.rendZone.gettxy(0, b.rendZone.h).y,maxy);
                    }
                }
                if (a.x - maxx > 0 &&  a.y- maxy > 0)
                {
                    
                    rects.Add(new Shapes.Rectangle(maxx, maxy, a.x - maxx, a.y - maxy));
                }

            }
            Shapes.Rectangle res = null;
            foreach (var a in rects) 
            {
                //なるべく面積の小さい奴を選ぶ
                if (res == null || res.menseki > a.menseki) 
                {
                    if (a.w >= w && a.h >= h) 
                    {
                        res = a;
                    }
                }
            }
            //何もなかったら無理やり左上の点を取る。
            if (res == null) 
            {
                res = new Shapes.Rectangle(0, 0, w, h);
            }
            {
                var fxy = res.gettxy(res.w, res.h);
                res = new Shapes.Rectangle(Mathf.max(fxy.x-w,0), Mathf.max(fxy.y - h, 0)
                    ,w,h);
            }
            var returns = new TextRenderer(this, _TextRender, res);
            textRenderers.Add(returns);
            return returns;   
        }

        internal void ReleaseTextRenderer(TextRenderer D)
        {
            textRenderers.Remove(D);
        }
        internal void Drawed(TextRenderer D)
        {
            foreach (var a in textRenderers) 
            {
                if (a!=D) 
                {
                    //境界線を含まないためこのざま
                    foreach (var b in a.rendZone.getzettaipoints())
                    {
                        if (D.rendZone.onhani(b.x, b.y, 0.9999f))
                        {
                            
                            a.Changed();
                            break;
                        }
                    }
                }
            }
        }
    }
    #endregion
}
