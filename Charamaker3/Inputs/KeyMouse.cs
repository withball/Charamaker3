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
    public class KeyMouse
    {
        static public KeyMouse raw = new KeyMouse();
        List<IButton> k = new List<IButton>();
        List<IButton> pk = new List<IButton>();
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public KeyMouse() { }
        /// <summary>
        /// マウスの座標。ディスプレイ準拠
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
        public void down(IButton i)
        {
            if (!IButton.Contains(i,k))
            {
                k.Add(i);
            }

        }
        /// <summary>
        /// キーを離す
        /// </summary>
        /// <param name="i">離すキー</param>
        public void up(IButton i)
        {
            IButton.Remove(i,k);
        }
        /// <summary>
        /// 入力を過去のものとする。ゲームのティックに伴って呼び出す。さもなくばitypeが機能しない
        /// </summary>
        public void topre()
        {
            pk.Clear();
            foreach (IButton i in k)
            {
                pk.Add(i.Clone());
            }
        }
      

        /// <summary>
        /// マウスの座標をカーソルからセットする。rawにもセットされる
        /// </summary>
        /// <param name="cam">活動制限のためのhyojiman</param>
        /// <param name="f">座標変換のためのフォーム</param>
        /// /// <param name="gamennai">falseの時、画面外の指定を可能にする</param>
        public void setpointer(Form f, bool gamennai = true)
        {
            var cu = f.PointToClient(Cursor.Position);
            raw.x = cu.X / (float)f.ClientRectangle.Width;
            raw.y = cu.Y / (float)f.ClientRectangle.Height;

            x = raw.x;
            y = raw.y;
        }

        /// <summary>
        /// カメラ座標のインプットを受け取る 
        /// </summary>
        /// <param name="cam"></param>
        /// <param name="gamennai"></param>
        /// <returns></returns>
        public FXY GetCursourPoint(Camera cam, bool gamennai = true)
        {
            FXY res;
            var FXY = new FXY(x, y);
            if (gamennai)
            {
                res = cam.watchRect.gettxy(FXY.x * cam.watchRect.w, FXY.y * cam.watchRect.h);
            }
            else
            {
                res = cam.watchRect.gettxy(Mathf.min(Mathf.max(FXY.x * cam.watchRect.w, 0), cam.watchRect.w)
                    , Mathf.min(Mathf.max(FXY.y * cam.watchRect.h, 0), cam.watchRect.h));
            }
            return res;
        }
        /// <summary>
        /// マウスの座標を中心点との差分でポインタの座標からセットする。
        /// マウスが外に行かないモード
        /// </summary>
        /// 
        /// <param name="prepoint">前回の戻り値。nullはやめてね</param>
        /// <param name="cam">活動制限のためのhyojiman</param>
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
        public bool ok(IButton i, itype t)
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
        public List<IButton> getdownkey()
        {
            var res=new List<IButton>();
            foreach (var a in k) 
            {
                res.Add(a.Clone());
            }
            return res;
        }
        /// <summary>
        /// 過去押されていたキーを取得する
        /// </summary>
        /// <returns>キーの列のコピー</returns>
        public List<IButton> getupkey()
        {
            var res = new List<IButton>();
            foreach (var a in pk)
            {
                res.Add(a.Clone());
            }
            return res;
        }

    }
}

