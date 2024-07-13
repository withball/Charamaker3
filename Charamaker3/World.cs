using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charamaker3.Hitboxs;

namespace Charamaker3
{
    public class World
    {
        /// <summary>
        /// World作成時に追加されるエンテティ。BGMの再生とかにどうぞ。カメラの位置を指すために使ったりもできるし。
        /// </summary>
        public Entity staticEntity=new Entity();

        public World() 
        {
            staticEntity.add(this);
        }


        List<Entity> Entities = new List<Entity>();

        Dictionary<string, supersort<Entity>> _Edic = new Dictionary<string, supersort<Entity>>();

        /// <summary>
        /// タグとそのエンテティのリスト
        /// </summary>
        public Dictionary<string, List<Entity>> Edic = new Dictionary<string, List<Entity>>();
        /// <summary>
        /// DrawAbleのリスト。変えるなよ。
        /// </summary>
        public supersort<Drawable> Ddic = new supersort<Drawable>();
        /// <summary>
        /// hitboxのリスト。変えるなよ。
        /// </summary>
        public supersort<Hitbox> Hdic = new supersort<Hitbox>();
        /// <summary>
        /// PhysicsCompのリスト。変えるなよ。
        /// </summary>
        public supersort<PhysicsComp> Pdic = new supersort<PhysicsComp>();
        /// <summary>
        /// Entity.addでだけ呼び出す
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool add(Entity e)
        {
            if (!Entities.Contains(e))
            {
                Entities.Add(e);

                return true;
            }
            return false;
        } 
        /// <summary>
          /// Entity.removeでだけ呼び出す
          /// </summary>
          /// <param name="e"></param>
          /// <returns></returns>
        public bool remove(Entity e)
        {
            return Entities.Remove(e);
        }
        /// <summary>
        /// ここに分類したい奴を書き込んだりする。
        /// </summary>
        public event EventHandler<Entity> classifyed;

        /// <summary>
        /// フレームを更新する
        /// </summary>
        /// <param name="cl"></param>
        virtual public void update(float cl) 
        {
            Edic.Clear();
            _Edic.Clear();
            Ddic.Clear();
            Pdic.Clear();
            Hdic.Clear();

            foreach (var a in Entities) 
            {
                classify(a);
            }
            foreach (var a in _Edic) 
            {
                a.Value.sort(false);
                Edic.Add(a.Key, a.Value.getresult());
            }
            Ddic.sort(false);
            Hdic.sort(false);
            Pdic.sort(false);


            foreach (var a in Hdic.getresult())
            {
                a.SetHitboxPosition();
            }
            toPreHitbox();

            calcHitbox();
            buturiall(cl);

            calcHitbox();
            foreach (var a in Hdic.getresult())
            {
                a.freeevent("a");
            }

            foreach (var a in Hdic.getresult())
            {
                a.SetPreboxPosition();
            }


            foreach (var a in getEdic()) 
            {
                a.update(cl);
            }




        }
        /// <summary>
        /// 当たり判定を一フレーム前のものとする。
        /// </summary>
        protected void toPreHitbox()
        {
            {
                var tasks = new List<Task>();
                foreach (var a in Hdic.getresult())
                {
                    tasks.Add(
                 Task.Run(() =>
                 {
                     a.topre();
                 }));
                }
                foreach (var a in tasks)
                {
                    a.Wait();
                }
            }
        }
        /// <summary>
        /// 当たり判定を一気に処理する
        /// </summary>
        protected void calcHitbox() 
        {
            {
                var tasks = new List<Task>();
                var hitentityes = getEdic("HasHitbox");
                foreach (var a in Hdic.getresult())
                {
                    tasks.Add(
                 Task.Run(() =>
                 {
                     a.Hitteds.Clear();

                     foreach (var b in hitentityes)
                     {
                         if (a.e != b)
                         {
                             if (a.Filters(b))
                             {
                                 if (a.Hits(b))
                                 {
                                     a.Hitteds.Add(b);
                                 }
                             }
                         }
                     }
                 }));
                }
                foreach (var a in tasks)
                {
                    a.Wait();
                }
            }
        }

        /// <summary>
        /// Edicにエンテティを追加する。
        /// </summary>
        public void addEdic(string key,Entity value,float sortkey) 
        {
            if (!_Edic.ContainsKey(key)) 
            {
                _Edic.Add(key, new supersort<Entity>());
            }
            _Edic[key].add(value,sortkey);
        }
        void classify(Entity e) 
        {
            classifyed?.Invoke(this,e);
            addEdic("def", e,0);
            {
                var lis = e.getcompos<Drawable>();
                foreach (var a in lis)
                {
                    Ddic.add(a, a.z);
                }
            }
            {
                var lis = e.getcompos<Hitbox>();
                if (lis.Count > 0) 
                {
                    addEdic("HasHitbox", e, 0);
                }
                foreach (var a in lis)
                {
                    Hdic.add(a,0);
                }
            }
            {
                var lis = e.getcompos<PhysicsComp>();
                if (lis.Count > 0)
                {
                    addEdic("HasPhysics", e,0);
                }
                foreach (var a in lis)
                {
                    Pdic.add(a, a.wei);
                }
            }
        }
        /// <summary>
        /// キーに即したえんててぃを持ってくる。全部入ってるのはdef
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<Entity> getEdic(string key="def") 
        {
            if (Edic.ContainsKey(key)) 
            {
                return new List<Entity>(Edic[key]);
            }
            return new List<Entity>();
        }

        #region physics
     
        /// <summary>
        /// すべての物体を重ならないように動かす
        /// </summary>
        /// <param name="ps">軽い順</param>
        protected void zurasiall(List<PhysicsComp> ps)
        {
            int i;


            //  foreach (var a in es) Console.WriteLine(a.bif.wei+" a sa ");
            //  Console.WriteLine(es.Count + " :;asdlgka:p ");

            for (i = 0; i < ps.Count; i++)
            {

                // if (es[i].bif.ovw) break;
                for (int t = 0; t < ps.Count; t++)
                {

                    if (i != t && !ps[i].isgrouped(ps[t]))
                    {
                        var ishapes = ps[i].e.getcompos<Hitbox>();
                        var tshapes = ps[t].e.getcompos<Hitbox>();

                        bool end = false;
                        foreach (var a in ishapes) 
                        {
                            foreach (var b in tshapes) 
                            {
                                if (a.Hitteds.Contains(b.e) && b.Hitteds.Contains(a.e))
                                {
                                    //     Console.WriteLine("aslkgapo");
                                    PhysicsComp.zuren(ps[i], a, ps[t], b, true);

                                    end = true;
                                    break;
                                }
                            }
                            if (end) break;
                        }
                      
                    }
                    else
                    {
                        break;
                    }

                }
            }


            for (i = 0; i < ps.Count; i++)
            {
                ps[i].groupclear();
                ps[i].energyConserv();
            }

        }
        /// <summary>
        /// すべての物体を反射させる
        /// </summary>
        /// <param name="ps">軽い順</param>
        protected void hansyaall(List<PhysicsComp> ps)
        {


            for (int i = 0; i < ps.Count; i++)
            {
                if (ps[i].ovw) break;
                for (int t = i + 1; t < ps.Count; t++)
                {
                    var ishapes = ps[i].e.getcompos<Hitbox>();
                    var tshapes = ps[t].e.getcompos<Hitbox>();

                    bool end = false;
                    foreach (var a in ishapes)
                    {
                        foreach (var b in tshapes)
                        {
                            if (PhysicsComp.setsessyoku(ps[i], a, ps[t], b))
                            {

                                end = true;
                                break;
                            }
                        }
                        if (end) break;
                    }
                }
            }



            for (int i = 0; i < ps.Count; i++)
            {
                 if (ps[i].ovw)
                {
                    break;
                }//	TRACE(_T("%f :asfas: %f\n"), es[i]->vx, es[i]->vy);
                 //es[i]->sessyokukaiten(cl);

                for (int t = i + 1; t < ps.Count; t++)
                {
                    var ishapes = ps[i].e.getcompos<Hitbox>();
                    var tshapes = ps[t].e.getcompos<Hitbox>();

                    foreach (var a in ishapes)
                    {
                        foreach (var b in tshapes)
                        {
                            PhysicsComp.SessyokuHansya(ps[i], a, ps[t], b);
                        }
                    }
                }
            }


            foreach (var a in ps)
            {
                a.resetsessyokus();

                //  Console.WriteLine(a.Acore.getCenter().ToString() + " daaa");
            }
        }
        /// <summary>
        /// すべての物体をずらし、物理系のほかのもする
        /// </summary>
        protected void buturiall(float cl)
        {
            var ps = Pdic.getresult(false);

         
            zurasiall(ps);
            hansyaall(ps);
        }
        #endregion
    }
}
