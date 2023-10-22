

using System.Collections.Generic;
using ZLogger;

namespace Aplem.Common
{
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    public class ObjectPool<T> : IPool where T : class, IPoolable, new()
    {
        public int PoolingCount => _pool.Count;
        public int UsingCount => Capacity - PoolingCount;

        public int Capacity { get; private set; }

        private Stack<T> _pool;

        protected readonly ILogger _logger = LogManager.GetLogger(typeof(ObjectPool<T>).Name);

        public ObjectPool() : this(0) { }
        public ObjectPool(int capacity)
        {
            Capacity = capacity;
            _pool = new Stack<T>(capacity);

            for (int i = 0; i < capacity; i++)
            {
                _pool.Push(new T());
            }
        }

        public void Warmup(int capacity)
        {
            int gap = capacity - Capacity;
            if (gap <= 0)
            {
                return;
            }

            for (int i = 0; i < gap; i++)
            {
                _pool.Push(new T());
            }
            Capacity = capacity;
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
            T retObj = obj as T;
            if (retObj is null)
            {
                _logger.ZLogError("returned object is not type of {0}", typeof(T));
            }

            retObj.IsPooling = true;
            retObj.OnReturned();
            _pool.Push(retObj);
        }

        public void Clear()
        {
            _pool.Clear();
        }
    }
}