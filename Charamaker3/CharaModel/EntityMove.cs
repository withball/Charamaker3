using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charamaker3.Utils;

namespace Charamaker3.CharaModel
{
    public partial class EntityMove : Component
    {


        /// <summary>
        /// モーションを作る
        /// </summary>
        /// <returns>__MOVE__</returns>
        static public Motion MakeMotion()
        {
            return new Motion();
        }

        /// <summary>
        /// 何もしないモーション。時間を作るときだけ使う
        /// </summary>
        /// <returns>__MOVE__</returns>
        static public Component Wait(float time)
        {
            return new Component(time);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time"></param>
        /// <param name="eventName"></param>
        /// <returns>__MOVE__</returns>
        static public FreeEventer FreeEventer(float time, string eventName)
        {
            return new FreeEventer(time, eventName);
        }

        //res.addmove(,False);
        /// <summary>
        /// XYDに動かすムーブ
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name">=""</param>
        /// <param name="dx">=0</param>
        /// <param name="dy">=0</param>
        /// <param name="ddegree">=0</param>
        /// <param name="withbigs">dx,dyを(w+h)/2に依存するか=false</param>
        /// <returns>__MOVE__</returns>
        static public EntityMove XYD(float time, string name = "", float dx = 0, float dy = 0, float ddegree = 0, bool withbigs = false)
        {
            EntityMove res;
            if (withbigs)
            {
                res = new EntityMove(time, dx, dy, float.NaN, float.NaN, float.NaN, float.NaN, ddegree, float.NaN, float.NaN, name);
                res.SO = scaleOption.scale;
            }
            else
            {
                res = new EntityMove(time, dx, dy, 0, 0, 0, 0, ddegree, 0, 0, name);
                res.SO = scaleOption.F;
            }
            return res;
        }
        /// <summary>
        /// JointXYに動かすムーブ
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name">=""</param>
        /// <param name="dpx">=0</param>
        /// <param name="dpy">=0</param>
        /// <returns>__MOVE__</returns>
        static public EntityMove JXY(float time, string name = "", float dpx = 0, float dpy = 0)
        {
            EntityMove res;

            {
                res = new EntityMove(time, 0, 0, 0, 0, 0, 0, 0, dpx, dpy, name);
                res.SO = scaleOption.F;
            }
            return res;
        }
        /// <summary>
        /// W,H,TX,TY,DX,DYを動かすムーブ
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name">=""</param>
        /// <param name="dw">=0</param>
        /// <param name="dh">=0</param>
        /// <param name="dtx">親のwに対する倍率=0</param>
        /// <param name="dty">親のhに対する倍率=0</param>
        /// <returns>__MOVE__</returns>
        static public EntityMove WHTXY(float time, string name = "", float dw = 0, float dh = 0, float dtx = 0, float dty = 0)
        {
            return new EntityMove(time, 0, 0, dw, dh, dtx, dty, 0, 0, 0, name);
        }
        /// <summary>
        /// 大きさを変更するムーブ
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name">=""</param>
        /// <param name="scw">=Nan</param>
        /// <param name="scy">=Nan</param>
        /// <param name="sctx">wに対するtx変化=NAn</param>
        /// <param name="scty">hに対するty変化=Nan</param>
        /// <param name="basescale">ベースに対する変化にするか=true</param>
        /// <param name="onlyroot">根のスケールだけ変更=false</param>
        /// <returns>__MOVE__</returns>
        static public EntityMove ScaleChange(float time, string name = "", float scw = float.NaN, float scy = float.NaN
            , float sctx = float.NaN, float scty = float.NaN, bool basescale = true, bool onlyroot = false)
        {
            var res = new EntityMove(time, 0, 0, scw, scy, sctx, scty, 0, float.NaN, float.NaN, name);
            if (basescale)
            {
                res.SO = scaleOption.basescale;
            }
            else
            {
                res.SO = scaleOption.scale;
            }
            if (onlyroot)
            {
                res.GO = goOption.onlyRoot;
            }
            else
            {
                res.GO = goOption.def;
            }
            return res;
        }
        /// <summary>
        /// 角度を狙った方向にする
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name">=""</param>
        /// <param name="degree">=0</param>
        /// <param name="rp">=rotatePath.shorts ,plus,minus</param>
        /// <param name="joint">ジョイント依存にする=true</param>
        /// <returns>__MOVE__</returns>
        static public EntityMove RotateTo(float time, string name = "", float degree = 0, rotatePath rp = rotatePath.shorts
            , bool joint = true)
        {
            var res = new EntityMove(time, 0, 0, 0, 0, 0, 0, degree, 0, 0, name);
            if (joint)
            {
                res.RO = rotateOption.joint;
            }
            else
            {
                res.RO = rotateOption.world;
            }
            res.RP = rp;
            return res;
        }
        /// <summary>
        /// 角度を狙った方向にする(速度上限付き)
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name">=""</param>
        /// <param name="degree">=0</param>
        /// <param name="speedlimit">=0</param>
        /// <param name="shortestPath">最短経路で移動するか=true</param>
        /// <param name="joint">ジョイント依存にする=true</param>
        /// <returns>__MOVE__</returns>
        static public EntityMove RotateToLim(float time, string name = "", float degree = 0, float speedlimit = 0
            , bool shortestPath = true
            , bool joint = true)
        {
            var res = new EntityMove(time, 0, 0, 0, 0, 0, 0, degree, 0, 0, name);
            if (joint)
            {
                res.RO = rotateOption.joint;
            }
            else
            {
                res.RO = rotateOption.world;
            }
            if (shortestPath)
            {
                res.RP = rotatePath.shorts;
            }
            else
            {
                if (speedlimit > 0)
                {
                    res.RP = rotatePath.plus;
                }
                else
                {
                    res.RP = rotatePath.minus;
                }
            }
            res.degreeSpeedLimit = Mathf.abs(speedlimit);
            return res;
        }
        /// <summary>
        /// ベースの角度基準で回転する
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name">=""</param>
        /// <param name="degree">=0</param>
        /// <param name="rp">rotatePath=shorts,plus,minus</param>
        /// <param name="goall">リーフもベースに向けて回転するか=false</param>
        /// <returns>__MOVE__</returns>
        static public EntityMove RotateToBace(float time, string name = "", float degree = 0, rotatePath rp = rotatePath.shorts
            , bool goall = false)
        {
            var res = new EntityMove(time, 0, 0, 0, 0, 0, 0, degree, 0, 0, name);

            res.RO = rotateOption.baseCharacter;

            res.RP = rp;
            if (goall)
            {
                res.GO = goOption.goAll;
            }
            else
            {
                res.GO = goOption.def;
            }
            return res;
        }

        /// <summary>
        /// ベースの角度基準で回転する
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name">=""</param>
        /// <param name="degree">=0</param>
        /// <param name="speedlimit">=0</param>
        /// <param name="shortestPath">短い経路で移動するか=true</param>
        /// <param name="goall">リーフにも適用するか=true</param>
        /// <returns>__MOVE__</returns>
        static public EntityMove RotateToLimBase(float time, string name = "", float degree = 0, float speedlimit = 0
            , bool shortestPath = true, bool goall = true)
        {
            var res = new EntityMove(time, 0, 0, 0, 0, 0, 0, degree, 0, 0, name);
            res.RO = rotateOption.baseCharacter;

            res.degreeSpeedLimit = Mathf.abs(speedlimit);
            if (goall)
            {
                res.GO = goOption.goAll;
            }
            else
            {
                res.GO = goOption.def;
            }

            if (shortestPath)
            {
                res.RP = rotatePath.shorts;
            }
            else
            {
                if (speedlimit > 0)
                {
                    res.RP = rotatePath.plus;
                }
                else
                {
                    res.RP = rotatePath.minus;
                }
            }
            return res;
        }
        /// <summary>
        /// ジョイントの位置を変更する
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name">=""</param>
        /// <param name="dx">0~1のwに対する割合=Nan</param>
        /// <param name="dy">0~1のhに対する割合=Nan</param>
        /// <param name="baseScale">ベースに対する割合にするか=true</param>
        /// <returns>__MOVE__</returns>
        static public EntityMove JointMove(float time, string name = "", float dx = float.NaN, float dy = float.NaN, bool baseScale = true)
        {
            var res = new EntityMove(time, 0, 0, float.NaN, float.NaN, float.NaN, float.NaN, 0, dx, dy, name);
            if (baseScale)
            {
                res.SO = scaleOption.basescale;
            }
            else
            {
                res.SO = scaleOption.scale;
            }
            res.GO = goOption.onlyRoot;
            return res;
        }
        /// <summary>
        /// Entity要素(w,h,tx,ty,px,py)をリセットするムーブ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time">=0</param>
        /// <param name="degree">角度もリセットするか=true</param>
        /// <returns>__MOVE__</returns>
        static public EntityMove ResetMove(string name = "", float time = 0, bool degree = true)
        {
            float deg;
            if (degree)
            {
                deg = 0;
            }
            else
            {
                deg = float.NaN;
            }
            var res = new EntityMove(time, 0, 0, 1, 1, 0, 0, deg, 0, 0, name);
            res.SO = scaleOption.basescale;
            res.RO = rotateOption.baseCharacter;
            res.GO = goOption.goAll;
            return res;
        }

        /// <summary>
        /// 反転をするムーブ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="mo">=Reverse,No,Mirror</param>
        /// <param name="goreef">リーフの方向も変えるか=true</param>
        /// <param name="time">=0</param>
        /// <param name="isMoveDegree">=角度もいい感じに回転させるか=true</param>
        /// <returns>__MOVE__</returns>
        static public EntityMirror Mirror(string name, MirrorOption mo = MirrorOption.Reverse, bool goreef = true, float time = 0, bool isMoveDegree = true)
        {
            return new EntityMirror(name, time, mo, goreef, isMoveDegree);
        }


        /// <summary>
        /// Zじゃない軸で回転するムーブ
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        /// <param name="startX">=0 現在の角度</param>
        /// <param name="endX">=0 終わりの角度</param>
        /// <param name="startY">=0</param>
        /// <param name="endy">=0</param>
        /// <param name="scalex">=1</param>
        /// <param name="scaley">=1</param>
        /// <param name="onlyroot">=false</param>
        /// <returns>__MOVE__</returns>
        static public ZRotateMove ZRotate(string name, float time, float startX = 0, float endX = 0, float startY = 0, float endy = 0
            , float scalex = 1, float scaley = 1, bool onlyroot = false)
        {
            var res = new ZRotateMove(time, startX, endX, startY, endy, scalex, scaley, name);
            if (onlyroot)
            {
                res.GO = goOption.onlyRoot;
            }
            else
            {
                res.GO = goOption.goAll;
            }
            return res;
        }
        /// <summary>
        /// SEを鳴らすムーブ。SoundComponentはMotionと相性が悪いのでAfterでくるんで渡す。
        /// </summary>
        /// <param name="file">ファイル名</param>
        /// <param name="volume">=1</param>
        /// <returns>__MOVE__</returns>
        static public Component PlaySound(string file, float volume = 1)
        {
            var sc = SoundComponent.MakeSE(FileMan.SE, file, volume, true);
            var c = new Component(0);
            c.afters.Add(sc);
            return c;
        }
    }
    public partial class DrawableMove : Component
    {
        /// <summary>
        /// z座標を変えるムーブ
        /// </summary>
        /// <param name="name">=""</param>
        /// <param name="dz">=0</param>
        /// <param name="time">=0</param>
        /// <param name="onlyroot">nameのパーツだけを変える=false</param>
        /// <returns>__MOVE__</returns>
        static public DrawableMove ZChange(string name = "", float dz = 0, float time = 0, bool onlyroot = false)
        {
            var res = new DrawableMove(time, dz, 0, 0, 0, 0, "_", name);
            if (onlyroot)
            {
                res.GO = goOption.onlyRoot;
            }
            else
            {
                res.GO = goOption.def;
            }
            return res;
        }
        /// <summary>
        /// 基準をもとにzを変える
        /// </summary>
        /// <param name="name">変える対象=""</param>
        /// <param name="Basename">元になるパーツ=""</param>
        /// <param name="pz">元パーツとのz差(割合)=0</param>
        /// <param name="time">=0</param>
        /// <param name="onlyroot">根本のみ変更=false</param>
        /// <returns>__MOVE__</returns>
        static public DrawableMove BaseZChange(string name = "", string Basename = "", float pz = 0
            , float time = 0, bool onlyroot = false)
        {
            var res = new DrawableMove(time, pz, float.NaN, float.NaN, float.NaN, float.NaN, "_", name);
            res.CO = changeOption.fromBase;
            res.zname = Basename;
            if (onlyroot)
            {
                res.GO = goOption.onlyRoot;
            }
            else
            {
                res.GO = goOption.def;
            }
            return res;
        }

        /// <summary>
        /// 基準をもとにzの倍率などをベースから変える
        /// </summary>
        /// <param name="name">変える対象=""</param>
        /// <param name="rZDelta">位相=Nan</param>
        /// <param name="rZRatio">倍率=Nan</param>
        /// <param name="time">=0</param>
        /// <param name="onlyroot">根本のみ変更=false</param>
        /// <returns>__MOVE__</returns>
        static public ZDeltaMove BaseZDeltaChange(string name = "", float rZDelta = float.NaN, float rZRatio = float.NaN
            , float time = 0, bool onlyroot = false)
        {
            var res = new ZDeltaMove(time, rZDelta, rZRatio,  name);
            res.CO = changeOption.fromBase;
            if (onlyroot)
            {
                res.GO = goOption.onlyRoot;
            }
            else
            {
                res.GO = goOption.def;//TODO:GoAllの方がいい？？？
            }
            return res;
        }
        /// <summary>
        /// 基準をもとにzの倍率などをベースから変える
        /// </summary>
        /// <param name="name">変える対象=""</param>
        /// <param name="rZDelta">位相=Nan</param>
        /// <param name="rZRatio">倍率=Nan</param>
        /// <param name="time">=0</param>
        /// <param name="onlyroot">根本のみ変更=false</param>
        /// <returns>__MOVE__</returns>
        static public ZDeltaMove ZDeltaChange(string name = "", float rZDelta = 0, float rZRatio = 0
            , float time = 0, bool onlyroot = false)
        {
            var res = new ZDeltaMove(time, rZDelta, rZRatio, name);
            res.CO = changeOption.difference;
            if (onlyroot)
            {
                res.GO = goOption.onlyRoot;
            }
            else
            {
                res.GO = goOption.def;//TODO:GoAllの方がいい？？？
            }
            return res;
        }

        /// <summary>
        /// 色を絶対的に増減させる
        /// </summary>
        /// <param name="time">=0</param>
        /// <param name="name">=""</param>
        /// <param name="opa">=0</param>
        /// <param name="r">=0</param>
        /// <param name="g">=0</param>
        /// <param name="b">=0</param>
        /// <param name="onlyroot">根本のみ変更=0</param>
        /// <returns>__MOVE__</returns>
        static public DrawableMove ChangeColor(float time = 0, string name = "", float opa = 0, float r = 0
            , float g = 0, float b = 0, bool onlyroot = false)
        {
            var res = new DrawableMove(time, 0, r, g, b, opa, "_", name);
            if (onlyroot)
            {
                res.GO = goOption.onlyRoot;
            }
            else
            {
                res.GO = goOption.def;
            }

            return res;
        }
        /// <summary>
        /// 色をベースから相対的に変える
        /// </summary>
        /// <param name="time">=0</param>
        /// <param name="name">=""</param>
        /// <param name="opa">=Nan</param>
        /// <param name="r">=nan</param>
        /// <param name="g">=nan</param>
        /// <param name="b">=nan</param>
        /// <param name="go">=goOption.def,onlyRoot,goAll</param>
        /// <returns>__MOVE__</returns>

        static public DrawableMove BaseColorChange(float time = 0, string name = "", float opa = float.NaN
            , float r = float.NaN, float g = float.NaN, float b = float.NaN, goOption go = goOption.def)
        {
            var res = new DrawableMove(time, float.NaN, r, g, b, opa, "_", name);
            res.CO = changeOption.fromBase;
            res.GO = go;
            return res;
        }
        /// <summary>
        /// テクスチャを変える
        /// </summary>
        /// <param name="name">=""</param>
        /// <param name="texture">=\\  _で変更なし、\\でベースになる</param>
        /// <param name="time">=0</param>
        /// /// <param name="gotree">root以下もテクスチャを変えるか=false</param>
        /// <returns>__MOVE__</returns>
        static public DrawableMove ChangeTexture(string name = "", string texture = "\\", float time = 0
            , bool gotree = false)
        {
            var res = new DrawableMove(time, 0, 0, 0, 0, 0, texture, name);
            if (gotree)
            {
                res.GO = goOption.goAll;
            }
            else
            {
                res.GO = goOption.def;
            }
            return res;
        }
        /// <summary>
        /// Drawable要素をリセットする
        /// </summary>
        /// <param name="z">zもリセットする=true</param>
        /// <param name="color">colorもリセットする=ture</param>
        /// <param name="texture">textureもリセットする=true</param>
        /// <param name="name">=""</param>
        /// <param name="time">=0</param>
        /// <param name="onlyroot">以下には適用しない=false</param>
        /// <returns>__MOVE__</returns>
        static public DrawableMove ResetMove(bool z=true,bool color=true,bool texture=true,string name = "", float time = 0, bool onlyroot = false)
        {

            var res = new DrawableMove(time, 0, 1, 1, 1, 1, "\\", name);
            if (z==false) 
            {
                res.basespeeds[_Z] = float.NaN;
                
            }
            if (color == false) 
            {
                res.basespeeds[_R] = float.NaN;
                res.basespeeds[_G] = float.NaN;
                res.basespeeds[_B] = float.NaN;
                res.basespeeds[_OPA] = float.NaN;
            }
            if (texture == false) 
            {
                res.texture = "_";
            }
            if (onlyroot)
            {
                res.GO = goOption.onlyRoot;
            }
            else
            {
                res.GO = goOption.goAll;
            }
            res.CO = changeOption.fromBase;
            return res;
        }


        /// <summary>
        /// テキストを消去する
        /// </summary>
        /// <param name="time">=0</param>
        /// <param name="name">=""</param>
        /// <returns>__MOVE__</returns>
        static public TextMove ClearText(float time = 0,string name = "")
        {
            var font = new FontC();
            font.ali = FontC.alignment.None;
            font.aliV = FontC.alignment.None;
            font.hutiZure = float.NaN;
            font.hutiColor.r = float.NaN;
            font.hutiColor.g = float.NaN;
            font.hutiColor.b = float.NaN;
            font.hutiColor.opa = float.NaN;
            font.isBold = -1;
            font.isItaric = -1;
            font.fontName = "";
            font.size = float.NaN;
            font.w = float.NaN;
            font.h = float.NaN;

            var res = new TextMove(name, time, "", 0, font, name);

            return res;
        }

        /// <summary>
        /// テキストをセットする。ちょっとずつやりたいなら消してからやってね
        /// </summary>>
        /// <param name="time">=0</param>
        /// <param name="name">=""</param>
        /// <param name="Text">=""</param
        /// <returns>__MOVE__</returns>
        static public TextMove SetText(float time = 0,string name = "",string Text="")
        {
            var font = new FontC();
            font.ali = FontC.alignment.None;
            font.aliV = FontC.alignment.None;
            font.hutiZure = float.NaN;
            font.hutiColor.r = float.NaN;
            font.hutiColor.g = float.NaN;
            font.hutiColor.b = float.NaN;
            font.hutiColor.opa = float.NaN;
            font.isBold = -1;
            font.isItaric = -1;
            font.fontName = "";
            font.size = float.NaN;
            font.w = float.NaN;
            font.h = float.NaN;

            var res = new TextMove(name, time, Text, Text.Length, font, name);

            return res;
        }
    }
    public enum scaleOption {
        /// <summary>
        /// スケールしない
        /// </summary>
        F = -1
                                  , scale = 1
            , basescale = 2 };
    public enum rotateOption { F = -1, world = 0, joint = 1, baseCharacter = 2 };
    public enum rotatePath { shorts = 0, plus = 1, minus = -1 };
    public enum goOption
    {
        /// <summary>
        /// 親と同じ動きを子供にも適用
        /// </summary>
        def = 0,
        /// <summary>
        /// 親だけに適用
        /// </summary>
        onlyRoot = -1,
        /// <summary>
        /// 親子すべてに対して効果を計算し、適用。
        /// </summary>
        goAll = 1
    };
    public enum MirrorOption
    {
        No = 1, Mirror = -1, Reverse = 0
    }

    public enum changeOption { difference = 1, fromBase = 2 };
    /// <summary>
    /// 値を直接的に変更するクラス。もう一つはmirrorのクラス。
    /// WARNING: basew=0のときあやしい
    /// </summary>
    public partial class EntityMove : Component
    {
        List<float[]> speeds = new List<float[]>();
        List<Entity> tags = new List<Entity>();
        List<Entity> tagBases = new List<Entity>();

        List<Joint> Jtags = new List<Joint>();
        List<Joint> JtagBases = new List<Joint>();

        float[] basespeeds = new float[9];

        //旧世代setumageのごり押し実装。0>=以上
        float degreeSpeedLimit = -1;
        //Joint Parent = null;
        //Joint BaseParent = null;

        const int _X = 0;
        const int _Y = 1;
        const int _W = 2;
        const int _H = 3;
        const int _TX = 4;
        const int _TY = 5;
        const int _DEGREE = 6;
        const int _DX = 7;
        const int _DY = 8;
        bool instant { get { return time <= 0; } }



        /// <summary>
        /// scaleをどう変化させるか
        /// </summary>
        protected scaleOption SO = scaleOption.F;
        /// <summary>
        /// 回転の依存対象
        /// </summary>
        protected rotateOption RO = rotateOption.F;
        /// <summary>
        /// 回転の方向
        /// </summary>
        protected rotatePath RP = rotatePath.shorts;

        /// <summary>
        /// サイズ、角度、中心、dxyを無理やり全体に適用する。
        /// </summary>
        goOption GO = goOption.def;


        public EntityMove() { }
        public EntityMove(float time, float dx = 0, float dy = 0, float dw = 0, float dh = 0, float dtx = 0, float dty = 0
            , float ddegree = 0, float ddx = 0, float ddy = 0, string name = "") : base(time, name)
        {

            basespeeds[_X] = dx;
            basespeeds[_Y] = dy;
            basespeeds[_W] = dw;
            basespeeds[_H] = dh;
            basespeeds[_TX] = dtx;
            basespeeds[_TY] = dty;
            basespeeds[_DEGREE] = ddegree;
            basespeeds[_DX] = ddx;
            basespeeds[_DY] = ddy;

            if (time == 0)
            {
            }
            else if (time < 0)
            {
                Debug.WriteLine("このコンポーネントはtime<0じゃダメなので勝手に0にしました♡");
                this.time = 0;
            }
            else
            {

            }

        }
        public override void copy(Component c)
        {
            var cc = (EntityMove)c;
            base.copy(c);
            for (int i = 0; i < basespeeds.Length; i++)
            {
                cc.basespeeds[i] = this.basespeeds[i];
            }
            cc.SO = this.SO;
            cc.RO = this.RO;
            cc.RP = this.RP;
            cc.degreeSpeedLimit = this.degreeSpeedLimit;
            cc.GO = this.GO;

        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("GO", (int)GO);
            res.packAdd("_X", basespeeds[_X]);
            res.packAdd("_Y", basespeeds[_Y]);
            res.linechange();

            res.packAdd("SO", (int)SO);
            res.packAdd("_W", basespeeds[_W]);
            res.packAdd("_H", basespeeds[_H]);
            res.packAdd("_TX", basespeeds[_TX]);
            res.packAdd("_TY", basespeeds[_TY]);

            res.linechange();

            res.packAdd("RO", (int)RO);
            res.packAdd("RP", (int)RP);
            res.packAdd("degreeSpeedLimit", degreeSpeedLimit);
            res.packAdd("_DEGREE", basespeeds[_DEGREE]);

            res.linechange();
            res.packAdd("_DX", basespeeds[_DX]);
            res.packAdd("_DY", basespeeds[_DY]);
            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);

            GO = (goOption)d.unpackDataF("GO");
            basespeeds[_X] = d.unpackDataF("_X");
            basespeeds[_Y] = d.unpackDataF("_Y");

            SO = (scaleOption)d.unpackDataF("SO");
            basespeeds[_W] = d.unpackDataF("_W");
            basespeeds[_H] = d.unpackDataF("_H");
            basespeeds[_TX] = d.unpackDataF("_TX");
            basespeeds[_TY] = d.unpackDataF("_TY");

            RO = (rotateOption)d.unpackDataF("RO");
            RP = (rotatePath)d.unpackDataF("RP");
            degreeSpeedLimit = d.unpackDataF("degreeSpeedLimit");
            basespeeds[_DEGREE] = d.unpackDataF("_DEGREE");

            basespeeds[_DX] = d.unpackDataF("_DX");
            basespeeds[_DY] = d.unpackDataF("_DY");


        }
        public override string ToString()
        {
            var res = base.ToString();
            res += $"\n{SO}:{RO}:{RP}:{GO}";
            res += $" -> X:{basespeeds[_X]}:Y:{basespeeds[_Y]}:W:{basespeeds[_W]}:H:{basespeeds[_H]}:TX:{basespeeds[_TX]}:TY:{basespeeds[_TY]}:DEGREE:{basespeeds[_DEGREE]}:";
            return res;
        }
        protected virtual void addDefference(float ratio)
        {

            if (degreeSpeedLimit >= 0)
            //旧世代のシステムをゴミみたいな感じで実装 
            {
                if (tags.Count > 0)
                {
                    var baseDegree = basespeeds[_DEGREE];
                    if (e.mirror)
                    {
                        baseDegree = Mathf.st180(tagBases[0].degree - Mathf.st180(baseDegree - tagBases[0].degree));
                    }
                    float distance;
                    if (RO == rotateOption.baseCharacter)
                    {
                        distance = Mathf.st180(tagBases[0].degree + baseDegree - tags[0].degree);
                    }
                    else if (Jtags[0] != null)
                    {
                        distance = Mathf.st180(Jtags[0].parent.degree + baseDegree - tags[0].degree);
                    }
                    else if (tags.Count > 0)//キャラクターを持ってるエンテティが対象だったら自動でワールド角度に
                    {
                        distance = Mathf.st180(baseDegree - tags[0].degree);

                    }
                    else
                    {
                        distance = 0;
                    }
                    if (RP == rotatePath.plus)
                    {
                        if (e.mirror)
                        {
                            if (distance > 0)
                            {
                                distance = -180 - 180 + distance;
                            }
                        }
                        else
                        {
                            if (distance < 0)
                            {
                                distance = 180 + 180 + distance;
                            }
                        }
                    }
                    if (RP == rotatePath.minus)
                    {
                        if (e.mirror)
                        {
                            if (distance < 0)
                            {
                                distance = 180 + 180 + distance;
                            }
                        }
                        else
                        {
                            if (distance > 0)
                            {
                                distance = -180 - 180 + distance;
                            }
                        }
                    }





                    if (Mathf.abs(Mathf.st180(distance)) < 0.1f) //行き過ぎ回転しないように
                    {
                        // Debug.WriteLine(distance + " : " + degreeSpeedLimit + " to " + tags[0].name);

                        for (int t = 0; t < tags.Count; t++)
                        {
                            tags[t].degree += distance;
                        }
                    }
                    else if (degreeSpeedLimit * ratio <= Mathf.abs(distance))
                    {
                        //  Debug.WriteLine("bigger than limit" + distance);
                        for (int t = 0; t < tags.Count; t++)
                        {
                            tags[t].degree += Mathf.sameSign(degreeSpeedLimit * ratio, distance);
                        }
                    }
                    else
                    {
                        // Debug.WriteLine("less than limit" + distance);
                        for (int t = 0; t < tags.Count; t++)
                        {
                            tags[t].degree += distance;
                        }
                    }

                }
            }
            else
            {
                // Debug.WriteLine("default degreee");

                for (int t = 0; t < tags.Count; t++)
                {
                    if (degreeSpeedLimit < 0)
                    {
                        tags[t].degree += (speeds[t][_DEGREE] * ratio);
                    }
                }
            }
            for (int t = 0; t < tags.Count; t++)
            {
                tags[t].x += (float)(speeds[t][_X] * ratio);
                tags[t].y += (float)(speeds[t][_Y] * ratio);

                var pos = tags[t].gettxy();
                tags[t].w += (float)(speeds[t][_W] * ratio);
                tags[t].h += (float)(speeds[t][_H] * ratio);
                tags[t].tx += (float)(speeds[t][_TX] * ratio);
                tags[t].ty += (float)(speeds[t][_TY] * ratio);
                tags[t].settxy(pos);

                if (Jtags[t] != null)
                {
                    Jtags[t].px += speeds[t][_DX] * ratio;
                    Jtags[t].py += speeds[t][_DY] * ratio;
                }
            }

        }
        protected override void onadd(float cl)
        {
            tags.Clear();
            tagBases.Clear();
            speeds.Clear();

            //characterから得るtag
            var cs = e.getcompos<Character>();
            if (cs.Count == 0)
            {

                tags.Add(e);
                Jtags.Add(null);
                tagBases.Add(e);
                JtagBases.Add(null);
            }
            else
            {
                if (SO == scaleOption.basescale || RO != rotateOption.F)
                {
                    //こっちはキャラクターのベースね。後で実装
                    foreach (var a in cs)
                    {
                        foreach (var b in a.getTree(name))
                        {
                            tags.Add(b);
                        }
                        foreach (var b in a.BaseCharacter.getTree(name))
                        {
                            tagBases.Add(b);
                        }
                    }
                }
                else
                {
                    foreach (var a in cs)
                    {
                        foreach (var b in a.getTree(name))
                        {
                            tags.Add(b);
                            tagBases.Add(b);
                        }
                    }
                }
                //ジョイントも設定。ジョイントはエンテティと1対1対応していないので注意
                for (int i = 0; i < tags.Count; i++)
                {
                    Jtags.Add(cs[0].getParentJoint(tags[i].name));
                    JtagBases.Add(cs[0].BaseCharacter.getParentJoint(tagBases[i].name));
                }
                //一斉に指定して子供までついてきた場合、親のジョイントがかぶる場合がある。Rootは絶対にかぶらないけども、かぶったやつはnullにする。
                for (int i = 0; i < tags.Count; i++)
                {
                    if (Jtags[i] != null) for (int t = i + 1; t < tags.Count; t++)
                        {
                            if (Jtags[i] == Jtags[t])
                            {
                                Jtags[t] = null;
                            }
                        }
                }
            }
            for (int t = 0; t < tags.Count; t++)
            {
                speeds.Add(new float[9]);
                for (int i = 0; i < basespeeds.Length; i++)
                {
                    //先頭の根本。Characterの保持者。
                    if (t == 0 || GO == goOption.goAll)
                    {
                        if (SO != scaleOption.F)
                        {
                            var baseE = tagBases[t];
                            switch (i)
                            {
                                case _X:
                                    if (!float.IsNaN(basespeeds[i]))
                                    {
                                        speeds[t][i] = basespeeds[i] * tagBases[t].w;
                                    }
                                    else
                                    {
                                        speeds[t][i] = 0;
                                    }
                                    break;
                                case _Y:
                                    if (!float.IsNaN(basespeeds[i]))
                                    {
                                        speeds[t][i] = basespeeds[i] * tagBases[t].h;
                                    }
                                    else
                                    {
                                        speeds[t][i] = 0;
                                    }
                                    break;
                                case _W:
                                    if (!float.IsNaN(basespeeds[i]))
                                    {
                                        speeds[t][i] = basespeeds[i] * tagBases[t].w - tags[t].w;
                                    }
                                    else
                                    {
                                        speeds[t][i] = 0;
                                    }
                                    break;
                                case _H:
                                    if (!float.IsNaN(basespeeds[i]))
                                    {
                                        speeds[t][i] = basespeeds[i] * tagBases[t].h - tags[t].h;
                                    }
                                    else
                                    {
                                        speeds[t][i] = 0;
                                    }
                                    break;
                                case _TX:
                                    float ratio;
                                    if (float.IsNaN(basespeeds[_W]))
                                    {
                                        ratio = 1;
                                    }
                                    else if (tags[t].w != 0)
                                    {
                                        ratio = (basespeeds[_W] * tagBases[t].w / tags[t].w);
                                    }
                                    else
                                    {
                                        ratio = (basespeeds[_W]);
                                    }
                                    if (!float.IsNaN(basespeeds[i]))
                                    {
                                        if (tagBases[t].w != 0)
                                        {
                                            speeds[t][i] =
                                                +ratio * (tagBases[t].w * basespeeds[i] * tags[t].w / tagBases[t].w
                                                + tagBases[t].tx * tags[t].w / tagBases[t].w)
                                                - tags[t].tx;
                                        }
                                        else
                                        {
                                            speeds[t][i] = ratio * (tagBases[t].w * basespeeds[i] + tagBases[t].tx) - tags[t].tx;
                                        }
                                    }
                                    else
                                    {

                                        speeds[t][i] = (ratio * tags[t].tx) - tags[t].tx;

                                    }
                                    break;
                                case _TY:
                                    if (float.IsNaN(basespeeds[_H]))
                                    {
                                        ratio = 1;
                                    }
                                    else if (tags[t].h != 0)
                                    {
                                        ratio = (basespeeds[_H] * tagBases[t].h / tags[t].h);
                                    }
                                    else
                                    {
                                        ratio = (basespeeds[_H]);
                                    }
                                    if (!float.IsNaN(basespeeds[i]))
                                    {
                                        if (tagBases[t].h != 0)
                                        {
                                            speeds[t][i] =
                                                +ratio * (tagBases[t].h * basespeeds[i] * tags[t].h / tagBases[t].h
                                                + tagBases[t].ty * tags[t].h / tagBases[t].h)
                                                - tags[t].ty;
                                        }
                                        else
                                        {
                                            speeds[t][i] = ratio * (tagBases[t].h * basespeeds[i] + tagBases[t].ty) - tags[t].ty;
                                        }
                                    }
                                    else
                                    {

                                        speeds[t][i] = (ratio * tags[t].ty) - tags[t].ty;


                                    }
                                    break;
                                case _DEGREE:
                                    speeds[t][i] = basespeeds[i] * Mathf.boolToSign(e.mirror);
                                    break;

                                case _DX:

                                    if (!float.IsNaN(basespeeds[i]) && Jtags[t] != null)
                                    {
                                        speeds[t][i] = basespeeds[i] + JtagBases[t].px - Jtags[t].px;


                                    }
                                    else
                                    {
                                        speeds[t][i] = 0;
                                    }
                                    break;
                                case _DY:

                                    if (!float.IsNaN(basespeeds[i]) && Jtags[t] != null)
                                    {
                                        speeds[t][i] = basespeeds[i] + JtagBases[t].py - Jtags[t].py;
                                    }
                                    else
                                    {
                                        speeds[t][i] = 0;
                                    }
                                    break;

                                default:
                                    speeds[t][i] = basespeeds[i];
                                    break;
                            }
                        }
                        else
                        {
                            if (i == _DEGREE)
                            {
                                speeds[t][i] = basespeeds[i] * Mathf.boolToSign(e.mirror);
                            }
                            else
                            {
                                speeds[t][i] = basespeeds[i];

                            }
                        }

                        if (i == _DEGREE && RO != rotateOption.F)
                        {
                            if (float.IsNaN(basespeeds[i]))
                            {
                                speeds[t][i] = 0;
                            }
                            else
                            {
                                var baseDegree = basespeeds[i];
                                if (e.mirror)
                                {
                                    baseDegree = Mathf.st180(tagBases[t].degree - Mathf.st180(baseDegree - tagBases[t].degree));
                                }
                                if (RO == rotateOption.joint)
                                {
                                    if (Jtags[t] != null)
                                    {

                                        speeds[t][i] = Mathf.st180(baseDegree + Jtags[t].parent.degree - tags[t].degree);
                                    }
                                    else//キャラクターの一番上なら自動でワールド角度に 
                                    {
                                        speeds[t][i] = Mathf.st180(baseDegree - tags[t].degree);
                                    }
                                }
                                else if (RO == rotateOption.baseCharacter)
                                {
                                    speeds[t][i] = Mathf.st180(tagBases[t].degree + baseDegree - tags[t].degree);
                                }
                                else
                                {
                                    speeds[t][i] = Mathf.st180(baseDegree - tags[t].degree);
                                };
                                switch (RP)
                                {
                                    case rotatePath.shorts:
                                        speeds[t][i] = speeds[t][i];
                                        break;
                                    case rotatePath.plus:
                                        if (e.mirror)
                                        {
                                            if (speeds[t][i] > 0)
                                            {
                                                speeds[t][i] = speeds[t][i] - 360;
                                            }
                                        }
                                        else
                                        {
                                            if (speeds[t][i] < 0)
                                            {
                                                speeds[t][i] = 360 + speeds[t][i];
                                            }
                                        }
                                        break;
                                    case rotatePath.minus:
                                        if (e.mirror)
                                        {
                                            if (speeds[t][i] < 0)
                                            {
                                                speeds[t][i] = 360 + speeds[t][i];
                                            }
                                        }
                                        else
                                        {
                                            if (speeds[t][i] > 0)
                                            {
                                                speeds[t][i] = speeds[t][i] - 360;
                                            }
                                        }

                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                    else if (GO == goOption.def)//Characterの傘下。scaleは計算しなおすけど、回転とかは根本と同じ
                    {
                        if (SO != scaleOption.F)//相対的な変化だったら
                        {
                            var baseE = tagBases[t];
                            switch (i)
                            {
                                case _W:
                                    if (!float.IsNaN(basespeeds[i]))
                                    {
                                        speeds[t][i] = basespeeds[i] * tagBases[t].w - tags[t].w;
                                    }
                                    else
                                    {
                                        speeds[t][i] = 0;
                                    }
                                    break;
                                case _H:
                                    if (!float.IsNaN(basespeeds[i]))
                                    {
                                        speeds[t][i] = basespeeds[i] * tagBases[t].h - tags[t].h;
                                    }
                                    else
                                    {
                                        speeds[t][i] = 0;
                                    }
                                    break;
                                case _TX:
                                    if (!float.IsNaN(basespeeds[_W]))
                                    {
                                        if (tags[t].w != 0)
                                        {
                                            speeds[t][i] = (basespeeds[_W]) * tagBases[t].w / tags[t].w * tags[t].tx - tags[t].tx;

                                        }
                                        else
                                        {
                                            speeds[t][i] = (basespeeds[_W]) * tags[t].tx - tags[t].tx;
                                        }
                                    }
                                    break;
                                case _TY:
                                    if (!float.IsNaN(basespeeds[_H]))
                                    {
                                        if (tags[t].h != 0)
                                        {
                                            speeds[t][i] = (basespeeds[_H]) * tagBases[t].h / tags[t].h * tags[t].ty - tags[t].ty;
                                        }
                                        else
                                        {
                                            speeds[t][i] = (basespeeds[_H]) * tags[t].ty - tags[t].ty;
                                        }
                                    }
                                    break;
                                case _DX:
                                    if (Jtags[t] != null && !float.IsNaN(basespeeds[i]))
                                    {
                                        speeds[t][i] = basespeeds[i] * JtagBases[t].px - Jtags[t].px;
                                    }
                                    else
                                    {
                                        speeds[t][i] = 0;
                                    }

                                    break;
                                case _DY:
                                    if (Jtags[t] != null && !float.IsNaN(basespeeds[i]))
                                    {
                                        speeds[t][i] = basespeeds[i] * JtagBases[t].py - Jtags[t].py;
                                    }
                                    else
                                    {
                                        speeds[t][i] = 0;
                                    }
                                    break;
                                default:
                                    speeds[t][i] = speeds[0][i];
                                    break;
                            }
                        }
                        else//絶対的な変化だったら。
                        {
                            switch (i)
                            {
                                case _W:
                                    speeds[t][i] = basespeeds[i] * 0;
                                    break;
                                case _H:
                                    speeds[t][i] = basespeeds[i] * 0;
                                    break;
                                case _TX:
                                    speeds[t][i] = basespeeds[i] * 0;
                                    break;
                                case _TY:
                                    speeds[t][i] = basespeeds[i] * 0;
                                    break;
                                case _DX:
                                    speeds[t][i] = basespeeds[i] * 0;
                                    break;
                                case _DY:
                                    speeds[t][i] = basespeeds[i] * 0;
                                    break;
                                default:
                                    speeds[t][i] = speeds[0][i];
                                    break;
                            }
                        }
                    }
                    else
                    {
                        speeds[t][i] = 0;
                    }
                }
            }
            if (degreeSpeedLimit >= 0) //無理やりの実装
            {
                //    addDefference(cl);
            }
            else if (instant)
            {
                addDefference(1);
            }
            /*else
            {
                float speed = 1 / time;
                addDefference(speed * cl);
            }*/
            base.onadd(cl);
        }
        protected override void onupdate(float cl)
        {
            if (cl > 0)
            {
                if (degreeSpeedLimit >= 0) //無理やりの実装
                {
                    addDefference(cl);

                }
                else if (!instant)
                {
                    float speed = 1 / time;
                    addDefference(speed * cl);
                }
            }
            base.onupdate(cl);

        }


        public static Motion ToloadM2(DataSaver d)
        {
            var res = new Motion();
            var dd = d.unpackDataD("motion");

            res.speed = dd.unpackDataF("sp");
            res.loop = dd.unpackDataB("loop");
            var moves = dd.unpackDataD("moves");
            foreach (var name in moves.getAllPacks())
            {
                var ns = name.Split(':')[0].Split('.');
                var tname = ns.Last();

                var ddd = moves.unpackDataD(name);

                if (tname == "setumageman")
                {


                    res.addmove(
                        EntityMove.RotateToLim(ddd.unpackDataF("time"), ddd.unpackDataS("name")
                        , ddd.unpackDataF("sitato"), Mathf.abs(ddd.unpackDataF("sitasp")), ddd.unpackDataB("saitan", false), true),
                        ddd.unpackDataB("stop"));
                }
                else if (tname == "radtoman")
                {
                    res.addmove(
                        EntityMove.RotateToLim(ddd.unpackDataF("time"), ddd.unpackDataS("name")
                        , ddd.unpackDataF("sitato"), Mathf.abs(ddd.unpackDataF("sitasp")), ddd.unpackDataB("saitan", false), false),
                        ddd.unpackDataB("stop"));
                }
                else if (tname == "texchangeman")
                {

                    res.addmove(
                        DrawableMove.ChangeTexture(ddd.unpackDataS("name"), ddd.unpackDataS("tex"))
                        , false);
                }
                else if (tname == "setuidouman")
                {
                    res.addmove(
                        EntityMove.XYD(ddd.unpackDataF("time"), ddd.unpackDataS("name"),
                        ddd.unpackDataF("vdx") * ddd.unpackDataF("time")
                        , ddd.unpackDataF("vdy") * ddd.unpackDataF("time")
                        , ddd.unpackDataF("vsita") * ddd.unpackDataF("time"))

                        , ddd.unpackDataB("stop"));
                }
                else if (tname == "Kzchangeman")
                {
                    res.addmove(
                        DrawableMove.BaseZChange(ddd.unpackDataS("name"), ddd.unpackDataS("toname"),
                        ddd.unpackDataF("dz"))
                        , false);
                }
            }
            return res;
        }


    }

    public class EntityMirror : Component
    {

        List<Entity> tags = new List<Entity>();
        List<float[]> speeds = new List<float[]>();
        List<Entity> tagBases = new List<Entity>();

        MirrorOption MO;

        bool goall = true;
        bool isMoveDegree = false;

        bool mirrored = false;
        float degree = 0;
        const int _W = 0;
        const int _H = 1;
        const int _TX = 2;
        const int _TY = 3;
        const int _DEGREE = 4;
        const int _MIRRORED = 5;
        bool instant { get { return time == 0; } }


        public EntityMirror() { }
        public EntityMirror(string name, float time, MirrorOption mo, bool goall, bool isMoveDegree = true) : base(time, name)
        {
            this.MO = mo;
            if (time == 0)
            {
            }
            else if (time < 0)
            {
                Debug.WriteLine("このコンポーネントはtime<0じゃダメなので勝手に0にしました♡");
                this.time = 0;
            }
            else
            {

            }
            this.goall = goall;
            this.isMoveDegree = isMoveDegree;
        }
        public override void copy(Component c)
        {
            var cc = (EntityMirror)c;
            base.copy(c);
            cc.MO = this.MO;
            cc.mirrored = this.mirrored;
            cc.degree = this.degree;
            cc.goall = this.goall;

            cc.isMoveDegree = this.isMoveDegree;
        }

        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("MO", (int)MO);
            res.packAdd("goall", goall);
            res.packAdd("isMoveDegree", isMoveDegree);
            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            MO = (MirrorOption)d.unpackDataF("MO");
            goall = d.unpackDataB("goall");
            isMoveDegree = d.unpackDataB("isMoveDegree", isMoveDegree);
        }

        public override void resettimer()
        {
            mirrored = false;
            degree = 0;
            base.resettimer();
        }
        void enmirror(bool degree)
        {
            if (tags.Count <= 0) return;
            bool topmirrored = tags[0].mirror;
            for (int i = 0; i < tags.Count; i++)
            {
                var a = tags[i];
                if (i == 0)
                {
                    if (a == e)//トップだったら
                    {
                        switch (MO)
                        {
                            case MirrorOption.No:
                                a.mirror = tagBases[i]==tags[i]? false: tagBases[i].mirror;
                                break;
                            case MirrorOption.Mirror:
                                a.mirror = tagBases[i] == tags[i] ? true:!tagBases[i].mirror;
                                break;
                            case MirrorOption.Reverse:
                                a.mirror = !a.mirror;
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (MO)
                        {
                            case MirrorOption.No:
                                a.mirror = e.mirror;
                                break;
                            case MirrorOption.Mirror:
                                a.mirror = !e.mirror;
                                break;
                            case MirrorOption.Reverse:
                                a.mirror = !a.mirror;
                                break;
                            default:
                                break;
                        }
                    }
                    topmirrored = topmirrored != a.mirror;

                    if (topmirrored && degree)
                    {
                        speeds[i][_MIRRORED] = 1;
                        a.degree = Mathf.st180(tagBases[i].degree - Mathf.st180(a.degree - tagBases[i].degree));
                    }
                }
                else if (goall)
                {
                    if (topmirrored)
                    {
                        a.mirror = !a.mirror;
                    }
                    if (topmirrored && degree)
                    {
                        speeds[i][_MIRRORED] = 1;
                        a.degree = Mathf.st180(tagBases[i].degree - Mathf.st180(a.degree - tagBases[i].degree));
                    }
                }



            }
        }
        protected override void onadd(float cl)
        {
            tags.Clear();
            tagBases.Clear();
            speeds.Clear();
            var cs = e.getcompos<Character>();
            if (cs.Count == 0)
            {

                tags.Add(e);
                tagBases.Add(e);
                speeds.Add(new float[] { e.w, 0, e.tx, 0, 0, 0 });
            }
            else
            {
                foreach (var a in cs)
                {
                    foreach (var b in a.getTree(name))
                    {
                        tags.Add(b);
                    }
                    foreach (var b in a.BaseCharacter.getTree(name))
                    {
                        tagBases.Add(b);
                    }
                    for (int i = 0; i < tagBases.Count; i++)
                    {
                        if (i == 0 || goall == true)
                        {
                            speeds.Add(new float[] { tags[i].w, 0, tags[i].tx, 0
                            ,
                         Mathf.st180(tagBases[i].degree - Mathf.st180(tags[i].degree - tagBases[i].degree)-tags[i].degree)
                        ,0});
                        }
                        else
                        {
                            speeds.Add(new float[] { 0, 0, 0, 0
                            ,0
                            ,0
                        });
                        }
                    }
                }

            }
            if (instant && !mirrored)
            {
                mirrored = true;
                enmirror(true);
                var ratio = 1;
                for (int i = 0; i < tagBases.Count; i++)
                {
                    var a = tags[i];
                    if (isMoveDegree && speeds[i][_MIRRORED] == 1)
                    {
                        a.degree += speeds[i][_DEGREE] * ratio;
                    }
                }
            }
            base.onadd(cl);
        }
        protected override void onupdate(float cl)
        {
            base.onupdate(cl);
            if (!instant)
            {
                if (!mirrored && timer > time / 2)
                {
                    mirrored = true;
                    enmirror(false);
                }
                float ddegree = cl / time * 180;
                float ratio = Mathf.abs(Mathf.cos(degree + ddegree)) - Mathf.abs(Mathf.cos(degree));
                float ratio2 = cl / time;

                for (int i = 0; i < tagBases.Count; i++)
                {
                    if (speeds[i][_MIRRORED] == 1)
                    {
                        var a = tags[i];
                        var xy = a.gettxy();
                        a.w += speeds[i][_W] * ratio;
                        a.h += speeds[i][_H] * ratio;

                        a.tx += speeds[i][_TX] * ratio;
                        a.ty += speeds[i][_TY] * ratio;
                        if (isMoveDegree)
                        {
                            a.degree += speeds[i][_DEGREE] * ratio2;
                        }
                        a.settxy(xy);
                    }
                }
                degree += ddegree;

            }
        }
    }




    /// <summary>
    /// 値を直接的に変更するクラス。もう一つはmirrorのクラス。
    /// TODO:mirror After 基準
    /// </summary>
    public partial class DrawableMove : Component
    {
        List<List<float[]>> speeds = new List<List<float[]>>();
        List<List<Drawable>> tags = new List<List<Drawable>>();
        List<List<Drawable>> tagBases = new List<List<Drawable>>();
        float[] basespeeds = new float[5];
        /// <summary>
        /// "\\"で、ベース。
        /// </summary>
        string texture = "_";
        string zname = "_";

        goOption GO = goOption.def;

        Drawable zparts;
        float minz, maxz;

        const int _Z = 0;
        const int _R = 1;
        const int _G = 2;
        const int _B = 3;
        const int _OPA = 4;
        bool instant { get { return time <= 0; } }




        protected changeOption CO = changeOption.difference;

        public DrawableMove() { }
        public DrawableMove(float time, float dz = 0, float dr = 0, float dg = 0, float db = 0, float dopa = 0, string texture = ""
            , string name = "") : base(time, name)
        {

            basespeeds[_Z] = dz;
            basespeeds[_R] = dr;
            basespeeds[_G] = dg;
            basespeeds[_B] = db;
            basespeeds[_OPA] = dopa;
            this.texture = texture;

            if (time == 0)
            {
            }
            else if (time < 0)
            {
                Debug.WriteLine("このコンポーネントはtime<0じゃダメなので勝手に0にしました♡");
                this.time = 0;
            }
            else
            {

            }
        }

        public override void copy(Component c)
        {
            var cc = (DrawableMove)c;
            base.copy(c);
            for (int i = 0; i < basespeeds.Length; i++)
            {
                cc.basespeeds[i] = this.basespeeds[i];
            }
            cc.texture = this.texture;
            cc.zname = this.zname;

            cc.CO = this.CO;
            cc.GO = this.GO;
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("GO", (goOption)GO);
            res.packAdd("_Z", basespeeds[_Z]);
            res.packAdd("_R", basespeeds[_R]);
            res.packAdd("_G", basespeeds[_G]);
            res.packAdd("_B", basespeeds[_B]);
            res.packAdd("_OPA", basespeeds[_OPA]);
            res.linechange();
            res.packAdd("texture", texture);
            res.packAdd("zname", zname);
            res.packAdd("CO", (changeOption)CO);

            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);

            GO = d.unpackDataE<goOption>("GO", goOption.def);
            basespeeds[_Z] = d.unpackDataF("_Z");
            basespeeds[_R] = d.unpackDataF("_R");
            basespeeds[_G] = d.unpackDataF("_G");
            basespeeds[_B] = d.unpackDataF("_B");
            basespeeds[_OPA] = d.unpackDataF("_OPA");

            texture = d.unpackDataS("texture");
            zname = d.unpackDataS("zname");

            CO = d.unpackDataE<changeOption>("CO", changeOption.difference);
        }


        public override string ToString()
        {
            var res = base.ToString();
            res += $"\n{CO}";
            res += $" -> Z:{basespeeds[_Z]}:R:{basespeeds[_R]}:G:{basespeeds[_G]}:B:{basespeeds[_B]}:OPA:{basespeeds[_OPA]}" +
                $":TEXTURE:{texture}:";
            return res;
        }
        protected virtual void addDefference(float ratio)
        {
            for (int t = 0; t < tags.Count; t++)
            {
                for (int i = 0; i < tags[t].Count; i++)
                {
                    tags[t][i].z += (float)(speeds[t][i][_Z] * ratio);
                    tags[t][i].col.r += (float)(speeds[t][i][_R] * ratio);
                    tags[t][i].col.g += (float)(speeds[t][i][_G] * ratio);
                    tags[t][i].col.b += (float)(speeds[t][i][_B] * ratio);

                    tags[t][i].col.opa += (float)(speeds[t][i][_OPA] * ratio);
                }
            }
        }
        protected virtual void changes()
        {
            if (texture != "_")
            {
                var lis = new List<List<Drawable>>();
                if (GO == goOption.goAll)
                {
                    lis = tags;
                }
                else
                {
                    if (tags.Count > 0)
                    {
                        lis.Add(tags[0]);
                    }
                }
                for (int i = 0; i < lis.Count; i++)
                {
                    for (int t = 0; t < lis[i].Count; t++)
                    {
                        var a = lis[i][t];
                        if (a.GetType() == typeof(Texture) || a.GetType().IsSubclassOf(typeof(Texture)))
                        {
                            var b = (Texture)(a);
                            var bb = (Texture)(tagBases[i][t]);
                            if (texture == "\\")
                            {
                                b.texname = bb.texname;
                            }
                            else
                            {
                                b.texname = texture;
                            }
                        }
                    }
                }
            }
        }
        protected override void onadd(float cl)
        {
            tags.Clear();
            tagBases.Clear();
            speeds.Clear();
            zparts = null;
            float zpartsdz = 0;
            //characterから得るtag
            var cs = e.getcompos<Character>();
            if (cs.Count == 0)
            {

                tags.Add(e.getcompos<Drawable>());
                tagBases.Add(e.getcompos<Drawable>());
            }
            else
            {
                //こっちはキャラクターのベースね。後で実装
                foreach (var a in cs)
                {
                    foreach (var b in a.getTree(name))
                    {

                        var lis = b.getcompos<Drawable>();
                        tags.Add(lis);
                    }
                    foreach (var b in a.BaseCharacter.getTree(name))
                    {
                        var lis = b.getcompos<Drawable>();
                        tagBases.Add(lis);
                        foreach (var c in lis)
                        {
                            minz = Mathf.min(minz, c.z);
                            maxz = Mathf.max(maxz, c.z);
                        }
                    }
                }
                if (zname != "_")
                {
                    var geC = cs[0].BaseCharacter.getEntity(zname);
                    var ge = cs[0].getEntity(name);
                    if (ge != null)
                    {
                        var drawsC = geC.getcompos<Drawable>();
                        var draws = ge.getcompos<Drawable>();
                        if (drawsC.Count > 0 && draws.Count > 0)
                        {
                            zparts = drawsC[0];
                            zpartsdz = ((maxz - minz) * basespeeds[_Z]) + zparts.z - draws[0].z;
                        }
                    }

                }
            }
            for (int t = 0; t < tags.Count; t++)
            {
                speeds.Add(new List<float[]>());
                for (int i = 0; i < tags[t].Count; i++)
                {
                    speeds[t].Add(new float[basespeeds.Length]);
                    for (int j = 0; j < basespeeds.Length; j++)
                    {
                        if (t == 0 || GO == goOption.goAll)
                        {
                            if (CO != changeOption.difference)
                            {
                                switch (j)
                                {
                                    case _Z:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {
                                            if (zparts != null)
                                            {
                                                speeds[t][i][j] = zpartsdz;

                                            }
                                            else
                                            {
                                                speeds[t][i][j] = ((maxz - minz) * basespeeds[j]) + tagBases[t][i].z - tags[t][i].z;

                                            }
                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;

                                        }
                                        break;
                                    case _R:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {
                                            speeds[t][i][j] = tagBases[t][i].col.r * basespeeds[j] - tags[t][i].col.r;
                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                    case _G:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {
                                            speeds[t][i][j] = tagBases[t][i].col.g * basespeeds[j] - tags[t][i].col.g;
                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                    case _B:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {
                                            speeds[t][i][j] = tagBases[t][i].col.b * basespeeds[j] - tags[t][i].col.b;
                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                    case _OPA:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {

                                            speeds[t][i][j] = tagBases[t][i].col.opa * basespeeds[j] - tags[t][i].col.opa;

                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                };

                            }
                            else
                            {
                                speeds[t][i][j] = basespeeds[j];
                            }
                        }
                        else if (GO != goOption.onlyRoot)
                        {
                            if (CO != changeOption.difference)
                            {
                                switch (j)
                                {

                                    case _R:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {
                                            speeds[t][i][j] = tagBases[t][i].col.r * basespeeds[j] - tags[t][i].col.r;
                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                    case _G:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {
                                            speeds[t][i][j] = tagBases[t][i].col.g * basespeeds[j] - tags[t][i].col.g;
                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                    case _B:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {
                                            speeds[t][i][j] = tagBases[t][i].col.b * basespeeds[j] - tags[t][i].col.b;
                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                    case _OPA:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {
                                            speeds[t][i][j] = tagBases[t][i].col.opa * basespeeds[j] - tags[t][i].col.opa;

                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                    default:
                                        if (speeds[0].Count > 0)
                                        {
                                            speeds[t][i][j] = speeds[0][i][j];
                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                }
                            }
                            else
                            {
                                if (j == _Z)
                                {
                                    if (zparts != null)
                                    {
                                        speeds[t][i][j] = zpartsdz;
                                    }
                                    else
                                    {
                                        speeds[t][i][j] = basespeeds[j];
                                    }
                                }
                                else
                                {
                                    speeds[t][i][j] = basespeeds[j];
                                }
                            }
                        }
                        else
                        {
                            speeds[t][i][j] = 0;
                        }
                    }
                }
            }
            if (instant)
            {
                addDefference(1);
            }
            /* else
             {
                 float speed = 1 / time;
                 addDefference(Math.Min(speed * cl,1));
             }*/
            base.onadd(cl);
        }
        protected override void onupdate(float cl)
        {
            if (cl > 0)
            {
                if (!instant)
                {
                    float speed = 1 / time;
                    addDefference(speed * cl);
                }
            }
            base.onupdate(cl);

        }
        protected override void onremove(float cl)
        {
            changes();
            base.onremove(cl);
        }


    }

    /// <summary>
    /// ZDeltaRatioを操作するクラス
    /// </summary>
    public partial class ZDeltaMove : Component
    {
        List<List<float[]>> speeds = new List<List<float[]>>();
        List<List<Drawable>> tags = new List<List<Drawable>>();
        List<List<Drawable>> tagBases = new List<List<Drawable>>();
        float[] basespeeds = new float[2];


        public goOption GO = goOption.def;

        Drawable zparts;

        const int _ZDelta = 0;
        const int _ZRatio = 1;
        bool instant { get { return time <= 0; } }




        public changeOption CO = changeOption.difference;

        public ZDeltaMove() { }
        public ZDeltaMove(float time, float zDelta = 0, float zRatio = 0, string name = "") : base(time, name)
        {

            basespeeds[_ZDelta] = zDelta;
            basespeeds[_ZRatio] = zRatio;

            if (time == 0)
            {
            }
            else if (time < 0)
            {
                Debug.WriteLine("このコンポーネントはtime<0じゃダメなので勝手に0にしました♡");
                this.time = 0;
            }
            else
            {

            }
        }

        public override void copy(Component c)
        {
            var cc = (ZDeltaMove)c;
            base.copy(c);
            for (int i = 0; i < basespeeds.Length; i++)
            {
                cc.basespeeds[i] = this.basespeeds[i];
            }

            cc.CO = this.CO;
            cc.GO = this.GO;
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("GO", (goOption)GO);
            res.packAdd("_ZDelta", basespeeds[_ZDelta]);
            res.packAdd("_ZRatio", basespeeds[_ZRatio]);
            res.linechange();
            res.packAdd("CO", (changeOption)CO);

            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);

            GO = d.unpackDataE<goOption>("GO", goOption.def);
            basespeeds[_ZDelta] = d.unpackDataF("_ZDelta");
            basespeeds[_ZRatio] = d.unpackDataF("_ZRatio");
            
            CO = d.unpackDataE<changeOption>("CO", changeOption.difference);
        }


        public override string ToString()
        {
            var res = base.ToString();
            res += $"\n{CO}";
            res += $" -> ZDelta:{basespeeds[_ZDelta]}:ZRatio:{basespeeds[_ZRatio]}";
            return res;
        }
        protected virtual void addDefference(float ratio)
        {
            for (int t = 0; t < tags.Count; t++)
            {
                for (int i = 0; i < tags[t].Count; i++)
                {
                    tags[t][i].zDelta += (float)(speeds[t][i][_ZDelta] * ratio);
                    tags[t][i].zRatio += (float)(speeds[t][i][_ZRatio] * ratio);
                }
            }
        }
        protected override void onadd(float cl)
        {
            tags.Clear();
            tagBases.Clear();
            speeds.Clear();
            zparts = null;
            float zpartsdz = 0;
            //characterから得るtag
            var cs = e.getcompos<Character>();
            if (cs.Count == 0)
            {

                tags.Add(e.getcompos<Drawable>());
                tagBases.Add(e.getcompos<Drawable>());
            }
            else
            {
                //こっちはキャラクターのベースね。後で実装
                foreach (var a in cs)
                {
                    foreach (var b in a.getTree(name))
                    {

                        var lis = b.getcompos<Drawable>();
                        tags.Add(lis);
                    }
                    foreach (var b in a.BaseCharacter.getTree(name))
                    {
                        var lis = b.getcompos<Drawable>();
                        tagBases.Add(lis);
                    }
                }
              
            }
            for (int t = 0; t < tags.Count; t++)
            {
                speeds.Add(new List<float[]>());
                for (int i = 0; i < tags[t].Count; i++)
                {
                    speeds[t].Add(new float[basespeeds.Length]);
                    for (int j = 0; j < basespeeds.Length; j++)
                    {
                        if (t == 0 || GO == goOption.goAll)
                        {
                            if (CO != changeOption.difference)
                            {
                                switch (j)
                                {
                                    case _ZDelta:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {

                                            speeds[t][i][j] = (basespeeds[j]* tagBases[t][i].zDelta) - tags[t][i].zDelta;

                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;

                                        }
                                        break;
                                    case _ZRatio:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {

                                            speeds[t][i][j] = (basespeeds[j] * tagBases[t][i].zRatio) - tags[t][i].zRatio;

                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                 
                                }
                                ;

                            }
                            else
                            {
                                speeds[t][i][j] = basespeeds[j];
                            }
                        }
                        else if (GO != goOption.onlyRoot)
                        {
                            if (CO != changeOption.difference)
                            {
                                switch (j)
                                {

                                    case _ZDelta:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {
                                            speeds[t][i][j] = (basespeeds[j] * tagBases[t][i].zDelta) - tags[t][i].zDelta;
                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;
                                    case _ZRatio:
                                        if (!float.IsNaN(basespeeds[j]))
                                        {
                                            speeds[t][i][j] = (basespeeds[j] * tagBases[t][i].zRatio) - tags[t][i].zRatio;
                                        }
                                        else
                                        {
                                            speeds[t][i][j] = 0;
                                        }
                                        break;

                                }
                            }
                            else
                            {

                                speeds[t][i][j] = basespeeds[j];

                            }
                        }
                        else
                        {
                            speeds[t][i][j] = 0;
                        }
                    }
                }
            }
            if (instant)
            {
                addDefference(1);
            }
            /* else
             {
                 float speed = 1 / time;
                 addDefference(Math.Min(speed * cl,1));
             }*/
            base.onadd(cl);
        }
        protected override void onupdate(float cl)
        {
            if (cl > 0)
            {
                if (!instant)
                {
                    float speed = 1 / time;
                    addDefference(speed * cl);
                }
            }
            base.onupdate(cl);

        }


    }
    /// <summary>
    /// Z以外の軸で回転させる動き
    /// </summary>
    public partial class ZRotateMove : Component
    {
        List<float[]> speeds = new List<float[]>();
        List<Entity> tags = new List<Entity>();
        List<Entity> tagbases = new List<Entity>();
        float[] basespeeds = new float[12];

        public goOption GO = goOption.def;


        const int _STX = 0;
        const int _ENX = 1;
        const int _STY = 2;
        const int _ENY = 3;
        const int _SCX = 4;
        const int _SCY = 5;
        const int _SCXE = 6;
        const int _SCYE = 7;

        const int _TXS = 8;
        const int _TXE = 9;
        const int _TYS = 10;
        const int _TYE = 11;
        bool instant { get { return time <= 0; } }





        public ZRotateMove() { }
        public ZRotateMove(float time, float stx, float enx, float sty, float eny, float scx, float scy, string name = "") : base(time, name)
        {

            basespeeds[_STX] = stx;
            basespeeds[_ENX] = enx;
            basespeeds[_STY] = sty;
            basespeeds[_ENY] = eny;

            basespeeds[_SCX] = scx;
            basespeeds[_SCY] = scy;

            if (time == 0)
            {
            }
            else if (time < 0)
            {
                Debug.WriteLine("このコンポーネントはtime<0じゃダメなので勝手に0にしました♡");
                this.time = 0;
            }
            else
            {

            }
        }

        public override void copy(Component c)
        {
            var cc = (ZRotateMove)c;
            base.copy(c);
            for (int i = 0; i < basespeeds.Length; i++)
            {
                cc.basespeeds[i] = this.basespeeds[i];
            }

            cc.GO = this.GO;
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("GO", (goOption)GO);
            res.packAdd("_STX", basespeeds[_STX]);
            res.packAdd("_ENX", basespeeds[_ENX]);
            res.packAdd("_STY", basespeeds[_STY]);
            res.packAdd("_ENY", basespeeds[_ENY]);
            res.packAdd("_SCX", basespeeds[_SCX]);
            res.packAdd("_SCY", basespeeds[_SCY]);

            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);

            GO = d.unpackDataE<goOption>("GO", goOption.def);
            basespeeds[_STX] = d.unpackDataF("_STX", 0);
            basespeeds[_ENX] = d.unpackDataF("_ENX", 0);
            basespeeds[_STY] = d.unpackDataF("_STY", 0);
            basespeeds[_ENY] = d.unpackDataF("_ENY", 0);
            basespeeds[_SCX] = d.unpackDataF("_SCX", 1);
            basespeeds[_SCY] = d.unpackDataF("_SCY", 1);

        }


        public override string ToString()
        {
            var res = base.ToString();
            res += $" -> STX:{basespeeds[_STX]}:ENX:{basespeeds[_ENX]}:STY:{basespeeds[_STY]}:ENY:{basespeeds[_ENY]}:SCX:{basespeeds[_SCX]}:SCY:{basespeeds[_SCY]}" +
                $"";
            return res;
        }
        protected virtual void ToValue(float time)
        {
            for (int t = 0; t < tags.Count; t++)
            {
                var txy = tags[t].gettxy();

                float nowX = speeds[t][_STX] + (speeds[t][_ENX] - speeds[t][_STX]) * time;
                float nowY = speeds[t][_STY] + (speeds[t][_ENY] - speeds[t][_STY]) * time;

                tags[t].w = (speeds[t][_SCX] + (speeds[t][_SCXE] - speeds[t][_SCX]) * time) * Mathf.cos(nowX);
                tags[t].tx = (speeds[t][_TXS] + (speeds[t][_TXE] - speeds[t][_TXS]) * time) * Mathf.cos(nowX);



                tags[t].h = (speeds[t][_SCY] + (speeds[t][_SCYE] - speeds[t][_SCY]) * time) * Mathf.cos(nowY);
                tags[t].ty = (speeds[t][_TYS] + (speeds[t][_TYE] - speeds[t][_TYS]) * time) * Mathf.cos(nowY);

                tags[t].settxy(txy);

            }
        }
        protected override void onadd(float cl)
        {
            tags.Clear();
            speeds.Clear();
            //characterから得るtag
            var cs = e.getcompos<Character>();
            if (cs.Count == 0)
            {

                tags.Add(e);
                tagbases.Add(e);
            }
            else
            {
                if (GO == goOption.def || GO == goOption.goAll)
                {
                    foreach (var a in cs)
                    {
                        foreach (var b in a.getTree(name))
                        {
                            tags.Add(b);
                        }
                        foreach (var b in a.BaseCharacter.getTree(name))
                        {
                            tagbases.Add(b);
                        }
                    }
                }
                else
                {
                    foreach (var a in cs)
                    {
                        {
                            var tag = a.getEntity(name);
                            if (tag != null)
                            {
                                tags.Add(tag);
                            }
                        }
                        {
                            var tag = a.BaseCharacter.getEntity(name);
                            if (tag != null)
                            {
                                tagbases.Add(tag);
                            }
                        }
                    }
                }

            }
            for (int t = 0; t < tags.Count; t++)
            {
                speeds.Add(new float[12]);
                for (int i = 0; i < 4; i++)
                {
                    speeds[t][i] = basespeeds[i];
                }
                if (Mathf.cos(speeds[t][_STX]) == 0)
                {
                    speeds[t][_SCX] = tagbases[t].w;
                    speeds[t][_TXS] = tagbases[t].tx;


                }
                else
                {
                    speeds[t][_SCX] = tags[t].w / Mathf.cos(speeds[t][_STX]);
                    speeds[t][_TXS] = tags[t].tx / Mathf.cos(speeds[t][_STX]);

                    speeds[t][_SCXE] = speeds[t][_SCX] * Mathf.cos(speeds[t][_ENX]);
                    speeds[t][_TXE] = speeds[t][_TXS] * Mathf.cos(speeds[t][_ENX]);
                }
                speeds[t][_SCXE] = speeds[t][_SCXE] * (basespeeds[_SCX]);
                speeds[t][_TXE] = speeds[t][_TXE] * (basespeeds[_SCX]);
                if (Mathf.cos(speeds[t][_STY]) == 0)
                {
                    speeds[t][_SCY] = tagbases[t].h;
                    speeds[t][_TYS] = tagbases[t].ty;
                }
                else
                {
                    speeds[t][_SCY] = tags[t].h / Mathf.cos(speeds[t][_STY]);
                    speeds[t][_TYS] = tags[t].ty / Mathf.cos(speeds[t][_STY]);
                }
                speeds[t][_SCYE] = speeds[t][_SCY] * basespeeds[_SCY];
                speeds[t][_TYE] = speeds[t][_TYS] * basespeeds[_SCY];

            }
            if (instant)
            {

                this.ToValue(1);
            }
            /* else
             {
                 float speed = 1 / time;
                 addDefference(Math.Min(speed * cl,1));
             }*/
            base.onadd(cl);
        }
        protected override void onupdate(float cl)
        {
            if (cl > 0)
            {
                if (!instant)
                {
                    this.ToValue(timer / time);
                }
            }
            base.onupdate(cl);

        }
        protected override void onremove(float cl)
        {
            base.onremove(cl);
        }


    }
    /// <summary>
    /// テキストを編集するムーブ
    /// </summary>
    public partial class TextMove : Component
    {
        public string Tag;
        public string Text;//即時変更 ""で変更なし。 ""にするにはTextLengthを0にする。

        public FontC Font;
        public float TextLength = 0;


        List<List<float[]>> speeds = new List<List<float[]>>();
        List<List<float[]>> bspeeds = new List<List<float[]>>();
        List<List<Text>> tags = new List<List<Text>>();
        List<List<string>> TagTexts=new List<List<string>>();
        bool instant { get { return time <= 0; } }

        const int _FSIZE = 0;
        const int _FW = 1;
        const int _FH = 2;
        const int _FHutizure = 3;
        const int _FHR = 4;
        const int _FHG = 5;
        const int _FHB = 6;
        const int _FHOPA = 7;
        const int _TEXTLENGTH = 8;
        const int _speedLength = 9;
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="Tag">変化させるターゲット</param>
        /// <param name="time"></param>
        /// <param name="Text">即時反映=""で変化なし</param>
        /// <param name="TextLength">変化するテキストの長さ=0で""になる</param>
        /// <param name="font">変化するフォント。isItaricなどは即時反映</param>
        /// <param name="name"></param>
        public TextMove(string Tag, float time, string Text, float TextLength, FontC font, string name = "") : base(time, name) 
        {
            this.Tag = Tag;
            this.Text = Text;
            this.TextLength = TextLength;

            this.Font = new FontC();
            font.copy(this.Font);
        }

        protected override void onadd(float cl)
        {
            tags.Clear();
            speeds.Clear();
            bspeeds.Clear();
            TagTexts.Clear();
            //characterから得るtag
            var cs = e.getcompos<Character>();
            if (cs.Count == 0)
            {

                tags.Add(e.getcompos<Text>());
            }
            else
            {
                foreach (var a in cs)
                {
                    foreach (var b in a.getTree(name))
                    {

                        var lis = b.getcompos<Text>();
                        tags.Add(lis);
                    }
                }
            }
            for (int t = 0; t < tags.Count; t++)
            {
                speeds.Add(new List<float[]>());
                bspeeds.Add(new List<float[]>());
                TagTexts.Add(new List<string>());
                for (int i = 0; i < tags[t].Count; i++)
                {
                    if (Text == "")
                    {

                        TagTexts[t].Add(tags[t][i].text);
                    }
                    else 
                    {
                        TagTexts[t].Add(Text);
                    }
                    speeds[t].Add(new float[_speedLength]);
                    bspeeds[t].Add(new float[_speedLength]);
                    for (int j = 0; j < speeds[t][i].Length; j++) 
                    {
                        switch (j)
                        {
                            case _FSIZE:
                                speeds[t][i][j] = Font.size;
                                bspeeds[t][i][j] = tags[t][i].font.size;
                                break;
                            case _FW:
                                speeds[t][i][j] = Font.w;
                                bspeeds[t][i][j] = tags[t][i].font.w;

                                break;
                            case _FH:
                                speeds[t][i][j] = Font.h;
                                bspeeds[t][i][j] = tags[t][i].font.h;

                                break;
                            case _FHutizure:
                                speeds[t][i][j] = Font.hutiZure;
                                bspeeds[t][i][j] = tags[t][i].font.hutiZure;
                                break;
                            case _FHR:
                                speeds[t][i][j] = Font.hutiColor.r;
                                bspeeds[t][i][j] = tags[t][i].font.hutiColor.r;
                                break;
                            case _FHG:
                                speeds[t][i][j] = Font.hutiColor.g;
                                bspeeds[t][i][j] = tags[t][i].font.hutiColor.g;
                                break;
                            case _FHB:
                                speeds[t][i][j] = Font.hutiColor.b;
                                bspeeds[t][i][j] = tags[t][i].font.hutiColor.b;
                                break;
                            case _FHOPA:
                                speeds[t][i][j] = Font.hutiColor.opa;
                                bspeeds[t][i][j] = tags[t][i].font.hutiColor.opa;
                                break;

                            case _TEXTLENGTH:
                                speeds[t][i][j] = TextLength;
                                bspeeds[t][i][j] = tags[t][i].text.Length;
                                break;
                        }
                    }
                }
            }
            if (Text == "") 
            {
                
            }
            if (instant)
            {
                setTexts(1);
            }
            /* else
             {
                 float speed = 1 / time;
                 addDefference(Math.Min(speed * cl,1));
             }*/
            base.onadd(cl);
        }
        protected override void onupdate(float cl)
        {

            if (cl > 0)
            {
                if (!instant)
                {
                    this.setTexts(timer / time);
                }
            }
            base.onupdate(cl);

        }
        void setTexts(float ratio) 
        {
            for (int t = 0; t < tags.Count; t++)
            {
                for (int i = 0; i < tags[t].Count; i++)
                {
                    if (Font.ali != FontC.alignment.None)
                    {
                        tags[t][i].font.ali = Font.ali;
                    }
                    if (Font.aliV != FontC.alignment.None)
                    {
                        tags[t][i].font.aliV = Font.aliV;
                    }

                    if (Font.isBold >= 0)
                    {
                        tags[t][i].font.isBold = Font.isBold;
                    }
                    if (Font.isItaric >= 0)
                    {
                        tags[t][i].font.isItaric = Font.isItaric;
                    }
                    if (Font.fontName != "")
                    {
                        tags[t][i].font.fontName = Font.fontName;
                    }
                    if (float.IsNaN(speeds[t][i][_FSIZE]) == false)
                    {
                        tags[t][i].font.size = (speeds[t][i][_FSIZE] * ratio + bspeeds[t][i][_FSIZE] * (1 - ratio));
                    }
                    if (float.IsNaN(speeds[t][i][_FW]) == false)
                    {
                        tags[t][i].font.w = (speeds[t][i][_FW] * ratio + bspeeds[t][i][_FW] * (1 - ratio));
                    }
                    if (float.IsNaN(speeds[t][i][_FH]) == false)
                    {
                        tags[t][i].font.h = (speeds[t][i][_FH] * ratio + bspeeds[t][i][_FH] * (1 - ratio));
                    }
                    if (float.IsNaN(speeds[t][i][_FHutizure]) == false)
                    {
                        tags[t][i].font.hutiZure = (speeds[t][i][_FHutizure] * ratio + bspeeds[t][i][_FHutizure] * (1 - ratio));
                    }
                    if (float.IsNaN(speeds[t][i][_FHR]) == false)
                    {
                        tags[t][i].font.hutiColor.r = (speeds[t][i][_FHR] * ratio + bspeeds[t][i][_FHR] * (1 - ratio));
                    }
                    if (float.IsNaN(speeds[t][i][_FHG]) == false)
                    {
                        tags[t][i].font.hutiColor.g = (speeds[t][i][_FHG] * ratio + bspeeds[t][i][_FHG] * (1 - ratio));
                    }
                    if (float.IsNaN(speeds[t][i][_FHB]) == false)
                    {
                        tags[t][i].font.hutiColor.b = (speeds[t][i][_FHB] * ratio + bspeeds[t][i][_FHB] * (1 - ratio));
                    }
                    if (float.IsNaN(speeds[t][i][_FHOPA]) == false)
                    {
                        tags[t][i].font.hutiColor.opa = (speeds[t][i][_FHOPA] * ratio + bspeeds[t][i][_FHOPA] * (1 - ratio));
                    }

                    float length;
                    if (float.IsNaN(speeds[t][i][_TEXTLENGTH]) == false)
                    {
                        length = (speeds[t][i][_TEXTLENGTH] * ratio + bspeeds[t][i][_TEXTLENGTH] * (1 - ratio));
                    }
                    else
                    {
                        length = tags[t][i].text.Length;
                    }
                    tags[t][i].text = TagTexts[t][i].Substring(0, (int)length);


                }
            }
        }
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.packAdd("Font",Font.ToSave().indent());
            d.linechange();
            d.packAdd("Text", this.Text);
            d.packAdd("TextLength", this.TextLength);

            return d;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            {
                this.Font = new FontC();
                var dd = d.unpackDataD("Font");
                this.Font.ToLoad(dd);
            }
            this.Text=d.unpackDataS("Text", this.Text);
            this.TextLength=d.unpackDataF("TextLength", this.TextLength);
        }
    }


}

   