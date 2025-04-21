using UnityEngine;

namespace Spectral.Core
{
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance != null) return _instance;
                _instance = (T)FindFirstObjectByType(typeof(T));
                if (_instance != null) return _instance;
                _instance = new GameObject(nameof(T), typeof(T)).GetComponent<T>();
                return _instance;
            }
        }
    }
}