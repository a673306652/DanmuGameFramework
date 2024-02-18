using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daan
{

    /// <summary>
    /// 工厂模式的对象接口
    /// </summary>
    public interface IFactory
    {
        IFactory CreateInstance();
    }
    
    /// <summary>
    /// 实例工厂
    /// </summary>
    public class MyFactory<K, V> where V : IFactory
    {
        private Dictionary<K, V> dict = new Dictionary<K, V>();

        public MyFactory()
        {

        }
        public V this[K k]
        {
            get
            {
                return this.Create(k);
            }
            set
            {
                this.Register(k, value);
            }
        }
        public void Register(K id, V x)
        {
            if (dict.ContainsKey(id))
            {
                return;
            }
            dict.Add(id, x);
        }

        public V Create(K id)
        {
            if (!dict.ContainsKey(id))
            {
                return default(V);
            }
            return (V)dict[id].CreateInstance();
        }

        public bool Contains(K key)
        {
            return dict.ContainsKey(key);
        }
    }
}
