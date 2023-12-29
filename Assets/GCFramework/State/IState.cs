namespace GCFramework.State
{
    public interface IEnter
    {
        void OnEnter();
    }
    
    public interface IState: IEnter
    {
        void OnAwake();
    }
}