

namespace Aplem.Common
{
    public class GlobalPool<T> where T : class, IPoolable, new()
    {
        private static readonly ObjectPool<T> _instance = new ObjectPool<T>();

        public static ObjectPool<T> Inst
        {
            get
            {
                return _instance;
            }
        }
    }
}