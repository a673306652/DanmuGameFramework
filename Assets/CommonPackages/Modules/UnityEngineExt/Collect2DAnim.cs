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

public class Collect2DAnim : ObjectPool<Collect2DAnimParticle>
{
    [Header("References")]
    [ShowIf("_AttractionMotion", Collect2DMotionMode.Fixed)]
    [SerializeField]
    private Vector2 _EndpointPosition = Vector2.zero;

    [Header("References")]
    [ShowIf("_AttractionMotion", Collect2DMotionMode.Transform)]
    [SerializeField]
    private Transform _EndpointTransform;

    [ShowIf("_EjectionMotion", Collect2DMotionMode.Transform)]
    [SerializeField]
    private Transform _EjectionTransform;

    [ShowIf("_EjectionMotion", Collect2DMotionMode.Fixed)]
    [SerializeField]
    private Vector2 _EjectionPosition;

    [Header("Data Properties")]
    [SerializeField]
    private Collect2DUnitType _UnitType = Collect2DUnitType.Int;

    [SerializeField]
    private Collect2DQuickUnitSplit
        _UnitSplit = Collect2DQuickUnitSplit.LastOneTakesAll;

    [Header("Ejection Properties")]
    [SerializeField]
    private Collect2DMotionMode _EjectionMotion = Collect2DMotionMode.Transform;

    [SerializeField]
    private bool _Prewarming;

    [Range(1, 100)]
    [SerializeField]
    private int _EjectionBulkNum = 5;

    [SerializeField]
    private float _EjectionSpeed = 1f;

    [SerializeField]
    private Vector2 _EjectionVectorScale = Vector2.one;

    [SerializeField]
    private AnimationCurve
        _EjectionSizeScale = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [SerializeField]
    private float _EjectionDistance;

    [SerializeField]
    private float _EjectionTime = 1f;

    [Range(0f, 1f)]
    [SerializeField]
    private float _EjectionDamping = 0f;

    [SerializeField]
    private Vector2 _EjectionGravity = Vector2.zero;

    [Header("Attraction Properties")]
    [SerializeField]
    private Collect2DMotionMode
        _AttractionMotion = Collect2DMotionMode.Transform;

    [SerializeField]
    private float _AttractionSpeed = 1f;

    [SerializeField]
    private float _AttractionTime = 1f;

    [SerializeField]
    private float _AttractionDelay = 0f;

    [SerializeField]
    private AnimationCurve _AttactionDelayCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 0f);

    [SerializeField]
    private AnimationCurve
        _AttractionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [SerializeField]
    private AnimationCurve
        _AttractionSizeScale = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [SerializeField]
    private Vector2 _OrbitOffset = new Vector2(0.5f, 0.5f);

    [SerializeField]
    private float _OrbitOffsetPeakTime = 0.5f;

    [Header("Random Properties")]
    [SerializeField]
    private bool _RandomRotation = false;

    [ShowIf("_RandomRotation")]
    [SerializeField]
    private float _RotationSpeed = 10f;

    [ShowIf("_RandomRotation")]
    [SerializeField]
    private Vector3 _MaxRandomRotation = Vector3.one * 180f;

    [ShowIf("_RandomRotation")]
    [SerializeField]
    private Vector3 _MinRandomRotation = Vector3.one * -180f;

    [SerializeField]
    private bool _RandomPosition = false;

    [ShowIf("_RandomPosition")]
    [SerializeField]
    private Vector2 _MaxRandomOffset = Vector3.one * 1f;

    [ShowIf("_RandomPosition")]
    [SerializeField]
    private Vector2 _MinRandomOffset = Vector3.one * -1f;

    [SerializeField]
    private bool _RandomSize;

    [ShowIf("_RandomSize")]
    [SerializeField]
    private float _MaxRandomSize = 1f;

    [ShowIf("_RandomSize")]
    [SerializeField]
    private float _MinRandomSize = 1f;

    private Dictionary<int, GameObject>
        particlesDict = new Dictionary<int, GameObject>();

    private Dictionary<int, ParticleLifetimeData>
        particlesData = new Dictionary<int, ParticleLifetimeData>();

    private UnityEvent<object> onItemCollected = new UnityEvent<object>();

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

    void Start()
    {
        if (_Prewarming)
        {
            for (var i = 0; i < _EjectionBulkNum; i++)
            {
                var go = GetOne();
                ReturnOne(go);
            }
        }
    }

    [Button("Play", enabledMode: EButtonEnableMode.Playmode)]
    public void Play()
    {
        StartCoroutine(PlayAnimation(GenerateCircleParticles()));
    }

    public void PlayRuntime(object payload)
    {
        StartCoroutine(PlayAnimation(GenerateCircleParticles(payload)));
    }

    public Collect2DAnim OnItemAdded(UnityAction<object> onAdded)
    {
        onItemCollected.RemoveAllListeners();
        onItemCollected.AddListener(onAdded);
        return this;
    }

    public Collect2DAnim SetEndpoint(Vector2 position)
    {
        _EndpointPosition = position;
        return this;
    }

    public Collect2DAnim SetEndpoint(Transform trans)
    {
        _EndpointTransform = trans;
        return this;
    }

    public Collect2DAnim SetStartpoint(Vector2 position)
    {
        switch (_EjectionMotion)
        {
            case Collect2DMotionMode.Fixed:
                _EjectionPosition = position;
                break;
            case Collect2DMotionMode.Transform:
                _EjectionTransform.position = position;
                break;
        }
        return this;
    }

    public void CollectItemsAt(Vector2 position, int num = -1)
    {
        CollectItemsAt(null, position, num);
    }

    public void CollectItemsAt(object payload, Vector2 position, int num = -1)
    {
        SetStartpoint(position);
        if (num > 0)
        {
            _EjectionBulkNum = num;
        }
        PlayRuntime(payload);
    }

    public void CollectItemsAt(object payload, int num = -1)
    {
        if (num > 0)
        {
            _EjectionBulkNum = num;
        }
        PlayRuntime(payload);
    }

    [Button("Clear", enabledMode: EButtonEnableMode.Playmode)]
    public void Clear()
    {
        var ids = particlesDict.Keys.ToList();
        foreach (var id in ids)
        {
            ReturnOne(id);
            particlesData.Remove(id);
            particlesDict.Remove(id);
        }
    }

    [Button("Preview", enabledMode: EButtonEnableMode.Editor)]
    public void Preview()
    {
        var ids = particlesDict.Keys.ToList();
        foreach (var id in ids)
        {
            DestroyImmediate(particlesDict[id]);
            particlesData.Remove(id);
            particlesDict.Remove(id);
        }
        GenerateCircleParticles();
    }

    private GameObject GetRawParticle()
    {
        var go = GetOne();
        go.transform.position = transform.position;
        go.transform.rotation = Quaternion.identity;
        return go;
    }

    private object GetPayload(ParticleLifetimeData pData)
    {
        switch (_UnitType)
        {
            case Collect2DUnitType.Int:
                return pData.PayloadInt;
            case Collect2DUnitType.Float:
                return pData.PayloadFloat;
            case Collect2DUnitType.BigNumber:
                return pData.PayloadBN;
        }
        return null;
    }

    private void AssignData(object payload, List<int> particleList)
    {
        if (_UnitType.Equals(Collect2DUnitType.Int))
        {
            var total = (int)payload;
            var remaining = total;

            for (var i = 0; i < particleList.Count; i++)
            {
                var pData = particlesData[particleList[i]];

                if (
                    _UnitSplit.Equals(Collect2DQuickUnitSplit.AlwaysAverage) ||
                    i < particleList.Count - 1
                )
                {
                    pData.PayloadInt = total / _EjectionBulkNum;
                    remaining -= pData.PayloadInt;
                }
                else
                {
                    pData.PayloadInt = remaining;
                }
                particlesData[particleList[i]] = pData;
            }
        }
        else if (_UnitType.Equals(Collect2DUnitType.Float))
        {
            var total = (float)payload;
            var remaining = total;
            for (var i = 0; i < particleList.Count; i++)
            {
                var pData = particlesData[particleList[i]];

                if (
                    _UnitSplit.Equals(Collect2DQuickUnitSplit.AlwaysAverage) ||
                    i < particleList.Count - 1
                )
                {
                    pData.PayloadFloat = total / _EjectionBulkNum;
                    remaining -= pData.PayloadFloat;
                }
                else
                {
                    pData.PayloadFloat = remaining;
                }
                particlesData[particleList[i]] = pData;
            }
        }
        else
        {
            var total = (BigNumber)payload;
            var remaining = total.Clone();
            for (var i = 0; i < particleList.Count; i++)
            {
                var pData = particlesData[particleList[i]];

                if (
                    _UnitSplit.Equals(Collect2DQuickUnitSplit.AlwaysAverage) ||
                    i < particleList.Count - 1
                )
                {
                    pData.PayloadBN = total / _EjectionBulkNum;
                    remaining -= pData.PayloadBN;
                }
                else
                {
                    pData.PayloadBN = remaining;
                }
                particlesData[particleList[i]] = pData;
            }
        }
    }

    private List<int> GenerateCircleParticles(object payload = null)
    {
        var angleUnit = 360f / _EjectionBulkNum;
        var particleList = new List<int>();

        for (var i = 0; i < _EjectionBulkNum; i++)
        {
            var go = GetRawParticle();
            var pData =
                new ParticleLifetimeData()
                {
                    AngleOffset = angleUnit * i,
                    BirthPosition =
                        _EjectionMotion.Equals(Collect2DMotionMode.Transform)
                            ? (
                            _EjectionTransform == null
                                ? (Vector2)transform.position
                                : (Vector2)_EjectionTransform.position
                            )
                            : _EjectionPosition
                };

            pData.SpeedVector =
                Vector2
                    .Scale((
                    Quaternion.AngleAxis(pData.AngleOffset, transform.forward) *
                    transform.right
                    ),
                    _EjectionVectorScale);
            pData.AttractionDelay = _AttactionDelayCurve.Evaluate((float)i / (float)_EjectionBulkNum) * _AttractionDelay;
            pData.AttractionDelayLT = -pData.AttractionDelay;
            pData.SizeScale = 1f;

            if (_RandomSize)
            {
                pData.SizeScale = RandomUtils.Range(_MinRandomSize, _MaxRandomSize);
            }

            if (_RandomRotation)
            {
                pData.Rotation =
                    RandomUtils.Range(_MinRandomRotation, _MaxRandomRotation);
            }
            if (_RandomPosition)
            {
                pData.SpeedVector +=
                    RandomUtils.Range(_MinRandomOffset, _MaxRandomOffset);
            }
            go.transform.position = pData.BirthPosition;
            particlesDict.Add(go.GetInstanceID(), go);
            particlesData.Add(go.GetInstanceID(), pData);
            particleList.Add(go.GetInstanceID());
        }

        if (payload != null)
        {
            AssignData(payload, particleList);
        }
        return particleList;
    }

    private IEnumerator PlayAnimation(List<int> particles)
    {
        if (particles.Count > 0)
        {
            var ejectionIds = new List<int>(particles);
            float t = 0;

            while (t < _EjectionTime)
            {
                t += Time.deltaTime;
                var frameCopy = new List<int>(ejectionIds);

                for (var i = 0; i < frameCopy.Count; i++)
                {
                    var iid = frameCopy[i];
                    var data = particlesData[iid];

                    if (!data.IsAttraction)
                    {
                        data.EjectionLT += Time.deltaTime;
                        data.DampingProgress =
                            Mathf.Clamp01(data.EjectionLT / _EjectionTime);
                        var speedDelta =
                            particlesData[iid].SpeedVector *
                            _EjectionSpeed *
                            Time.deltaTime;
                        var dampingMulti =
                            (1f - data.DampingProgress * _EjectionDamping);
                        var gravityMulti = _EjectionGravity * Time.deltaTime;
                        particlesDict[iid]
                            .transform
                            .Translate(speedDelta * dampingMulti + gravityMulti,
                            Space.World);
                        var ejectionSizeScale =
                            _EjectionSizeScale
                                .Evaluate(data.EjectionLT / _EjectionTime);
                        particlesDict[iid].transform.localScale = Vector3.one * ejectionSizeScale * data.SizeScale;

                        if (_RandomRotation)
                        {
                            particlesDict[iid].transform.localRotation =
                                Quaternion
                                    .RotateTowards(particlesDict[iid]
                                        .transform
                                        .localRotation,
                                    Quaternion
                                        .Euler(particlesData[iid].Rotation),
                                    _RotationSpeed * Time.deltaTime);
                        }

                        if (
                            Vector2
                                .Distance(data.BirthPosition,
                                particlesDict[iid].transform.position) >
                            _EjectionDistance
                            || (data.EjectionLT >= _EjectionTime)
                        )
                        {
                            var endPos = (_EndpointTransform == null
                                                        ? transform
                                                        : _EndpointTransform).position;
                            data.IsAttraction = true;
                            data.AttractionPosition = (Vector2)particlesDict[iid].transform.position;
                            data.OrbitOffsetPosition = data.AttractionPosition + (Vector2)(endPos - particlesDict[iid].transform.position) * _OrbitOffset;
                            ejectionIds.Remove(iid);
                        }

                        particlesData[iid] = data;
                    }
                }

                yield return new WaitForFixedUpdate();
            }

            t = 0f;
            var attractionIds = particles;

            while (t < _AttractionTime)
            {
                t += Time.deltaTime;
                var frameCopy = new List<int>(attractionIds);

                for (var i = 0; i < frameCopy.Count; i++)
                {
                    var iid = frameCopy[i];
                    var data = particlesData[iid];

                    if (data.AttractionDelayLT < 0f)
                    {
                        data.AttractionDelayLT += Time.deltaTime;
                    }
                    else
                    {
                        data.AttractionLT += Time.deltaTime;
                    }
                    var attractionTimeWODelay = _AttractionTime - data.AttractionDelay;
                    var transA = particlesDict[iid].transform;
                    var transB =
                        _EndpointTransform == null
                            ? transform
                            : _EndpointTransform;
                    var pointB =
                        _AttractionMotion.Equals(Collect2DMotionMode.Fixed)
                            ? _EndpointPosition
                            : new Vector2(transB.position.x, transB.position.y);
                    var pointC = data.OrbitOffsetPosition;
                    var scaledSpeed =
                        _AttractionCurve
                            .Evaluate(data.AttractionLT / attractionTimeWODelay) *
                        _AttractionSpeed;
                    var scaledSize =
                        _AttractionSizeScale
                            .Evaluate(Mathf.Abs(data.AttractionDelayLT) / data.AttractionDelay);

                    particlesDict[iid].transform.localScale = scaledSize * Vector3.one * data.SizeScale;

                    if (data.AttractionDelayLT >= 0f)
                    {
                        var bezierPoint = Modules.Physics.UnityPhysicsExt.GetBezierPoint(data.AttractionLT / attractionTimeWODelay, data.AttractionPosition, data.OrbitOffsetPosition, pointB);
                        particlesDict[iid].transform.position =
                            Vector3
                                .MoveTowards(transA.position, bezierPoint,
                                scaledSpeed * Time.deltaTime);
                    }

                    if (_RandomRotation)
                    {
                        particlesDict[iid].transform.localRotation =
                            Quaternion
                                .RotateTowards(particlesDict[iid]
                                    .transform
                                    .localRotation,
                                Quaternion.Euler(particlesData[iid].Rotation),
                                _RotationSpeed * Time.deltaTime);
                    }

                    if (Vector2.Distance(transA.position, pointB) < 0.01f)
                    {
                        var go = particlesDict[iid];
                        var payload = GetPayload(data);
                        particlesDict.Remove(iid);
                        particlesData.Remove(iid);
                        attractionIds.Remove(iid);
                        ReturnOne(go);
                        onItemCollected?.Invoke(payload);
                    }
                    else if (t >= _AttractionTime)
                    {
                        var go = particlesDict[iid];
                        var payload = GetPayload(data);
                        particlesDict.Remove(iid);
                        particlesData.Remove(iid);
                        attractionIds.Remove(iid);
                        ReturnOne(go);
                        onItemCollected?.Invoke(payload);
                    }
                    else
                    {
                        particlesData[iid] = data;
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }
        yield return null;
    }

    public struct ParticleLifetimeData
    {
        public float EjectionLT;
        public float AttractionLT;
        public float AttractionDelayLT;
        public float AngleOffset;
        public float DampingProgress;
        public float AttractionDelay;
        public float SizeScale;
        public Vector2 BirthPosition;
        public Vector2 AttractionPosition;
        public Vector2 OrbitOffsetPosition;
        public Vector2 SpeedVector;
        public Vector3 Rotation;
        public bool IsAttraction;
        public int PayloadInt;
        public float PayloadFloat;
        public BigNumber PayloadBN;
    }
}

public enum Collect2DMotionMode
{
    Fixed,
    Transform
}

public enum Collect2DUnitType
{
    BigNumber,
    Int,
    Float
}

public enum Collect2DQuickUnitSplit
{
    AlwaysAverage,
    LastOneTakesAll
}
