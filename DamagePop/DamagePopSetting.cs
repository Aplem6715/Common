using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;
using Aplem.Common;
using Unity.Mathematics;


namespace Aplem.Project
{
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    [CreateAssetMenu(menuName = "Project/Setting/DamagePopSetting")]
    public class DamagePopSetting : ScriptableObject
    {
        [SerializeField] [MinMaxSlider(0, 3, true)]
        private Vector2 _scaleRange;

        [SerializeField] private int _scaleLimitDamage;

        [SerializeField] private float _animTime = 1.0f;

        [SerializeField] private float _popAngleDistDeg = 15.0f;

        public float AnimTime => _animTime;
        public float PopAngleDistDeg => _popAngleDistDeg;

#if DEBUG
        private void Awake()
        {
            Debug.Assert(_scaleLimitDamage != 0);
        }
#endif

        public float GetScale(int damage)
        {
            return math.remap(0, _scaleLimitDamage, _scaleRange.x, _scaleRange.y, damage);
        }
    }
}
