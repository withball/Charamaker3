using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Charamaker3.Inputs
{
    /// <summary>
    /// インプットのタイプ,押したとき・離されたとき・前述二個OR押されているときの三種類
    /// </summary>
    public enum itype
    {
        /// <summary>
        /// 押されたとき
        /// </summary>
        down,

        /// <summary>
        /// 押されたとき、離されたとき、押されている時すべて
        /// </summary>
        ing,

        /// <summary>
        /// 離されたとき
        /// </summary>
        up
    }
    /// <summary>
    /// キーボードとマウスの入力を扱うクラス。
    /// 毎フレームごとにtopre()とsetpointer()を行い、
    /// formのkeydown,keyupイベントにdown(),up()を接続すれば、
    /// 入力が完成する。
    /// </summary>
    [Serializable]
    public class KeyMouse<T>
        where T:IButton
    {
        static public KeyMouse<T> raw = new KeyMouse<T>();
        List<T> k = new List<T>();
        List<T> pk = new List<T>();
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public KeyMouse() { }
        /// <summary>
        /// マウスの座標
        /// </summary>
        public float x = 0, y = 0;
     
        /// <summary>
        /// 今の全てのボタンを上げる
        /// </summary>
        public void upall()
        {
            this.k.Clear();
        }
        /// <summary>
        /// 前の全てのボタンを上げる
        /// </summary>
        public void preupall()
        {
            this.pk.Clear();
        }
        /// <summary>
        /// キーを押す
        /// </summary>
        /// <param name="i">押すキー</param>
        public void down(T i)
        {
            if (!IButton.Contains<T>(i,k))
            {
                k.Add(i);
            }

        }
        /// <summary>
        /// キーを離す
        /// </summary>
        /// <param name="i">離すキー</param>
        public void up(T i)
        {
            IButton.Remove<T>(i,k);
        }
        /// <summary>
        /// 入力を過去のものとする。ゲームのティックに伴って呼び出す。さもなくばitypeが機能しない
        /// </summary>
        public void topre()
        {
            pk.Clear();
            foreach (T i in k)
            {
                pk.Add((T)i.Clone());
            }
        }
        /// <summary>
        /// 生の入力から座標をもらってくる。UI座標の時使え
        /// </summary>
        /// <param name="hyo">変換先のカメラ</param>
        /// <returns></returns>
        static public FXY fromRaw(Camera cam) 
        {
            FXY xy;
            {
                xy = cam.e.gettxy(Mathf.min(Mathf.max(raw.x * cam.watchRect.w, 0), cam.watchRect.w)
                    , Mathf.min(Mathf.max(raw.y * cam.watchRect.h, 0), cam.watchRect.h));
            }
            return xy;
        }
        /// <summary>
        /// マウスの座標をカーソルからセットする。rawにもセットされる
        /// </summary>
        /// <param name="hyojiman">活動制限のためのhyojiman</param>
        /// <param name="f">座標変換のためのフォーム</param>
        /// /// <param name="gamennai">falseの時、画面外の指定を可能にする</param>
        public void setpointer(Camera cam, Form f, bool gamennai = true)
        {
            var cu = f.PointToClient(Cursor.Position);
            raw.x = cu.X / (float)f.ClientRectangle.Width;
            raw.y = cu.Y / (float)f.ClientRectangle.Height;

            FXY xy;
            if (gamennai)
            {
                xy = cam.watchRect.gettxy(raw.x * cam.watchRect.w, raw.y * cam.watchRect.h);
            }
            else 
            {
                xy = cam.watchRect.gettxy(Mathf.min(Mathf.max(raw.x * cam.watchRect.w,0),cam.watchRect.w)
                    , Mathf.min(Mathf.max(raw.y * cam.watchRect.h, 0),cam.watchRect.h));
            }
            x = xy.x;
            y = xy.y;

        }
        /// <summary>
        /// マウスの座標を中心点との差分でポインタの座標からセットする。
        /// マウスが外に行かないモード
        /// </summary>
        /// 
        /// <param name="prepoint">前回の戻り値。nullはやめてね</param>
        /// <param name="hyojiman">活動制限のためのhyojiman</param>
        /// <param name="f">座標変換のためのフォーム</param>
        /// <param name="gamennai">falseの時、画面外の指定を可能にする</param>
        /// <returns>保存しといて次代入するポイント</returns>>
        public FXY setlockpointer(FXY prepoint,Camera cam, Form f, bool gamennai = true)
        {
            var cu = Cursor.Position;
            var pcu = new FXY(f.Location.X + f.Width / 2, f.Location.Y + f.Height / 2);

            FXY xy;
            var rawx = cu.X / (float)f.ClientRectangle.Width;
            var rawy = cu.Y / (float)f.ClientRectangle.Height;
            if (gamennai)
            {
                xy = cam.watchRect.gettxy(rawx * cam.watchRect.w, rawy * cam.watchRect.h);
            }
            else
            {
                xy = cam.watchRect.gettxy(Mathf.min(Mathf.max(rawx * cam.watchRect.w, 0), cam.watchRect.w)
                    , Mathf.min(Mathf.max(rawy * cam.watchRect.h, 0), cam.watchRect.h));
            }

            x = prepoint.x + xy.x;
            y = prepoint.y + xy.y;

            prepoint = new FXY((int)x, (int)y);

            Cursor.Position = pcu;
            return prepoint;
        }
        /// <summary>
        /// 何かしらのボタンが押されてたりしてるか判定する
        /// </summary>
        /// <param name="t">押し方のタイプ</param>
        /// <returns>押されたりしているか</returns>
        public bool ok(itype t)
        {
            switch (t)
            {
                case itype.down:
                    foreach (var a in k)
                    {
                        if (!IButton.Contains(a,pk)) return true;
                    }
                    return false;
                case itype.ing:
                    return k.Count > 0 || pk.Count > 0 ;
                case itype.up:
                    foreach (var a in pk)
                    {
                        if (!IButton.Contains(a,k)) return true;
                    }
                    return false;
                default:
                    return false;
            }

        }
        /// <summary>
        /// 特定のキーが押されたりしているか判定する
        /// </summary>
        /// <param name="i">そのキー</param>
        /// <param name="t">押されたり仕方のタイプ</param>
        /// <returns>押されたりしているか</returns>
        public bool ok(T i, itype t)
        {

            switch (t)
            {
                case itype.down:

                    return IButton.Contains(i,k) && !IButton.Contains(i,pk);

                case itype.ing:
                    return IButton.Contains(i, k);
                case itype.up:
                    return !IButton.Contains(i, k) && IButton.Contains(i, pk);
                default:
                    return false;
            }

        }
     
        /// <summary>
        /// 現在押されているキーを取得する
        /// </summary>
        /// <returns>キーの列のコピー</returns>
        public List<T> getdownkey()
        {
            var res=new List<T>();
            foreach (var a in k) 
            {
                res.Add((T)a.Clone());
            }
            return res;
        }
        /// <summary>
        /// 過去押されていたキーを取得する
        /// </summary>
        /// <returns>キーの列のコピー</returns>
        public List<T> getupkey()
        {
            var res = new List<T>();
            foreach (var a in pk)
            {
                res.Add((T)a.Clone());
            }
            return res;
        }

    }
}

