using System;
using System.Collections.Generic;
using Modules.Patterns;
using UnityEngine;

namespace Modules.ResourcePool
{
    public partial class ResourcePool : Singleton<ResourcePool>
    {
        #region Directory
        private const string WINDOW_PREFAB_DIR = "Prefabs/Windows/";
        private const string WINDOW_ITEMS_PREFAB_DIR = "Prefabs/WindowItems/";
        private const string GAME_PREFAB_DIR = "Prefabs/Game/";
        private const string OTHERS_PREFAB_DIR = "Prefabs/Others/";
        private const string MATERIALS_PREFAB_DIR = "Materials/";
        private const string ATLAS_DIR = "Atlas/";
        private const string SPRITES_DIR = "Sprites/";
        private const string JSON_DIR = "Json/";
        private const string AUDIO_DIR = "Audio/";
        private const string AUTOPOOLS_DIR = "Prefabs/AutoPools/";
        private const string CUSTOM_DIR = "Custom/";
        #endregion

        private Dictionary<string, GameObject> prefabDict;
        private Dictionary<string, Material> matDict;
        private Dictionary<string, Dictionary<string, Sprite>> atlasDict;
        private Dictionary<string, Dictionary<string, Sprite>> rawSpriteDict;
        private Dictionary<string, UnityEngine.Object> customDict;

        public ResourcePool()
        {
            prefabDict = new Dictionary<string, GameObject>();
            matDict = new Dictionary<string, Material>();
            atlasDict = new Dictionary<string, Dictionary<string, Sprite>>();
            rawSpriteDict = new Dictionary<string, Dictionary<string, Sprite>>();
            customDict = new Dictionary<string, UnityEngine.Object>();
        }

        public GameObject GetWindowItem(string prefabName)
        {
            return GetItemPrefab(WINDOW_PREFAB_DIR + prefabName);
        }

        public GameObject GetWindowItemItem(string prefabName)
        {
            return GetItemPrefab(WINDOW_ITEMS_PREFAB_DIR + prefabName);
        }

        public GameObject GetGameItem(string prefabName)
        {
            return GetItemPrefab(GAME_PREFAB_DIR + prefabName);
        }

        public GameObject GetOthersItem(string prefabName)
        {
            return GetItemPrefab(OTHERS_PREFAB_DIR + prefabName);
        }

        public Material GetMaterial(string matName)
        {
            Material material = null;
            var matNameStr = MATERIALS_PREFAB_DIR + matName;
            if (matDict.ContainsKey(matNameStr))
            {
                return matDict[matNameStr];
            }
            if (material == null)
            {
                material = Resources.Load<Material>(matNameStr);
            }
            if (material != null)
            {
                matDict[matNameStr] = material;
                return material;
            }
            else
            {
                Debug.LogError("Material " + matNameStr + " does not exist");
            }
            return null;
        }

        public string GetJsonString(string name)
        {
            var resNameStr = JSON_DIR + name;
            var asset = Resources.Load<TextAsset>(resNameStr);
            return asset.text;
        }

        public Sprite GetRawSpriteItem(string dirName, string spriteName)
        {
            var resNameStr = dirName + spriteName;
            var asset = Resources.Load<Sprite>(resNameStr);
            return asset;
        }

        public AudioClip GetAudioClip(string name)
        {
            var resNameStr = AUDIO_DIR + name;
            var asset = Resources.Load<AudioClip>(resNameStr);
            return asset;
        }

        public GameObject GetAutoPoolsItem(string name)
        {
            var resNameStr = AUTOPOOLS_DIR + name;
            var asset = Resources.Load<GameObject>(resNameStr);
            return asset;
        }

        private GameObject GetItemPrefab(string prefabNameStr)
        {
            GameObject prefab = null;
            if (prefabDict.ContainsKey(prefabNameStr))
            {
                return prefabDict[prefabNameStr];
            }
            if (prefab == null)
            {
                prefab = Resources.Load<GameObject>(prefabNameStr);
            }
            if (prefab != null)
            {
                prefabDict[prefabNameStr] = prefab;
                return prefab;
            }
            else
            {
                Debug.LogError("Prefab " + prefabNameStr + " does not exist");
            }
            return null;
        }

        public Sprite GetSpriteItemRaw(string folderName, string spriteName)
        {
            var atlas = GetRawSprites(folderName);
            if (atlas.ContainsKey(spriteName)) return atlas[spriteName];
            return null;
        }

        public Sprite GetSpriteItemRaw(List<string> atlasIdentifier)
        {
            return GetSpriteItemRaw(atlasIdentifier[0], atlasIdentifier[1]);
        }

        public Sprite GetSpriteItem(string atlasName, string spriteName)
        {
            var atlas = GetAtlas(atlasName);
            if (atlas.ContainsKey(spriteName)) return atlas[spriteName];
            return null;
        }

        public Sprite GetSpriteItem(string[] atlasIdentifier)
        {
            return GetSpriteItem(atlasIdentifier[0], atlasIdentifier[1]);
        }

        public Sprite GetSpriteItem(List<string> atlasIdentifier)
        {
            return GetSpriteItem(atlasIdentifier[0], atlasIdentifier[1]);
        }

        public Dictionary<string, Sprite> GetRawSprites(string folderName)
        {
            var rawSpritesNameStr = SPRITES_DIR + folderName;
            if (rawSpriteDict.ContainsKey(rawSpritesNameStr))
            {
                return rawSpriteDict[rawSpritesNameStr];
            }
            Sprite[] sprites = Resources.LoadAll<Sprite>(rawSpritesNameStr) as Sprite[];
            if (sprites.Length > 0)
            {
                var spritesDict = new Dictionary<string, Sprite>();
                for (var i = 0; i < sprites.Length; i++)
                {
                    spritesDict.Add(sprites[i].name, sprites[i]);
                }
                rawSpriteDict[rawSpritesNameStr] = spritesDict;
                return spritesDict;
            }
            else
            {
                Debug.LogError("Atlas " + rawSpritesNameStr + " does not exist or has no sprite");
            }
            return null;
        }

        public Dictionary<string, Sprite> GetAtlas(string atlasName)
        {
            var atlasNameStr = ATLAS_DIR + atlasName;
            if (atlasDict.ContainsKey(atlasNameStr))
            {
                return atlasDict[atlasNameStr];
            }
            Sprite[] sprites = Resources.LoadAll<Sprite>(atlasNameStr) as Sprite[];
            if (sprites.Length > 0)
            {
                var spritesDict = new Dictionary<string, Sprite>();
                for (var i = 0; i < sprites.Length; i++)
                {
                    spritesDict.Add(sprites[i].name, sprites[i]);
                }
                atlasDict[atlasNameStr] = spritesDict;
                return spritesDict;
            }
            else
            {
                Debug.LogError("Atlas " + atlasNameStr + " does not exist or has no sprite");
            }
            return null;
        }

        public T GetCustomItem<T>(string name, bool shouldCached = false) where T : class
        {
            var resNameStr = CUSTOM_DIR + name;
            UnityEngine.Object asset;
            if (shouldCached)
            {
                if (customDict.ContainsKey(resNameStr)) return customDict[resNameStr] as T;
            }
            asset = Resources.Load(resNameStr, typeof(T));
            customDict[resNameStr] = asset;
            return asset as T;
        }
    }
}