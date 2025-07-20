using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charamaker3.CharaModel;

namespace Charamaker3.Utils
{
    /// <summary>
    /// 時間が来たら勝手に消える性質をエンテティに追加する。
    /// </summary>
    public class LifeTimer : Component
    {
        bool removeFromEntity = false;
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name"></param>
        /// <param name="removefromEntity">このコンポーネントもエンテティとともに消えるか</param>
        public LifeTimer(float time, string name = "", bool removefromEntity = false) : base(time, name)
        {
            this.removeFromEntity = removefromEntity;
        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public LifeTimer() { }

        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (LifeTimer)c;
            cc.removeFromEntity = this.removeFromEntity;
        }
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.linechange();
            d.packAdd("removeFromEntity", removeFromEntity);
            return d;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);

            this.removeFromEntity = d.unpackDataB("removeFromEntity");
        }

        public override bool eternal { get { return base.eternal || (removeFromEntity == false); } }
        protected override void onupdate(float cl)
        {
            base.onupdate(cl);
            if (remaintime <= 0)
            {
                e.remove();
            }

        }


    }

    /// <summary>
    /// 時間が来たら(エンテティからリムーブされたら)エンテティを追加するコンポーネント<br></br>
    /// 召喚する位置はonupdatedとかで操作するといいんじゃないかな。
    /// </summary>
    public class SummonEntity : Component
    {
        /// <summary>
        /// エンテティを生み出したときにそのエンテティに対して発動するイベント
        /// </summary>
        public event EventHandler<Entity> onSummon;
        /// <summary>
        /// 召喚するエンテティ
        /// </summary>
        public Entity summon=new Entity();

        public float LifeTimer = 0;

        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name"></param>
        /// <param name="lifetimer">-1で無限</param>
        public SummonEntity(Entity summonentity, float time, string name = "", float lifetimer = -1) : base(time, name)
        {
            summon = summonentity;

            this.LifeTimer = lifetimer;
        }
        /// <summary>
        /// 召喚した際の位置Nanで中心位置
        /// </summary>
        public float dxp = float.NaN;

        /// <summary>
        /// 召喚した際の位置Nanで中心位置
        /// </summary>
        public float dyp = float.NaN;


        /// <summary>
        /// 召喚した際の大きさ割合でNanでそのまんま。割と雰囲気
        /// </summary>
        public float sizep = float.NaN;

        /// <summary>
        /// 召喚した際のzの割合 -だったら下に行っちゃうからちうい。Nanでそのまんま
        /// </summary>
        public float dz = float.NaN;

        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public SummonEntity() { }

        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (SummonEntity)c;
            cc.summon = this.summon.clone();
            cc.dxp=this.dxp;
            cc.dyp =this.dyp;
            cc.sizep = this.sizep;
            cc.dz = this.dz;
        }
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.linechange();
            var dd=summon.ToSave();
            dd.indent();
            d.packAdd("summon",dd);
            d.packAdd("dxp", dxp);
            d.packAdd("dyp", dyp);
            d.packAdd("sizep", sizep);
            d.packAdd("dz", dz);
            return d;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            var dd=d.unpackDataD("summon");
            this.summon=Entity.ToLoadEntity(dd);

            dxp=d.unpackDataF("dxp", dxp);
            dyp = d.unpackDataF("dyp", dyp);
            sizep = d.unpackDataF("sizep", sizep);
            dz = d.unpackDataF("dz", dz);
        }
        protected override void onremove(float cl)
        {
            base.onremove(cl);
            DoSummon(e,cl);

        }
        virtual protected void DoSummon(Entity e,float cl) 
        {
            var summons = summon;

            onSummon?.Invoke(this, summons);
            if (float.IsNaN(sizep) == false)
            {
                float motosize = (summon.w + summon.h) / 2;
                if (motosize != 0)
                {
                    float size = (e.w + e.h) / 2*sizep;
                    EntityMove.ScaleChange(10, "", size / motosize, size / motosize).addAndRemove(summons, 100);

                    //summons.getCharacter().SetBaseCharacter();
                }
            }
            FXY setxy = summon.gettxy();
            if (float.IsNaN(dxp) == false)
            {
                setxy.x = e.gettxy2(dxp, 0).x + (e.gettxy2().x - e.gettxy2(0,0).x);
            }
            if (float.IsNaN(dyp) == false)
            {
                setxy.y = e.gettxy2(0, dyp).y + (e.gettxy2().y - e.gettxy2(0, 0).y);
            }
            summon.settxy(setxy);
            if(float.IsNaN(dz) == false)
            {
                DrawableMove.ZDeltaChange(0,"", e.getDrawable<Drawable>().zDelta, e.getDrawable<Drawable>().zRatio).addAndRemove(summon, 100);
                DrawableMove.ZChange(0,"", e.getDrawable<Drawable>().z+dz).addAndRemove(summon, 100);
                
            }
            if (LifeTimer >= 0)
            {
                new Utils.LifeTimer(LifeTimer).add(summon);
            }

            summons.add(world);
            summons.update(cl);


        }
    }

    public class SummonSerif : SummonEntity 
    {
        string tag;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag">召喚に対応した部位</param>
        /// <param name="summonentity"></param>
        /// <param name="time"></param>
        /// <param name="lifetime"></param>
        /// <param name="name"></param>
        public SummonSerif(string tag, Serif summonentity, float time, float lifetime ,string name = "") : base(summonentity, time,name,lifetime)
        {
            this.tag = tag;
        }
        protected override void DoSummon(Entity e, float cl)
        {
            var tagE=e.getCharacter().getEntity(tag);
            if (tagE != null)
            {
                ((Serif)summon).Target = tagE;
                base.DoSummon(tagE, cl);
            }
            else
            {
                ((Serif)summon).Target = e;
                base.DoSummon(e, cl);
            }
        }
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.packAdd("tag",tag);
            d.linechange();
            return d;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            tag= d.unpackDataS("tag", tag);
        }
        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (SummonSerif)c;
            cc.tag = this.tag;
        }
    }
    /// <summary>
    /// 時間が来たら(エンテティからリムーブされたら)エンテティを追加するコンポーネント<br></br>
    /// 召喚する位置はonupdatedとかで操作するといいんじゃないかな。
    /// </summary>
    public class SummonCharacter : Component
    {
        /// <summary>
        /// エンテティを生み出したときにそのエンテティに対して発動するイベント
        /// </summary>
        public event EventHandler<Entity> onSummon;
        /// <summary>
        /// 召喚するキャラクターのパス
        /// </summary>
        string charaPath="";

        float scale = 1;
        float x=0,y=0;
        string charaName="";
        public float LifeTime = 0;
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="path">キャラクターのパス</param>
        /// <param name="scale">召喚したキャラクターの大きさ</param>
        /// <param name="charaname">召喚したキャラクターに付ける名前</param>
        /// <param name="time"></param>
        /// <param name="name"></param>
        public SummonCharacter(string path, float scale, string charaname, float time, string name = "", float lifetime=0) : base(time, name)
        {
            charaPath = path;
            this.scale= scale;
            this.charaName = charaname;
            this.LifeTime = lifetime;
        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public SummonCharacter() { }



        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (SummonCharacter)c;
            cc.charaPath = this.charaPath;
            cc.scale = this.scale;
            cc.charaName = this.charaName;
        }
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.linechange();
            d.packAdd("charaPath", charaPath);
            d.packAdd("scale", scale);
            d.packAdd("charaName", charaName);
            return d;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.charaPath = d.unpackDataS("charaPath");
            this.scale = d.unpackDataF("scale");
            this.charaName = d.unpackDataS("charaName"); ;
        }
        protected override void onremove(float cl)
        {
            base.onremove(cl);
            var c = FileMan.ldCharacter(charaPath);

            new Utils.LifeTimer(LifeTime,"").add(c.e);


            c.e.settxy(0, 0);
            
            onSummon?.Invoke(this, c.e);

            Character.SetupCharacter(c.e, charaName,scale,1,1);

            c.e.add(world);
            c.e.update(cl);
        }
    }
    /// <summary>
    /// 時間が来たら(エンテティからリムーブされたら)コンポーネントを追加するコンポーネント。<br></br>
    /// 追加する対象は""で自分、"name"でその名前で初めて見つかったやつ、"name/partname"でcharacterの関節を検索できる。
    /// </summary>
    public class SummonComponent : Component
    {
        /// <summary>
        /// コンポーネントを生み出したときにそのコンポーネントに対して発動するイベント
        /// </summary>
        public event EventHandler<Component> onSummon;
        /// <summary>
        /// 召喚するエンテティ
        /// </summary>
        public Component summon=new Component();

        /// <summary>
        /// コンポーネントを追加するターゲット。
        /// </summary>
        protected string tag="";

        /// <summary>
        /// addしたときにフレームする
        /// </summary>
        protected float addCl=0;
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name"></param>
        /// <param name="addcl">addしたときにするフレーム</param>
        public SummonComponent(Component summoncomponent,string targetname, float time, float addcl=0, string name = "") : base(time, name)
        {
            summon = summoncomponent;
            tag= targetname;
            this.addCl = addcl;
        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public SummonComponent() { }

        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (SummonComponent)c;
            cc.summon = this.summon.clone();
            cc.tag = this.tag;
            cc.addCl= this.addCl;
        }
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.linechange();
            d.packAdd("tag", tag);
            d.packAdd("addCl", addCl);
            d.linechange();
            var dd = summon.ToSave();
            dd.indent();
            d.packAdd("summon", dd);
            return d;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            tag = d.unpackDataS("tag");
            addCl = d.unpackDataF("addCl", 0);
            var dd = d.unpackDataD("summon");
            this.summon = Component.ToLoadComponent(dd);
        }
        public override void update(float cl)
        {
            base.update(cl);
        }
        protected override void onremove(float cl)
        {
            base.onremove(cl);
            if (tag == "")
            {
                onSummon?.Invoke(this, summon);
                summon.add(e,addCl+cl);
            }
            else  
            {
                var lis = world.Entities;
                var tags = tag.Split('/');
                // '/'で分解して、ひとつづつ奥へ潜っていく。初めはworldそれ以降はキャラクター。該当者なしなら何もせん。
                for (int i=0;i<tags.Count();i++) 
                {
                    var a = tags[i];
                    var named=World.getNamedEntity(a, lis);
                    //Debug.WriteLine(a+" search "+named.Count);
                    if (named.Count == 0)
                    {
                        break;
                    }
                    else if (i >= tags.Count() - 1)
                    {
                        onSummon?.Invoke(this, summon);
                        summon.add(named[0],addCl+cl);
                    }
                    else 
                    {
                        var cs=named[0].getcompos<CharaModel.Character>();
                        if (cs.Count == 0)
                        {
                            break;
                        }
                        else 
                        {
                            lis = cs[0].getTree("");
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 時間が来たら(エンテティからリムーブされたら)エンテティを追加するコンポーネント<br></br>
    /// 召喚する位置はonupdatedとかで操作するといいんじゃないかな。
    /// </summary>
    public class SummonMotion : Component
    {
        /// <summary>
        /// コンポーネントを生み出したときにそのコンポーネントに対して発動するイベント
        /// </summary>
        public event EventHandler<Component> onSummon;
        /// <summary>
        /// 召喚するキャラクターのパス
        /// </summary>
        string motionPath="";

        float speed = 1;
        float addCl=0;
        /// <summary>
        /// モーションを追加するターゲット。
        /// </summary>
        protected string tag = "";
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="path">モーションのパス</param>
        /// <param name="speed"></param>
        /// <param name="targetname">対象の名前</param>
        /// <param name="time"></param>
        /// <param name="name"></param>
        /// <param name="addcl">addしたときにするフレーム</param>
        public SummonMotion(string path, float speed, string targetname, float time,float addcl=0, string name = "") : base(time, name)
        {
            motionPath = path;
            tag = targetname;
            this.speed = speed;
            this.addCl=addcl;
        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public SummonMotion() { }

        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (SummonMotion)c;
            cc.speed = this.speed;
            cc.motionPath = this.motionPath;
            cc.tag = this.tag;
            cc.addCl = this.addCl;
        }
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.linechange();
            d.packAdd("tag", tag);
            d.packAdd("motionPath", motionPath);
            d.packAdd("speed", speed);
            d.packAdd("addCl", addCl);
            return d;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            tag = d.unpackDataS("tag");
            motionPath = d.unpackDataS("motionPath");
            speed = d.unpackDataF("speed");
            addCl = d.unpackDataF("addCl");
        }
        protected override void onremove(float cl)
        {
            base.onremove(cl);
            var summon = FileMan.ldMotion(motionPath);
            summon.speed= this.speed;
            if (tag == "")
            {
                onSummon?.Invoke(e, summon);
                summon.add(e,addCl + cl);
            }
            else
            {
                var lis = world.Entities;
                var tags = tag.Split('/');
                // '/'で分解して、ひとつづつ奥へ潜っていく。初めはworldそれ以降はキャラクター。該当者なしなら何もせん。
                for (int i = 0; i < tags.Count(); i++)
                {
                    var a = tags[i];
                    var named = World.getNamedEntity(a, lis);
                    if (named.Count == 0)
                    {
                        break;
                    }
                    else if (i >= tags.Count() - 1)
                    {
                        onSummon?.Invoke(e, summon);
                        summon.add(named[0],addCl+cl);
                        break;
                    }
                    else
                    {
                        var cs = named[0].getcompos<CharaModel.Character>();
                        if (cs.Count == 0)
                        {
                            break;
                        }
                        else
                        {
                            lis = cs[0].getTree("");
                        }
                    }
                }
            }
        }
    }
    /// <summary>
    /// 終わった時にエンテティ中のフリーイベントを発動させるコンポーネント
    /// </summary>
    public class FreeEventer :Component
    {
        string freeEventName="";

        public FreeEventer(float timer, string eventName) : base(timer) 
        {
            freeEventName = eventName;
        }
        public FreeEventer() { }

        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (FreeEventer)c;
            cc.freeEventName = freeEventName;
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.linechange();
            res.packAdd("freeEventName", freeEventName);
            return res;

        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.freeEventName = d.unpackDataS("freeEventName", this.freeEventName);
        }
        protected override void onremove(float cl)
        {
            base.onremove(cl);
            foreach (var a in e.components) 
            {
                a.freeevent(freeEventName);
            }
        }

    }
    /// <summary>
    /// セリフの表示のエンテティ。
    /// </summary>
    public class Serif:Entity 
    {
        public Character c { get { return getCharacter(); } }
        public Entity TextE { get { return getCharacter().getEntity("Text"); } }
        public Text Text { get { return TextE.getDrawable<Text>(); } }
        public Entity BodyE { get { return getCharacter().getEntity("Body"); } }
        public Entity WakuE { get { return getCharacter().getEntity("Waku"); } }

        public Joint BodyJ { get { return getCharacter().getParentJoint("Body"); } }
        public Joint WakuJ { get { return getCharacter().getParentJoint("Waku"); } }

        public Joint TagJ { get { return getCharacter().getParentJoint("WTag"); } }

        public Entity WTagE { get { return getCharacter().getEntity("WTag"); } }

        public Entity BTagE { get { return getCharacter().getEntity("BTag"); } }

        /// <summary>
        /// 枠の幅
        /// </summary>
        public float WakuHaba = 0;

        /// <summary>
        /// 白い部分の大きさ
        /// </summary>
        public float SiroSiro = 4;

        public Entity Target=null;

        bool CanRend = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">セリフを表示する枠の大きさ(whだけコピー)</param>
        /// <param name="t">セリフのテキスト</param>
        /// <param name="dis">表示するディスプレー(初期化に使う。)</param>
        /// <returns></returns>
        static public Serif MakeSerif(Entity e, Text t, Display dis = null)
        {
            var res = new Serif();
            e.copy(res);
            Entity Body = new Entity();
            Body.name = "Body";
            new DRectangle(0.01f, new ColorC(1, 1, 1, 1)).add(Body);

            Entity Text = new Entity();
            e.copy(Text);
            Text.name = "Text";
            t.z = 0.05f;
            t.add(Text);

            Entity Waku = new Entity();
            Waku.name = "Waku";
            new DRectangle(0.00f, new ColorC(0, 0, 0, 1)).add(Waku);


            Entity WTag = new Entity();
            WTag.name = "WTag";
            new DTriangle(0.5f, 0.00f, new ColorC(0, 0, 0, 1)).add(WTag);

            Entity BTag = new Entity();
            BTag.name = "BTag";
            new DTriangle(0.5f, 0.01f, new ColorC(1, 1, 1, 1)).add(BTag);

            var c = new Character();
            c.addJoint(new Joint("TextJoi", 0.5f, 0.5f, null, new List<Entity> { Text }));
            c.addJoint(new Joint("BodyJoi", 0.0f, 0.0f, Text, new List<Entity> { Body }));
            c.addJoint(new Joint("WakuJoi", 0.0f, 0.0f, Text, new List<Entity> { Waku }));
            c.addJoint(new Joint("TagJoi", 0.0f, 0.0f, Text, new List<Entity> { WTag, BTag }));

            EntityMove.ScaleChange(10, "", 10, 10).addAndRemove(Body, 100);
            EntityMove.ScaleChange(10, "", 10, 10).addAndRemove(Waku, 100);
            EntityMove.ScaleChange(10, "", 10, 10).addAndRemove(WTag, 100);
            EntityMove.ScaleChange(10, "", 10, 10).addAndRemove(BTag, 100);

            c.add(res);
            c.assembleCharacter();
            c.SetBaseCharacter();

            if (dis != null)
            {
                t.MakeTrender(dis);
            }
            else 
            {
                res.CanRend = false;
                DrawableMove.BaseColorChange(10,"",0).addAndRemove(res,100);
            }

                return res;
        }

        protected void SetSerifWaku() 
        {
            var t = Text;
            var te = TextE;
            var be = BodyE;
            var we = WakuE;
            var bj = BodyJ;
            var wj = WakuJ;
            var Wtge = WTagE;
            var Btge = BTagE;
            var tgj = TagJ;

            if (CanRend) 
            {
                Text.SetRayout();
            }
            if (t.Right != 0 && CanRend)
            {
                be.w = Math.Max(te.w * t.Right, te.h * t.Bottom) + SiroSiro * 2;
                we.w = be.w + (WakuHaba) * 2;
                switch (t.font.ali)
                {
                    case FontC.alignment.left:

                        bj.px = 0;
                        wj.px = 0;

                        be.tx = SiroSiro;
                        we.tx = WakuHaba + SiroSiro;
                        break;
                    case FontC.alignment.center:

                        bj.px = 0.5f;
                        wj.px = 0.5f;

                        be.tx = be.w / 2;//+ SiroSiro;
                        we.tx = we.w / 2;// + WakuHaba + SiroSiro * 2;


                        break;
                    case FontC.alignment.right:
                        break;
                    case FontC.alignment.justify:
                        break;
                    case FontC.alignment.None:
                        break;
                }

                be.h = te.h * t.Bottom + (SiroSiro) * 2;
                we.h = be.h + (WakuHaba) * 2;
                switch (t.font.aliV)
                {
                    case FontC.alignment.left:
                        bj.py = 0; wj.py = 0;
                        be.ty = SiroSiro; we.ty = WakuHaba + SiroSiro;
                        break;
                    case FontC.alignment.center:
                        bj.py = 0.5f; wj.py = 0.5f;
                        be.ty = be.h / 2; we.ty = we.h / 2;
                        break;
                    case FontC.alignment.right:
                        bj.py = 1.0f; wj.py = 1.0f;
                        be.ty = be.h - SiroSiro; we.ty = we.h - (WakuHaba + SiroSiro);
                        break;
                    case FontC.alignment.None:
                        break;
                }
            }
            else
            {
                be.w = 0;
                we.w = 0;
                be.h = 0;
                we.h = 0;
            }
            getCharacter().assembleCharacter();

            if (Target != null && t.Right != 0)
            {
                var dxy = Target.gettxy() - Wtge.gettxy();
                dxy.length -= Target.bigs / 2;

                Wtge.w = dxy.length;
                Wtge.h = Mathf.min(Mathf.sqrt(we.w * we.w / 4 + we.h * we.h / 4), we.w, we.h) * 0.75f;

                Wtge.tx = Wtge.h / 2 * 0;
                Wtge.ty = Wtge.h / 2;
                Wtge.degree = dxy.degree;

                Btge.w = dxy.length - WakuHaba;
                Btge.h = Wtge.h - WakuHaba * 2;

                Btge.tx = Btge.h / 2 * 0;
                Btge.ty = Btge.h / 2;
                Btge.degree = dxy.degree;

                switch (t.font.ali)
                {
                    case FontC.alignment.left:
                        tgj.px = (Wtge.h / 2) / tgj.parent.w;
                        break;
                    case FontC.alignment.center:
                        tgj.px = 0.5f;
                        break;
                }
                switch (t.font.aliV)
                {
                    case FontC.alignment.left:
                        tgj.py = t.Bottom / 2;
                        break;
                    case FontC.alignment.center:
                        tgj.py = 0.5f;
                        break;
                    case FontC.alignment.right:
                        tgj.py = 1 - t.Bottom / 2;
                        break;
                }
            }
            else
            {
                Wtge.w = 0;
                Wtge.h = 0;

                Btge.w = 0;
                Btge.h = 0;
            }
            getCharacter().assembleCharacter();
        }

        public override void update(float cl)
        {



            base.update(cl);
            SetSerifWaku(); SetSerifWaku();
        }
        public override void AfterDrawUpdate()
        {
            //TODO:初Frameのとき、おかしくなるので透明にして解除する仕組み
            if (CanRend == false)
            {
                DrawableMove.BaseColorChange(10, "", 1).addAndRemove(this, 100);
                CanRend = true;
            }
            base.AfterDrawUpdate();
        }
       
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.packAdd("WakuHaba", WakuHaba);
            res.packAdd("SiroSiro", SiroSiro);
            res.linechange();
            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            WakuHaba = d.unpackDataF("WakuHaba", WakuHaba);
            SiroSiro = d.unpackDataF("SiroSiro", SiroSiro);
        }
        public override void copy(Entity e)
        {
            base.copy(e);
            var ee = (Serif)e;

            ee.Target = this.Target;
            ee.WakuHaba = this.WakuHaba;
            ee.SiroSiro = this.SiroSiro;
        }
    }
}
