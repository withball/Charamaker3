using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Charamaker3.Utils;
namespace Charamaker3.CharaModel
{

    public class AnimeLoader
    {
        /*
        /// <summary>
        /// 音を流す。
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <param name="volume">=1</param>
        /// <returns>__ANIM__**</returns>
        static public SummonComponent SE(float time, string target, string path, float volume = 1)
        {
            try
            {
                var se = SoundComponent.MakeSE(FileMan.SE, path, volume);
                return new SummonComponent(se, target, time, 0, "SoundEffect::" + path);
            }
            catch (Exception ex)
            {
                Debug.Mess("SoundEffect Load Missed " + path + " \n" + ex.ToString());
                return new SummonComponent(new Component(), "", 0);
            }
        }

        /// <summary>
        /// BGMを流すコンポーネント。
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="stop">止める時間</param>
        /// <param name="path"></param>
        /// <param name="volume">=1</param>
        /// <param name="fadein">=-1</param>
        /// <param name="fadeout">=-1</param>
        /// <returns>__ANIM__**</returns>
        static public SummonEntity BGM(float time,float stop, string path, float volume,float fadein=-1,float fadeout=-1)
        {
            try
            {
                var bgm = SoundComponent.MakeBGM(FileMan.BGM, path, volume, fadein,fadeout);
                var lifetime = new LifeTimer(stop);

                var e = Entity.make(0,0,1,1,1,1);
                bgm.add(e);
                lifetime.add(e);

                return new SummonEntity(e, time, "BGM::" + path);
            }
            catch (Exception ex)
            {
                Debug.Mess("BGM Load Missed " + path + " \n" + ex.ToString());

                var e = Entity.make(0, 0, 1, 1, 1, 1);
                return new SummonEntity(e, time, "BGM::" + path);
            }
        }

        /// <summary>
        /// キャラクターをパスから召喚する
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="scale">=1</param>
        /// <returns>__ANIM__**</returns>
        static public SummonCharacter SummonCharacter(float time,string path,string name,float scale=1) 
        {
            return new SummonCharacter(path,scale,name,time,path+name+"summon");
        }
        /// <summary>
        /// モーションをパスから召喚する
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="path">パス</param>
        /// <param name="tag">対象</param>
        /// <param name="speed">=1</param>
        /// <param name="addcl">召喚された瞬間に経過する時間=0</param>
        /// <returns>__ANIM__**</returns>
        static public SummonMotion SummonMotion(float time, string path, string tag, float speed = 1
            ,float addcl=0)
        {
            return new SummonMotion(path, speed, tag, time,addcl, path  + "summon");
        }

        /// <summary>
        /// 即席のキャラクターを召喚する。textureは"core"に追加される
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="texture"></param>
        /// <param name="size"></param>
        /// <param name="name"></param>
        /// <returns>__ANIM__**</returns>
        static public SummonEntity SummonICharacter(float time,string texture,float size,string name)
        {
            var e=CharaModel.Character.MakeCharacter(texture, 0, 0, size, 0.5f, 0.5f, 0);
            e.name= name; 
            return new SummonEntity(e,time);
        }*/


        DataSaver d;
        public AnimeLoader(DataSaver d) 
        {
            //Debug.WriteLine(d.getData());

            {
                var config = d.unpackDataD("config");

                var func=config.unpackDataD("function");

                foreach (var a in func.getAllPacks()) 
                {
                    registfunction(a, func.unpackDataD(a));
                }

                d=d.PackRemoves("config");
            }
            {
                bool replaced = true;
                while (replaced) 
                {
                    d = replaseCustomfunction(d, out replaced);
                }
                //Debug.WriteLine(d.getData());
            }

            this.d = d;
            {
                bool replaced = true;
                while (replaced)
                {
                    this.d = replaseData(this.d, out replaced);
                }
            }

        }
        /// <summary>
        /// データを置き換える。
        /// </summary>
        protected DataSaver replaseCustomfunction(DataSaver d, out bool replaced)
        {
            
            replaced = false;
            if (d.getAllPacks().Count == 0)
            {
                return d;
            }
            var res = new DataSaver(d.getData());
            foreach (var a in d.getAllPacks())
            {
                string outPackname;
                var funcname = getfuncname(a, out outPackname);
                //在ったら変換
                if (customFunctions.ContainsKey(funcname))
                {

                    res = res.PackReplace(a, "", customFunctions[funcname].Invoke(outPackname,d.unpackDataD(a)).getData());
                    replaced = true;
                }
            }
            return res;
        }
        /// <summary>
        /// データを置き換える。
        /// </summary>
        protected DataSaver replaseData(DataSaver d,out bool replaced)
        {
            replaced = false;
            if (d.getAllPacks().Count == 0) 
            {
                return d;
            }
            var res=new DataSaver(d.getData());
            foreach (var a in d.getAllPacks())
            {
                string outPackname;
                var funcname = getfuncname(a,out outPackname);
                //関数以外だったら
                if (funcname=="")
                {

                    bool Chiledreplaced;
                    res =res.PackReplace(a,a,replaseData(d.unpackDataD(a), out Chiledreplaced).getData());
                    if(Chiledreplaced)replaced = true;
                }
                else
                {
                    bool Chiledreplaced;
                    var recursedD = replaseData(d.unpackDataD(a),out Chiledreplaced);
                    Debug.WriteLine(funcname+" replaced!!!");
                    replaced = true;
                    if (outPackname != funcname)
                    {
                        var rep = replace(funcname, recursedD);
                        res = res.PackReplace(a, outPackname, rep.getData());
                        //Debug.WriteLine(rep.getData() + " rep");
                    }
                    else 
                    {
                        var rep = replace(funcname, recursedD);
                        res = res.PackReplace(a, "", rep.getData());
                       // Debug.WriteLine(rep.getData() + " rep");
                    }
                }
            }
            return res;
        }
        protected string getfuncname(string packname,out string outPackname) 
        {
            var sp = packname.Split(':');
            if (sp.Length < 2) 
            {
                outPackname = "";
                return "";
            }
            if (sp[0] == "F") 
            {
                var res = sp[1];
                for (int i = 2; i < sp.Count(); i++) 
                {
                    res += ":" + sp[i];
                }
                outPackname = res;
                return sp[1];
            }
            outPackname = "";
            return "";
        }


        /// <summary>
        /// データ置き換えの法則
        /// </summary>
        /// <param name="funcname">関数の名前</param>
        /// <param name="d"></param>
        /// <returns>何もなかったら引数と同時に返される</returns>
        protected virtual DataSaver replace(string funcname,DataSaver d) 
        {
            DataSaver resolve(MethodInfo methinfo) 
            {
                var sp = d.allUnpackDataS();
                var param = methinfo.GetParameters();

                List<object> args = new List<object>();
                for (int i = 0; i < param.Count(); i++)
                {

                    var pt = param[i].ParameterType;
                    var conv = TypeDescriptor.GetConverter(pt);
                    //{ []{}[]{}[]{} }の引数を解きほぐす
                    if (i<sp.Count)
                    {
                        try
                        {
                            var conved = conv.ConvertFrom(sp[i]);
                            args.Add(conved);
                        }
                        catch (Exception)
                        {
                            if (param[i].HasDefaultValue)
                            {
                                args.Add(param[i].DefaultValue);
                            }
                            else
                            {
                                Debug.Mess($"function {funcname} 's argument is not collect\n argument number is{i} named {param[i].Name}");
                                return new DataSaver();
                            }
                        }
                    }
                    else if (param[i].HasDefaultValue)
                    {
                        args.Add(param[i].DefaultValue);
                    }
                    else
                    {
                        Debug.Mess($"function {funcname} 's argument is not collect\n argument number is{i} named {param[i].Name}");
                        return new DataSaver();
                    }
                }
              
                var comp = (Component)methinfo.Invoke(null, args.ToArray());
                return comp.ToSave();
            }
            {
                var t = typeof(AnimeLoader);
                var methinfo = t.GetMethod(funcname);
                if (methinfo != null)
                {
                    return resolve(methinfo);
                }
            }
            {
                var t = typeof(EntityMove);
                var methinfo=t.GetMethod(funcname);
                if (methinfo != null) 
                {
                   return resolve(methinfo);
                }
            }
            {
                var t = typeof(EntityMirror);
                var methinfo = t.GetMethod(funcname);
                if (methinfo != null)
                {
                    return resolve(methinfo);
                }
            }
            {
                var t = typeof(DrawableMove);
                var methinfo = t.GetMethod(funcname);
                if (methinfo != null)
                {
                    return resolve(methinfo);
                }
            }
          
            Debug.Mess($"function {funcname} not found");
            return d;
        }

        delegate DataSaver convertFunction(string name,DataSaver inData);
        Dictionary<string,convertFunction> customFunctions=new Dictionary<string,convertFunction>();

        /// <summary>
        /// アクションを登録する
        /// [argument]で:引数を指定し、[format]の中に変換器を入れる
        /// 生成されるアクションのキャラクターはformatの先頭にあるアクションになる
        /// </summary>
        /// <param name="functionName"></param>
        /// <param name="data"></param>
        public void registfunction(string functionName, DataSaver data)
        {
            //Console.WriteLine(actionname + " ::newAction");
           
            var inkakko = data.unpackDataD("argument").splitDataS();
            var format = data.unpackDataS("format");

            customFunctions.Add(functionName, (name,inData) =>
            {
                string tyos = format;
                tyos=tyos.Replace("@name@", name);
               // Debug.WriteLine(functionName + " IN SITAO! " + inData.getData()+" asd "+inkakko.Count().ToString());
                //inkakkoで全部変換する

                for (int i = 0; i < inkakko.Count; i++)
                {
                    if (inkakko[i] != "")
                    {
                        var sp = inkakko[i].Split('=');

                        if (sp.Length > 1)
                        {
                            var to = henkanText(sp, inData, i);

                            tyos = tyos.Replace(sp[0], to);
                        }
                        else
                        {
                            var sp2 = new string[] { sp[0], "_" };
                            var to = henkanText(sp2, inData, i);

                            tyos = tyos.Replace(sp2[0], to);
                        }
                    }
                }

               // Debug.WriteLine(functionName + " OUT SITAO! " + tyos);
                return new DataSaver(tyos);
            }
            );
        }

        /// <summary>
        /// 先頭の記号で変なエスケープをつける。入力がない時の処理もするよ
        /// </summary>
        /// <param name="sp">=でスプリットした結果のやつ</param>
        /// <param name="d">与えられたデータ[yoshino:1]の方</param>
        /// <param name="idx">与えられたデータの何番目からとるか</param>
        /// <returns></returns>
        string henkanText(string[] sp, DataSaver d, int idx)
        {
            var res = "_";
            //最初の=以降は無効にする感じ。
            for (int t = 2; t < sp.Length; t++)
            {
                sp[1] += "=" + sp[t];
            }
            var packs=d.getAllPacks();
            while (packs.Count <= idx) { packs.Add("_"); }
            if (sp[1][0] == '+')
            {
                var remed = sp[1].Substring(1);
                var pos = d.unpackDataF(packs[idx], 0);
                res = (float.Parse(remed) + pos).ToString();

            }
            else if (sp[1][0] == '-')
            {
                var remed = sp[1].Substring(1);
                var pos = d.unpackDataF(packs[idx], 0);
                res = (float.Parse(remed) - pos).ToString();

            }
            else if (sp[1][0] == '*')
            {
                var remed = sp[1].Substring(1);
                var pos = d.unpackDataF(packs[idx], 1);
                res = (float.Parse(remed) * pos).ToString();

            }
            else if (sp[1][0] == '/')
            {
                var remed = sp[1].Substring(1);
                var pos = d.unpackDataF(packs[idx], 1);
                res = (float.Parse(remed) / pos).ToString();

            }
            else if (sp[1][0] == '^')
            {
                var remed = sp[1].Substring(1);
                var pos = d.unpackDataF(packs[idx], 1);
                res = (Math.Pow(float.Parse(remed), pos)).ToString();

            }
            else
            {
                res = d.unpackDataS(packs[idx], sp[1]);
            }
            return res;
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
        /// <param name="pack">開けるパック。""で普通に</param>
        public DataSaver MakeAnime(Entity e,string pack="")
        {
            if (pack == "")
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
                return d;
            }
            else 
            {
                var d = this.d.unpackDataD(pack);
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
                return d;
            }
        }
        public string Check() 
        {
            return d.getAllkouzou();
        }
        public List<String> Check2() 
        {

            return d.getAllPacks();
        }
        public string Check3()
        {
            return d.getData();
        }


    }
    /////////////////////////////////////////////////////////////////////////////////////////////////////
    public class AnimBlock
    {
        protected Entity GetTarget(Entity e, string Target, string parts) 
        {
            if (e.world != null)
            {
                {
                    var lis = e.world.Entities;
                    // '/'で分解して、ひとつづつ奥へ潜っていく。初めはworldそれ以降はキャラクター。該当者なしなら何もせん。<-無くてよくね？

                    var named = World.getNamedEntity(Target, lis);
                    if (named.Count == 0)
                    {
                        Debug.WriteLine(ToString() + " Cant Find Target");
                    }
                    else
                    {
                        if (named.Count > 1)
                        {
                            Debug.WriteLine(ToString() + " TargetCOunter Over 2 now=" + named.Count);
                        }
                        foreach (var a in named)
                        {
                            var tag = a.getCharacter().getEntity(parts);
                            if (tag == null)
                            {
                                Debug.WriteLine(ToString() + " Target Parts is null!");
                            }
                            else
                            {
                                return tag;
                            }
                        }
                    }
                }

            }
            return null;
        }
        /// <summary>
        /// X座標を取得
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="parts"></param>
        /// <param name="wari">=Nan で中心点</param>
        /// <returns>__ANIM__</returns>
        protected string GetX(Entity e, string Target, string parts, float wari = float.NaN)
        {
            var tag = GetTarget(e,Target,parts);
            if (tag != null) 
            {
                return tag.gettxy(wari, wari).x.ToString();
            }
            return "";
        }

        /// <summary>
        /// Y座標を取得
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="parts"></param>
        /// <param name="wari">=Nan で中心点</param>
        /// <returns>__ANIM__</returns>
        protected string GetY(Entity e, string Target, string parts, float wari = float.NaN)
        {
            var tag = GetTarget(e, Target, parts);
            if (tag != null)
            {
                return tag.gettxy(wari, wari).y.ToString();
            }
            return "";
        }

        /// <summary>
        /// 幅を取得
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="parts"></param>
        /// <returns>__ANIM__</returns>
        protected string GetW(Entity e, string Target, string parts)
        {
            var tag = GetTarget(e, Target, parts);
            if (tag != null)
            {
                return tag.w.ToString();
            }
            return "";
        }

        /// <summary>
        /// 高さを取得
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="parts"></param>
        /// <returns>__ANIM__</returns>
        protected string GetH(Entity e, string Target, string parts)
        {
            var tag = GetTarget(e, Target, parts);
            if (tag != null)
            {
                return tag.h.ToString();
            }
            return "";
        }

        /// <summary>
        /// 中心点Xを取得
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="parts"></param>
        /// <returns>__ANIM__</returns>
        protected string GetTX(Entity e, string Target, string parts)
        {
            var tag = GetTarget(e, Target, parts);
            if (tag != null)
            {
                return tag.tx.ToString();
            }
            return "";
        }

        /// <summary>
        /// 中心点Yを取得
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="parts"></param>
        /// <returns>__ANIM__</returns>
        protected string GetTY(Entity e, string Target, string parts)
        {
            var tag = GetTarget(e, Target, parts);
            if (tag != null)
            {
                return tag.ty.ToString();
            }
            return "";
        }


        /// <summary>
        /// 角度を取得
        /// </summary>
        /// <param name="Target"></param>
        /// <param name="parts"></param>
        /// <returns>__ANIM__</returns>
        protected string GetDegree(Entity e, string Target, string parts)
        {
            var tag = GetTarget(e, Target, parts);
            if (tag != null)
            {
                return tag.degree.ToString();
            }
            return "";
        }


        /// <summary>
        /// Blockのコンポーネントを発射。する前にSetComponentしておくこと
        /// </summary>
        /// <param name="e"></param>
        /// <param name="time">現在の時間</param>
        /// <param name="mng">マネージャー</param>
        /// <returns></returns>
        public bool Add(Entity e, float time,AnimeBlockManager mng)
        {
            //変数処理はこっちに組み込む
           
            if (AddComponent != null)
            {
                //ここで発動。
                if (time >= StartTime)
                {
                    {
                        var d = new DataSaver(Script);
                        string t = "";
                        bool ok = true;
                        switch (Command)
                        {
                            case "GetX":
                                t = GetX(e, d.splitOneDataS(0, "", ','), d.splitOneDataS(1, "", ','), d.splitOneDataF(2, float.NaN, ','));

                                break;
                            case "GetY":
                                t = GetY(e, d.splitOneDataS(0, "", ','), d.splitOneDataS(1, "", ','), d.splitOneDataF(2, float.NaN, ','));
                                break;

                            case "GetW":
                                t = GetW(e, d.splitOneDataS(0, "", ','), d.splitOneDataS(1, "", ','));
                                break;

                            case "GetH":
                                t = GetH(e, d.splitOneDataS(0, "", ','), d.splitOneDataS(1, "", ','));
                                break;

                            case "GetTX":
                                t = GetTX(e, d.splitOneDataS(0, "", ','), d.splitOneDataS(1, "", ','));
                                return true;

                            case "GetTY":
                                t = GetTY(e, d.splitOneDataS(0, "", ','), d.splitOneDataS(1, "", ','));
                                break;

                            case "GetDegree":
                                t = GetDegree(e, d.splitOneDataS(0, "", ','), d.splitOneDataS(1, "", ','));
                                break;
                            default:
                                ok = false;
                                break;
                        }
                        if (ok == true)
                        {
                            mng.RegistVariable(Target, t);
                            AddComponent = null;
                            return true;
                        }
                    }


                    if (e.world != null)
                    {
                        AddComponent = GetComponentData(mng);//replaceがあるのでもう一度設置
                        if (Target == "")
                        {
                            AddComponent.add(e, time - StartTime + AddCl);
                        }
                        else
                        {
                            var lis = e.world.Entities;
                            // '/'で分解して、ひとつづつ奥へ潜っていく。初めはworldそれ以降はキャラクター。該当者なしなら何もせん。<-無くてよくね？
                          
                            var named = World.getNamedEntity(Target, lis);
                            if (named.Count == 0)
                            {
                                Debug.WriteLine(ToString()+" Cant Find Target");
                            }
                            else
                            {
                                if (named.Count > 1) 
                                {
                                    Debug.WriteLine(ToString() + " TargetCOunter Over 2 now=" + named.Count);
                                }
                                foreach(var a in named) 
                                {
                                    AddComponent.add(a, time - StartTime + AddCl);
                                    //Debug.WriteLine(AddComponent.ToSave().getData()+" AddedTo "+a.name+" andFramed " +(time-StartTime));
                                    break;
                                }
                            }
                        }


                        AddComponent = null;
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// テキストファイルの何行目か
        /// </summary>
        public int Gyo = 0;
        /// <summary>
        /// 作動する時間
        /// </summary>
        public float StartTime = 0;

        /// <summary>
        /// 効果のターゲット
        /// </summary>
        public string Target = "NoTarget";


        /// <summary>
        /// コマンド
        /// </summary>
        public string Command = "NoCommand";


        /// <summary>
        /// モーションを追加するときに追加クロックする時間
        /// </summary>
        public float AddCl = 0;

        /// <summary>
        /// スクリプト
        /// </summary>
        public string Script = "NoScript";
        //TODO:もじをFPからとれるように。
        //summonPicture,"FirePic",10:"Fire",10,"fire",100,100,50,50,20,20,1,1,1,1;
        //Scalechange,"FirePic",10:10,1,1;  

        public Component AddComponent = null;
        public float GetBlockTime(AnimeBlockManager mng)
        {
            var tcomp = AddComponent;
            if (tcomp == null)
            {
                tcomp = GetComponentData(mng);
            }
            if (tcomp == null)
            {
                return 0;
            }
            var res = tcomp.time;
            if (Mathf.isSubClassOf(tcomp.GetType(), typeof(SummonEntity)))
            {
                res += ((SummonEntity)tcomp).LifeTimer;
            }
            else if (Mathf.isSubClassOf(tcomp.GetType(), typeof(SummonCharacter)))
            {
                res += ((SummonCharacter)tcomp).LifeTime;
            }
            else if (Mathf.isSubClassOf(tcomp.GetType(), typeof(Motion)))
            {
                res /= ((Motion)tcomp).speed;
            }
            return res;

        }

        public void SetAddComponent(AnimeBlockManager mng)
        {
            switch (Command)
            {
                case "GetX":
                case "GetY":
                case "GetW":
                case "GetH":
                case "GetTX":
                case "GetTY":
                case "GetDegree":
                   AddComponent= new Component(0);
                    mng.RegistVariable(Target, "");
                    return ;
            }
            AddComponent = GetComponentData(mng);
        }

        /// <summary>
        /// データ置き換えの法則
        /// </summary>
        /// <param name="funcname">関数の名前</param>
        /// <param name="d"></param>
        /// <returns>何もなかったら引数と同時に返される</returns>
        protected virtual Component GetComponentData(AnimeBlockManager mng)
        {
            Component resolve(MethodInfo methinfo)
            {
                var sp = Script.Split(',');
                var param = methinfo.GetParameters();

                List<object> args = new List<object>();
                for (int i = 0; i < param.Count(); i++)
                {

                    var pt = param[i].ParameterType;
                    var conv = TypeDescriptor.GetConverter(pt);
                    //Scriptの引数を解きほぐす
                    if (i < sp.Length)
                    {
                        try
                        {
                            var st = sp[i];
                            foreach (var a in mng.variables) 
                            {
                                float f;
                                if (float.TryParse(a.Value,out f))
                                {
                                    st = st.Replace($"-@{a.Key}@", (-f).ToString());
                                }
                                st =st.Replace($"@{a.Key}@",a.Value);
                                

                            }
                            var conved = conv.ConvertFrom(st);
                            args.Add(conved);
                        }
                        catch (Exception)
                        {
                            if (param[i].HasDefaultValue)
                            {
                                args.Add(param[i].DefaultValue);
                            }
                            else
                            {
                                Debug.WriteLine($"{this.ToString()} 's argument is not collect\n argument number is{i} named {param[i].Name}\n {methinfo.ToString()}");
                                return new Component();
                            }
                        }
                    }
                    else if (param[i].HasDefaultValue)
                    {
                        args.Add(param[i].DefaultValue);
                    }
                    else
                    {
                        //Debug.WriteLine($"{this.ToString()} 's argument is not collect\n argument number {i} named {param[i].Name}\n {methinfo.ToString()}");
                        return new Component();
                    }
                }

                var comp = (Component)methinfo.Invoke(null, args.ToArray());
                return comp;
            }
         
            {
                var t = typeof(AnimeBlockManager);
                var methinfo = t.GetMethod(Command);
                if (methinfo != null)
                {
                    return resolve(methinfo);
                }
            }
            {
                var t = typeof(EntityMove);
                var methinfo = t.GetMethod(Command);
                if (methinfo != null)
                {
                    return resolve(methinfo);
                }
            }
            {
                var t = typeof(EntityMirror);
                var methinfo = t.GetMethod(Command);
                if (methinfo != null)
                {
                    return resolve(methinfo);
                }
            }
            {
                var t = typeof(DrawableMove);
                var methinfo = t.GetMethod(Command);
                if (methinfo != null)
                {
                    return resolve(methinfo);
                }
            }
            if (Command != "")
            {
                Debug.WriteLine($"{this.ToString()} function not found");
            }
            return new Component();
        }




        public DataSaver ToSave()
        {
            var d = new DataSaver();
            d.packAdd("Gyo", Gyo);
            d.packAdd("StartTime", StartTime);
            d.packAdd("Target", Target);
            d.packAdd("Command", Command);
            d.packAdd("Script", Script);
            d.packAdd("AddCl", AddCl);
            return d;
        }

        public void ToLoad(DataSaver d)
        {
            Gyo = (int)d.unpackDataF("Gyo", Gyo);
            StartTime = d.unpackDataF("StartTime", StartTime);
            Target = d.unpackDataS("Target", Target);
            Command = d.unpackDataS("Command", Command);
            Script = d.unpackDataS("Script", Script);
            AddCl = d.unpackDataF("AddCl", AddCl);
        }

        public String ToString()
        {
            return $"{Gyo}@{StartTime},{Target},{Command},{AddCl}:{Script}";
        }

    }

    public class AnimeBlockManager
    {
        /// <summary>
        /// 音を流す。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="volume">=1</param>
        /// <returns>__ANIM__</returns>
        static public Component SE(string path, float volume = 1)
        {
            try
            {
                return SoundComponent.MakeSE(FileMan.SE, path, volume);
            }
            catch (Exception ex)
            {
                Debug.Mess("SoundEffect Load Missed " + path + " \n" + ex.ToString());
                return new Component();
            }
        }

        /// <summary>
        /// BGMを流すコンポーネントを持つエンテティを召喚する。名前はBGM
        /// </summary>
        /// <param name="stop">止める時間</param>
        /// <param name="path"></param>
        /// <param name="volume">=1</param>
        /// <param name="fadein">=-1</param>
        /// <param name="fadeout">=-1</param>
        /// <returns>__ANIM__</returns>
        static public SummonEntity BGM(float stop, string path, float volume, float fadein = -1, float fadeout = -1)
        {
            try
            {
                var bgm = SoundComponent.MakeBGM(FileMan.BGM, path, volume, fadein, fadeout);
                var lifetime = new LifeTimer(stop);

                var e = Entity.make(0, 0, 1, 1, 1, 1);
                bgm.add(e);
                lifetime.add(e);

                return new SummonEntity(e, 0, SoundComponent.BGMname);
            }
            catch (Exception ex)
            {
                Debug.Mess("BGM Load Missed " + path + " \n" + ex.ToString());

                var e = Entity.make(0, 0, 1, 1, 1, 1);
                return new SummonEntity(e, 0, SoundComponent.BGMname);
            }
        }

        /// <summary>
        /// キャラクターをパスから召喚する
        /// </summary>
        /// <param name="lifetime">効果時間</param>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="scale">=1</param>
        /// <returns>__ANIM__</returns>
        static public SummonCharacter SummonCharacter(float lifetime, string path, string name, float scale = 1)
        {
            return new SummonCharacter(path, scale, name, 0, path + name + "summon", lifetime);
        }
        /// <summary>
        /// モーションをパスから追加する。
        /// </summary>
        /// <param name="path">パス</param>
        /// <param name="speed">=1</param>>
        /// <param name="Times">何回繰り返すか=1</param>
        /// <returns>__ANIM__</returns>
        static public Motion Motion(string path, float speed = 1, float Times = 1)
        {
            var matome = new Motion();
            for (int i = 0; i < Times; ++i)
            {
                var m = FileMan.ldMotion(path);
                matome.addmove(m, true);
            }
            matome.speed = speed;
            return matome;
        }




        /// <summary>
        /// 即席のキャラクターを召喚する。textureは"core"に追加される
        /// </summary>
        /// <param name="LifeTimer">効果時間</param>
        /// <param name="texture">テクスチャ</param>
        /// <param name="size">サイズ</param>
        /// <param name="name">名前</param>
        /// <returns>__ANIM__</returns>
        static public SummonEntity SummonICharacter(float LifeTimer, string texture, float size, string name)
        {
            var e = CharaModel.Character.MakeCharacter(texture, 0, 0, size, 0.5f, 0.5f, 0);
            e.name = name;
            return new SummonEntity(e, 0, name, LifeTimer);
        }

        List<AnimBlock> _Blocks = new List<AnimBlock>();
        public List<AnimBlock> Blocks { get { return new List<AnimBlock>(_Blocks); } }

        public Dictionary<string, string> variables = new Dictionary<string, string>();

        /// <summary>
        /// StartTime,TargetName,CommandName:引数
        /// でモーション系の関数を飛ばせる。
        /// </summary>
        /// <param name="InD">未escape状態ので</param>
        public AnimeBlockManager(DataSaver InD)
        {
            //Debug.WriteLine(d.getData());

            string[] s = InD.getData().Split('\n');
            float Ttime = 0;
            for (int i = 0; i < s.Length; ++i)
            {

                var newb = new AnimBlock();

                newb.Gyo = i + 1;

                var LeftData = new DataSaver(DataSaver.escapen(s[i])).splitOneDataS(0, "", ':');
                if (LeftData != "")
                {
                    if (LeftData.Length >= 1 && LeftData.Substring(0, 1) == "#")
                    {
                        switch (LeftData.Substring(1))
                        {
                            case "AddTime":
                                Ttime += new DataSaver(DataSaver.escapen(s[i])).splitOneDataF(1, 0, ':');
                                break;
                            case "StartTime":
                                _StartTime += new DataSaver(DataSaver.escapen(s[i])).splitOneDataF(1, 0, ':');
                                break;
                            case "EndTime":
                                _EndTime += new DataSaver(DataSaver.escapen(s[i])).splitOneDataF(1, 0, ':');
                                break;
                        }
                        continue;

                    }
                    else if (LeftData.Length >= 2 && LeftData.Substring(0, 2) == "//")
                    {
                        continue;
                    }
                    var Left = new DataSaver(LeftData);
                    newb.Script = new DataSaver(DataSaver.escapen(s[i])).splitOneDataS(1, "", ':');

                    newb.StartTime = Left.splitOneDataF(0, 0, ',')+Ttime;
                    newb.Target = Left.splitOneDataS(1, "", ',');
                    newb.Command = Left.splitOneDataS(2, "", ',');
                    newb.AddCl = Left.splitOneDataF(3, 0, ',');
                    _Blocks.Add(newb);
                }
            }

        }
        /// <summary>
        /// 開始処理。ブロック・変数を全部リセット
        /// </summary>
        public void Start()
        {
            _Time = 0;
            _MaxTime = 0;
            foreach (var a in _Blocks)
            {
                a.SetAddComponent(this);

                _MaxTime = Math.Max(a.GetBlockTime(this) + a.StartTime, _MaxTime);
            }
            variables.Clear();
        }


        protected float _Time = 0;
        /// <summary>
        /// マネージャーの中の時間
        /// </summary>
        public float Time { get { return _Time; } }
        /// <summary>
        /// 読み取るだけよ。最大時間
        /// </summary>
        protected float _MaxTime = 0;

        /// <summary>
        /// 最大の時間
        /// </summary>
        public float MaxTime { get { return _MaxTime; } }

        /// <summary>
        /// アニメーションの開始時間
        /// </summary>
        public float StartTime{
            get
            {
                if (_StartTime < 0) { return 0; }
                return _StartTime;
            }
        }

        /// <summary>
        /// アニメーションの終了時間
        /// </summary>
        public float EndTime
        {
            get
            {
                if (_EndTime < 0) { return _MaxTime; }
                return _EndTime;
            }
        }

        /// <summary>
        /// 開始時間
        /// </summary>
        protected float _StartTime = -1;
        /// <summary>
        /// 終了時間
        /// </summary>
        protected float _EndTime = -1;

        /// <summary>
        /// アニメーションを進める
        /// </summary>
        /// <param name="ManageEntity">コンポーネントをつかさどるエンテティ</param>
        /// <param name="cl"></param>
        public void update(Entity ManageEntity,float cl) 
        {
            _Time += cl;
            foreach (var a in _Blocks)
            {
                if (a.Add(ManageEntity, _Time,this))
                {
               //    Debug.WriteLine(_Time+" Block Invoked!!:" + a.ToString());
                }
            }
        }
        public void RegistVariable(string name,string value) 
        {
            if (variables.ContainsKey(name) == false)
            {
                variables.Add(name, value);
            }
            else 
            {
                variables[name] = value;
            }
        }

        public string ToString() 
        {
            var res = "";
            foreach (var a in _Blocks) 
            {
            res+= a.ToString()+"\n";
            }
            return res;
        }

    }
}
