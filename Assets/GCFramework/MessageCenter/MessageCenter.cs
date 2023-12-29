using System;
using System.Collections.Generic;
using UnityEngine;

namespace GCFramework.MessageCenter
{
    public interface IUnRegister
    {
        void UnRegister();
    }

    /// <summary>
    /// 自定义可注销的类
    /// </summary>
    public struct CustomUnRegister : IUnRegister
    {
        /// <summary>
        /// 委托对象，用于调用取消订阅函数
        /// </summary>
        private Action OnUnRegister { get; set; }

        /// <summary>
        /// 带参构造
        /// </summary>
        /// <param name="onUnRegister"></param>
        public CustomUnRegister(Action onUnRegister)
        {
            OnUnRegister = onUnRegister;
        }

        /// <summary>
        /// 资源释放
        /// </summary>
        public void UnRegister()
        {
            OnUnRegister.Invoke();
            OnUnRegister = null;
        }
    }

    public struct Event
    {
        public Action<object> OnEvent;
    }

    public class MessageCenter : MonoBehaviour
    {
        private static Dictionary<Type, Event> _eventDictionary = new();

        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="onEvent"></param>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public static IUnRegister Subscribe<TEvent>(Action<TEvent> onEvent) where TEvent : new()
        {
            var type = typeof(TEvent);
            
            void CallBack(object o) => onEvent((TEvent)o);
            
            if (!_eventDictionary.ContainsKey(type))
            {
                _eventDictionary.Add(type, new Event());
            }

            var eventTmp = _eventDictionary[type];
            eventTmp.OnEvent += CallBack;
            _eventDictionary[type] = eventTmp;
            return new CustomUnRegister(() => { UnSubscribe(onEvent); });
        }

        /// <summary>
        /// 注销
        /// </summary>
        /// <param name="onEvent"></param>
        /// <typeparam name="TEvent"></typeparam>
        public static void UnSubscribe<TEvent>(Action<TEvent> onEvent)
        {
            var type = typeof(TEvent);

            void CallBack(object o) => onEvent((TEvent)o);
            
            if(!_eventDictionary.ContainsKey(type))
                return;
            var eventTmp = _eventDictionary[type];
            eventTmp.OnEvent -= CallBack;
            _eventDictionary[type] = eventTmp;
        }
        
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="obj"></param>
        /// <typeparam name="TEvent"></typeparam>
        public static void Fire<TEvent>(TEvent obj = default) where TEvent : new()
        {
            var type = typeof(TEvent);
            if(!_eventDictionary.TryGetValue(type, out var actions))
                return;
            
            actions.OnEvent?.Invoke(obj);
        }

        /// <summary>
        /// 清空所有事件
        /// </summary>
        public static void ClearAllEvent() => _eventDictionary = null;

    }
}
