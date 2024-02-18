using System;
using System.Collections;
using System.Collections.Generic;
using Modules.Patterns;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Modules.ResourcePool
{
    public partial class ResourcePool : Singleton<ResourcePool>
    {
        private const string JSON_DIR_ASYNC = "Assets/ResourcesForAddressable/Json/";
        private const string BASE_DIR_ASYNC = "Assets/ResourcesForAddressable/";

        private Dictionary<object, UnityEngine.Object>
            asyncDict = new Dictionary<object, UnityEngine.Object>();

        /// <summary>
        /// Load async resource with object pooling.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void LoadAsync<T>(
            object path,
            UnityAction<T, bool> onComplete = null
        )
            where T : class
        {
            if (asyncDict.ContainsKey(path))
            {
                onComplete?.Invoke(asyncDict[path] as T, true);
                return;
            }
            LoadAsyncOnce<UnityEngine.Object>(path,
            (t, ok) =>
            {
                if (ok)
                {
                    asyncDict[path] = t;
                    onComplete?.Invoke(t as T, ok);
                    return;
                }
                onComplete?.Invoke(default(T), false);
            });
        }

        /// <summary>
        /// Load async resource once in lifetime.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="onComplete"></param>
        /// <typeparam name="T"></typeparam>
        public void LoadAsyncOnce<T>(
            object path,
            UnityAction<T, bool> onComplete = null
        )
        {
            Addressables.LoadAssetAsync<T>(path).Completed += (
                AsyncOperationHandle<T> handle
            ) =>
            {
                if (handle.IsValid())
                {
                    onComplete?.Invoke(handle.Result, true);
                }
                else
                {
                    onComplete?.Invoke(default(T), false);
                }
                Addressables.Release(handle);
            };
        }

        public void GetItemPrefabAsync(
            string prefabNameStr,
            UnityAction<GameObject, bool> onComplete = null)
        {
            if (prefabDict.ContainsKey(prefabNameStr))
            {
                onComplete?.Invoke(prefabDict[prefabNameStr], true);
                return;
            }

            LoadAsync<GameObject>(prefabNameStr, (prefab, valid) =>
            {
                if (prefab != null)
                {
                    prefabDict[prefabNameStr] = prefab;
                    onComplete?.Invoke(prefabDict[prefabNameStr], valid);
                }
                else
                {
                    Debug.LogError("Prefab " + prefabNameStr + " does not exist");
                }
            });
            return;
        }

        public void GetJsonStringAsync(
            string name,
            UnityAction<string, bool> onComplete = null,
            bool useCache = false
        )
        {
            var resNameStr = JSON_DIR_ASYNC + name;
            if (useCache)
            {
                LoadAsync<TextAsset>(resNameStr,
                (ta, valid) =>
                {
                    onComplete?.Invoke(ta?.text, valid);
                });
                return;
            }
            LoadAsyncOnce<TextAsset>(resNameStr,
            (ta, valid) =>
            {
                onComplete?.Invoke(ta?.text, valid);
            });
        }

        public void GetGameItemAsync(string prefabName, UnityAction<GameObject, bool> onComplete = null)
        {
            GetItemPrefabAsync(BASE_DIR_ASYNC + GAME_PREFAB_DIR + prefabName, onComplete);
        }

        public void GetABResCatalogStatus(string name, UnityAction<string, bool> onComplete = null)
        {
            var resNameStr = "Assets/ResourcesForAddressable/" + name;
            LoadAsyncOnce<TextAsset>(resNameStr,
            (ta, valid) =>
            {
                onComplete?.Invoke(ta?.text, valid);
            });
        }
    }
}
