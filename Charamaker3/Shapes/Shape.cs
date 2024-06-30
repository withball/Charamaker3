using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charamaker3;
namespace Charamaker3.Shapes
{
    /// <summary>
    ///	接触したライン等表すクラス
    /// </summary>
    public class lineX
    {
        public FXY begin, end, bs;

        public float length { get { return (end - begin).length; } }

        public float degree { get { return (end - begin).degree; } }
        public float hosen
        {
            get
            {

                float deg = (end - begin).degree;
                float deg2 = (bs - begin).degree;

                if (Mathf.st180(deg2 - deg) < 0)
                {
                    return Mathf.st180(deg + 90);
                }
                else
                {
                    return Mathf.st180(deg - 90);

                }
                return deg;
            }
        }
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="a">始まりの点</param>
        /// <param name="b">終わりの点</param>
        /// <param name="kijyun">辺の内側の中心点</param>
        public lineX(FXY a, FXY b, FXY kijyun)
        {
            begin = a;
            end = b;
            bs = kijyun;
        }/// <summary>
         /// 普通のコンストラクタ
         /// </summary>
         /// <param name="bx">開始点x</param>
         /// <param name="by">開始点y</param>
         /// <param name="ex">終了店x</param>
         /// <param name="ey">終了店y</param>
         /// <param name="kx">基準点(内側の点)x</param>
         /// <param name="ky">基準点y</param>
        public lineX(float bx, float by, float ex, float ey, float kx, float ky)
        {
            begin = new FXY(bx, by);
            end = new FXY(ex, ey);
            bs = new FXY(kx, ky);
        }

        /// <summary>
        /// 線の中間地点
        /// </summary>
        public FXY center
        {
            get { return (begin + end) / 2; }
        }

        /// <summary>
        /// その点が辺の内側にあるか検知する
        /// </summary>
        /// <param name="poi">その点</param>
        /// <returns>内側であればtrue</returns>
        public bool onInside(FXY poi)
        {
            float r = degree;
            float kij = (bs - begin).degree;
            float p = (poi - begin).degree;
            kij = Mathf.st180(kij - r);
            p = Mathf.st180(p - r);
            return (kij >= 0) == (p >= 0);
        }
        /// <summary>
        /// 点と直線の距離を求めてくれる
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <returns>.lengthでちゃんとした距離</returns>
        public FXY getkyori(float px, float py)
        {
            var res = new FXY(0, 0);
            float degree = -this.degree;
            float degree2 = -this.hosen;

            //TRACE(_T("rad:::%f  hos %f\n"), rad/M_PI*180, rad2 / M_PI * 180);
            float a1 = (float)Mathf.sin(degree), b1 = (float)Mathf.cos(degree), c1 = -this.begin.x, d1 = -this.begin.y;

            float a2 = (float)Mathf.sin(degree2), b2 = (float)Mathf.cos(degree), c2 = -px, d2 = -py;


            if (a1 == 0)
            {
                res.x = -c2;
                res.y = -d1;
            }
            else if (b1 == 0)
            {
                res.x = -c1;
                res.y = -d2;
            }
            else
            {
                res.x = (b1 * (a2 * c2 + d2 * b2) - b2 * (a1 * c1 + d1 * b1)) / (a1 * b2 - a2 * b1);
                res.y = (a2 * (a1 * c1 + d1 * b1) - a1 * (a2 * c2 + d2 * b2)) / (a1 * b2 - a2 * b1);
            }
            return res;
        }
        override public string ToString()
        {
            return begin.ToString() + " :begin end: " + end.ToString() + " ->"
                + degree + " :rad hos: " + hosen ;
        }


        public bool crosses(lineX l)
        {
            double psx = this.begin.x, psy = this.begin.y, pex = this.end.x, pey = this.end.y
                , qsx = l.begin.x, qsy = l.begin.y, qex = l.end.x, qey = l.end.y;
            double c1 = (pex - psx) * (qsy - psy) - (pey - psy) * (qsx - psx);

            double c2 = (pex - psx) * (qey - psy) - (pey - psy) * (qex - psx);

            double c3 = (qex - qsx) * (psy - qsy) - (qey - qsy) * (psx - qsx);

            double c4 = (qex - qsx) * (pey - qsy) - (qey - qsy) * (pex - qsx);

            if (c1 * c2 < 0 && c3 * c4 < 0)
            {


                return true;
            }
            return false;
        }

        public static bool operator ==(lineX a, lineX b)
        {
            bool anul = a is null;
            bool bnul = b is null;
            if (anul && bnul) return true;
            if (anul || bnul) return false;
            return a.begin == b.begin && a.end == b.end && a.bs == b.bs;
        }
        public static bool operator !=(lineX a, lineX b)
        {

            bool anul = a is null;
            bool bnul = b is null;
            if (anul && bnul) return false;
            if (anul || bnul) return true;
            return a.begin != b.begin || a.end != b.end || a.bs != b.bs;
        }
    };

    /// <summary>
    /// 図形の基底クラス
    /// </summary>
    public class Shape
    {
        #region statics
        static public T ToLoadShape<T>(DataSaver d)
         where T : Shape
        {
            var type = Type.GetType(d.unpackDataS("type"));
            if (typeof(T) == type)
            {
                var res = (T)Activator.CreateInstance(type);
                res.ToLoad(d);
                return res;
            }
            Debug.WriteLine(type + " != " + typeof(T) + " 指定されたタイプじゃないので死にました。");
            return null;
        }
        static public Shape ToLoadShape(DataSaver d)
        {

            var type = Type.GetType(d.unpackDataS("type"));
            if (type == null)
            {
                Debug.WriteLine("Shapeじゃないよ！ちゃんと読み込めなかったよ！");

                return new Shape();
            }
            var res = (Shape)Activator.CreateInstance(type);
            res.ToLoad(d);

            return res;
        }
        /// <summary>
        /// 図形二つの重なり。両側からやるよ
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool atarun(Shape a, Shape b)
        {
            return a.kasanari(b);
        }
        /// <summary>
        /// 図形2つとそれぞれの位置フレーム前での重なり
        /// </summary>
        /// <returns></returns>
        public static bool atarun(Shape a, Shape pa, Shape b, Shape pb)
        {
            //  var t = Task.Run(() => { return gousei(a, pa); });
            //  var t2 = Task.Run(() => { return gousei(b, pb); });

            //  t.Wait();
            // t2.Wait();

            //  return atarun(t.Result, t2.Result);
            return atarun(gousei(a, pa), gousei(b, pb));
        }

        /// <summary>
        /// 二つの図形を合成する。結果は最大の面積を持つ図形になる
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Shape gousei(Shape a, Shape b)
        {
            var lis = a.getzettaipoints();
            lis.AddRange(b.getzettaipoints());

            var res = new List<FXY>();
            FXY tm = lis.First(), pm = lis.Last();

            for (int t = 0; t < lis.Count; t++)
            {
                if (tm.x < lis[t].x)
                {
                    tm = lis[t];
                }
                if (pm.x > lis[t].x)
                {
                    pm = lis[t];
                }
            }
            res.Add(pm);
            FXY now = pm;
            float kaku = (now - tm).degree;
            do
            {
                var removes = new List<FXY>();
                //     Console.WriteLine(now.X+" noww "+now.Y+" lis.count "+lis.Count);
                tm = lis.First();
                for (int i = lis.Count - 1; i >= 0; i--)
                {
                    //今の点から注目点への角度
                    float rad = Mathf.st360((lis[i] - now).degree - kaku);
                    if (lis[i] != now)
                    {
                        if (Mathf.st360((tm - now).degree - kaku) > rad || tm == now)
                        {
                            tm = lis[i];
                            removes.Clear();
                        }
                        else if (Mathf.st360((tm - now).degree - kaku) == rad)
                        {
                            if ((tm - now).length < (lis[i] - now).length)
                            {
                                var tmp = tm;
                                tm = lis[i];

                                removes.Add(tmp);
                            }
                            else
                            {
                                removes.Add(lis[i]);
                            }
                        }
                    }
                }
                kaku = (tm - now).degree;

                now = tm;
                res.Add(now);
                lis.Remove(now);
                foreach (var c in removes) lis.Remove(c);
                // Console.WriteLine(now.X + " denden  " + now.Y + " lis.count " + lis.Count);
            } while (now != pm && lis.Count > 0);
            if (now == pm) res.RemoveAt(res.Count - 1);

            /*  foreach (var c in res) 
              {
                  Console.WriteLine(c.X + " XY " + c.Y);
              }
              Console.WriteLine("da");
            */
            FXY max = new FXY(res.First()), min = new FXY(res.First());
            for (int i = 1; i < res.Count; i++)
            {
                if (max.x < res[i].x) max.x = res[i].x;
                if (max.y < res[i].y) max.y = res[i].y;
                if (min.x > res[i].x) min.x = res[i].x;
                if (min.y > res[i].y) min.y = res[i].y;
            }
            float www = max.x - min.x, hhh = max.y - min.y;

            foreach (var c in res)
            {
                if (www > 0) c.x = (c.x - min.x) / www;
                else c.x = 0;
                if (hhh > 0) c.y = (c.y - min.y) / hhh;
                else c.y = 0;
            }

            return new Shape(min.x, min.y, www, hhh, 0, res);
        }

       
        /// <summary>
        /// 線がクロスしているのかを判定する
        /// </summary>
        /// <param name="mom">{x1,y1,x2,y2}</param>
        /// <param name="mom2">{x1,y1,x2,y2}</param>
        /// <returns></returns>
        static protected bool crosses(double[] mom, double[] mom2)
        {
            return crosses(mom[0], mom[1], mom[2], mom[3], mom2[0], mom2[1], mom2[2], mom2[3]);
        }
        /// <summary>
        /// 線がクロスしているのかを判定する
        /// </summary>
        /// <returns></returns>
        static protected bool crosses(FXY ps, FXY pe, FXY qs, FXY qe)
        {
            return crosses(ps.x, ps.y, pe.x, pe.y, qs.x, qs.y, qe.x, qe.y);
        }
        /// <summary>
        /// 線がクロスしているのかを判定する
        /// </summary>
        /// <returns></returns>
        static protected bool crosses(double psx, double psy, double pex, double pey, double qsx, double qsy, double qex, double qey)
        {

            double c1 = (pex - psx) * (qsy - psy) - (pey - psy) * (qsx - psx);

            double c2 = (pex - psx) * (qey - psy) - (pey - psy) * (qex - psx);

            double c3 = (qex - qsx) * (psy - qsy) - (qey - qsy) * (psx - qsx);

            double c4 = (qex - qsx) * (pey - qsy) - (qey - qsy) * (pex - qsx);

            if (c1 * c2 < 0 && c3 * c4 < 0)
            {


                return true;
            }
            return false;
        }
        static float nasukaku(float ax, float ay, float bx, float by)
        {
            return Mathf.atan(by - ay, bx - ax);
        }
        static float kyori(float ax, float ay, float bx, float by)
        {
            return Mathf.sqrt(Mathf.pow(ax - bx, 2) + Mathf.pow(ay - by, 2));
        }
        /// <summary>
        /// 図形内部の点を割り出す
        /// </summary>
        /// <param name="kaku">中心からの角度degree</param>
        /// <param name="nagwari">長さの割合</param>
        /// <param name="kaiten">trueならkakuが図形の回転型されたものになる</param>
        /// <returns></returns>
        public virtual FXY getinnerpoint(float kaku, float nagwari, bool kaiten = false)
        {
            var c = getCenter();
            var lis = getzettaipoints(false);
            int idx = 1;
            for (int i = 1; i < lis.Count - 1; i++)
            {
                if (Mathf.st180((lis[i - 1] - c).degree - kaku) * Mathf.st180((lis[i] - c).degree - kaku) <= 0)
                {
                    {
                        idx = i;
                        break;
                    }
                }
            }
            var L = lis[idx - 1] - c;
            var R = lis[idx] - c;
            float n = 0;
            var soi = (new FXY(L.length, kaku) - L);

            if (R.x != 0)
            {
                n = Mathf.max(Mathf.abs(soi.x / R.x), n);
            }
            if (R.y != 0)
            {
                n = Mathf.max(Mathf.abs(soi.y / R.y), n);
            }
            var Lwari = 1 / (n + 1);
            var res = L * Lwari * nagwari + R * (1 - Lwari) * nagwari;
            if (kaiten) res.degree -= degree;
            return res + c;
        }

        /// <summary>
        /// 点を線と重なるようにずらす幅を教えてくれる。どうしても重ならない場合は(0,0)
        /// </summary>
        /// <param name="point"></param>
        /// <param name="l"></param>
        /// <param name="yokomugen">線が無限の長さだと解釈する</param>
        /// <param name="outer">外側の点だとしてもずらす</param>
        /// <returns></returns>
        public static FXY getzurasi(FXY point, lineX l, bool yokomugen = false, bool outer = false)
        {
            //	TRACE(_T("%f::%f ||||%f %f:idoukaku:%f %f  %f\n"),point.x,point.y, l.begin.x, l.begin.y, l.end.x, l.end.y, l.getrad());
            /* Console.WriteLine(point.X + " point " + point.Y);
             Console.WriteLine(l.begin.X + " lineB " + l.begin.Y);
             Console.WriteLine(l.end.X + " lineE " + l.end.Y);
             Console.WriteLine(l.bs.X + " lineK " + l.bs.Y);
          */
            if (!l.onInside(point) && !outer)
            {
                float r = l.degree;
                float kij = (l.bs - l.begin).degree;
                float p = (point - l.begin).degree;
                kij = Mathf.st180(kij - r);
                p = Mathf.st180(p - r);
                //    Console.WriteLine(kij+ "::ohnooQQ::"+p+"  :->: "+point.ToString());
                return new FXY(float.NaN, float.NaN); ;
            }
            var res = l.getkyori(point.x, point.y);

            // Console.WriteLine(res.X + " tentyoku " + res.Y);

            float bigx = Mathf.max(l.begin.x, l.end.x);
            float minx = Mathf.min(l.begin.x, l.end.x);
            float bigy = Mathf.max(l.begin.y, l.end.y);
            float miny = Mathf.min(l.begin.y, l.end.y);

            if (yokomugen ||
                ((minx <= res.x && res.x <= bigx) && (miny <= res.y && res.y <= bigy)
               ))
            {
                /* double r = l.rad;
                 double kij = (l.bs - l.begin).rad;
                 double p = (point - l.begin).rad;
                 kij = Shape.radseiki(kij - r);
                 p = Shape.radseiki(p - r);
                 Console.WriteLine(kij + "::OKKKKKKQQ::" + p + "  :->: " + point.ToString());

                 Console.WriteLine(minx + "::OKKKKKKQQ::" + bigx + " ->  " + miny + " :: " + bigy + " <- " + res.ToString());
                 Console.WriteLine("ZZZZ " + (minx <= res.X && res.X <= bigx) + " :: " + (miny <= res.Y && res.Y <= bigy));
                */

                res.x -= point.x;
                res.y -= point.y;


                return res;
            }
            // Console.WriteLine("YkosugiTA");
            //TRACE(_T("ohno:: %f %f\n"), res.x, res.y);
            return new FXY(float.NaN, 0);


        }
        /// <summary>
        /// 図形を合成する
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Shape operator +(Shape a, Shape b)
        {
            return gousei(a, b);
        }
        #endregion
        #region define
        /// <summary>
        /// 図形の基本要素
        /// </summary>
        public float x=0, y=0, w=0, h=0;
        /// <summary>
        /// カクード
        /// </summary>
        protected float _degree;
        /// <summary>
        /// セットする際は-Pi＜＝degree＜＝Piの範囲にする。そして重心で回転させる
        /// </summary>
        public float degree
        {
            get { return _degree; }
            set
            {
                var  xy = gettxy();
                _degree = value;
                settxy(xy);
                _degree = Mathf.st180(_degree);
            }
        }
        /// <summary>
        /// 図形の頂点の相対座標ども
        /// </summary>
        List<FXY> points=new List<FXY>();



        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="xx">ｘ座標</param>
        /// <param name="yy">ｙ座標</param>
        /// <param name="ww">幅</param>
        /// <param name="hh">高さ</param>
        /// <param name="radd">角度</param>
        /// <param name="points">図形の頂点の相対座標(w,hに対する比)ども(0~1)</param>
        public Shape(float xx, float yy, float ww, float hh, float radd, List<FXY> points)
        {

            x = xx;
            y = yy;
            w = ww;
            h = hh;
            _degree = Mathf.st180(radd);


            setpoints(points);

        }
        /// <summary>
        /// からのコンストラクタ。セーブとか保存用
        /// </summary>
        public Shape() { }
        public Shape clone()
        {
            var res = (Shape)Activator.CreateInstance(this.GetType());
            this.copy(res);
            return res;

        }
       /// <summary>
       /// 図形の情報をコピーする
       /// </summary>
       /// <param name="s">コピー先の図形(同じ型であること前提)</param>
       /// <returns></returns>
        virtual public void copy(Shape s) 
        {
            
            s.x = this.x;
            s.y = this.y;
            s.w = this.w;
            s.h = this.h;
            s._degree = this.degree;
            s.points = this.clonepoints();
        }
        /// <summary>
        /// 図形をセーブできるようにする
        /// </summary>
        /// <returns></returns>
        public virtual DataSaver ToSave() 
        {
            var res = new DataSaver();
            res.packAdd("type", this.GetType().ToString());
            res.packAdd("x", x);
            res.packAdd("y", y);
            res.linechange();
            res.packAdd("w", w);
            res.packAdd("h", h);
            res.packAdd("degree", degree);
            res.linechange();
            var d = new DataSaver();
            for (int i = 0; i < points.Count; i++) 
            {
                var pd = new DataSaver();
                pd.packAdd("x", points[i].x);
                pd.packAdd("y", points[i].y);
                d.packAdd(i.ToString(), pd);
            }
            d.indent();
            res.packAdd("points",d);
            res.linechange();
            return res;
        }
        /// <summary>
        /// 図形の情報をデータセーバーからロードしてくる
        /// </summary>
        /// <param name="d"></param>
        public virtual void ToLoad(DataSaver d) 
        {

            this.x = d.unpackDataF("x");
            this.y = d.unpackDataF("y");
            this.w = d.unpackDataF("w");
            this.h = d.unpackDataF("h");
            this._degree = d.unpackDataF("degree");
            var points = d.unpackDataD("points");
            foreach (var p in points.allUnpackDataD()) 
            {
                this.points.Add(new FXY(p.unpackDataF("x"), p.unpackDataF("y")));
            }
        }

        /// <summary>
        /// 頂点を複製する
        /// </summary>
        /// <returns></returns>
        protected List<FXY> clonepoints()
        {
            var res = new List<FXY>();
            if (points.Count > 0)
            {
                //Debug.WriteLine(points.Count+" A");
                for (int i = 1; i < points.Count - 1; i++)
                {
                    res.Add(new FXY(points[i]));
                }

                res.Add(points[0]);

                res.Insert(0, points[points.Count - 2]);
            }
            return res;
        }


        /// <summary>
        /// 絶対座標に直した各頂点を取得する
        /// </summary>
        /// <param name="kaburinasi">被らない範囲のやつだけ</param>
        /// <returns></returns>
        public List<FXY> getzettaipoints(bool kaburinasi = true)
        {
            List<FXY> res = new List<FXY>();

            if (kaburinasi)
            {
                for (int i = 1; i < points.Count - 1; i++)
                {
                    res.Add(new FXY(x + (float)Mathf.cos(degree) * points[i].x * w - (float)Mathf.sin(degree) * points[i].y * h
                        , y + (float)Mathf.sin(degree) * points[i].x * w + (float)Mathf.cos(degree) * points[i].y * h));
                }
            }
            else
            {
                for (int i = 0; i < points.Count; i++)
                {
                    res.Add(new FXY(x + (float)Mathf.cos(degree) * points[i].x * w - (float)Mathf.sin(degree) * points[i].y * h
                      , y + (float)Mathf.sin(degree) * points[i].x * w + (float)Mathf.cos(degree) * points[i].y * h));

                }


            }
            return res;
        }

        /// <summary>
        /// 各頂点の相対座標を取得する
        /// </summary>
        /// <param name="kaburinasi">被らない範囲のやつだけ</param>
        /// <returns></returns>
        public List<FXY> getSoutaiPoints(bool kaburinasi = true)
        {
            List<FXY> res = new List<FXY>();

            if (kaburinasi)
            {
                for (int i = 1; i < points.Count - 1; i++)
                {
                    res.Add(new FXY(points[i].x * w, points[i].y * h));
                }
            }
            else
            {
                for (int i = 0; i < points.Count; i++)
                {
                    res.Add(new FXY(points[i].x * w, points[i].y * h));
                }
            }
            return res;
        }
        /// <summary>
        /// 図形が当たっているか
        /// </summary>
        /// <param name="p">1フレーム前の自分</param>
        /// <param name="e">相手</param>
        /// <param name="pe">1フレーム前の相手</param>
        /// <returns></returns>
        public bool atarun2(Shape p, Shape e, Shape pe)
        {
            var a=this + p;
            var b = e + pe;

            return atarun(this, p, e, pe);
        }/// <summary>
         /// 図形が当たっているか
         /// </summary>
         /// <param name="e"></param>
        public bool atarun(Shape e)
        {
            return atarun(this, e);
        }
        /// <summary>
        /// 図形の外周の長さを返す
        /// </summary>
        /// <returns></returns>
        public float gaisyuu
        {
            get
            {
                float sum = 0;
                for (int i = 1; i < points.Count - 1; i++)
                {

                    sum += Mathf.sqrt(Mathf.pow(points[i + 1].y - points[i].y, 2) + Mathf.pow(points[i + 1].x - points[i].x, 2));
                }
                return sum;
            }
        }
        /// <summary>
        /// 図形の面積を返す
        /// </summary>
        /// <returns></returns>
        public float menseki
        {
            get
            {
                var c = getSoutaiCenter();
                var points = getSoutaiPoints(false);
                float sum = 0;
                for (int i = 1; i < points.Count - 1; i++)
                {
                    var k = Mathf.st180(nasukaku(points[i].x, points[i].y, c.x, c.y)
                        - nasukaku(points[i].x, points[i].y, points[i + 1].x, points[i + 1].y));
                    var kyo = kyori(points[i].x, points[i].y, c.x, c.y);
                    //TRACE(_T("%f aklfoaii\n"), fabs(kyo * kyo * sinf(k) * cosf(k)));
                    sum += Mathf.abs(kyo * kyo * Mathf.sin(k) * Mathf.cos(k));

                }
                //TRACE(_T("%f OMOSA\n"),sum);
                return sum;
            }
        }
        /// <summary>
        /// 頂点の座標をセットする
        /// </summary>
        /// <param name="points"></param>
        protected void setpoints(List<FXY> points)
        {

            this.points = new List<FXY>();
            if (points.Count > 0)
            {
                for (int i = 0; i < points.Count; i++)
                {
                    this.points.Add(new FXY(points[i]));
                }
                this.points.Add(points[0]);

                this.points.Insert(0, points[points.Count - 1]);
            }
        }
        #endregion

        /// <summary>
        /// 図形をいい感じに反転させる(方向を決める)メソッド大体のやつは左右等しいので意味ないけど
        /// </summary>
        /// <param name="mir">1で右向き、-1で左向きに0で普通の反転</param>
        public virtual void setMirror(int mir)
        {

        }

        /// <summary>
        /// 図形上の一点のx座標を取得する(回転の影響を考慮してるってこと)
        /// </summary>
        /// <param name="ww">左上を0としたときの図形の点の位置,NaNで重心</param>
        /// <param name="hh">左上を0としたときの図形の点の位置,NaNで重心</param>
        /// <returns>返されるのは座標</returns>
        public FXY gettxy(float ww = float.NaN, float hh = float.NaN)
        {
            float rx = 0, ry = 0; ;
            bool xxx = float.IsNaN(ww),yyy= float.IsNaN(hh);
            if (!xxx)
            {
                rx = x + ww * Mathf.cos(degree) - hh * Mathf.sin(degree);
            }
            if (!yyy)
            {

                ry = y + ww * Mathf.sin(degree) + hh * Mathf.cos(degree);
            }


            if (xxx || yyy) 
            {
                var c = getCenter();
                if (xxx) rx = c.x;
                if (yyy) ry = c.y;

            }
         
            return new FXY(rx, ry);
        }
      
        /// <summary>
        /// 中心の絶対座標を求める
        /// </summary>
        /// <returns></returns>
        public FXY getCenter()
        {
            var points = getzettaipoints();
            FXY res = new FXY(0, 0);
            for (int i = 0; i < points.Count; i++)
            {

                res.x += points[i].x;
                res.y += points[i].y;
            }
            res.x = res.x / points.Count;
            res.y = res.y / points.Count;

            return res;
        }
        /// <summary>
        /// 中心の相対座標を求める
        /// </summary>
        /// <returns></returns>
        public FXY getSoutaiCenter()
        {
            FXY res = new FXY(0, 0);
            for (int i = 1; i < points.Count - 1; i++)
            {

                res.x += points[i].x;
                res.y += points[i].y;
            }

            res.x = res.x / points.Count * w;
            res.y = res.y / points.Count * h;

            return res;
        }
        
        /// <summary>
        /// 重心をxy座標にセットする。
        /// </summary>
        /// <param name="xx">セットするx座標</param>
        /// <param name="yy">セットするy座標</param>
        virtual public void settxy(float xx, float yy)
        {
            var c = gettxy();
            x += xx - c.x;
            y += yy - c.y;

        }
        public void settxy(FXY xy) 
        {
            settxy(xy.x, xy.y);
        }
        /// <summary>
        /// 図形上の任意の一点をxy座標にセットする
        /// </summary>
        /// <param name="xx">セットするx座標</param>
        /// <param name="yy">セットするy座標</param>
        /// <param name="cw">図形上のwの点,Nanで重心</param>
        /// <param name="ch">図形上のhの点,Nanで重心</param>
        public void settxy(float xx, float yy, float cw=float.NaN, float ch=float.NaN)
        {
            bool xxx = float.IsNaN(cw), yyy = float.IsNaN(ch);

            if (!xxx)x = xx - cw * (float)Mathf.cos(degree) + ch * (float)Mathf.sin(degree);
            if(!yyy)y = yy - cw * (float)Mathf.sin(degree) - ch * (float)Mathf.cos(degree);

            if (xxx || yyy)
            {
                var c = gettxy();
                if (xxx) x += xx - c.x;
                if (yyy) y += yy - c.y;
            }
        }
        /// <summary>
        /// 任意の点で図形の角度をセットする
        /// </summary>
        /// <param name="tdegree">回転角</param>
        /// <param name="cw">図形上のwの点,Nanで重心</param>
        /// <param name="ch">図形上のhの点,Nanで重心</param>
        public void setradcxy(float tdegree, float cw=float.NaN, float ch=float.NaN)
        {
            tdegree = Mathf.st180(tdegree);
            FXY xy = gettxy(cw, ch);
            degree = tdegree;
            settxy(x, y, cw, ch);
        }

        /// <summary>
        /// 図形をピクチャーにかぶせるっていうかセットする
        /// </summary>
        /// <param name="p"></param>
        public void setto(Entity p)
        {
            w = p.w;
            h = p.h;
            _degree = p.degree;
            x = p.x;
            y = p.y;
        }
        /// <summary>
        /// 中心の座標を変えずにサイズを拡大縮小する
        /// </summary>
        /// <param name="sc">スケール</param>
        public void scale(float sc)
        {
            FXY xy = gettxy();;
            w = this.w * sc;
            h = this.h * sc;

            settxy(xy);
        }
        /*
        /// <summary>
        /// 図形を描画する
        /// </summary>
        /// <param name="hyojiman">描画するhyojiman</param>
        /// <param name="R">線の色</param>
        /// <param name="G">線の色</param>
        /// <param name="B">線の色</param>
        /// <param name="A">線の不透明度</param>
        /// <param name="hutosa">線のふとさ</param>
        /// <param name="begin">hyojimanのbegindrawをついでにするか(すると重くなるので外部でやるのがおススメ)</param>
        public void drawshape(hyojiman hyojiman, float R, float G, float B, float A, float hutosa = 3, bool begin = false)
        {
            if (begin)
            {
                hyojiman.render.BeginDraw();
            }
            drawn(new Color4(R, G, B, A), hutosa, hyojiman);
            if (begin)
            {
                hyojiman.render.EndDraw();
            }

        }*/
        /*
        /// <summary>
        /// 図形それぞれの描画の部分の本体
        /// </summary>
        /// <param name="col">カラー</param>
        /// <param name="hyo">表示する画面</param>
        /// <param name="hutosa">線の太さ</param>
        protected void drawn(Color4 col, float hutosa, hyojiman hyo)
        {
            var lis = getzettaipoints(false);

            for (int i = 1; i < lis.Count - 1; i++)
            {
                hyo.drawLine(lis[i - 1].X, lis[i - 1].Y, lis[i].X, lis[i].Y, col, hutosa);
            }
        }*/
        /// <summary>
        /// その図形が図形内にあるかのどうかの判定。両側からやってくれる
        /// </summary>
        /// <param name="s">その図形</param>
        /// <param name="onis">1.0001とかでちょっとアバウトに判定してくれる</param>
        /// <returns>あるか</returns>
        public bool onhani(Shape s, float onis = 1)
        {
            foreach (var a in this.getzettaipoints())
            {
                if (s.onhani(a.x, a.y, onis)) return true;
            }
            foreach (var a in s.getzettaipoints())
            {
                if (this.onhani(a.x, a.y, onis)) return true;
            }
            return false;
        }

        /// <summary>
        /// その点が図形内にあるかのどうかの判定
        /// </summary>
        /// <param name="px">その点のx座標</param>
        /// <param name="py">その点のy座標</param>
        /// <param name="onis">1.0001とかでちょっとアバウトに判定してくれる</param>
        /// <returns>あるか</returns>
        virtual public bool onhani(float px, float py, float onis = 1)
        {
            FXY myCenter = gettxy();
            float XXX = px - myCenter.x;
            float YYY = py - myCenter.y;
            var points = getzettaipoints(false);
            //TRACE(_T("%f :SOY: %f\n"), x, y);
            for (int i = 1; i < points.Count - 1; i++)
            {
                float x1 = points[i].x - myCenter.x;
                float y1 = points[i].y - myCenter.y;
                float x2 = points[i + 1].x - myCenter.x;
                float y2 = points[i + 1].y - myCenter.y;
                //Console.WriteLine(x1 + "::" + y1 + " これがオン範囲のあれや！ " + x2 + " :: " + y2);

                float t, s;
                if (y1 == 0)
                {
                    s = YYY / y2;
                    t = (XXX - x2 * s) / x1;
                }
                else
                {
                    s = (XXX - YYY * x1 / y1) / (x2 - y2 * x1 / y1);
                    t = (YYY - y2 * s) / y1;
                }

                //Console.WriteLine(s+" +++ "+t+" = "+(s+t));

                if (s >= 0 && t >= 0 && s + t <= onis)
                {
                    // Console.WriteLine(s + " :onEQQEQEWQEQEhani: " + t);
                    // Console.WriteLine(points[i].X+" :onEQQEQEWQEQEhani: "+ points[i].Y);
                    return true;
                }
                else
                {
                    //TRACE(_T("%f :onhani: %f\n"), points[i].x, points[i].y);
                }

            }
            return false;
        }
        /// <summary>
        /// その線が図形と接触するかどうかの判定
        /// </summary>
        /// <param name="px">その点のx座標1</param>
        /// <param name="py">その点のy座標1</param>
        /// <param name="ppx">その点のx座標2</param>
        /// <param name="ppy">その点のy座標2</param>
        /// <returns></returns>
        virtual public bool onhani(float px, float py, float ppx, float ppy)
        {
            FXY sp = new FXY(px - x, py - y);
            FXY spp = new FXY(ppx - x, ppy - y);

            sp.degree -= degree;
            spp.degree -= degree;

            bool Lsp = sp.x < 0, Rsp = sp.x > w;
            bool Usp = sp.y < 0, Dsp = sp.y > h;

            bool Lspp = spp.x < 0, Rspp = spp.x > w;
            bool Uspp = spp.y < 0, Dspp = spp.y > h;

            var lis = getzettaipoints(false);

            if ((Lsp && Lspp) || (Rsp && Rspp) || (Usp && Uspp) || (Dsp && Dspp))
            { return false; }



            if (onhani(px, py) || onhani(ppx, ppy)) return true;

            for (int i = 1; i < lis.Count - 1; i++)
            {
                var mom1 = new double[] { lis[i].x, lis[i].y, lis[i + 1].x, lis[i + 1].y };
                var mom2 = new double[] { px, py, ppx, ppy };
                if (Shape.crosses(mom1, mom2)) return true;
            }
            return false;
        }
        /// <summary>
        /// 図形の外周の辺のリストを手に入れる。引数無しで重心を中心とした相対座標で出してくれる
        /// </summary>
        /// <param name="x">中心座標x</param>>
        /// <param name="y">中心座標y</param>>
        /// <param name="rad">重心を中心とした回転角</param>>
        /// <returns>外周のリスト(相対座標){{x1,y1,x2,y2},{x1,y1,x2,y2}}</returns>
        protected List<double[]> getgaisyuus(float x = 0, float y = 0, double rad = 0)
        {
            var res = new List<double[]>();

            FXY sc = getSoutaiCenter();

            for (int i = 1; i < points.Count - 1; i++)
            {
                FXY one, two;
                one = new FXY(points[i].x * w, points[i].y * h);
                two = new FXY(points[i + 1].x * w, points[i + 1].y * h);

                one = one - sc;
                two = two - sc;

                one.degree += degree;
                two.degree += degree;

                res.Add(new double[] { one.x + x, one.y + y, two.x + x, two.y + y });

            }
            return res;
        }


        /// <summary>
        /// 外周を絶対座標にして返す
        /// </summary>
        /// <returns>外周のリスト(絶対座標)</returns>
        protected List<double[]> getgaisyuus2()
        {
            var lis = getgaisyuus();
            double a, b, c, d;
            for (int i = 0; i < lis.Count; i++)
            {

                a = lis[i][0] * Mathf.cos(degree) - lis[i][1] * Mathf.sin(degree);
                b = lis[i][0] * Mathf.sin(degree) + lis[i][1] * Mathf.cos(degree);

                c = lis[i][2] * Mathf.cos(degree) - lis[i][3] * Mathf.sin(degree);
                d = lis[i][2] * Mathf.sin(degree) + lis[i][3] * Mathf.cos(degree);
                var fxy = gettxy();
                lis[i][0] = a + fxy.x;
                lis[i][1] = b + fxy.y;
                lis[i][2] = c + fxy.x;
                lis[i][3] = d + fxy.y;
            }
            return lis;
        }
        
        /// <summary>
        /// 図形の重心からの最大の距離
        /// </summary>
        /// <returns>その射程</returns>
        public float syatei()
        {
            float max = 0;
            var lis = getSoutaiPoints();
            var center = getSoutaiCenter();
            foreach (var a in lis)
            {
                max = Mathf.max(max, Mathf.abs((a - center).length));
            }
            return max;
        }

        /// <summary>
        ///図形と図形の重心の距離を測る
        /// </summary>
        /// <param name="s">その図形の片割れ</param>
        /// <returns>距離</returns>
        public float kyori(Shape s)
        {
            return (s.gettxy() - gettxy()).length;
        }

        /// <summary>
        /// 図形の重心とある座標の距離を測る
        /// </summary>
        /// <param name="px">そのx座標</param>
        /// <param name="py">そのy座標</param>
        /// <returns>距離</returns>
        public float kyori(float px, float py)
        {
            var fxy = gettxy();
            fxy.x -= px;fxy.y -= py;

            return fxy.length;
        }
        /// <summary>
        /// 図形と図形の重心の紡ぐ線の角度を計る
        /// </summary>
        /// <param name="s">その図形</param>
        /// <returns>角度</returns>
        public float nasukaku(Shape s)
        {
            return (s.gettxy()-this.gettxy()).degree;
        }
        /// <summary>
        /// 図形の重心とある座標の紡ぐ線の角度を測る
        /// </summary>
        /// <param name="px">そのx座標</param>
        /// <param name="py">そのy座標</param>
        /// <returns>角度</returns>
        public float nasukaku(float px, float py)
        {
            var fxy = gettxy();
            fxy.x = px - fxy.x;
            fxy.y = py - fxy.y;
            return fxy.degree;
        }
        /// <summary>
        /// 接触していないことを判定する。接触していること判定できない
        /// </summary>
        /// <param name="s"></param>
        /// <returns>絶対に接触していないか</returns>
        public bool atattenai(Shape s)
        {
            if (kyori(s) > syatei() + s.syatei()) return true;
            return false;
        }


        /// <summary>
        /// 図形同士が重なっているか調べる。両側からやる
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public bool kasanari(Shape s)
        {
            /*
            var no = Task.Run(() => { return atattenai(s); });

            var ons = new List<Task<bool>>();
            ons.Add(Task.Run(() => { return onhani(s); }));

            var points = getzettaipoints(false);
            var spoints = s.getzettaipoints(false);
            for (int i = 1; i < points.Count - 1; i++)
            {

                ons.Add(Task.Run(() => { return s.onhani(points[i - 1].X, points[i - 1].Y, points[i].X, points[i].Y); }));

            }
            no.Wait();
            if (no.Result) return false;
            while (ons.Count > 0) 
            {
                for (int i = ons.Count - 1; i >= 0; i--) 
                {
                    if (ons[i].IsCompleted) 
                    {
                        if (ons[i].Result) 
                        {
                            return true;
                        }
                        ons.RemoveAt(i);
                    }
                }
            }
            return false;*/
            if (atattenai(s))
            {
                //Console.WriteLine("当たるわけねーだろ！");
                return false;
            }
            var points = getzettaipoints(false);
            var spoints = s.getzettaipoints(false);
            if (onhani(s))
            {
                return true;
            }
            for (int i = 1; i < points.Count - 1; i++)
            {

                if (s.onhani(points[i - 1].x, points[i - 1].y, points[i].x, points[i].y))
                {
                    return true;
                }

            }
            return false;
        }

        /// <summary>
        /// 指定した点に最もふさわしい辺を見つける
        /// </summary>
        public lineX getnearestline(FXY Z)
        {
            return getnearestline(Z.x, Z.y);
        }
        /// <summary>
        /// 指定した点に最もふさわしい辺を見つける
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <returns></returns>
        virtual public lineX getnearestline(float px, float py)
        {
            //Console.WriteLine("nearest");
            FXY center = new FXY(px, py);
            var points = getzettaipoints(false);

            FXY mycenter = getCenter();

            var linest = new FXY(center.x - mycenter.x, center.y - mycenter.y);
            linest.length += gaisyuu;

            linest.x += mycenter.x;
            linest.y += mycenter.y;
            var res = new lineX((points[0]), (points[0 + 1]), (mycenter));
            float mina = -1;

            //TRACE(_T("%f %f OIOI  %f %f\n"), mycenter.x, mycenter.y, linest.x, linest.y);
            for (int i = 1; i < points.Count() - 1; i++)
            {

                //TRACE(_T("%f %f OTYA  %f %f\n"), points[i].x, points[i].y, points[i+1].x, points[i+1].y);
                if (crosses(mycenter, linest, points[i], points[i + 1]))
                {
                    //      Console.WriteLine("crossed!");
                    var tmp = new lineX((points[i]), (points[i + 1]), (mycenter));
                    FXY sco = getzurasi(center, tmp, true);
                    float score = sco.x * sco.x + sco.y * sco.y;
                    //	TRACE(_T("%f :KITAKORE:  %f %f\n",mina,sco.x,sco.y));
                    if (mina < 0)
                    {
                        res = tmp;
                        mina = score;

                    }
                    else if (score > mina)
                    {

                        res = tmp;
                        mina = score;
                    }
                    else if (score == mina)
                    {
                        // TRACE(_T("owattaaaaa\n"));
                        res = new lineX(new FXY(res.begin.x - tmp.end.x + tmp.begin.x, res.begin.y - tmp.end.y + tmp.begin.y)
                            , new FXY(-res.begin.x + tmp.end.x + tmp.begin.x, -res.begin.y + tmp.end.y + tmp.begin.y)
                            , (mycenter));
                        mina = score;
                    }
                }
                else
                {
                    //       Console.WriteLine("1: " + mycenter.ToString());
                    //         Console.WriteLine("2: " + linest.ToString());
                    //         Console.WriteLine("3: " + points[i].ToString());
                    //          Console.WriteLine("4: " + points[i+1].ToString());
                }

            }
            //	TRACE(_T(" %f %f:idoukaku:%f %f  %f\n"), res.begin.x, res.begin.y, res.end.x, res.end.y,res.getrad());

            //   Console.WriteLine("END");
            return res;

        }

        /// <summary>
        /// 外周が時計回りで並んでいるかどうか
        /// </summary>
        /// <returns>時計回りか？</returns>
        public bool tokeimawari2()
        {
            return tokeimawari(getgaisyuus());
        }
        /// <summary>
        /// 外周が時計回りで並んでいるかどうか
        /// </summary>
        /// <param name="gaisyuu">なんでもいいから外周</param>
        /// <returns></returns>
        protected bool tokeimawari(List<double[]> gaisyuu)
        {
            if (gaisyuu.Count >= 2)
            {
                var a = Math.Atan2(gaisyuu[0][3] - gaisyuu[0][1], gaisyuu[0][2] - gaisyuu[0][0]);
                var b = Math.Atan2(gaisyuu[1][3] - gaisyuu[1][1], gaisyuu[1][2] - gaisyuu[1][0]);
                return radseiki(b - a) >= 0;
            }
            return true;
        }
        
        /// <summary>
        /// ラジアンの正規化
        /// </summary>
        /// <param name="rad">正規化するラジアン</param>
        /// <returns>-PI~+PI正規化されたラジアン</returns>
        static double radseiki(double rad) 
        {
            int i = 1;
            if (rad < 0) i = -1;
            var a = (rad * i) % (Math.PI * 2);

            if (a >= Math.PI) return -(Math.PI * 2 - a) * i;
            return a * i;
        }
        /// <summary>
        /// 対象の角の法線ベクトルを出す
        /// </summary>
        /// <param name="gaisyuu">なんでもいいから外周</param>
        /// <param name="taisyo"></param>
        /// <returns></returns>
        protected double hosenton(List<double[]> gaisyuu, double taisyo)
        {
            if (tokeimawari(gaisyuu))
            {
                return radseiki(taisyo - Math.PI);
            }
            else
            {
                return radseiki(taisyo + Math.PI);
            }
        }
        /// <summary>
        /// 図形を単純に移動させる
        /// </summary>
        /// <param name="dx">移動するxの距離</param>
        /// <param name="dy">移動するyの距離</param>
        public void idou(float dx, float dy)
        {

            x += dx; y += dy;
        }



    }
    /// <summary>
    /// 四角形を表すクラス
    /// </summary>
    public class Rectangle : Shape
    {
        /// <summary>
        /// 四角形を作る。ちなみにこの時回転は指定したx,yを中心に行われる
        /// </summary>
        /// <param name="xx">x座標</param>
        /// <param name="yy">y座標</param>
        /// <param name="ww">幅</param>
        /// <param name="hh">高さ</param>
        /// <param name="degree">回転角度</param>
        public Rectangle(float xx , float yy = 0, float ww = 0, float hh = 0, float degree = 0)
            : base(xx, yy, ww, hh, degree, new List<FXY> { new FXY(0, 0), new FXY(1, 0), new FXY(1, 1), new FXY(0, 1) })
        {

        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public Rectangle() { }
        //ToSaveとかは全く変わらないので何もなしでOK!

        public override void copy(Shape s)
        {
            base.copy(s);
        }
        public override DataSaver ToSave()
        {
            return base.ToSave();
        }
        public override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
        }

        public static implicit operator System.Drawing.RectangleF(Rectangle d)
        {
            return new System.Drawing.RectangleF(d.x,d.y,d.w,d.h);
        }

        public static implicit operator Vortice.RawRectF(Rectangle d)
        {
            return new Vortice.RawRectF(d.x, d.y, d.x+ d.w, d.y+ d.h);
        }

    }
    /// <summary>
    /// 三角形を表すクラス。
    /// しかし|>こういう左の辺がy軸と平行で右の先端が左の辺のy座標の間にあるな形の三角形しか作れない。
    /// </summary>
    public class Triangle : Shape
    {
        int _houkou = 1;
        int _basehoukou = 1;
        /// <summary>
        /// 三角形の先の方向。1で右-1で左2で下
        /// </summary>
        public int basehoukou { get { return _basehoukou; } set { if (-2 <= value && value <= 2 && value != 0) _basehoukou = value; } }
        public int houkou
        {
            get { return _houkou; }
            set
            {
                int pre = _houkou;
                if (-2 <= value && value <= 2 && value != 0) _houkou = value;
                if (pre != _houkou) changehoukou();
            }

        }

        public override void setMirror(int mir)
        {
            if (Mathf.abs(houkou) == 1)
            {
                if (mir > 0)
                {
                    houkou = basehoukou * 1;
                }
                else if (mir < 0)
                {
                    houkou = basehoukou * -1;
                }
                else
                {
                    houkou *= -1;
                }
            }
            base.setMirror(mir);
        }
        float _haji;
        /// <summary>
        /// 先端の高さの割合hajiは0<=<=1の範囲で変化する。
        /// </summary>
        float haji { get { return _haji; } set { _haji = value; if (_haji < 0) _haji = 0; if (_haji > 1) _haji = 1; } }
        /// <summary>
        /// 三角形を作る。左辺が底で右が先端。
        /// </summary>
        /// <param name="xx">x座標</param>
        /// <param name="yy">y座標</param>
        /// <param name="ww">幅</param>
        /// <param name="hh">高さ</param>
        /// <param name="hajih">高さと先端の高さの割合。0で左上の角が90°になる</param>
        /// <param name="hou">三角形の先端の方向1で右-1で左2で下</param>
        /// <param name="degree">回転角度</param>
        public Triangle(float xx , float yy = 0, float ww = 0, float hh = 0, float hajih = 0.5f, float degree = 0, int hou = 1)
            : base(xx, yy, ww, hh, degree, new List<FXY>())
        {
            haji = hajih;
            houkou = hou;
            basehoukou = hou;
            changehoukou();
        }

        public Triangle() { }

        public override void copy(Shape s)
        {
            base.copy(s);
            var t = (Triangle)s;
            t.haji = this.haji;
            t.houkou = this.houkou;
            t.basehoukou = this.basehoukou;
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("hajih",haji);
            res.packAdd("houkou", houkou);
            res.packAdd("basehoukou", basehoukou);
            return res;
        }
        public override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            haji=d.unpackDataF("hajih");
            houkou = (int)d.unpackDataF("houkou");
            basehoukou = (int)d.unpackDataF("basehoukou");
        }


        /// <summary>
        /// 三角形の方向を変えたり変えなかったり
        /// </summary>
        protected void changehoukou()
        {
            List<FXY> lis = new List<FXY>();
            switch (houkou)
            {
                case 1:
                    lis = new List<FXY> { new FXY(0, 0), new FXY(1, haji), new FXY(0, 1) };
                    break;
                case -1:
                    lis = new List<FXY> { new FXY(0, haji), new FXY(1, 0), new FXY(1, 1) };
                    break;
                case 2:
                    lis = new List<FXY> { new FXY(0, 0), new FXY(0, 1), new FXY(haji, 1) };
                    break;
                case -2:
                    lis = new List<FXY> { new FXY(haji, 0), new FXY(1, 1), new FXY(0, 1) };
                    break;
            }
            setpoints(lis);
        }

    }
    /// <summary>
    /// 円を表すクラス
    /// </summary>
    public class Circle : Shape
    {
        int kinji = 5;
        /// <summary>
        /// 1角の角度
        /// </summary>
        float onedegree { get { return 360 / kinji; } }
        /// <summary>
        /// 1辺の長さ
        /// </summary>
        float onelen
        {
            get
            {
                return (float)Mathf.cos(degree / 2);
            }
        }
        /// <summary>
        /// 円を作り出す。円はあたり判定の時、多角形に近似される。
        /// </summary>
        /// <param name="xx">x座標</param>
        /// <param name="yy">y座標</param>
        /// <param name="ww">幅</param>
        /// <param name="hh">高さ</param>
        /// <param name="degreee">回転角</param>
        /// <param name="pointkinji">近似する多角形の画数</param>
        public Circle(float xx , float yy = 0, float ww = 0, float hh = 0, float degreee = 0, int pointkinji = 20)
            : base(xx, yy, ww, hh, degreee, new List<FXY>())
        {
            if (pointkinji > kinji)
            {
                kinji = pointkinji;
                //  Console.WriteLine(pointkinji+"aslfkjaslslkinji"+kinji);
            }

            float degree = onedegree;
            float nag = onelen;
            //	TRACE(_T("%f Circle %f  %f\n"), nag * w, nag * h,nag);
            List<FXY> lis = new List<FXY> { };
            for (int i = 0; i < kinji; i++)
            {
                lis.Add(new FXY(0.5f + 0.5f * (float)Mathf.cos(degree * i), 0.5f + 0.5f * (float)Mathf.sin(degree * i)));
            }
            setpoints(lis);
        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public Circle() { }

        public override void copy(Shape s)
        {
            Circle ss= (Circle)s; 
            base.copy(ss);

            ss.kinji = this.kinji;
        }
        public override DataSaver ToSave()
        {
            var res=base.ToSave();
            res.linechange();
            res.packAdd("kinji",this.kinji);
            return res;
        }
        public override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.kinji = (int)d.unpackDataF("kinji");
        }
        public override bool onhani(float px, float py, float onis = 1)
        {
            var fxy = gettxy();
            float dx = px - fxy.x;
            float dy = py - fxy.y;

            double ddx = dx * Mathf.cos(-degree) - dy * Mathf.sin(-degree);
            double ddy = dx * Mathf.sin(-degree) + dy * Mathf.cos(-degree);



            return (ddx * ddx / (w / 2 * w / 2) + ddy * ddy / (h / 2 * h / 2) <= onis);
        }
        public override bool onhani(float px, float py, float ppx, float ppy)
        {
            var c = getCenter();



            float spx = px - c.x;
            float spy = py - c.y;
            float sppx = ppx - c.x;
            float sppy = ppy - c.y;

            float
                fpx = spx * (float)Mathf.cos(-degree) - spy * (float)Mathf.sin(-degree),
                fpy = spx * (float)Mathf.sin(-degree) + spy * (float)Mathf.cos(-degree),
                fppx = sppx * (float)Mathf.cos(-degree) - sppy * (float)Mathf.sin(-degree),
                fppy = sppx * (float)Mathf.sin(-degree) + sppy * (float)Mathf.cos(-degree);


            var kyo = new lineX(fpx, fpy, fppx, fppy, 0, 0).getkyori(0, 0);

            return Mathf.abs(kyo.x) <= Mathf.abs(w / 2) && Mathf.abs(kyo.y) <= Mathf.abs(h / 2);

        }
        /*
        /// <summary>
        /// 指定した点に最もふさわしい辺を見つける
        /// </summary>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <returns></returns>
        override public lineX getnearestline(float px, float py)
        {
            if (w != 0 && h != 0)
            {

                var c = getCenter();
                var k = new FXY(px, py);
                k -= c;
                k.rad -= this.rad;
                k.X /= w/2;
                k.Y /= h/2;

                var tyu = new FXY(0.5f, k.rad);
                var nags = new FXY(onelen/2, k.rad+Math.PI/2);

                var res = new lineX(tyu+nags, tyu - nags, new FXY(0,0));

                res.begin.X *= w / 2;
                res.begin.Y *= h / 2;
                res.end.X *= w / 2;
                res.end.Y *= h / 2;
                res.begin.rad += this.rad;
                res.end.rad += this.rad;

                res.begin += c;
                res.end += c;
                res.bs += c;

                return res;
            }
            return new lineX(getCenter(), getCenter(), getCenter());
        }*/

    }
}
