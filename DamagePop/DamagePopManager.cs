using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;
using Aplem.Common;


namespace Aplem.Common
{

    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class DamagePopManager : SingletonMono<DamagePopManager>
    {
        private MonoPool<DamagePop>[] _pools;

        public void SetPrefs(DamagePop[] prefs)
        {
            _pools = new MonoPool<DamagePop>[prefs.Length];
            for (int i = 0; i < prefs.Length; i++)
            {
                var pref = prefs[i];
                var parentObj = new GameObject($"{pref.name}");
                parentObj.transform.SetParent(transform);

                _pools[i] = new MonoPool<DamagePop>(pref.gameObject, parentObj.transform, 32);
            }
        }

        public void Spawn(Vector3 pos, int damage, int styleId)
        {
            if(styleId < 0 || styleId >= _pools.Length)
            {
                _logger.ZLogError("未登録のダメージ表示スタイル id:{0}", styleId);
            }
            
            var pop = _pools[styleId].Rent();
            pop.Setup(pos, damage);
        }
    }
}
