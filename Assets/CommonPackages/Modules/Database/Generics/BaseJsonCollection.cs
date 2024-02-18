namespace Modules.Database.Generics
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Modules.Converters;
    using Modules.Database;
    using Newtonsoft.Json;
    using UnityEngine;

    /// <summary>
    /// JSON字典型数据集抽象类，此类型无法存储自定义模型类
    /// 支持范围详见 @TypeConverter
    /// </summary>
    /// <typeparam name="T">数据模型类</typeparam>
    /// <typeparam name="E">存储方法类</typeparam>
    public class BaseJsonCollection<T> : IJsonCollection, IDBSerializer<Dictionary<string, object>> where T : Storage
    {
        protected Dictionary<string, object> cacheSpace;
        protected float lastCacheTime = -1f; // 初始默认值置为 -1，防止初始时并发请求 (否则可能与 Time.time 初始值 0f 冲突)
        protected bool updatedSinceLastCache = false;
        protected virtual float CacheInterval
        {
            get { return 10f; }
        }
        protected virtual bool ForceSaved
        {
            get
            {
                return false;
            }
        }

        public BaseJsonCollection()
        {
            Sync();
        }

        /// <summary>
        /// 同步函数
        /// </summary>
        protected void Sync()
        {
            if (lastCacheTime < 0f)
            {
                LoadPersistent();
                return;
            }

            if (ForceSaved || (Time.time - lastCacheTime > CacheInterval) && updatedSinceLastCache)
            {
                SavePersistent();
                return;
            }
        }

        /// <summary>
        /// 刷新缓存
        /// </summary>
        protected void LoadPersistent()
        {
            lastCacheTime = Time.time;
            updatedSinceLastCache = false;
            if (typeof(T).Equals(typeof(Storage.FILE)))
            {
                cacheSpace = Deserialize(PersistenceManager.Instance.GetFile(GetType().Name));
                return;
            }
            cacheSpace = Deserialize(PersistenceManager.Instance.GetPrefs(GetType().Name));
        }

        /// <summary>
        /// 持久化缓存
        /// </summary>
        protected void SavePersistent()
        {
            lastCacheTime = Time.time;
            updatedSinceLastCache = false;
            if (typeof(T).Equals(typeof(Storage.FILE)))
            {
                PersistenceManager.Instance.SetFile(GetType().Name, Serialize(cacheSpace));
                return;
            }
            PersistenceManager.Instance.SetPrefs(GetType().Name, Serialize(cacheSpace));
        }

        /// <summary>
        /// 查询集合键值
        /// </summary>
        public E GetValue<E>(string key, E returnOnDefault = default(E))
        {
            if (!cacheSpace.ContainsKey(key))
            {
                return returnOnDefault;
            }
            return TypeConverter.DOConvert<E>(cacheSpace[key]);
        }

        /// <summary>
        /// 以Json解析序列化对象，泛型只支持原生类型及相关数组、列表组合
        /// </summary>
        public E GetValueCustomFromStr<E>(string key, E returnOnDefault = default(E))
        {
            if (!cacheSpace.ContainsKey(key))
            {
                return returnOnDefault;
            }
            return JsonConvert.DeserializeObject<E>(cacheSpace[key] as string);
        }

        /// <summary>
        /// 更新集合键值
        /// </summary>
        public void SetValue<E>(string key, E value)
        {
            Sync();
            cacheSpace[key] = value;
            updatedSinceLastCache = true;
            Sync();
        }


        /// <summary>
        /// 以Json序列化对象，泛型只支持原生类型及相关数组、列表组合
        /// </summary>
        public void SetValueCustomAsStr<E>(string key, E value)
        {
            Sync();
            cacheSpace[key] = JsonConvert.SerializeObject(value);
            updatedSinceLastCache = true;
            Sync();
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        public void Remove(string key)
        {
            Sync();
            cacheSpace.Remove(key);
            updatedSinceLastCache = true;
            Sync();
        }

        /// <summary>
        /// 强制持久化
        /// </summary>
        public void Save()
        {
            SavePersistent();
        }

        /// <summary>
        /// 默认序列化函数，对于特殊类型可继承覆写自定义序列化函数
        /// </summary>
        /// <param name="raw">原始对象</param>
        /// <returns>序列化值</returns>
        public string Serialize(Dictionary<string, object> raw)
        {
            if (null == raw)
            {
                return null;
            }
            return JsonConvert.SerializeObject(raw);
        }

        /// <summary>
        /// 默认反序列化函数，对于特殊类型可继承覆写自定义反序列化函数
        /// </summary>
        /// <param name="raw">序列化值</param>
        /// <returns>原始对象</returns>
        public Dictionary<string, object> Deserialize(string raw)
        {
            if (null == raw)
            {
                return new Dictionary<string, object>();
            }
            return JsonConvert.DeserializeObject<Dictionary<string, object>>(raw);
        }
    }
}