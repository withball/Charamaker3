using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charamaker3.Utils
{
    /// <summary>
    /// 時間が来たら勝手に消える性質をエンテティに追加する。
    /// </summary>
    public class LifeTimer:Component
    {
        /// <summary>
        /// 普通のコンストラクタ
        /// </summary>
        /// <param name="time"></param>
        /// <param name="name"></param>
        public LifeTimer(float time,string name=""):base(-Mathf.abs(time),name)
        {
        
        }
        /// <summary>
        /// 空のコンストラクタ
        /// </summary>
        public LifeTimer() { }

        public override void copy(Component c)
        {
            base.copy(c);
        }
        protected override void ToLoad(DataSaver d)
        {
            base.ToLoad(d);
        }
        public override DataSaver ToSave()
        {
            return base.ToSave();
        }
        protected override void onupdate(float cl)
        {
            base.onupdate(cl);
            if (-timer < time) 
            {
                e.remove();
            }
        }

    }
}
