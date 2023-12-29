namespace GCFramework.Core
{
    /// <summary>
    /// 系统框架接口
    /// </summary>
    public interface IArchitecture
    {
        void RegisterModel<T>(T model) where T : IModel;

        void RegisterSystem<T>(T system) where T : ISystem;

        void RegisterUtility<T>(T utility) where T : IUtility;

        T GetModel<T>() where T : class, IModel;

        T GetSystem<T>() where T : class, ISystem;

        T GetUtility<T>() where T : class, IUtility;
    }

    #region 接口规则

    /// <summary>
    /// 属于架构
    /// </summary>
    public interface IBelongToArchitecture
    {
        IArchitecture GetArchitecture();
    }

    public interface ICanSetArchitecture
    {
        void SetArchitecture(IArchitecture architecture);
    }

    public interface ICanGetModel : IBelongToArchitecture
    {
        
    }
    
    // 扩展
    public static class CanGetModelExtension
    {
        /// <summary>
        /// 通过在注册时注入架构，可以实现在Model类中直接this.GetModel()调用方法
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetModel<T>(this ICanGetModel self) where T : class, IModel
        {
            return self.GetArchitecture().GetModel<T>();
        }
    }

    public interface ICanGetSystem : IBelongToArchitecture
    {
        
    }

    public static class CanGetSystemExtension
    {
        public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem
        {
            return self.GetArchitecture().GetSystem<T>();
        }
    }

    public interface ICanGetUtility : IBelongToArchitecture
    {
        
    }

    public static class CanGetUtilityExtension
    {
        public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility
        {
            return self.GetArchitecture().GetUtility<T>();
        }
    }

    #endregion
    
    
}