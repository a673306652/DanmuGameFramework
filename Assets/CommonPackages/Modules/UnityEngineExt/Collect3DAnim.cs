using System.Linq;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Modules.Patterns;
using Modules.Utilities;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Collect3DAnim : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Collect3DPool _PoolType = Collect3DPool.Default;
    [SerializeField] private GameObject _ParticlePool;
    [ShowIf("_AttractionMode", Collect3DAttractionMode.Fixed)]
    [SerializeField] private Vector3 _EndpointPosition = Vector3.zero;
    [Header("References")]
    [ShowIf("_AttractionMode", Collect3DAttractionMode.Transform)]
    [SerializeField] private Transform _EndpointTransform;
    [Header("Ejection Properties")]
    [SerializeField] private Collect3DEjectionMode _EjectionMode = Collect3DEjectionMode.Circle;
    [Range(1, 100)]
    [SerializeField] private int _EjectionBulkNum = 5;
    [SerializeField] private float _EjectionSpeed = 0.1f;
    [SerializeField] private Vector3 _EjectionVectorScale = Vector3.one;
    [SerializeField] private float _EjectionTime = 1f;
    [Range(0f, 1f)]
    [SerializeField] private float _EjectionDamping = 0f;
    [SerializeField] private Vector3 _EjectionGravity = Vector3.zero;
    [ShowIf("_EjectionMode", Collect3DEjectionMode.Cone)]
    [SerializeField] private float _ConeHeight = 1f;
    [Header("Attraction Properties")]
    [SerializeField] private Collect3DAttractionMode _AttractionMode = Collect3DAttractionMode.Transform;
    [SerializeField] private float _AttractionSpeed = 1f;
    [SerializeField] private float _AttractionTime = 1f;
    [SerializeField] private AnimationCurve _AttractionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    [Header("Random Properties")]
    [SerializeField] private bool _RandomRotation = false;
    [ShowIf("_RandomRotation")]
    [SerializeField] private float _RotationSpeed = 10f;

    [ShowIf("_RandomRotation")]
    [SerializeField] private Vector3 _MaxRandomRotation = Vector3.one * 180f;
    [ShowIf("_RandomRotation")]
    [SerializeField] private Vector3 _MinRandomRotation = Vector3.one * -180f;
    [SerializeField] private bool _RandomPosition = false;
    [ShowIf("_RandomPosition")]
    [SerializeField] private Vector3 _MaxRandomOffset = Vector3.one * 1f;
    [ShowIf("_RandomPosition")]
    [SerializeField] private Vector3 _MinRandomOffset = Vector3.one * -1f;

    private Dictionary<int, GameObject> particlesDict = new Dictionary<int, GameObject>();
    private Dictionary<int, ParticleLifetimeData> particlesData = new Dictionary<int, ParticleLifetimeData>();
    private UnityEvent onItemCollected = new UnityEvent();

    public float TotalTime
    {
        get
        {
            return _AttractionTime + _EjectionTime;
        }
    }

    public bool IsPlaying
    {
        get
        {
            return particlesDict.Count > 0;
        }
    }

    public void Play(GameObject pickable)
    {
        if (_EjectionMode.Equals(Collect3DEjectionMode.PickUp))
        {
            GeneratePickUp(pickable);
        }
    }

    [Button("Play", enabledMode: EButtonEnableMode.Playmode)]
    public void Play()
    {
        if (_EjectionMode.Equals(Collect3DEjectionMode.Circle))
        {
            GenerateCircleParticles();
            return;
        }
        if (_EjectionMode.Equals(Collect3DEjectionMode.Cone))
        {
            GenerateConeParticles();
            return;
        }
    }

    public Collect3DAnim OnItemAdded(UnityAction onAdded)
    {
        onItemCollected.RemoveAllListeners();
        onItemCollected.AddListener(onAdded);
        return this;
    }

    public void PlayAt(Vector3 position, int num = -1)
    {
        _EndpointPosition = position;
        if (num > 0)
        {
            _EjectionBulkNum = num;
        }
        Play();
    }

    public void PlayAt(Transform trans, int num = -1)
    {
        _EndpointTransform = trans;
        if (num > 0)
        {
            _EjectionBulkNum = num;
        }
        Play();
    }

    [Button("Clear", enabledMode: EButtonEnableMode.Editor)]
    public void Clear()
    {
        var ids = particlesDict.Keys.ToArray();
        foreach (var id in ids)
        {
            GetPool().ReturnParticle(particlesDict[id]);
            particlesData.Remove(id);
            particlesDict.Remove(id);
        }
    }

    [Button("Preview", enabledMode: EButtonEnableMode.Editor)]
    public void Preview()
    {
        Clear();
        if (_EjectionMode.Equals(Collect3DEjectionMode.Circle))
        {
            GenerateCircleParticles();
        }
        else
        {
            GenerateConeParticles();
        }
    }

    private IParticlePool<Collect3DAnimParticle> GetPool()
    {
        return _ParticlePool.GetComponent<IParticlePool<Collect3DAnimParticle>>();
    }

    private GameObject GetRawParticle()
    {
        var go = GetPool().GetParticle();
        go.transform.position = transform.position;
        go.transform.rotation = Quaternion.identity;
        return go;
    }

    private void GenerateCircleParticles()
    {
        var angleUnit = 360f / _EjectionBulkNum;
        for (var i = 0; i < _EjectionBulkNum; i++)
        {
            var go = GetRawParticle();
            var pData = new ParticleLifetimeData()
            {
                AngleOffset = angleUnit * i
            };
            pData.SpeedVector = Vector3.Scale((Quaternion.AngleAxis(pData.AngleOffset, transform.up) * transform.forward), _EjectionVectorScale);
            if (_RandomRotation)
            {
                pData.Rotation = RandomUtils.Range(_MinRandomRotation, _MaxRandomRotation);
            }
            if (_RandomPosition)
            {
                pData.SpeedVector += RandomUtils.Range(_MinRandomOffset, _MaxRandomOffset);
            }
            particlesDict.Add(go.GetInstanceID(), go);
            particlesData.Add(go.GetInstanceID(), pData);
        }
    }

    private void GenerateConeParticles()
    {
        var angleUnit = 360f / _EjectionBulkNum;
        for (var i = 0; i < _EjectionBulkNum; i++)
        {
            var go = GetRawParticle();
            var pData = new ParticleLifetimeData()
            {
                AngleOffset = angleUnit * i
            };
            pData.SpeedVector = Vector3.Scale((Quaternion.AngleAxis(pData.AngleOffset, transform.up) * transform.forward), _EjectionVectorScale) + transform.up * _ConeHeight;
            if (_RandomRotation)
            {
                pData.Rotation = RandomUtils.Range(_MinRandomRotation, _MaxRandomRotation);
            }
            if (_RandomPosition)
            {
                pData.SpeedVector += RandomUtils.Range(_MinRandomOffset, _MaxRandomOffset);
            }
            particlesDict.Add(go.GetInstanceID(), go);
            particlesData.Add(go.GetInstanceID(), pData);
        }
    }

    private void GeneratePickUp(GameObject pickable)
    {
        var go = pickable;
        var pData = new ParticleLifetimeData()
        {
            AngleOffset = 0
        };
        var direction = (transform.position - pickable.transform.position).normalized;
        pData.SpeedVector = Vector3.Scale(-direction, _EjectionVectorScale);
        if (_RandomRotation)
        {
            pData.Rotation = RandomUtils.Range(_MinRandomRotation, _MaxRandomRotation);
        }
        if (!particlesDict.ContainsKey(go.GetInstanceID()))
        {
            particlesDict.Add(go.GetInstanceID(), go);
            particlesData.Add(go.GetInstanceID(), pData);
        }
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        if (particlesData.Count > 0)
        {
            var instanceIds = particlesDict.Keys;
            foreach (var iid in instanceIds)
            {
                var data = particlesData[iid];
                Gizmos.color = Color.blue;
                var startPos = particlesDict[iid].transform.position;
                var endPos = particlesDict[iid].transform.position + data.SpeedVector * _EjectionSpeed * _EjectionTime / 0.04f;
                Handles.DrawBezier(startPos, endPos, startPos, endPos, Color.blue, null, 5f);
                Gizmos.DrawSphere(endPos, 0.1f);
            }
        }
    }
#endif

    void FixedUpdate()
    {
        if (particlesDict.Count > 0)
        {
            var instanceIds = particlesDict.Keys;
            var iid = 0;
            for (var i = 0; i < instanceIds.Count; i++)
            {
                iid = instanceIds.ElementAt(i);
                var data = particlesData[iid];
                if (data.IsAttraction)
                {
                    data.AttractionLT += Time.deltaTime;
                    var transA = particlesDict[iid].transform;
                    var transB = _EndpointTransform == null ? transform : _EndpointTransform;
                    var pointB = _AttractionMode.Equals(Collect3DAttractionMode.Fixed) ? _EndpointPosition : transB.position;
                    var scaledSpeed = _AttractionCurve.Evaluate(data.AttractionLT / _AttractionTime) * _AttractionSpeed;
                    particlesDict[iid].transform.position = Vector3.MoveTowards(transA.position, pointB, scaledSpeed * Time.deltaTime);

                    if (_RandomRotation)
                    {
                        particlesDict[iid].transform.localRotation = Quaternion.RotateTowards(particlesDict[iid].transform.localRotation, Quaternion.Euler(particlesData[iid].Rotation), _RotationSpeed * Time.deltaTime);
                    }

                    if (Vector3.Distance(transA.position, pointB) < 0.01f)
                    {
                        var go = particlesDict[iid];
                        particlesDict.Remove(iid);
                        particlesData.Remove(iid);
                        GetPool().ReturnParticle(go);
                        onItemCollected?.Invoke();
                        return;
                    }
                }
                else
                {
                    data.EjectionLT += Time.deltaTime;
                    data.DampingProgress = Mathf.Clamp01(data.EjectionLT / _EjectionTime);
                    var speedDelta = particlesData[iid].SpeedVector * _EjectionSpeed * Time.deltaTime;
                    var dampingMulti = (1f - data.DampingProgress * _EjectionDamping);
                    var gravityMulti = _EjectionGravity * Time.deltaTime;
                    particlesDict[iid].transform.Translate(speedDelta * dampingMulti + gravityMulti, Space.World);

                    if (_RandomRotation)
                    {
                        particlesDict[iid].transform.localRotation = Quaternion.RotateTowards(particlesDict[iid].transform.localRotation, Quaternion.Euler(particlesData[iid].Rotation), _RotationSpeed * Time.deltaTime);
                    }

                    if (data.EjectionLT >= _EjectionTime)
                    {
                        data.IsAttraction = true;
                    }
                }
                particlesData[iid] = data;
            }
        }
    }

    public struct ParticleLifetimeData
    {
        public float EjectionLT;
        public float AttractionLT;
        public float AngleOffset;
        public float DampingProgress;
        public Vector3 SpeedVector;
        public Vector3 Rotation;
        public bool IsAttraction;
    }
}

public enum Collect3DEjectionMode
{
    Circle,
    Cone,
    PickUp,
}
public enum Collect3DAttractionMode
{
    Fixed,
    Transform
}

public enum Collect3DPool
{
    Default,
    Custom
}