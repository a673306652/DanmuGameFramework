using System.Collections;
using System.Collections.Generic;
using DanMuGame;
using UnityEngine;
using UnityEngine.UI;

public class TTNoticeGIft : MonoBehaviour
{
    public Image icon;
    public Text name;
    public Image giftImg;
    public bool load = false;
    public Text content;
    private string formatter;
    private string id;

    public void Init(string uid,Sprite gift,string giftName)
    {
        id = uid;
        giftImg.sprite = gift;
        formatter = "送<color=#ffc800>" + giftName + "</color> x 1";
        content.text = formatter;
        load = false;
    }
    
    private void Update()
    {
        if (!this.load)
        {
            var user = FakeUserManager.Instance.existFakeUserDict.TryGet(this.id);
            if (user != null && user.faceUrl != null)
            {
                this.load = true;

                var sprite = DynamicAtlasManager.Instance.GetUserIcon(user.faceUrl);
                if (null == sprite)
                {
                    var self = this;
                    DynamicAtlasManager.Instance.DownloadUserIcon(user.faceUrl, (s) => { self.icon.sprite = s; });
                }
                else
                {
                    this.icon.sprite = sprite;
                }

                name.text = user.nickname;
            }
        }
    }
}
