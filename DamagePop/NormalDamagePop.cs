using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;
using Aplem.Common;
using Unity.Mathematics;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;

namespace Aplem.Project
{
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class NormalDamagePop : DamagePop
    {
        [SerializeField] [InlineEditor] private DamagePopSetting _setting;

        private float _timer;
        private Vector2 _dir;
        private Vector2 _startPos;

        public override async UniTask Open(Vector3 pos, int damage)
        {
            var scale = _setting.GetScale(damage);
            transform.localScale = Vector3.one * scale;

            await base.Open(pos, damage);
        }

        protected override async UniTask Play()
        {
            _canvas.alpha = 1;
            _startPos = transform.position;
            _timer = _setting.AnimTime;

            var deg = 90.0f + RandomUtil.GlobalRand.NextFloat(-_setting.PopAngleDistDeg, _setting.PopAngleDistDeg);
            var angle = deg / 360 * math.PI * 2.0f;
            _dir = new Vector3(math.cos(angle), math.sin(angle));

            await UniTask.WaitUntil(() => _timer <= 0);
        }

        private void FixedUpdate()
        {
            var remain = _timer / _setting.AnimTime;
            _canvas.alpha = remain;
            transform.position = _startPos + _dir * (1 - remain);
            _timer -= Time.fixedDeltaTime;
        }
    }
}
