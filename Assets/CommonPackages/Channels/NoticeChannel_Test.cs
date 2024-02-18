namespace Channels.Test
{
    using Channels.NoticeChannel;
    using Modules.Utilities;
    using UnityEngine;

    public class NoticeChannel_Test : MonoBehaviour
    {
        public NoticeChannel channel;
        private float delta;
        private string[] users = new string[] { "David", "Lucy", "Rose" };
        private string[] placements = new string[] { "Left", "Right", "Top", "Bottom" };

        void Start()
        {
            channel = NoticeChannel.New()
                                    .Configure("Left")
                                    .Configure("Right")
                                    .Configure("Top")
                                    .Configure("Bottom");

        }

        void Update()
        {
            delta += Time.deltaTime;
            if (delta >= 0.25f)
            {
                delta = 0f;
                for (var i = 0; i < 10; i++)
                {
                    channel.Put(users[RandomUtils.Range(0, users.Length)],
                                "Gift",
                                placements[RandomUtils.Range(0, placements.Length)],
                                RandomUtils.Range(0, 10),
                                RandomUtils.Range(1f, 3f),
                                1,
                                (self) =>
                                {
                                    Debug.LogWarning("播放消息：" + self.UserId + "|" + self.MsgType + "|" + self.Placement + "|" + (int)self.Payload);
                                },
                                (other, self) =>
                                {
                                    self.Payload = (int)other.Payload + (int)self.Payload;
                                    return self;
                                },
                                (self) =>
                                {
                                    Debug.LogWarning("销毁消息：" + self.UserId + "|" + self.MsgType + "|" + self.Placement + "|" + (int)self.Payload);
                                });
                }
            }
            channel.Tick(Time.deltaTime);
        }
    }
}