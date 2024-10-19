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
using Charamaker3.Shapes;
using Rectangle = Charamaker3.Shapes.Rectangle;

namespace Charamaker3
{

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
        {
            get
            {
                if (!isBitmap) return null;
                return (ID2D1BitmapRenderTarget)render;
            }
        }
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
                return render.Size.Width / Mathf.abs(watchRect.w);
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
        public Camera(Entity WatchRect, float z, ColorC col, ID2D1RenderTarget render, Display d) : base(z, col)
        {
            this.d = d;
            this.render = render;
            isBitmap = render.GetType() == typeof(ID2D1BitmapRenderTarget);
            this.watchRect = WatchRect;

        }
        /// <summary>
        /// テキストなど事前描画が必要なやつを一気に処理する
        /// </summary>
        virtual public void PreDraw(float cl)
        {
            //cl=0でも呼び出される
            if (watchRect.world != null)
            {
                foreach (var a in watchRect.world.Ddic.getresult())
                {
                    a.goPreDraw(this);
                }
            }
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
        protected RawRectF source { get { return new RawRectF(0, 0, render.Size.Width, render.Size.Height); } }
    }
    /// <summary>
    /// なぜか解放できないレンダーのセット
    /// </summary>
    public class RenderSet
    {
        public RenderSet() 
        {
            ID2D1RenderTarget render;
        }
    }
    public class Display
    {
        /// <summary>
        /// カメラのWatchRectの標準の名前
        /// </summary>
        public const string WatchRectName = "WatchRect";
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

        /// <summary>
        /// 画像のブレンドに一次的に使うレンダーに使うレンダー      
        /// </summary>
        ID2D1BitmapRenderTarget _BlendRender;


        ID2D1Factory fac;
        public readonly float resol;
        public float width { get { return _render.Size.Width; } }
        public float height { get { return _render.Size.Height; } }

        public Display(ContainerControl f, float resolution)
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
            _render = fac.CreateHwndRenderTarget(renpro, hrenpro);


            var fom = new Vortice.DCommon.PixelFormat();
            _SCSRender = render.CreateCompatibleRenderTarget(si, si
                , fom
                , CompatibleRenderTargetOptions.GdiCompatible);

            var TextRSize = new System.Drawing.Size((int)((wi + hei) * resolution), (int)((wi + hei) * resolution));
            _TextRender = render.CreateCompatibleRenderTarget(TextRSize, TextRSize
                , fom
                , CompatibleRenderTargetOptions.GdiCompatible);

            _BlendRender = render.CreateCompatibleRenderTarget(TextRSize, TextRSize
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
        public Camera makeCamera(ColorC backcolor)
        {
            Entity Watchrect = Entity.make2(0, 0, render.Size.Width / resol, render.Size.Height / resol, name: WatchRectName);

            return makeCamera(Watchrect, backcolor);
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
        public void draw(float cl = 1)
        {
            PreDraw(cl);
            render.BeginDraw();
            foreach (var a in cameras)
            {
                a.e.update(cl);
            }
            render.EndDraw();
        }
        /// <summary>
        /// テキストのやつなど、事前に描画が必要なやつなどをまとめて処理する
        /// </summary>
        /// <param name="cl"></param>
        protected void PreDraw(float cl)
        {
            _TextRender.BeginDraw();
            foreach (var a in cameras)
            {
                a.PreDraw(cl);
            }
            _TextRender.EndDraw();
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
            draw(0);
            _SCSRender.EndDraw();
            foreach (var a in cameras)
            {
                if (a.render == _SCSRender)
                {
                    a.render = render;
                }
            }
            screenShot(_SCSRender);
            //screenShot(_TextRender);
            //screenShot(_BlendRender);
            //screenShot(_BlendRender2);
        }
        /// <summary>
        /// ビットマップを->扱いやすい形に変える
        /// </summary>
        /// <param name="image"></param>
        /// <param name="renderTarget"></param>
        /// <param name="zone">変換する領域。nullで全部</param>
        /// <returns></returns>
        static public List<List<ColorC>> GetPixels(ID2D1Bitmap image, ID2D1RenderTarget renderTarget, Rectangle zone = null)
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
            var size = (map.Pitch * image.PixelSize.Height);
            byte[] bytes = new byte[size];
            Marshal.Copy(map.Bits, bytes, 0, size);
            //bitmap1.Unmap();
            //bitmap1.Dispose();
            bitmap1.Release();

            //bitmap1.Dispose();
            //deviceContext2d.Release();
            deviceContext2d.Dispose();
            var res = new List<List<ColorC>>();

            if (zone == null)
            {
                zone = new Rectangle(0, 0, image.PixelSize.Width, image.PixelSize.Height);
            }

            for (int y = (int)zone.y; y < image.PixelSize.Height && y < zone.y + zone.h; y++)
            {
                res.Add(new List<ColorC>());
                var yy = y;

                for (int x = (int)zone.x; x < image.PixelSize.Width && x < zone.x + zone.w; x++)
                {
                    var position = map.Pitch * yy + x * (map.Pitch / image.PixelSize.Width);
                    var c = new ColorC(bytes[position + 2] / 255, bytes[position + 1] / 255f
                        , bytes[position + 0] / 255f, bytes[position + 3] / 255f);
                    res[yy - (int)zone.y].Add(c);

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

            string dir = FileMan.rootpath + @"shots\";

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
                    save.SetPixel(x, y, System.Drawing.Color.FromArgb((int)((pxs[y][x].opa) * 255)
                        , (int)(pxs[y][x].r * 255), (int)(pxs[y][x].g * 255), (int)(pxs[y][x].b * 255))
                        );
                }
            }
            string name = addname + DateTime.Now.ToString() + "." + format;

            name = name.Replace("/", "_");
            name = name.Replace(" ", "_");
            name = name.Replace(":", "_");
            Debug.WriteLine(dir + name + "SHOT");
            save.Save(dir + name);


        }

        /// <summary>
        /// 高速正確四角形の当たり判定
        /// </summary>
        /// <param name="r"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static bool onHani(Rectangle r, float x, float y)
        {
            if ((r.x <= x && x <= r.x + r.w)
                && (r.y <= y && y <= r.y + r.h))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// 高速正確四角形の当たり判定
        /// </summary>
        /// <param name="r"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static bool onHani(Rectangle r, float x, float y, float x2, float y2)
        {
            if (onHani(r, x, y)) return true;
            if (onHani(r, x2, y2)) return true;

            if (Shape.crosses(r.x, r.y, r.x + r.w, r.y, x, y, x2, y2)) return true;
            if (Shape.crosses(r.x + r.w, r.y, r.x + r.w, r.y + r.h, x, y, x2, y2)) return true;
            if (Shape.crosses(r.x + r.w, r.y + r.h, r.x, r.y + r.h, x, y, x2, y2)) return true;
            if (Shape.crosses(r.x, r.y + r.h, r.x, r.y, x, y, x2, y2)) return true;

            return false;
        }
        /// <summary>
        /// 高速正確四角形の当たり判定
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        static bool onHani(Rectangle r, Rectangle r2)
        {
            if (onHani(r, r2.x       , r2.y       , r2.x + r2.w, r2.y)) return true;
            if (onHani(r, r2.x + r2.w, r2.y       , r2.x + r2.w, r2.y + r2.h)) return true;
            if (onHani(r, r2.x + r2.w, r2.y + r2.h, r2.x       , r2.y + r2.h)) return true;
            if (onHani(r, r2.x       , r2.y + r2.h, r2.x       , r2.y)) return true;

            return false;
        }
        List<TextRenderer> textRenderers = new List<TextRenderer>();
        internal TextRenderer makeTextRenderer(float w, float h)
        {
           
            //  Debug.WriteLine("make TextRendere" + w + " :: " + h);
            //右下から順に確保していく。
            w = Mathf.ceil(w);
            h = Mathf.ceil(h);

            List<FXY> Points = new List<FXY>();
            Points.Add(new FXY(_TextRender.Bitmap.Size.Width - 1, _TextRender.Bitmap.Size.Height - 1));
            foreach (var a in textRenderers)
            {
                //上に追加
                {
                    var np = a.rendZone.gettxy(a.rendZone.w, 0) - new FXY(0, 1);
                    if (np.x > 0 && np.y > 0)
                    {
                        bool ok = true;
                        for (int i = 0; i < textRenderers.Count; i++)
                        {
                            var b = textRenderers[i];
                            if (onHani(b.rendZone, np.x, np.y))
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
                    var np = a.rendZone.gettxy(0, a.rendZone.h) - new FXY(1, 0);
                    if (np.x > 0 && np.y > 0)
                    {
                        bool ok = true;

                        for (int i = 0; i < textRenderers.Count; i++)
                        {
                            var b = textRenderers[i];
                            if (onHani(b.rendZone, np.x, np.y))
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
                float maxx = 0, maxy = 0;
                foreach (var b in textRenderers)
                {
                    if (onHani(b.rendZone, a.x, a.y
                        , a.x - _TextRender.Size.Width, a.y))
                    {
                        maxx = Mathf.max(b.rendZone.gettxy(b.rendZone.w, 0).x, maxx);
                    }
                }
                foreach (var b in textRenderers)
                {
                    if (onHani(b.rendZone, a.x, a.y
                        , a.x, a.y - _TextRender.Size.Height))
                    {
                        maxy = Mathf.max(b.rendZone.gettxy(0, b.rendZone.h).y, maxy);
                    }
                }
                if (a.x - maxx > 0 && a.y - maxy > 0)
                {

                    rects.Add(new Shapes.Rectangle(maxx, maxy, a.x - maxx, a.y - maxy));
                }

            }
            Shapes.Rectangle res = null;
            foreach (var a in rects)
            {
                //なるべく面積の大きいやつを選ぶ
                if (res == null || res.menseki < a.menseki)
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
                res = new Shapes.Rectangle(Mathf.max(fxy.x - w, 0), Mathf.max(fxy.y - h, 0)
                    , w, h);
            }
            var returns = new TextRenderer(this, _TextRender, res);
            textRenderers.Add(returns);
            //右下順に並べる
            var ss = new supersort<TextRenderer>();
            foreach (var a in textRenderers)
            {
                ss.add(a, a.rendZone.x + a.rendZone.y);
            }
            ss.sort(true);
            textRenderers = ss.getresult();
            // Debug.WriteLine("returns "+returns.rendZone.ToSave().getData());

            return returns;
        }

        internal void ReleaseTextRenderer(TextRenderer D)
        {

            textRenderers.Remove(D);
        }
        internal void Drawed(TextRenderer D)
        {

            var lis = new List<TextRenderer>(textRenderers);
            foreach (var a in lis)
            {
                if (a != D)
                {
                    if(onHani(D.rendZone,a.rendZone))
                    {

                        a.Changed();
                        
                    }
                }
            }
        }

        /// <summary>
        /// ビットマップに色を付けて返す。
        /// </summary>
        /// <param name=""></param>
        /// <param name=""></param>
        /// <returns></returns>
        public ID2D1Bitmap Blend(ID2D1Bitmap bitmap,ColorC color) 
        {

            var d2dContext = _BlendRender.QueryInterface<ID2D1DeviceContext>();


            //_BlendRender.PushAxisAlignedClip(new Rectangle(0,0,bitmap.PixelSize.Width, bitmap.PixelSize.Height)
            //    , AntialiasMode.PerPrimitive);


            var blend = new Vortice.Direct2D1.Effects.ColorMatrix(d2dContext);
            blend.SetInput(0,bitmap, new SharpGen.Runtime.RawBool(false));
            var colormatrix = new Matrix5x4();
            colormatrix.M11 = color.r;
            colormatrix.M12 = 0;
            colormatrix.M13 = 0;
            colormatrix.M14 = 0;
            colormatrix.M21 = 0;
            colormatrix.M22 = color.g;
            colormatrix.M23 = 0;
            colormatrix.M24 = 0;
            colormatrix.M31 = 0;
            colormatrix.M32 = 0;
            colormatrix.M33 = color.b;
            colormatrix.M34 = 0;
            colormatrix.M41 = 0;
            colormatrix.M42 = 0;
            colormatrix.M43 = 0;
            colormatrix.M44 = color.opa;
            colormatrix.M51 = 0;
            colormatrix.M52 = 0;
            colormatrix.M53 = 0;
            colormatrix.M54 = 0;
            
            blend.Matrix = colormatrix;


            //_BlendRender.BeginDraw();
            d2dContext.BeginDraw();
            
            
            d2dContext.Clear(new ColorC(0, 0, 0, 0));
            //_BlendRender.Clear(new ColorC(0, 0, 0, 0));
        

            d2dContext.DrawImage(blend);
         
            d2dContext.EndDraw();
            //_BlendRender.BeginDraw();

            d2dContext.Dispose();

            blend.Release();
            //blend.Dispose();
            //_BlendRender.PopAxisAlignedClip();
            return _BlendRender.Bitmap;
        
        }
    }
}



