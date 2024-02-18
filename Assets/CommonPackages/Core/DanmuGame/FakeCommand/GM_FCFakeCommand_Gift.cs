namespace DanMuGame
{
    using UnityEngine;
    public class GM_FCFakeCommand_Gift : GM_FakeCommandBase
    {
        public FakeDanmuClient client;
        public string giftName;
        public int giftNum;
        public int giftPrice;

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
                        data.faceUrl = string.IsNullOrEmpty(this.faceUrl) ? fakeData.faceUrl : this.faceUrl;
                        FakeUserManager.Instance.existFakeUserDict.Add(this.uid, data);
                    }

                    this.client.GiftMessageHandlerAsync(this.uid, data.nickname, data.faceUrl, this.giftName, "0", this.giftPrice, this.giftNum);
                }
            }
        }
    }
}