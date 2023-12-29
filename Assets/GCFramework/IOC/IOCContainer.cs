using System;
using System.Collections.Generic;

namespace GCFramework.IOC
{
    public class IOCContainer
    {
        /// <summary>
        /// 存放需要经常被各种类获取的 类对象
        /// </summary>
        private readonly Dictionary<Type, object> _instances = new();

        /// <summary>
        /// 存入容器中，供其他类获取
        /// </summary>
        /// <param name="instance"></param>
        /// <typeparam name="T"></typeparam>
        public void Register<T>(T instance)
        {
            var key = typeof(T);

            if (!_instances.ContainsKey(key))
            {
                _instances.Add(key, instance);
                return;
            }
            _instances[key] = instance;
        }

        /// <summary>
        /// 获取容器中的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Get<T>() where T : class
        {
            var key = typeof(T);

            if (_instances.TryGetValue(key, out var refInstance))
                return refInstance as T;
            return null;
        }
    }
}