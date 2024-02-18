using System;
using System.Collections;
using System.Collections.Generic;
using DanMuGame; 
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TTTestTool : HisaoMono
{
    public static TTTestTool Instance;
     public string _uid;
     public string _nickName;

   

    [Range(0,3)]
    public float delayTime;

    public InputField uidText;

    public InputField nickNameText;

    public TalkItem TalkTemplate;
    public GiftMsgItem GiftTemplate;
    public ScrollRect rect;
    public BaseDanmuClient Clinet;
    public Transform contentAnchor;
    public InputField debugContent;

    public Transform[] noticeAnchor;

    private void Awake()
    {
        uidText.text = "001216";
        nickNameText.text = "郑介隆";
        Instance = this;
        TTNoticeManager.InitNoticeList(new string[]{"0","1"});
    }

    // Update is called once per frame
    void Update()
    {
        _uid = uidText.text;
        _nickName = nickNameText.text;
        TTNoticeManager.UpdateNoticeSystem();
    }

    public void TTTalk(string content)
    {
        if (!string.IsNullOrEmpty(_uid) && !string.IsNullOrEmpty(_nickName))
        {

            var uid = _uid;
            var nickName = _nickName;
             
            var data = FakeUserManager.Instance.existFakeUserDict.TryGet(uid);
            if (data == null)
            {
                var fakeData = FakeUserManager.Instance.GetFakeUser();
                data = new FakeUserManager.Data();
                data.nickname = _nickName;
                data.faceUrl = fakeData.faceUrl;
                FakeUserManager.Instance.existFakeUserDict.Add(uid, data);
            }
            Exec((a) =>
            {
                this.Clinet.DanmuMessageHandlerAsync(uid, data.nickname, content, data.faceUrl);
            }, delayTime);
           
        }
    }

    public void FakeMessage(string content)
    {
        var a = Instantiate(TalkTemplate, contentAnchor);
        a.Init("-1", content);
        a.GetComponent<ContentSizeFitter>().enabled = false;
        a.GetComponent<ContentSizeFitter>().enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(a.GetRectTransform());
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentAnchor.GetRectTransform());
        rect.verticalNormalizedPosition = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentAnchor.GetRectTransform());
        
    }
    public void FakeTalk(string uid,string content)
    {
  
        var a = Instantiate(TalkTemplate, contentAnchor);
        a.Init(uid, content);
        a.GetComponent<ContentSizeFitter>().enabled = false;
        a.GetComponent<ContentSizeFitter>().enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(a.GetRectTransform());
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentAnchor.GetRectTransform());
        rect.verticalNormalizedPosition = 0;
        LayoutRebuilder.ForceRebuildLayoutImmediate(contentAnchor.GetRectTransform());
        
    }

    public void TTLike()
    {
        if (!string.IsNullOrEmpty(_uid) && !string.IsNullOrEmpty(_nickName))
        {
            var uid = _uid;
            var nickName = _nickName;
        
            var a = Instantiate(TalkTemplate, contentAnchor);
            var data = FakeUserManager.Instance.existFakeUserDict.TryGet(uid);
            if (data == null)
            {
                var fakeData = FakeUserManager.Instance.GetFakeUser();
                data = new FakeUserManager.Data();
                data.nickname = nickName;
                data.faceUrl = fakeData.faceUrl;
                FakeUserManager.Instance.existFakeUserDict.Add(uid, data);
            }

            Exec((a) =>
            {
                this.Clinet.LikeMessageHandlerAsync(uid, data.nickname, data.faceUrl,1);
            }, delayTime);
           
            a.Init2(uid, "点赞了主播！");
            a.GetComponent<ContentSizeFitter>().enabled = false;  
            a.GetComponent<ContentSizeFitter>().enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(a.GetRectTransform());
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentAnchor.GetRectTransform());
            rect.verticalNormalizedPosition = 0;
            LayoutRebuilder.ForceRebuildLayoutImmediate(contentAnchor.GetRectTransform());
        }
    }

    public void TTGift(string giftID,string giftName,TTGiftItem Giftitem)
    {
        if (!string.IsNullOrEmpty(_uid) && !string.IsNullOrEmpty(_nickName))
        {
            var uid = _uid;
            var nickName = _nickName;
         
            var a = Instantiate(GiftTemplate, contentAnchor);
            var data = FakeUserManager.Instance.existFakeUserDict.TryGet(uid);
            if (data == null)
            {
                var fakeData = FakeUserManager.Instance.GetFakeUser();
                data = new FakeUserManager.Data();
                data.nickname = nickName;
                data.faceUrl = fakeData.faceUrl;
                FakeUserManager.Instance.existFakeUserDict.Add(uid, data);
            }

            Exec((a) =>
            {
                this.Clinet.GiftMessageHandlerAsync(uid, data.nickname, data.faceUrl, giftName, giftID, 1, 1);
            }, delayTime);

          
            // TTNoticeManager.EnqueueFreeNotice(1, (item) =>
            // {
            //     var c = Resources.Load<TTNoticeGIft>("Prefabs/TTNoticeGift");
            //     var x = Instantiate(c, noticeAnchor[int.Parse(item.Keys[0])]);
            //     x.Init(uid,Giftitem.GiftSprite,Giftitem.GiftName);
            //     item.SetExComplete(() =>
            //     {
            //         Destroy(x);
            //     });
            // }, () =>
            // {
            //     
            // },uid+"TT",3);
           
            a.Init(uid, Giftitem);
 
            rect.verticalNormalizedPosition = 0;
         

        }
    }
 
}