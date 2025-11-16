using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charamaker3.Shapes;

namespace Charamaker3.Hitboxs
{
    public enum HitType 
    {
        enter//今当たり始めた
            ,ing//当たり中
            ,end//当たり終わった。
    }

    /// <summary>
    /// 一つ以上追加しない方が身のためだぞ<br></br>
    /// 当たり判定をするコンポーネント当たり判定は一斉に処理される。<br></br>
    /// システムに食い込んでるタイプのコンポーネント。エンジン側で結構いじられる。<br></br>
    /// </summary>
    public class Hitbox:Component
    {
        /// <summary>
        /// ヒットする図形。SetHitboxPosition()でセットする
        /// </summary>
        public Shape HitShape;
        /// <summary>
        /// 1フレーム前の図形。SetHitboxPosition()でセットする
        /// </summary>
        public Shape preHitShape;



        /// <summary>
        /// 自身についている属性。intだけど、enumを入れたほうがいい。無しで当たり判定しない。
        /// </summary>
        public List<int> tag=new List<int>();

        /// <summary>
        /// これがないならぶつからないってタグ。空にするとすべてに対してぶつかる。フィルターは後でかけてもいいけど、ここでやったほうが軽い
        /// </summary>
        public List<int> tagfilter=new List<int>();


        /// <summary>
        /// ぶつかったと判断されたやつ。Worldが勝手にいじる。
        /// </summary>
        public List<WeakReference<Entity>>Hitteds= new List<WeakReference<Entity>>();

        public void AddHitteds (Entity e)
        {
            Hitteds.Add(new WeakReference<Entity>(e));
        }

        public bool HittedsContains(Entity e) 
        {
            for(int i=Hitteds.Count-1;i>=0;--i)
            {
                var a = Hitteds[i];
                Entity te;
                if (a.TryGetTarget(out te))
                {
                    if (te == e)
                    {
                        return true;
                    }
                }
                else 
                {
                Hitteds.RemoveAt(i);
                }
            }
            return false;
        }
        public List<Entity> GetHitteds()
        {
            var res=new List<Entity>();
            for (int i = Hitteds.Count - 1; i >= 0; --i)
            {
                var a = Hitteds[i];
                Entity te;
                if (a.TryGetTarget(out te))
                {
                    if (te == e)
                    {
                        res.Add(te);
                    }
                }
                else
                {
                    Hitteds.RemoveAt(i);
                }
            }
            return res;
        }
        /// <summary>
        /// 一フレーム前のぶつかったと判断されたやつ。Worldが勝手にいじる。
        /// </summary>
        public List<WeakReference<Entity>> preHitteds = new List<WeakReference<Entity>>();
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="HitShape">Enttyにはめる図形の形</param>
        /// <param name="tag">当たり判定のタグ</param>
        /// <param name="filter">これがないならぶつからないってタグ</param>
        /// <param name="name">名前</param>
        /// <param name="time">何フレームで消えるか。-1で無限</param>
        public Hitbox(Shape HitShape,List<int>tag,List<int>filter,string name="",float time=-1) : base(time,name) 
        {
            this.HitShape = HitShape;

            this.preHitShape = HitShape.clone();
            this.tag = new List<int>(tag);
            this.tagfilter = new List<int>(filter);
        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public Hitbox() { }
        protected override void onadd(float cl)
        {
            base.onadd(cl);
        }
        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (Hitbox)c;
            cc.tag = new List<int>(this.tag);
            cc.tagfilter = new List<int>(this.tagfilter);

            cc.HitShape = this.HitShape != null ? this.HitShape.clone() : null;
            cc.preHitShape = this.preHitShape != null ? this.preHitShape.clone() : null;
        }
        public override DataSaver ToSave()
        {
            var res= base.ToSave();
            res.linechange();
            var hitD = HitShape.ToSave();
            hitD.indent();
            res.packAdd("hit",hitD);
            res.linechange();
            var tagD = new DataSaver();
            for (int i = 0; i < tag.Count; i++) 
            {
                tagD.packAdd(i.ToString(), tag[i]);
            }
            tagD.indent();
            res.packAdd("tag", tagD);

            var filterD = new DataSaver();
            for (int i = 0; i < tagfilter.Count; i++)
            {
                filterD.packAdd(i.ToString(), tagfilter[i]);
            }
            filterD.indent();
            res.packAdd("filter", filterD);
            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            HitShape = Shape.ToLoadShape(d.unpackDataD("HitShape"));
            tag.Clear();
            var tagD = d.unpackDataD("tag");
            foreach (var a in tagD.getAllPacks()) 
            {
                tag.Add((int)tagD.unpackDataF(a));
            }
            tagfilter.Clear();
            var filterD = d.unpackDataD("filter");
            foreach (var a in filterD.getAllPacks())
            {
                tagfilter.Add((int)filterD.unpackDataF(a));
            }
        }
        /// <summary>
        /// Hittedsを1フレーム前のものとする。これはworldで勝手に呼び出す。
        /// </summary>
        public void topre() 
        {
            this.preHitteds = new List<WeakReference<Entity>>(this.Hitteds);
        }
        /// <summary>
        /// ぶつかったエンテティを取得する。多分重い
        /// </summary>
        /// <param name="h">当たり方のタイプ</param>
        /// <returns></returns>
        public List<Entity> GetHitteds(HitType h) 
        {
            List<Entity> res=new List<Entity>();
            switch (h)
            {
                case HitType.enter:
                    for (int i = Hitteds.Count-1; i >=0; --i)
                    {
                        if (preHitteds.Contains(Hitteds[i]) == false)
                        {
                            Entity a;
                            if (Hitteds[i].TryGetTarget(out a))
                            {
                                res.Add(a);
                            }
                            else 
                            {
                            Hitteds.RemoveAt(i);
                            }
                        }
                    }
                    break;
                case HitType.ing:

                    for (int i = Hitteds.Count - 1; i >= 0; --i)
                    {
                        if (preHitteds.Contains(Hitteds[i]) == true)
                        {
                            Entity a;
                            if (Hitteds[i].TryGetTarget(out a))
                            {
                                res.Add(a);
                            }
                            else
                            {
                                Hitteds.RemoveAt(i);
                            }
                        }
                    }

                    break;
                case HitType.end:
                    for (int i = preHitteds.Count - 1; i >= 0; --i)
                    {
                        if (Hitteds.Contains(preHitteds[i]) == false)
                        {
                            Entity a;
                            if (preHitteds[i].TryGetTarget(out a))
                            {
                                res.Add(a);
                            }
                            else
                            {
                                preHitteds.RemoveAt(i);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return res;
        }

        public override void addtoworld(float cl = 0)
        {
            base.addtoworld(cl);
            SetHitboxPosition();
            SetPreboxPosition();
        }

        /// <summary>
        /// ヒットボックス用の図形をEntityの大きさにはめる
        /// </summary>
        public void SetHitboxPosition()
        {
            if (e != null)
            {
                HitShape.setto(e);
            }

        }
        /// <summary>
        /// 前フレームのヒットボックス用の図形を現在のhitboxにはめる
        /// </summary>
        public void SetPreboxPosition()
        {
            preHitShape = HitShape.clone();
        }
        /// <summary>
        /// 当たり判定がぶつかるかどうか
        /// </summary>
        /// <param name="h">相手のコンポーネント</param>
        /// <returns></returns>
        public bool Hits(Hitbox h) 
        {
            var res = this.HitShape.atarun2(this.preHitShape, h.HitShape, h.preHitShape);
            //var res2 = this.HitShape.atarun( h.HitShape);
          
            return res;
        }
        static public int debugchekman=0;
        /// <summary>
        /// 当たり判定がぶつかるかどうか。一つでもHitboxがぶつかればOK
        /// </summary>
        /// <param name="e">相手のエンテティ</param>
        /// <returns></returns>
        public bool Hits(Entity e)
        {
            debugchekman = 0;
            foreach (var a in e.getcompos<Hitbox>())
            {
                debugchekman++;
                if (this.Hits(a)) 
                {
                    return true;
                }
            }
            return false; 
        }

        /// <summary>
        /// フィルターをどういう感じに通るか
        /// </summary>
        public enum FilterOption{ThisFilter//このコンポーネントのフィルターに通る
                ,AndFilter//両方のフィルターに通る
                ,OrFilter //どちらかのフィルターに通る
                ,EFilter//フィルターに通る
        }
        /// <summary>
        /// このコンポーネントのフィルターを相手のコンポーネントが通るか
        /// </summary>
        /// <param name="h">タグを参照する相手のコンポーネント</param>
        /// <param name="filteroption">フィルターのオプション</param>
        /// <returns></returns>
        public bool Filters(Hitbox h, FilterOption filteroption=FilterOption.AndFilter) 
        {
            //タグが無ければ当たらない。
            if(this.tag.Count==0)return false;

            bool thisOK = true;
            var thisFilter = new List<int>(this.tagfilter);
            if (thisFilter.Count == 0) { thisFilter.AddRange(h.tag); }

            for (int i=0;i<thisFilter.Count;i++)
            {
                if (h.tag.Contains(thisFilter[i]) == false)
                {
                    thisOK = false;
                    break;
                }
            }
            switch (filteroption)
            {
                case FilterOption.ThisFilter:
                    return thisOK;
                    break;
                case FilterOption.AndFilter:
                    return thisOK&&h.Filters(this,FilterOption.ThisFilter);
                    break;
                case FilterOption.OrFilter:
                    return thisOK || h.Filters(this, FilterOption.ThisFilter);
                    break;
                case FilterOption.EFilter:

                    return  h.Filters(this, FilterOption.ThisFilter);
                    break;
                default:
                    break;
            }

            return thisOK;

        }
        /// <summary>
        /// このコンポーネントのフィルターを相手が通るか。一つでもHitboxがとおればOK
        /// </summary>
        /// <param name="e">相手のえんててぃ</param>
        /// <param name="filteroption">フィルターのオプション</param>
        /// <returns></returns>
        public bool Filters(Entity e, FilterOption filteroption = FilterOption.AndFilter)
        {
            foreach (var a in e.getcompos<Hitbox>())
            {
                if (this.Filters(a,filteroption))
                {
                    return true;
                }
            }
            return false;

        }
        protected override void onupdate(float cl)
        {
            base.onupdate(cl);

        }

    }
}
