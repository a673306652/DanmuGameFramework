using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class PerfectBezierShaper : MonoBehaviour
{
    public static PerfectBezierShaper instance;

    private void Awake()
    {
        instance = this;

#if UNITY_EDITOR
        if (Directory.Exists($"{Application.dataPath}/Resources/BezierShaper") == false)
        {
            Directory.CreateDirectory($"{Application.dataPath}/Resources/BezierShaper");
        }

        if (Directory.Exists($"{Application.dataPath}/Resources/BezierPreset") == false)
        {
            Directory.CreateDirectory($"{Application.dataPath}/Resources/BezierPreset");
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }

    public ShapeTask ShapeObj2D(BezierShaperPreset preset, GameObject bullet, GameObject explosion, Vector3 startPos,
        GameObject targetObj, int count, Transform parent,Vector3 offset)
    {
        var s = new ShapeTask();
        List<GameObject> bulletPool = new List<GameObject>();
        List<Vector3> startDir = new List<Vector3>();
        for (int i = 0; i < count; i++)
        {
            var bullets = Instantiate(bullet, parent);
            bullet.transform.localScale = Vector3.one;
            bullets.transform.position = startPos;
            bulletPool.Add(bullets);
            startDir.Add(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0));
            bullets.SetActive(false);
        }
        s.bulletPool = bulletPool;
        s.WithMainAction(() =>
        {
          s.RunningTask = this.StartCoroutine(s.ShapeRun(bulletPool, startDir, explosion, targetObj, preset,offset));

        },this);
        return s;
    }

    
    public BezierShaperPreset GetBSPreset(string url)
    {
        return Resources.Load<BezierShaperPreset>(url);
    }

    public GameObject GetBSResources(string url)
    {
        return Resources.Load<GameObject>(url);
    }

    public GameObject CreateObj(GameObject obj)
    {
        return Instantiate(obj);
    }

    public void DestroyObj(GameObject obj,float delay = -1)
    {
        if (delay<=0)
        {
            Destroy(obj);
            return;
        }
        Destroy(obj,delay);
    }
}

public struct BezierShaperResourcesUrl
{
    public const string StarBullet = "BezierShaper/Star";
    public const string StarExp = "BezierShaper/exp";
    public const string StarPreset = "BezierPreset/01";
}

public class ShapeTask
{
    public List<GameObject> bulletPool;
    public Coroutine RunningTask;
    private UnityAction<ShapeTask,GameObject> FirstAction;
    private UnityAction<ShapeTask> EndAction;
    private UnityAction<ShapeTask,GameObject> SingleAction;
    private UnityAction MainAction;
    private PerfectBezierShaper Shaper;
    private GameObject explosion;
    private List<Vector3> dir;
    private List<float> lifeTime;
    private List<float> SpeedAddtive;

    public ShapeTask WithFirstAction(UnityAction<ShapeTask,GameObject> oc)
    {
        FirstAction = oc;
        return this;
    }

    public ShapeTask WithEndAction(UnityAction<ShapeTask> oc)
    {
        EndAction = oc;
        return this;
    }

    public ShapeTask WithSingleAction(UnityAction<ShapeTask,GameObject> oc)
    {
        SingleAction = oc;
        return this;
    }

    public ShapeTask WithMainAction(UnityAction oc,PerfectBezierShaper shaper)
    {
        Shaper = shaper;
        MainAction = oc;
        return this;
    }

    public ShapeTask Play()
    {
        MainAction?.Invoke();
        return this;
    }

    public void StopAndDestroyAll(bool InvokeCompleteOC = false)
    {
        if (null!= RunningTask)
        {
            Shaper.StopCoroutine(RunningTask);
            RunningTask = null;
        }

        for (var i = 0; i < bulletPool.Count; i++)
        {
            Shaper.DestroyObj(bulletPool[i]);
        }
        
        if (InvokeCompleteOC)
        {
            EndAction?.Invoke(this);
        }
    }

    public void StopAndDetonateAll(bool InvokeCompleteOC = false)
    {
        if (null!= RunningTask)
        {
            Shaper.StopCoroutine(RunningTask);
            RunningTask = null;
        }

        for (var i = 0; i < bulletPool.Count; i++)
        {
            Detonate(bulletPool[i]);
        }

        if (InvokeCompleteOC)
        {
            EndAction?.Invoke(this);
        }
    }

    private void Detonate(GameObject bullet)
    {
        var i = bulletPool.IndexOf(bullet);
        var exp = Shaper.CreateObj(explosion);
        exp.transform.position = bullet.transform.position;
        exp.transform.SetParent(bullet.transform.parent);
        exp.transform.localScale = Vector3.one;
        Shaper.DestroyObj(exp, 2);
        bulletPool.Remove(bullet);
        lifeTime.Remove(lifeTime[i]);
        dir.Remove(dir[i]);
        Shaper.DestroyObj(bullet);
        if (null != SingleAction)
        {
            SingleAction.Invoke(this,bullet);
        }
    }
    public IEnumerator ShapeRun(List<GameObject> _bulletPool, List<Vector3> _dir, GameObject explosion, GameObject endPos, BezierShaperPreset preset,Vector3 offset)
         {
             bool firstComplete = false;
             SpeedAddtive = new List<float>();
             bulletPool = _bulletPool;
             dir = _dir;
             this.lifeTime = new List<float>();
             this.explosion = explosion;
             for (int i = 0; i < bulletPool.Count; i++)
             {
                 lifeTime.Add(0);
                 SpeedAddtive.Add(1);
                 bulletPool[i].SetActive(true);
             }
             while (bulletPool.Count > 0)
             {
                 for (int i = 0; i < bulletPool.Count; i++)
                 {
                     var bullet = bulletPool[i];
                     var startRatio = Mathf.Clamp(lifeTime[i] / preset.StartDuration, 0, 1);
                     var targetDir = -(bullet.transform.position - (endPos.transform.position+offset)).normalized;
                     bullet.transform.position +=
                         (preset.StartSpeed * (1 - startRatio) * dir[i] +
                          targetDir * preset.FlySpeed * SpeedAddtive[i] * startRatio) * Time.fixedDeltaTime;
                     SpeedAddtive[i] += 0.02f;
                     lifeTime[i] += Time.fixedDeltaTime;
                     if (Vector3.SqrMagnitude((endPos.transform.position+offset) - bullet.transform.position) < 1)
                     {
                         if (!firstComplete)
                         {
                             firstComplete = true;
                             FirstAction?.Invoke(this,bullet);
                         }
                         Detonate(bullet);
                     }
                 }
                 yield return new WaitForFixedUpdate();
             }
             if (null != EndAction)
             {
                 EndAction.Invoke(this);
             }
         }

}