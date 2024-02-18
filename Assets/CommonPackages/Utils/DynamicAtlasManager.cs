using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class DynamicAtlasManager : MonoBehaviour
{
    private int atlasSize = 2048;
    private int maxSpritePerAtlas = 20;
    private TextureFormat textureFormat = TextureFormat.RGB24;
    private bool useMipmaps = false;

    private static DynamicAtlasManager instance;

    public static DynamicAtlasManager Instance
    {
        get
        {
            if (null == instance)
            {
                GameObject go = new GameObject("DynamicAtlasManager");
                instance = go.AddComponent<DynamicAtlasManager>();
            }
            return instance;
        }
    }

    private Dictionary<string, AtlasPack> atlasDictionary;
    private Dictionary<string, Rect> spriteRects;
    private Dictionary<string, Sprite> originalSpriteCache;
    private Dictionary<string, Material> materialMap;
    private Dictionary<string, Sprite> url2SpriteMap;

    void Awake()
    {
        atlasDictionary = new Dictionary<string, AtlasPack>();
        spriteRects = new Dictionary<string, Rect>();
        originalSpriteCache = new Dictionary<string, Sprite>();
        materialMap = new Dictionary<string, Material>();
        url2SpriteMap = new Dictionary<string, Sprite>();
    }

    public Material GetMaterial(string url)
    {
        if (string.IsNullOrEmpty(url)) return null; 
        if (materialMap.ContainsKey(url))
        {
            return materialMap[url];
        }
        return null;
    }

    public Material SetMaterial(string url, Material material)
    {
        materialMap[url] = material;
        return GetMaterial(url);
    }

    public Sprite GetUserIcon(string url)
    {
        if (string.IsNullOrEmpty(url)) return null;
        if (url2SpriteMap.ContainsKey(url))
        {
            return url2SpriteMap[url];
        }
        return null;
    }

    public void AddSpritesToDynamicAtlas(string atlasName, Sprite[] sprites)
    {
        if (null == sprites || sprites.Length == 0) return;

        AtlasPack atlasPack;
        Texture2D atlas;
        if (atlasDictionary.ContainsKey(atlasName))
        {
            atlasPack = atlasDictionary[atlasName];
            atlas = atlasPack.T2D;
        }
        else
        {
            atlas = new Texture2D(atlasSize, atlasSize, textureFormat, useMipmaps);
            atlas.filterMode = FilterMode.Bilinear;
            atlasPack = new AtlasPack() { Name = atlasName, T2D = atlas };
            atlasDictionary.Add(atlasName, atlasPack);
        }

        for (var i = 0; i < sprites.Length; i++)
        {
            if (!originalSpriteCache.ContainsKey(sprites[i].name))
            {
                originalSpriteCache.Add(sprites[i].name, sprites[i]);
            }
        }

        int xOffset = 0;
        int yOffset = 0;
        int maxHeight = 0;

        for (int i = 0; i < sprites.Length; i++)
        {
            Sprite sprite = sprites[i];
            Texture2D spriteTexture = sprite.texture;

            if (xOffset + sprite.rect.width > atlas.width)
            {
                xOffset = 0;
                yOffset += maxHeight;
                maxHeight = 0;
            }

            // Copy the texture using CopyTexture method
            Graphics.CopyTexture(spriteTexture, 0, 0, (int)sprite.rect.x, (int)sprite.rect.y, (int)sprite.rect.width, (int)sprite.rect.height, atlas, 0, 0, xOffset, yOffset);

            spriteRects[sprite.name] = new Rect(xOffset, yOffset, sprite.rect.width, sprite.rect.height);

            xOffset += (int)sprite.rect.width;
            maxHeight = Mathf.Max(maxHeight, (int)sprite.rect.height);
            atlasPack.Count += 1;
        }
        atlasPack.T2D = atlas;

        atlasDictionary[atlasPack.Name] = atlasPack;
    }

    public Sprite GetSpriteFromDynamicAtlas(string atlasName, string spriteName)
    {
        if (!atlasDictionary.ContainsKey(atlasName) || !spriteRects.ContainsKey(spriteName))
        {
            return null;
        }

        AtlasPack atlasPack = atlasDictionary[atlasName];
        Texture2D atlas = atlasPack.T2D;
        Rect spriteRect = spriteRects[spriteName];

        // Get the original sprite
        if (!originalSpriteCache.ContainsKey(spriteName))
        {
            return null;
        }

        Sprite originalSprite = originalSpriteCache[spriteName];

        // Calculate the border of the new sprite based on the original sprite's border
        Vector4 border = originalSprite.border;

        // Create the new sprite with the correct border
        return Sprite.Create(atlas, spriteRect, new Vector2(0.5f, 0.5f), originalSprite.pixelsPerUnit, 0, SpriteMeshType.Tight, border);
    }

    public void DownloadUserIcon(string url, UnityAction<Sprite> onComplete = null)
    {
        StartCoroutine(DownloadIcon(url, true, onComplete));
    }

    private IEnumerator DownloadIcon(string url, bool firstTry = true, UnityAction<Sprite> onComplete = null)
    {
        var originUrl = url;

        // if (firstTry)
        // {
        //     url = $"{url}@64w_64h.jpg";
        // }
        // else
        // {
        //     Debug.LogWarning("第二次下载用户头像");
        // }
        using (var uwr = UnityWebRequest.Get(url))
        {
            yield return uwr.SendWebRequest();
            if (uwr.result == UnityWebRequest.Result.ConnectionError || uwr.result == UnityWebRequest.Result.ProtocolError || uwr.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogWarning(uwr.error);
                if (firstTry)
                {
                    StartCoroutine(DownloadIcon(originUrl, false, onComplete));
                }
            }
            else
            {
                var data = uwr.downloadHandler.data;
                var tex = new Texture2D(64, 64);
                var success = tex.LoadImage(data);
                if (!success)
                {
                    Debug.LogWarning("头像还是有点问题，我再试试");
                    tex.LoadRawTextureData(data);
                }
                tex.Apply();
                url2SpriteMap[url] = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5F, 0.5F));
                onComplete?.Invoke(url2SpriteMap[url]);
            }
        }
    }

    private struct AtlasPack
    {
        public string Name;
        public Texture2D T2D;
        public int Count;
    }
}