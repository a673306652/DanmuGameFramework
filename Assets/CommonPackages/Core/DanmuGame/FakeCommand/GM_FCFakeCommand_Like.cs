namespace DanMuGame
{
    using UnityEngine;


    public class GM_FCFakeCommand_Like : GM_FakeCommandBase
    {
        public FakeDanmuClient client;

        private float blockingDelta = 0f;

        private void Awake()
        {
            if (this.client == null)
            {
                this.client = GameObject.FindObjectOfType<FakeDanmuClient>();
            }
        }
        // Update is called once per frame
        void Update()
        {
            blockingDelta += Time.deltaTime;
            if (Input.GetKeyDown(this.key))
            {
                if (blockingDelta >= 0.25f)
                {
                    blockingDelta = 0f;
                    var data = FakeUserManager.Instance.existFakeUserDict.TryGet(this.uid);
                    if (data == null)
                    {
                        var fakeData = FakeUserManager.Instance.GetFakeUser();
                        data = new FakeUserManager.Data();
                        data.nickname = string.IsNullOrEmpty(this.nickname) ? fakeData.nickname : this.nickname;
                        if (string.IsNullOrEmpty(fakeData.faceUrl))
                        {
                            data.faceUrl = fakeData.faceUrl;// string.IsNullOrEmpty(this.faceUrl) ? fakeData.faceUrl : this.faceUrl;
                        }
                        // https://p11.douyinpic.com/aweme/100x100/aweme-avatar/tos-cn-avt-0015_6421ea1ef6d7a831f4f55ed41b18c05c.jpeg?from=3067671334
                        FakeUserManager.Instance.existFakeUserDict.Add(this.uid, data);
                    }

                    this.client.LikeMessageHandlerAsync(this.uid, data.nickname, data.faceUrl, 10);
                }
            }
        }
    }
}