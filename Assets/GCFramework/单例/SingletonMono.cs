using UnityEngine;

namespace GCFramework.单例
{
    public class SingletonMono<T> : MonoBehaviour where T : SingletonMono<T>
    {
        private static T _instance;

        public static T Ins
        {
            get
            {
                if (_instance) return _instance;

                _instance = FindObjectOfType<T>();

                if (!_instance)
                {
                    _instance = new GameObject("Singleton Of " + typeof(T)).AddComponent<T>();
                    _instance.InitAwake();
                }
                else _instance.InitAwake();

                return _instance;
            }
        }
        
        [Header("是否切换场景不进行销毁")]
        public bool isDontDestroyOnLoad = true;

        private void Awake()
        {
            if (_instance)
            {
                Destroy(gameObject);
                return;
            }

            if (isDontDestroyOnLoad)
                DontDestroyOnLoad(gameObject);

            _instance = (T)this;
            
            InitAwake();
        }

        protected virtual void InitAwake()
        {
            
        }

        public static bool IsInitialized => _instance != null;

        protected virtual void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }
    }
}