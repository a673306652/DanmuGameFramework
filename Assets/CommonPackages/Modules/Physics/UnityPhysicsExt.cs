namespace Modules.Physics
{
    using UnityEngine;
    public static class UnityPhysicsExt
    {
        /// <summary>
        /// Get collider core metrics on default world space and up axis.
        /// </summary>
        public static CapsuleInfo GetColliderInfo(CapsuleCollider collider, Vector3 lossyScale)
        {
            var radius = collider.radius.ToWorldScale(lossyScale);
            var center = collider.transform.position + collider.center.ToWorldScale(lossyScale);
            var height = collider.height.ToWorldScale(lossyScale);
            // Ignore collider direction, use world-up instead.
            var point0 = center + Vector3.up * (height / 2f - radius);
            var point1 = center - Vector3.up * (height / 2f - radius);

            return new CapsuleInfo()
            {
                Radius = radius,
                Center = center,
                Height = height,
                Point0 = point0,
                Point1 = point1
            };
        }

        /// <summary>
        /// Get collider core metrics on self transform space and direction axis.
        /// </summary>
        public static CapsuleInfo GetColliderInfoXYZ(CapsuleCollider collider, Vector3 lossyScale)
        {
            var radius = collider.radius.ToWorldScale(lossyScale);
            var center = collider.transform.position + collider.center.ToWorldScale(lossyScale);
            var height = collider.height.ToWorldScale(lossyScale);
            var direction = collider.direction;
            Vector3 point0 = center;
            Vector3 point1 = center;
            switch (direction)
            {
                case 0: // X
                    point0 += collider.transform.right * (height / 2f - radius);
                    point1 -= collider.transform.right * (height / 2f - radius);
                    break;
                case 1: // Y
                    point0 += collider.transform.up * (height / 2f - radius);
                    point1 -= collider.transform.up * (height / 2f - radius);
                    break;
                case 2: // Z
                    point0 += collider.transform.forward * (height / 2f - radius);
                    point1 -= collider.transform.forward * (height / 2f - radius);
                    break;
            }

            return new CapsuleInfo()
            {
                Radius = radius,
                Center = center,
                Height = height,
                Point0 = point0,
                Point1 = point1
            };
        }

        /// <summary>
        /// Evaluate a bezier curve point with specific parameters.
        /// </summary>
        /// <param name="t">Value from 0 to 1 with 0 at starting point and 1 at endpoint.</param>
        /// <param name="start">The starting position of curve.</param>
        /// <param name="center">The middle-control position of curve.</param>
        /// <param name="end">The end position of curve.</param>
        public static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
        {
            return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
        }
    }

    public struct CapsuleInfo
    {
        public float Radius;
        public Vector3 Center;
        public float Height;
        public Vector3 Point0;
        public Vector3 Point1;
    }
}