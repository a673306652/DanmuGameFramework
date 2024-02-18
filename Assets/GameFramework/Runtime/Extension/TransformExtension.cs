using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TransformExtension
{
    public static RectTransform GetRectTransform(this Component cp)
    {
        return cp.transform as RectTransform;
    }

    public static void DestroyAllChildren(this Transform self)
    {
        for (int i = 0; i < self.childCount; ++i)
        {
            GameObject.Destroy(self.GetChild(i).gameObject);
        }
    }

    public static void SetActiveAllChildren(this Transform self, bool active, bool recursive = false)
    {
        for (int i = 0; i < self.childCount; ++i)
        {
            Transform child = self.GetChild(i);
            child.gameObject.SetActive(active);
            if (recursive)
            {
                child.SetActiveAllChildren(active, recursive);
            }
        }
    }

    public static List<T> GetChildren<T>(this Transform trans, string name = null)
    {
        var subTrans =
            name == null ? trans : trans.Find(name);

        var list = new List<T>(subTrans.childCount);

        for (int i = 0; i < subTrans.childCount; i++)
            list.Add(subTrans.GetChild(i).GetComponent<T>());
        return list;
    }

    public static void SetLayerAllChildren(this Transform self, string layerName, bool recursive = false)
    {
        self.SetLayerAllChildren(LayerMask.NameToLayer(layerName), recursive);
    }

    public static void SetLayerAllChildren(this Transform self, int layer, bool recursive = false)
    {
        for (int i = 0; i < self.childCount; ++i)
        {
            Transform child = self.GetChild(i);
            child.gameObject.layer = layer;
            if (recursive)
            {
                child.SetLayerAllChildren(layer, recursive);
            }
        }
    }

    public static void SetEnabledComponentsInChildren<T>(this Transform self, bool enabled, bool recursive = false) where T : Behaviour
    {
        for (int i = 0; i < self.childCount; ++i)
        {
            Transform child = self.GetChild(i);
            T component = child.GetComponent<T>();
            if (component != null)
            {
                component.enabled = enabled;
            }
            if (recursive)
            {
                child.SetEnabledComponentsInChildren<T>(enabled, recursive);
            }
        }
    }

    public static T Find<T>(this Transform self, string name) where T : Component
    {
        Transform child = self.Find(name);
        return (child != null) ? child.GetComponent<T>() : null;
    }

    public static Transform FindChild(this Transform self, System.Predicate<Transform> predicate)
    {
        for (int i = 0; i < self.childCount; ++i)
        {
            Transform child = self.GetChild(i);
            if (predicate(child))
            {
                return child;
            }
        }
        return null;
    }

    public static T FindChild<T>(this Transform self, System.Predicate<T> predicate) where T : Component
    {
        for (int i = 0; i < self.childCount; ++i)
        {
            T child = self.GetChild(i).GetComponent<T>();
            if (predicate(child))
            {
                return child;
            }
        }
        return null;
    }

    public static Transform FindFirstActiveChild(this Transform self)
    {
        return self.FindChild((child) => child.gameObject.activeSelf);
    }

    public static Transform FindFirstInactiveChild(this Transform self)
    {
        return self.FindChild((child) => !child.gameObject.activeSelf);
    }

    public static T FindFirstActiveChild<T>(this Transform self) where T : Component
    {
        return self.FindChild<T>((child) => child.gameObject.activeSelf);
    }

    public static T FindFirstInactiveChild<T>(this Transform self) where T : Component
    {
        return self.FindChild<T>((child) => !child.gameObject.activeSelf);
    }

    public static Transform FindNearest2D(this Transform self, float radius, int layerMask,
        System.Func<Transform, Transform, bool> condition = null)
    {
        Transform nearest = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(self.position, radius, layerMask);
        float minDistance = float.MaxValue;
        foreach (Collider2D collider in colliders)
        {
            if (collider.transform == self) continue;
            if (condition != null && !condition(collider.transform, self)) continue;
            float distance = Vector2.Distance(self.position, collider.transform.position);
            if (distance < minDistance)
            {
                nearest = collider.transform;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static Transform FindNearest2DNonAlloc(this Transform self, float radius, Collider2D[] colliders, int layerMask,
        System.Func<Transform, Transform, bool> condition = null)
    {
        Transform nearest = null;
        int count = Physics2D.OverlapCircleNonAlloc(self.position, radius, colliders, layerMask);
        float minDistance = float.MaxValue;
        for (int i = 0; i < count; ++i)
        {
            Transform target = colliders[i].transform;
            if (target == self) continue;
            if (condition != null && !condition(target, self)) continue;
            float distance = Vector2.Distance(self.position, target.position);
            if (distance < minDistance)
            {
                nearest = target;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static T FindNearest2D<T>(this Transform self, float radius, int layerMask,
        System.Func<T, Transform, bool> condition = null) where T : Component
    {
        T nearest = null;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(self.position, radius, layerMask);
        float minDistance = float.MaxValue;
        foreach (Collider2D collider in colliders)
        {
            if (collider.transform == self) continue;
            T component = collider.GetComponent<T>();
            if (component == null) continue;
            if (condition != null && !condition(component, self)) continue;
            float distance = Vector2.Distance(self.position, collider.transform.position);
            if (distance < minDistance)
            {
                nearest = component;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static T FindNearest2DNonAlloc<T>(this Transform self, float radius, Collider2D[] colliders, int layerMask,
        System.Func<T, Transform, bool> condition = null) where T : Component
    {
        T nearest = null;
        int count = Physics2D.OverlapCircleNonAlloc(self.position, radius, colliders, layerMask);
        float minDistance = float.MaxValue;
        for (int i = 0; i < count; ++i)
        {
            if (colliders[i].transform == self) continue;
            T component = colliders[i].GetComponent<T>();
            if (component == null) continue;
            if (condition != null && !condition(component, self)) continue;
            float distance = Vector2.Distance(self.position, component.transform.position);
            if (distance < minDistance)
            {
                nearest = component;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static Transform[] FindAllNearby2D(this Transform self, float radius, int layerMask,
        System.Func<Transform, Transform, bool> condition = null)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(self.position, radius, layerMask);
        List<Transform> results = new List<Transform>();
        foreach (Collider2D collider in colliders)
        {
            if (collider.transform == self) continue;
            if (condition != null && !condition(collider.transform, self)) continue;
            results.Add(collider.transform);
        }
        return results.ToArray();
    }

    public static Transform[] FindAllNearby2DNonAlloc(this Transform self, float radius, Collider2D[] colliders, int layerMask,
        System.Func<Transform, Transform, bool> condition = null)
    {
        int count = Physics2D.OverlapCircleNonAlloc(self.position, radius, colliders, layerMask);
        List<Transform> results = new List<Transform>();
        for (int i = 0; i < count; ++i)
        {
            Transform target = colliders[i].transform;
            if (target == self) continue;
            if (condition != null && !condition(target, self)) continue;
            results.Add(target);
        }
        return results.ToArray();
    }

    public static T[] FindAllNearby2D<T>(this Transform self, float radius, int layerMask,
        System.Func<T, Transform, bool> condition = null)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(self.position, radius, layerMask);
        List<T> results = new List<T>();
        foreach (Collider2D collider in colliders)
        {
            if (collider.transform == self) continue;
            T component = collider.GetComponent<T>();
            if (condition != null && !condition(component, self)) continue;
            results.Add(component);
        }
        return results.ToArray();
    }

    public static T[] FindAllNearby2DNonAlloc<T>(this Transform self, float radius, Collider2D[] colliders, int layerMask,
        System.Func<T, Transform, bool> condition = null)
    {
        int count = Physics2D.OverlapCircleNonAlloc(self.position, radius, colliders, layerMask);
        List<T> results = new List<T>();
        for (int i = 0; i < count; ++i)
        {
            if (colliders[i].transform == self) continue;
            T component = colliders[i].GetComponent<T>();
            if (condition != null && !condition(component, self)) continue;
            results.Add(component);
        }
        return results.ToArray();
    }

    public static int ForEachAllNearby2D(this Transform self, float radius, Collider2D[] colliders, int layerMask,
        System.Action<Transform> action, System.Func<Transform, Transform, bool> condition = null)
    {
        int count = Physics2D.OverlapCircleNonAlloc(self.position, radius, colliders, layerMask);
        for (int i = 0; i < count; ++i)
        {
            if (colliders[i].transform == self) continue;
            Transform transform = colliders[i].transform;
            if (condition != null && !condition(transform, self)) continue;
            action.Invoke(transform);
        }
        return count;
    }

    public static int ForEachAllNearby2D<T>(this Transform self, float radius, Collider2D[] colliders, int layerMask,
        System.Action<T> action, System.Func<T, Transform, bool> condition = null)
    {
        int count = Physics2D.OverlapCircleNonAlloc(self.position, radius, colliders, layerMask);
        for (int i = 0; i < count; ++i)
        {
            if (colliders[i].transform == self) continue;
            T component = colliders[i].GetComponent<T>();
            if (condition != null && !condition(component, self)) continue;
            action.Invoke(component);
        }
        return count;
    }

    public static Transform FindNearest3D(this Transform self, float radius, int layerMask,
        System.Func<Transform, Transform, bool> condition = null)
    {
        Transform nearest = null;
        Collider[] colliders = Physics.OverlapSphere(self.position, radius, layerMask);
        float minDistance = float.MaxValue;
        foreach (Collider collider in colliders)
        {
            if (collider.transform == self) continue;
            if (condition != null && !condition(collider.transform, self)) continue;
            float distance = Vector3.Distance(self.position, collider.transform.position);
            if (distance < minDistance)
            {
                nearest = collider.transform;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static Transform FindNearest3DNonAlloc(this Transform self, float radius, Collider[] colliders, int layerMask,
        System.Func<Transform, Transform, bool> condition = null)
    {
        Transform nearest = null;
        int count = Physics.OverlapSphereNonAlloc(self.position, radius, colliders, layerMask);
        float minDistance = float.MaxValue;
        for (int i = 0; i < count; ++i)
        {
            Transform target = colliders[i].transform;
            if (target == self) continue;
            if (condition != null && !condition(target, self)) continue;
            float distance = Vector3.Distance(self.position, target.position);
            if (distance < minDistance)
            {
                nearest = target;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static T FindNearest3D<T>(this Transform self, float radius, int layerMask,
        System.Func<T, Transform, bool> condition = null) where T : Component
    {
        T nearest = null;
        Collider[] colliders = Physics.OverlapSphere(self.position, radius, layerMask);
        float minDistance = float.MaxValue;
        foreach (Collider collider in colliders)
        {
            if (collider.transform == self) continue;
            T component = collider.GetComponent<T>();
            if (component == null) continue;
            if (condition != null && !condition(component, self)) continue;
            float distance = Vector3.Distance(self.position, collider.transform.position);
            if (distance < minDistance)
            {
                nearest = component;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static T FindNearest3DNonAlloc<T>(this Transform self, float radius, Collider[] colliders, int layerMask,
        System.Func<T, Transform, bool> condition = null) where T : Component
    {
        T nearest = null;
        int count = Physics.OverlapSphereNonAlloc(self.position, radius, colliders, layerMask);
        float minDistance = float.MaxValue;
        for (int i = 0; i < count; ++i)
        {
            if (colliders[i].transform == self) continue;
            T component = colliders[i].GetComponent<T>();
            if (component == null) continue;
            if (condition != null && !condition(component, self)) continue;
            float distance = Vector3.Distance(self.position, component.transform.position);
            if (distance < minDistance)
            {
                nearest = component;
                minDistance = distance;
            }
        }
        return nearest;
    }

    public static Transform[] FindAllNearby3D(this Transform self, float radius, int layerMask,
        System.Func<Transform, Transform, bool> condition = null)
    {
        Collider[] colliders = Physics.OverlapSphere(self.position, radius, layerMask);
        List<Transform> results = new List<Transform>();
        foreach (Collider collider in colliders)
        {
            if (collider.transform == self) continue;
            if (condition != null && !condition(collider.transform, self)) continue;
            results.Add(collider.transform);
        }
        return results.ToArray();
    }

    public static Transform[] FindAllNearby3DNonAlloc(this Transform self, float radius, Collider[] colliders, int layerMask,
        System.Func<Transform, Transform, bool> condition = null)
    {
        int count = Physics.OverlapSphereNonAlloc(self.position, radius, colliders, layerMask);
        List<Transform> results = new List<Transform>();
        for (int i = 0; i < count; ++i)
        {
            Transform target = colliders[i].transform;
            if (target == self) continue;
            if (condition != null && !condition(target, self)) continue;
            results.Add(target);
        }
        return results.ToArray();
    }

    public static T[] FindAllNearby3D<T>(this Transform self, float radius, int layerMask,
        System.Func<T, Transform, bool> condition = null)
    {
        Collider[] colliders = Physics.OverlapSphere(self.position, radius, layerMask);
        List<T> results = new List<T>();
        foreach (Collider collider in colliders)
        {
            if (collider.transform == self) continue;
            T component = collider.GetComponent<T>();
            if (condition != null && !condition(component, self)) continue;
            results.Add(component);
        }
        return results.ToArray();
    }

    public static T[] FindAllNearby3DNonAlloc<T>(this Transform self, float radius, Collider[] colliders, int layerMask,
        System.Func<T, Transform, bool> condition = null)
    {
        int count = Physics.OverlapSphereNonAlloc(self.position, radius, colliders, layerMask);
        List<T> results = new List<T>();
        for (int i = 0; i < count; ++i)
        {
            if (colliders[i].transform == self) continue;
            T component = colliders[i].GetComponent<T>();
            if (condition != null && !condition(component, self)) continue;
            results.Add(component);
        }
        return results.ToArray();
    }

    public static int ForEachAllNearby3D(this Transform self, float radius, Collider[] colliders, int layerMask,
        System.Action<Transform> action, System.Func<Transform, Transform, bool> condition = null)
    {
        int count = Physics.OverlapSphereNonAlloc(self.position, radius, colliders, layerMask);
        for (int i = 0; i < count; ++i)
        {
            if (colliders[i].transform == self) continue;
            Transform transform = colliders[i].transform;
            if (condition != null && !condition(transform, self)) continue;
            action.Invoke(transform);
        }
        return count;
    }

    public static int ForEachAllNearby3D<T>(this Transform self, float radius, Collider[] colliders, int layerMask,
        System.Action<T> action, System.Func<T, Transform, bool> condition = null)
    {
        int count = Physics.OverlapSphereNonAlloc(self.position, radius, colliders, layerMask);
        for (int i = 0; i < count; ++i)
        {
            if (colliders[i].transform == self) continue;
            T component = colliders[i].GetComponent<T>();
            if (condition != null && !condition(component, self)) continue;
            action.Invoke(component);
        }
        return count;
    }

    public static Transform RayCast2D(this Transform self, float maxDistance = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers)
    {
        return self.RayCast2D(self.right, maxDistance, layerMask);
    }

    public static Transform RayCast2D(this Transform self, Vector2 direction, float maxDistance = Mathf.Infinity, int layerMask = Physics2D.DefaultRaycastLayers)
    {
        RaycastHit2D hit = Physics2D.Raycast(self.position, direction, maxDistance, layerMask);
        return hit.transform;
    }

    public static T RayCast2D<T>(this Transform self, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers) where T : Component
    {
        Transform target = self.RayCast2D(maxDistance, layerMask);
        return (target != null) ? target.GetComponent<T>() : null;
    }

    public static T RayCast2D<T>(this Transform self, Vector2 direction, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers) where T : Component
    {
        Transform target = self.RayCast2D(direction, maxDistance, layerMask);
        return (target != null) ? target.GetComponent<T>() : null;
    }

    public static Transform RayCast3D(this Transform self, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers)
    {
        return self.RayCast3D(self.forward, maxDistance, layerMask);
    }

    public static Transform RayCast3D(this Transform self, Vector3 direction, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers)
    {
        RaycastHit hit;
        if (Physics.Raycast(self.position, direction, out hit, maxDistance, layerMask))
        {
            return hit.transform;
        }
        return null;
    }

    public static T RayCast3D<T>(this Transform self, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers) where T : Component
    {
        Transform target = self.RayCast3D(maxDistance, layerMask);
        return (target != null) ? target.GetComponent<T>() : null;
    }

    public static T RayCast3D<T>(this Transform self, Vector3 direction, float maxDistance = Mathf.Infinity, int layerMask = Physics.DefaultRaycastLayers) where T : Component
    {
        Transform target = self.RayCast3D(direction, maxDistance, layerMask);
        return (target != null) ? target.GetComponent<T>() : null;
    }

    public static bool HitTest(this Transform self)
    {
        return self.HitTest(Input.mousePosition);
    }

    public static bool HitTest(this Transform self, Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        RaycastHit hit;
        return (Physics.Raycast(ray, out hit) && hit.collider.gameObject == self.gameObject);
    }

    public static void Show(this Transform self)
    {
        foreach (Renderer renderer in self.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
    }

    public static void Hide(this Transform self)
    {
        foreach (Renderer renderer in self.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
    }

    public static void Blink(this Transform self)
    {
        foreach (Renderer renderer in self.GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = !renderer.enabled;
        }
    }
}

