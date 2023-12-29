using System;
using System.Collections.Generic;
using UnityEngine;

namespace GCFramework.Extension
{
    public static class ExtensionUtility
    {
        /// <summary>
        /// 必须添加的组件
        /// </summary>
        /// <param name="go"></param>
        /// <param name="component"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool RequireComponent<T>(this GameObject go, ref T component) where T : Component
        {
            if (!go)
            {
#if ENABLE_UNET
                Debug.LogWarning($"不能给空对象添加组件, gameObject为{go}");
                return false;
#endif
            }

            if (component == null && !go.TryGetComponent(out component))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"不能给对象添加空组件, component为{component}");
                return false;
#endif
            }

            return true;
        }

        public static bool RequireComponent<T>(this Component behaviour, ref T component) where T : Component
        {
            return RequireComponent<T>(behaviour.gameObject, ref component);
        }

        /// <summary>
        /// List的扩展方法，尝试添加数据
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool TryAdd<T>(this List<T> list, T value)
        {
            if (list.Contains(value))
                return false;
            list.Add(value);
            return true;
        }

        
    }
}