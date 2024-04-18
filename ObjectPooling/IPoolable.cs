using UnityEngine;

namespace Aplem.Common
{
    public interface IPoolable
    {
        bool IsPooling { get; set; }
        IPool _pool { get; set; }
        void OnRent();
        void OnReturned();

        void SetPool(IPool pool)
        {
            _pool = pool;
        }
    }

    public static class IPoolableExtension
    {
        // ReSharper disable Unity.PerformanceAnalysis
        public static bool Return(this IPoolable poolable)
        {
            if (poolable._pool == null)
            {
                Debug.LogError("Pool is null");
                return false;
            }

            poolable._pool.Return(poolable);
            return true;
        }
    }

    public interface IPoolableMono : IPoolable
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}
