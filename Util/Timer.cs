using System;
using UnityEngine;

namespace Aplem.Common
{
    public enum TimerType
    {
        // ループなし
        Once,
        // 正確にループ（時間切れの際，カウントオーバーした時間分を次のループに持ち越す）
        // 経過時間 = ループ数 x タイマー時間　となる
        // タイマー時間のn倍のdtが発生した場合，nフレーム間継続して呼び出しが発生する
        LoopAccurate,
        // 適当なループ（時間切れの際，カウントオーバーした時間分を次のループに持ち越さない）
        // タイマー時間の数倍のdtが発生した場合も1回だけ呼び出される
        LoopInaccurate, 
    }

    /// <summary>
    /// シンプルなタイマー<br/>
    /// ※dtが大きくなった際，1フレームで最大1回の呼び出しになってしまう点に注意<br/>
    /// 　この場合，次のフレームでも呼び出される。
    /// </summary>
    public class Timer
    {
        public bool IsActive { get; private set; }

        private float _timer;
        private float _timeMax;
        private TimerType _type;

        public Timer(float time, TimerType type, bool activeOnInit=true)
        {
            _timeMax = time;
            _type = type;
            IsActive = activeOnInit;
        }

        public void Start()
        {
            IsActive = true;
        }

        public void Pause()
        {
            IsActive = false;
        }

        public void Reset()
        {
            _timer = _timeMax;
        }

        public bool Update(float dt)
        {
            if(!IsActive)
                return false;

            _timer -= dt;
            if(_timer <= 0)
            {
                switch (_type)
                {
                    case TimerType.Once:
                        IsActive = false;
                        break;
                    case TimerType.LoopAccurate:
                        _timer += _timeMax;
                        break;
                    case TimerType.LoopInaccurate:
                        _timer = _timeMax;
                        break;
                }
                return true;
            }

            return false;
        }
    }
}