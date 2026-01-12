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


            float DegreeTo = power.degree  - BaseDegree;

            if (DegreeTo > MaxDegree)
            {
                DegreeTo = MaxDegree;
            }
            else if (DegreeTo < -MaxDegree)
            {
                DegreeTo = -MaxDegree;
            }


            var parent = e.getCharacter().getParent(Target);
            var tag = e.getCharacter().getEntity(Target);
            var Btag = e.getCharacter().BaseCharacter.getEntity(Target);
            if (parent!=null&&tag != null && Btag != null)
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
                    var tdeg = tag.degree - Btag.degree + parent.degree;
                    var sptdeg = tag.degree - Btag.degree + speed + parent.degree;
                    var tyouka = Mathf.abs(sptdeg - parent.degree) - (MaxDegree) + MaxDegreeSpeed * 0;
                    var tyoukapre = Mathf.abs(tdeg - parent.degree) - (MaxDegree) + MaxDegreeSpeed * 0;
                    if (tyouka > 0 && tyouka > tyoukapre)//超過が小さくなる方向には動ける
                    {
                        speed -= Mathf.sameSign(tyouka, speed);
                    }
                }
                
                EntityMove.XYD(1, "", 0, 0, speed * FileMan.plusminus(tag.mirror, false))
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
        public string Target = "";
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

            var parent = e.getCharacter().getParent(Target);
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
                    var tyouka = Mathf.abs(sptdeg - parent.degree) - (MaxDegree) + MaxDegreeSpeed * 0;
                    var tyoukapre = Mathf.abs(tdeg - parent.degree) - (MaxDegree) + MaxDegreeSpeed * 0;

                    if (tyouka > 0 && tyouka > tyoukapre)//超過が小さくなる方向には動ける
                    {
                        speed -= Mathf.sameSign(tyouka, speed);
                    }
                }

                EntityMove.XYD(1, "", 0, 0, speed * FileMan.plusminus(tag.mirror, false))
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
    public class PartsYure : Component
    {
        /// <summary>
        /// パーツをゆらゆらさせる対象
        /// </summary>
        public string Tag=""; 
        /// <summary>
        /// 速度
        /// </summary>
        public float DegreeSpeed = 0;
        /// <summary>
        /// whoとかの変化にかかる時間(つまり速度)
        /// </summary>
        public float WhoTime = 0;
        /// <summary>
        /// float Mill
        /// </summary>
        public FloatMill Degree = new FloatMill(), W = new FloatMill(), H = new FloatMill(), Opa = new FloatMill();

        public PartsYure() 
        { 
           
        }
        protected override void onadd(float cl)
        {
            base.onadd(cl);
            Degree.reset();
            W.reset();
            H.reset();
            Opa.reset();
        }
        protected override void onupdate(float cl)
        {
            base.onupdate(cl);

            Degree.update(cl);
            W.update(cl);
            H.update(cl);
            Opa.update(cl);

            if (e.getCharacter().HasBaseCharacter == true)
            {
                EntityMove.RotateToLim(cl, Tag, Degree, DegreeSpeed).addAndRemove(e, cl);
                EntityMove.ScaleChange(WhoTime, Tag, W, H).addAndRemove(e, cl);
                DrawableMove.BaseColorChange(WhoTime, Tag, Opa).addAndRemove(e, cl);
            }
        }
        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (PartsYure)c;
            cc.Tag = this.Tag;
            cc.DegreeSpeed = this.DegreeSpeed;
            cc.WhoTime = this.WhoTime;
            
            this.Degree.paste(cc.Degree);
            this.W.paste(cc.W);
            this.H.paste(cc.H);
            this.Opa.paste(cc.Opa);

        }

        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.packAdd("Tag", Tag);
            d.packAdd("DegreeSpeed", DegreeSpeed);
            d.packAdd("WhoTime", WhoTime);
            d.linechange();

            d.packAdd("Degree",Degree.ToSave());
            d.linechange();

            d.packAdd("W", W.ToSave());
            d.linechange();

            d.packAdd("H", H.ToSave());
            d.linechange();

            d.packAdd("Opa", Opa.ToSave());
            d.linechange();

            return d;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            Tag = d.unpackDataS("Tag", Tag);
            DegreeSpeed = d.unpackDataF("DegreeSpeed", DegreeSpeed);
            WhoTime = d.unpackDataF("WhoTime", WhoTime);

            Degree = new FloatMill(d.unpackDataD("Degree"));

            W = new FloatMill(d.unpackDataD("W"));

            H = new FloatMill(d.unpackDataD("H"));

            Opa = new FloatMill(d.unpackDataD("Opa"));


        }

    }
    /// <summary>
    /// 体に追加されている全てのエンテティを利用してがんばって伸ばす
    /// 途中でパーツ構成が変わったら死ぬ
    /// </summary>
    public class CharacterNobasi : Component
    {
        /// <summary>
        /// ターゲットのパーツ
        /// </summary>
        public String Target = "";
        /// <summary>
        /// 対象の回転をのパワー
        /// </summary>
        public float DegreePower = 0;
        /// <summary>
        /// 速度に変換する割合1未満にすること。
        /// </summary>
        public float SpeedPower = 0;
        /// <summary>
        /// 最大の角度
        /// </summary>
        public float MaxScale = 1;
        /// <summary>
        /// 最大の回転速度
        /// </summary>
        public float MaxScaleSpeed = 0;
        /// <summary>
        /// 速度の小さくなり具合
        /// </summary>
        public float Teikou = 0.9f;
        /// <summary>
        /// Scale1に戻る力
        /// </summary>
        public float Gravity = 1;
        /// <summary>
        /// 速度。これは保存しない。
        /// </summary>
        public float SpeedW = 0,SpeedH = 0;

        List<FXY> prePosition = new List<FXY>();
        float preDegree = float.NaN;

        void SetPrePosition()
        {
            preDegree = float.NaN;
            prePosition.Clear();
            foreach (var a in e.getCharacter().BaseCharacter.getTree(""))
            {
                var b = e.getCharacter().getEntity(a.name);
                if (b != null)
                {
                    prePosition.Add(b.gettxy());
                }
            }

            var tag = e.getCharacter().getEntity(Target);
            if (tag != null) 
            {
                preDegree = tag.degree;
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
            


            var parent = e.getCharacter().getParent(Target);
            var tag = e.getCharacter().getEntity(Target);
            var Btag = e.getCharacter().BaseCharacter.getEntity(Target);
            if (tag != null && Btag != null)
            {
                var TruePower = FXY.bylength(power.length, power.degree + tag.degree);
                if (float.IsNaN(preDegree) == false)
                {
                    if (cl != 0)
                    {
                        var dp = Mathf.abs(tag.degree - preDegree) * DegreePower / cl;
                        TruePower.length += dp / 2;
                        TruePower.x = dp / 2;
                        TruePower.y = dp / 2;
                    }
                }
                SpeedW += Math.Abs(TruePower.x);
                SpeedH += Math.Abs(TruePower.y);

              

                if (SpeedW > 0)
                {
                    SpeedW -= Gravity * cl;
                    
                }
                if (SpeedH > 0)
                {
                    SpeedH -= Gravity * cl;
                }

                SpeedW *= (1-Teikou);
                SpeedH *= (1-Teikou);

                SpeedW = Mathf.clamp(SpeedW,-MaxScaleSpeed,MaxScale);
                SpeedH = Mathf.clamp(SpeedH, -MaxScaleSpeed, MaxScale);

                var mokuW = tag.w / Btag.w + SpeedW * cl;
                var mokuH = tag.h / Btag.h + SpeedH * cl;

                mokuW = Mathf.clamp(mokuW, 1, MaxScale);
                mokuH = Mathf.clamp(mokuH, 1, MaxScale);

                EntityMove.ScaleChange(1, Target, mokuW, mokuH).addAndRemove(e, 1);
                //Debug.WriteLine(tag.w +"/"+Btag.w + " = "+(tag.w / Btag.w) +" Nobasi " + SpeedW*cl + "::" + mokuW);
                //Debug.WriteLine(power.ToString() + " Nobasi " + SpeedW + "::" + mokuW);

            }


            prePosition = boxs;

        }
        public override void addtoworld(float cl = 0)
        {
            base.addtoworld(cl);
            SetPrePosition();
            SpeedW = 0;
            SpeedH = 0;
        }
        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (CharacterNobasi)c;
            cc.Target = this.Target;
            cc.DegreePower = this.DegreePower;
            cc.SpeedPower = this.SpeedPower;
            cc.MaxScale = this.MaxScale;
            cc.MaxScaleSpeed = this.MaxScaleSpeed;
            cc.Gravity = this.Gravity;
            cc.Teikou = this.Teikou;
            cc.SpeedW = this.SpeedW;
            cc.SpeedH = this.SpeedH;
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.packAdd("Target", this.Target);
            res.linechange();
            res.packAdd("DegreePower", this.DegreePower);
            res.packAdd("SpeedPower", this.SpeedPower);
            res.linechange();
            res.packAdd("MaxScale", this.MaxScale);
            res.packAdd("MaxScaleSpeed", this.MaxScaleSpeed);
            res.linechange();
            res.packAdd("Teikou", this.Teikou);
            res.packAdd("Gravity", this.Gravity);
            return res;

        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.Target = d.unpackDataS("Target", this.Target);
            this.DegreePower = d.unpackDataF("DegreePower", this.DegreePower);
            this.SpeedPower = d.unpackDataF("SpeedPower", this.SpeedPower);
            this.MaxScale = d.unpackDataF("MaxScale", this.MaxScale);
            this.MaxScaleSpeed = d.unpackDataF("MaxScaleSpeed", this.MaxScaleSpeed);
            this.Teikou = d.unpackDataF("Teikou", this.Teikou);
            this.Gravity = d.unpackDataF("Gravity", this.Gravity);
        }
    }
}
