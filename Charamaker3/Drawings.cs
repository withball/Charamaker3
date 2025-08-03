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
    #region cameraUtils
    /// <summary>
    /// カメラの動きに対してどのぐらい動くかを変化させる。無理やりね。<br></br>
    /// これによってe.setTxとかもおかしくなる。<br></br>
    /// 二つ追加はできない
    /// </summary>
    public class Haikei : Component
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
        public Haikei(float px, float py, Camera cam)
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
        protected Camera cam = null;
        /// <summary>
        /// 本来の位置
        /// </summary>
        protected float truex, truey;

        /// <summary>
        /// 前回のの位置
        /// </summary>
        protected float prex, prey;
        public override bool add(Entity ee, float cl = 0)
        {
            if (ee.getcompos<Haikei>().Count == 0)
            {
                return base.add(ee, cl);
            }
            Debug.WriteLine("!!ERROR!! " + this.GetType() + " を同じエンテティに二つ追加しようとしたのでキャンセルしましたわ");
            return false;
        }
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
        /// 左上座標をカメラから見ての座標にするよ。
        /// </summary>
        /// <param name="e">そうしたいエンテティ</param>
        /// <param name="tx">移動させたい中心位置</param>
        /// <param name="ty">移動させたい中心位置</param>
        /// <returns>このエンテティがHaikeiを持ち、カメラから見ての座標に移動したか</returns>
        static public bool SetCameraZahyou(Entity e, float tx, float ty)
        {
            var lis = e.getcompos<Haikei>();
            if (lis.Count <= 0) return false;
            var h = lis[0];
            //prex == truex + center.x*(px - 1) == tx(によって作られる左上座標x)
            h.setZahyou(false);//清算
            if (h.cam == null)
            {
                throw new Exception("追従するカメラが無いですやん");
            }
            var center = h.cam.watchRect.gettxy();
            e.settxy(tx, ty);
            //この辺を逆算
            h.prex = e.x;
            h.prey = e.y;
            h.truex = h.prex - center.x * (h.px - 1);
            h.truey = h.prey - center.y * (h.py - 1);


            return true;
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
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return ReferenceEquals(a, null) == ReferenceEquals(b, null);
            }

            return a.r == b.r && a.g == b.g && a.b == b.b && a.opa == b.opa;
        }
        public static bool operator !=(ColorC a, ColorC b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return ReferenceEquals(a, null) != ReferenceEquals(b, null);
            }
            return a.r != b.r || a.g != b.g || a.b != b.b || a.opa != b.opa;
        }

        public DataSaver ToSave()
        {
            var d = new DataSaver();
            d.packAdd("r", r);
            d.packAdd("g", g);
            d.packAdd("b", b);
            d.packAdd("opa", opa);
            return d;
        }
        public void ToLoad(DataSaver d)
        {
            r = d.unpackDataF("r");
            g = d.unpackDataF("g");
            b = d.unpackDataF("b");
            opa = d.unpackDataF("opa");
        }
    }
    public abstract class Drawable : Component
    {
        /// <summary>
        /// 色。大体透明度しか意味ない。
        /// </summary>
        public ColorC col = new ColorC(0, 0, 0, 0);
        /// <summary>
        /// 描画の順番
        /// </summary>
        public float z;


        /// <summary>
        /// 描画順番のプラスされる値。保存されない
        /// </summary>
        public float zDelta=0;

        /// <summary>
        /// 描画順番の倍率。保存されない
        /// </summary>
        public float zRatio=1;


        /// <summary>
        /// 線形補完をするか。一個一個じゃなくてプログラム側で一気に変えた方が良き。回転するとよくなくなる。
        /// </summary>
        public bool linear = true;
        public Drawable(float z, ColorC col, float time = -1, string name = "") : base(time, name)
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

            cc.zDelta = this.zDelta;
            cc.zRatio = this.zRatio;
            cc.linear = this.linear;

            cc.col = new ColorC(this.col);
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("z", this.z);
            res.packAdd("zDelta", this.zDelta);
            res.packAdd("zRatio", this.zRatio);
            res.linechange();
            res.packAdd("linear", this.linear);
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
            this.zDelta=d.unpackDataF("zDelta", this.zDelta);
            this.zRatio=d.unpackDataF("zRatio", this.zRatio);

            this.linear = d.unpackDataB("linear",linear);
            this.col.r = d.unpackDataF("r");
            this.col.g = d.unpackDataF("g");
            this.col.b = d.unpackDataF("b");
            this.col.opa = d.unpackDataF("opa");
        }
        
        /// <summary>
        /// 描画可能な条件
        /// </summary>
        /// <returns></returns>
        virtual public bool CanDraw(Camera cam) 
        {
            return col.opa > 0 && onCamera(cam);
        }


        /// <summary>
        /// これをする前にCandrawで確かめておくこと！
        /// </summary>
        /// <param name="cam"></param>
        public abstract void draw(Camera cam);

        /// <summary>
        /// これをする前にCandrawで確かめておくこと！
        /// </summary>
        /// <param name="cam"></param>
        public virtual void PreDraw(Camera cam, DisplaySemaphores semaphores) { }

        protected bool onCamera(Camera cam)
        {
            if (e != null)
            {
                var s = new Shapes.Rectangle(0);
                s.setto(cam.watchRect);


                var s2 = new Shapes.Rectangle(0);
                s2.setto(this.e);
                return s.atarun(s2);
            }
            return false;
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



            return a;
        }
        /// <summary>
        /// 四角形の移動とスケールも含めたトランスフォーム
        /// </summary>
        /// <param name="cam"></param>
        /// <returns></returns>
        protected Matrix3x2 rectTransMax(Camera cam,float bitmapW,float bitmapH)
        {

            if (bitmapW <= 0 || bitmapH <= 0)
            {
                return Matrix3x2.CreateScale(0, 0);
            }
            var watch = cam.watchRect;

            //  左上
            var upleft = camsoutai(cam, e.gettxy2(0, 0));
            var upright = camsoutai(cam, e.gettxy2(1, 0));
            //右下
            var leftdown = camsoutai(cam, e.gettxy2(0, 1));
            var rightdown = camsoutai(cam, e.gettxy2(1, 1));

            float w = (upright - upleft).length;
            float h = (upright - rightdown).length;
            //0.5の中心
            var center = camsoutai(cam, e.gettxy(e.w / 2, e.h / 2));


            //回転の合計
            var rad = Mathf.toRadian(-watch.degree + e.degree);
            var a = Matrix3x2.CreateScale(1,1);
            if (watch.mirror)
            {
                a = Matrix3x2.Multiply(Matrix3x2.CreateScale(-1, 1), a);

                a = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(-1*watch.w,0), a);
                //Debug.WriteLine($"{a.M11} {a.M12} \n {a.M21} {a.M22} \n {a.M31} {a.M32} \n");
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

            a = Matrix3x2.Multiply( Matrix3x2.CreateTranslation(upleft.x, upleft.y),a);
          






            a = Matrix3x2.Multiply(Matrix3x2.CreateRotation((float)rad, new Vector2(0, 0)), a);


            a = Matrix3x2.Multiply(Matrix3x2.CreateScale(w, h), a);
            a = Matrix3x2.Multiply(Matrix3x2.CreateScale(1 / (bitmapW), 1 / (bitmapH)), a);



            if (e.mirror)
            {
                a = Matrix3x2.Multiply(new Matrix3x2(-1, 0, 0, 1, 0, 0), a);
                // a = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(-(upleft.x) * 2, 0), a);
            }
            if (e.w < 0)
            {
                a = Matrix3x2.Multiply(new Matrix3x2(-1, 0, 0, 1, 0, 0), a);

                // a = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(-(w) * 2, 0), a);
            }


            if (e.h < 0)
            {
                a = Matrix3x2.Multiply(new Matrix3x2(1, 0, 0, -1, 0, 0), a);

                //a = Matrix3x2.Multiply(Matrix3x2.CreateTranslation(0, -(h) * 2), a);
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

            var w = Mathf.sameSign((camsoutai(cam, e.gettxy(e.w, 0)) - upleft).length, e.w * 0 + 1);
            var h = Mathf.sameSign((camsoutai(cam, e.gettxy(0, e.h)) - upleft).length, e.h * 0 + 1);


            return new RectangleF(upleft.x, upleft.y, w, h);
        }


    }
    /// <summary>
    /// 描画可能な四角形
    /// </summary>
    public class DRectangle : Drawable
    {
        public DRectangle(float z, ColorC c, float time = -1, string name = "") : base(z, c, time, name)
        {
        }
        public DRectangle() : base()
        {
        }
        public override void draw(Camera cam)
        {
            var render = cam.render.Render;

            render.Transform = rectTrans(cam);
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

    /// <summary>
    /// 描画可能な三角形
    /// </summary>
    public class DTriangle : Drawable
    {
        //頂点の位置
        float wariai = 0.5f;
        public DTriangle(float wariai,float z, ColorC c, float time = -1, string name = "") : base(z, c, time, name)
        {
            this.wariai = wariai;
        }
        public DTriangle() : base()
        {
        }
        public override DataSaver ToSave()
        {
            var ret = base.ToSave();
            ret.packAdd("wariai", wariai);
            return ret;
        }
        protected override void ToLoad(DataSaver d)
        {
            this.wariai = d.unpackDataF("wariai", wariai);
            base.ToLoad(d);
        }
        public override void copy(Component c)
        {
            var cc = (DTriangle)c;
            base.copy(c);
            cc.wariai = this.wariai;
        }

        public override void draw(Camera cam)
        {
            //TODO:メモリリークメモリリークとかしてたらごめん
            var render = cam.render.Render;

            render.Transform = Matrix3x2.CreateTranslation(0,0);//rectTrans(cam);
            //var rect = rectRectF(cam);

            using (var brh = render.CreateSolidColorBrush(col))
            {
                ID2D1PathGeometry geometry = render.Factory.CreatePathGeometry();

                ID2D1GeometrySink sink = geometry.Open();

                var leftup= camsoutai(cam,e.gettxy2(0, 0));
                var leftdown = camsoutai(cam, e.gettxy2(0, 1));
                var rightup = camsoutai(cam, e.gettxy2(1, wariai));
                sink.BeginFigure(new PointF(leftup.x,leftup.y),FigureBegin.Filled);

                sink.AddLine(new PointF(leftdown.x, leftdown.y));
                sink.AddLine(new PointF(rightup.x, rightup.y));
                sink.AddLine(new PointF(leftup.x, leftup.y));

                sink.EndFigure(FigureEnd.Closed);

                sink.Close();

                render.FillGeometry(geometry,brh);
                render.DrawGeometry(geometry, brh);
            }

        }
    }

    public class Texture : Drawable
    {
        /// <summary>
        /// 登録されているテクスチャ―
        /// </summary>
        public Dictionary<string, string> textures = new Dictionary<string, string>();
        /// <summary>
        /// 今選択されているテクスチャの名前
        /// </summary>
        public string texname = "def";
        /// <summary>
        /// 現在のテクスチャー
        /// </summary>
        public string nowtex { get { if (textures.ContainsKey(texname)) return textures[texname];
                return FileMan.c_nothing;
            } }

        /// <summary>
        /// 書き込み元のレクタングル(割合ですよ)
        /// </summary>
        public float CropL = 0;
        public float CropR = 1;
        public float CropU = 0;
        public float CropD = 1;

        public Texture(float z, ColorC c, Dictionary<string, string> texs, float time = -1, string name = "") : base(z, c, time, name)
        {
            textures = new Dictionary<string, string>(texs);
            if (textures.Keys.Count > 0)
            {
                texname = new List<string>(textures.Keys)[0];
            }
        }
        public Texture() : base()
        { }

        public override void copy(Component c)
        {
            var cc = (Texture)c;
            base.copy(c);
            cc.texname = this.texname;
            cc.textures = new Dictionary<string, string>(textures);
            cc.CropL = this.CropL;
            cc.CropR = this.CropR;
            cc.CropU = this.CropU;
            cc.CropD = this.CropD;
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("texname", texname);
            res.linechange();
            var d = new DataSaver();
            foreach (var a in textures)
            {
                d.packAdd(a.Key, a.Value);
                d.linechange();
            }
            d.indent();
            res.packAdd("textures", d);
            res.linechange();
            res.packAdd("CropL", CropL);
            res.packAdd("CropR", CropR);
            res.packAdd("CropU", CropU);
            res.packAdd("CropD", CropD);
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

            CropL = d.unpackDataF("CropL", CropL);
            CropR = d.unpackDataF("CropR", CropR);
            CropU = d.unpackDataF("CropU", CropU);
            CropD = d.unpackDataF("CropD", CropD);
        }
        static public Texture make(float z, float opa, params KeyValuePair<string, string>[] textures)
        {
            var res = new Texture(z, new ColorC(1, 1, 1, opa), new Dictionary<string, string>());
            foreach (var a in textures)
            {
                res.textures.Add(a.Key, a.Value);
            }
            if (textures.Length > 0)
            {
                res.texname = textures[0].Key;
            }
            return res;
        }


        public override void draw(Camera cam)
        {
            var render = cam.render.Render;
           

            ID2D1Bitmap bitmap;
            bitmap = cam.d.ldtex(nowtex);
            if (bitmap == null)
            {
                bitmap = cam.d.ldtex(FileMan.c_nothing);
            }


            //色ついてない場合高速描画
            if (this.col.r == 1&&this.col.g == 1 && this.col.b == 1)
            {
                var rect = rectRectF(cam);
                render.Transform = rectTrans(cam);
                //var blended = cam.d.Blend(bitmap, this.col);
                BitmapInterpolationMode mode ;
                if (linear == true)
                {
                    mode = BitmapInterpolationMode.Linear;
                }
                else
                {
                    mode = BitmapInterpolationMode.NearestNeighbor;
                }
                render.DrawBitmap(bitmap
                       , rect
                       , this.col.opa, mode
                       , new RawRectF(bitmap.Size.Width * CropL, bitmap.Size.Height * CropU
                       , bitmap.Size.Width * CropR - 0.0f, bitmap.Size.Height * CropD - 0.0f));//嘘->-0.5しないと1マスが2マスになって絶望したりする。
            }
            else if(1==1)
            {

                render.Transform =Matrix3x2.CreateScale(1,1);
                //_BlendRender.PushAxisAlignedClip(new Rectangle(0,0,bitmap.PixelSize.Width, bitmap.PixelSize.Height)
                //    , AntialiasMode.PerPrimitive);


                /////////////////////////////クロップ
              

                var crop0 = cam.render.ECrop0;
                crop0.SetInput(0, bitmap, new SharpGen.Runtime.RawBool(true));
                crop0.BorderMode = BorderMode.Hard;


                Rectangle sourceRect;

                //ビットマップが小さすぎると計算誤差で死んでしまうので最低の幅を保証する。->ホント？

                float hosyo = 10;
                /*
                if (bitmap.PixelSize.Width < hosyo && bitmap.PixelSize.Height < hosyo)
                {
                    crop0.TransformMatrix = Matrix3x2.CreateScale(hosyo / bitmap.PixelSize.Width
                        , hosyo / bitmap.PixelSize.Height);
                    sourceRect = new Rectangle(hosyo * CropL - 0.0f, hosyo * CropU - 0.0f
                  , hosyo * CropR - hosyo * CropL, hosyo * CropD - hosyo * CropU);

                }
                else if (bitmap.PixelSize.Width < hosyo)
                {
                    crop0.TransformMatrix = Matrix3x2.CreateScale(hosyo / bitmap.PixelSize.Width
                       , 1);
                    sourceRect = new Rectangle(hosyo * CropL - 0.0f, bitmap.Size.Height * CropU - 0.0f
                  , hosyo * CropR - hosyo * CropL, bitmap.PixelSize.Height * CropD - bitmap.Size.Height * CropU);
                }
                else if (bitmap.PixelSize.Height < hosyo)
                {
                    crop0.TransformMatrix = Matrix3x2.CreateScale(1
                           , hosyo / bitmap.PixelSize.Height);
                    sourceRect = new Rectangle(bitmap.Size.Width * CropL - 0.0f, hosyo * CropU - 0.0f
                  , bitmap.PixelSize.Width * CropR - bitmap.Size.Width * CropL, hosyo * CropD - hosyo * CropU);
                }
                else*/
                {
                    crop0.TransformMatrix = Matrix3x2.CreateScale(1,1);
                    sourceRect = new Rectangle(bitmap.Size.Width * CropL - 0.0f, bitmap.Size.Height * CropU - 0.0f
                  , bitmap.PixelSize.Width * CropR - bitmap.Size.Width * CropL, bitmap.PixelSize.Height * CropD - bitmap.Size.Height * CropU);
                }

                var crop1 = cam.render.ECrop1;
                crop1.SetInputEffect(0, crop0, new SharpGen.Runtime.RawBool(false));
                crop1.BorderMode = BorderMode.Hard;




                crop1.Rectangle = new Vector4(sourceRect.x,sourceRect.y,sourceRect.x+sourceRect.w, sourceRect.y + sourceRect.h);
                
                var crop2 = cam.render.ECrop2;
                crop2.SetInputEffect(0, crop1, new SharpGen.Runtime.RawBool(false));

                crop2.TransformMatrix = Matrix3x2.CreateTranslation(-sourceRect.x,-sourceRect.y);
                //crop.Rectangle = new Vector4(0, 0, 5, 5);

                /////////////////////////色

                var blend = cam.render.EBlend;
                blend.SetInput(0, bitmap, new SharpGen.Runtime.RawBool(false));

                var colormatrix = new Matrix5x4();
                {
                    colormatrix.M11 = col.r;
                    colormatrix.M12 = 0;
                    colormatrix.M13 = 0;
                    colormatrix.M14 = 0;
                    colormatrix.M21 = 0;
                    colormatrix.M22 = col.g;
                    colormatrix.M23 = 0;
                    colormatrix.M24 = 0;
                    colormatrix.M31 = 0;
                    colormatrix.M32 = 0;
                    colormatrix.M33 = col.b;
                    colormatrix.M34 = 0;
                    colormatrix.M41 = 0;
                    colormatrix.M42 = 0;
                    colormatrix.M43 = 0;
                    colormatrix.M44 = col.opa;
                    colormatrix.M51 = 0;
                    colormatrix.M52 = 0;
                    colormatrix.M53 = 0;
                    colormatrix.M54 = 0;
                }
                blend.Matrix = colormatrix;

                var trans = cam.render.ETrans;
                trans.SetInputEffect(0, blend, new SharpGen.Runtime.RawBool(false));

                var transmatrix = rectTransMax(cam, sourceRect.w,sourceRect.h);
                //Debug.WriteLine($"{bitmap.PixelSize.Width},asd {bitmap.PixelSize.Height}");
                trans.TransformMatrix = transmatrix;
                //_BlendRender.BeginDraw();


                //d2dContext.Clear(new ColorC(0, 0, 0, 0));
                //_BlendRender.Clear(new ColorC(0, 0, 0, 0));

                var rect = rectRectF(cam);
                //d2dContext.PushAxisAlignedClip(rect, AntialiasMode.PerPrimitive);
                cam.render.DeviceContext.DrawImage(trans,InterpolationMode.NearestNeighbor,CompositeMode.SourceOver);


            }

        }

    }

    #region Text
    /// <summary>
    /// フォントを扱うクラス
    /// </summary>
    public class FontC
    {
        //EncodehはUTF-8BOMつきでお願い。
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
            justify,
            /// <summary>
            /// 無効な値
            /// </summary>
            None
        }

        /// <summary>
        /// フォントの名前
        /// </summary>
        public string fontName = "MS UI Gothic";

        /// <summary>
        /// 斜体
        /// </summary>
        public int isItaric = 0;

        /// <summary>
        /// 太字
        /// </summary>
        public int isBold = 0;
        /// <summary>
        /// フォントサイズ
        /// </summary>
        public float size = 16;


        /// <summary>
        /// ふちをずらす割合。0でふちは無し
        /// </summary>
        public float hutiZure = 0.00f;
        /// <summary>
        /// ふちの色。透明度は関係ない。
        /// </summary>
        public ColorC hutiColor = new ColorC(1, 1, 1, 0);

        /// <summary>
        /// 文字を描画できる範囲描画の範囲
        /// </summary>
        public float w = 1, h = 1;

        /// <summary>
        /// テキストの横のアライメント
        /// </summary>
        public alignment ali = alignment.left;


        /// <summary>
        /// テキストの縦のアライメントあんま信用しないで。
        /// </summary>
        public alignment aliV = alignment.left;

        /// <summary>
        /// フォントのコンストラクタ
        /// </summary>
        /// <param name="size">フォントサイズ</param>
        /// <param name="w">文字を書き込める範囲(行の幅に近い)</param>
        /// <param name="h">文字を書き込める範囲(列の数に近い)</param>
        /// <param name="fontName">フォントの名前</param>
        /// <param name="isItaric">斜体</param>
        /// <param name="isBold">太字</param>
        /// <param name="alignment">テキストの位置</param>
        /// <param name="alignmentV">テキストの縦の位置</param>
        public FontC(float size, float w, float h, string fontName = "MS UI Gothic"
            , int isItaric = 0, int isBold = 0, alignment alignment = alignment.left
            , alignment alignmentV = alignment.left)
        {
            this.size = size;
            this.fontName = fontName;
            this.isItaric = isItaric;
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
            c.size = this.size;
            c.w = this.w;
            c.h = this.h;
            c.fontName = this.fontName;
            c.isItaric = this.isItaric;
            c.isBold = this.isBold;
            c.ali = this.ali;
            c.aliV = this.aliV;
            c.hutiZure = this.hutiZure;
            c.hutiColor = new ColorC(this.hutiColor);
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
            d.linechange();
            d.packAdd("alignment", ali);
            d.packAdd("alignmentV", aliV);
            d.linechange();
            d.packAdd("hutiZure", hutiZure);
            d.packAdd("hutiColor", hutiColor.ToSave());
            return d;
        }
        public void ToLoad(DataSaver d)
        {
            this.size = d.unpackDataF("size", this.size);
            this.w = d.unpackDataF("w", this.w);
            this.h = d.unpackDataF("h", this.h);
            this.fontName = d.unpackDataS("fontName", this.fontName);
            this.isItaric = (int)d.unpackDataF("isItaric", this.isItaric);
            this.isBold = (int)d.unpackDataF("isBold", this.isBold);
            this.ali = d.unpackDataE("alignment", this.ali);
            this.aliV = d.unpackDataE("alignmentV", this.aliV);
            this.hutiZure = d.unpackDataF("hutiZure");
            this.hutiColor.ToLoad(d.unpackDataD("hutiColor"));
        }
        /// <summary>
        /// ちゃんとしたフォントに変える
        /// </summary>
        /// <returns></returns>
        public IDWriteTextFormat ToFont()
        {
            var fa = Vortice.DirectWrite.DWrite.DWriteCreateFactory<IDWriteFactory>();

            Vortice.DirectWrite.FontStyle style = Vortice.DirectWrite.FontStyle.Normal;
            if (isItaric==1)
            {
                style = Vortice.DirectWrite.FontStyle.Italic;
            }
            var Weight = FontWeight.Light;
            if (isBold==1)
            {
                Weight = FontWeight.UltraBold;
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

            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return ReferenceEquals(a, null) == ReferenceEquals(b, null);
            }

            return a.size == b.size && a.w == b.w && a.h == b.h &&
                a.fontName == b.fontName && a.isItaric == b.isItaric
                && a.isBold == b.isBold && a.hutiZure == b.hutiZure && a.hutiColor == b.hutiColor;
        }
        static public bool operator !=(FontC a, FontC b)
        {
            if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            {
                return ReferenceEquals(a, null) != ReferenceEquals(b, null);
            }


            return a.size != b.size && a.w != b.w || a.h != b.h ||
                a.fontName != b.fontName || a.isItaric != b.isItaric
                || a.isBold != b.isBold || a.hutiZure != b.hutiZure || a.hutiColor != b.hutiColor;
        }
        static public int ByteCount(string s)
        {
            var encode = System.Text.Encoding.UTF8;
            return encode.GetByteCount(s);
        }
    }

    /// <summary>
    /// bmp上の領域を確保して文字を書くためのクラス
    /// </summary>
    class TextRenderer
    {
        internal C3BitmapRenderSet render;

        /// <summary>
        /// Rectangleだが、回転は無視される。
        /// </summary>
        public Shapes.Rectangle rendZone;

        bool _MustReset = false;
        /// <summary>
        /// 領域がかぶってしまったので別の場所に確保すべきフラグ
        /// </summary>
        public bool MustReset { get { return _MustReset; } }
        bool NoChange = false;
        bool NoChange2 = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="render"></param>
        /// <param name="drawto">回転は無しね</param>
        public TextRenderer(Display parent, C3BitmapRenderSet render, Shapes.Rectangle drawto)
        {
            this.parent = parent;
            this.render = render;
            rendZone = drawto;
     
        }
        ~TextRenderer()
        {
           Release();//ここ、マジ意味ない
        }
        /// <summary>
        /// こいつを管理している親
        /// </summary>
        readonly Display parent;

        /// <summary>
        /// 確保した場所がほかのTEXTRENDERの描画で変化した場合に呼びだす。
        /// </summary>
        public void Changed()
        {
            _MustReset = true;
            NoChange = false;
            NoChange2 = false;
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
                NoChange2 = false;
                return true;
            }

            if (PastFont == null || PastFont != F)
            {
                NoChange = false;
                NoChange2 = false;
                return true;
            }
            var tColor = new ColorC(color);
            tColor.opa = 1;//オパシティは条件に含まない
            if (PastColor == null || PastColor != tColor)
            {
                NoChange = false;
                NoChange2 = false;
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
            PastFont = new FontC();
            F.copy(PastFont);
            PastColor = new ColorC(color);
            PastColor.opa = 1;//オパシティは条件に含まない

        }


        /***************************************************************************************/


        float _bottom = 0;//!<文字が書かれた領域の最下端

        float _right = 0;//!<文字が書かれた領域の最右端
        /// <summary>
        /// 領域内でも字が書かれた最下端
        /// </summary>
        public float bottom
        {
            get
            {
                if (rendZone.h == 0) return 0.5f;
                return Mathf.min(_bottom / rendZone.h, 1);
            }
        }

        /// <summary>
        /// 領域内でも字が書かれた最右端
        /// </summary>
        public float right
        {
            get
            {
                if (rendZone.w == 0) return 0.5f;
                return Mathf.min(_right / rendZone.w, 1);
            }
        }

        private string PastText = null;
        private FontC PastFont = null;
        private ColorC PastColor = null;

        internal void SetRayout(string Text, FontC F)
        {
            IDWriteTextLayout Layout = render.WriteFactory.CreateTextLayout(Text, F.ToFont(), rendZone.w, rendZone.h);

            int maxline = 2;
            if (F.size > 0)
            {
                maxline = (int)(rendZone.h / F.size) * 2;
            }
            LineMetrics[] Lmets = new LineMetrics[maxline];
            ClusterMetrics[] Cmets = new ClusterMetrics[maxline * 100];
            int LCount;
            int CCount;
            var Lres = Layout.GetLineMetrics(Lmets, out LCount);
            var Cres = Layout.GetClusterMetrics(Cmets, out CCount);

            if (Lres.Success)
            {
                //文字列の高さをさぐる
                _bottom = 0;
                for (int t = 0; t < LCount; ++t)
                {
                    _bottom += Lmets[t].Height;
                }
            }
            if (Cres.Success)
            {
                //文字列の幅をさぐる
                _right = 0;
                for (int t = 0; t < CCount; ++t)
                {
                    _right += Cmets[t].Width;
                }
            }
            Layout.Dispose();
        }

        public void Draw(string Text, FontC F, ColorC color,DisplaySemaphores semaphores)
        {
            if (CheckChange(Text, F, color))
            {
                SetPast(Text, F, color);
            }

            if (NoChange == false)
            {
                //Debug.WriteLine(Text + " Drawed!");
                //  Text+= "\n->"+rendZone.gettxy(0, 0) + " :TO: " + rendZone.gettxy(rendZone.w, rendZone.h);
                var Clip = (RawRectF)rendZone;

                Clip.Left -= 1;
                Clip.Right += 1;
                Clip.Top -= 1;
                Clip.Bottom += 1;


                semaphores.TextRender.Wait();
                
                render.BitmapRender.PushAxisAlignedClip(Clip, AntialiasMode.PerPrimitive);
                render.BitmapRender.Transform = Matrix3x2.CreateTranslation(0, 0);

                float R = 1, G = 0.98f, B = 0.97f;
                {

                    //render.Clear(new ColorC(FileMan.whrandhani(R), FileMan.whrandhani(G), FileMan.whrandhani(B)
                    //    , 0.5f));
                    render.BitmapRender.Clear(new ColorC(R,G,B,0));
                }
                if (F.hutiZure > 0)
                {
                    
                    var c = new ColorC(F.hutiColor);
                    c.opa = F.hutiColor.opa;
                    var slb = render.BitmapRender.CreateSolidColorBrush(c);
                    var lis = new List<Shapes.Rectangle>();
                    lis.Add((Shapes.Rectangle)rendZone.clone());
                    lis.Add((Shapes.Rectangle)rendZone.clone());
                    lis.Add((Shapes.Rectangle)rendZone.clone());
                    lis.Add((Shapes.Rectangle)rendZone.clone());
                    float zure = F.hutiZure * F.size;
                    lis[0].x += zure;
                    lis[1].x -= zure;
                    lis[2].y += zure;
                    lis[3].y -= zure;
                    for (int i = 0; i < lis.Count; i++)
                    {
                   
                        render.BitmapRender.DrawText(Text, F.ToFont(), lis[i], slb);
                    }
                }
                SetRayout(Text,F);
                {
                    var slb = render.BitmapRender.CreateSolidColorBrush(PastColor);
                    render.BitmapRender.DrawText(Text, F.ToFont(), rendZone, slb);
                }
                render.BitmapRender.PopAxisAlignedClip();
                semaphores.TextRender.Release();

                /*
                {
                    var slb = render.CreateSolidColorBrush(new ColorC(0, 0, 0, 1));
                    render.DrawRectangle((RawRectF)rendZone, slb);
                }*/

                parent.Drawed(this);
                NoChange = true;
            }

        }
        /// <summary>
        /// 画面に描画する前に呼び出すよ
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="F"></param>
        /// <param name="color"></param>
        public void OnDraw(string Text, FontC F, ColorC color)
        {
            if (NoChange2 == false)
            {
                //一番下を決める。上にそろえる場合は必要ないので計算しない。
                if (F.aliV != FontC.alignment.left)
                {

                    /*
                    var list = Display.GetPixels(render, rendZone);

                    bool breaked = false;
                    for (int y = list.Count - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < list[y].Count; x++)
                        {
                            if (list[y][x].opa > 0)
                            {
                                _bottom = y;
                                breaked = true;
                                break;
                            }
                        }


                        if (breaked) break;
                    }
                    _bottom += 1;*/
                }
                else
                {

                }
                NoChange2 = true;
            }
        }
    }


    public class Text : Drawable
    {
        public string text = "";
        public FontC font = new FontC();
        private TextRenderer _Trender = null;

        internal TextRenderer Trender { get { return _Trender; }

            set {
                _Trender?.Release();
                _Trender = value;
            }
        }
        public Text(float z, ColorC c, string text, FontC font, float time = -1, string name = "") : base(z, c, time, name)
        {
            this.text = text;
            this.font = new FontC();
            font.copy(this.font);
        }
        public Text() { }
        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (Text)c;
            cc.text = this.text;
            cc.font = new FontC();
            this.font.copy(cc.font);
        }
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.linechange();
            d.packAdd("text", text);
            var dd = font.ToSave();
            d.linechange();
            dd.indent();
            d.packAdd("font", dd);
            return d;
        }
        public override void update(float cl)
        {
            base.update(cl);
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.text = d.unpackDataS("text", "なんもない");
            this.font = new FontC();
            var dd = d.unpackDataD("font");
            this.font.ToLoad(dd);

        }
        /// <summary>
        /// 文字列の一番下の位置
        /// </summary>
        public float Bottom
        {
            get
            {
                if (Trender == null)
                {
                    return 0;
                }
                return Trender.bottom;
            }
        }

        /// <summary>
        /// 文字列の一番右の位置
        /// </summary>
        public float Right
        {
            get
            {
                if (Trender == null)
                {
                    return 0;
                }
                return Trender.right;
            }
        }

        /// <summary>
        /// テキスト描画のためのTrenderを作成する。
        /// </summary>
        /// <param name="dis">間違えないでね！</param>
        internal void MakeTrender(Display dis) 
        {
            if (Trender == null)
            {
                //ここでテンポラリなやつをもらう。
                Trender = dis.makeTextRenderer(font.w, font.h);
            }
            else if (Trender.MustReset || (Trender.rendZone.w != font.w && Trender.rendZone.h != font.h))
            {
                Trender.Release();
                Trender = dis.makeTextRenderer(font.w, font.h);
            }
            Trender.SetRayout(text, font);
        }

        /// <summary>
        /// 描画のレイアウトを、Trenderを取得しているならセットする
        /// </summary>
        internal void SetRayout() 
        {

            Trender?.SetRayout(text, font);
        }

        public override void PreDraw(Camera cam, DisplaySemaphores semaphores)
        {
            MakeTrender(cam.d);
            base.PreDraw(cam, semaphores);
            Trender.Draw(text, font, col,semaphores);
        }
        public override void draw(Camera cam)
        {
            Trender.OnDraw(text, font, col);
            var render = cam.render.Render;


            float yZure = 0;
            switch (font.aliV)//無理やり上下にアライメントする
            {
                case FontC.alignment.left://上揃え。何もしない

                    break;
                case FontC.alignment.center:
                    yZure = e.h / 2 - (Trender.bottom * e.h) / 2;
                    // Debug.WriteLine(e.h + " :: " + Trender.bottom + " == " + yZure);
                    break;
                case FontC.alignment.right:
                    yZure = e.h - (Trender.bottom * e.h);
                    break;
                case FontC.alignment.justify:
                    break;
                default:
                    break;
            }
            render.Transform = rectTrans(cam);
            var rect = rectRectF(cam);

            var zure = new FXY(0, yZure);
            zure = camsoutai(cam, zure + cam.watchRect.gettxy(0, 0));
            zure.degree += e.degree;
            rect.Left += zure.x;
            rect.Right += zure.x;
            rect.Top += zure.y;
            rect.Bottom += zure.y;
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

                var source = new Rectangle();
                Trender.rendZone.copy(source);
                /*source.x += 1;
                source.w -= 2;
                source.y += 1;
                source.h -= 2;
                */
                render.DrawBitmap(Trender.render.BitmapRender.Bitmap
                  , rect
                  , this.col.opa, mode
                    , source);
            }
            if (onWorld==false)
            {

                _Trender?.Release();
                _Trender = null;
            }
        }
        public override void removetoworld(float cl = 0)
        {
            base.removetoworld(cl);

            _Trender?.Release();
            _Trender = null;
        }
        ~Text()
        {
            _Trender?.Release();
            _Trender = null;
        }
    }

    #endregion
}