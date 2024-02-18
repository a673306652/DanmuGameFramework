using System;
using System.Collections;
using System.Collections.Generic;
using DanMuGame;
using UnityEngine;
using UnityEngine.UI;

public class TalkItem : MonoBehaviour
{
    private string id;
    private string User;
    private string _content;
    private string nickname;
    public bool load = false;
    private Sprite userIcon;
    public Text text;
    public bool RealUser;

    public void Init(string uid, string content)
    {
        load = false;
        id = uid;
        _content = "<color=#8DE1F6>{0}:</color>" + content;
        var formatter = string.Format(_content, "神秘人");
        text.text = formatter;
    }

    public void Init2(string uid, string content)
    {
        load = false;
        id = uid;
        _content = "<color=#8DE1F6>{0}</color>" + content;
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
            else if (null == user)
            {
                var u2 = UserManager.Instance.GetUser(this.id);
                
                if (u2 != null && u2.faceUrl != null)
                {
                    nickname = u2.nickname;
                    this.load = true;

                    var sprite = DynamicAtlasManager.Instance.GetUserIcon(u2.faceUrl);
                    if (null == sprite)
                    {
                        var self = this;
                        DynamicAtlasManager.Instance.DownloadUserIcon(u2.faceUrl, (s) => { self.userIcon = s; });
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
}