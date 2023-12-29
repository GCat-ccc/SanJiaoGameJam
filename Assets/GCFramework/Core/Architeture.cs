using GCFramework.IOC;
using GCFramework.单例;

namespace GCFramework.Core
{
    public abstract class Architecture<T> : SingletonMono<T>, IArchitecture where T : Architecture<T>, new()
    {
        /// <summary>
        /// ioc容器用于存放需要经常被使用的类对象
        /// </summary>
        private readonly IOCContainer _iocContainer = new();

        protected override void InitAwake()
        {
            base.InitAwake();
            
            Init();
        }

        protected abstract void Init();
        
        /// <summary>
        /// 注册数据模型
        /// </summary>
        /// <param name="model"></param>
        /// <typeparam name="TModel"></typeparam>
        public void RegisterModel<TModel>(TModel model) where TModel : IModel
        {
            model.SetArchitecture(this);
            _iocContainer.Register<TModel>(model);
            model.Init();
        }

        public void RegisterSystem<TSystem>(TSystem system) where TSystem : ISystem
        {
            system.SetArchitecture(this);
            _iocContainer.Register<ISystem>(system);
            system.Init();
        }

        public void RegisterUtility<TUtility>(TUtility utility) where TUtility : IUtility
        {
            _iocContainer.Register<IUtility>(utility);
        }

        /// <summary>
        /// 获取模型
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        public TModel GetModel<TModel>() where TModel : class, IModel
        {
            return _iocContainer.Get<TModel>();
        }

        public TSystem GetSystem<TSystem>() where TSystem : class, ISystem
        {
            return _iocContainer.Get<TSystem>();
        }

        public TUtility GetUtility<TUtility>() where TUtility : class, IUtility
        {
            return _iocContainer.Get<TUtility>();
        }
    }
}