namespace DanMuGame
{
    using Modules.Patterns;

    public class FakeDanmuClient : BaseDanmuClient
    {
        public FakeDanmuClient Instance
        {
            get
            {
                return MonoSingletonGeneric<FakeDanmuClient>.Instance;
            }
        }

        public override void DanmuMessageHandlerAsync(string uid, string nickname, string content, string faceUrl)
        {
            var user = UserManager.Instance.UpdateUserInfo(this.platform, this.roomId, uid, nickname, faceUrl);
            UserManager.Instance.UpdateActivePlayerList(this.platform, this.roomId, uid);
            DanmuCommandManager.Instance.ParseCommand(UserManager.GetUserKey(user.info), content);
        }

        public override void GiftMessageHandlerAsync(string uid, string nickname, string faceUrl, string giftName, string giftId, int giftPrice, int giftCount)
        {
            var user = UserManager.Instance.UpdateUserInfo(this.platform, this.roomId, uid, nickname, faceUrl);
            UserManager.Instance.UpdateActivePlayerList(this.platform, this.roomId, uid);
            this.ParseGift(UserManager.GetUserKey(user.info), giftName, giftId, giftPrice, giftCount);
        }

        public override void LikeMessageHandlerAsync(string uid, string nickname, string faceUrl, int likeCount)
        {
            var user = UserManager.Instance.UpdateUserInfo(this.platform, this.roomId, uid, nickname, faceUrl);
            UserManager.Instance.UpdateActivePlayerList(this.platform, this.roomId, uid);
            DanmuCommandManager.Instance.ParseCommand(UserManager.GetUserKey(user.info), $"点赞", likeCount);
        }
    }
}