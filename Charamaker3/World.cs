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

        virtual public void update(float cl) 
        {
            Edic.Clear();
            _Edic.Clear();
            Ddic.Clear();
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

            foreach (var a in Hdic.getresult())
            {
               a.SetHitboxPosition();
            }
            {
                var tasks = new List<Task>();
                var hitentityes=getEdic("HasHitbox");
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
            foreach (var a in Hdic.getresult())
            {
               a.SetPreboxPosition();
            }

            foreach (var a in Entities) 
            {
                a.update(cl);
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

    }
}
