using System.Collections;
using System.Collections.Generic;
using DanMuGame;
using UnityEngine;
using UnityEngine.UI;

public class GiftMsgItem : MonoBehaviour
{
    private string id;
    private string User;
    private string _content;
    private string nickname;
    public bool load = false;
    private Sprite userIcon;
    public Text text;
    public Image giftIcon;

    public void Init(string uid, TTGiftItem item)
    {
        load = false;
        id = uid;
        _content = "<color=#8DE1F6>{0}:</color>" + "送出了1个";
        giftIcon.sprite = item.GiftSprite;
        var formatter = string.Format(_content, "神秘人");
        text.text = formatter;
    }
    
    private void Update()
    {
        if (!this.load)
        {
            var user = FakeUserManager.Instance.existFakeUserDict.TryGet(this.id);
            if (user != null && user.faceUrl != null)
            {
                nickname = user.nickname;
                this.load = true;

                var sprite = DynamicAtlasManager.Instance.GetUserIcon(user.faceUrl);
                if (null == sprite)
                {
                    var self = this;
                    DynamicAtlasManager.Instance.DownloadUserIcon(user.faceUrl, (s) => { self.userIcon = s; });
                }
                else
                {
                    this.userIcon = sprite;
                }

                var formatter = string.Format(_content, nickname);
                text.text = formatter;
            }
        }
    }
}
