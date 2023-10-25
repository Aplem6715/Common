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
        [SerializeField]
        private Vector3 _spawnShift = new Vector3(0, 0.2f, 0);

        [SerializeField]
        private int _poolSize = 32;

        [SerializeField]
        private DamagePop[] _prefs;

        private MonoPool<DamagePop>[] _pools;

        protected override void Awake()
        {
            base.Awake();
            SetPrefs(_prefs);
        }

        private void SetPrefs(DamagePop[] prefs)
        {
            _pools = new MonoPool<DamagePop>[prefs.Length];
            for (int i = 0; i < prefs.Length; i++)
            {
                var pref = prefs[i];
                if (pref == null)
                {
                    continue;
                }
                var parentObj = new GameObject($"{pref.name}");
                parentObj.transform.SetParent(transform);

                _pools[i] = new MonoPool<DamagePop>(pref.gameObject, parentObj.transform, _poolSize);
            }
        }

        public void Spawn(Vector3 pos, int damage, int styleId)
        {
            Debug.Assert(_pools != null, "DamagePopupのPrefabが設定されていません。\nダメージ表示を行う前にDamagePopManager.SetPrefsを実行してください");
            if (styleId < 0 || styleId >= _pools.Length)
            {
                _logger.ZLogError("未登録のダメージ表示スタイル id:{0}", styleId);
            }

            var pop = _pools[styleId].Rent();
            pop.Setup(pos + _spawnShift, damage);
        }
    }
}
