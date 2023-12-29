namespace GCFramework.单例
{
    public class SingletonNoMono<T> where T : new()
    {
        private static T _instance;

        public static T Ins
        {
            get
            {
                if (_instance == null)
                    _instance = new T();
                return _instance;
            }
        }
    }
}