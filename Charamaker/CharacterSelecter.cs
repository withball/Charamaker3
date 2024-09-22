using Charamaker3.CharaModel;
using Charamaker3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charamaker
{
    public class CharacterSelecter
    {
        public Character? c=null;
        public Joint? j =null;
        public Entity? e =null;

        public CharacterSelecter(Character c) 
        {
            this.c=c;
        }

        public CharacterSelecter(Entity c)
        {
            this.c = c.getcompos<Character>()[0];
        }
        public void add(World w)
        {
            c?.e?.add(w);
        }
        public void remove()
        {
            c?.e?.remove();
        }
        public String getData() 
        {
            if (e == null)
            {
                var moto = c.e;
                c.remove();
                var res = moto.ToSave();
                {

                    var js = new DataSaver();
                    int cou = 0;
                    foreach (var a in c.joints)
                    {
                        var jd = a.ToSave();
                        var cd = new DataSaver();
                        for (int i = 0; i < a.childs.Count; i++)
                        {
                            cd.packAdd(i.ToString(), a.childs[i].name);
                        }
                        jd.packAdd("childsName", cd);

                        jd.indent(2);

                        js.packAdd((cou++).ToString(), jd);
                    }
                    js.indent(2);
                    res.packAdd("CHARA_CONSTRUCT", js);
                }

                c.add(moto);
                return res.getData();
            }
            else 
            {
                var res = e.ToSave();

                res.packAdd("joint",j.ToSave().PackRemoves("ParentName"));
                return res.getData();
            }
            return e.ToSave().getData();
        }
        
        public CharacterSelecter setData(String data)
        {
            if (e == null)
            {
                /*
                 [0]{ joint~~[childsName]{}}
                 
                 */

                var d = new DataSaver(data);
                var jd = d.unpackDataD("CHARA_CONSTRUCT");

                var newc = new Character();
                foreach (var a in jd.allUnpackDataD())
                {
                    var j = new Joint("", 0, 0, null, new List<Entity>());
                    j.ToLoad(a);
                    var pn = a.unpackDataS("parentName");
                    if (pn != c.e.name)
                    {
                        j.parent = c.getEntity(pn);
                    }
                    var cd = a.unpackDataD("childsName");
                    foreach (var b in cd.allUnpackDataS())
                    {
                        var ce = c.getEntity(b);
                        if (ce == null)//新しい名前を入れたら自動生成
                        {
                            ce = Entity.make(0,0, c.e.bigs, c.e.bigs, c.e.bigs/2, c.e.bigs/2,0,b);
                            Texture.make(0, 1, new KeyValuePair<string, string>("def", "nothing")).add(ce);
                        }
                        j.childs.Add(ce);
                    }
                    newc._joints_.Add(j);
                }



                var newe = Entity.ToLoadEntity(d);
                //ベースを受け渡す。
                {
                    newc.add(newe);
                    var newcd = newc.ToSave();
                    newcd.packAdd("BaseCharacter", c.ToSave().unpackDataD("BaseCharacter"));
                    newc.remove();
                    newc = Component.ToLoadComponent<Character>(newcd);

                }
                newc.add(newe);

                var res = newe.clone();
                
                var resc = res.getcompos<Character>()[0];
                var resres = new CharacterSelecter(resc);

                return resres;
            }
            else 
            {
                var newc = c.e.clone().getcompos<Character>()[0];


                var newj=newc.getParentJoint(e.name);
                var d = new DataSaver(data);
                newj.ToLoad(d.unpackDataD("joint"));

                var newe = Entity.ToLoadEntity(d);
                for (int i = 0; i < newj.childs.Count; i++) 
                {
                    if (e.name == newj.childs[i].name) 
                    {
                        newj.childs[i] = newe;
                    }
                }

                var res = newc.e.clone();
                var resc = res.getcompos<Character>()[0];
                var resres = new CharacterSelecter(resc);
                resres.SelectReef(newe.name);
                return resres;
            }
            return null;

        }
        int precou = 0;
        string pres = "";
        public void selectbyPoint(float x, float y) 
        {
            string nexts="";
            var lis = new List<Entity>();
            foreach (var a in c.getTree("", false)) 
            {
                var s = new Charamaker3.Shapes.Rectangle(0);
                s.setto(a);
                if (s.onhani(x, y)) 
                {
                    lis.Add(a);
                    nexts += a.name;
                }
            }
            if (lis.Count > 0)
            {
                if (pres == nexts)
                {
                }
                else
                {
                    precou = 0;
                }
                pres = nexts;
                SelectReef(lis[precou%lis.Count].name);
                precou++;
            }
            else 
            {
                precou = 0;
                pres = "";
                SelectReef("");
            }

        }
        public void selectbyOld(CharacterSelecter cl)
        {
            if (cl.e != null)
            {
                SelectReef(cl.e.name);
            }
            else { }
        }
        public void SelectByName(string name) 
        {
            var lis = new List<Entity>();
            foreach (var a in c.getTree("", false))
            {
                if (a.name==name)
                {
                    SelectReef(name);
                    break;
                }
            }
        }
            protected void SelectReef(string name) 
        {
            var sel=c.getEntity(name);
            if (sel == c.e)
            {
                e = null; j = null;
            }
            else 
            {
                e = sel;
                j = c.getParentJoint(name);
            }
        }
        public void setPoints(Drawable toe, Drawable totxy,float opa) 
        {
            toe.col.opa = opa;
            totxy.col.opa = opa;
            if (e == null)
            {
                c.e.copy(toe.e);
                totxy.e.settxy(c.e.gettxy());
                totxy.e.degree=c.e.degree;

            }
            else 
            {
                e.copy(toe.e);
                totxy.e.settxy(e.gettxy());
                totxy.e.degree = e.degree;
            }
        }
    }
}
