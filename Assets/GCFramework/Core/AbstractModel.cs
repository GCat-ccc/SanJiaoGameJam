

namespace GCFramework.Core
{
    /// <summary>
    /// 数据模型接口
    /// </summary>
    public interface IModel : IBelongToArchitecture, ICanSetArchitecture, IUtility
    { 
        void Init();
    }
    
    
    /// <summary>
    /// 抽象数据模型，用于子类继承
    /// </summary>
    public abstract class AbstractModel: IModel
    {
        private IArchitecture _architecture;
        
        void IModel.Init()
        {
            OnInit();
        }

        public IArchitecture GetArchitecture()
        {
            return _architecture;
        }

        public void SetArchitecture(IArchitecture architecture)
        {
            _architecture = architecture;  
        }

        protected abstract void OnInit();
    }
}