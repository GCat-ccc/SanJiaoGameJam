
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using GCFramework.Extension;
using GCFramework.单例;
using JetBrains.Annotations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GCFramework.Resource
{
    public enum AssetType
    {
        Model,
        Audio,
        UIPrefab,
        Sprite,
        Prefab
    }

    /// <summary>
    /// 资源路径可调整
    /// </summary>
    public static class Path
    {
        public static readonly string ModelRootPath = "Models/";
        public static readonly string AudioRootPath = "Audios/";
        public static readonly string UIPrefabRootPath = "UIPrefabs/";
        public static readonly string SpriteRootPath = "Sprites/";
        public static readonly string PrefabRootPath = "Prefabs/";

        public static string GetResPath(string resName, AssetType assetType)
        {
            switch (assetType)
            {
                case AssetType.Model:
                    return ModelRootPath + resName;
                case AssetType.Audio:
                    return AudioRootPath + resName;
                case AssetType.UIPrefab:
                    return UIPrefabRootPath + resName;
                case AssetType.Sprite:
                    return SpriteRootPath + resName;
                case AssetType.Prefab:
                    return PrefabRootPath + resName;
                default:
#if UNITY_EDITOR
                    Debug.LogWarning($"该类型不存在！{assetType}");    
#endif
                    break;
            }

            return "";
        }
    }

    public class AssetInfo
    {
        public string AssetName;
        public AssetType AssetType;
        public object AssetObject;
    }
    
    /// <summary>
    /// 简易Resources加载框架，仅Demo项目使用
    /// </summary>
    public class GCResourceManager : SingletonNoMono<GCResourceManager>
    {
        private readonly List<AssetInfo> _assetCachePool = new();
    
        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="assetType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T LoadRes<T>(string resName, AssetType assetType) where T : Object
        {
            AssetInfo assetInfo = _assetCachePool.Find(x => x.AssetName == resName);
            if (assetInfo != null)
            {
                return (T)assetInfo.AssetObject;
            }

            string path = Path.GetResPath(resName, assetType);
            T res = Resources.Load<T>(path);
            if (res == default(T))
            {
#if UNITY_EDITOR
                Debug.LogWarning($"加载的资源不存在，resName: {resName}");
#endif
                return default(T);
            }

            AssetInfo newAssetInfo = new AssetInfo
            {
                AssetName = resName,
                AssetType = assetType,
                AssetObject = res
            };
            _assetCachePool.TryAdd(newAssetInfo);
            return res;
        }

        // ReSharper disable Unity.PerformanceAnalysis
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="assetType"></param>
        /// <param name="completed"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerator LoadResAsync<T>(string resName, AssetType assetType, Action<AssetInfo> completed) where T : Object
        {
            AssetInfo assetInfo = _assetCachePool.Find(x => x.AssetName == resName);
            if (assetInfo != null)
            {
                completed.Invoke(assetInfo);
                yield break;
            }

            string path = Path.GetResPath(resName, assetType);
            ResourceRequest resourceRequest = Resources.LoadAsync<T>(path);
            if (resourceRequest.asset == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"加载的资源不存在！resName: {resName}");
#endif
                completed.Invoke(null);
                yield break;
            }
            // 等待资源加载完成
            yield return resourceRequest.isDone;

            AssetInfo newAssetInfo = new AssetInfo
            {
                AssetName = resName,
                AssetType = assetType,
                AssetObject = resourceRequest.asset
            };
            
            _assetCachePool.TryAdd(newAssetInfo);
            completed.Invoke(newAssetInfo);
            yield return null;
        }

        /// <summary>
        /// 同步加载多个资源
        /// </summary>
        /// <param name="resFileName"></param>
        /// <param name="assetType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T[] LoadAllRes<T>(string resFileName, AssetType assetType) where T : Object
        {
            string path = Path.GetResPath(resFileName, assetType);
            T[] allRes = Resources.LoadAll<T>(path);
            if (allRes.Length == 0)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"加载的资源不存在！resFileName: {resFileName}");
#endif
                return default;
            }

            foreach (var res in allRes)
            {
                AssetInfo newAssetInfo = new AssetInfo
                {
                    AssetName = res.name,
                    AssetType = assetType,
                    AssetObject = res
                };
                _assetCachePool.TryAdd(newAssetInfo);
            }

            return allRes;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        /// <param name="resName"></param>
        public void ReleaseRes(string resName)
        {
            var assetInfo = _assetCachePool.Find(x => x.AssetName == resName);
            if (assetInfo != null)
                _assetCachePool.Remove(assetInfo);
        }
        
        /// <summary>
        /// 释放缓存
        /// </summary>
        public void ClearAllCacheRes()
        {
            _assetCachePool.Clear();
        }
        
        /// <summary>
        /// 获取缓存资源数量
        /// </summary>
        /// <returns></returns>
        public int GetCacheResCount() => _assetCachePool.Count;
    }

}