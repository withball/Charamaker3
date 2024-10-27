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
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name"></param>
        public SummonEntity(Entity summonentity,float time, string name = "") : base(time, name)
        {
            summon = summonentity;
        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public SummonEntity() { }

        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (SummonEntity)c;
            cc.summon = this.summon.clone();
        }
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.linechange();
            var dd=summon.ToSave();
            dd.indent();
            d.packAdd("summon",dd);
            return d;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            var dd=d.unpackDataD("summon");
            this.summon=Entity.ToLoadEntity(dd);
        }
        protected override void onremove(float cl)
        {
            base.onremove(cl);
            var summons = summon;
            onSummon?.Invoke(this,summons);
            summons.add(world);
            summons.update(cl);
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
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="path">キャラクターのパス</param>
        /// <param name="scale">召喚したキャラクターの大きさ</param>
        /// <param name="charaname">召喚したキャラクターに付ける名前</param>
        /// <param name="time"></param>
        /// <param name="name"></param>
        public SummonCharacter(string path,float scale,string charaname, float time, string name = "") : base(time, name)
        {
            charaPath = path;
            this.scale= scale;
            this.charaName = charaname; 
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
            EntityMove.ScaleChange(10, "", scale, scale, scale, scale).add(c.e,100);
            EntityMove.ScaleChange(10, "", scale, scale, scale, scale).add(c.BaseCharacter.e, 100);
            c.e.name = charaName;
            c.BaseCharacter.e.name= charaName;
            e.settxy(0, 0);
            onSummon?.Invoke(this, c.e);
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
}
