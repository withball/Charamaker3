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
using System.Threading;

namespace Charamaker3
{
    /// <summary>
    /// レンダーのセット！
    /// </summary>
    public class C3RenderSet 
    {
        public ID2D1RenderTarget Render;

        public ID2D1DeviceContext DeviceContext;

        /// <summary>
        /// テクスチャ描画に使うエフェクトたち
        /// </summary>
        public Vortice.Direct2D1.Effects.AffineTransform2D ECrop0;
        public Vortice.Direct2D1.Effects.Crop ECrop1;
        public Vortice.Direct2D1.Effects.AffineTransform2D ECrop2;
        public Vortice.Direct2D1.Effects.ColorMatrix EBlend;
        public Vortice.Direct2D1.Effects.AffineTransform2D ETrans;

        /// <summary>
        /// エフェクトとかも確保してくれる
        /// </summary>
        /// <param name="render"></param>
        public C3RenderSet(ID2D1RenderTarget render) 
        {
        this.Render = render;
            this.DeviceContext=this.Render.QueryInterface<ID2D1DeviceContext>();
            this.ECrop0 = new Vortice.Direct2D1.Effects.AffineTransform2D(DeviceContext);
            this.ECrop1 = new Vortice.Direct2D1.Effects.Crop(DeviceContext);
            this.ECrop2 = new Vortice.Direct2D1.Effects.AffineTransform2D(DeviceContext);
            this.EBlend = new Vortice.Direct2D1.Effects.ColorMatrix(DeviceContext);
            this.ETrans = new Vortice.Direct2D1.Effects.AffineTransform2D(DeviceContext);
        }
        /// <summary>
        /// レンダーまでは開放しない
        /// </summary>
         ~C3RenderSet() 
        {
            ECrop0.Release();
            ECrop1.Release();
            ECrop2.Release();
            EBlend.Release();
            ETrans.Release();
            DeviceContext.Release();

        }
    }
    /// <summary>
    /// カメラは現在、コピーSAVE不可
    /// </summary>
    public class Camera : Drawable
    {
        public Entity watchRect;
        /// <summary>
        /// レンダー
        /// </summary>
        public C3RenderSet render;

        public ID2D1BitmapRenderTarget Brender
        {
            get
            {
                if (!isBitmap) return null;
                return (ID2D1BitmapRenderTarget)render.Render;
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
                return render.Render.Size.Width / Mathf.abs(watchRect.w);
            }
        }
        /// <summary>
        /// カメラのx解像度。draw関数の中だとこっち
        /// </summary>
        public float resoly
        {
            get { return render.Render.Size.Height / Mathf.abs(watchRect.h); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="WatchRect">この人は別途worldに追加した方がいい</param>
        /// <param name="z"></param>
        /// <param name="col"></param>
        /// <param name="render"></param>
        /// <param name="d"></param>
        public Camera(Entity WatchRect, float z, ColorC col,C3RenderSet render, Display d) : base(z, col)
        {
            this.d = d;
            this.render = render;
            isBitmap = render.GetType() == typeof(ID2D1BitmapRenderTarget);
            this.watchRect = WatchRect;

        }
        
        /// <summary>
        /// テキストなど事前描画が必要なやつを一気に処理する
        /// </summary>
        virtual public void PreDraw(float cl,DisplaySemaphores semaphores)
        {
            List<Task>tasks = new List<Task>();
            //cl=0でも呼び出される
            if (watchRect.world != null)
            {

                tasks.Add(Task.Run(() =>
                {
                    foreach (var a in watchRect.world.Ddic.getresult())
                    {
                        if (a.CanDraw(this))
                        {
                            a.PreDraw(this, semaphores);
                        }

                    }
                }));
            }
            foreach (var task in tasks) { task.Wait(); }
        }
        public override void update(float cl)
        {
            //この中ではcl=0のとき呼び出されないやつとかある。
            base.update(cl);

            //cl=0でも呼び出される
            if (isBitmap)
            {
                render.Render.BeginDraw();
            }
            if (!(!isBitmap && col.opa <= 0))
            {
                render.Render.Clear(col);
            }
            if (watchRect.world != null)
            {


                var lis = watchRect.world.Ddic.getresult();
                var oks = new bool[lis.Count];

                List<Task> tasks = new List<Task>();

                for (int i = 0; i < lis.Count; i++)//描画判定を先に済ませる
                {
                    int ii = i;
                    tasks.Add(Task.Run(() =>
                    {
                        oks[ii] = lis[ii].CanDraw(this);
                    }));
                }

                foreach (var task in tasks) { task.Wait(); }

                for (int i = 0;i<lis.Count;i++)
                {
                    if (oks[i])
                    {
                        lis[i].draw(this);
                    }
                }
                render.Render.Transform = Matrix3x2.CreateRotation(0);
            }
            if (isBitmap)
            {
                render.Render.EndDraw();
            }
        }
        ~Camera()
        {
            //d.removeCamera(this);//いらないよね？
            if (isBitmap)
            {
                Brender?.Bitmap.Dispose();
                Brender.Dispose();
            }
        }
        override public void draw(Camera cam)
        {
            if (isBitmap)
            {
                var render = cam.render;
                render.Render.Transform = rectTrans(cam);
                render.Render.DrawBitmap(Brender.Bitmap, rectRectF(cam), col.opa, BitmapInterpolationMode.Linear, source);
            }
        }
        /// <summary>
        /// ビットマップ全体
        /// </summary>
        /// <returns></returns>
        protected RawRectF source { get { return new RawRectF(0, 0, render.Render.Size.Width, render.Render.Size.Height); } }
    }
    public class DisplaySemaphores
    {
        public SemaphoreSlim TextRender=new SemaphoreSlim(1,1);
    }
    public class Display
    {
        /// <summary>
        /// カメラのWatchRectの標準の名前
        /// </summary>
        public const string WatchRectName = "WatchRect";
        public C3RenderSet render { get { return _render; } }
        C3RenderSet _render;


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
        public float width { get { return _render.Render.Size.Width; } }
        public float height { get { return _render.Render.Size.Height; } }

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
            _render = new C3RenderSet(fac.CreateHwndRenderTarget(renpro, hrenpro));


            var fom = new Vortice.DCommon.PixelFormat();
            _SCSRender = render.Render.CreateCompatibleRenderTarget(si, si
                , fom
                , CompatibleRenderTargetOptions.GdiCompatible);

            var TextRSize = new System.Drawing.Size((int)((wi + hei) * resolution)*3, (int)((wi + hei) * resolution)*3);
            _TextRender = render.Render.CreateCompatibleRenderTarget(TextRSize, TextRSize
                , fom
                , CompatibleRenderTargetOptions.GdiCompatible);

            _TextRender.BeginDraw();

            _TextRender.Clear(new ColorC(1, 0, 0, 1));//異常チェック用に赤に塗っとく
            _TextRender.EndDraw();

            _BlendRender = render.Render.CreateCompatibleRenderTarget(TextRSize, TextRSize
                , fom
                , CompatibleRenderTargetOptions.GdiCompatible);

        }

        List<CP<Camera>> cameras = new List<CP<Camera>>();
        /// <summary>
        /// 画面に直接描画するカメラを作る。Cameraが追加されてるEntityはマジどうでもいい
        /// </summary>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public Camera makeCamera(Entity Watchrect, ColorC backcolor)
        {
            Entity back = Entity.make2(0, 0, render.Render.Size.Width, render.Render.Size.Height);
            Camera res;
            res = new Camera(Watchrect, 0, backcolor, render, this);
            res.add(back);

            cameras.Add(Component.ToPointer(res));
            return res;
        }

        /// <summary>
        /// 画面に直接描画するカメラを作る。Watchrectはあとでworldに追加してね。Cameraが追加されてるEntityはマジどうでもいい
        /// </summary>
        /// <returns></returns>
        public Camera makeCamera(ColorC backcolor)
        {
            Entity Watchrect = Entity.make2(0, 0, render.Render.Size.Width / resol, render.Render.Size.Height / resol, name: WatchRectName);

            return makeCamera(Watchrect, backcolor);
        }
        /// <summary>
        /// Cameraデストラクタでだけ呼び出す。
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool removeCamera(CP<Camera> c)
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
            render.Render.BeginDraw();
            foreach (var a in cameras)
            {
                if (a.c.watchRect.added)
                {
                    a.c.e.update(cl);
                }
            }
            render.Render.EndDraw();
        }


        //事前描画のセマフォア
        private DisplaySemaphores Semaphores =new DisplaySemaphores();

        

        /// <summary>
        /// テキストのやつなど、事前に描画が必要なやつなどをまとめて処理する
        /// </summary>
        /// <param name="cl"></param>
        protected void PreDraw(float cl)
        {
            _TextRender.BeginDraw();
            foreach (var a in cameras)
            {
                a.c.PreDraw(cl,Semaphores);
            }
            _TextRender.EndDraw();
        }
        /// <summary>
        /// このカメラのスクショをとる
        /// <param name="cam">描画するカメラ。</param>
        /// </summary>
        public void ShotThisScreen(Camera cam)
        {
            var Size = render.Render.PixelSize;
            //サイズの違いでバグる可能性あり！！！丸めてどうにかしたが、画質の値によっては今後もやばいぞ！
            foreach (var a in cameras)
            {
                if (a.c.render.Render == render.Render)
                {
                    a.c.render.Render = _SCSRender;
                }
            }
            _SCSRender.BeginDraw();
            draw(0);
            _SCSRender.EndDraw();
            foreach (var a in cameras)
            {
                if (a.c.render.Render == _SCSRender)
                {
                    a.c.render.Render = render.Render;
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
                    var c = new ColorC(bytes[position + 2] / 255f, bytes[position + 1] / 255f
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

            string dir = FileMan.s_rootpath + @"shots\";

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
                string r = "";
                for (int x = 0; x < size.Width; x++)
                {
                    //  Console.WriteLine(pxs[y][x].A+" al:skfa :");
                    save.SetPixel(x, y, System.Drawing.Color.FromArgb((byte)((pxs[y][x].opa) * 255)
                        , (byte)(pxs[y][x].r * 255), (byte)(pxs[y][x].g * 255), (byte)(pxs[y][x].b * 255))
                        );
                    /*
                    r = (byte)((pxs[y][x].opa) * 255) + " A "
                        + (byte)(pxs[y][x].r * 255) + " R "
                        + (byte)(pxs[y][x].g * 255) + " G "
                        + (byte)(pxs[y][x].b * 255) + " B ";
                */}
                //Debug.WriteLine(r);
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
        List<TextRenderer> textRenderersRemove = new List<TextRenderer>();
        /// <summary>
        /// テキスト描画用の奴の数
        /// </summary>
        public int TextRenderesNum { get { return textRenderers.Count; } }
        public int TextRenderesRemoveNum { get { return textRenderersRemove.Count; } }
        internal TextRenderer makeTextRenderer(float w, float h)
        {
            for (int i=textRenderersRemove.Count-1;i>=0;i--)
            {
                textRenderers.Remove(textRenderersRemove[i]);
                textRenderersRemove.RemoveAt(i);
            }
            //  Debug.WriteLine("make TextRendere" + w + " :: " + h);
            //右下から順に確保していく。
            w = Mathf.ceil(w);
            h = Mathf.ceil(h);

            var hanteis = new List<Task>();

            List<Shapes.Rectangle> rects = new List<Shapes.Rectangle>();

            void addrect(FXY np) 
            {
                float maxx = 0, maxy = 0;
                foreach (var b in textRenderers)
                {
                    if (onHani(b.rendZone, np.x, np.y
                        , np.x - _TextRender.Size.Width, np.y))
                    {
                        maxx = Mathf.max(b.rendZone.gettxy(b.rendZone.w, 0).x, maxx);
                    }
                }
                foreach (var b in textRenderers)
                {
                    if (onHani(b.rendZone, np.x, np.y
                        , np.x, np.y - _TextRender.Size.Height))
                    {
                        maxy = Mathf.max(b.rendZone.gettxy(0, b.rendZone.h).y, maxy);
                    }
                }
                if (np.x - maxx > 0 && np.y - maxy > 0)
                {
                    var rect = new Shapes.Rectangle(maxx, maxy, np.x - maxx, np.y - maxy);

                    rects.Add(rect);

                }
            }

            hanteis.Add(Task.Run(() => { addrect(new FXY(_TextRender.Bitmap.Size.Width - 1, _TextRender.Bitmap.Size.Height - 1)); }));

            for ( int t=0;t< textRenderers.Count;t++)
            {
                var a=textRenderers[t];
                hanteis.Add(Task.Run(() =>
                {
                    FXY left=null,up=null;
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
                            if (ok )
                            {
                                addrect(np);
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
                                addrect(np);
                            }
                        }
                    }
                }));
            }
            foreach (var a in hanteis) 
            {
                a.Wait();
            }

            
            Shapes.Rectangle res = null;
            foreach (var a in rects)
            {
                if (a != null)
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
            //右下順に並べる
            var ss = new supersort<TextRenderer>();
            ss.add(returns, returns.rendZone.x + returns.rendZone.y);
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
            textRenderersRemove.Add(D);
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
        /// ビットマップに色を付けて返す。少しでも色付けちゃうと重くなる悪いシステム
        /// </summary>
        /// <param name=""></param>
        /// <param name="color">opacityは関係ない</param>
        /// <returns></returns>
        public ID2D1Bitmap Blend(ID2D1Bitmap bitmap, ColorC color)
        {
            if (color.r == 1 && color.g == 1 && color.b == 1) { return bitmap; }
            var d2dContext = _BlendRender.QueryInterface<ID2D1DeviceContext>();


            //_BlendRender.PushAxisAlignedClip(new Rectangle(0,0,bitmap.PixelSize.Width, bitmap.PixelSize.Height)
            //    , AntialiasMode.PerPrimitive);


            var blend = new Vortice.Direct2D1.Effects.ColorMatrix(d2dContext);
            blend.SetInput(0, bitmap, new SharpGen.Runtime.RawBool(true));

            var trans=new Vortice.Direct2D1.Effects.AffineTransform2D(d2dContext);
            trans.SetInputEffect(0,blend, new SharpGen.Runtime.RawBool(true));

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
            colormatrix.M44 = 1;
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



