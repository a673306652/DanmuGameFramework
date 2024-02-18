namespace Modules.Physics
{
    using UnityEngine;
    public interface IVelocityObject
    {
        public Vector3 GetVelocity();
        public Vector3 GetMovementDirection();
    }
}