namespace Modules.Database.Generics
{
    using System.Collections.Generic;
    using Modules.Database;
    using Newtonsoft.Json;
    using UnityEngine;
    /// <summary>
    /// 列表型数据集抽象类
    /// </summary>
    /// <typeparam name="T">数据模型类</typeparam>
    /// <typeparam name="E">存储方法类</typeparam>
    public abstract class BaseListCollection<T, E> : IListCollection<T>, IDBSerializer<List<T>> where E : Storage
    {
        protected List<T> cacheSpace = new List<T>();
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

        public BaseListCollection()
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
                if (null == cacheSpace)
                {
                    cacheSpace = new List<T>();
                }
                return;
            }
            cacheSpace = Deserialize(PersistenceManager.Instance.GetPrefs(GetType().Name));
            if (null == cacheSpace)
            {
                cacheSpace = new List<T>();
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
        /// 查询集合对象
        /// </summary>
        /// <param name="i">对象索引</param>
        /// <returns>T</returns>
        public T Get(int i)
        {
            return cacheSpace[i];
        }

        /// <summary>
        /// 批量查询集合对象
        /// </summary>
        /// <returns>T[]</returns>
        public T[] GetAll()
        {
            var results = new T[cacheSpace.Count];
            cacheSpace.CopyTo(results, 0);
            return results;
        }
        /// <summary>
        /// 当前集合对象计数
        /// </summary>
        /// <returns></returns>
        public int Count() {
            return cacheSpace.Count;
        }
        /// <summary>
        /// 范围插入对象
        /// </summary>
        /// <param name="models">对象模型</param>
        public void AddRange(IEnumerable<T> models)
        {
            Sync();
            cacheSpace.AddRange(models);
            updatedSinceLastCache = true;
            Sync();
        }

        /// <summary>
        /// 插入对象到集合
        /// </summary>
        /// <param name="i">插入索引</param>
        /// <param name="model">对象模型</param>
        public void Insert(int i, T model)
        {
            Sync();
            cacheSpace.Insert(i, model);
            updatedSinceLastCache = true;
            Sync();
        }

        /// <summary>
        /// 在指定位置更新对象
        /// </summary>
        /// <param name="i">插入索引</param>
        /// <param name="model">对象模型</param>
        public void Replace(int i, T model)
        {
            Sync();
            cacheSpace[i] = model;
            updatedSinceLastCache = true;
            Sync();
        }

        /// <summary>
        /// 插入对象到集合首位
        /// </summary>
        /// <param name="model">对象模型</param>
        public void Prepend(T model)
        {
            Sync();
            cacheSpace.Insert(0, model);
            updatedSinceLastCache = true;
            Sync();
        }

        /// <summary>
        /// 插入对象到集合末位
        /// </summary>
        /// <param name="model">对象模型</param>
        public void Append(T model)
        {
            Sync();
            cacheSpace.Insert(cacheSpace.Count, model);
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
        /// 按索引删除对象
        /// </summary>
        /// <param name="i">对象索引</param>
        public void RemoveAt(int i)
        {
            Sync();
            cacheSpace.RemoveAt(i);
            updatedSinceLastCache = true;
            Sync();
        }

        /// <summary>
        /// 按模型删除对象
        /// </summary>
        /// <param name="model">对象模型</param>
        public void Remove(T model)
        {
            Sync();
            cacheSpace.Remove(model);
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
        public string Serialize(List<T> raw)
        {
            if (null == raw)
            {
                raw = new List<T>();
            }
            return JsonConvert.SerializeObject(raw);
        }

        /// <summary>
        /// 默认反序列化函数，对于特殊类型可继承覆写自定义反序列化函数
        /// </summary>
        /// <param name="raw">序列化值</param>
        /// <returns>原始对象</returns>
        public List<T> Deserialize(string raw)
        {
            if (null == raw)
            {
                return new List<T>();
            }
            return JsonConvert.DeserializeObject<List<T>>(raw);
        }
    }
}