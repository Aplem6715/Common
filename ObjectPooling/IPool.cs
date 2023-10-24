

using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Aplem.Common
{
    public interface IPool
    {
        public int PoolingCount { get; }
        public int ActiveCount { get; }
        public int Capacity { get; }
        public bool IsPendingDestroy { get; }

        public void Return(IPoolable obj);

        /// <summary>
        /// 非同期で順次解放.<br/>
        /// プーリングされているものはフレームごとにn個ずつDestroy.<br/>
        /// Returnされてきたものは順次Destroy.
        /// </summary>
        public UniTask DestroyAsync(CancellationToken token);

        public void DestroyAllPooled();

        // すべての要素が返されるまで待機
        public async UniTask WaitUntilReturnAll(CancellationToken token)
        {
            await UniTask.WaitUntil(() => ActiveCount == 0, cancellationToken: token);
        }
    }
}