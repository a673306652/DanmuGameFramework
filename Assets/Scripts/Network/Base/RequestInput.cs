namespace Modules.Network
{
    using Newtonsoft.Json;
    public class RequestInput
    {
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}