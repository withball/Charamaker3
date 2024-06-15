using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charamaker3
{

    /// <summary>
    /// シーンに渡すスタティックめな情報。
    /// </summary>
    public class SceneContaner
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
        public SceneContaner(Display display,Inputs.NameInput input) 
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
        /// SceneManager
        /// </summary>
        public SceneContaner sc;
        /// <summary>
        /// 自動で作られる一番安易なカメラ
        /// </summary>
        public readonly Camera cam;
        /// <summary>
        /// 次のシーン
        /// </summary>
        public Scene next;
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="s">シーンマネージャ</param>
        /// <param name="next">次のシーン</param>
        public Scene(SceneContaner s, Scene next = null)
        {
            this.next = next;
            sc = s;
            cam = sc.display.makeCamera(new ColorC(0, 0.8f, 0.9f, 1));
        }
        private bool _started = false;
        /// <summary>
        /// start(),end()が複数回発動しないようにするフラグ。startでtrueになる。
        /// 直接いじってもいいけどstart,endで変えてくれるんだけど
        /// </summary>
        protected bool started { get { return _started; } set { _started = value; } }
        /// <summary>
        /// シーンを開始したいときに発動してね。
        /// smにこれが代入されてnextがない時は何かしらを代入しておくといい
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
        /// シーンの正しきスタート時に呼び出される。標準ではアップデートが呼び出される
        /// </summary>
        /// <param name="cl">ついでに呼びだされるアップデートの時間</param>
        virtual protected void onStart(float cl)
        {
            onStarts?.Invoke(this, cl);
            Update(cl);
        }

        /// <summary>
        /// 画面の描画。
        /// </summary>
        /// <param name="cl">クロック時間</param>
        /// <param name="OnDraw">画面の描画を行うか</param>
        virtual public void Update(float cl=1,bool OnDraw=true)
        {
            if(OnDraw)sc.display.draw(cl);
            onUpdate(cl);
        }
        /// <summary>
        /// フレーム動作。
        /// </summary>
        /// <param name="cl"></param>
        virtual protected void onUpdate(float cl)
        {
            onUpdates?.Invoke(this, cl);
        }
        /// <summary>
        /// 標準はnextをスタートしてstartedをfalseにするだけ
        /// </summary>
        virtual public void End(float cl=0)
        {
            if (started)
            {
                onEnd(cl);
                next.Start(cl);
                _started = false;
            }
        }

        /// <summary>
        /// シーンの正しきエンド時に呼び出される。標準では何もしない
        /// </summary>
        virtual protected void onEnd(float cl)
        {

            onEnds?.Invoke(this, cl);
        }
    }
}
