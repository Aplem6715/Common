using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using ZLogger;

namespace Aplem.Common
{
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class ObjectPool<T> : IPool where T : class, IPoolable, new()
    {
        public int PoolingCount => _pool.Count;
        public int ActiveCount => Capacity - PoolingCount;
        public bool IsPendingDestroy { get; private set; } = false;

        private int _capacity;

        private Stack<T> _pool;

        private int _destroyPerFrame;
        private const int AsyncDestroyPerFrame = 10;

        protected readonly ILogger _logger = LogManager.GetLogger(typeof(ObjectPool<T>).Name, "blue");

        public int Capacity
        {
            get => _capacity;
            private set
            {
                _capacity = value;
                Debug.Assert(ActiveCount >= 0);
            }
        }

        public ObjectPool() : this(0)
        {
        }

        public ObjectPool(int capacity, int destroyPerFrame = AsyncDestroyPerFrame)
        {
            _destroyPerFrame = destroyPerFrame;
            _pool = new Stack<T>(capacity);
            for (var i = 0; i < capacity; i++)
                _pool.Push(new T());
            Capacity = capacity;
        }

        public void Warmup(int capacity)
        {
            var gap = capacity - Capacity;
            if (gap <= 0)
                return;

            for (var i = 0; i < gap; i++)
            {
                _pool.Push(new T());
                Capacity++;
            }
        }

        public T Rent()
        {
            T obj;
            if (_pool.Count == 0)
            {
                obj = new T();
                Capacity++;
            }
            else
            {
                obj = _pool.Pop();
            }

            obj.IsPooling = false;
            obj.SetPool(this);
            obj.OnRent();
            return obj;
        }

        public void Return(IPoolable obj)
        {
            var retObj = obj as T;
            if (retObj is null)
                _logger.ZLogError("returned object is not type of {0}", typeof(T));

            retObj.IsPooling = true && !IsPendingDestroy;
            retObj.OnReturned();
            if (IsPendingDestroy)
                Capacity--;
            else
                _pool.Push(retObj);
        }

        public async UniTask DestroyAsync(CancellationToken token)
        {
            IsPendingDestroy = true;
            while (_pool.Count != 0)
            {
                if (token.IsCancellationRequested)
                    return;

                for (var i = 0; i < _destroyPerFrame; i++)
                {
                    if (!_pool.TryPop(out var item))
                        return;
                    Capacity--;
                }

                await UniTask.DelayFrame(1);
            }

            await ((IPool)this).WaitUntilReturnAll(token);
        }

        public void DestroyAllPooled()
        {
            _pool.Clear();
        }
    }
}
