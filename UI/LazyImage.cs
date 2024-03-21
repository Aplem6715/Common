using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using ZLogger;
using Aplem.Common;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Threading;

namespace Aplem.Common.UI
{
    public class LazyImage : Image
    {
        [SerializeField] private IView _loadingView;

        public async UniTask SetSpriteAsync(string spriteAddr, CancellationToken token)
        {
            this.enabled = false;
            _loadingView.Show();
            {
                var loadedSprite = await Addressables.LoadAssetAsync<Sprite>(spriteAddr).ToUniTask(cancellationToken: token);
                if (!token.IsCancellationRequested)
                {
                    this.sprite = loadedSprite;
                }
            }
            _loadingView.Hide();
            this.enabled = true;
        }
    }
}