using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vortice.XAudio2;
using Vortice.Multimedia;
using static Charamaker3.FileMan;
using NAudio.Wave;
using WaveFormatEncoding = Vortice.Multimedia.WaveFormatEncoding;
using WaveFormat = Vortice.Multimedia.WaveFormat;
using System.Runtime.InteropServices;

namespace Charamaker3
{
    public  class SoundEngine
    {


        public readonly string name;
        /// <summary>
        /// audioのあれ
        /// </summary>
        protected IXAudio2 audio;
        /// <summary>
        /// 音を合成したときの奴を表す奴
        /// </summary>
        protected IXAudio2MasteringVoice MV;

        /// <summary>
        /// グローバルボリューム。0＜＝＜＝1
        /// </summary>
        public float volume { get { return _glovol; } 
            set 
            {
                _glovol = value; if (_glovol < 0) _glovol = 0; if (_glovol > 1) _glovol = 1; MV.SetVolume(_glovol);
            } 
        }
        float _glovol = 1f;

        /// 読み込んだ音を保存しとく
        /// </summary>
        public Dictionary<string, otoman> otos = new Dictionary<string, otoman>();
        /// <summary>
        /// 今ならしている音を保存しとく。ちゃんとメモリ開放できるように。
        /// </summary>
        internal List<otoman> oton = new List<otoman>();


        /// <summary>
        /// 同時にならすことのできる音の数
        /// </summary>
        public int maxOto = 50;
        /// <summary>
        /// 同時にならすことのできる同じ音の数
        /// </summary>
        public int maxSameOto = 10;


        public SoundEngine(string name)
        {
            this.name = name;
            audio = Vortice.XAudio2.XAudio2.XAudio2Create();
            MV = audio.CreateMasteringVoice();
        }

        /// <summary>
        /// 音をロードする。ロードするだけ
        /// </summary>
        /// <param name="Tfile">.\oto\*.wavの*部分</param>
        public void loadoto(string Tfile)
        {
            //Debug.WriteLine(Tfile + "  oto play dayo ");
            var file = slashFormat(Tfile);
            var a = Path.GetExtension(file);
            var filem = file;
            bool mp3 = false;
            if (a != ".mp3") filem += ".mp3";

            if (File.Exists(s_rootpath + @"sounds\" + filem) == true)
            {
                file = filem;
                mp3 = true;
            }
            else
            {
                if (a != ".wav") file += ".wav";
            }

            if (File.Exists(s_rootpath+@"sounds\" + file) == false)
            {

                Debug.WriteLine(file + "  no such oto file");
                return;
            }
            var reader = new BinaryReader(File.OpenRead(s_rootpath + @"sounds\" + file));
            int mp3channel = 0;
            int mp3samplerate = 0;
            if (mp3)
            {
                var ms = new MemoryStream();
                //  using ()
                {
                    Stream stream = reader.BaseStream;
                    using (var Sreader = new NAudio.Wave.Mp3FileReader(stream))
                    {
                        mp3samplerate = ((Mp3FileReader)Sreader).WaveFormat.SampleRate;
                        mp3channel = ((Mp3FileReader)Sreader).WaveFormat.Channels;
                        //     Console.WriteLine(Sreader.WaveFormat.Encoding+" asda aw2"+Sreader.Length);
                        if (Sreader.WaveFormat.Encoding == NAudio.Wave.WaveFormatEncoding.IeeeFloat)
                        {
                            NAudio.Wave.WaveFloatTo16Provider w32to16 = new NAudio.Wave.WaveFloatTo16Provider(Sreader);
                            byte[] tmp = new byte[10000];
                            w32to16.Read(tmp, 0, 10000);
                            NAudio.Wave.WaveFileWriter.WriteWavFileToStream(ms, w32to16);
                        }
                        if (Sreader.WaveFormat.Encoding == NAudio.Wave.WaveFormatEncoding.Pcm)
                        {
                            NAudio.Wave.WaveFileWriter.WriteWavFileToStream(ms, Sreader);
                        }
                    }
                }
                /*
                using (FileStream fs = new FileStream(@".\unun.wav", FileMode.Create))
                {

                    //ファイルに書き込む
                    ms.WriteTo(fs);
                }*/
                reader.Dispose();
                reader = new BinaryReader(ms);

                reader.BaseStream.Position = 0;
                // ms.Dispose();
            }
            // Read in the wave file header.
            var chunkId = new string(reader.ReadChars(4));
            var chunkSize = reader.ReadInt32();
            var format = new string(reader.ReadChars(4));
            var subChunkId = new string(reader.ReadChars(4));
            var subChunkSize = reader.ReadInt32();
            var audioFormat = (WaveFormatEncoding)reader.ReadInt16();
            var numChannels = reader.ReadInt16();
            var sampleRate = reader.ReadInt32();
            var bytesPerSecond = reader.ReadInt32();
            var blockAlign = reader.ReadInt16();
            var bitsPerSample = reader.ReadInt16();
            if (mp3)
            {
                var gomi = new string(reader.ReadChars(2));
            }
            var dataChunkId = new string(reader.ReadChars(4));
            var dataSize = reader.ReadInt32();

            // Check that the chunk ID is the RIFF format
            // and the file format is the WAVE format
            // and sub chunk ID is the fmt format
            // and the audio format is PCM
            // and the wave file was recorded in stereo format
            // and at a sample rate of 44.1 KHz
            // and at 16 bit format
            // and there is the data chunk header.
            // Otherwise return false.
            //Console.WriteLine(chunkId + " " + format + " " + subChunkId.Trim() + " " + audioFormat + " = " + WaveFormatEncoding.Pcm + " " + numChannels + " " + sampleRate + " " + bitsPerSample + " " + dataChunkId);
            if ((!mp3 || 1 == 1) && (chunkId != "RIFF" || format != "WAVE" || subChunkId.Trim() != "fmt" || audioFormat != WaveFormatEncoding.Pcm || bitsPerSample != 16 || dataChunkId != "data"))
            {
                Console.WriteLine(chunkId + format + subChunkId.Trim() + (audioFormat != WaveFormatEncoding.Pcm) + bitsPerSample + dataChunkId + " otoloadsippai"
                    + "\n " + dataSize);
                return;
            }


            // Set the buffer description of the secondary sound buffer that the wave file will be loaded onto and the wave format.


            // Create a temporary sound buffer with the specific buffer settings.
            WaveFormat formattt = new WaveFormat(sampleRate, 16, numChannels); ;

            var SecondaryBuffer = audio.CreateSourceVoice(formattt);

            var waveData = reader.ReadBytes(dataSize);

            int size = Marshal.SizeOf(waveData[0]) * waveData.Length;
            IntPtr WDintPtr = Marshal.AllocHGlobal(size);

            Marshal.Copy(waveData, 0, WDintPtr, size);
            var buffer = new AudioBuffer();
            buffer.Flags = BufferFlags.EndOfStream;

            buffer.AudioBytes = dataSize;
            buffer.AudioDataPointer = WDintPtr;



            SecondaryBuffer.SubmitSourceBuffer(buffer);


            // Read in the wave file data into the temporary buffer.


            // Close the reader
            reader.Close();
            reader.Dispose();

            if (otos.ContainsKey(Tfile))
            {
                //otos[Tfile].dispo();
                otos[Tfile] = new otoman(Tfile,this,SecondaryBuffer, formattt, buffer);
            }
            else
            {
                //  Console.WriteLine(file + " otoloadok");

                otos.Add(Tfile, new otoman(Tfile,this,SecondaryBuffer, formattt, buffer));
                //    Console.WriteLine("asfajsfjsal");

                //    Console.WriteLine(file + " otoloadok");
            }

            Console.WriteLine(Tfile + "  otoload by" + name);
        }


        /// <summary>
        /// 効果音を鳴らすやつをロードする
        /// </summary>
        /// <param name="file">.\oto\*.wavの*部分</param>
        /// <param name="vol">この音のボリューム</param>
        virtual public otoman playoto(string file, float vol = 1)
        {
            if (file == c_nothing) return null;
            file = slashFormat(file);
            /*
            var a = Path.GetExtension(file);
            if (a != ".wav") file += ".wav";
            */

            if (!otos.ContainsKey(file))
            {

                loadoto(file);


                // Console.WriteLine(otos.Count() + "otocount");

            }
            if (otos.ContainsKey(file))
            {
                var otoman = otos[file];

                int kannin = maxSameOto;
                
                for (int i = oton.Count-1;i>=0 ; i--) 
                {
                    if (kannin-- == 0) 
                    {
                        //oton[i].dispo();
                        oton.RemoveAt(i);

                        kannin++;
                    }
                }

                var nbuf = audio.CreateSourceVoice(otoman.wvf);


                //nbuf.SubmitSourceBuffer(otoman.buf);
                //nbuf.Start();
                //playへ


                // Lock the secondary buffer to write wave data into it.


                nbuf.SetVolume(vol);
                var res = new otoman(file,this, nbuf, otoman.wvf, otoman.buf);
                oton.Add(res);
                


                if (oton.Count > maxOto)
                {
                    oton[0].Stop();
                    oton.RemoveAt(0);


                }
                return res;
            }
            return null;
        }

        /// <summary>
        /// BGMとか効果音を強制的に全部止める
        /// </summary>
        public void stopAll() 
        {
            foreach (var a in oton) 
            {
                a.Stop();
            }
        }
    }


    /*
    public class BGMEngine : SoundEngine
    {
        public BGMEngine(string name) : base(name) 
        {
            maxOto = 2;
            maxSameOto = 1;
        }
        /// <summary>
        /// bgmを止めるためのbgm
        /// </summary>
        const string stopbgm = "stop";
        string nowbgm { get { if (oton.Count>0) { return oton.Last().name; } return ""; } }
     
   
        /// <summary>
        /// BGMがフェードするフレーム-1でフェードしない
        /// </summary>
        public float bgmfadeframe = 600;

        public bool canFade { get { return bgmfadeframe >= 0; } }

        /// <summary>
        /// BGMの音量を調節する時間。+でフェードアウト
        /// </summary>
        public float bgmfadetimer = 0;
        public override otoman playoto(string file, float vol = 1)
        {
            //Console.WriteLine(file+" bgm played");
            if (file == nothing) return null;
            file = slashFormat(file);
      


            if (file == stopbgm)//+".wav")
            {
                if (!canFade)
                {
                    for (int  i=0;i<oton.Count;i++)
                    {
                        oton[i].sorce.Stop();
                        oton[i].dispo();
                    }
                    oton.Clear();

                }
                else
                {
                    for (int i = oton.Count-2; i >=0; i--)
                    {
                        oton[i].dispo();
                        oton.RemoveAt(i);
                    }
                    if (bgmfadetimer < 0)
                    {
                        bgmfadetimer = bgmfadetimer + bgmfadeframe;
                    }
                    else if (bgmfadetimer == 0)
                    {
                        bgmfadetimer = bgmfadeframe;
                    }
                }
                return;
            }
            file = @"bgm\" + file;

            if (!otos.ContainsKey(file))
            {

                loadoto(file);


                // Console.WriteLine(file + " BGMload"+ otos.ContainsKey(file)+ "  "+(nowbgm != file || butu));
             
            }

            if (otos.ContainsKey(file) && (nowbgm != file))
            {

                // Console.WriteLine(file + " BGMpaly");

                var otoman = otos[file];


                var nbuf = audio.CreateSourceVoice(otoman.wvf);
                otoman.buf.LoopCount = 255;

                nbuf.SubmitSourceBuffer(otoman.buf);



                // Lock the secondary buffer to write wave data into it.


                nbuf.SetVolume(vol);



                otoman.buf.LoopCount = 0;




                if (!canFade)
                {
                    for(int i=0;i<oton.Count;i++)
                    {
                        oton[i].sorce.Stop();
                        oton[i].dispo();
                    }
                    oton.Clear();
                    var bgmman = new otoman(nbuf, otoman.wvf, otoman.buf);
                    bgmman.name = file;
                    nbuf.Start();
                    oton.Add(bgmman);
                }
                else
                {
                    if (oton.Count>0)
                    {
                        if (oton.Count >= 2)
                        {
                            //1つにする。
                            for (int i = oton.Count - 2; i >= 0; i--)
                            {
                                oton[i].sorce.Stop();
                                oton[i].dispo();
                                oton.RemoveAt(i);
                            }
                        }
                        else 
                        {
                            if (bgmfadetimer < 0)
                            {
                                bgmfadetimer = bgmfadetimer + bgmfadeframe;
                            }
                            else if (bgmfadetimer == 0)
                            {
                                bgmfadetimer = bgmfadeframe;
                            }
                        }
                        var bgmman2 = new otoman(nbuf, otoman.wvf, otoman.buf);
                        bgmman2.name = file;
                        oton.Add(bgmman2);
                    }
                    else
                    {
                        bgmfadetimer = -bgmfadeframe;
                        var bgmman = new otoman(nbuf, otoman.wvf, otoman.buf);
                        bgmman.name = file;
                        nbuf.Start();
                        oton.Add(bgmman);
                    }
                }

            }
        }
        override public void Update(float cl)
        {
            base.Update(cl);
            // Console.WriteLine(bgmfadetime + " bgmfadetime");
            if (bgmfadetimer > 0)
            {
                bgmfadetimer -= cl;
                if (bgmfadetimer <= 0)
                {
                    bgmfadetimer = 0;

                    if (oton.Count > 0)
                    {
                        oton[0].sorce.Stop();
                        oton[0].dispo();
                        oton.RemoveAt(0);
                    }
                    if (oton.Count > 0)
                    {
                        oton[0].sorce.Start();
                        bgmfadetimer = -bgmfadeframe;
                    }

                }
                if (oton.Count>0)
                {
                    oton[0].setvolume(bgmfadetimer / bgmfadeframe);
                }
            }
            if (bgmfadetimer < 0)
            {
                bgmfadetimer += cl;
                if (bgmfadetimer >= 0)
                {
                    bgmfadetimer = 0;
                }
                if (oton.Count > 0)
                {
                    oton[0].setvolume(1 + (bgmfadetimer / bgmfadeframe));
                }
            }
        }
    }*/
    /// <summary>
    /// 鳴らしたい音を保存しとくクラス
    /// </summary>
    public class otoman
    {
        /// <summary>
        /// 己が生み出されたSoundEngine
        /// </summary>
        public readonly SoundEngine parent;
        internal IXAudio2SourceVoice source;
        internal WaveFormat wvf;
        internal AudioBuffer buf;

        /// <summary>
        /// ボリューム
        /// </summary>
        protected float _defvolume;

        public float defvolume{ get { return _defvolume; } }

        /// <summary>
        /// 音のパス
        /// </summary>
        public string path;



        /// <summary>
        /// 再生が終わったか
        /// </summary>
        public bool isEnd { get { return source.State.BuffersQueued <= 0; } }

        /// <summary>
        /// 再生時間0~1;
        /// </summary>
        public float playtime
        {
            get
            {
                if (isEnd)
                {
                    return 1;

                }
                return ((float)source.State.SamplesPlayed / (buf.AudioBytes / wvf.BlockAlign));
            }
        }


        internal otoman(string path,SoundEngine parent, IXAudio2SourceVoice s, WaveFormat wf, AudioBuffer ab)
        {
            this.path = path;
            this.parent = parent;
            _defvolume = s.Volume;
            //Console.WriteLine("oukQ");
            source = s;
            wvf = wf;
            buf = ab;


            /*意味なかった
            source.BufferEnd += (aa) => 
            {
                Debug.WriteLine("bufend");
            };
            source.LoopEnd += (aa) =>
            {
                Debug.WriteLine("LOOPend");
            };
            source.ProcessingPassEnd += () =>
            {
                Debug.WriteLine("Passend");
            };
            source.StreamEnd += () =>
            {
                Debug.WriteLine("STREAMend");
            };
            source.BufferStart += (a) => 
            {
                Debug.WriteLine("bufstart");
            };
            source.ProcessingPassStart += (a) =>
            {
                Debug.WriteLine("PassStart");
            };*/
        }
        /// <summary>
        /// 再生を始める
        /// </summary>
        public void Start()
        {
            source.SubmitSourceBuffer(buf);
            source.Start();
        }
        /// <summary>
        /// 再生を止める
        /// </summary>
        public void Stop()
        {
            if (source != null/* && source.IsDisposed == false*/)
            {
                source.ExitLoop(0);
                source.Stop();
                source.FlushSourceBuffers();
            }
        }
        public void SetLoop(int loop)
        {
            buf.LoopCount = loop;
        }

        /// <summary>
        /// 音量を決める
        /// </summary>
        /// <param name="vol">0~1</param>
        public void setVolume(float vol)
        {
            source.SetVolume(defvolume * vol);
        }
        ~otoman()
        {
            dispo();
        }
        private void dispo()
        {
            {
                Stop();
                parent.oton.Remove(this);

            }

            if (source!= null/*&&!source.IsDisposed*/)
            {
                wvf = null;
                source.DestroyVoice();

                buf.Dispose();
                source.Dispose();

            }
        }

    }
    /// <summary>
    /// 音を鳴らすコンポーネント。Worldに追加されることで音が鳴る
    /// </summary>
    public class SoundComponent : Component
    {
        protected otoman _sound;

        protected SoundEngine SE;

        /// <summary>
        /// BGMに着けられる名前
        /// </summary>
        public const string BGMname = "BGM";
        /// <summary>
        /// SoundEffectに着けられる名前
        /// </summary>
        public const string SEname = "SE";

        /// <summary>
        /// 音の素
        /// </summary>
        public otoman sound { get { return _sound; } }

        float fadein = 0;
        float fadeout = 0;
        int _loop = 0;

        /// <summary>
        /// 音量。soundの上から重ね掛けする。
        /// </summary>
        protected float _volume = 1;


        /// <summary>
        /// 音量。soundの上から重ね掛けする。
        /// </summary>
        public float volume{ get { return _volume; }
            set { _volume = value; setVolume(); }
        }
        /// <summary>
        /// 再生が終了したら自動でエンテティから外れる。被ダメ声とか何度も使用したりかぶっちゃダメな場合はtrueにする。
        /// </summary>
        public bool playThenEnd;

        /// <summary>
        /// ループ回数255で無限だったかな
        /// </summary>
        public int loop { get { return _loop; }set { _loop = value; sound?.SetLoop(_loop); } }



        /// <summary>
        /// 音の再生が終わった時に発動するイベント
        /// </summary>
        public event EventHandler<otoman> PlayEnd;
        bool prePlayed = false;



        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="file">音のファイルパス</param>
        /// <param name="volume">音量</param>
        /// <param name="SE">音を鳴らすエンジン</param>
        /// <param name="playThenEnd">再生が終わったら自ら死を選ぶ</param>
        /// <param name="loop">ループする回数0でいいよ</param>
        /// <param name="fadein">音のフェードイン</param>
        /// <param name="fadeout">音のフェードアウト(自ら再生を終える場合は作用しない。
        /// Stopを外部から呼び出したときにのみ発動。つまりBGM専用)</param>
        /// <param name="name">名前</param>
        public SoundComponent(SoundEngine SE,string file, float volume=1, bool playThenEnd=true, int loop=0
            ,float fadein=0,float fadeout = 0,string name=""):base(name:name)
        {
            _sound = SE.playoto(file, volume);
            this.loop = loop;
            this.fadein= fadein;
            this.fadeout = fadeout;
            this.playThenEnd = playThenEnd;
            this.SE = SE;

           // Debug.WriteLine("Sound Component Made!");
        }

        /// <summary>
        /// SEを作る
        /// </summary>
        /// <param name="SE">用いるサウンドエンジン</param>
        /// <param name="file">ファイルパス</param>
        /// <param name="volume">音量0~1</param>
        /// <param name="playThenEnd">再生が終わったら消えるか</param>
        /// <returns></returns>
        static public SoundComponent MakeSE(SoundEngine SE, string file, float volume = 1, bool playThenEnd = true) 
        {
            return new SoundComponent(SE,file,volume,playThenEnd,name:SEname);
        }

        /// <summary>
        /// BGMを作る。名前がBGMになるので探してあげてね。
        /// </summary>
        /// <param name="SE">用いるサウンドエンジン</param>
        /// <param name="file">ファイルパス</param>
        /// <param name="volume">音量0~1</param>
        /// <param name="fadein">フェードインの秒数</param>
        /// <param name="fadeout">フェードアウトの音量</param>
        /// <returns></returns>
        static public SoundComponent MakeBGM(SoundEngine SE, string file, float volume = 1
            , float fadein = 0, float fadeout = 0)
        {
            //Debug.WriteLine("Make Bgm " + file);
            return new SoundComponent(SE, file, volume, false,255,fadein,fadeout, name:BGMname);
        }


        /// <summary>
        /// カラコン
        /// </summary>
        public SoundComponent() { }

        public override void copy(Component c)
        {
            base.copy(c);
            var cc = (SoundComponent)c;
            cc.fadein=this.fadein;
            cc.fadeout=this.fadeout;
            cc.loop= this.loop;
            cc.playThenEnd = this.playThenEnd;
            cc.SE = this.SE;
            cc._sound = this.SE.playoto(this.sound.path,this.sound.defvolume);
            cc.volume = this.volume;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override DataSaver ToSave()
        {
            var d = base.ToSave();
            d.linechange();
            d.packAdd("SoundEngine",SE.name);
            d.packAdd("playThenEnd", playThenEnd);
            d.linechange();
            d.packAdd("path", this.sound.path);
            d.packAdd("soundvol", this.sound.defvolume);
            d.packAdd("compvol", this.volume);
            d.linechange();
            d.packAdd("loop", loop);
            d.packAdd("fadein", this.fadein);
            d.packAdd("fadeout", this.fadeout);

            return d;
        }
        protected override void ToLoad(DataSaver d)
        { 
          
            base.ToLoad(d);
            this.SE = FileMan.GetSoundEngine(d.unpackDataS("SoundEngine"));
            this.playThenEnd = d.unpackDataB("playThenEnd");
            this._sound = this.SE.playoto(d.unpackDataS("path"), d.unpackDataF("soundvol",1));
            this.volume = d.unpackDataF("compvol");
            this.loop = (int)d.unpackDataF("loop");
            this.fadein = d.unpackDataF("fadein");
            this.fadeout = d.unpackDataF("fadeout");

        }

        public override void addtoworld(float cl = 0)
        {
            base.addtoworld(cl);
            Play();
            //Debug.WriteLine(sound.path + " Sound Component Started");
        }
        public override void removetoworld(float cl = 0)
        {
            base.removetoworld(cl);
            Stop(true);
            //Debug.WriteLine(sound.path + " Sound SToped");
        }
        protected override void onupdate(float cl)
        {
            base.onupdate(cl);

            setVolume();
            if (sound != null)
            {
                if ((prePlayed && sound.isEnd && playThenEnd)
                    || (fadeOutTimer >= 0 && timer >= fadeOutTimer + fadeout))
                {

                    PlayEnd?.Invoke(this, sound);
                    remove();
                    //Debug.WriteLine(timer + "  " + fadeOutTimer + fadeout);
                }

                prePlayed = !sound.isEnd;
            }
        }

        public void Play()
        {
            sound?.Start();
            prePlayed = true;
            resettimer();
        }
        public override void resettimer()
        {
            base.resettimer();
            fadeOutTimer = -1;
        }

        private float fadeOutTimer=-1;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="stopkyousei">強制的に止める</param>
        public void Stop(bool stopkyousei=false)
        {
            if (stopkyousei||fadeout <= 0 || (fadeOutTimer>=0&&timer >= fadeOutTimer + fadeout))
            {
                sound?.Stop();
            }
            else
            {
                fadeOutTimer = timer;
            }
        }
        /// <summary>
        /// 音量を設定する。YOKUNAI:updateでも、update外の変更でも、重複して呼び出されるのでよくない実装カモ
        /// </summary>
        protected void setVolume() 
        {
            float vol;
            if (fadeout > 0 && fadeOutTimer >= 0)
            {
               vol=(Mathf.max(Mathf.min(1 - (timer - fadeOutTimer) / fadeout, 1), 0));
                //  Debug.WriteLine((timer - fadeOutTimer)+" :: "+fadeout);
            }
            else if (fadein > 0)
            {
                vol = (Mathf.min(timer / fadein, 1));
            }
            else
            {
                vol = (1);
            }

            sound?.setVolume(vol*volume);
        }

    }
}
