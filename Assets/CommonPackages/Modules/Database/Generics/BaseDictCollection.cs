namespace Modules.Database.Generics
{
    using System.Collections.Generic;
    using Modules.Database;
    using Newtonsoft.Json;
    using UnityEngine;
    /// <summary>
    /// 字典型数据集抽象类
    /// </summary>
    /// <typeparam name="T">数据模型类</typeparam>
    /// <typeparam name="E">存储方法类</typeparam>
    public abstract class BaseDictCollection<T, E> : IDictCollection<T>, IDBSerializer<Dictionary<string, T>> where E : Storage
    {
        protected Dictionary<string, T> cacheSpace = new Dictionary<string, T>();
        protected float lastCacheTime = -1f; // 初始默认值置为 -1，防止初始时并发请求 (否则可能与 Time.time 初始值 0f 冲突)
        protected bool updatedSinceLastCache;
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

        public BaseDictCollection()
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
        /// 默认序列化函数，对于特殊类型可继承覆写自定义序列化函数
        /// </summary>
        /// <param name="raw">原始对象</param>
        /// <returns>序列化值</returns>
        public virtual string Serialize(Dictionary<string, T> raw)
        {
            if (null == raw)
            {
                raw = new Dictionary<string, T>();
            }
            return JsonConvert.SerializeObject(raw);
        }
        /// <summary>
        /// 默认反序列化函数，对于特殊类型可继承覆写自定义反序列化函数
        /// </summary>
        /// <param name="raw">序列化值</param>
        /// <returns>原始对象</returns>
        public virtual Dictionary<string, T> Deserialize(string raw)
        {
            if (null == raw)
            {
                return new Dictionary<string, T>();
            }
            return JsonConvert.DeserializeObject<Dictionary<string, T>>(raw);
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
                if (null == cacheSpace)
                {
                    cacheSpace = new Dictionary<string, T>();
                }
                return;
            }
            cacheSpace = Deserialize(PersistenceManager.Instance.GetPrefs(GetType().Name));
            if (null == cacheSpace)
            {
                cacheSpace = new Dictionary<string, T>();
            }
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
        /// 是否存在 Key 值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Contains(string key)
        {
            return cacheSpace.ContainsKey(key);
        }

        public int Count
        {
            get
            {
                return cacheSpace.Count;
            }
        }

        /// <summary>
        /// 查询集合对象
        /// </summary>
        /// <param name="key">对象主键</param>
        /// <returns>T</returns>
        public T Get(string key)
        {
            return cacheSpace [key];
        }

        /// <summary>
        /// 若存在 Key 值则返回查询对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="returnOnDefault"></param>
        /// <returns></returns>
        public T GetIfContains(string key, T returnOnDefault = default(T))
        {
            if (Contains(key))
            {
                return Get(key);
            }
            return returnOnDefault;
        }

        /// <summary>
        /// 批量查询集合对象
        /// </summary>
        /// <returns>T[]</returns>
        public T [] GetAll()
        {
            var results = new T [cacheSpace.Count];
            cacheSpace.Values.CopyTo(results, 0);
            return results;
        }

        public string [] GetAllKeys()
        {
            var results = new string [cacheSpace.Count];
            cacheSpace.Keys.CopyTo(results, 0);
            return results;
        }

        /// <summary>
        /// 写入对象到集合
        /// </summary>
        /// <param name="key">对象主键</param>
        /// <param name="model">对象模型</param>
        public void Set(string key, T model)
        {
            Sync();
            cacheSpace [key] = model;
            updatedSinceLastCache = true;
            Sync();
        }

        /// <summary>
        /// 写入对象到集合并强制保存
        /// </summary>
        /// <param name="key">对象主键</param>
        /// <param name="model">对象模型</param>
        public void SetAndSave(string key, T model)
        {
            Set(key, model);
            Save();
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="key">对象主键</param>
        public void Remove(string key)
        {
            Sync();
            cacheSpace.Remove(key);
            updatedSinceLastCache = true;
            Sync();
        }

        /// <summary>
        /// 清空集合
        /// </summary>
        public void ClearAll()
        {
            Sync();
            cacheSpace.Clear();
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
    }
}