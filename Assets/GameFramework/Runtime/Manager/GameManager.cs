using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ResourceManager = Daan.ResourceManager;

namespace Daan
{
    using SceneManager = UnityEngine.SceneManagement.SceneManager;
    public enum RendererMode
    {
        Opaque,
        Cutout,
        Fade,
        Transparent,
    }

    public interface IManager
    {
        void Init();
        void Tick();
        void Reset();
        void OnLevelLoaded(Scene scene, LoadSceneMode mode);
    }
    public class GameManager : Manager<GameManager>
    {
        [HideInInspector]
        public int gameSeed;
        [HideInInspector]
        public System.Random random;

        private List<IManager> managers = new List<IManager>();

        public int frameRate = -1;
        private void Awake()
        {
            if (Instance == null)
            {
                this.Init();
            }
            if (this.frameRate != -1)
            {
                Application.targetFrameRate = this.frameRate;
            }
        }

        public override void Init()
        {
            //利用当前时间戳当做随机种子
            this.random = new System.Random((int)DateTime.Now.Ticks);
            this.gameSeed = random.Next();
            GameObject.DontDestroyOnLoad(this.gameObject);

            Debug.Log("初始化GameManager");
            SceneManager.sceneLoaded += (Scene scene, LoadSceneMode mode) =>
            {
                Debug.Log("切换场景");
                foreach (var item in this.managers)
                {
                    item.OnLevelLoaded(scene, mode);
                }
                var otherManager = FindObjectsOfType<GameManager>();
                for (int i = 0; i < otherManager.Length; i++)
                {
                    if (otherManager[i] == Instance) continue;
                    GameObject.Destroy(otherManager[i].gameObject);
                }
                StopAllCoroutines();
            };
        }


        public void RegisterManager(IManager manager)
        {
            this.managers.Add(manager);
        }
        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.LeftArrow))
            //{
            //    Time.timeScale -= 0.5F;
            //}

            //if (Input.GetKeyDown(KeyCode.RightArrow))
            //{
            //    Time.timeScale += 0.5F;
            //}

            //if (Input.GetKeyDown(KeyCode.UpArrow))
            //{
            //    Time.timeScale = 1;
            //}

            //if (Input.GetKeyDown(KeyCode.DownArrow))
            //{
            //    Time.timeScale = 0;
            //}
            foreach (var item in this.managers)
            {
                item.Tick();
            }
        }

        /// <summary>
        /// 切换材质渲染模式
        /// </summary>
        /// <param name="material"></param>
        /// <param name="rendererMode"></param>
        public void SetMaterialRendererMode(Material material, RendererMode rendererMode)
        {
            switch (rendererMode)
            {
                case RendererMode.Opaque:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = -1;
                    break;
                case RendererMode.Cutout:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                    material.SetInt("_ZWrite", 1);
                    material.EnableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 2450;
                    break;
                case RendererMode.Fade:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.EnableKeyword("_ALPHABLEND_ON");
                    material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
                case RendererMode.Transparent:
                    material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                    material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    material.SetInt("_ZWrite", 0);
                    material.DisableKeyword("_ALPHATEST_ON");
                    material.DisableKeyword("_ALPHABLEND_ON");
                    material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    material.renderQueue = 3000;
                    break;
            }
        }
        AsyncOperation async;
        IEnumerator ClearResources()
        {
            async = Resources.UnloadUnusedAssets(); //清除物体占用的内存, 能彻底清理内存
            yield return async;
        }
    }
}