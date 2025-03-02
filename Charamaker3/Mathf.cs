using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Charamaker3
{
    static public class Debug 
    {
        static public void WriteLine(string w)
        {
            System.Diagnostics.Debug.WriteLine(w);
        }

        static public void Error(string w)
        {
            System.Diagnostics.Debug.WriteLine(w);
        }

        static List<string>messages= new List<string>();

        /// <summary>
        /// メッセージをため込む
        /// </summary>
        /// <param name="w"></param>
        static public void Mess(string w)
        {
            messages.Add(w);
        }
        /// <summary>
        /// メッセージをすべて取り出す。
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        static public List<string> downMess()
        {
          var mess= new List<string>(messages);
            messages.Clear();
            return mess;
        }
    }
    public class FXY
    {
        public float x, y;


        public float length {get{ return Mathf.sqrt(Mathf.pow(x, 2) + Mathf.pow(y, 2)); }
            set {
                var deg = degree;
                x = value * Mathf.cos(deg);
                y = value * Mathf.sin(deg);
            }
        }

        public float degree { get { return Mathf.atan(y, x); }
            set { 
                var input = Mathf.st180(value);
                var len = length;
                x = len * Mathf.cos(input); 
                y = len * Mathf.sin(input); } 
        }
        public FXY unit { get { var res = new FXY(this); res.length = 1; return res; } }
        public FXY(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        static public FXY bylength(float length, float degree) 
        {
           var s= new FXY(0,0);
            s.length = length;
            s.degree = degree;
            return s;
        }
        public FXY(FXY fxy)
        {
            this.x = fxy.x;
            this.y = fxy.y;
        }

        public static FXY operator +(FXY a, FXY b)
        {
            return new FXY(a.x + b.x, a.y + b.y);
        }
        public static FXY operator -(FXY a, FXY b)
        {
            return new FXY(a.x - b.x, a.y - b.y);
        }
        public static bool operator ==(FXY a, FXY b)
        {
            bool anul = a is null;
            bool bnul = b is null;
            if (anul && bnul) return true;
            if (anul || bnul) return false;
            return a.x == b.x && a.y == b.y;
        }
        public static bool operator !=(FXY a, FXY b)
        {
            bool anul = a is null;
            bool bnul = b is null;
            if (anul && bnul) return false;
            if (anul || bnul) return true;
            return !(a.x == b.x && a.y == b.y);
        }
        public static FXY operator *(FXY a, float b)
        {
            return new FXY(a.x * b, a.y * b);
        }
        public static FXY operator /(FXY a, float b)
        {
            return new FXY(a.x / b, a.y / b);
        }
        /// <summary>
        /// 要素を掛け算するだけ
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FXY operator *(FXY a, FXY b)
        {
            return new FXY(a.x * b.x, a.y * b.y);
        }

        /// <summary>
        /// 要素を割り算するだけ
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static FXY operator /(FXY a, FXY b)
        {
            return new FXY(a.x / b.x, a.y / b.y);
        }
        public static implicit operator System.Drawing.Point(FXY xy) 
        {
            return new System.Drawing.Point((int)xy.x, (int)xy.y);
        }
        override public string ToString()
        {
            return x + " :XY: " + y;
        }
    }
    static public class Mathf 
    {
        /// <summary>
        ///  クラスの親子関係 == かサブクラス
        /// </summary>
        /// <param name="t">対象</param>
        /// <param name="parent">親か同じクラス</param>
        /// <returns>t == parent || t subclassed parent</returns>
        public static bool isSubClassOf(Type t, Type parent) 
        {
            return t == parent || t.IsSubclassOf(parent);
        }
        /// <summary>
        /// 四捨五入
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        public static float round(float value,int digits) 
        {
            return (float)Math.Round(value,digits);
        }

        /// <summary>
        /// 切り上げ
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ceil(float value)
        {
            return (float)Math.Ceiling(value);
        }

        /// <summary>
        /// 切り捨て
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float floor(float value)
        {
            return (float)Math.Floor(value);
        }

        /// <summary>
        /// dgree TO radian
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static double toRadian(float degree) 
        {
            return Math.PI * degree / 180;
        }
        /// <summary>
        /// radian to degree
        /// </summary>
        /// <param name="degree"></param>
        /// <returns></returns>
        public static float toDegree(double radian)
        {
            return (float)(radian/Math.PI*180);
        }
        /// <summary>
        /// -180~+180度に標準化する
        /// </summary>
        /// <returns></returns>
        public static float st180(float d) 
        {
            int i = 1;
            if (d < 0) i = -1;
            var a = (d * i) % (360);

            if (a >= 180) return -(360 - a) * i;
            return a * i;
        }
        /// <summary>
        /// 0~+360度に標準化する
        /// </summary>
        /// <returns></returns>
        public static float st360(float d)
        {
            var res = st180(d);
            if (res < 0) res = 360 + res;
            return res;
        }
        /// <summary>
        /// -90~+90度に標準化する
        /// </summary>
        /// <returns></returns>
        public static float st90(float d)
        {
            var res = st180(d);
            if (Mathf.abs(d) > 90)
            {
                d += 180;
            }
            return st180(d);
        }
        public static float sin(float k360) 
        {
            return (float)Math.Sin(Mathf.toRadian(k360));
        }

        public static float cos(float k360)
        {
            return (float)Math.Cos(Mathf.toRadian(k360));
        }
        public static float tan(float k360)
        {
            return (float)Math.Tan(Mathf.toRadian(k360));
        }
        public static float atan(float y,float x)
        {
            return (float)Mathf.toDegree(Math.Atan2(y,x));
        }
        public const float pi =(float)Math.PI; 
        public const float e =(float)Math.E; 
        /// <summary>
        /// aのb乗
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float pow(float a,float b)
        {
            
            return (float)Math.Pow(a,b);
        }

        public static float sqrt(float a) 
        {
            return (float)Math.Sqrt(a);
        }

        public static float abs(float a) 
        {
            return Math.Abs(a);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b">底</param>
        /// <returns></returns>
        public static float log(float a, float b = Mathf.e) 
        {
            return (float)Math.Log(a,b);
        }
        /// <summary>
        /// 最も大きい値を返す
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static float max(params float[] a) 
        {
            int idx = 0;
            
            for (int i = 1; i < a.Length; i++) 
            {
                if (
                    (float.IsNaN(a[idx])|| (a[idx] < a[i])) && !float.IsNaN(a[i])
                    ) idx = i;
            }
            return a[idx];
        }

        /// <summary>
        /// 最も大きい値を返す
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static float min(params float[] a)
        {
            int idx = 0;
            for (int i = 1; i < a.Length; i++)
            {
                if (
                   (float.IsNaN(a[idx]) || (a[idx] > a[i])) && !float.IsNaN(a[i])
                   ) idx = i;
            }

            return a[idx];
        }

        /// <summary>
        /// 値の+-を参照に合わせる
        /// </summary>
        /// <param name="value"></param>
        /// <param name="refer">参照</param>
        /// <returns></returns>
        public static float sameSign(float value, float refer) 
        {
            if (refer < 0) 
            {
                return Mathf.abs(value)*-1;
            }
            return Mathf.abs(value);
        }
        /// <summary>
        /// 同じ+-かどうか判定。0は+-両方
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsSameSign(float a, float b)
        {
            if (a > 0 && b > 0) return true;
            if (a < 0 && b < 0) return true;
            if (a == 0 || b == 0) return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">falseなら1,trueなら-1</param>
        /// <returns>falseなら1,trueなら-1</returns>
        public static float boolToSign(bool value)
        {
            if (value) 
            {
                return -1;
            }
            return 1;
        }
    }

    //ソートするためのノード
    class sortnode<T>
        where T : class
    {
        /// <summary>
        /// ソート対象のオブジェクト
        /// </summary>
        public T o;
        /// <summary>
        /// ソートするための値
        /// </summary>
        public float v;
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="obj">ソートするオブジェクト</param>
        /// <param name="value">その値</param>
        public sortnode(T obj, float value)
        {
            o = obj;
            v = value;
        }

    };

    /// <summary>
    /// 任意の物体をソートするためのクラス(安定ソート)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class supersort<T>
        where T : class
    {
        /// <summary>
        /// ランダムに並べ替える
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        static public List<T> Random(List<T> list) 
        {
            var sort=new supersort<T>();
            foreach (var a in list) 
            {
                sort.add(a, 0);
            }
            sort.SameRandom();
            sort.sort(false);
            return sort.getresult();
        }

        List<sortnode<T>> list = new List<sortnode<T>>();
        /// <summary>
        /// ソートするものを追加する
        /// </summary>
        /// <param name="obj">物体</param>
        /// <param name="value">ソートするための値</param>
        public void add(T obj, float value)
        {
            list.Add(new sortnode<T>(obj, value));
        }
        /// <summary>
        /// 同じ値をランダムに並び変える(安定ソートなのでぐちゃぐちゃにするだけ)
        /// </summary>
        public void SameRandom() 
        {
            var sot = new supersort<sortnode<T>>();
            foreach (var a in list)
            {
                sot.add(a, FileMan.whrandhani(list.Count));
            }
            sot.sort(false);
            list = sot.getresult();
        }
        /// <summary>
        /// ソートを行う
        /// </summary>
        /// <param name="ookiijyunn">大きい順に並べるか</param>
        public void sort(bool ookiijyunn)
        {
            if (list.Count <= 0) return;
            List<List<sortnode<T>>> sorts = new List<List<sortnode<T>>>();
            int si = list.Count();
            int i, p, q;
            sortnode<T> temp;
            int tsi = 0;
            while (true)
            {
                sorts.Add(new List<sortnode<T>>());
                for (i = 0; i < 64; i++)
                {
                    if (tsi * 64 + i >= si)
                    {
                        break;
                    }
                    sorts[tsi].Add(list[tsi * 64 + i]);

                }

                if (tsi * 64 + i >= si)
                {

                    break;
                }
                tsi++;


            }

            for (i = 0; i <= tsi; i++)
            {

                for (p = 1; p < sorts[i].Count; p++)
                {

                    temp = sorts[i][p];
                    if (sorts[i][p - 1].v < temp.v ^ !ookiijyunn)
                    {
                        q = p;
                        do
                        {
                            sorts[i][q] = sorts[i][q - 1];
                            q--;
                        } while (q > 0 && (sorts[i][q - 1].v < temp.v ^ !ookiijyunn));
                        sorts[i][q] = temp;
                    }
                }

            }

            while (sorts.Count > 1)
            {
                p = 1;
                while (p < sorts.Count)
                {
                    i = 0; q = 0;
                    //マージ


                    while (i < sorts[p - 1].Count)
                    {
                        if (sorts[p - 1][i].v < sorts[p][q].v ^ !ookiijyunn)
                        {
                            sorts[p - 1].Insert(i, sorts[p][q]);
                            i++;
                            q++;
                        }
                        else
                        {
                            i++;
                        }
                        if (q >= sorts[p].Count) break;
                    }
                    while (q < sorts[p].Count)
                    {
                        sorts[p - 1].Add(sorts[p][q]);
                        q++;
                    }
                    //戦後処理
                    sorts.RemoveAt(p);
                    if (sorts.Count() - 1 == p)
                    {
                    }
                    else p++;
                }


            }
            list = sorts[0];
        }
        /// <summary>
        /// 物体を交換する
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void swap(int x, int y)
        {
            var temp = list[x];
            list[x] = list[y];
            list[y] = temp;
        }
        /// <summary>
        /// 物体を指定位置に挿入するように移動させる
        /// </summary>
        /// <param name="koitu">物体のインデックス</param>
        /// <param name="kokoni">挿入する位置</param>
        void conveyor(int koitu, int kokoni)
        {
            //	TRACE(_T("%d %d conv\n "), koitu,kokoni);
            var temp = list[koitu];
            if (koitu < kokoni)
            {
                for (int i = koitu; i < kokoni; i++)
                {
                    swap(i, i + 1);
                }
            }
            else
            {
                for (int i = koitu; i > kokoni; i--)
                {
                    swap(i, i - 1);
                }
            }
            list[kokoni] = temp;
        }
        /// <summary>
        /// ソートされたベクトルを受け取る
        /// </summary>
        /// <param name="back">逆にして受け取るか</param>
        /// <returns></returns>
        public List<T> getresult(bool back = false)
        {
            List<T> res = new List<T>();
            if (!back)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    res.Add(list[i].o);
                    //	TRACE(_T("%f "),sorts[i].v);
                }
            }
            else
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    res.Add(list[i].o);
                }
            }
            //TRACE(_T("DA\n"));
            return res;
        }
        public void Clear() 
        {
            list.Clear();
           
        }
    };
    /// <summary>
    /// タイマー
    /// </summary>
    public class SuperTimer
    {
        /// <summary>
        /// 最大の時間
        /// </summary>
        public float MaxTime = 0;
        /// <summary>
        /// 最小の時間
        /// </summary>
        public float MinTime = 0;
        /// <summary>
        /// 今の時間
        /// </summary>
        public float Timer = 0;

        /// <summary>
        /// タイマーを進める。Max,Minどちらかが0いかなら作動しない。
        /// </summary>
        /// <param name="clock"></param>
        /// <returns>何回タイマーが作動したか</returns>
        public int Update(float clock)
        {
            if (MaxTime > 0 && MinTime > 0)
            {
                Timer -= clock;
                int timercount = 0;
                while ((Timer > 0) == false)
                {
                    Timer += FileMan.whrandhani(MaxTime - MinTime) + MinTime;
                    timercount++;
                }
                return timercount;
            }
            return 0;
        }
        /// <summary>
        /// タイマーをリセットする
        /// </summary>
        public void Reset()
        {
            Timer = FileMan.whrandhani(MaxTime - MinTime) + MinTime;
        }

        public SuperTimer(float MinTime,float MaxTime) 
        {
            this.MinTime = MinTime;
            this.MaxTime = MaxTime;
            Reset();
        }
    }
}
