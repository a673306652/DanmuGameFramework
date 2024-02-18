using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using Microsoft.CSharp;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
namespace Daan
{
    public class ResourceManager : Singleton<ResourceManager>
    {
        //不同平台下StreamingAssets的路径设置  
        public static readonly string PathURL =
#if UNITY_ANDROID
    "jar:file://" + Application.dataPath + "!/assets/";
#elif UNITY_IPHONE
    Application.dataPath + "/Raw/";
#elif UNITY_STANDALONE_WIN || UNITY_EDITOR
    "file://" + Application.dataPath + "/StreamingAssets/";
#else
    string.Empty;
#endif

        public static readonly string ConfigLocalPath = "/AssetBundle/Config/";

        class AssetBundleInfo
        {
            public AssetBundle ab;
            /// <summary>
            /// 依赖数量，当依赖数量为0时，直接卸载
            /// </summary>
            public int dependenceCount;

            public AssetBundleInfo(AssetBundle ab)
            {
                this.ab = ab;
                this.dependenceCount = 1;
            }
        }
        /// <summary>
        /// 初始化完成回调，通常用于游戏初始化
        /// </summary>
        public delegate void OnInitComplete();
        // 回调方法
        public delegate void OnLoadComplete(string abkey, object obj);

        public delegate void PopPool(PoolObject obj);

        public delegate void GetManifest(AssetBundleManifest manifest);

        public delegate void OnAssetBundleDownLoadOver();

        /// <summary>
        /// 临时对象池父节点
        /// </summary>
        public Transform temp3DPool = null;
        public Transform temp2DPool = null;
        public Canvas canvas;


        /// <summary>
        /// 非编辑器的对象池
        /// </summary>
        public Dictionary<string, List<GameObject>> poolObjcDic = new Dictionary<string, List<GameObject>>();

        public override void Init()
        {
            this.CreateTempPool();
        }


        public override void OnLevelLoaded(Scene scene, LoadSceneMode mode)
        {
            base.OnLevelLoaded(scene, mode);
            //Debug.Log("切换场景：清楚对象池引用");
            this.poolObjcDic.Clear();
            this.CreateTempPool();
        }

        void CreateTempPool()
        {
            this.temp3DPool = GameObject.Find("3DPool")?.transform;
            if (!this.temp3DPool)
            {
                this.temp3DPool = new GameObject("3DPool").transform;
            }

            this.canvas = GameObject.FindGameObjectWithTag("MainCanvas")?.GetComponent<Canvas>();
            if (this.canvas)
            {
                this.temp2DPool = this.canvas.transform.Find("2DPool");
                if (!this.temp2DPool)
                {
                    this.temp2DPool = new GameObject("2DPool").MustComponent<RectTransform>();
                    this.temp2DPool.SetParent(this.canvas.transform, false);
                }
            }
        }

        public T PlayEffect<T>(string resName, Vector3 point) where T : Component
        {
            var effect = ResourceManager.Instance.Spawn<T>(resName);
            effect.transform.position = point;
            return effect;
        }

        #region Pop/Push    直接从对象池中调用，没有回调，所以逻辑上，一定要在Load资源后才能Pop,所有通过Pop取到的资源，一定会被添加PoolObj

        public T SpawnUI<T>(string resName, bool must = false) where T : Component
        {
            var obj = this.Spawn<T>(resName, this.temp2DPool, must);
            return obj;
        }

        /// <summary>
        /// 获取内存池资源
        /// </summary>
        /// <param name="resName"></param>
        /// <returns></returns>

        public T Spawn<T>(string resName, Transform parent = null, bool must = false) where T : Component
        {
            GameObject obj = this.Spawn(resName, parent);

            if (obj)
            {
                if (must)
                {
                    return obj.MustComponent<T>();
                }
                else
                {
                    return obj.GetComponent<T>();
                }
            }
            return null;
        }
        /// <summary>
        /// 为了偷懒，该方法只能在c#层调用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resName"></param>
        /// <returns></returns>
        public GameObject Spawn(string resName, Transform parent = null)
        {
            GameObject obj = null;
            //先从对象池中获取
            if (this.poolObjcDic.ContainsKey(resName))
            {
                List<GameObject> list = this.poolObjcDic[resName];
                if (list.Count > 0)
                {
                    obj = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                    obj.gameObject.SetActive(true);
                }
                else
                {
                    obj = this.Instantiate(resName);
                }
            }
            else
            {
                obj = this.Instantiate(resName);
            }
            obj?.OnSpawn();
            obj.name = obj.name.Replace("(Clone)", "");
            obj.transform.SetParent(parent, false);
            return obj;
        }


        /// <summary>
        /// 之后加一个异步的管理
        /// </summary>
        /// <param name="resName"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public Coroutine Spawn(string resName, Action<GameObject> action)
        {
            return GameManager.Instance.StartCoroutine(this.AsyncLoad(resName, action));
        }


        IEnumerator AsyncLoad(string resName, Action<GameObject> action)
        {
            var res = Resources.LoadAsync(resName);
            yield return res;
            if (res.asset) action(GameObject.Instantiate(res.asset) as GameObject);
            else Debug.LogError("找不到该资源" + resName);
        }


        private GameObject Instantiate(string resName)
        {
            var temp = Resources.Load<GameObject>(resName);
            if (temp == null)
            {
                Debug.LogError("找不到该资源" + resName);
                return null;
            }
            var go = GameObject.Instantiate(temp);
            if (go == null)
            {
                Debug.LogError("找不到该资源" + resName);
            }
            else
            {
                go.Init(resName);
            }
            return go;
        }

        public void Despawn<T>(List<T> component) where T : PoolObject
        {
            for (int i = 0; i < component.Count; i++)
            {
                this.Despawn(component[i]);
            }
            component.Clear();
        }

        public void Despawn(Component component, string path = "")
        {
            var c = component ? component.gameObject : null;
            this.Despawn(c, path);
        }
        public void Despawn(PoolObject obj, Transform pool = null)
        {
            if (obj == null)
            {
                return;
            }
            List<GameObject> list;

            obj.transform.SetParent(pool ?? this.temp3DPool, false);
            obj.gameObject.SetActive(false);
            string path = obj.poolObjectPath ?? "";
            //无路径对象回收
            //if (string.IsNullOrEmpty(path))
            //{
            //    //obj.poolObjectPath = obj.name.Replace("(Clone)", "");
            //    //obj.poolObjectPath = obj.name.Replace(" ", "");
            //}

            if (this.poolObjcDic.ContainsKey(path))
            {
                list = this.poolObjcDic[path];
                list.Add(obj.gameObject);
            }
            else
            {
                list = new List<GameObject>() { obj.gameObject };
                this.poolObjcDic.Add(path, list);
            }

            obj.OnDespawn();

        }

        /// <summary>
        /// 回收一个物体
        /// </summary>
        /// <param name="obj"></param>
        public void Despawn(GameObject obj, string path = "")
        {
            if (obj == null)
            {
                Debug.LogError("尝试回收一个空物体" + path);
                return;
            }
            PoolObject pobj = obj.GetComponent<PoolObject>();
            if (pobj == null && path == "")
            {
                Debug.LogError("非法回收！！！" + obj.name);
                return;
            }

            List<GameObject> list;

            obj.transform.SetParent(this.temp3DPool, false);
            obj.gameObject.SetActive(false);
            path = pobj ? pobj.poolObjectPath : path;
            //无路径对象回收
            //if (string.IsNullOrEmpty(path))
            //{
            //    //obj.poolObjectPath = obj.name.Replace("(Clone)", "");
            //    //obj.poolObjectPath = obj.name.Replace(" ", "");
            //}

            if (this.poolObjcDic.ContainsKey(path))
            {
                list = this.poolObjcDic[path];
                list.Add(obj);
            }
            else
            {
                list = new List<GameObject>() { obj };
                this.poolObjcDic.Add(path, list);
            }

            obj.OnDespawn();
        }


        #endregion

    }


}