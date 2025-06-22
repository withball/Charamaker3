using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Charamaker3.CharaModel
{
    public class Joint 
    {
        public string name;
        /// <summary>
        /// ジョイントの位置の割合
        /// </summary>
        public float px, py;
        /// <summary>
        /// 親Entity
        /// </summary>
        public Entity parent;
        public List<Entity> childs = new List<Entity>();
        public Joint(string name, float px, float py, Entity parent, List<Entity> childs)
        {
            this.name = name;
            this.px = px;
            this.py = py;
            this.parent = parent;
            this.childs = new List<Entity>(childs);
        }
        public Joint clone() 
        {
            return new Joint(name, px, py, null, new List<Entity>());

        }
        public DataSaver ToSave() 
        {
            var jd = new DataSaver();

            if (parent != null)
            {
                jd.packAdd("parentName", parent.name);
            }
            else 
            {
                jd.packAdd("parentName", "");
            }
            jd.packAdd("name", name);
            jd.linechange();
            if (parent != null)
            {
                jd.packAdd("dx", px * parent.w);
                jd.packAdd("dy", py * parent.h);
                jd.linechange();
                jd.packAdd("pw", parent.w);
                jd.packAdd("ph", parent.h);
            }
            else 
            {
                jd.packAdd("px", px);
                jd.packAdd("py", py);
            }
            jd.linechange();
            return jd;
        }

        public void ToLoad(DataSaver d)
        {
            var jd = new DataSaver();

            name=d.unpackDataS("name");
            
            px=d.unpackDataF("px");
            py=d.unpackDataF("py");
            if (float.IsNaN(px)) 
            {
                px = d.unpackDataF("dx") / d.unpackDataF("pw");
                py = d.unpackDataF("dy") / d.unpackDataF("ph");
            }
        }
        public void assemble()
        {
            //head :assemble: -16.100002 :XY: -52.69647 :: 12.06187 :: 10.44727
            //hair :assemble: -16.100002 :XY: -54.59593 :: 0 :: 0
            foreach (var a in childs)
            {
                a.settxy(gettxy());
                var lis = a.getcompos<Texture>();
                //float z = -999;
                //if (lis.Count > 0) { z = lis[0].z; }
                //Debug.WriteLine(parent.name+":p::c:"+a.name + " :assemble: " + a.gettxy().ToString() + " :: " +z);
            }
        }
        public FXY gettxy() 
        {
            return parent.gettxy(parent.w * px, parent.h * py);
        }

    }
    /// <summary>
    /// キャラクター。addされたときに自動でsetBaseされる。
    /// </summary>
    public class Character : Component
    {
        /// <summary>
        /// キャラクターの最も小さなZ座標を取得する
        /// </summary>
        /// <param name="e"></param>
        static public float GetMinZ(Entity e) 
        {
            float z = float.NaN;
            foreach (var a in e.getCharacter().getTree("")) 
            {
                foreach (var b in a.getcompos<Drawable>())
                {
                    if (float.IsNaN(z) == true)
                    {
                        z = b.z;
                    }
                    else
                    {
                        z = Mathf.min(z, b.z);
                    }
                }
            }
            if (float.IsNaN(z)) 
            {
                z = 0;
            }
            return z;
        }

        /// <summary>
        /// キャラクターを基準も含めてセットする
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="dz"></param>
        /// <param name="zbai">zを全部倍にしたり</param>
        /// <param name="opacity"></param>
        static public void SetupCharacter(Entity e,string name,float scale, float dz,float zbai=1, float opacity = 1)
        {
            if (name != null)
            {
                e.name = name;
            }

      

            var cs = e.getcompos<Character>();
            foreach (var c in cs) 
            {
                if (c.BaseCharacter != c)
                {
                    var lis = new List<Character> { c, c.BaseCharacter };
                    foreach (var cc in lis)
                    {
                        EntityMove.ScaleChange(0, "", scale, scale).add(cc.e, 10);
                        DrawableMove.BaseColorChange(0, "", opacity).add(cc.e, 10);

                        foreach (var a in cc.getTree("")) 
                        {
                            foreach (var b in a.getcompos<Drawable>()) 
                            {
                                b.zDelta = dz;
                                b.zRatio = zbai;
                            }
                        }

                    }


                }
                else
                {
                    EntityMove.ScaleChange(0, "", scale, scale).add(c.e, 10);
                    DrawableMove.BaseColorChange(0, "", opacity).add(c.e, 10);

                    foreach (var a in c.getTree(""))
                    {
                        foreach (var b in a.getcompos<Drawable>())
                        {
                            b.zDelta = dz;
                            b.zRatio = zbai;
                        }
                    }

                }
                break;
            }
        }

        //先頭のジョイントはEntityがコアの特別なジョイント
        List<Joint> _joints = new List<Joint>();
        public List<Joint> joints { get { return new List<Joint>(_joints); } }

        /// <summary>
        /// ベースキャラクターのEntityが一時的に、ほんの一瞬だけなる名前。StackOverflowの防止の観点より導入
        /// この名前を素で使うと呪われるので注意！
        /// </summary>
        const string BaseCharacterEName= "BaseCharacter";
        /// <summary>
        /// 
        /// </summary>
        public List<Joint> _joints_ { get { return _joints; } }

        Character _BaseCharacter =null;

        

        public override void addtoworld(float cl = 0)
        {
            base.addtoworld(cl);
            foreach (var j in _joints)
            {
                foreach (var a in j.childs)
                {
                    a.add(this.world);
                }
            }
        }

        public override void removetoworld(float cl = 0)
        {
            base.removetoworld(cl);
            foreach (var j in _joints)
            {
                foreach (var a in j.childs)
                {
                    a.remove();
                }
            }
        }
        protected override void onadd(float cl)
        {
            base.onadd(cl);
            if (_joints.Count > 0)
            {
                _joints[0].parent = e;
                foreach (var a in _joints) 
                {
                    if (a.parent == null) a.parent = e;
                }
            }
            /*
            if (e.name != BaseCharacter,name) 
            {
                Debug.WriteLine(e.name + " different Name" + BaseCharacter.name);
                SetBaseCharacter();
            }*/
        }

        protected override void onupdate(float cl)
        {
            base.onupdate(cl);
            assembleCharacter();
        }
        /// <summary>
        /// キャラクターを作る
        /// </summary>
        /// <param name="tex">元テクスチャー</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="size">サイズ+テクスチャーの横幅に合わせる<br></br>-でたて幅に合わせる</param>
        /// <param name="tpx">中心の割合</param>
        /// <param name="tpy"></param>
        /// <param name="z"></param>
        /// <param name="corename"></param>
        /// <returns></returns>
        static public Entity MakeCharacter(string tex, float x, float y, float size,
            float tpx = 0.5f, float tpy = 0.5f, float z = 10000, string corename = "core")
        {
            var bmpWh = FileMan.GetTextureSize(tex);

            float w, h;

            if (size > 0)
            {
                w = size;
                h = bmpWh.y * size / bmpWh.x;
            }
            else
            {

                w = bmpWh.x * -size / bmpWh.y;
                h = -size;
            }


            var res = Entity.make(x, y, w, h, w * tpx, h * tpy, 0, corename);

            var c = new Character(new Joint(corename + "JOI", 0.5f, 0.5f, null, new List<Entity>()));


            var t = new Texture(z, new ColorC(1, 1, 1, 1), new Dictionary<string, string> { { "def", tex } });
            t.add(res);

            c.add(res);

            c.SetBaseCharacter();
            return res;
        }
        /// <summary>
        /// キャラクターの普通のコンストラクタ。
        /// </summary>
        /// <param name="corejoint">核となるジョイント。parentは必要ない。</param>
        public Character(Joint corejoint) : base(-1)
        {
            _joints.Add(corejoint);
        }
        /// <summary>
        /// ジョイントを追加する。
        /// </summary>
        /// <param name="j"></param>
        public void addJoint(Joint j) 
        {
            _joints_.Add(j);
        }

        public Character() : base(-1)
        {

        }
        /// <summary>
        /// コピーする。ベースもコピーする
        /// </summary>
        /// <param name="c"></param>
        public override void copy(Component c)
        {
            var cc = (Character)c;
            base.copy(c);
            cc._joints = new List<Joint>();
            foreach (var a in this._joints)
            {
                var njoi = a.clone();
                cc._joints.Add(njoi);
                var childs = new List<Entity>();
                foreach (var b in a.childs)
                {
                    var newEntity = b.clone();

                    //Debug.WriteLine(newEntity.ToString());
                    //Debug.WriteLine(newEntity.ToString());
                    //Debug.WriteLine("OK!");

                    childs.Add(b.clone());

                }
                njoi.childs = childs;

            }
            for (int i = 0; i < this._joints.Count; i++)
            {
                var name = this._joints[i].parent.name;
                foreach (var a in cc._joints)
                {
                    foreach (var b in a.childs)
                    {
                        if (b.name == name)
                        {
                            cc._joints[i].parent = b;
                            break;
                        }
                    }
                }
            }
            //null
            cc.doubleChildecheck();

            if (this._BaseCharacter != null) 
            {
                cc.SetBaseCharacter( this.BaseCharacter.e.clone().getcompos<Character>()[0]);
            }
        }

        /// <summary>
        /// 子が重複してたらヤヴァイので調査＆削除する<br></br>
        /// 
        /// </summary>
        void doubleChildecheck() 
        {
            //子供に親が混じってたら無限参照するため、親を消し飛ばす。
            foreach (var a in joints) 
            {
                foreach (var b in a.childs) 
                {
                    if (a.parent != null && a.parent.name == b.name) 
                    {
                        a.parent = e;
                    }
                }
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var a in joints) 
            {
                for (int i = a.childs.Count-1; i >=0; i--) 
                {
                    if (dic.ContainsKey(a.childs[i].name))
                    {
                        Debug.WriteLine(a.childs[i].name+ " 重複する名前のやつが削除されました from "+a.name);
                        a.childs.RemoveAt(i);
                    }
                    else 
                    {
                        dic.Add(a.childs[i].name, a.childs[i].name);
                    }
                }
            }
        }

        public override DataSaver ToSave()
        {
            var res = base.ToSave();

            var d = new DataSaver();
            for (int i = 0; i < _joints.Count; i++) 
            {
                var jd = _joints[i].ToSave();
                jd.linechange();

                var childs = new DataSaver();
                for (int t = 0; t < _joints[i].childs.Count; t++) 
                {
                    var cd = _joints[i].childs[t].ToSave();
                    cd.indent();
                    childs.packAdd(_joints[i].childs[t].name, cd);
                    childs.linechange();
                }
                childs.indent();
                jd.packAdd("childs", childs);


                d.packAdd(i.ToString(),jd);
                d.linechange();
            }
            res.packAdd("joints", d);
            res.linechange();
            if (_BaseCharacter != null) 
            {
                res.packAdd("BaseCharacter",_BaseCharacter.e.ToSave());
            }
            return res;
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);

            var jd=d.unpackDataD("joints");

            var lis = jd.allUnpackDataD();
            foreach (var a in lis)
            {
                var njoi = new Joint("",0,0, null, new List<Entity>());
                njoi.ToLoad(a);
                var childs = a.unpackDataD("childs");
                foreach (var b in childs.allUnpackDataD())
                {
                    njoi.childs.Add(Entity.ToLoadEntity(b));
                }
                _joints.Add(njoi);
            }
            for (int i = 0; i < lis.Count; i++)
            {
                _joints[i].parent = getEntity(lis[i].unpackDataS("parentName"));
               
            }

            var basec = d.unpackDataD("BaseCharacter");
            if (basec.getData() != "")
            {
                var e = Entity.ToLoadEntity(basec);

                _BaseCharacter = e.getcompos<Character>()[0];
                doubleChildecheck();
            }

        }
        public override string ToString()
        {
            var res = base.ToString();
            foreach (var a in _joints)
            {
                if (a.parent != e && a.parent != null)
                {
                    res += "\n" + a.parent.ToString();
                }
                else
                {
                    res += "\n" + a.name + "waaaa";
                }
            }
            return res;
        }
        public List<Entity> getTree(string name, bool addroot = true)
        {
            var res = new List<Entity>();
            var root = getEntity(name);
            if (root == null) { return res; }
            if (addroot) res.Add(root);
            foreach (var a in getJoint(root.name))
            {
                foreach (var b in a.childs)
                {
                    res.Add(b);
                    res.AddRange(getTree(b.name, false));
                }
            }
            return res;
        }
        /// <summary>
        /// そのEntityに追加されてるジョイントを全部取得する
        /// </summary>
        /// <param name="Parentname"></param>
        /// <returns></returns>
        public List<Joint> getJoint(string Parentname)
        {
            if (Parentname == "")
            {
                var res = new List<Joint>();
                foreach (var a in joints)
                {
                    if (a.parent==e)
                    {
                        res.Add(a);
                    }
                }
                return res;
            }
            else 
            {
                var res = new List<Joint>();
                foreach (var a in joints)
                {
                    if (a.parent.name == Parentname)
                    {
                        res.Add(a);
                    }
                }
                return res;
            }

        }
        /// <summary>
        /// パーツの親のパーツを返す
        /// </summary>
        /// <param name="Childname"></param>
        /// <returns></returns>
        public Entity getParent(string Childname)
        {
            var res = getParentJoint(Childname);
            if (res == null) return null;
            return res.parent;

        }
        /// <summary>
        /// パーツの親のジョイントを返す
        /// </summary>
        /// <param name="Childname"></param>
        /// <returns></returns>
        public Joint getParentJoint(string Childname)
        {
            if (Childname == "") return null;
            foreach (var a in joints)
            {
                foreach (var b in a.childs)
                {
                    if (b.name == Childname)
                    {
                        return a;
                    }

                }
            }
            return null;

        }

        public Entity getEntity(string name)
        {
            if (name == ""||(e!=null&&name==e.name)) return e;
            foreach (var a in joints)
            {
                foreach (var b in a.childs)
                {
                    if (b.name == name)
                    {
                        return b;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        static public Character ToloadC2(DataSaver d)
        {
            var res = new Character(); ;
            var e = Entity.make(d.unpackDataF("x"), d.unpackDataF("y")
                , d.unpackDataF("w"), d.unpackDataF("h")
                , d.unpackDataF("tx"), d.unpackDataF("ty")
                , Mathf.toDegree(d.unpackDataF("rad")));

            res.add(e);

            var core = d.unpackDataD("core");



            res._joints = ToloadC2Setu(core, e);
            res._joints[0].px += 0.5f;
            res._joints[0].py += 0.5f;

            res.SetBaseCharacter();

            res.doubleChildecheck();
            return res;
        }
        static protected List<Joint> ToloadC2Setu(DataSaver d, Entity parentE)
        {

            var dp = d.unpackDataD("picture");
            var e = Entity.make(dp.unpackDataF("x"), dp.unpackDataF("y")
                , dp.unpackDataF("w"), dp.unpackDataF("h")
                , dp.unpackDataF("tx"), dp.unpackDataF("ty")
                , Mathf.toDegree(dp.unpackDataF("rad")), d.unpackDataS("name"));

            e.mirror = dp.unpackDataB("mirror");

            var texs = dp.unpackDataD("textures");
            List<KeyValuePair<string, string>> textures = new List<KeyValuePair<string, string>>();
            foreach (var a in texs.getAllPacks())
            {
                textures.Add(new KeyValuePair<string, string>(a, texs.unpackDataS(a)));
            }
            var texture = Texture.make(dp.unpackDataF("z"), dp.unpackDataF("opa"), textures.ToArray());
            texture.texname = dp.unpackDataS("texname");

            texture.add(e);



            var j = new Joint(d.unpackDataS("name") + "JOI", d.unpackDataF("dx") / parentE.w, d.unpackDataF("dy") / parentE.h
                , parentE, new List<Entity>());

            j.childs.Add(e);


            var res = new List<Joint>();

            var dsts = d.unpackDataD("sts");
            foreach (var a in dsts.allUnpackDataD())
            {
                res.AddRange(ToloadC2Setu(a, e));
            }
            res.Insert(0, j);

            return res;


        }
        /// <summary>
        /// キャラクターを組み立てる、というかジョイントとかで整列させる
        /// </summary>
        public void assembleCharacter()
        {
            if (_joints.Count > 0)
            {


                List<Entity> parent = new List<Entity> { e };
                List<Joint> workingjoint = new List<Joint>(_joints);

                while (workingjoint.Count > 0 && parent.Count > 0)
                {
                    var p = parent[0];
                    for (int i = 0; i < workingjoint.Count; i++)
                    {
                        var w = workingjoint[i];
                        if (w.parent == p)
                        {
                            // Debug.WriteLine(p.name + " lol");
                            w.assemble();
                            parent.AddRange(w.childs);
                            workingjoint.RemoveAt(i--);

                        }

                    }

                    parent.RemoveAt(0);
                }
            }
        }
        #region basecharacter

        /// <summary>
        /// 現在の状態をベースにする。もちろんaddされてる状態で呼び出してね。
        /// </summary>
        public void SetBaseCharacter()
        {
            var nm = e.name;
            
            e.name = BaseCharacterEName;
            var clone = e.clone();
            clone.name = e.name;

            _BaseCharacter = clone.getcompos<Character>(this.name)[0];

            e.name = nm;
            BaseCharacter.name = nm;
            BaseCharacter._BaseCharacter = null;

        }
        /// <summary>
        /// ベースをセットする
        /// </summary>
        /// <param name="c">クローンされたアドレスフリーのやつね</param>
        public void SetBaseCharacter(Character c)
        {
            var nm = c.e.name;
            c.e.name = BaseCharacterEName;
            _BaseCharacter =c;
            _BaseCharacter.name = nm;

            BaseCharacter._BaseCharacter = null;
        }
        public Character BaseCharacter { get { if (_BaseCharacter == null) return this; return _BaseCharacter; } }

       

        #endregion
    }




}
