using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charamaker3
{

    /// <summary>
    /// 継承するときはcopyをオーバライドすることを忘れずに
    /// </summary>
    public class Entity
    {
        static public T ToLoadEntity<T>(DataSaver d)
            where T : Entity
        {

            var type = Type.GetType(d.unpackDataS("type"));
            if (typeof(T) == type)
            {
                var res = (T)Activator.CreateInstance(type);
                res.ToLoad(d);
                return res;
            }
            Debug.WriteLine("指定されたタイプじゃないので死にました。");
            return null;
        }
        static public Entity ToLoadEntity(DataSaver d)
        {

            var type = Type.GetType(d.unpackDataS("type"));
            if (type == null)
            {
                Debug.WriteLine("Entityじゃないよ！ちゃんと読み込めなかったよ！");

                return new Entity();
            }
            var res = (Entity)Activator.CreateInstance(type);
            res.ToLoad(d);

            return res;
        }

        #region 定義
        public float x=0, y = 0, w = 0, h = 0, tx = 0, ty = 0;
        private float _degree = 0;

        bool _added = false;
        World _world = null;
        public string name="";

        /// <summary>
        /// エンテティの辺の長さの平均
        /// </summary>
        public float bigs { get { return (Mathf.abs(w) + Mathf.abs(h)) / 2; } }
        public bool added { get { return _added; } }
        public World world { get { return _world; } }

        public float degree { get { return _degree; } 
            set {
                var fxy = gettxy(); 
                _degree  = Mathf.st180(value); 
                settxy(fxy); } }

        public bool mirror=false;
        /// <summary>
        /// entityの回転に合わせた位置を取得する
        /// </summary>
        /// <param name="cx">画像上のwの点。NaNで回転中心</param>
        /// <param name="cy">画像上のhの点。NaNで回転中心</param>
        /// <returns></returns>
        public FXY gettxy(float ww=float.NaN,float hh= float.NaN) 
        {
            if (float.IsNaN(ww)) ww = tx;
            if (float.IsNaN(hh)) hh = ty;
            float W; if (mirror) W = w - ww; else W = ww;
            float H; if (mirror && 1 == 0) H = h - hh; else H = hh;
            float rx = x + W * Mathf.cos(degree) - H * Mathf.sin(degree);
            float ry = y + W * Mathf.sin(degree) + H * Mathf.cos(degree);
            return new FXY(rx,ry);
        }

        /// <summary>
        /// entityの回転に合わせた位置を(w,hに対する割合で)
        /// </summary>
        /// <param name="cx">画像上のwの点。NaNで回転中心</param>
        /// <param name="cy">画像上のhの点。NaNで回転中心</param>
        /// <returns></returns>
        public FXY gettxy2(float ww = float.NaN, float hh = float.NaN)
        {

            if (float.IsNaN(ww))
            {
                ww = tx;
            }
            else 
            {
                ww = ww * w;
            }
            if (float.IsNaN(hh))
            {
                hh = ty;
            }
            else 
            {
                hh = hh * h;
            }
            float W; if (mirror) W = w - ww; else W = ww;
            float H; if (mirror && 1 == 0) H = h - hh; else H = hh;
            float rx = x + W * Mathf.cos(degree) - H * Mathf.sin(degree);
            float ry = y + W * Mathf.sin(degree) + H * Mathf.cos(degree);
            return new FXY(rx, ry);
        }

        /// <summary>
        /// 任意の一点をxy座標にセットする
        /// </summary>
        /// <param name="xx">セットするx座標</param>
        /// <param name="yy">セットするy座標</param>
        /// <param name="ww">画像上のwの点。Nanで回転中心</param>
        /// <param name="hh">画像上のhの点。Nanで回転中心</param>
        public void settxy(float xx, float yy, float ww=float.NaN, float hh= float.NaN)
        {
            if (float.IsNaN(ww)) ww = tx;
            if (float.IsNaN(hh)) hh = ty;
            var txy = gettxy(ww, hh);
            x += xx - txy.x;
            y += yy - txy.y;
        }
        /// <summary>
        /// 任意の一点をxy座標にセットする
        /// </summary>
        /// <param name="xx">セットするx座標</param>
        /// <param name="yy">セットするy座標</param>
        /// <param name="ww">画像上のwの点。Nanで回転中心</param>
        /// <param name="yy">画像上のhの点。Nanで回転中心</param>
        public void settxy(FXY xy, float ww = float.NaN, float hh = float.NaN)
        {
            settxy(xy.x, xy.y, ww, hh);
        }
        /// <summary>
        /// entityを作る
        /// </summary>
        /// <param name="x">中心の座標x</param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="txp"></param>
        /// <param name="typ"></param>
        /// <param name="degree"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        static public Entity make(float x, float y, float w, float h, float tx , float ty , float degree = 0,string name="") 
        {
            var res = new Entity();
            res.x = x;
            res.y = y;
            res.w = w;
            res.h = h;
            res.tx = tx;
            res.ty = ty;
            res.degree = degree;
            res.name = name;
            res.settxy(x,y);
            return res;
        }
        /// <summary>
        /// entityを作る
        /// </summary>
        /// <param name="x">中心の座標x</param>
        /// <param name="y"></param>
        /// <param name="w"></param>
        /// <param name="h"></param>
        /// <param name="txp">中心位置の割合</param>
        /// <param name="typ">中心位置の割合</param>
        /// <param name="degree"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        static public Entity make2(float x, float y, float w, float h, float txp = 0.5f, float typ = 0.5f, float degree = 0, string name = "")
        {
            return make(x, y, w, h, w * txp, h * typ, degree, name);
        }
        public Entity() { }
        public Entity clone() 
        {
            var res = (Entity)Activator.CreateInstance(this.GetType());
            this.copy(res);
            foreach (var a in components) 
            {
                a.clone().add(res);
            }
            return res;

        }
        /// <summary>
        /// コンポーネントはコピーされない。コンポーネントをコピーしたい場合はそれぞれをクローンして追加してください。
        /// </summary>
        /// <param name="e">コピー先</param>
        /// <returns></returns>
       virtual public void copy(Entity e) 
        {
            e.x = this.x;
            e.y = this.y;
            e.w = this.w;
            e.h = this.h;
            e.tx = this.tx;
            e.ty= this.ty;
            e.degree = this.degree;
            e.name=this.name;
            e.mirror = this.mirror;
        }

        /// <summary>
        /// Entityを保存するためのメソッド
        /// </summary>
        /// <returns></returns>
        public virtual DataSaver ToSave()
        {
            var res = new DataSaver();
            res.packAdd("type",this.GetType().ToString());
            res.packAdd("name",this.name);
            res.linechange();
            res.packAdd("x", this.x);
            res.packAdd("y", this.y);
            res.linechange();
            res.packAdd("w", this.w);
            res.packAdd("h", this.h);
            res.linechange();

            res.packAdd("tx", this.tx);
            res.packAdd("ty", this.ty);
            res.linechange();
            res.packAdd("degree", this.degree);
            res.linechange();
            res.packAdd("mirror", this.mirror);
            res.linechange();


            res.linechange();

            var d =new DataSaver();
            var compos = components;
            for (int i = 0; i < compos.Count; i++) 
            {
                var cd = compos[i].ToSave();
                cd.indent();
                d.packAdd(i.ToString(),cd);
                d.linechange();
            }
            d.indent();
            res.packAdd("components", d);

            return res;
        }
        protected virtual void ToLoad(DataSaver d) 
        {
            this.name = d.unpackDataS("name", "");

            this.x = d.unpackDataF("x",0);
            this.y = d.unpackDataF("y", 0);
            this.w = d.unpackDataF("w", 1);
            this.h = d.unpackDataF("h", 1);

            this.tx = d.unpackDataF("tx", 0.5f);
            this.ty = d.unpackDataF("ty", 0.5f);
            this.degree = d.unpackDataF("degree", 0);
            this.mirror = d.unpackDataB("mirror", false);
            
            var dd = d.unpackDataD("components");
            foreach (var a in dd.allUnpackDataD()) 
            {
                Component.ToLoadComponent(a).add(this);
            }
        }
        /// <summary>
        /// 文字にする。
        /// 継承するなら StringInfo
        /// </summary>
        /// <returns></returns>
        public override string ToString() 
        {
            var res = StringInfo();
            foreach (var a in components) 
            {
                res+="\n"+a.ToString();
            }
            return res;
        }
        /// <summary>
        /// ToStringの一部になるやつ
        /// </summary>
        /// <returns></returns>
        virtual protected string StringInfo() 
        {
            return this.GetType().ToString() + "{" + $"{name}:{x},{y},{w},{h},{tx},{ty},{degree}" + "}";
        }
        #endregion

        #region addとか
        protected List<Component> _components = new List<Component>();

        /// <summary>
        /// 技のリストのコピーをもらえる
        /// </summary>
        public List<Component> components { get { return new List<Component>(_components); } }

        /// <summary>
        /// タイプで絞り込んで技のリストを取得する
        /// </summary>
        /// <param name="t">技のタイプ</param>
        /// <returns>新しいリスト</returns>
        public List<Component> getcompos(Type t)
        {

            if (t == typeof(Component)) return new List<Component>(_components);
            if (!t.IsSubclassOf(typeof(Component))) return new List<Component>();
            var lis = new List<Component>();
            Type tt;
            foreach (var a in _components)
            {
                tt = a.GetType();
                if (tt == t || tt.IsSubclassOf(t))
                {
                    lis.Add(a);
                }
            }
            return lis;
        }

        

        /// <summary>
        /// 名前で絞り込んで技のリストを取得する
        /// </summary>
        /// <typeparam name="T">タイプ</typeparam>
        /// <returns>技のリスト</returns>
        public List<T> getcompos<T>(string name=null)
        where T : Component
        {
            var res = new List<T>();
            if (typeof(T) == typeof(Component))
            {
                foreach (var a in _components)
                {
                    if (name==null||name == a.name)
                    {
                        res.Add((T)a);
                    }
                }
                return res;
            }

            Type tt;
            foreach (var a in _components)
            {
                tt = a.GetType();
                if ((tt == typeof(T) || tt.IsSubclassOf(typeof(T))) && (name == null || name == a.name))
                {
                    res.Add((T)a);
                }
            }
            return res;
        }

        public bool add(World w)
        {
            if (!added)
            {
                if (w.add(this))
                {
                    _world = w;
                    _added = true;
                    onadd();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// addされたときに呼びだされる。
        /// </summary>
        virtual protected void onadd() 
        {
            var lis = components;
            foreach (var a in lis)
            {
                a.addtoworld();
            }
        }
        virtual public void update(float cl) 
        {
            var lis = components;
            foreach (var a in lis)
            {
                a.update(cl);
            }
        }
        public bool remove()
        {
            if (added)
            {
                if (world.remove(this))
                {
                    onremove();
                    _added = false;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// removeされたときに呼びだされる。
        /// </summary>
        virtual protected void onremove()
        {
            var lis = components;
            foreach (var a in lis)
            {
                a.removetoworld();
            }
        }

        /// <summary>
        /// Componentだけから呼び出す
        /// </summary>
        /// <param name="c"></param>
        virtual public void compoadd(Component c,float cl) 
        {
            _components.Add(c);
            if (added) //Entiyがすでに顕現していたら
            {
                c.addtoworld(cl);
            }
        }
        /// <summary>
        /// Componentだけから呼び出す
        /// </summary>
        /// <param name="c"></param>
        virtual public void comporemove(Component c)
        {
            if (_components.Remove(c) && added)
            {
                c.removetoworld();
            }
        }

        #endregion

        #region bennri,CompShortCuts

        /// <summary>
        /// 先頭のキャラクターをもらう。持っていない場合は架空のキャラクターを返す。
        /// </summary>
        /// <returns></returns>
        virtual public CharaModel.Character getCharacter() 
        {
            var cs = this.getcompos<CharaModel.Character>();
            if (cs.Count > 0) 
            {
                return cs[0];
            }
            return new CharaModel.Character(new CharaModel.Joint("coreJ",0,0,this,new List<Entity>()));
        }
        /// <summary>
        /// 先頭のDrawableをもらう。持っていない場合は架空のDrawableを返す
        /// </summary>
        /// <returns></returns>
        virtual public D getDrawable<D>()
            where D :Drawable
        {
            var cs = this.getcompos<D>();
            if (cs.Count > 0)
            {
                return cs[0];
            }
            if (typeof(D) == typeof(Drawable))
            {
                var res = ((DRectangle)Activator.CreateInstance(typeof(DRectangle)));

                return (D)Component.ToLoadComponent(res.ToSave());
            }
            else 
            {
                var res = ((D)Activator.CreateInstance(typeof(D)));

                return Component.ToLoadComponent<D>(res.ToSave());
            }
        }

        /// <summary>
        /// このEntityの持つCharacterやHitBoxの位置を整列させる。Etityが移動したときとかにどうぞ。
        /// </summary>
        virtual public void RefreshComponentsPosition() 
        {
            {
                var lis = getcompos<CharaModel.Character>();
                foreach (var a in lis) 
                {
                    a.assembleCharacter();
                }
            }
            {
                setHitbox(false);
            }
        }

        /// <summary>
        /// Hitboxコンポーネントを持っていたらヒットしたエンテティを返す。
        /// </summary>
        /// <returns></returns>
        public List<Entity> getHits()
        {
            var res = new List<Entity>();
            foreach (var a in getcompos<Hitboxs.Hitbox>())
            {
                res.AddRange(a.Hitteds);
            }
            return res;
        }

        /// <summary>
        /// 持っているHitboxコンポーネントをもとに当たり判定をとる。
        /// </summary>
        /// <returns></returns>
        public bool Hits(Shapes.Shape pres,Shapes.Shape s)
        {
            var res = new List<Entity>();
            foreach (var a in getcompos<Hitboxs.Hitbox>())
            {
                if (s.atarun2(pres, a.HitShape, a.preHitShape))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Hitboxコンポーネントを持っていたら、現在の状態に当たり判定をそろえる
        /// </summary>
        /// <param name="pretoo">1フレーム前の当たり判定もそろえるワープ移動とかした後にはtrueで</param>
        /// <returns></returns>
        public void setHitbox(bool pretoo = true)
        {

            foreach (var a in getcompos<Hitboxs.Hitbox>())
            {
                a.SetHitboxPosition();
                if (pretoo)
                {
                    a.SetPreboxPosition();
                }
            }
        }

        #endregion

        /// <summary>
        /// EntityをShapeにかぶせるっていうかセットする
        /// </summary>
        /// <param name="p"></param>
        public void setto(Shapes.Shape s)
        {
            w = s.w;
            h = s.h;
            _degree = s.degree;
            x = s.x;
            y = s.y;
        }
    }


    /// <summary>
    /// コンポーネント。次元式でないものはEntityに残り続ける。
    /// </summary>
    public class Component
    {
        static public T ToLoadComponent<T>(DataSaver d)
           where T : Component
        {
           var type = Type.GetType(d.unpackDataS("type"));
            if (typeof(T) == type)
            {
                var res = (T)Activator.CreateInstance(type);
                res.ToLoad(d);
                return res;
            }
            Debug.WriteLine(type+" != "+typeof(T)+" 指定されたタイプじゃないので死にました。");
            return null;
        }
        static public Component ToLoadComponent(DataSaver d)
        {

            var type = Type.GetType(d.unpackDataS("type"));
            if (type == null)
            {
                Debug.WriteLine(d.unpackDataS("type","no [type]") + "はComponentじゃないよ！ちゃんと読み込めなかったよ！");
                return new Component();
            }
            var res = (Component)Activator.CreateInstance(type);
            res.ToLoad(d);

            return res;
        }
        #region 定義
        /// <summary>
        /// エンテティに追加されたとき
        /// </summary>
        public event EventHandler<float> added;

        /// <summary>
        /// エンテティから取り除かれたとき
        /// </summary>
        public event EventHandler<float> removed;
        /// <summary>
        /// エンテティが追加されたとき == この世界に顕現した時
        /// </summary>
        public event EventHandler<float> Wadded;
        /// <summary>
        /// エンテティが取り除かれたとき == この世界から消えた時
        /// </summary>
        public event EventHandler<float> Wremoved;
        /// <summary>
        /// アップデート
        /// </summary>
        public event EventHandler<float> updated;
        /// <summary>
        /// 好きに使える奴
        /// </summary>
        public event EventHandler<string> freeEvent;
        /// <summary>
        ///好きに使っていい奴を発動する奴
        /// </summary>
        /// <param name="input"></param>
        public void freeevent(string input)
        {
            freeEvent?.Invoke(this, input);
        }
        protected Entity _e;
        public Entity e { get { return _e; } }
        public bool ended { get { return _e == null; } }
        /// <summary>
        /// worldのショートカット
        /// </summary>
        public World world { get { if (e == null) return null; return _e.world; } }
        protected bool _onWorld=false;
        public bool onWorld { get { return _onWorld; } }
        public string name="";
        /// <summary>
        /// 終わる時間0より小さければ終わらない。
        /// </summary>
        public float time = -1;
        public float timer=0;

        /// <summary>
        /// タイマーをリセットする。時間で変化するコンポーネントをコピーするときに必要。
        /// </summary>
        virtual public void resettimer() 
        {
            if (!eternal)
            {
                timer = 0;
            }
            else 
            {
                timer = 0;
            }
        }
        /// <summary>
        /// 時間経過でエンテティから取り除かれないか
        /// </summary>
       virtual public bool eternal { get { return time < 0; } }
        public float remaintime { get { if (time>=0) return Mathf.max(time - timer, 0);return -1; } }
        /// <summary>
        /// updateで可能な時間だけ取り出す。
        /// </summary>
        /// <param name="cl"></param>
        /// <returns></returns>
       virtual public float canclocktime(float cl) 
        {
            if (eternal) return cl;

            return Mathf.min(remaintime,cl);
        }
        /// <summary>
        /// endしたときに発動する奴等。
        /// </summary>
        public List<Component> afters = new List<Component>();
        
        public Component(float time = -1, string name = "") 
        {
            this.time = time;
            this.name = name;
        }
        public Component()
        {
        }
        /// <summary>
        /// クローンを作る。ただし、ラムダ式での奴は無理
        /// </summary>
        /// <returns></returns>
        public Component clone() 
        {
            var res = (Component)Activator.CreateInstance(this.GetType());
            this.copy(res);
            foreach (var a in afters) 
            {
                res.afters.Add(a.clone());
            }
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c">コピー先</param>
        virtual public void copy(Component c) 
        {
            c.time = this.time;
            c.name = this.name;
            c.timer=this.timer;

        }


        /// <summary>
        /// Componentを保存するためのメソッド
        /// </summary>
        /// <returns></returns>
        public virtual DataSaver ToSave()
        {
            var res = new DataSaver();
            res.packAdd("type", this.GetType().ToString());
            res.packAdd("name", this.name);
            res.linechange();
            res.packAdd("time",time);

            res.linechange();
            var d = new DataSaver();
            var compos = afters;
            for (int i = 0; i < compos.Count; i++)
            {
                var cd = compos[i].ToSave();
                cd.indent();
                d.packAdd(i.ToString(),cd);
                d.linechange();
            }
            d.indent();
            res.packAdd("afters", d);

            return res;
        }

        protected virtual void ToLoad(DataSaver d)
        {
            this.name = d.unpackDataS("name", "");

            this.time = d.unpackDataF("time", 0);

            var dd = d.unpackDataD("afters");
            foreach (var a in dd.allUnpackDataD())
            {
                this.afters.Add(Component.ToLoadComponent(a));
            }

        }
        /// <summary>
        /// 追加して消す。motionを少しだけ動かすときとかに。
        /// </summary>
        /// <param name="ee"></param>
        /// <param name="cl"></param>
        /// <returns>追加削除ができたか</returns>
        public bool addAndRemove(Entity ee, float cl)
        {
            if (this.add(ee, cl)) 
            {
                this.remove();
                return true;
            }
            return false;
        }
        virtual public bool add(Entity ee,float cl=0)
        {
            if (e == null)
            {
                this._e = ee;
                this.e.compoadd(this, cl);
                onadd(cl);
                return true;
            }
            
                Debug.WriteLine(name+this.GetType().ToString()+" is already added");
            return false;
        }

        virtual protected void onadd(float cl) 
        {
            added?.Invoke(e, cl);
            update(cl);
        }
        virtual public void update(float cl)
        {
            var ncl = canclocktime(cl);
            timer += ncl;
            if (ncl >= 0)
            {
                onupdate(ncl);
                if (!eternal&&remaintime <= 0) 
                {
                    remove(cl-ncl);
                }
            }
        }
        virtual protected void onupdate(float cl) 
        {
            updated?.Invoke(e,cl);
        }

        virtual public bool remove(float cl=0) 
        {
            if (e != null)
            {
                onremove(cl);
                this.e.comporemove(this);
                this._e = null;

                return true;
            }
            return false;
        }
        virtual protected void onremove(float cl) 
        {
            removed?.Invoke(e, cl);
            foreach (var a in afters) 
            {
                a.add(e,cl);
            }
        }
        /// <summary>
        /// 世界に追加されたときに発動するメソッド.
        ///　onaddより先に発動する。
        /// </summary>
        /// <param name="cl"></param>
        virtual public void addtoworld(float cl=0)
        {
            this._onWorld = true;
            Wadded?.Invoke(e, canclocktime(cl));
            
        }

        /// <summary>
        /// 世界から消えた時に発動するメソッド
        /// </summary>

        virtual public void removetoworld(float cl = 0)
        {
            this._onWorld = false;
            Wremoved?.Invoke(e, canclocktime(cl));
        }

        override public string ToString() 
        {
            return this.GetType().ToString()+$" {name}:{timer}/{time}:";
        }
        #endregion

        #region statics
        /// <summary>
        /// 終わった後に発動させる。 a + b +c ではaのあとにbとcが発動します。
        /// </summary>
        /// <param name="moto"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        static public Component operator +(Component moto, Component add)
        {
            moto.afters.Add(moto);
            return moto;
        }
        /// <summary>
        /// 終わった後に発動させるやつの再後尾に追加する。 a * b *c ではa->b->cと発動します。
        /// </summary>
        /// <param name="moto"></param>
        /// <param name="add"></param>
        /// <returns></returns>
        static public Component operator *(Component moto, Component add)
        {
            while (moto.afters.Count > 0)
            {
                moto = moto.afters[0];
            }
            moto.afters.Add(moto);
            return moto;
        }
        #endregion
    }

    /// <summary>
    /// モーションの中のコンポーネントは個別で追跡する必要がある。
    /// </summary>
    public class Motion : Component
    {
        List<Component> cs = new List<Component>();
        List<bool> stops = new List<bool>();
        public bool loop = false;

        public float speed = 1;
        int idx = -1;
        /// <summary>
        /// モーションが空か
        /// </summary>
        public bool empty { get { return cs.Count==0; } }
        /// <summary>
        /// 途中時間で終わらないComponentがあるとおかしくなるかもしれないけど。
        /// </summary>
        /// <returns></returns>
        public float sumtime  
        { get {
                float max = 0;
                float line = 0;
                for (int i = 0; i < cs.Count; i++) 
                {
                    if (stops[i])
                    {
                        if (cs[i].eternal && !cs[i].ended)
                        {
                            return -1;
                        }

                        line += cs[i].time;

                        max = Mathf.max(line, max);
                    }
                    else
                    {
                        if (cs[i].eternal&&!cs[i].ended)
                        {
                            return -1;
                          }
                        max = Mathf.max(max, line + cs[i].time);


                    }
                }
                return max;
            } }
        public Motion(float speed = 1, bool loop = false, string name = "") : base(0, name)
        {
            this.speed = speed;
            this.loop = loop;
        }
        public Motion() : base() { }
        public override void copy(Component c)
        {
            var cc = (Motion)c;
            base.copy(c);
            for (int i=0;i<cs.Count;i++)
            {
                cc.cs.Add(this.cs[i].clone());
                cc.stops.Add(this.stops[i]);
            }
            cc.idx = this.idx;
            cc.loop = this.loop;
            cc.speed = this.speed;
        }
        public override DataSaver ToSave()
        {
            var res = base.ToSave();
            res.packAdd("loop",loop);
            res.packAdd("speed", speed);
            var d=new DataSaver();
            for (int i = 0; i < cs.Count; i++)
            {
                var cd = cs[i].ToSave();
                cd.indent();
                d.packAdd(i.ToString()+":"+stops[i],cd);
            }
            d.indent();
            res.packAdd("cs",d);
            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
            this.loop = d.unpackDataB("loop",false);
            this.speed = d.unpackDataF("speed", 1);
            var cs = d.unpackDataD("cs");
            var lis = cs.getAllPacks();
            for (int i = 0; i < lis.Count; i++) 
            {
                var stop = new DataSaver(lis[i]).splitOneDataB(1);
                this.addmove(Component.ToLoadComponent(cs.unpackDataD(lis[i])),stop);
            }
        }

        public override string ToString()
        {
            var res = base.ToString()+" now "+idx+"/"+cs.Count;
            for (int i = 0; i < cs.Count; i++) 
            {
                res += "\n"+ cs[i].ToString();
                if (stops[i]) res += "\nSTOP!";
            }
            return res;
        }
        void fakeadd(Component c,float cl=0) //fakeRemoveは要らないなぜならonRemoveは問題なく発動するから。
        {
            if (this.e != null)
            {
                c.add(this.e, cl);
                e.comporemove(c);
            }
            
        }
        public override void resettimer()
        {
            base.resettimer();
            idx = -1;
            time = sumtime;
            foreach (var a in cs) 
            {
                a.resettimer();
            }
        }
        void incrementidx() 
        {
            idx += 1;
            if (idx < cs.Count)
            {
                fakeadd(cs[idx]);
                cs[idx].resettimer();
            }
        }
        /// <summary>
        /// 追加したmoveを全て消す。MotionMakaerで使うと便利
        /// </summary>
        public void Clear() 
        {
            cs.Clear();
            stops.Clear();
        }
        public void addmove(Component c,bool stop=false)
        {
            cs.Add(c);
            stops.Add(stop);
            time = sumtime;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cl">余った時間。</param>
        protected override void onadd(float cl)
        {
            //start();
            time = sumtime;

            for (int i = 0; i <= idx && i < cs.Count; i++)
            {
                fakeadd(cs[i],cl*speed);//ここどうなんだろうか
            }
            base.onadd(cl);
        }

        override public void update(float cl)
        {
           
            var ncl = cl*speed;
            if (idx == -1)
            {
                incrementidx();
                time = sumtime;
            }
            
            float endcl = cl*2;
            for (int i = 0; i <= idx && i < cs.Count; i++)
            {
                float clocked = cs[i].canclocktime(ncl);

                //余った時間。これが0より大きければこのupdateで終わったということになる。
                if (cs[i].ended == false)
                {
                    cs[i].update(ncl);
                }
                if (cs[i].ended == true) //終わっていた嫌でも1の余りを出すってことにする
                {
                    endcl = Mathf.min(endcl, 1);
                }
                else 
                {
                    endcl = Mathf.min(endcl, ncl - clocked);
                }

                if (i == idx)
                {
                    if (stops[i])
                    {
                        if (cs[i].ended)
                        {
                            incrementidx();
                            ncl -= clocked;
                        }
                    }
                    else
                    {
                        incrementidx();
                    }
                }
            }
            float nokocl = cl-endcl;
            timer += nokocl;//timerはあくまで経過時間。motionの中身の経過時間で表す。
            if (ncl >= 0)
            {
                onupdate(ncl);
                if (endcl>0&&idx>=cs.Count)
                {
                    if (!loop)
                    {
                        remove(endcl);
                    }
                    else 
                    {
                        resettimer();
                        //!!!注意！!!!
                        //update(endcl)
                        //っておきたいけど、endcl=clだった時に無限ループが発生するためやりたくない？
                        //けどそんな無限ループする奴を置くようなやつが悪いので
                        update(endcl/speed);
                    }
                }
            }
        }
        /// <summary>
        /// これがないと。csはEntityに認識されていないため、addtoworldがでない
        /// </summary>
        /// <param name="cl"></param>
        public override void addtoworld(float cl = 0)
        {
            base.addtoworld(cl);
            for (int i = 0; i <= idx && i < cs.Count; i++)
            {
                cs[i].addtoworld(cl);
            }
        }

        public override void removetoworld(float cl = 0)
        {
            base.removetoworld(cl);
            for (int i = 0; i <= idx && i < cs.Count; i++)
            {
                cs[i].removetoworld(cl);
            }
        }


        /// <summary>
        /// タイプで絞り込んで技のリストを取得する
        /// </summary>
        /// <param name="t">技のタイプ</param>
        /// <returns>新しいリスト</returns>
        public List<Component> getcompos(Type t)
        {

            if (t == typeof(Component)) return new List<Component>(cs);
            if (!t.IsSubclassOf(typeof(Component))) return new List<Component>();
            var lis = new List<Component>();
            Type tt;
            foreach (var a in cs)
            {
                tt = a.GetType();
                if (tt == t || tt.IsSubclassOf(t))
                {
                    lis.Add(a);
                }
            }
            return lis;
        }

        /// <summary>
        /// タイプで絞り込んで技のリストを取得する
        /// </summary>
        /// <typeparam name="T">タイプ</typeparam>
        /// <returns>技のリスト</returns>
        public List<T> getcompos<T>()
        where T : Component
        {
            var res = new List<T>();
            if (typeof(T) == typeof(Component))
            {
                foreach (var a in cs)
                {
                    res.Add((T)a);
                }
                return res;
            }

            Type tt;
            foreach (var a in cs)
            {
                tt = a.GetType();
                if (tt == typeof(T) || tt.IsSubclassOf(typeof(T)))
                {
                    res.Add((T)a);
                }
            }
            return res;
        }

        /// <summary>
        /// タグで絞り込んで技のリストを取得する
        /// </summary>
        /// <typeparam name="T">タイプ</typeparam>
        /// <returns>技のリスト</returns>
        public List<T> getcomps<T>(string tag)
        where T : Component
        {
            var res = new List<T>();
            if (typeof(T) == typeof(Component))
            {
                foreach (var a in cs)
                {
                    if (tag == a.name)
                    {
                        res.Add((T)a);
                    }
                }
                return res;
            }

            Type tt;
            foreach (var a in cs)
            {
                tt = a.GetType();
                if ((tt == typeof(T) || tt.IsSubclassOf(typeof(T))) && tag == a.name)
                {
                    res.Add((T)a);
                }
            }
            return res;
        }
       
    }
}
