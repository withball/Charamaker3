using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charamaker3.CharaModel
{
    public class AnimeLoader
    {
        DataSaver d;
        protected float _maxtime=1;
        public float maxtime { get { return _maxtime; } }
        public AnimeLoader(DataSaver d) 
        {
            this.d = d;
            {
                var config = d.unpackDataD("config");

                _maxtime = Mathf.abs(config.unpackDataF("maxtime", 1));

            }
        }
        public DataSaver ToSave() 
        {
            var res=new DataSaver();
            var config=new DataSaver();
            config.packAdd("maxtime", 1);
            config.indent();
            res.packAdd("config", config);
            
            res.packAdd("0",new Utils.SummonEntity(new Entity(),0).ToSave().indent());
            res.packAdd("1", new Utils.SummonCharacter("",1,"",0).ToSave().indent());
            res.packAdd("2", new Utils.SummonComponent(new Component(),"",0).ToSave().indent());
            res.packAdd("3", new Utils.SummonMotion("",1,"",0).ToSave().indent());

            return res;
        }
        /// <summary>
        /// アニメを起動する
        /// </summary>
        /// <param name="e">アニメの進行役を務めるエンテティ</param>
        public void MakeAnime(Entity e) 
        {

            foreach (var a in d.getAllPacks())
            {
                if (a == "config")
                {
                }
                else
                {
                    var c = Component.ToLoadComponent(d.unpackDataD(a));
                    
                    c.add(e);
                }
            }
        }
        public string Check() 
        {
            return d.getAllkouzou();
        }

    }
}
