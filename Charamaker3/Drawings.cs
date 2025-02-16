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
        //描画の順番
        public float z;
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
        virtual public bool goDraw(Camera cam)
        {
            if (col.opa > 0 && onCamera(cam))
            {
                draw(cam);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 事前描画をする
        /// </summary>
        /// <param name="cam"></param>
        /// <returns>描画したか(カメラ範囲外とかだと描画しない)</returns>
        virtual public bool goPreDraw(Camera cam)
        {
            if (onCamera(cam))
            {
                PreDraw(cam);
                return true;
            }
            return false;
        }

        protected abstract void draw(Camera cam);
        protected virtual void PreDraw(Camera cam) { }

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
        protected override void draw(Camera cam)
        {
            var render = cam.render;

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
                return FileMan.nothing;
            } }

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


        protected override void draw(Camera cam)
        {
            var render = cam.render;

            ID2D1Bitmap bitmap;
            bitmap = FileMan.ldtex(nowtex);
            if (bitmap == null)
            {
                bitmap = FileMan.ldtex(FileMan.nothing);
            }
            render.Transform = rectTrans(cam);
            var rect = rectRectF(cam);
            //色のエフェクトを作る

            var blended =cam.d.Blend(bitmap, this.col);
            render.DrawBitmap(blended
                   , rect
                   , this.col.opa, BitmapInterpolationMode.Linear
                   , new RawRectF(0, 0, bitmap.Size.Width-0.5f, bitmap.Size.Height - 0.5f));//-0.5しないと1マスが2マスになって絶望したりする。


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
            justify
        }

        /// <summary>
        /// フォントの名前
        /// </summary>
        public string fontName = "MS UI Gothic";

        /// <summary>
        /// 斜体
        /// </summary>
        public bool isItaric = false;

        /// <summary>
        /// 太字
        /// </summary>
        public bool isBold = false;
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
            , bool isItaric = false, bool isBold = false, alignment alignment = alignment.left
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
            this.isItaric = d.unpackDataB("isItaric", this.isItaric);
            this.isBold = d.unpackDataB("isBold", this.isBold);
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
            if (isItaric)
            {
                style = Vortice.DirectWrite.FontStyle.Italic;
            }
            var Weight = FontWeight.Light;
            if (isBold)
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
        internal ID2D1BitmapRenderTarget render;

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
        public TextRenderer(Display parent, ID2D1BitmapRenderTarget render, Shapes.Rectangle drawto)
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

            if (PastColor == null || PastColor != color)
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

        }


        /***************************************************************************************/


        float _bottom = 0;//!<文字が書かれた領域の最下端
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

        private string PastText = null;
        private FontC PastFont = null;
        private ColorC PastColor = null;
        public void Draw(string Text, FontC F, ColorC color)
        {
            if (CheckChange(Text, F, color))
            {
                SetPast(Text, F, color);
            }

            if (NoChange == false)
            {
                //Debug.WriteLine(Text + " Drawed!");
                //  Text+= "\n->"+rendZone.gettxy(0, 0) + " :TO: " + rendZone.gettxy(rendZone.w, rendZone.h);
                var raw = (RawRectF)rendZone;
                render.PushAxisAlignedClip(rendZone, AntialiasMode.PerPrimitive);
                render.Transform = Matrix3x2.CreateTranslation(0, 0);

                float R = 1, G = 0.98f, B = 0.97f;
                {

                    //render.Clear(new ColorC(FileMan.whrandhani(R), FileMan.whrandhani(G), FileMan.whrandhani(B)
                    //    , 0.5f));
                    render.Clear(new ColorC(R,G,B,0));
                }
                if (F.hutiZure > 0)
                {

                    var c = new ColorC(F.hutiColor);
                    c.opa = F.hutiColor.opa;
                    var slb = render.CreateSolidColorBrush(c);
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
                        render.DrawText(Text, F.ToFont(), lis[i], slb);
                    }


                }
                {
                    var slb = render.CreateSolidColorBrush(color);
                    render.DrawText(Text, F.ToFont(), rendZone, slb);
                }
                render.PopAxisAlignedClip();


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

                    
                    var list = Display.GetPixels(render.Bitmap, render, rendZone);

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
                    _bottom += 1;
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
            this.font = font;
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
        public override bool goPreDraw(Camera cam)
        {
            if (Trender == null)
            {
                //ここでテンポラリなやつをもらう。
                Trender = cam.d.makeTextRenderer(font.w, font.h);
            }
            else if (Trender.MustReset||(Trender.rendZone.w != font.w && Trender.rendZone.h != font.h))
            {
                Trender.Release();
                Trender = cam.d.makeTextRenderer(font.w, font.h);
            }
            return base.goPreDraw(cam);
        }
        protected override void PreDraw(Camera cam)
        {
            base.PreDraw(cam);
            Trender.Draw(text, font, col);
        }
        protected override void draw(Camera cam)
        {
            Trender.OnDraw(text, font, col);
            var render = cam.render;


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
                render.DrawBitmap(Trender.render.Bitmap
                  , rect
                  , this.col.opa, BitmapInterpolationMode.Linear
                    , Trender.rendZone);
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