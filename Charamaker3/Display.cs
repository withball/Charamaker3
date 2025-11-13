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

using Vortice;
using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.Mathematics;
using Color = System.Drawing.Color;

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
        /// ファクトリー
        /// </summary>
        public IDWriteFactory WriteFactory;
        /// <summary>
        /// テクスチャ描画に使うエフェクトたち
        /// </summary>
        public Vortice.Direct2D1.Effects.AffineTransform2D ECrop0=null;
        public Vortice.Direct2D1.Effects.Crop ECrop1 = null;
        public Vortice.Direct2D1.Effects.AffineTransform2D ECrop2 = null;
        public Vortice.Direct2D1.Effects.ColorMatrix EBlend = null;
        public Vortice.Direct2D1.Effects.AffineTransform2D ETrans = null;

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
            DWrite.DWriteCreateFactory<IDWriteFactory>(Vortice.DirectWrite.FactoryType.Shared,out this.WriteFactory);
        }
        /// <summary>
        /// レンダーまでは開放しない
        /// </summary>
         ~C3RenderSet()
        {
            ECrop0?.Release();
            ECrop1?.Release();
            ECrop2?.Release();
            EBlend?.Release();
            ETrans?.Release();
            DeviceContext?.Release();

        }
    }

    /// <summary>
    /// レンダーのセット！
    /// </summary>
    public class C3BitmapRenderSet: C3RenderSet
    {

        /// <summary>
        /// エフェクトとかも確保してくれる
        /// </summary>
        /// <param name="render"></param>
        public C3BitmapRenderSet(ID2D1BitmapRenderTarget render) : base(render)
        {
            var bitmapProperties = new BitmapProperties1();
            bitmapProperties.BitmapOptions = BitmapOptions.CannotDraw | BitmapOptions.CpuRead;
            bitmapProperties.PixelFormat = render.PixelFormat;
            

            //DeviceContext.Fac
            Bitmap1 = this.DeviceContext.CreateBitmap(DeviceContext.PixelSize, IntPtr.Zero
                    , 0, bitmapProperties);

           
        }
        /// <summary>
        /// 事前に用意しておく
        /// </summary>
        public ID2D1Bitmap1 Bitmap1;
        //BitmapProperties1 _bitmapProperties;
        public ID2D1BitmapRenderTarget BitmapRender { get { return (ID2D1BitmapRenderTarget)Render; } }

        /// <summary>
        /// レンダーまでは開放しない
        /// </summary>
        ~C3BitmapRenderSet()
        {
            Bitmap1?.Release();

        }
    }
    /// <summary>
    /// カメラは現在、SAVE不可
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
                if (!_isBitmap) return null;
                return (ID2D1BitmapRenderTarget)render.Render;
            }
        }
        public bool IsBitmap { get { return _isBitmap; } }
        /// <summary>
        /// このカメラがビットマップか。
        /// </summary>
        protected bool _isBitmap;
        /// <summary>
        /// ディスプレイ
        /// </summary>
        public Display d;

        /// <summary>
        /// 描画を停止する。
        /// </summary>
        public bool stopDraw = false;


        /// <summary>
        /// ヒットボックスの表示フラグ
        /// </summary>
        public bool HitboxVisible = false;

        public override void copy(Component c)
        {
            var cc = (Camera)c;
            cc.watchRect = this.watchRect!=null? this.watchRect.clone():null;
            cc.render = this.render;

            cc._isBitmap = this._isBitmap;
            cc.d = this.d;
            cc.stopDraw = this.stopDraw;
            cc.HitboxVisible = this.HitboxVisible;

            base.copy(c);
        }

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
        /// 注意！いろいろnull!
        /// </summary>
        public Camera() 
        {
        
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
            _isBitmap = Mathf.isSubClassOf(render.GetType(),typeof(C3BitmapRenderSet));
            this.watchRect = WatchRect;

        }
        /// <summary>
        /// テキストなど事前描画が必要なやつを一気に処理する
        /// </summary>
        public override void PreDraw(Camera cam, DisplaySemaphores semaphores)
        {
            base.PreDraw(cam, semaphores);

            if (stopDraw == true) { return; }
            if (IsBitmap)
            {
                if (col.opa < 0)
                {
                    return;
                }
            }
            List<Task> tasks = new List<Task>();
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
                }
                ));
            }
            foreach (var task in tasks) { task.Wait(); }
            foreach (var task in tasks) { task.Dispose(); }
            tasks.Clear();
        }
        public override void update(float cl)
        {
            //この中ではcl=0のとき呼び出されないやつとかある。
            base.update(cl);


            if (stopDraw == true) { return; }
            //cl=0でも呼び出される
            if (_isBitmap == true)
            {
                if (col.opa < 0) 
                {
                    return;
                }
                render.Render.BeginDraw();
            }
            if (_isBitmap == false)
            {
                if (col.opa != 0)
                {
                    render.Render.Clear(col);
                }
            }
            else //ビットマップの場合、背景は透明に。
            {
                render.Render.Clear(new ColorC(0,0,0,0));
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
                    }
                    ));
                }

                foreach (var task in tasks) { task.Wait(); }
                foreach (var task in tasks) { task.Dispose(); }

                tasks.Clear();

                for (int i = 0; i < lis.Count; i++)
                {
                    if (oks[i])
                    {
                        lis[i].draw(this);
                    }
                }
                render.Render.Transform = Matrix3x2.CreateRotation(0);
                if (HitboxVisible)
                {
                    foreach (var e in watchRect.world.getEdic("HasHitbox"))
                    {
                        foreach (var b in e.getcompos<Hitboxs.Hitbox>())
                        {
                            if (b.Hitteds.Count == 0)
                            {
                                DrawHitbox(b.HitShape, new ColorC(0, 0, 0.5f, 0.5f));

                                DrawHitbox(b.preHitShape, new ColorC(0, 0.5f, 0, 0.5f));
                            }
                            else
                            {
                                DrawHitbox(b.HitShape, new ColorC(0, 0, 1f, 0.5f));
                                DrawHitbox(b.preHitShape, new ColorC(0, 1f, 0, 0.5f));
                            }
                            var text = "tag:";
                            foreach (var a in b.tag)
                            {
                                text += a + ",";
                            }
                            text += " filter:";
                            foreach (var a in b.tagfilter)
                            {
                                text += a + ",";
                            }
                            DrawText(b.HitShape, text, 16, new ColorC(0, 0, 0, 1));
                        }
                    }
                }
            }
            if (_isBitmap)
            {
                render.Render.EndDraw();
            }
        }
        protected void DrawEntity(Entity e)
        {
            var cam = this;
            var watch = cam.watchRect;
            {


                //  左上
                var upleft = camsoutai(cam, e.gettxy(0, 0));

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
                if (watch.w < 0)
                {
                    a = Matrix3x2.Multiply(Matrix3x2.CreateScale(-1, 1), a);

                }
                if (watch.h < 0)
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



                render.Render.Transform = a;
            }


            using (var brh = this.render.Render.CreateSolidColorBrush(new ColorC(0, 0, 1, 0.5f)))
            {
                var upleft = camsoutai(cam, e.gettxy(0, 0));
                //wも、hも絶対値で四角形を作り、transformの所で反転させて折り合いをつける。

                var w = Mathf.sameSign((camsoutai(cam, e.gettxy(e.w, 0)) - upleft).length, e.w * 0 + 1);
                var h = Mathf.sameSign((camsoutai(cam, e.gettxy(0, e.h)) - upleft).length, e.h * 0 + 1);


                var rect = new RectangleF(upleft.x, upleft.y, w, h);
                render.Render.FillRectangle(rect, brh);
                //render.DrawLine(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top/2+rect.Bottom/2),brh,20);
                //render.DrawLine(new PointF(rect.Left, rect.Bottom), new PointF(rect.Right, rect.Top / 2 + rect.Bottom / 2), brh, 20);

                //render.DrawLine(new PointF(rect.Left, rect.Top), new PointF(rect.Left, rect.Bottom), brh, 20);
            }
        }

        protected void DrawHitbox(Shape e,ColorC col)
        {
            var cam = this;
            var watch = cam.watchRect;
            {


                //  左上
                var upleft = camsoutai(cam, e.gettxy(0, 0));

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
                if (watch.w < 0)
                {
                    a = Matrix3x2.Multiply(Matrix3x2.CreateScale(-1, 1), a);

                }
                if (watch.h < 0)
                {
                    a = Matrix3x2.Multiply(Matrix3x2.CreateScale(1, -1), a);

                }

                a = Matrix3x2.Multiply(Matrix3x2.CreateRotation((float)rad, new Vector2(upleft.x, upleft.y)), a);


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



                render.Render.Transform = a;
            }


            using (var brh = this.render.Render.CreateSolidColorBrush(col))
            {
                var upleft = camsoutai(cam, e.gettxy(0, 0));
                //wも、hも絶対値で四角形を作り、transformの所で反転させて折り合いをつける。

                var w = Mathf.sameSign((camsoutai(cam, e.gettxy(e.w, 0)) - upleft).length, e.w * 0 + 1);
                var h = Mathf.sameSign((camsoutai(cam, e.gettxy(0, e.h)) - upleft).length, e.h * 0 + 1);


                var rect = new RectangleF(upleft.x, upleft.y, w, h);
                render.Render.FillRectangle(rect, brh);
                //render.DrawLine(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top/2+rect.Bottom/2),brh,20);
                //render.DrawLine(new PointF(rect.Left, rect.Bottom), new PointF(rect.Right, rect.Top / 2 + rect.Bottom / 2), brh, 20);

                //render.DrawLine(new PointF(rect.Left, rect.Top), new PointF(rect.Left, rect.Bottom), brh, 20);
            }
        }

        protected void DrawText(Shape e,string text,int size, ColorC col)
        {
            var cam = this;
            var watch = cam.watchRect;
            {


                //  左上
                var upleft = camsoutai(cam, e.gettxy(0, 0));

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
                if (watch.w < 0)
                {
                    a = Matrix3x2.Multiply(Matrix3x2.CreateScale(-1, 1), a);

                }
                if (watch.h < 0)
                {
                    a = Matrix3x2.Multiply(Matrix3x2.CreateScale(1, -1), a);

                }

                a = Matrix3x2.Multiply(Matrix3x2.CreateRotation((float)rad, new Vector2(upleft.x, upleft.y)), a);


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



                render.Render.Transform = a;
            }


            using (var brh = this.render.Render.CreateSolidColorBrush(col))
            {
                var upleft = camsoutai(cam, e.gettxy(0, 0));
                //wも、hも絶対値で四角形を作り、transformの所で反転させて折り合いをつける。

                var w = Mathf.sameSign((camsoutai(cam, e.gettxy(e.w, 0)) - upleft).length, e.w * 0 + 1);
                var h = Mathf.sameSign((camsoutai(cam, e.gettxy(0, e.h)) - upleft).length, e.h * 0 + 1);


                var rect = new RectangleF(upleft.x, upleft.y, w, h);

                var fa = Vortice.DirectWrite.DWrite.DWriteCreateFactory<IDWriteFactory>();
                var fom = fa.CreateTextFormat("MS UI Gothic", FontWeight.Light, Vortice.DirectWrite.FontStyle.Normal, size);

                render.Render.DrawText(text,fom,rect, brh);
                //render.DrawLine(new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top/2+rect.Bottom/2),brh,20);
                //render.DrawLine(new PointF(rect.Left, rect.Bottom), new PointF(rect.Right, rect.Top / 2 + rect.Bottom / 2), brh, 20);

                //render.DrawLine(new PointF(rect.Left, rect.Top), new PointF(rect.Left, rect.Bottom), brh, 20);
            }
        }
        ~Camera()
        {
            //d.removeCamera(this);//いらないよね？
            if (_isBitmap==true)
            {
                if (Mathf.isSubClassOf(render.GetType(), typeof(C3BitmapRenderSet)))
                {
                    d.BackBitMapRenderSet((C3BitmapRenderSet)render);
                }
                d.removeCamera(this);
            }
        }
        public override bool CanDraw(Camera cam)
        {
            // 自身には映らない
            if (this==cam) { return false; }
            return base.CanDraw(cam);
        }

        override public void draw(Camera cam)
        {
            if (_isBitmap)
            {
                BitmapInterpolationMode mode;
                if (linear == true)
                {
                    mode = BitmapInterpolationMode.Linear;
                }
                else
                {
                    mode = BitmapInterpolationMode.NearestNeighbor;
                }

                var render = cam.render;
                render.Render.Transform = rectTrans(cam);
                render.Render.DrawBitmap(Brender.Bitmap, rectRectF(cam), col.opa, mode, source);
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
        #region textureLoader

        public void setupTextureLoader()
        {
            texs.Clear();
            Dictionary<string, System.Drawing.Color> basebitnames = new Dictionary<string, System.Drawing.Color>
                {
                    {"red",System.Drawing.Color.Red },{"blue",System.Drawing.Color.Blue },{"green",System.Drawing.Color.Green },
                    {"white",System.Drawing.Color.White },{"gray",System.Drawing.Color.Gray },{"black",System.Drawing.Color.Black },
                    {"yellow",System.Drawing.Color.Yellow },{"cyan",System.Drawing.Color.Cyan },{"purple",System.Drawing.Color.Purple },
                    {"aqua",System.Drawing.Color.Aqua },{"brown",System.Drawing.Color.Brown },{"crimson",System.Drawing.Color.Crimson },
                    {"pink",System.Drawing.Color.Pink },{"orange",System.Drawing.Color.Orange },{"indigo",System.Drawing.Color.Indigo }

                ,{"trans",System.Drawing.Color.FromArgb(0,0,0,0) }

                };
            foreach (var a in basebitnames)
            {
                registBit(a.Key, a.Value);
            }
            //nothingを作る
            {
                CreateBitmapSS.Wait();
                int stride = 3 * sizeof(int);
                using (var tempStream = new DataStream(stride * 3, true, true))
                {
                    List<Color> cols = new List<Color>
                    {
                    Color.FromArgb(255,255,255),Color.FromArgb(128,128,128),Color.FromArgb(255,0,0),
                    Color.FromArgb(255, 0 ,255),Color.FromArgb(0,255,0)    ,Color.FromArgb(255,255,0),
                    Color.FromArgb(0,0,255)    ,Color.FromArgb(0  ,255,255),Color.FromArgb(0,0,0)
                    };
                    for (int i = 0; i < 9; i++)
                    {
                        var color = cols[i];
                        int rgba = color.R | (color.G << 8) | (color.B << 16) | (color.A << 24);
                        tempStream.Write(rgba);
                    }
                    var bitmapProperties = new BitmapProperties(new PixelFormat(Vortice.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));

                    registBmp(FileMan.c_nothing, render.Render.CreateBitmap(new System.Drawing.Size(3, 3), tempStream.BasePointer, sizeof(int), bitmapProperties));

                }
                CreateBitmapSS.Release();
            }
        }
        //画像を読み込むとき、透明とみなす色。
        public Color transColor = Color.FromArgb(0, 254, 254);

        /// <summary>
        /// 読み込んだテクスチャーを保存しとく
        /// </summary>
        Dictionary<string, ID2D1Bitmap> texs = new Dictionary<string, ID2D1Bitmap>();



        /// <summary>
        /// texsに色のついたビットを追加する。
        /// </summary>
        /// <param name="bitname">bitname+"bit"</param>
        /// <param name="color">色</param>
        private void registBit(string bitname, System.Drawing.Color color)
        {
            using (var tempStream = new DataStream(1, true, true))
            {
                CreateBitmapSS.Wait();
                int rgba = color.R | (color.G << 8) | (color.B << 16) | (color.A << 24);
                tempStream.Write(rgba);
                var bitmapProperties = new BitmapProperties(new PixelFormat(Vortice.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                registBmp(bitname + "bit", render.Render.CreateBitmap(new System.Drawing.Size(1, 1), tempStream.BasePointer, sizeof(int), bitmapProperties));
                CreateBitmapSS.Release();
            }

        }

        /// <summary>
        /// bmpをtexsに登録する
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bmp"></param>
        /// <param name="dispouwagaki">同じ名前のbmpを登録するときdisposeするか。Renderからのbmpを登録してる場所にはfalseじゃないとだめ</param>
        public void registBmp(string name, ID2D1Bitmap bmp, bool dispouwagaki = true)
        {
            {
                name = FileMan.slashFormat(name);
                name = FileMan.bmpExtention(name);
            }

            if (texs.ContainsKey(name))
            {

                if (dispouwagaki) texs[name]?.Dispose();
                texs[name] = bmp;
            }
            else
            {
                //if (bmp != null) Console.WriteLine(bmp.PixelSize.ToString());
                Debug.WriteLine(name + " load sitao!");
                texs.Add(name, bmp);
            }
        }


        /// <summary>
        /// bmpがnullか調べる
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static public bool bmpisnull(ID2D1Bitmap bmp)
        {

            return bmp == null;
        }


        private SemaphoreSlim CreateBitmapSS = new SemaphoreSlim(1, 1);

        /// <summary>
        /// bitmapテクスチャーを読み込む。既に読み込んでいた場合は読み込まずに返す。
        /// .bmpはつけてもつけなくてもいい
        /// </summary>
        /// <param name="file">.\tex\に続くファイルパス</param>
        /// <param name="reset">強制的に再読み込みする</param>
        /// <returns>clmcolを透明にしたビットマップ</returns>
        public ID2D1Bitmap ldtex(string file, bool reset = false)
        {
            {
                file = FileMan.slashFormat(file);
                file = FileMan.bmpExtention(file);
            }

            if (!texs.ContainsKey(file) || reset)
            {
                string path = FileMan.s_rootpath + @"tex\" + file;
                if (!File.Exists(path))
                {
                    Debug.WriteLine("texture " + path + " not exists");
                    registBmp(file, null);
                    return texs[FileMan.bmpExtention(FileMan.c_nothing)];
                }

                // System.Drawing.Imageを使ってファイルから画像を読み込む
                {
                    using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(path))
                    {
                        // BGRA から RGBA 形式へ変換する
                        // 1行のデータサイズを算出
                        int stride = bitmap.Width * sizeof(int);
                        using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                        {
                            // 読み込み元のBitmapをロックする
                            var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                            var bitmapData = bitmap.LockBits(sourceArea, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                            // 変換処理
                            for (int y = 0; y < bitmap.Height; y++)
                            {
                                int offset = bitmapData.Stride * y;
                                for (int x = 0; x < bitmap.Width; x++)
                                {

                                    // 1byteずつデータを読み込む
                                    byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                    byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                    byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                    byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                    //Console.WriteLine(B + " " + G + " " + R + " " + A);
                                    byte a = 0;
                                    //透明
                                    int gaba = a | (a << 8) | (a << 16) | (a << 24);
                                    //tempStream.Write(gaba);


                                    int rgba = R | (G << 8) | (B << 16) | (A << 24);
                                    if (R == transColor.R && G == transColor.G && B == transColor.B) tempStream.Write(gaba);
                                    else
                                        tempStream.Write(rgba);


                                }
                            }
                            // 読み込み元のBitmapのロックを解除する
                            bitmap.UnlockBits(bitmapData);
                            tempStream.Position = 0;

                            // 変換したデータからBitmapを生成して返す
                            CreateBitmapSS.Wait();
                            var size = new System.Drawing.Size(bitmap.Width, bitmap.Height);
                            var bitmapProperties = new BitmapProperties(new PixelFormat(Vortice.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));

                            var result = render.Render.CreateBitmap(size, tempStream.BasePointer, stride, bitmapProperties);
                            registBmp(file, result);
                            CreateBitmapSS.Release();
                        }
                    }
                }

            }
            // Console.WriteLine(texs.Count() + "texcount");
            return texs[file];
        }
        /// <summary>
        /// テクスチャーの大きさだけ取得する
        /// </summary>
        /// <param name="file">.\tex\に続くファイルパス</param>
        /// <param name="reset">強制的に再読み込みする</param>
        /// <returns>大きさ</returns>
        public FXY texSize(string file, bool reset = false)
        {
            var tex = ldtex(file, reset);
            if (tex == null)
            {
                return new FXY(0, 0);
            }
            return new FXY(tex.Size.Width, tex.Size.Height);
        }
        #endregion


        /// <summary>
        /// カメラの標準の名前
        /// </summary>
        public const string CameraName = "Camera";
        /// <summary>
        /// カメラのWatchRectの標準の名前
        /// </summary>
        public const string WatchRectName = "WatchRect";
        public C3RenderSet render { get { return _render; } }
        C3RenderSet _render;


        /// <summary>
        /// スクリーンショットに使うレンダー      
        /// </summary>
        C3BitmapRenderSet _SCSRender;


        /// <summary>
        /// Textの一次的な描画に使うレンダーに使うレンダー      
        /// </summary>
        C3BitmapRenderSet _TextRender;

        /// <summary>
        /// 画像のブレンドに一次的に使うレンダーに使うレンダー      
        /// </summary>
        C3BitmapRenderSet _BlendRender;


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
            _SCSRender = new C3BitmapRenderSet(render.Render.CreateCompatibleRenderTarget(si, si
                , fom
                , CompatibleRenderTargetOptions.GdiCompatible));

            var TextRSize = new System.Drawing.Size((int)((wi + hei) * resolution)*3, (int)((wi + hei) * resolution)*3);
            _TextRender = new C3BitmapRenderSet(render.Render.CreateCompatibleRenderTarget(TextRSize, TextRSize
                , fom
                , CompatibleRenderTargetOptions.GdiCompatible));

            _TextRender.BitmapRender.BeginDraw();

            _TextRender.BitmapRender.Clear(new ColorC(1, 0, 0, 1));//異常チェック用に赤に塗っとく
            _TextRender.BitmapRender.EndDraw();

            _BlendRender = new C3BitmapRenderSet(render.Render.CreateCompatibleRenderTarget(TextRSize, TextRSize
                , fom
                , CompatibleRenderTargetOptions.GdiCompatible));
            
            setupTextureLoader(); 
        }

        /// <summary>
        /// 新しくビットマップレンダーを作る
        /// </summary>
        /// <returns>必ず返すように！</returns>
        internal C3BitmapRenderSet GetBitMapRenderSet()
        {
            //残ってないときに作成
            if (BitmapCameraRenderSetTaiki.Count == 0) {
                var fom = new Vortice.DCommon.PixelFormat();

                var TextRSize = new System.Drawing.Size((int)((_render.Render.Size.Width)) , (int)((_render.Render.Size.Height)) );
                var tempRender = new C3BitmapRenderSet(render.Render.CreateCompatibleRenderTarget(TextRSize, TextRSize
                    , fom
                    , CompatibleRenderTargetOptions.GdiCompatible));
                BitmapCameraRenderSetTaiki.Add(tempRender);
                BitmapCameraRenderSet.Add(tempRender);
            }
            var res = BitmapCameraRenderSetTaiki[0];
            BitmapCameraRenderSetTaiki.RemoveAt(0);
            return res;
        }

        /// <summary>
        /// ビットマップレンダーを返す
        /// </summary>
        /// <param name="back">返すときはしょうめつするとき！</param>
        internal void BackBitMapRenderSet(C3BitmapRenderSet back)
        {
            BitmapCameraRenderSetTaiki.Add(back);
        }

        List<CP<Camera>> cameras = new List<CP<Camera>>();

        /// <summary>
        /// 画像を作るカメラのレンダーを貯めておく
        /// </summary>
        List<C3BitmapRenderSet> BitmapCameraRenderSet=new List<C3BitmapRenderSet>();

        /// <summary>
        /// 画像を作るカメラのレンダーのもう待機に入ってて仕える奴等
        /// </summary>
        List<C3BitmapRenderSet> BitmapCameraRenderSetTaiki = new List<C3BitmapRenderSet>();
        /// <summary>
        /// 画面に直接描画するカメラを作る。Cameraが追加されてるEntityはマジどうでもいい
        /// </summary>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        private Camera makeCamera(Entity Watchrect, ColorC backcolor)
        {
            Entity back = Entity.make2(0, 0, render.Render.Size.Width, render.Render.Size.Height, name: CameraName);
            CP<Camera> res;
            res = Component.ToPointer(new Camera(Watchrect, 0, backcolor, render, this));
            res.add(back);

            cameras.Add(res);

            return res.c;
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
        /// ワールドにおくカメラを返す。
        /// </summary>
        /// <param name="backcolor"></param>
        /// <returns></returns>
        public CP<Camera> makeBitmapCamera(ColorC backcolor) 
        {
            Entity Watchrect = Entity.make2(0, 0, render.Render.Size.Width / resol, render.Render.Size.Height / resol, name: WatchRectName);

            Entity back = Entity.make2(0, 0, render.Render.Size.Width, render.Render.Size.Height, name: CameraName);
            CP<Camera> res;
            res = Component.ToPointer(new Camera(Watchrect, 0, backcolor, GetBitMapRenderSet(), this));
            res.add(back);

            //ついかしないとPreDrawができない　　
            //cameras.Add(res);
            return res;

        }

        /// <summary>
        /// Cameraをdisplayの雑な管理から外す
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public bool removeCamera(Camera c)
        {
            for (int i = cameras.Count - 1; i >= 0; --i) 
            {
                if (cameras[i].c == c)
                {
                    cameras.RemoveAt(i);
                    return true;
                }
            }
            return false;   
        }
        public void clearCamera()
        {
            cameras.Clear();
        }

        /// <summary>
        /// カメラを描画(Updateもするよあんま意味ないけど)する。カメラごとの描画順は雑。
        /// </summary>
        /// <param name="cl">カメラのフレームスピード</param>
        /// <param name="cameras">描画するカメラ。このカメラはWorldに所属していない方がいい。(二重にUpdateされちゃうし)<br></br>
        /// また、Cameraはもちろんこのdisplayから創出された画面用のカメラ</param>
        /// <param name="AddCameras">追加で描画するカメラ 順番通りになる</param>
        public void draw(float cl = 1, List<CP<Camera>> AddCameras = null)
        {
            ThreadNum = 0;
            PreDraw(cl,AddCameras);
            render.Render.BeginDraw();

            foreach (CP<Camera> a in new List<CP<Camera>>(cameras))
            {
                if (a.c.watchRect.added && a.c.IsBitmap==false)
                {
                    a.c.e.update(cl);
                    ThreadNum += 1;
                }

            }
            if (AddCameras != null)
            {
                foreach (CP<Camera> a in AddCameras)
                {
                    if (a.c.watchRect.added)
                    {
                        a.c.e.update(cl);
                        ThreadNum += 1;
                    }
                }
            }
            render.Render.EndDraw();
        }

        static public int ThreadNum = 0;

        //事前描画のセマフォア
        private DisplaySemaphores Semaphores =new DisplaySemaphores();

        /// <summary>
        /// テキストのやつなど、事前に描画が必要なやつなどをまとめて処理する
        /// </summary>
        /// <param name="cl"></param>
        protected void PreDraw(float cl, List<CP<Camera>> AddPredraws=null)
        {
            _TextRender.BitmapRender.BeginDraw();
            foreach (var a in new List<CP<Camera>>(cameras))
            {
                a.c.PreDraw(a,Semaphores);
            }
            if (AddPredraws != null) 
            {
                foreach (var a in AddPredraws) 
                {
                    a.c.PreDraw(a, Semaphores);
                }
            }
            _TextRender.BitmapRender.EndDraw();
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
                if (a.c.render == render)
                {
                    a.c.render = _SCSRender;
                }
            }
            _SCSRender.Render.BeginDraw();
            draw(0);
            _SCSRender.Render.EndDraw();
            foreach (var a in cameras)
            {
                if (a.c.render == _SCSRender)
                {
                    a.c.render = render;
                }
            }
            screenShot(_SCSRender,render);
//            screenShot(_TextRender,render);
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
        static public List<List<ColorC>> GetPixels(C3BitmapRenderSet image, Rectangle zone = null)
        {

            //image.Bitmap1.CopyFromBitmap(image.BitmapRender.Bitmap);

            image.Bitmap1.CopyFromBitmap(image.Render.CreateSharedBitmap(image.BitmapRender.Bitmap
                , new BitmapProperties(image.BitmapRender.PixelFormat)));


            var BitmapMap = image.Bitmap1.Map(MapOptions.Read);

            var size = (BitmapMap.Pitch * image.BitmapRender.PixelSize.Height);
            byte[] bytes = new byte[size];
            Marshal.Copy(BitmapMap.Bits, bytes, 0, size);


            //bitmap1.Unmap();
            //bitmap1.Dispose();
            //bitmap1.Release();

            //bitmap1.Dispose();
            var res = new List<List<ColorC>>();

            if (zone == null)
            {
                zone = new Rectangle(0, 0, image.BitmapRender.PixelSize.Width, image.BitmapRender.PixelSize.Height);
            }

            for (int y = (int)zone.y; y < image.BitmapRender.PixelSize.Height && y < zone.y + zone.h; y++)
            {
                res.Add(new List<ColorC>());
                var yy = y;

                for (int x = (int)zone.x; x < image.BitmapRender.PixelSize.Width && x < zone.x + zone.w; x++)
                {
                    var position = BitmapMap.Pitch * yy + x * (BitmapMap.Pitch / image.BitmapRender.PixelSize.Width);
                    var c = new ColorC(bytes[position + 2] / 255f, bytes[position + 1] / 255f
                        , bytes[position + 0] / 255f, bytes[position + 3] / 255f);
                    res[yy - (int)zone.y].Add(c);

                }
            }

            image.Bitmap1.Unmap();
            return res;
        }



        /// <summary>
        /// 画面のスクリーンショットを取る。effectcharaがぶれるバグあり！
        /// </summary>
        /// <param name="h">保存する表示マン</param>
        /// <param name="format">保存フォーマット</param>
        /// <param name="addname">追加で付ける名前</param>
        static public void screenShot(C3BitmapRenderSet bt,C3RenderSet renderset, string format = "png", string addname = "")
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


            var pxs = GetPixels(bt);

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
            for (int i = textRenderersRemove.Count - 1; i >= 0; i--)
            {
                textRenderers.Remove(textRenderersRemove[i]);
                textRenderersRemove.RemoveAt(i);
            }
            //Debug.WriteLine("make TextRendere" + w + " :: " + h);
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
                        , np.x - _TextRender.BitmapRender.Size.Width, np.y))
                    {
                        maxx = Mathf.max(b.rendZone.gettxy(b.rendZone.w, 0).x, maxx);
                    }
                }
                foreach (var b in textRenderers)
                {
                    if (onHani(b.rendZone, np.x, np.y
                        , np.x, np.y - _TextRender.BitmapRender.Size.Height))
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

            hanteis.Add(Task.Run(() => { addrect(new FXY(_TextRender.BitmapRender.Bitmap.Size.Width - 1, _TextRender.BitmapRender.Bitmap.Size.Height - 1)); }));

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
                }
                ));
            }
            foreach (var a in hanteis) 
            {
                a.Wait();
            }
            foreach (var task in hanteis) { task.Dispose(); }
            hanteis.Clear();
            
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
            var d2dContext = _BlendRender.DeviceContext;


            //_BlendRender.PushAxisAlignedClip(new Rectangle(0,0,bitmap.PixelSize.Width, bitmap.PixelSize.Height)
            //    , AntialiasMode.PerPrimitive);


            var blend = new Vortice.Direct2D1.Effects.ColorMatrix(d2dContext);
            blend.SetInput(0, bitmap, new SharpGen.Runtime.RawBool(true));

            var trans=new Vortice.Direct2D1.Effects.AffineTransform2D(d2dContext);
            trans.SetInputEffect(0,blend, new SharpGen.Runtime.RawBool(true));

            var colormatrix = new Matrix5x4(
            color.r,0,0,0,
            0,color.g,0,0,
            0,0,color.b,0,
            0,0,0,1,
            0,0,0,0
            );

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

            return _BlendRender.BitmapRender.Bitmap;

        }
    }
}



