namespace Modules.Network
{
    using Newtonsoft.Json;
    public enum Method
    {
        Post,
        Get
    }
    public abstract class RequestOperation
    {
        public abstract string Route { get; }
        public abstract Method Method { get; }
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}