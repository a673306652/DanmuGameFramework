using Modules.Patterns;
using UnityEngine;

public class Collect3DParticlePool : ObjectPool<Collect3DAnimParticle>, IParticlePool<Collect3DAnimParticle>
{
    public GameObject GetParticle()
    {
        return GetOne();
    }
    public void ReturnParticle(GameObject go)
    {
        ReturnOne(go);
    }
}