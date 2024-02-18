using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Daan
{
    /// <summary>
    /// 单例模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T> : IManager where T : IManager, new()
    {

        private static T instance;
        
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new T();
                    instance.Init();
                }
                return instance;
            }
        }

        public void Destroy()
        {
            instance = default(T);
        }
        public virtual void OnLevelLoaded(Scene scene, LoadSceneMode mode) { }
        public virtual void Init() { }
        public virtual void Reset() { }
        public virtual void Tick() { }
    }
}