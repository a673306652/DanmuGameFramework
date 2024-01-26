using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BtnRandomUser : MonoBehaviour
{
    public Queue<string> uid;

    public void Awake()
    {
        var c = GetComponent<Button>();
        c.onClick.AddListener(ChangePreset);
        PreCreate();
    }

    private void PreCreate()
    {
        uid = new Queue<string>();
        for (int i = 0; i < 500; i++)
        {
            var seed = gameObject.GetInstanceID()+ "s1" + i;
            uid.Enqueue(seed);
        }
    }

    private void ChangePreset()
    {
        string id = null;
        if (uid.Count>0)
        {
            id = uid.Dequeue();
        }
        else
        {
            return;
        }
        TTTestTool.Instance.uidText.text = id;
        TTTestTool.Instance.nickNameText.text = "种子用户"+id;
        TTTestTool.Instance._uid = id;
        TTTestTool.Instance._nickName = "种子用户"+id;
    }
}
