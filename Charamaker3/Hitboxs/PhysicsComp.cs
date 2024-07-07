using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charamaker3.Shapes;

namespace Charamaker3.Hitboxs
{
    /// <summary>
    /// 接点を保存するクラス
    /// </summary>
    public class TouchPoint
    {
        /// <summary>
        /// 接点の座標
        /// </summary>
        public FXY xy;
        /// <summary>
        /// 接している辺の情報
        /// </summary>
        public lineX line;
        /// <summary>
        /// 誰との接点か、誰の頂点によって接触しているか
        /// </summary>
        public Entity e, from;
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="fxy">接点の位置</param>
        /// <param name="linex">接戦</param>
        /// <param name="from">誰による頂点で接触したか</param>
        /// <param name="e">接触相手</param>
        public TouchPoint(FXY fxy, lineX linex, Entity from, Entity e)
        {
            xy = fxy;
            line = linex;
            this.e = e;
            this.from = from;
        }

    };
    /// <summary>
    /// 物理の情報を入れたやつ。一つ以上追加しない方が身のためだぞ。<br></br>
    /// HitBoxがあればいい感じに当たってくれるようになる。
    /// </summary>
    public class PhysicsComp:Component
    {
        /// <summary>
        /// 御存じ速度加速度
        /// </summary>
        public float vx, _vy, ax, ay;

        public float vy { get { return _vy; } set { _vy = value; } }
        /// <summary>
        /// 物体の速度
        /// </summary>
        public float speed { get { return (float)Math.Sqrt(vx * vx + vy * vy); } }
        /// <summary>
        /// スピードの方向
        /// </summary>
        public double speedvec { get { return Math.Atan2(vy, vx); } }

        /// <summary>
        /// 反発係数？
        /// </summary>
        protected float _hanpatu;
        /// <summary>
        /// 反発係数0~1
        /// </summary>
        public float hanpatu { get { return _hanpatu; } set { _hanpatu = value; if (_hanpatu < 0) _hanpatu = 0; if (_hanpatu > 1) _hanpatu = 1; } }
        /// <summary>
        /// 空気抵抗？
        /// </summary>
        protected float _teikou;
        /// <summary>
        /// 空気抵抗0~1
        /// </summary>
        public float teikou { get { return _teikou; } set { _teikou = value; if (_teikou < 0) _teikou = 0; if (_teikou > 1) _teikou = 1; } }
        /// <summary>
        /// 摩擦係数というか鉛直速度1あたりの摩擦パワー
        /// </summary>
        protected float _masatu = 0;
        /// <summary>
        /// 摩擦係数というか鉛直速度1あたりの摩擦パワー　0~+inf
        /// </summary>
        public float masatu { get { return _masatu; } set { _masatu = value; if (_masatu < 0) _masatu = 0; } }
        /// <summary>
        /// 重さ？
        /// </summary>
        protected float _wei;
        /// <summary>
        /// 重さの最大値
        /// </summary>
        public static float MW = 1000000;
        /// <summary>
        /// 重さ 0＜＜MW
        /// -1でMW
        /// </summary>
        public float wei { get { return _wei; } set { _wei = value; if (_wei <= 0) _wei = MW; if (_wei >= MW) _wei = MW; } }
        /// <summary>
        /// 重さが最大かどうか
        /// </summary>
        public bool ovw { get { return overweights(_wei); } }
        /// <summary>
        /// その重さが無限かどうか調べる。
        /// </summary>
        /// <param name="weight"></param>
        /// <returns></returns>
        static public bool overweights(float weight)
        {
            return weight >= MW;
        }

        /// <summary>
        /// 反射したときに呼び出されるイベント
        /// </summary>
        public EventHandler<PhysicsComp> onHansya;

        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="wei">重さ</param>
        /// <param name="tik">空気抵抗</param>
        /// <param name="hanp">反射係数</param>
        /// <param name="mas">摩擦係数</param>
        /// <param name="vx">速度</param>
        /// <param name="vy">速度</param>
        /// <param name="ax">加速度</param>
        /// <param name="ay">加速度</param>
        public PhysicsComp(float wei = 1, float tik = 0, float hanp = 0, float mas = 0, float vx = 0, float vy = 0, float ax = 0, float ay = 0)
        {
            this.wei = wei;
            teikou = tik;
            hanpatu = hanp;
            masatu = mas;
            this.vx = vx;
            this.vy = vy;
            this.ax = ax;
            this.ay = ay;
        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public PhysicsComp() { }

        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (PhysicsComp)c;
            cc.wei = this.wei;
            cc.teikou = this.teikou;
            cc.hanpatu = this.hanpatu;
            cc.masatu = this.masatu;
            cc.vx = this.vx;
            cc.vy = this.vy;
            cc.ax = this.ax;
            cc.ay = this.ay;

        }

        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("weight",wei);
            res.packAdd("teikou", teikou);
            res.packAdd("hanpatu", hanpatu);
            res.packAdd("masatu", masatu);
            res.linechange();
            res.packAdd("vx", vx);
            res.packAdd("vy", vy);
            res.packAdd("ax", ax);
            res.packAdd("ay", ay);
            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.wei=d.unpackDataF("weight");
            this.teikou=d.unpackDataF("teikou");
            this.hanpatu=d.unpackDataF("hanpatu");
            this.masatu=d.unpackDataF("masatu");
            this.vx=d.unpackDataF("vx");
            this.vy=d.unpackDataF("vy");
            this.ax=d.unpackDataF("ax");
            this.ay=d.unpackDataF("ay");
        }

        /// <summary>
        /// 重さに応じて加速できたりする
        /// </summary>
        /// <param name="vx">x方向の加速度</param>
        /// <param name="vy">y方向の加速度</param>
        /// <param name="weight">加速の重さ。0以下でvxyをそのままぶち込む</param>
        public void kasoku(float vx, float vy, float weight = -1)
        {
            if (weight <= 0)
            {
                this.vx += vx;
                this.vy += vy;
            }
            else
            {
                this.vx += vx * weight / (weight + this.wei);
                this.vy += vy * weight / (weight + this.wei);
            }
        }
        /// <summary>
        /// 重さに応じて加速できたりする。こちらは加速度として処理する。（つまりaxみたいに毎フレーム呼び出される奴はこっち使え）
        /// </summary>
        /// <param name="e">二度手間だけどお願い。そういう仕組みなんだ</param>
        /// <param name="vx">x方向の加速度</param>
        /// <param name="vy">y方向の加速度</param>
        /// <param name="weight">加速の重さ。0以下でvxyをそのままぶち込む</param>
        /// <param name="cl">今何フレーム経過してんのよ</param>
        public void kasoku(Entity e, float vx, float vy, float weight, float cl)
        {
            if (weight <= 0)
            {
                e.x += vx * cl * cl / 2;
                e.y += vy * cl * cl / 2;
                this.vx += vx * cl;
                this.vy += vy * cl;
            }
            else
            {
                e.x += vx * weight / (weight + this.wei) * cl * cl / 2;
                e.y += vy * weight / (weight + this.wei) * cl * cl / 2;
                this.vx += vx * cl * weight / (weight + this.wei);
                this.vy += vy * cl * weight / (weight + this.wei);
            }
        }
        protected override void onupdate(float cl)
        {
            base.onupdate(cl);

            vx += ax * cl;
            vy += ay * cl;
            vx *= (float)Math.Pow(1 - teikou, cl);
            vy *= (float)Math.Pow(1 - teikou, cl);

            var idou = new FXY(vx * cl + ax * cl * cl / 2, vy * cl + ay * cl * cl / 2);
            e.settxy(e.gettxy()+idou);

        }
        /// <summary>
        /// 相手と自分をなんか計算してずらす！
        /// ちなみにここでは大丈夫だけどataribinding.coreを動かしたらcharasetしないと移動が反映されないからちうい
        /// </summary>
        /// <param name="A">物理物体A</param>
        /// <param name="AS">Aの図形</param>
        /// <param name="B">物理物体B</param>
        /// <param name="BS">Bの図形</param>
        /// <param name="group">グループとして当たるか</param>
        /// <returns>ずらしたかどうか</returns>
        static public lineX zuren(PhysicsComp A,Hitbox AS,PhysicsComp B,Hitbox BS, bool group)
        {
            //  return zuren2(thiis, e, group);
            if (/*!e.atariable || !thiis.atariable ||*/ (A.ovw && B.ovw))
            {

                // Console.WriteLine("ZOUiojoijN");
                return null;
            }
            if (group)
            {
                if (overweights(A.groupwei) && overweights(B.groupwei))
                {
                    //Console.WriteLine("ZOUN");
                    return null;
                }
            }
            bool ok1 = false, ok2 = false;
            bool ok12 = false, ok22 = false;

            FXY idou = new FXY(0, 0);
            FXY idou2 = new FXY(0, 0);
            FXY idou1 = new FXY(0, 0);
            lineX line, line1, line2;
            FXY prekan = BS.preHitShape.getCenter() - AS.preHitShape.getCenter();
            {
                line1 = BS.HitShape.getnearestline(BS.HitShape.getCenter() - prekan);//thiis.PAcore.getCenter());
                var points = AS.HitShape.getzettaipoints(false);

                // Console.WriteLine(line1.ToString() + " asd ");
                //TRACE(_T("!!!!!!!!!!!!!\n"));
                for (int i = 1; i < points.Count - 1; i++)
                {

                    var tmp = Shape.getzurasi(points[i], line1);
                   /* Debug.WriteLine(points[i].ToString()+" :thiis");
                    Debug.WriteLine(tmp.x + " soy " + tmp.y+" -> hos "+line1.hosen);
                    Debug.WriteLine(idou + " soythis " + (tmp.length >= idou.length));
                    */
                    if (!float.IsNaN(tmp.x) && tmp.length >= idou1.length)
                    {
                        var kakusa = tmp.degree - line1.hosen;
                        kakusa = Mathf.st180(kakusa);
                      // Debug.WriteLine(tmp+" -> "+kakusa + " =1=1= " +tmp.degree+ " - " + line1.hosen);
                        if (Mathf.abs(kakusa) >= 90)
                        {

                       //     Debug.WriteLine("OKAAAAAAAAA" + tmp);
                            idou1.x = tmp.x;
                            idou1.y = tmp.y;
                            ok1 = true;
                            ok12 = true;
                        }
                        else
                        {
                            ok12 = true;
                        }
                    }
                    else if (!float.IsNaN(tmp.y))
                    {
                        ok12 = true;
                    }

                }
            }
            //TRACE(_T("!!!!!!!!!!!!!\n"));
            {
                line2 = AS.HitShape.getnearestline(AS.HitShape.getCenter() + prekan);//BS.HitShape.getCenter());
                var points = BS.HitShape.getzettaipoints(false);

                //Console.WriteLine(line2.ToString()+" asd ");
                for (int i = 1; i < points.Count - 1; i++)
                {

                    // Console.WriteLine(points[i].ToString() + " :E");
                    var tmp = Shape.getzurasi(points[i], line2);
                    /*
                    Debug.WriteLine(points[i].ToString() + " :eees");
                    Debug.WriteLine(tmp.x + " soy " + tmp.y+" hos -> "+line2.hosen);
                    Debug.WriteLine(idou2 + " soyees " + (tmp.length >= idou2.length));
                    */
                    //  Console.WriteLine(tmp.X + " sey " + tmp.Y);
                    if (!float.IsNaN(tmp.x) && tmp.length >= idou2.length)
                    {

                        var kakusa = tmp.degree - line2.hosen;
                        kakusa = Mathf.st180(kakusa);
                      //  Debug.WriteLine(tmp + " -> " + kakusa + " =2=2= " + tmp.degree + " - " + line2.hosen);
                        if (Mathf.abs(kakusa) <= 90)
                        {
                        //    Debug.WriteLine("OKAAAAAAAAA" +tmp);
                            idou2.x = -tmp.x;
                            idou2.y = -tmp.y;
                            ok2 = true;
                            ok22 = true;
                        }
                        else
                        {
                            ok22 = true;
                        }
                    }
                    else if (!float.IsNaN(tmp.y))
                    {
                        ok22 = true;
                    }

                }
            }
            //	idou.x += idou2.x;
            //	idou.y += idou2.y;
            // if (A.ovw || B.ovw)
            //    Console.WriteLine(idou.ToString() + "asfa" + idou2.ToString());
           // Debug.WriteLine(idou1+" zurezure man tikusyou "+idou2+" ok ->"+ok2+" "+ok1+" ");
            
            /*  if (idou.length > 50||idou2.length>50)
              {
                  Console.WriteLine(ok1 + " KKKKKKKKKK " + ok2);
                  Console.WriteLine(idou.X + " a:fkqqql " + idou.Y );
                  Console.WriteLine(line1.ToString() + "  ::linee");
                  Console.WriteLine(idou2.X + " a:fqkqql " + idou2.Y);
                  Console.WriteLine(line2.ToString() + "  ::linee2");
              }*/
            if (!ok12 || !ok22) return null;
            if (ok1 && ok2)
            {
                //Console.WriteLine(idou.ToString()+"double" +idou2.ToString());
                if (Math.Pow(idou2.x, 2) + Math.Pow(idou2.y, 2) < Math.Pow(idou1.x, 2) + Math.Pow(idou1.y, 2))
                {
                    idou.x = idou2.x;
                    idou.y = idou2.y;
                    line = line2;
                }
                else
                {
                    idou.x = idou1.x;
                    idou.y = idou1.y;
                    line = line1;
                }
            }
            else if (!ok1)
            {
                idou.x = idou2.x;
                idou.y = idou2.y;
                line = line2;
            }
            else
            {
                idou.x = idou1.x;
                idou.y = idou1.y;
                line = line1;
            }

            //  if (idou.X == 0 && idou.Y == 0) return null;
            //   if(A.ovw||B.ovw)
            //Debug.WriteLine(line1.hosen+" 1line2 " + line2.hosen);
            //Debug.WriteLine("idou:->" +idou.ToString());

            //TRACE(_T("%d idou %f::%f\n"),notNAN,idou.x,idou.y);

            float eweight;
            float thisweight;

            if (group)
            {
                eweight = B.groupwei;
                thisweight = A.groupwei;
            }
            else
            {

                eweight = B.wei;
                thisweight = A.wei;
            }

            if (B.ovw)
            {
                if (group)
                {
                    if (A.ovw)
                    {
                        PhysicsComp.idou(A,idou.x, idou.y);
                    }
                    else
                    {
                        PhysicsComp.groupidou(A,AS, idou.x, idou.y);
                    }
                }
                else
                {
                    PhysicsComp.idou(A,  idou.x, idou.y);
                }
                //this.idouAdd(idouinfo(idou, Entity::overweight));
            }
            else if (A.ovw)
            {
                if (group)
                {
                    if (B.ovw)
                    {
                        PhysicsComp.idou(B, -idou.x, -idou.y);
                    }
                    else
                    {

                        PhysicsComp.groupidou(B,BS, -idou.x, -idou.y);
                    }
                }
                else
                {
                    PhysicsComp.idou(B, -idou.x, -idou.y);
                }
            }
            else
            {
                //Console.WriteLine(thiis.c.gettx()+" <-this x e-> "+e.c.gettx());
                //Console.WriteLine(line.ToString() + " yapaa00000 " + idou.ToString());
                //Console.WriteLine(line1.ToString() + " yapaa11111 "+ idou1.ToString());
                //Console.WriteLine(line2.ToString() + " yapaa22222 "+ idou2.ToString());
                // Console.WriteLine(thiis.PAcore.gettx() + " :XY: " +AS.HitShape.getty() + " <PRE> "
                //  + BS.HitShape.gettx() + " :XY: " + BS.HitShape.getty());
                //  Console.WriteLine(AS.HitShape.gettx() + " :XY: " + AS.HitShape.getty() + " <bef> "
                //     + BS.HitShape.gettx() + " :XY: " + BS.HitShape.getty());
                //Console.WriteLine(idou.ToString()+" al;fkapo ");
                var hi = thisweight / (thisweight + eweight);
                if (group)
                {
                    PhysicsComp.groupidou(A,AS, (1 - hi) * idou.x, (1 - hi) * idou.y);
                    PhysicsComp.groupidou(B,BS, -(hi) * idou.x, -(hi) * idou.y);


                    if (idou.x != 0 || idou.y != 0)
                    {
                        PhysicsComp.groupAdd(A, AS,B,BS);
                    }
                }
                else
                {
                    PhysicsComp.idou(A,  (1 - hi) * idou.x, (1 - hi) * idou.y);

                    PhysicsComp.idou(B, (-hi) * idou.x, (-hi) * idou.y);
                }

                //    Console.WriteLine(AS.HitShape.gettx() + " :XY: " + AS.HitShape.getty() + " <aft> "
                //        + BS.HitShape.gettx() + " :XY: " + BS.HitShape.getty());

                //Console.WriteLine(thiis.c.gettx() + " <-this afterx e-> " + e.c.gettx());
                //	TRACE(_T("%f ZOY %f + %f\n"),idou.x, (1 - hi) * idou.x, (hi) * idou.x);
            }
            // Console.WriteLine(idou.X + " a:fkl " + idou.Y + " :: " + thisweight + " -> " + eweight);
            // Console.WriteLine(line1.ToString()+"  ::linee");

            //var line = e.s.getnearestline(this.s.getCenter());
            return line1;
        }


        /// <summary>
        /// 自分と対象ので反射を引き起こす。回転は考慮に入ってないつらいから
        /// </summary>
        /// <param name="A">物理物体A</param>
        /// <param name="AS">Aの図形</param>
        /// <param name="B">物理物体B</param>
        /// <param name="BS">Bの図形</param>
        /// <param name="line">ぶつかった辺</param>
        /// <returns>反射が起きたかどうか</returns>
        static public bool hansya(PhysicsComp A, Hitbox AS, PhysicsComp B, Hitbox BS, lineX line)
        {
            //Console.WriteLine(line.ToString());
            if (line == null)
            {
                return false;
            }
            return hansya(A,AS,B,BS, line.degree);
        }
        /// <summary>
        /// 自分と対象ので反射を引き起こす。回転は考慮に入ってないつらいから
        /// </summary>
        /// <param name="A">物理物体A</param>
        /// <param name="AS">Aの図形</param>
        /// <param name="B">物理物体B</param>
        /// <param name="BS">Bの図形</param>
        /// <param name="degree">ぶつかった辺の角度</param>
        /// <returns>反射が起きたかどうか</returns>
        static public bool hansya(PhysicsComp A, Hitbox AS, PhysicsComp B, Hitbox BS, float degree)
        {
            float eweight = B.wei;
            float thisweight = A.wei;

            if (PhysicsComp.overweights(eweight) && PhysicsComp.overweights(thisweight))
            {
                return false;
            }
            var hansya = (A.hanpatu + B.hanpatu) / 2;
            var masatu = (A.masatu + B.masatu) / 2;

            FXY toe = new FXY(0, 0);
            FXY tothis = new FXY(0, 0);


            var hos = degree;
            var hoh =AS.preHitShape.nasukaku(BS.preHitShape);
            if (Mathf.st180(hoh - hos) >= 0)
            {
                hos -= 90;
            }
            else
            {
                hos += 90;
            }
            hos = Mathf.st180(hos);
           
            //TRACE(_T(" %f :kakudo: %f\n"), rad / M_PI * 180, hos / M_PI * 180);
            //hos = line.gethosen();
            //rad = line.getrad();
            //TRACE(_T("%f :: %f   %f = %f\n"),hos/M_PI*180, rad / M_PI * 180, line2.getrad(), line.getrad())

            float evx = B.vx, evy = B.vy, thisvx = A.vx, thisvy = A.vy;


            var sp = (float)(Mathf.cos(hos) * (thisvx - evx) + Mathf.sin(hos) * (thisvy - evy));
            var spDMMY = (float)(Mathf.cos(hos) * (thisvx) + Mathf.sin(hos) * (thisvy));
            var gsp = (float)(Mathf.cos(hos) * (-thisvx + evx) + Mathf.sin(hos) * (-thisvy + evy));
            var sp2 = (float)(Mathf.cos(degree) * (thisvx - evx) + Mathf.sin(degree) * (thisvy - evy));
            if (sp < 0)
            {

            }
            else
            {
               //     Debug.WriteLine(A.vx + " " + A.vy + "  :sokubef: " + B.vx + " " + B.vy);

                 //   Debug.WriteLine(sp+" :CANCELLED: "+hos+" moto ="+degree+" :deg hoh: "+hoh);
                return false;
            }
            // Console.WriteLine(sp + " :KEIZOKUED: " + hos / Math.PI * 180);

            if (A.ovw)
            {
                var msatupower = Mathf.abs(masatu * sp);
                if (msatupower > Mathf.abs(sp2))
                {
                    sp2 = sp2;
                }
                else
                {
                    if (sp2 < 0)
                    {
                        sp2 = -msatupower;
                    }
                    else
                    {
                        sp2 = +msatupower;
                    }

                }
                //TRACE(_T("ohhhhhh111\n"));
                toe.x += sp * (hansya + 1) * Mathf.cos(hos);
                toe.y += sp * (hansya + 1) * Mathf.sin(hos);

                toe.x += sp2 * Mathf.cos(degree);
                toe.y += sp2 * Mathf.sin(degree);
            }
            else if (B.ovw)
            {
                var msatupower = Mathf.abs(masatu * sp);
                if (msatupower > Mathf.abs(sp2))
                {
                    sp2 = sp2;
                }
                else
                {
                    if (sp2 < 0)
                    {
                        sp2 = -msatupower;
                    }
                    else
                    {
                        sp2 = +msatupower;
                    }

                }
                //TRACE(_T("ohhhhhh222\n"));
                tothis.x -= sp * (hansya + 1) * Mathf.cos(hos);
                tothis.y -= sp * (hansya + 1) * Mathf.sin(hos);

                tothis.x -= sp2 * Mathf.cos(degree);
                tothis.y -= sp2 * Mathf.sin(degree);
            }
            else
            {
                float masatupower;
                float m1 = thisweight;
                float m2 = eweight;
                {
                    var v1z = Mathf.cos(hos) * (thisvx) + Mathf.sin(hos) * (thisvy);
                    var v2z = Mathf.cos(hos) * (evx) + Mathf.sin(hos) * (evy);
                    var vm =
                        (Mathf.cos(hos) * (thisvx * thisweight + evx * eweight)
                        + Mathf.sin(hos) * (thisvy * thisweight + evy * eweight));
                    //  Console.WriteLine(v1z + " v1v2" + v2z + " = " + vm);

                    var vvm = Mathf.cos(hos) * Mathf.cos(hos) * (thisvx * thisvx * thisweight + evx * evx * eweight) / 2
                        + Mathf.sin(hos) * Mathf.sin(hos) * (thisvy * thisvy * thisweight + evy * evy * eweight) / 2;

                    //TRACE(_T("%f jaisfjoa \n"),cos(hos) * cos(hos) * (this.vx * this.vx * thisweight + e.vx * e.vx * eweight));
                    //TRACE(_T("%f jaisfjoa %f\n"), cos(hos) ,(this.vx * this.vx * thisweight + e.vx * e.vx * eweight));
                    //TRACE(_T("%f jaisfjoa %f :: %f\n"), hos, (this.vx * this.vx * thisweight),( e.vx * e.vx * eweight));
                    float v1 = 0, v2 = 0;
                    float v11 = 0;
                    float v12 = 0;




                    float hansyapower = (1 - hansya) * Mathf.sqrt(2 * vvm / (m1 + m2)) * (m1 + m2);

                    //TRACE(_T("%f . %f oar %f\n"), vm, vm * hansya + hansyapower, vm * hansya - hansyapower);
                    if (Mathf.abs(hansyapower - vm) < Mathf.abs(-hansyapower - vm))
                    {
                        vm = vm * hansya + hansyapower;
                    }
                    else
                    {
                        vm = vm * hansya - hansyapower;
                    }
                    //TRACE(_T("%f ninatavmVM\n"), vm);
                    float naka = (4 * m1 * m1 / m2 / m2 * vm * vm) - 4 * (m1 + m1 * m1 / m2) * (vm * vm / m2 - 2 * vvm);
                    if (naka < 0)
                    {
                        //	TRACE(_T("ohhhhhh %f\n"),naka);
                        naka *= -1;
                    }
                    float sqrt = (float)Math.Sqrt(naka);

                    v11 = (2 * vm * m1 / m2 + sqrt) / 2 / (m1 + m1 * m1 / m2);
                    v12 = (2 * vm * m1 / m2 + sqrt) / 2 / (m1 + m1 * m1 / m2);

                    v1 = v11;

                    v2 = (vm - v1 * m1) / m2;
                    // Console.WriteLine(v1 + " yattaze " + v2+" a ;: "+vm);
                    if (Mathf.abs(-v1 * thisweight + v2 * eweight - vm)
                    > Mathf.abs(v1 * thisweight + v2 * eweight - vm))
                    {
                        v1 = -v1;
                    }
                    if (Mathf.abs(-v1 * thisweight + v2 * eweight - vm)
                        > Mathf.abs(-v1 * thisweight - v2 * eweight - vm))
                    {
                        v2 = -v2;
                    }
                    if (Mathf.abs(-v1 * thisweight + v2 * eweight - vm)
                        > Mathf.abs(v1 * thisweight - v2 * eweight - vm))
                    {
                        v1 = -v1;

                        v2 = -v2;
                    }

                    //TRACE(_T("%f or %f  tina %f\n v2.%f tina %f\n"), v11, v12, v1z,v2,v2z);
                    //TRACE(_T("%f = %f ,,\n,, %f = %f\n %f ~~~ %f\n"),v1*m1+v2*m2,vm
                    //	,v1*v1*m1/2+v2*v2*m2/2,vvm,v1,v2);

                    //TRACE(_T("%f :kekktoku: %f\n"), -(v1+v1z) * cos(hos), (v2+v2z) * cos(hos));

                    //	TRACE(_T("%f . %f :vmvvm: %f  \n"), vm,vm*vm/m1,-vvm);
                    //	TRACE(_T(" %f :v1v2: %f   tina %f - %f > 0 ?? %f\n"), v1, v2, (4 * m2 * m2 / m1 / m1*vm*vm) , 4 * (vm * vm/m1 - vvm) * (m2 * m2 / m1+m1),sqrt);
                    // Console.WriteLine(v1 + " yattaze " + v2);
                    tothis.x += -(v1 + v1z) * Mathf.cos(hos);
                    tothis.y += -(v1 + v1z) * Mathf.sin(hos);

                    toe.x += (v2 - v2z) * Mathf.cos(hos);
                    toe.y += (v2 - v2z) * Mathf.sin(hos);
                    masatupower = Mathf.abs(v1 + v1z) / 2 * thisweight + Mathf.abs(v2 - v2z) / 2 * eweight;

                }


                {
                    float thisspeeed = thisvx * Mathf.cos(degree) + thisvy * Mathf.sin(degree);
                    float espeeed = evx * Mathf.cos(degree) + evy * Mathf.sin(degree);

                    var vm = (thisspeeed * thisweight) + (espeeed * eweight);

                    float dousoku = vm / (thisweight + eweight);
                    float gone = dousoku - thisspeeed;
                    gone *= thisweight;
                    if (Mathf.abs(gone) > masatupower)
                    {
                        if (gone > 0)
                        {
                            gone = masatupower;
                        }
                        else
                        {
                            gone = -masatupower;
                        }
                    }
                    //  Console.WriteLine(thisspeeed+" sad "+dousoku+" asf "+espeeed );
                    // Console.WriteLine((gone) / thisweight + " -> "+(gone) / thisweight * Mathf.cos(rad)+" as:ew "+ (gone) / thisweight * Mathf.sin(rad));
                    // Console.WriteLine(-(gone) / eweight + " ->"+-(gone) / eweight * Mathf.cos(rad) + " as:66 " + -(gone) / eweight * Mathf.sin(rad));

                    tothis.x += (gone) / thisweight * Mathf.cos(degree);
                    tothis.y += (gone) / thisweight * Mathf.sin(degree);

                    toe.x += -(gone) / eweight * Mathf.cos(degree);
                    toe.y += -(gone) / eweight * Mathf.sin(degree);
                }

            }

          //  Debug.WriteLine(A.vx + " " + A.vy + "  :sokubef: " + B.vx + " " + B.vy);
            A.vx += tothis.x;
            A.vy += tothis.y;
            B.vx += toe.x;
            B.vy += toe.y;

            
         //   Debug.WriteLine(A.vx + " " + A.vy + "  :sokuaft: " + B.vx + " " + B.vy);
            
            A.onHansya?.Invoke(A, B);
            B.onHansya?.Invoke(B, A);

            return true;
        }

        /// <summary>
        /// ちゃんと移動させるメソッド。Acoreもcharacterも動かす
        /// しかし、abを基準に移動するため、waza.framedとかで軽率に呼び出すと死ぬ
        /// </summary>
        /// <param name="A">設計理念上こうなってしまったんや済まない</param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        static public void idou(PhysicsComp A,float dx, float dy)
        {
            var pre=A.e.gettxy();
            var txy = A.e.gettxy() + new FXY(dx, dy);
            if (Haikei.SetCameraZahyou(A.e, txy.x, txy.y))//背景で超越した動きをする場合、こんな動きになる
            {

            }
            else
            {
                A.e.settxy(txy.x,txy.y);
            }
            A.e.RefreshComponentsPosition();
        }

        #region group
        List<PhysicsComp> groupsP = new List<PhysicsComp>();
        List<Hitbox> groupsH = new List<Hitbox>();
        

        /// <summary>
        /// 構成しているグループごと移動させる
        /// </summary>
        /// <param name="thiis">ごめん</param>
        /// <param name="dx">移動距離</param>
        /// <param name="dy"></param>
        static protected void groupidou(PhysicsComp A,Hitbox AS, float dx, float dy)
        {
            /*{
                float sumx = 0;
                float sumy = 0;
                foreach (var a in groups)
                {
                    sumx += a.Acore.gettx() - thiis.Acore.gettx();
                    sumy += a.Acore.getty() - thiis.Acore.getty();
                }
                Console.WriteLine(sumx + " soutaibef " + sumy);
            }*/
            idou(A,dx, dy);

            for (int i = 0; i < A.groupsH.Count; i++)
            {
                //var tyomu = new FXY(1, );
                //var kaku = A.groupsH[i].HitShape.nasukaku(groupsH[i].HitShape);
                var kaku = AS.HitShape.getnearestline(A.groupsH[i].HitShape.getCenter()).hosen;
                var sa = 1 - (float)Mathf.abs(Mathf.st180(Mathf.atan(dy, dx) - kaku)
                    / 90);
                if (sa < 0)
                {
                    sa = 0;
                }
                PhysicsComp.idou(A.groupsP[i], dx * sa, dy * sa);
            }
            /*{
                float sumx = 0;
                float sumy = 0;
                foreach (var a in groups)
                {
                    sumx += a.Acore.gettx() - thiis.Acore.gettx();
                    sumy += a.Acore.getty() - thiis.Acore.getty();
                }
                Console.WriteLine(sumx + " soutaiaft " + sumy);
            }*/
            // Console.WriteLine(x+":group "+groups.Count+" idoued: "+y);
        }

        /// <summary>
        /// グループ合計の重さを取得する
        /// </summary>
        /// <returns></returns>
        protected float groupwei
        {
            get
            {
                float sum = wei;
                for (int i = 0; i < groupsP.Count; i++)
                {
                    sum += groupsP[i].wei;
                }
                return sum;
            }
        }
        /// <summary>
        /// グループをリセットする
        /// </summary>
        public void groupclear()
        {
            groupsH.Clear();
            groupsP.Clear();
        }
        /// <summary>
        /// グループを追加する。両方にグループが追加されるから安心！<br></br>
        /// もちろんグループが統合されるよ
        /// </summary>
        /// <param name="thiis">ごめん</param>
        /// <param name="e">グループする相手</param>
        static protected void groupAdd(PhysicsComp A, Hitbox AS, PhysicsComp B, Hitbox BS)
        {
            int cou = 0;
            List<int> ittied = new List<int>();
            bool go;
            for (int i = 0; i < A.groupsH.Count; i++)
            {
                go = true;
                for (int t = 0; t < B.groupsH.Count; t++)
                {
                    if (A.groupsH[i] == B.groupsH[t])
                    {
                        go = false;
                        ittied.Add(t);
                        break;
                    }
                }
                if (go)
                {
                    B.groupsH.Add(A.groupsH[i]);
                    B.groupsP.Add(A.groupsP[i]);
                    cou += 1;
                }
            }
            for (int i = 0; i < B.groupsH.Count - cou; i++)
            {
                go = true;
                for (int t = 0; t < ittied.Count; t++)
                {
                    if (i == ittied[t])
                    {
                        go = false;
                        break;
                    }
                }
                if (go)
                {
                    A.groupsH.Add(B.groupsH[i]);
                    A.groupsP.Add(B.groupsP[i]);

                }
            }
            A.groupsH.Add(BS);
            B.groupsH.Add(AS);
            A.groupsP.Add(B);
            B.groupsP.Add(A);

        }
        /// <summary>
        /// グループになっているか
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool isgrouped(PhysicsComp e)
        {
            return groupsP.Contains(e);
        }

        /// <summary>
        /// グループになっているか
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool isgrouped(Hitbox e)
        {
            return groupsH.Contains(e);
        }
        #endregion
        #region TouchPoint
        List<TouchPoint> sessyokus = new List<TouchPoint>();

        /// <summary>
        /// 接触点を追加する
        /// </summary>
        /// <param name="where">追加する接触点の座標</param>
        /// <param name="l">接触する辺</param>
        /// <param name="from">誰の接点から接触するのか</param>
        /// <param name="e">接触相手</param>
        protected void sessyokuAdd(FXY where, lineX l, Entity from, Entity e)
        {
            //TRACE(_T("%f :sessyoku: %f\n"),where.x,where.y);
            sessyokus.Add(new TouchPoint(where, l, from, e));
        }

        /// <summary>
        /// 接触点をセットする。当然両者のAcoreは存在してるよね？
        /// </summary>
        /// <param name="thiis">当たるエンテティ</param>
        /// <param name="E">接点をセットする相手</param>
        /// <returns></returns>
        static public bool setsessyoku(PhysicsComp A,Hitbox AS, PhysicsComp B ,Hitbox BS)
        {
            if (AS.HitShape.atattenai(BS.HitShape)) return false;

            var thispoints = AS.HitShape.getzettaipoints();
            var Epoints = BS.HitShape.getzettaipoints();
            bool added = false;
            //Console.WriteLine("settyoku");

            for (int i = 0; i < thispoints.Count; i++)
            {
                if (BS.HitShape.onhani(thispoints[i].x, thispoints[i].y, 1.00001f))
                {
                    //         Console.WriteLine("FOOO");
                    added = true;
                    var line = BS.HitShape.getnearestline(thispoints[i]);
                    A.sessyokuAdd(thispoints[i], line, A.e, B.e);
                    B.sessyokuAdd(thispoints[i], line, A.e, A.e);//何でこれ両方Aなん？
                }
            }
            for (int i = 0; i < Epoints.Count; i++)
            {
                if (AS.HitShape.onhani(Epoints[i].x, Epoints[i].y, 1.00001f))
                {
                    //        Console.WriteLine("FOOwwwwwO");
                    added = true;
                    var line = AS.HitShape.getnearestline(Epoints[i]);
                    A.sessyokuAdd(Epoints[i], line, B.e, B.e);//何でこれ両方Bなん？
                    B.sessyokuAdd(Epoints[i], line, B.e, A.e);
                }
            }
            // Console.WriteLine(added+" settyokuEND ");
            return added;
        }
        /// <summary>
        /// 接触点によって反射を行う
        /// </summary>
        /// <param name="thiis">このbifの所属してるやつだよ！</param>
        /// <param name="e">反射相手</param>
        static public void SessyokuHansya(PhysicsComp A, Hitbox AS, PhysicsComp B, Hitbox BS)
        {
            //  Console.WriteLine("SESSSYOKUHANSYA");
            List<TouchPoint> vex = new List<TouchPoint>();
            for (int i = 0; i < A.sessyokus.Count; i++)
            {
                var eee = A.sessyokus[i];
                if (B.e == eee.e)
                {
                    vex.Add(A.sessyokus[i]);

                }
            }
            if (vex.Count == 0 || (B.ovw && A.ovw)) { return; }
            // Console.WriteLine("VEX: "+vex.Count);
            if (vex.Count == 1)
            {
             //   Debug.WriteLine(" hansya by line");
                PhysicsComp.hansya(A,AS,B,BS, vex[0].line);
                //e->hansya(this, vex[0].line.getrad());
            }
            else if (vex.Count > 1)
            {
                float degree = 0;
                //一つの接触点から、最も遠い接触点を持ってきて、その線を反射に用いる。
                for (int i = 0; i < vex.Count; i++)
                {
                    float temp = 0;
                    float tempkyo = -1;
                    for (int t = 0; t < vex.Count; t++)
                    {

                        FXY soy; ;
                        if (i < t)
                        {
                            soy = (vex[i].xy - vex[t].xy);
                        }
                        else 
                        {
                            soy = (vex[t].xy - vex[i].xy);
                        }
                        float slen = soy.length;
                        if ( slen> tempkyo)
                        {
                            temp = Mathf.st90(soy.degree);
                            tempkyo = slen;
                        }
                    }
                   // Debug.WriteLine(temp  + " tou"+ vex[i].line.ToString());
                    degree += temp / vex.Count;
                }
               // Debug.WriteLine(" hansya by degree + "+degree);
                PhysicsComp.hansya(A,AS,B,BS, degree);
            }

        }
        /// <summary>
        /// 接触点をクリアする
        /// </summary>
        public void resetsessyokus()
        {
            sessyokus.Clear();
        }
        /*
         /// <summary>
         /// 接触点から回転軸を取得する
         /// </summary>
         /// <returns>必ず2つの点</returns>
         lineX getkaitenjiku();

         /// <summary>
         /// 接触点をもとに回転を行う
         /// </summary>
         /// <param name="cl">時間の速さ</param>
         void sessyokukaiten(float cl);*/
        /// <summary>
        /// 接触点をリセットする
        /// </summary>

        #endregion

        #region energy
        FXY energyPoint = new FXY(0, 0);
        /// <summary>
        /// atarableに付けてね
        /// 位置エネルギーの基準点をセットする
        /// </summary>
        public void setEnergyPoint()
        {
            energyPoint = e.gettxy();
        }
        /// <summary>
        /// 位置エネルギーを保存する.
        /// atarableに付けてね
        /// </summary>
        public void energyConserv()
        {
            return;
            var c = e.gettxy();
            float dx = -c.x + energyPoint.x;
            float dy = -c.y + energyPoint.y;
            float xxx = (float)Math.Sqrt(Mathf.abs(ax * dx * 2));
            float yyy = (float)Math.Sqrt(Mathf.abs(ay * dy * 2));
            if (ax * dx < 0)
            {
                kasoku(-xxx, 0);
            }
            else
            {
                kasoku(xxx, 0);
            }
            if (ay * dy < 0)
            {
                kasoku(0, -yyy);
            }
            else
            {
                kasoku(0, yyy);
            }
            setEnergyPoint();
        }
        #endregion

    }
}
