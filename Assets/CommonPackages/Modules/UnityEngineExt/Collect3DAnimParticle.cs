using Modules.Patterns;
using UnityEngine;

public class Collect3DAnimParticle : MonoBehaviour, PoolableObject
{
    public virtual void OnObjectRecycle()
    {
    }

    public virtual void OnObjectReset()
    {
    }
}