namespace Modules.Controllers
{
    using System.Collections.Generic;
    using Modules.Patterns;
    using ResourcePool;
    using UnityEngine;

    public class OPController : MonoSingleton<OPController>
    {
        private Dictionary<string, IObjectPool> pools = new Dictionary<string, IObjectPool>();

        public IObjectPool GetPool(string resName)
        {
            if (!pools.ContainsKey(resName))
            {
                var prefab = ResourcePool.Instance.GetAutoPoolsItem(resName);
                var instance = Instantiate(prefab, transform, false);
                pools[resName] = instance.GetComponent<IObjectPool>();
            }
            return pools[resName];
        }
    }
}