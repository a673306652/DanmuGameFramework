namespace Modules.Database.Generics
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Modules.Database;
    using Newtonsoft.Json;
    using UnityEngine;

    /// <summary>
    /// 单例型数据集抽象类
    /// </summary>
    /// <typeparam name="T">数据模型类</typeparam>
    /// <typeparam name="E">存储方法类</typeparam>
    public class BaseSingletonCollection<T, E> : ISingletonCollection<T>, IDBSerializer<T> where E : Storage
    {
        protected T cacheSpace;
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

        public BaseSingletonCollection()
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
            if (typeof(E).Equals(typeof(Storage.FILE)))
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
            if (typeof(E).Equals(typeof(Storage.FILE)))
            {
                PersistenceManager.Instance.SetFile(GetType().Name, Serialize(cacheSpace));
                return;
            }
            PersistenceManager.Instance.SetPrefs(GetType().Name, Serialize(cacheSpace));
        }

        /// <summary>
        /// 查询集合对象
        /// </summary>
        public T Get()
        {
            return cacheSpace;
        }

        public T GetIfContains(T returnOnDefault = default(T))
        {
            if (null != cacheSpace)
                return cacheSpace;
            return returnOnDefault;
        }

        /// <summary>
        /// 更新对象到集合
        /// </summary>
        public void Set(T model)
        {
            cacheSpace = model;
            updatedSinceLastCache = true;
            Sync();
        }
        /// <summary>
        /// 更新并保存对象到集合
        /// </summary>
        /// <param name="model"></param>
        public void SetAndSave(T model)
        {
            Set(model);
            Save();
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        public void Remove()
        {
            Sync();
            cacheSpace = default(T);
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
        public string Serialize(T raw)
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
        public T Deserialize(string raw)
        {
            if (null == raw)
            {
                return default(T);
            }
            return JsonConvert.DeserializeObject<T>(raw);
        }
    }
}