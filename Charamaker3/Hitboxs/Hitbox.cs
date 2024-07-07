using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charamaker3.Shapes;

namespace Charamaker3.Hitboxs
{


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
        /// 自身についている属性。intだけど、enumを入れたほうがいい。
        /// </summary>
        List<int> tag=new List<int>();

        /// <summary>
        /// これがないならぶつからないってタグ。空にするとすべてに対してぶつかる。フィルターは後でかけてもいいけど、ここでやったほうが軽い
        /// </summary>
        List<int> tagfilter=new List<int>();




        /// <summary>
        /// ぶつかったと判断されたやつ。Worldが勝手にいじる。
        /// </summary>
        public List<Entity>Hitteds= new List<Entity>();
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
            this.tag = tag;
            tagfilter = filter;
        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public Hitbox() { }

        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (Hitbox)c;
            cc.tag = new List<int>(this.tag);
            cc.tagfilter = new List<int>(this.tagfilter);
            cc.HitShape = this.HitShape.clone();
            cc.preHitShape = this.preHitShape.clone();
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
            var tagD = d.unpackDataD("tag");
            foreach (var a in tagD.getAllPacks()) 
            {
                tag.Add((int)tagD.unpackDataF(a));
            }
            var filterD = d.unpackDataD("filter");
            foreach (var a in filterD.getAllPacks())
            {
                tagfilter.Add((int)filterD.unpackDataF(a));
            }
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

        /// <summary>
        /// 当たり判定がぶつかるかどうか。一つでもHitboxがぶつかればOK
        /// </summary>
        /// <param name="e">相手のエンテティ</param>
        /// <returns></returns>
        public bool Hits(Entity e)
        {
            foreach (var a in e.getcompos<Hitbox>()) 
            {
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
            bool thisOK = true;
            var thisFilter = this.tagfilter;
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


    }
}
