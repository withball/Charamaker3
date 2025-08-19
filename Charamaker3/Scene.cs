using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charamaker3
{

    /// <summary>
    /// シーンに渡すスタティックめな情報<br></br>
    /// 便利メソッドもあるよ。
    /// </summary>
    public class SceneContainer
    {
        /// <summary>
        /// インプットデータ
        /// </summary>
        public Inputs.NameInput input;
        /// <summary>
        /// 今のシーンを表すデータ。書き換えてシーン移行する
        /// </summary>
        public Scene nowScene=null;
        /// <summary>
        /// 画面を表すデータ。
        /// </summary>
        public Display display;
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        public SceneContainer(Display display,Inputs.NameInput input) 
        {
            this.display = display;
            this.input = input;
        }
    }
    /// <summary>
    /// シーン
    /// </summary>
    public class Scene
    {

        /// <summary>
        /// onstartの際に呼び出される
        /// </summary>
        public event EventHandler<float> onStarts;
        /// <summary>
        /// onendの際に呼び出される
        /// </summary>
        public event EventHandler<float> onEnds;
        /// <summary>
        /// frameの際に呼び出される
        /// </summary>
        public event EventHandler<float> onUpdates;


        /// <summary>
        /// frameの際に呼び出される
        /// </summary>
        public event EventHandler<float> onAfterDrawUpdate;

        /// <summary>
        /// SceneManager
        /// </summary>
        public SceneContainer sc;
        /// <summary>
        /// 自動で作られる一番安易なカメラ
        /// </summary>
        public readonly Camera cam;
        /// <summary>
        /// 次のシーン
        /// </summary>
        public Scene next;

        /// <summary>
        /// ワールド
        /// </summary>
        public World wol;
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="s">シーンマネージャ</param>
        /// <param name="next">次のシーン</param>
        public Scene(SceneContainer s, Scene next = null)
        {
            this.next = next;
            sc = s;

            cam = sc.display.makeCamera(new ColorC(0, 0.8f, 0.9f, 1));

            WorldReset();
        }

        /// <summary>
        /// ワールドを全消しする。カメラもなくなるのでWacthrect.addもしなさい
        /// </summary>
        public void WorldReset() 
        {
            wol = new World();
        }

        private bool _started = false;
        /// <summary>
        /// start(),end()が複数回発動しないようにする。start=falseであるとUpdateすら発動しない。startでtrueになる。
        /// 直接いじってもいいけどstart,endで変えてくれるんだけど
        /// </summary>
        protected bool started { get { return _started; } set { _started = value; } }
        /// <summary>
        /// シーンを開始したいときに発動してね。
        /// scにこれが代入されてnextがない時は何かしらを代入しておくといい
        /// </summary>
        virtual public void Start(float cl=0)
        {
            sc.nowScene = this;
            if (!started)
            {
                onStart(cl);
                //if (next == null) next = this;
                _started = true;
            }
        }
        /// <summary>
        /// シーンの正しきスタート時に呼び出される。標準ではカメラのwatchrectが追加される
        /// </summary>
        /// <param name="cl">ついでに呼びだされるアップデートの時間</param>
        virtual protected void onStart(float cl)
        {

            cam.watchRect.add(wol);
            onStarts?.Invoke(this, cl);
            Update(cl,false);
        }

        /// <summary>
        /// 画面の描画。
        /// </summary>
        /// <param name="cl">クロック時間</param>
        /// <param name="OnDraw">画面の描画を行うか(しない場合、AfterDrawUpdateを呼び出すこと)</param>
        virtual public void Update(float cl=1,bool OnDraw=true)
        {
            if (started)
            {
                onUpdate(cl);

                if (OnDraw)
                {
                    sc.display.draw(cl);
                    onAfterDrawUpadete();

                }
            }
        }
        /// <summary>
        /// フレーム動作。標準はUpdatesのInvocとwolのupdate
        /// </summary>
        /// <param name="cl"></param>
        virtual protected void onUpdate(float cl)
        {
            onUpdates?.Invoke(this, cl);
            wol.update(cl);
        }

        /// <summary>
        /// updateでついでに描画しない場合、呼び出す！！！
        /// </summary>
        virtual public void onAfterDrawUpadete()
        {
            onAfterDrawUpdate?.Invoke(this, 0);
            wol.AfterDrawUpdate();
        }
        /// <summary>
        /// 標準はnextをスタートしてstartedをfalseにするだけ。
        /// </summary>
        virtual public void End(float cl=0)
        {
            if (started)
            {
                onEnd(cl);
                next?.Start(cl);
                _started = false;
            }
            else 
            {
                next?.Start(cl);
            }
        }

        /// <summary>
        /// シーンの正しきエンド時に呼び出される。標準ではカメラのWatchrectリムーブ。<br></br>
        /// ここに次のシーンを何にするか描くといい。
        /// </summary>
        virtual protected void onEnd(float cl)
        {
            onEnds?.Invoke(this, cl);
            cam.watchRect.remove();
        }


        /// <summary>
        /// マウスカーソルのシーン標準のカメラでの座標を取得する
        /// </summary>
        /// <param name="gamennnai">画面内に座標を抑えるか</param>
        /// <returns></returns>
        public FXY GetCursourPoint(bool gamennnai = true)
        {
            return sc.input.GetCursourPoint(cam, gamennnai);
        }
        /// <summary>
        /// マウスカーソルの座標。ちょっと重いけど便利。
        /// </summary>
        public FXY cursour 
        {
            get { return GetCursourPoint(); }
            set //無理やりカーソルを動かせる。意味あるのかな？
                {
                var dp=value-cursour;
                sc.input.input.x += dp.x;
                sc.input.input.y += dp.y;
            }
        }

    }
}
