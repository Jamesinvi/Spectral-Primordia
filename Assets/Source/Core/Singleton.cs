namespace Spectral.Core
{
    public abstract class Singleton<T> where T : new()
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = new T();
                return _instance;
            }
        }
    }
}