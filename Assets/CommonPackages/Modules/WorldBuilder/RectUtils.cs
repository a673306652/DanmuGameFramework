namespace Modules.WorldBuilder
{
    using UnityEngine;
    public static class RectUtils
    {
        public static void GetCenterPos(GameObject target, out Vector3 centerPos, out Vector3 size)
        {
            Transform parent = target.transform;
            Vector3 center = Vector3.zero;
            Renderer[] renders = parent.GetComponentsInChildren<Renderer>();
            if (renders.Length > 0)
            {
                foreach (Renderer child in renders)
                {
                    center += child.bounds.center;
                }
                center /= renders.Length;
                Bounds bounds = new Bounds(center, Vector3.zero);
                foreach (Renderer child in renders)
                {
                    bounds.Encapsulate(child.bounds);
                }
                centerPos = bounds.center;
                size = bounds.size;
            }
            else
            {
                size = Vector3.one;
                centerPos = target.transform.position;
            }
        }
    }
}