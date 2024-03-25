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
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Aplem.Common.UI
{
    public class LazyImage : Image
    {
        [SerializeField] private IView _loadingView;
        private CancellationTokenSource _prevTokenSource;

        public async UniTask SetSpriteAsync(string spriteAddr)
        {
            if(_prevTokenSource != null)
            {
                _prevTokenSource.Cancel();
            }
            _prevTokenSource = new CancellationTokenSource();

            this.enabled = false;
            _loadingView.Show();
            {
                var handle = Addressables.LoadAssetAsync<Sprite>(spriteAddr);
                await handle.ToUniTask(cancellationToken: _prevTokenSource.Token);
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    this.sprite = handle.Result;
                }
            }
            _loadingView.Hide();
            this.enabled = true;
        }
    }
}