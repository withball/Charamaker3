using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Vortice;
using Vortice.DCommon;
using Vortice.Direct2D1;
using Vortice.Mathematics;
using Color = System.Drawing.Color;
using Charamaker3.CharaModel;
using System.Threading;

namespace Charamaker3
{
    static public class FileMan
    {
        /// <summary>
        /// ファイルマネージャーのルートパス。サウンドエンジンにも影響。
        /// </summary>
        static public string rootpath = @".\";

        /// <summary>
        /// スラッシュをバックスラッシュに変換する。
        /// だってモーションとかの登録を入力パッスにしてるからさ、
        /// \/が混じってると二回ロードしちゃうんだよな
        /// </summary>
        /// <returns></returns>
        public static string slashFormat(string path)
        {
            return path.Replace(@"/", @"\");
        }
        /// <summary>
        /// バックスラッシュをスラッシュに変換する。
        /// これでfolder\nantokaが勘違いされなくなる
        /// </summary>
        /// <returns></returns>
        public static string nonBackSlash(string path)
        {
            return path.Replace(@"\", @"/");
        }
        /// <summary>
        /// .bmpをつける。.pngならそのまま
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static string bmpExtention(string name)
        {
            var aa = Path.GetExtension(name);

            if (aa != ".bmp" && aa != ".png" && aa != ".jpg") name += ".bmp";
            return name;
        }
        /// <summary>
        /// 何もないことを示す魔法の言葉
        /// </summary>
        public const string nothing = "nothing";
        #region texture

        static public void setupTextureLoader(Display d)
        {
            texs.Clear();
            baseRender = d.render;
            Dictionary<string, System.Drawing.Color> basebitnames = new Dictionary<string, System.Drawing.Color>
                {
                    {"red",System.Drawing.Color.Red },{"blue",System.Drawing.Color.Blue },{"green",System.Drawing.Color.Green },
                    {"white",System.Drawing.Color.White },{"gray",System.Drawing.Color.Gray },{"black",System.Drawing.Color.Black },
                    {"yellow",System.Drawing.Color.Yellow },{"cyan",System.Drawing.Color.Cyan },{"purple",System.Drawing.Color.Purple },
                    {"aqua",System.Drawing.Color.Aqua },{"brown",System.Drawing.Color.Brown },{"crimson",System.Drawing.Color.Crimson },
                    {"pink",System.Drawing.Color.Pink },{"orange",System.Drawing.Color.Orange },{"indigo",System.Drawing.Color.Indigo }
                ,{"trans",System.Drawing.Color.FromArgb(0,0,0,0) }

                };
            foreach (var a in basebitnames)
            {
                registBit(a.Key, a.Value);
            }
            //nothingを作る
            {
                CreateBitmapSS.Wait();
                int stride = 3 * sizeof(int);
                using (var tempStream = new DataStream(stride*3, true, true))
                {
                    List<Color> cols = new List<Color>
                    {
                    Color.FromArgb(255,255,255),Color.FromArgb(128,128,128),Color.FromArgb(255,0,0),
                    Color.FromArgb(255, 0 ,255),Color.FromArgb(0,255,0)    ,Color.FromArgb(255,255,0),
                    Color.FromArgb(0,0,255)    ,Color.FromArgb(0  ,255,255),Color.FromArgb(0,0,0)
                    };
                    for (int i = 0; i < 9; i++)
                    {
                        var color = cols[i];
                        int rgba = color.R | (color.G << 8) | (color.B << 16) | (color.A << 24);
                        tempStream.Write(rgba);
                    }
                    var bitmapProperties = new BitmapProperties(new PixelFormat(Vortice.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                    
                    registBmp(nothing, baseRender.CreateBitmap(new System.Drawing.Size(3, 3), tempStream.BasePointer, sizeof(int), bitmapProperties));
                  
                }
                CreateBitmapSS.Release();
            }
        }
        //新しいbmpを作るために必要なrender
        static ID2D1RenderTarget baseRender;
        //画像を読み込むとき、透明とみなす色。
        static public Color transColor = Color.FromArgb(0, 254, 254);

        /// <summary>
        /// 読み込んだテクスチャーを保存しとく
        /// </summary>
        static Dictionary<string, ID2D1Bitmap> texs = new Dictionary<string, ID2D1Bitmap>();


        /// <summary>
        /// texsに色のついたビットを追加する。
        /// </summary>
        /// <param name="bitname">bitname+"bit"</param>
        /// <param name="color">色</param>
        static private void registBit(string bitname, System.Drawing.Color color)
        {
            using (var tempStream = new DataStream(1, true, true))
            {
                CreateBitmapSS.Wait();
                int rgba = color.R | (color.G << 8) | (color.B << 16) | (color.A << 24);
                tempStream.Write(rgba);
                var bitmapProperties = new BitmapProperties(new PixelFormat(Vortice.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                registBmp(bitname + "bit", baseRender.CreateBitmap(new System.Drawing.Size(1, 1), tempStream.BasePointer, sizeof(int), bitmapProperties));
                CreateBitmapSS.Release();
            }

        }

        /// <summary>
        /// bmpをtexsに登録する
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bmp"></param>
        /// <param name="dispouwagaki">同じ名前のbmpを登録するときdisposeするか。Renderからのbmpを登録してる場所にはfalseじゃないとだめ</param>
        static public void registBmp(string name, ID2D1Bitmap bmp, bool dispouwagaki = true)
        {
            {
                name = slashFormat(name);
                name = bmpExtention(name);
            }

            if (texs.ContainsKey(name))
            {

                if (dispouwagaki) texs[name]?.Dispose();
                texs[name] = bmp;
            }
            else
            {
                //if (bmp != null) Console.WriteLine(bmp.PixelSize.ToString());
                Debug.WriteLine(name + " load sitao!");
                texs.Add(name, bmp);
            }
        }


        /// <summary>
        /// bmpがnullか調べる
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static public bool bmpisnull(ID2D1Bitmap bmp)
        {

            return bmp == null;
        }


        private static SemaphoreSlim CreateBitmapSS = new SemaphoreSlim(1, 1);

        /// <summary>
        /// bitmapテクスチャーを読み込む。既に読み込んでいた場合は読み込まずに返す。
        /// .bmpはつけてもつけなくてもいい
        /// </summary>
        /// <param name="file">.\tex\に続くファイルパス</param>
        /// <param name="reset">強制的に再読み込みする</param>
        /// <returns>clmcolを透明にしたビットマップ</returns>
        static public ID2D1Bitmap ldtex(string file, bool reset = false)
        {
            {
                file = slashFormat(file);
                file = bmpExtention(file);
            }

            if (!texs.ContainsKey(file) || reset)
            {
                string path = rootpath+@"tex\" + file;
                if (!File.Exists(path))
                {
                    Debug.WriteLine("texture " + path + " not exists");
                    registBmp(file, null);
                    return texs[bmpExtention(nothing)];
                }

                // System.Drawing.Imageを使ってファイルから画像を読み込む
                {
                    using (var bitmap = (System.Drawing.Bitmap)System.Drawing.Image.FromFile(path))
                    {
                        // BGRA から RGBA 形式へ変換する
                        // 1行のデータサイズを算出
                        int stride = bitmap.Width * sizeof(int);
                        using (var tempStream = new DataStream(bitmap.Height * stride, true, true))
                        {
                            // 読み込み元のBitmapをロックする
                            var sourceArea = new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height);
                            var bitmapData = bitmap.LockBits(sourceArea, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                            // 変換処理
                            for (int y = 0; y < bitmap.Height; y++)
                            {
                                int offset = bitmapData.Stride * y;
                                for (int x = 0; x < bitmap.Width; x++)
                                {

                                    // 1byteずつデータを読み込む
                                    byte B = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                    byte G = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                    byte R = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                    byte A = Marshal.ReadByte(bitmapData.Scan0, offset++);
                                    //Console.WriteLine(B + " " + G + " " + R + " " + A);
                                    byte a = 0;
                                    //透明
                                    int gaba = a | (a << 8) | (a << 16) | (a << 24);
                                    //tempStream.Write(gaba);


                                    int rgba = R | (G << 8) | (B << 16) | (A << 24);
                                    if (R == transColor.R && G == transColor.G && B == transColor.B) tempStream.Write(gaba);
                                    else
                                        tempStream.Write(rgba);


                                }
                            }
                            // 読み込み元のBitmapのロックを解除する
                            bitmap.UnlockBits(bitmapData);
                            tempStream.Position = 0;

                            // 変換したデータからBitmapを生成して返す
                            CreateBitmapSS.Wait();
                            var size = new System.Drawing.Size(bitmap.Width, bitmap.Height);
                            var bitmapProperties = new BitmapProperties(new PixelFormat(Vortice.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));

                            var result = baseRender.CreateBitmap(size, tempStream.BasePointer, stride, bitmapProperties);
                            registBmp(file, result);
                            CreateBitmapSS.Release();
                        }
                    }
                }

            }
            // Console.WriteLine(texs.Count() + "texcount");
            return texs[file];
        }
        /// <summary>
        /// テクスチャーの大きさだけ取得する
        /// </summary>
        /// <param name="file">.\tex\に続くファイルパス</param>
        /// <param name="reset">強制的に再読み込みする</param>
        /// <returns>大きさ</returns>
        static public FXY texSize(string file, bool reset = false)
        {
            var tex = ldtex(file,reset);
            if (tex == null) 
            {
                return new FXY(0, 0);
            }
            return new FXY(tex.Size.Width, tex.Size.Height);
        }
        #endregion

        static public string dialog(string filename,string ext)
        {

            System.Windows.Forms.SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = @".\";
            sfd.FileName=filename+ DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + "_" + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second +ext;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                return sfd.FileName;
            }
            return @".\temp.temp";
        }



        #region Random

        static public Random r = new Random();
        /// <summary>
        /// 0~wの範囲でランダムな整数を発生させる
        /// </summary>
        /// <param name="w"></param>
        /// <param name="minusToo">-w~wの範囲にする</param>
        /// <returns></returns>
        static public float whrandhani(float w, bool minusToo = false)
        {
            if (minusToo)
            {

                return (float)r.NextDouble() * w * 2 - w;
            }
            return (float)r.NextDouble() * w;

        }
        /// <summary>
        /// Listからランダムに一つピックする
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        static public T pickone<T>(List<T> list)

        {
            if (list.Count > 0)
            {
                return list[r.Next() % list.Count];
            }
            return default(T);
        }
        /// <summary>
        /// 配列からランダムに一つピックする
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        static public T pickone<T>(params T[] list)

        {
            if (list.Length > 0)
            {
                return list[r.Next() % list.Length];
            }
            return default(T);
        }

        /// <summary>
        /// nより小さい自然数数(0含む)を返す
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        static public int randmods(int w)
        {
            return r.Next() % w;

        }
        /// <summary>
        /// +1か-1を返す
        /// </summary>
        /// <param name="per">+1を返す確率</param>
        /// <returns></returns>
        static public float plusminus(float per = 50)
        {
            var a = r.NextDouble() * 100;
            if (per >= a)
            {
                return 1;
            }
            return -1;

        }
        /// <summary>
        /// +1か-1を返す
        /// </summary>
        /// <param name="ok">+1を返すか</param>
        /// <param name="plus">ok=trueの解き返すほう</param>
        /// <returns></returns>
        static public float plusminus(bool ok, bool plus = true)
        {
            if (plus)
            {
                if (ok) return 1;
                return -1;
            }
            if (ok) return -1;
            return 1;

        }
        /// <summary>
        /// なんとなくの確率でTrueを返す
        /// </summary>
        /// <param name="per">パーセント(100で確実よもちろん)</param>
        /// <returns></returns>
        static public bool percentin(float per)
        {
            var a = (double)r.Next() / (double)int.MaxValue * 100;
            return per >= a;
        }
        #endregion

        #region Sound

        /// <summary>
        /// SEを再生するときに使う方
        /// </summary>
        static public SoundEngine SE { get { return GetSoundEngine("SE"); } }
        /// <summary>
        /// BGMを再生するときに使うやつ
        /// </summary>
        static public SoundEngine BGM { get { return GetSoundEngine("BGM"); } }
        static List<SoundEngine> _SoundEngines=new List<SoundEngine>();
        /// <summary>
        /// すべてのサウンドエンジン
        /// </summary>
        static public List<SoundEngine> SoundEngines  { get { return new List<SoundEngine>(_SoundEngines); } }
        /// <summary>
        /// Set Up Sound Engine
        /// </summary>
        static public void SoundSetUP()
        {
            //Debug.WriteLine(" EEEEEEEEEEEEEEEEEEEEEEE");
            _SoundEngines.Add(new SoundEngine("SE"));
            _SoundEngines.Add(new SoundEngine("BGM"));
        }
        /// <summary>
        /// サウンドエンジンを返す。何もないのを指定したら一番最初のを返す。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        static public SoundEngine GetSoundEngine(string name) 
        {
            foreach (var a in SoundEngines)
            {
                if (a.name == name) 
                {
                    return a;
                }
            }
            if (SoundEngines.Count > 0) 
            {
                return SoundEngines[0];
            }
            return null;
        }

        #endregion


        #region DataSaverManager

        /// <summary>
        /// ロード済みのデータセーバーのパス。消せばリロードしてくれる
        /// </summary>
        static public Dictionary<string,DataSaver> LoadedDS = new Dictionary<string, DataSaver>();

        

        /// <summary>
        /// データセーバーをロードし、アセット的に保存しておく。
        /// </summary>
        /// <param name="path">自由なパス</param>
        /// <param name="reset">強制的にロードするか</param>
        /// <param name="ext">拡張子</param>
        /// <returns></returns>
        static public DataSaver loadDS(string path,bool reset=false,string ext=".txt",bool escape=true) 
        {
            DataSaver res;
            path = slashFormat(path);
            if (LoadedDS.ContainsKey(path)==false)
            {
                Debug.WriteLine(path + " LoadAndRegist!");
                res = DataSaver.loadFromPath(path, escape, ext);
                LoadedDS.Add(path, res);

            }
            else if (reset)
            {
                Debug.WriteLine(path + " RE LoadAndRegist!");
                res = DataSaver.loadFromPath(path, escape, ext);
                LoadedDS[path] = res;
            } 
            else 
            {
                res = LoadedDS[path];
            }

            return res;

        }
        /// <summary>
        /// キャラクターをロードする
        /// </summary>
        /// <param name="path">パス。\characer\が自動で入る。</param>
        /// <param name="reset"></param>
        /// <returns></returns>
        static public Entity loadCharacter(string path, bool reset = false) 
        {
            var d=loadDS(rootpath+@"character\"+path,reset,".ctc");
            return Entity.ToLoadEntity(d);
        }

        /// <summary>
        /// キャラクターをロードする
        /// </summary>
        /// <param name="path">パス。\characer\が自動で入る。</param>
        /// <param name="reset"></param>
        /// <returns></returns>
        static public Character ldCharacter(string path, bool reset = false)
        {
            var d = loadCharacter(path,reset);
            return d.getcompos<Character>()[0];
        }

        /// <summary>
        /// キャラクターをロードする
        /// </summary>
        /// <param name="path">パス。\motion\が自動で入る。</param>
        /// <returns>もう一回ファイルをロードしなおす</returns>
        static public MotionSaver loadMotion(string path, bool reset = false)
        {
            var d = loadDS(rootpath+@"motion\" + path, reset, ".ctm",false);
            var res = MotionSaver.ToLoad(d);
            return res;
        }


        /// <summary>
        /// モーションをロードする
        /// </summary>
        /// <param name="path">自由なパス</param>
        /// <param name="reset"></param>
        /// <returns></returns>
        static public Motion ldMotion(string path,bool reset=false)
        {
          
            return loadMotion(path,reset).Motion;
        }
        /// <summary>
        /// モーションをセーブする
        /// </summary>
        /// <param name="path">自由なパス</param>
        /// <returns></returns>
        static public void saveMotion(string path, string script, Motion motion)
        {
            new MotionSaver(script, motion).ToSave().saveToPath(path, ext: ".ctm");

        }

        #endregion
    }
    public class MotionSaver
    {
        public string Script="";
        public Motion Motion=new Motion();
        public MotionSaver(string s, Motion c)
        {
            Script  = s;
            Motion = c;
        }
        public DataSaver ToSave() 
        {
            var res = new DataSaver();
            res.packAdd("Script", Script);
            res.linechange();
            res.packAdd("Motion", Motion.ToSave());
            return res;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="d">Escapeしないでね♡</param>
        /// <returns></returns>
        static public MotionSaver ToLoad(DataSaver d)
        {
            var script = d.unpackDataS("Script", "");
            d=d.escaped();
            var res = new MotionSaver(script
                ,Component.ToLoadComponent<Motion>(d.unpackDataD("Motion")));
            if (res.Motion == null) { res.Motion = new Motion(1,false); }
            return res;
        }

    }

    /// <summary>
    /// [name]{naiyou}をロードするクラス。
    /// [name]{naiyou2}
    /// って感じで二つ並んでたら基本上しか読み取れない。
    /// 元々セーブデータ用だしね。
    /// [name]の方は別に保存されてない
    /// []がディレクトリで{}が中に書いてあるファイルorディレクトリのイメージ。
    /// </summary>
    public class DataSaver
    {
        string Data = "";

        /// <summary>
        /// 最後の所に改行を追加する
        /// </summary>
        public void linechange(int cou = 1)
        {
            Data += new string('\n', cou);
        }

        /// <summary>
        /// 全体をインデントする
        /// </summary>
        /// <param name="num"></param>
        /// <returns>便利だし、自身を返す</returns>
        public DataSaver indent(int num = 1)
        {
            var s = Data.Split('\n');
            string res = "";
            for (int i = 0; i < s.Length; i++)
            {
                res += new string(' ', num) + s[i] + "\n";
            }
            Data = res;
            return this;
        }

        /// <summary>
        /// 改行->消す。
        /// \n->改行。
        /// に変換する
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string escapen(string s)
        {
            s = s.Replace("\n", "");
            s = s.Replace("\r", "");
            s = s.Replace(@"\n", "\n");

            return s;
        }
        /// <summary>
        /// 何もない行(改行とスペースしかないところ)を消す
        /// スペース括弧閉じとかも
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string nanimonaitokokesi(string s)
        {
            var res = "";
            var lis = s.Split('\n');
            for (int i = 0; i < lis.Length; i++)
            {
                var a = lis[i];
                var b = a.Replace(" ", "");

                if (b.Length > 0)
                {
                    res += a;
                    if (i < lis.Length - 1) res += "\n";
                }
            }
            return res;
        }


        /// <summary>
        /// データをエスケープする。データを読み取る前に一回行ってね
        /// 改行->消す。
        /// \n->改行。
        /// に変換する
        /// </summary>
        /// <param name="s"></param>
        /// <returns>そのまま自分を返す。便利なだけ</returns>
        public DataSaver escaped()
        {
            Data = escapen(Data);

            int count = 0;
            while (1 == 1)
            {
                var i=Data.IndexOf("[]");
                if (i > 0)
                {
                    var nd=Data.Substring(0, i);
                    nd += $"[__{count++}__]";
                    if (i + 2 < Data.Length)
                    {
                     nd+=   Data.Substring(i + 2, Data.Length - (i + 2));
                    }
                    Data = nd;
                }
                else 
                {
                    break;
                }
            }
            return this;
        }

        /// <summary>
        /// 書いてある内容をもらう。
        /// </summary>
        /// <returns></returns>
        public string getData()
        {
            return Data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="escape">読み込んだ奴をエスケープするか</param>
        /// <param name="ext">拡張子</param>
        /// <returns></returns>
        static public DataSaver loadFromPath(string path, bool escape = true, string ext = ".txt")
        {
            path = FileMan.slashFormat(path);
            if (Path.GetExtension(path) != ext)
            {
                path = path + ext;
            }
            DataSaver res;
            try
            {
                using (var reader = new StreamReader(path))
                {
                    res = new DataSaver(reader.ReadToEnd());
                }
            }
            catch (Exception e)
            {
                res = new DataSaver("ふぁいるないよないよ");
                Debug.WriteLine(path + " sippai" + e.ToString());
            }
            if (escape)
            {
                res.escaped();
            }
            return res;

        } /// <summary>
          /// 
          /// </summary>
          /// <param name="path"></param>
          /// <param name="ext">拡張子</param>
          /// <returns></returns>
        public void saveToPath(string path, string ext = ".txt")
        {
            if (Path.GetExtension(path) != ext)
            {
                path = path + ext;
            }
            try
            {

                using (var writer = new StreamWriter(path))
                {
                    writer.Write(Data);
                }
                Debug.WriteLine(path + " Savekanryou!");

            }
            catch (Exception e)
            {
                Debug.WriteLine(path + " savesippai " + e.ToString());
            }
        }
        public DataSaver(string data = "")
        {
            Data = data;
        }
        /// <summary>
        /// 今のデータに新しいパックを追加する
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="kaigyo"></param>
        public void packAdd(string name, string data, bool kaigyo = false)
        {
            string k = "";
            if (kaigyo) k = "\n";
            Data += "[" + name + "]" + "{" + k + data + "}" + k;
        }

        /// <summary>
        /// こっちのバージョンもある
        /// </summary>
        /// <param name="name"></param>
        /// <param name="d"></param>
        /// <param name="kaigyo"></param>
        public void packAdd(string name, DataSaver d, bool kaigyo = true)
        {
            packAdd(name, d.Data, kaigyo);
        }
        /// <summary>
        /// こっちのバージョンもある
        /// </summary>
        /// <param name="name"></param>
        /// <param name="d"></param>
        /// <param name="kaigyo"></param>
        public void packAdd(string name, bool d, bool kaigyo = false)
        {
            packAdd(name, d.ToString(), kaigyo);
        }

        /// <summary>
        /// Enumバージョンもある
        /// </summary>
        /// <param name="name"></param>
        /// <param name="d"></param>
        /// <param name="kaigyo"></param>
        public void packAdd<T>(string name, int d, bool kaigyo = false)
        {
            packAdd(name, ((int)d).ToString(), kaigyo);
        }

        /// <summary>
        /// こっちのバージョンもある
        /// </summary>
        /// <param name="name"></param>
        /// <param name="d"></param>
        /// <param name="kaigyo"></param>
        public void packAdd<T>(string name, T d, bool kaigyo = false)
            where T : IFormattable
        {
            packAdd(name, d.ToString(), kaigyo);
        }
        /// <summary>
        /// パックを削除したデータを新しく作る。
        /// </summary>
        /// <param name="packs"></param>
        /// <returns>削除が完了したデータ</returns>

        public DataSaver PackRemoves(params string[] packs) 
        {
            var res = new DataSaver(this.getData());
            foreach (var a in packs)
            {

                int st,end;
                res.PackSearch(a,out st,out end);
                var n=res.Data.Substring(0,st);
                if (end < res.Data.Length)
                {
                    n += res.Data.Substring(end + 1, res.Data.Length - end - 1);
                 //   n += res.Data.Substring(end+1, res.Data.Length);
                }
                res = new DataSaver(n);
            }
            return res;
        }

        /// <summary>
        /// 特定のパックを新しいものに置き換える
        /// </summary>
        /// <param name="remove">置き換えるパック</param>
        /// <param name="newname">新しい名前=""でデータをパックせずに置く</param>
        /// <param name="newdata">新しいデータ</param>
        /// <returns></returns>

        public DataSaver PackReplace(string remove, string newname, string newdata)
        {
            var res = new DataSaver(this.getData());
            {
                int st, end;
                res.PackSearch(remove, out st, out end);

                var n = res.Data.Substring(0, st);
                if (newname != "")
                {
                    n += "[" + newname + "]" + "{" + newdata + "}";
                }
                else 
                {
                    n +=  newdata ;
                }
                if (end < res.Data.Length)
                {
                    n += res.Data.Substring(end+1, res.Data.Length-end-1);
                }
                res = new DataSaver(n);
            }
            return res;
        }

        /// <summary>
        /// データの中にパックがある前提ね。文字にアンパックする
        /// </summary>
        /// <param name="name">対象</param>
        /// <param name="nothing">もし中身がなかった時に返す値</param>
        /// <returns></returns>
        public string unpackDataS(string name, string nothing = "")
        {
            name = "[" + name + "]";
            int hiraki = 0;
            bool findname = false;
            int start = -1;
            for (int idx = 0; idx < Data.Length; idx++)
            {
                if (hiraki == 0)
                {
                    if (start == -1 && !findname)
                    {
                        if (idx + name.Length < Data.Length)
                        {
                            if (Data.Substring(idx, name.Length) == name)
                            {
                                findname = true;
                            }
                        }
                    }
                    else
                    {

                    }
                }
                if (Data[idx] == '|')
                {
                    if (findname)
                    {
                        hiraki += 1;
                        findname = false;
                        start = idx + 1;
                    }
                }
                if (Data[idx] == '{')
                {
                    hiraki += 1;
                    if (findname)
                    {
                        findname = false;
                        start = idx + 1;
                    }
                }
                if (Data[idx] == '}')
                {
                    hiraki -= 1;
                    hiraki = Math.Max(0, hiraki);
                    if (start != -1 && hiraki == 0)
                    {
                        var dd = idx - start;
                        if (dd == 0)
                        {
                            return nothing;
                        }
                        /*if (escape)エスケープをやってた時の名残
                        {
                            return escapen(Data.Substring(start, dd));
                        }
                        else*/
                        {
                            return Data.Substring(start, dd);
                        }
                        start = -1;
                    }
                }
            }
            if (hiraki > 0)
            {
                Debug.WriteLine("PackHirakiErrorStart " + Data + " PackHirakiError " + name);
                return Data.Substring(start, Data.Length - start);
            }
            //   Console.WriteLine("nakattayo " + name);
            return nothing;
        }

        /// <summary>
        /// データの中にパックがある前提ね。文字にアンパックする
        /// </summary>
        /// <param name="name">対象</param>
        /// <param name="nothing">もし中身がなかった時に返す値</param>
        /// <returns></returns>
        public void PackSearch(string name,out int startidx,out int endidx)
        {
            startidx = 0;endidx = 0;
            name = "[" + name + "]";
            int hiraki = 0;
            bool findname = false;
            int start = -1;
            for (int idx = 0; idx < Data.Length; idx++)
            {
                if (hiraki == 0)
                {
                    if (start == -1 && !findname)
                    {
                        if (idx + name.Length < Data.Length)
                        {
                            if (Data.Substring(idx, name.Length) == name)
                            {
                                findname = true;
                            }
                        }
                        startidx = idx;
                    }
                    else
                    {

                    }
                }
                if (Data[idx] == '|')
                {
                    if (findname)
                    {
                        hiraki += 1;
                        findname = false;
                        start = idx + 1;
                    }
                }
                if (Data[idx] == '{')
                {
                    hiraki += 1;
                    if (findname)
                    {
                        findname = false;
                        start = idx + 1;
                    }
                }
                if (Data[idx] == '}')
                {
                    hiraki -= 1;
                    hiraki = Math.Max(0, hiraki);
                    if (start != -1 && hiraki == 0)
                    {
                        var dd = idx - start;
                        if (dd == 0)
                        {
                            endidx = idx;
                            return;
                        }
                        /*if (escape)エスケープをやってた時の名残
                        {
                            return escapen(Data.Substring(start, dd));
                        }
                        else*/
                        {
                            endidx = idx;
                            return;
                        }
                        start = -1;
                    }
                }
            }
            if (hiraki > 0)
            {
                endidx = Data.Length ;
            }
            endidx = Data.Length ;
        }

        /// <summary>
        /// Datasaverにアンパックする
        /// 最初にロードするときだけescape=trueにする。
        /// </summary>
        /// <param name="name">対象</param>
        /// <param name="escape">\nとかを変換するか</param>
        /// <returns></returns>
        public DataSaver unpackDataD(string name)
        {
            var sou = unpackDataS(name);
            //    if (sou == "") return null;

            return new DataSaver(sou);
        }

        public E unpackDataE<E>(string name,E Nan)
            where E : struct, Enum
        {
            var sou = unpackDataS(name);
            E res ;
            if (sou != null && Enum.TryParse(sou, out res)) return res;
            return Nan;
        }
        /// <summary>
        /// floatでアンパックする
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Nan">もし中身がなかった時の返す値</param>
        /// <param name="escape">\nとかを変換するかどうか</param>
        /// <returns></returns>
        public float unpackDataF(string name, float Nan = float.NaN)
        {
            var sou = unpackDataS(name);
            float res = 0;
            if (sou != null &&float.TryParse(sou, out res)) return res;
            return Nan;
        }
        /// <summary>
        /// boolでアンパックする
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="Nan">もし中身がなかった時の返す値</param>
        /// <param name="escape">\nとかを変換するかどうか</param>
        /// <returns></returns>
        public bool unpackDataB(string name, bool Nan = false)
        {
            var sou = unpackDataS(name);
            bool res;
            if (sou != null && bool.TryParse(sou, out res)) return res;
            return Nan;
        }
        /// <summary>
        /// 全てのデータをアンパックする。
        /// 初めてアンパックするときだけescape=trueに
        /// </summary>
        /// <param name="escape">\nとかを変換</param>
        /// <returns></returns>
        public List<string> allUnpackDataS()
        {
            var res = new List<string>();
            foreach (var a in getAllPacks())
            {
                res.Add(unpackDataS(a));
            }
            return res;
        }
        /// <summary>
        /// 全てのデータをアンパックする。
        /// 初めてアンパックするときだけescape=trueに
        /// </summary>
        /// <param name="escape">\nとかを変換</param>
        /// <returns></returns>
        public List<DataSaver> allUnpackDataD()
        {
            var res = new List<DataSaver>();
            foreach (var a in getAllPacks())
            {
                res.Add(unpackDataD(a));
            }
            return res;
        }
        /// <summary>
        /// 全てのパックを取得する
        /// 
        /// </summary>
        /// <returns></returns>
        public List<string> getAllPacks()
        {
            var res = new List<string>();
            int hiraki = 0;
            int start = -1;
            for (int idx = 0; idx < Data.Length; idx++)
            {
                if (hiraki == 0)
                {
                    if (start == -1)
                    {
                        if (Data[idx] == '[')
                        {
                            start = idx + 1;
                        }
                    }
                    else
                    {
                        if (Data[idx] == ']')
                        {
                            var dd = idx - start;
                            if (dd == 0)
                            {
                                res.Add("");
                            }
                            else
                            {
                                res.Add(Data.Substring(start, dd));

                            }
                        }
                    }
                }
                if (Data[idx] == '|')
                {
                    if (hiraki == 0)
                    {
                        hiraki += 1;
                        start = -1;
                    }
                }
                if (Data[idx] == '{')
                {
                    hiraki += 1;
                    start = -1;
                }
                if (Data[idx] == '}')
                {
                    hiraki -= 1;
                    start = -1;
                }
            }
            return res;
        }
        public void reset()
        {
            Data = "";
        }
        /// <summary>
        /// データを特定の文字で区切って変換する
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public List<string> splitDataS(char c = ':')
        {
            var sp = Data.Split(c);
            var res = new List<string>();
            foreach (var a in sp)
            {
                res.Add(a);
            }
            return res;
        }

        /// <summary>
        /// データを特定の文字で区切って変換する
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public List<float> splitDataF(char c = ':')
        {
            var sp = Data.Split(c);
            var res = new List<float>();
            foreach (var a in sp)
            {
                res.Add(float.Parse(a));
            }
            return res;
        }
        /// <summary>
        /// データを特定の文字で区切ったのち、idx番目を変換する
        /// </summary>
        /// <param name="c"></param>
        /// <param name="def">idx番目が存在しなかった時の戻り値</param>
        /// <returns></returns>
        public string splitOneDataS(int idx, string def = "", char c = ':')
        {
            var sp = Data.Split(c);
            if (idx >= sp.Length)
            {
                return def;
            }
            return sp[idx];
        }
        /// <summary>
        /// データを特定の文字で区切ったのち、idx番目を変換する
        /// </summary>
        /// <param name="c"></param>
        /// <param name="def">idx番目が存在しなかった時の戻り値</param>
        /// <returns></returns>
        public float splitOneDataF(int idx, float def = float.NaN, char c = ':')
        {
            var sp = Data.Split(c);
            float res = def;
            if (idx >= sp.Length)
            {
                return res;
            }
            if (!float.TryParse(sp[idx], out res)) { res = def; }

            return res;
        }
        /// <summary>
        /// データを特定の文字で区切ったのち、idx番目を変換する
        /// </summary>
        /// <param name="c"></param>
        /// <param name="def">idx番目が存在しなかった時の戻り値</param>
        /// <returns></returns>
        public bool splitOneDataB(int idx, bool def = false, char c = ':')
        {
            var sp = Data.Split(c);
            bool res = def;
            if (idx >= sp.Length)
            {
                return res;
            }
            if (!bool.TryParse(sp[idx], out res)) { res = def; }

            return res;
        }

        /// <summary>
        /// データのすべての構造を取得する。多分呼び出すのは一番上でなのでescape=trueがいいな。
        /// </summary>
        /// <param name="escape">\nとかを変換するか</param>
        /// <param name="indent">0でいいよ</param>
        /// <returns></returns>
        public string getAllkouzou(int indent = 0)
        {
            var st = "";
            var lis = getAllPacks();
            foreach (var a in lis)
            {
                ;
                var b = unpackDataD(a);
                st += new string('.', indent)+":" + a + "\n";
                st += new string('.', indent) + b.getAllkouzou(indent + 1) ;

            }

            return st;
        }


        /// <summary>
        /// 中身のデータを 簡易記法(  name:value\n   )をパックに変える エスケープはしといた方がいいかも
        /// </summary>
        /// <param name="nameEnd"></param>
        /// <param name="packEnd"></param>
        /// <returns></returns>
        public DataSaver KaniPack(char nameEnd=':', char packEnd=';') 
        {
            var res = new DataSaver(); ;
            var sp = this.getData().Split(packEnd);
            foreach (var a in sp) 
            {
                var d = new DataSaver(a);
                var kugis = d.splitDataS(nameEnd);

                if (kugis.Count >= 2) 
                {
                    var data = "";
                    for (int i = 1; i < kugis.Count; i++) 
                    {
                        if (i > 1) { data += ":"; }
                        data += kugis[i];
                    }
                    res.packAdd(kugis[0], data);

                }
            }

            return res;
        }
        /// <summary>
        /// 中身のデータを 簡易記法(  name: )を階層に変え 中身も再帰的に簡易記法から返還する。
        /// </summary>
        /// <param name="nameEnd"></param>
        /// <param name="packEnd"></param>
        /// <param name="kaisous"></param>
        /// <returns></returns>
        public DataSaver KaniKaisou(char nameEnd, char packEnd,params string[] kaisous)
        {
            var kaisou = new List<string>(kaisous);
            var res = new DataSaver();
            string nowkaisou = "";
            var sp= this.getData().Split(packEnd);
            List<DataSaver> horyudata = new List<DataSaver>();
            List<string> horyuname = new List<string>();
            foreach (var a in sp)
            {
                var kugis = new DataSaver(a).splitDataS(nameEnd);
                bool add = true;
                if (kugis.Count >= 2)
                {
                    var kaisounumber = kaisou.IndexOf(kugis[0]);
                    var packname = "";
                    for (int i = 1; i < kugis.Count; i++)
                    {
                        if (i > 1) { packname += ":"; }
                        packname += kugis[i];
                    }



                    if (kaisounumber >= 0)
                    {
                        for (int i = horyudata.Count - 1; i >= kaisounumber; i--)
                        {
                            if (i == 0)
                            {
                                res.packAdd(kaisou[i] + horyuname[i], horyudata[i]);
                                horyudata.RemoveAt(i);
                                horyuname.RemoveAt(i);
                            }
                            else
                            {
                                horyudata[i - 1].packAdd(kaisou[i] + horyuname[i], horyudata[i]);
                                horyudata.RemoveAt(i);
                                horyuname.RemoveAt(i);
                            }
                        }
                    }

                    if (kaisounumber == horyudata.Count)
                    {
                        horyudata.Add(new DataSaver());
                        horyuname.Add(packname);
                        add = false;
                    }
                    else if(kaisounumber>=0)
                    {
                        Debug.WriteLine(packname+" kaisouError "+res.getData() + " kaisouError " + getData()); 
                        return res;
                    }
                }
                if (add && horyudata.Count > 0)
                {
                    var data = new DataSaver(a + packEnd).KaniPack(nameEnd,packEnd);

                    foreach (var b in data.getAllPacks())
                    {
                        horyudata[horyudata.Count - 1].packAdd(b, data.unpackDataD(b),false);
                    }
                }


            }
            for (int i = horyudata.Count - 1; i >=0; i--)
            {
                if (i == 0)
                {
                    res.packAdd(kaisou[i] + horyuname[i], horyudata[i]);
                    horyudata.RemoveAt(i);
                }
                else
                {
                    horyudata[i - 1].packAdd(kaisou[i] + horyuname[i], horyudata[i]);
                    horyudata.RemoveAt(i);
                }
            }
            return res;
        }

    }

}
