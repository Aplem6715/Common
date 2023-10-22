

using UnityEngine;

namespace Aplem.Common
{
    public interface IPool
    {
        void Return(IPoolable obj);
    }
}