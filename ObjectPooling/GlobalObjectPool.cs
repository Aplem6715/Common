namespace Aplem.Common
{
    public class GlobalPool<T> where T : class, IPoolable, new()
    {
        private static readonly ObjectPool<T> _instance = new();

        public static ObjectPool<T> Inst => _instance;
    }
}
