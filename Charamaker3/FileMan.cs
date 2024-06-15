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


namespace Charamaker3
{
    static public class FileMan
    {
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
                int rgba = color.R | (color.G << 8) | (color.B << 16) | (color.A << 24);
                tempStream.Write(rgba);
                var bitmapProperties = new BitmapProperties(new PixelFormat(Vortice.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));
                registBmp(bitname + "bit", baseRender.CreateBitmap(new System.Drawing.Size(1, 1), tempStream.BasePointer, sizeof(int), bitmapProperties));

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
                string path = @".\tex\" + file;
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

                            var size = new System.Drawing.Size(bitmap.Width, bitmap.Height);
                            var bitmapProperties = new BitmapProperties(new PixelFormat(Vortice.DXGI.Format.R8G8B8A8_UNorm, AlphaMode.Premultiplied));

                            var result = baseRender.CreateBitmap(size, tempStream.BasePointer, stride, bitmapProperties);
                            registBmp(file, result);

                        }
                    }
                }
            }
            // Console.WriteLine(texs.Count() + "texcount");
            return texs[file];
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

        /// <summary>
        /// モーションをロードする
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public MotionSaver loadMotion(string path) 
        {
            var res=MotionSaver.ToLoad(DataSaver.loadFromPath(path,false,ext:".ctm"));
            return res;
        }
        /// <summary>
        /// モーションをセーブする
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static public void saveMotion(string path,string script,Motion motion)
        {
            new MotionSaver(script, motion).ToSave().saveToPath(path,ext:".ctm");
            
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
        static public SoundEngine SoundEffect;
        /// <summary>
        /// BGMを再生するときに使うやつ
        /// </summary>
        static public SoundEngine BGM;

        /// <summary>
        /// Set Up Sound Engine
        /// </summary>
        static public void SoundSetUP()
        {
            //Debug.WriteLine(" EEEEEEEEEEEEEEEEEEEEEEE");
            SoundEffect = new SoundEngine("SoundEffect");
            BGM = new SoundEngine("BGM");

        }
        /// <summary>
        /// UpDate Sound Engine まあBGMのフェードとか
        /// </summary>
        /// <param name="cl"></param>
        static public void SoundUpdate(float cl) 
        {
            SoundEffect.Update(cl);
            BGM.Update(cl);
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
        public void indent(int num = 1)
        {
            var s = Data.Split('\n');
            string res = "";
            for (int i = 0; i < s.Length; i++)
            {
                res += new string(' ', num) + s[i] + "\n";
            }
            Data = res;
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
                    n += res.Data.Substring(end, res.Data.Length);
                }
                res = new DataSaver(n);
            }
            return res;
            /*
            var res = new DataSaver();
            var lis = this.getAllPacks();
            foreach (var a in lis) 
            {
                if (!packs.Contains(a)) 
                {
                    res.packAdd(a,this.unpackDataS(a));
                }
            }
            return res;*/
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
                st += new string(' ', indent) + a + "\n";
                st += new string(' ', indent) + b.getAllkouzou(indent + 1) + "\n";

            }

            return st;
        }

    }

}
