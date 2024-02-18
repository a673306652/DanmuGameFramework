namespace Services.Feishu
{
    using Modules.Network;
    using UnityEngine;

    public class FeedbackService
    {
        public static void SendMessage(string title, string content)
        {
            // if (string.IsNullOrEmpty(CommonDataCollection.UserId))return;
            var clientBaseRequest = new ClientBaseRequest<FeedbackOperation, FeedbackRequestInput, FeedbackRequestOutput>();
            clientBaseRequest.ConfigHttpHeader(false);
            clientBaseRequest.ConfigHttpDomain(false);

            var payload = new FeedbackPayload();
            var body = new FeedbackContentBody();
            payload.title = title;
            payload.message = content;
            body.text = payload.Format();
            clientBaseRequest.RequestAPI.input.content = body;
            clientBaseRequest.Request((result) =>
            {
                Debug.Log($"Request success: {result.output}");
            }, (error) =>
            {
                Debug.LogError($"Request error: {error.code}|{error.message}");
            });
        }
        
        public static void SendMessage2(string title, string content)
        {
            // if (string.IsNullOrEmpty(CommonDataCollection.UserId))return;
            var clientBaseRequest = new ClientBaseRequest<FeedbackOperation2, FeedbackRequestInput, FeedbackRequestOutput>();
            clientBaseRequest.ConfigHttpHeader(false);
            clientBaseRequest.ConfigHttpDomain(false);

            var payload = new FeedbackPayload();
            var body = new FeedbackContentBody();
            payload.title = title;
            payload.message = content;
            body.text = payload.Format();
            clientBaseRequest.RequestAPI.input.content = body;
            clientBaseRequest.Request((result) =>
            {
                Debug.Log($"Request success: {result.output}");
            }, (error) =>
            {
                Debug.LogError($"Request error: {error.code}|{error.message}");
            });
        }
    }
}