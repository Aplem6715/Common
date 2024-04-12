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

        // ReSharper disable Unity.PerformanceAnalysis
        bool Return()
        {
            if (_pool == null)
            {
                Debug.LogError("Pool is null");
                return false;
            }

            _pool.Return(this);
            return true;
        }
    }

    public interface IPoolableMono : IPoolable
    {
        GameObject gameObject { get; }
        Transform transform { get; }
    }
}
