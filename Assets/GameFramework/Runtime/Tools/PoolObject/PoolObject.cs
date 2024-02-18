using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Daan
{
    public class PoolObject : MonoBehaviour
    {
        [HideInInspector]
        public string poolObjectPath; //池子对象的资源路径 也是标识

        /// <summary>
        /// 物体初次创建时执行的操作
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// 每次生成都会调用
        /// </summary>
        public virtual void OnSpawn()
        {

        }
        /// <summary>
        /// 每次回收都会调用
        /// </summary>
        public virtual void OnDespawn()
        {

        }
    }
}
