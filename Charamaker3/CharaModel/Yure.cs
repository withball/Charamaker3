using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charamaker3.Hitboxs;

namespace Charamaker3.CharaModel
{


    /// <summary>
    /// 体に追加されているHitBoxを利用してがんばって揺らす。
    /// </summary>
    public class Yure : Component
    {
        /// <summary>
        /// ターゲットのパーツ
        /// </summary>
        public String Target = "";
        /// <summary>
        /// 通常状態を何度として扱うか
        /// </summary>
        public float BaseDegree = 0;
        /// <summary>
        /// 速度に変換する割合1未満にすること。
        /// </summary>
        public float SpeedPower = 0;
        /// <summary>
        /// 最大の角度
        /// </summary>
        public float MaxDegree = 0;
        /// <summary>
        /// 最大の回転速度
        /// </summary>
        public float MaxDegreeSpeed = 0;
        /// <summary>
        /// 速度の小さくなり具合
        /// </summary>
        public float Teikou = 0.9f;
        /// <summary>
        /// BaseDegreeに戻る力
        /// </summary>
        public float Gravity = 1;
        /// <summary>
        /// 速度。これは保存しない。
        /// </summary>
        public float DegreeSpeed = 0;

        protected override void onupdate(float cl)
        {
            base.onupdate(cl);
            var boxs = new List<Hitbox>();
            foreach (var a in e.getCharacter().getTree(""))
            {
                foreach (var b in a.getcompos<Hitbox>())
                {
                    boxs.Add(b);
                }
            }
            //ヒットボックスの移動具合の合計がこのフレームのパワー
            FXY power = new FXY(0, 0);
            foreach (var a in boxs)
            {
                power += (a.HitShape.gettxy() - a.preHitShape.gettxy()) / boxs.Count * SpeedPower;
            }
            var Gravoty = new FXY(Gravity, 0) * cl;
            Gravoty.degree = BaseDegree;
            power += Gravoty;


            float DegreeTo = power.degree - BaseDegree;

            if (DegreeTo > MaxDegree)
            {
                DegreeTo = MaxDegree;
            }
            else if (DegreeTo < -MaxDegree)
            {
                DegreeTo = -MaxDegree;
            }

            var tag = e.getCharacter().getEntity(Target);
            var Btag = e.getCharacter().BaseCharacter.getEntity(Target);
            if (tag != null && Btag != null)
            {
                if (Mathf.st180(DegreeTo - tag.degree) > 0)
                {
                    DegreeSpeed += power.length;
                }
                else
                {
                    DegreeSpeed -= power.length;

                }
                DegreeSpeed *= (float)Math.Pow(1 - Teikou, cl);

                var speed = cl * Mathf.sameSign(Mathf.min(Mathf.abs(DegreeSpeed), MaxDegreeSpeed), DegreeSpeed);

                if (MaxDegree < 180)
                {
                    var tdeg = tag.degree - Btag.degree;
                    var sptdeg = tag.degree - Btag.degree + speed;
                    var tyouka = Mathf.abs(sptdeg) - MaxDegree + MaxDegreeSpeed * 0;
                    var tyoukapre = Mathf.abs(tdeg) - MaxDegree + MaxDegreeSpeed * 0;
                    if (tyouka > 0 && tyouka > tyoukapre)//超過が小さくなる方向には動ける
                    {
                        speed -= Mathf.sameSign(tyouka, speed);
                    }
                }
                EntityMove.XYD(1, "", 0, 0, speed)
                    .addAndRemove(tag, 1);


            }



        }
        public override void addtoworld(float cl = 0)
        {
            base.addtoworld(cl);
            DegreeSpeed = 0;
        }
        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (CharacterYure)c;
            cc.Target = this.Target;
            cc.BaseDegree = this.BaseDegree;
            cc.SpeedPower = this.SpeedPower;
            cc.MaxDegree = this.MaxDegree;
            cc.MaxDegreeSpeed = this.MaxDegreeSpeed;
            cc.Gravity = this.Gravity;
            cc.Teikou = this.Teikou;
            cc.DegreeSpeed = this.DegreeSpeed;
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.packAdd("Target", this.Target);
            res.linechange();
            res.packAdd("BaseDegree", this.BaseDegree);
            res.packAdd("SpeedPower", this.SpeedPower);
            res.linechange();
            res.packAdd("MaxDegree", this.MaxDegree);
            res.packAdd("MaxDegreeSpeed", this.MaxDegreeSpeed);
            res.linechange();
            res.packAdd("Teikou", this.Teikou);
            res.packAdd("Gravity", this.Gravity);
            return res;

        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.Target = d.unpackDataS("Target", this.Target);
            this.BaseDegree = d.unpackDataF("BaseDegree", this.BaseDegree);
            this.SpeedPower = d.unpackDataF("SpeedPower", this.SpeedPower);
            this.MaxDegree = d.unpackDataF("MaxDegree", this.MaxDegree);
            this.MaxDegreeSpeed = d.unpackDataF("MaxDegreeSpeed", this.MaxDegreeSpeed);
            this.Teikou = d.unpackDataF("Teikou", this.Teikou);
            this.Gravity = d.unpackDataF("Gravity", this.Gravity);
        }
    }

    /// <summary>
    /// 体に追加されている全てのエンテティを利用してがんばって揺らす。
    /// 途中でパーツ構成が変わったら死ぬ
    /// </summary>
    public class CharacterYure : Component
    {
        /// <summary>
        /// ターゲットのパーツ
        /// </summary>
        public String Target = "";
        /// <summary>
        /// 通常状態を何度として扱うか
        /// </summary>
        public float BaseDegree = 0;
        /// <summary>
        /// 速度に変換する割合1未満にすること。
        /// </summary>
        public float SpeedPower = 0;
        /// <summary>
        /// 最大の角度
        /// </summary>
        public float MaxDegree = 0;
        /// <summary>
        /// 最大の回転速度
        /// </summary>
        public float MaxDegreeSpeed = 0;
        /// <summary>
        /// 速度の小さくなり具合
        /// </summary>
        public float Teikou = 0.9f;
        /// <summary>
        /// BaseDegreeに戻る力
        /// </summary>
        public float Gravity = 1;
        /// <summary>
        /// 速度。これは保存しない。
        /// </summary>
        public float DegreeSpeed = 0;

        List<FXY>prePosition = new List<FXY>();

        void SetPrePosition() 
        {
            prePosition.Clear();
            foreach (var a in e.getCharacter().BaseCharacter.getTree(""))
            {
                var b = e.getCharacter().getEntity(a.name);
                if (b != null)
                {
                    prePosition.Add(b.gettxy());
                }
            }
        }
        protected override void onupdate(float cl)
        {
            base.onupdate(cl);
            var boxs = new List<FXY>();
            foreach (var a in e.getCharacter().BaseCharacter.getTree(""))
            {
                var b = e.getCharacter().getEntity(a.name);
                if (b != null)
                {
                    boxs.Add(b.gettxy());
                }
            
            }

            //ヒットボックスの移動具合の合計がこのフレームのパワー
            FXY power = new FXY(0, 0);
            for (int i = 0; i < Mathf.min(boxs.Count, prePosition.Count); i++)
            {
                if (cl != 0)
                {
                    power -= (boxs[i] - prePosition[i]) / Mathf.min(boxs.Count, prePosition.Count) * SpeedPower / cl;
                }
            }
            var Gravoty = new FXY(Gravity, 0) * cl;
            Gravoty.degree = BaseDegree;
            power += Gravoty;
            

            float DegreeTo = power.degree - BaseDegree;

            if (DegreeTo > MaxDegree)
            {
                DegreeTo = MaxDegree;
            }
            else if (DegreeTo < -MaxDegree)
            {
                DegreeTo = -MaxDegree;
            }

            var tag = e.getCharacter().getEntity(Target);
            var Btag = e.getCharacter().BaseCharacter.getEntity(Target);
            if (tag != null&&Btag!=null)
            {
                if (Mathf.st180(DegreeTo - tag.degree) > 0)
                {
                    DegreeSpeed += power.length;
                }
                else 
                {
                    DegreeSpeed -= power.length;

                }
                DegreeSpeed *= (float)Math.Pow(1 - Teikou, cl);

                var speed = cl * Mathf.sameSign(Mathf.min(Mathf.abs(DegreeSpeed), MaxDegreeSpeed), DegreeSpeed);

                if (MaxDegree<180) 
                {
                    var tdeg = tag.degree -  Btag.degree;
                    var sptdeg = tag.degree - Btag.degree  + speed;
                    var tyouka = Mathf.abs(sptdeg) - MaxDegree + MaxDegreeSpeed*0;
                    var tyoukapre = Mathf.abs(tdeg) - MaxDegree + MaxDegreeSpeed * 0;
                    if (tyouka > 0&&tyouka>tyoukapre)//超過が小さくなる方向には動ける
                    {
                        speed -= Mathf.sameSign(tyouka,speed);
                    }
                }
                    EntityMove.XYD(1, "", 0, 0, speed)
                        .addAndRemove(tag, 1);


            }

            
            //Debug.WriteLine(power.ToString() + " YURE " + DegreeSpeed.ToString());
            prePosition = boxs;
            
        }
        public override void addtoworld(float cl = 0)
        {
            base.addtoworld(cl);
            SetPrePosition();
            DegreeSpeed = 0;
        }
        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (CharacterYure)c;
            cc.Target = this.Target;
            cc.BaseDegree = this.BaseDegree;
            cc.SpeedPower = this.SpeedPower;
            cc.MaxDegree = this.MaxDegree;
            cc.MaxDegreeSpeed = this.MaxDegreeSpeed;
            cc.Gravity = this.Gravity;
            cc.Teikou = this.Teikou;
            cc.DegreeSpeed = this.DegreeSpeed;
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.packAdd("Target", this.Target);
            res.linechange();
            res.packAdd("BaseDegree", this.BaseDegree);
            res.packAdd("SpeedPower", this.SpeedPower);
            res.linechange();
            res.packAdd("MaxDegree", this.MaxDegree);
            res.packAdd("MaxDegreeSpeed", this.MaxDegreeSpeed);
            res.linechange();
            res.packAdd("Teikou", this.Teikou);
            res.packAdd("Gravity", this.Gravity);
            return res;

        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.Target = d.unpackDataS("Target", this.Target);
            this.BaseDegree = d.unpackDataF("BaseDegree", this.BaseDegree);
            this.SpeedPower = d.unpackDataF("SpeedPower", this.SpeedPower);
            this.MaxDegree = d.unpackDataF("MaxDegree", this.MaxDegree);
            this.MaxDegreeSpeed = d.unpackDataF("MaxDegreeSpeed", this.MaxDegreeSpeed);
            this.Teikou = d.unpackDataF("Teikou", this.Teikou);
            this.Gravity = d.unpackDataF("Gravity", this.Gravity);
        }
    }
}
