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
        /// <summary>
        /// 音を流す。
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <param name="volume">=1</param>
        /// <returns>__ANIM__</returns>
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
        /// <returns>__ANIM__</returns>
        static public SummonEntity BGM(float time,float stop, string path, float volume,float fadein=-1)
        {
            try
            {
                var bgm = SoundComponent.MakeBGM(FileMan.BGM, path, volume, fadein);
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
        /// <returns>__ANIM__</returns>
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
        /// <returns>__ANIM__</returns>
        static public SummonMotion SummonMotion(float time, string path, string tag, float speed = 1
            ,float addcl=0)
        {
            return new SummonMotion(path, speed, tag, time,addcl, path  + "summon");
        }

        /// <summary>
        /// コンポーネントをデータから召喚する
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="tag">対象</param>
        /// <param name="addcl">召喚された瞬間に経過する時間=0</param>
        /// <param name="componentData">コンポーネントのデータ</param>
        /// <returns>__ANIM__</returns>
        static public SummonComponent SummonComponent(float time, string tag
            , float addcl = 0, string componentData="")
        {
            return new SummonComponent(Component.ToLoadComponent(new DataSaver(componentData))
                ,  tag, time, addcl, tag + "summon");
        }

        /// <summary>
        /// 即席のキャラクターを召喚する。textureは"core"に追加される
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="texture"></param>
        /// <param name="size"></param>
        /// <param name="name"></param>
        /// <returns>__ANIM__</returns>
        static public SummonEntity SummonICharacter(float time,string texture,float size,string name)
        {
            var e=CharaModel.Character.MakeCharacter(texture, 0, 0, size, 0.5f, 0.5f, 0);
            e.name= name; 
            return new SummonEntity(e,time);
        }


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

    class AnimBlock
    {
        /// <summary>
        /// テキストファイルの何行目か
        /// </summary>
        int gyo;
        /// <summary>
        /// 作動する時間
        /// </summary>
        float starttime;

        /// <summary>
        /// 効果のターゲット
        /// </summary>
        string Target;


        /// <summary>
        /// スクリプト
        /// </summary>
        string command;

        /// <summary>
        /// スクリプト
        /// </summary>
        string script;
        //TODO:もじをFPからとれるように。
        //summonPicture,"FirePic",10:"Fire",10,"fire",100,100,50,50,20,20,1,1,1,1;
        //Scalechange,"FirePic",10:10,1,1;  
        
    }

    public class AnimeLoader2
    {
        /// <summary>
        /// 音を流す。
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="target"></param>
        /// <param name="path"></param>
        /// <param name="volume">=1</param>
        /// <returns>__ANIM__</returns>
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
        /// <returns>__ANIM__</returns>
        static public SummonEntity BGM(float time, float stop, string path, float volume, float fadein = -1)
        {
            try
            {
                var bgm = SoundComponent.MakeBGM(FileMan.BGM, path, volume, fadein);
                var lifetime = new LifeTimer(stop);

                var e = Entity.make(0, 0, 1, 1, 1, 1);
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
        /// <returns>__ANIM__</returns>
        static public SummonCharacter SummonCharacter(float time, string path, string name, float scale = 1)
        {
            return new SummonCharacter(path, scale, name, time, path + name + "summon");
        }
        /// <summary>
        /// モーションをパスから召喚する
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="path">パス</param>
        /// <param name="tag">対象</param>
        /// <param name="speed">=1</param>
        /// <param name="addcl">召喚された瞬間に経過する時間=0</param>
        /// <returns>__ANIM__</returns>
        static public SummonMotion SummonMotion(float time, string path, string tag, float speed = 1
            , float addcl = 0)
        {
            return new SummonMotion(path, speed, tag, time, addcl, path + "summon");
        }

        /// <summary>
        /// コンポーネントをデータから召喚する
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="tag">対象</param>
        /// <param name="addcl">召喚された瞬間に経過する時間=0</param>
        /// <param name="componentData">コンポーネントのデータ</param>
        /// <returns>__ANIM__</returns>
        static public SummonComponent SummonComponent(float time, string tag
            , float addcl = 0, string componentData = "")
        {
            return new SummonComponent(Component.ToLoadComponent(new DataSaver(componentData))
                , tag, time, addcl, tag + "summon");
        }

        /// <summary>
        /// 即席のキャラクターを召喚する。textureは"core"に追加される
        /// </summary>
        /// <param name="time">召喚する時間</param>
        /// <param name="texture"></param>
        /// <param name="size"></param>
        /// <param name="name"></param>
        /// <returns>__ANIM__</returns>
        static public SummonEntity SummonICharacter(float time, string texture, float size, string name)
        {
            var e = CharaModel.Character.MakeCharacter(texture, 0, 0, size, 0.5f, 0.5f, 0);
            e.name = name;
            return new SummonEntity(e, time);
        }


        DataSaver d;
        public AnimeLoader2(DataSaver d)
        {
            //Debug.WriteLine(d.getData());

            {
                var config = d.unpackDataD("config");

                var func = config.unpackDataD("function");

                foreach (var a in func.getAllPacks())
                {
                    registfunction(a, func.unpackDataD(a));
                }

                d = d.PackRemoves("config");
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

                    res = res.PackReplace(a, "", customFunctions[funcname].Invoke(outPackname, d.unpackDataD(a)).getData());
                    replaced = true;
                }
            }
            return res;
        }
        /// <summary>
        /// データを置き換える。
        /// </summary>
        protected DataSaver replaseData(DataSaver d, out bool replaced)
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
                //関数以外だったら
                if (funcname == "")
                {

                    bool Chiledreplaced;
                    res = res.PackReplace(a, a, replaseData(d.unpackDataD(a), out Chiledreplaced).getData());
                    if (Chiledreplaced) replaced = true;
                }
                else
                {
                    bool Chiledreplaced;
                    var recursedD = replaseData(d.unpackDataD(a), out Chiledreplaced);
                    Debug.WriteLine(funcname + " replaced!!!");
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
        protected string getfuncname(string packname, out string outPackname)
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
        protected virtual DataSaver replace(string funcname, DataSaver d)
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
                    if (i < sp.Count)
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
                var methinfo = t.GetMethod(funcname);
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

        delegate DataSaver convertFunction(string name, DataSaver inData);
        Dictionary<string, convertFunction> customFunctions = new Dictionary<string, convertFunction>();

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

            customFunctions.Add(functionName, (name, inData) =>
            {
                string tyos = format;
                tyos = tyos.Replace("@name@", name);
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
            var packs = d.getAllPacks();
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
            var res = new DataSaver();
            var config = new DataSaver();
            config.packAdd("maxtime", 1);
            config.indent();
            res.packAdd("config", config);

            res.packAdd("0", new Utils.SummonEntity(new Entity(), 0).ToSave().indent());
            res.packAdd("1", new Utils.SummonCharacter("", 1, "", 0).ToSave().indent());
            res.packAdd("2", new Utils.SummonComponent(new Component(), "", 0).ToSave().indent());
            res.packAdd("3", new Utils.SummonMotion("", 1, "", 0).ToSave().indent());

            return res;
        }
        /// <summary>
        /// アニメを起動する
        /// </summary>
        /// <param name="e">アニメの進行役を務めるエンテティ</param>
        /// <param name="pack">開けるパック。""で普通に</param>
        public DataSaver MakeAnime(Entity e, string pack = "")
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
}
