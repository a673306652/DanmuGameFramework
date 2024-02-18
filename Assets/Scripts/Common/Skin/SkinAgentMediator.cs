using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[ClassAvatar("Icon_Editor_Guide","皮肤管理器")]
public class SkinAgentMediator : MonoBehaviour
{
    public SkinAgent[] skins;

    public SkinAgent GetRandomSkin()
    {
        return skins[Random.Range(0, skins.Length)];
    }

    public SkinAgent GetSkinByID(int id)
    {
        return skins[id];
    }

    public void OpenSkin(int id)
    {
        for (var i = 0; i < skins.Length; i++)
        {
            skins[i].gameObject.SetActive(i == id);
        }
    }
}