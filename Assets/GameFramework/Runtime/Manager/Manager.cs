using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Daan
{
    public abstract class Manager<T> : MonoBehaviour, IManager where T : Manager<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<T>(true);
                    if (instance == null)
                    {
                        instance = new GameObject("Manager").MustComponent<T>();
                    }
                    instance.Init();
                }
                return instance;
            }
        }

        public virtual void Init()
        {
            if (instance == null)
            {
                instance = this as T;
                GameManager.instance.RegisterManager(this);
            }
        }

        public virtual void OnLevelLoaded(Scene scene, LoadSceneMode mode)
        {
        }

        public virtual void Reset()
        {
        }

        public virtual void Tick()
        {
        }
    }

}