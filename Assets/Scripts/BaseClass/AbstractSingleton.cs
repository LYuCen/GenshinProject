using UnityEngine;

namespace BaseClass
{
    public abstract class AbstractSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new object();
        private static bool _isShuttingDown = false;

        private void OnApplicationQuit()
        {
            _isShuttingDown = true;
        }

        private void OnDestroy()
        {
            _isShuttingDown = true;
        }

        public static T Instance
        {
            get
            {
                if (_isShuttingDown) return null;
                if (!_instance)
                {
                    lock (_lock)
                    {
                        if (!_instance)
                        {
                            _instance = FindObjectOfType<T>();

                            if (!_instance)
                            {
                                GameObject singletonObj = new GameObject(typeof(T).Name);
                                _instance = singletonObj.AddComponent<T>();
                                DontDestroyOnLoad(singletonObj);
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            T singleton = FindObjectOfType<T>();
            if (singleton != this)
            {
                Destroy(gameObject);
            }
        }
    }
}