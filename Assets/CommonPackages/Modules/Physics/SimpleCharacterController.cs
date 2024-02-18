using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
using EditorExtention;
#endif


namespace Modules.Physics
{
    [RequireComponent(typeof (CapsuleCollider))]
    public class SimpleCharacterController : MonoBehaviour, IVelocityObject
    {
        [Header("Physics")]
        [SerializeField]
        protected ControlType _ControlType = ControlType.Rigidbody;

        [SerializeField]
        protected CollisionResolver
            _CollisionResolver = CollisionResolver.HorizontalAvailable;

        [ShowIf("_ControlType", ControlType.Rigidbody)]
        [SerializeField]
        protected Rigidbody _PlayerBody;

        [SerializeField]
        protected Transform _RotTransform;

        [ShowIf("_ControlType", ControlType.Transform)]
        [SerializeField]
        protected Transform _PosTransform;

        [SerializeField]
        protected LayerMask _ObstacleLayers;

        [SerializeField]
        protected LayerMask _GroundLayers;

        [SerializeField]
        protected bool _WalkOnGround;

        [SerializeField]
        protected bool _NoCollider;

        [SerializeField]
        protected bool _HasJoyStick;

        [SerializeField]
        protected bool _FreezeHorizontal;

        [SerializeField]
        protected bool _RotateTowardsVelocity = true;

        [ShowIf("_HasJoyStick")]
        [SerializeField]
        protected VariableJoystick _JoyStick;

        [SerializeField]
        protected bool _HasAnimator;

        [ShowIf("_HasAnimator")]
        [SerializeField]
        protected Animator _Animator;

        [Space]
        [Header("Properties")]
        [SerializeField]
        protected float _MoveSpeed = 5f;

        [SerializeField]
        protected float _RotationSpeed = 360f;

        [SerializeField]
        protected float _SlopeStepOffset = 0.0001f;

        [SerializeField]
        protected float _MaxGroundOffset = 0.5f;

        [SerializeField]
        protected float _MaxSlopeAngle = 50f;

        [MaxValue(0f)]
        [SerializeField]
        protected float _Gravity = 0f;

        [MinValue(0f)]
        [SerializeField]
        protected float _Resistence = 1f;

        [Range(0f, 100f)]
        [SerializeField]
        protected float _CollisionCheckRangeFar = 2f;

        [Range(0f, 100f)]
        [SerializeField]
        protected float _CollisionCheckRangeNear = 0.05f;

        [Range(0f, 100f)]
        [SerializeField]
        protected float _CollisionCheckRangeRot = 0.5f;

        [Range(0f, 360f)]
        [SerializeField]
        protected float _RotationalCollisionAngle = 360f;

        [Range(0, 36f)]
        [SerializeField]
        protected int _RotationalCollisionNum = 6;

        [SerializeField]
        protected int _MaxCollisionFrame = 30;

        public UnityEvent OnMoveStart = new UnityEvent();

        public UnityEvent OnMoveStop = new UnityEvent();

        public UnityEvent<int, int>
            OnGroundLayerChange = new UnityEvent<int, int>(); // (prevLayer, currLayer)

        private Vector2 axisInput = Vector2.zero;

        private Vector3 velocity = Vector3.zero;

        private Vector3 gravityVelocity = Vector3.zero;

        private Transform cameraTransform;

        private Vector3 lastSafePosition;

        private int collisionFrame;

        protected CapsuleCollider collider;

        public int AnchoredLayer { get; private set; }

        protected virtual void Awake()
        {
            collider = GetComponent<CapsuleCollider>();
            collider.enabled = !_NoCollider;
        }

        protected virtual void Start()
        {
            cameraTransform = Camera.main.transform;
            lastSafePosition = GetTransform().position;
        }

        protected virtual void FixedUpdate()
        {
            UpdateMovement(GetMovementDirection(), Time.fixedDeltaTime);
        }

        public Vector3 Velocity
        {
            get
            {
                return velocity;
            }
        }

        public VariableJoystick JoyStick
        {
            set
            {
                _JoyStick = value;
            }
            get
            {
                return _JoyStick;
            }
        }

        public Animator Animator
        {
            get
            {
                return _Animator;
            }
            set
            {
                _Animator = value;
            }
        }

        public float MoveSpeed
        {
            get
            {
                return _MoveSpeed;
            }
            set
            {
                _MoveSpeed = value;
            }
        }

        public CollisionResolver Resolver
        {
            get
            {
                return _CollisionResolver;
            }
            set
            {
                _CollisionResolver = value;
            }
        }

        public bool RotateTowardsVelocity
        {
            get
            {
                return _RotateTowardsVelocity;
            }
            set
            {
                _RotateTowardsVelocity = value;
            }
        }

        public Transform Transform
        {
            get
            {
                return GetTransform();
            }
        }

        public Transform RotTransform
        {
            get
            {
                return GetRotTransform();
            }
        }

        public bool FreezeHorizontal
        {
            get
            {
                return _FreezeHorizontal;
            }
            set
            {
                _FreezeHorizontal = value;
            }
        }

        public CapsuleCollider Collider
        {
            get
            {
                return collider;
            }
        }

        public Vector3 ColliderCenter
        {
            get
            {
                return transform.TransformPoint(collider.center);
            }
        }

        private void StickToGround(Vector3 moveDirection, float deltaTime)
        {
            // 仅用于非物理重力模式
            if (
                _ControlType.Equals(ControlType.Transform) ||
                !_PlayerBody.useGravity
            )
            {
                var playerTransform = GetTransform();
                Ray downwardRay =
                    new Ray(playerTransform.position, Vector3.down);
                var (inCol, downwardInCol, upwardInCol) =
                    CheckGroundColliders();

                if (inCol || (!inCol && upwardInCol))
                {
                    // Resistance
                    gravityVelocity += Vector3.up * deltaTime * _Resistence;
                    ApplyGravity (deltaTime);
                }
                else if (!downwardInCol)
                {
                    // Clear resistance
                    if (gravityVelocity.y > 0)
                    {
                        gravityVelocity = Vector3.zero;
                    }

                    // Apply gravity if there is any ground underneath
                    if (
                        UnityEngine
                            .Physics
                            .Raycast(downwardRay,
                            out RaycastHit hitInfo,
                            Mathf.Infinity,
                            _GroundLayers)
                    )
                    {
                        gravityVelocity += Vector3.up * deltaTime * _Gravity;
                        ApplyGravity (deltaTime);
                    }
                }
                else
                {
                    gravityVelocity = Vector3.zero;
                    // 合理接触 !inCol && nxtInCol
                }
            }
        }

        /// <summary>
        /// Check collision with ground
        /// </summary>
        /// <returns>(inCollision, downwardInCollision, upwardInCollision)</returns>
        private (bool, bool, bool) CheckGroundColliders()
        {
            var direction = new Vector3 { [collider.direction] = 1 };

            var offset = collider.height / 2 - collider.radius;
            var localPoint0 = collider.center - direction * offset;
            var localPoint1 = collider.center + direction * offset;
            var start = transform.TransformPoint(localPoint0);
            var end = transform.TransformPoint(localPoint1);

            var r =
                transform
                    .TransformVector(collider.radius,
                    collider.radius,
                    collider.radius);
            var radius =
                Enumerable
                    .Range(0, 3)
                    .Select(xyz => xyz == collider.direction ? 0 : r[xyz])
                    .Select(Mathf.Abs)
                    .Max();
            var downwardOffset = Vector3.down * _SlopeStepOffset;
            var upwardOffset = Vector3.up * _SlopeStepOffset;
            var currGrounds =
                UnityEngine
                    .Physics
                    .OverlapCapsule(start + downwardOffset,
                    end + downwardOffset,
                    radius,
                    _GroundLayers);
            if (currGrounds.Length > 0)
            {
                var prevLayer = AnchoredLayer;
                AnchoredLayer = currGrounds[0].gameObject.layer;
                if (prevLayer != AnchoredLayer)
                {
                    OnGroundLayerChange?.Invoke(prevLayer, AnchoredLayer);
                }
            }

            return (
                UnityEngine
                    .Physics
                    .CheckCapsule(start, end, radius, _GroundLayers),
                UnityEngine
                    .Physics
                    .CheckCapsule(start + downwardOffset,
                    end + downwardOffset,
                    radius,
                    _GroundLayers),
                UnityEngine
                    .Physics
                    .CheckCapsule(start + upwardOffset,
                    end + upwardOffset,
                    radius,
                    _GroundLayers)
            );
        }

        /// <summary>
        /// Check forward and rotational obstacles
        /// </summary>
        /// <returns>(nearBlocked, farBlocked, rotationalBlocked, BlockingDetectionResult[])</returns>
        private (bool, bool, bool, BlockingDetectionResult[])
        CheckObstacles(Vector3 velocity, float deltaTime)
        {
            var direction = new Vector3 { [collider.direction] = 1 };
            var deltaPos = velocity * deltaTime;
            var offset = collider.height / 2 - collider.radius;
            var localPoint0 = collider.center - direction * offset;
            var localPoint1 = collider.center + direction * offset;
            var start = transform.TransformPoint(localPoint0);
            var end = transform.TransformPoint(localPoint1);
            var axisDelta = end - start;
            var blockingResults =
                new BlockingDetectionResult[_RotationalCollisionNum + 1];

            var r =
                transform
                    .TransformVector(collider.radius,
                    collider.radius,
                    collider.radius);
            var radius =
                Enumerable
                    .Range(0, 3)
                    .Select(xyz => xyz == collider.direction ? 0 : r[xyz])
                    .Select(Mathf.Abs)
                    .Max();

            var rotation = GetRotTransform().rotation;
            var finalOffsetNear = deltaPos * _CollisionCheckRangeNear;
            var finalOffsetFar = deltaPos * _CollisionCheckRangeFar;
            var finalOffsetRot = deltaPos * _CollisionCheckRangeRot;
            var hasNearObs =
                UnityEngine
                    .Physics
                    .CheckCapsule(start + finalOffsetNear,
                    end + finalOffsetNear,
                    radius,
                    _ObstacleLayers);
            var hasFarObs =
                UnityEngine
                    .Physics
                    .CheckCapsule(start + finalOffsetFar,
                    end + finalOffsetFar,
                    radius,
                    _ObstacleLayers);

            // Walk on ground only
            if (_WalkOnGround)
            {
                hasNearObs =
                    hasNearObs ||
                    !UnityEngine
                        .Physics
                        .CheckCapsule(start +
                        finalOffsetNear +
                        Vector3.down * _MaxGroundOffset,
                        end + finalOffsetNear,
                        radius,
                        _GroundLayers);
                hasFarObs =
                    hasFarObs ||
                    !UnityEngine
                        .Physics
                        .CheckCapsule(start +
                        finalOffsetFar +
                        Vector3.down * _MaxGroundOffset,
                        end + finalOffsetFar,
                        radius,
                        _GroundLayers);
            }
            var halfRotAngles = _RotationalCollisionAngle / 2f;
            var rotationalBlocked = true;
            for (var i = 0; i <= _RotationalCollisionNum; i++)
            {
                RaycastHit hitInfo;
                var angleOffset =
                    _RotationalCollisionAngle / _RotationalCollisionNum * i -
                    halfRotAngles;
                var rotPointSt =
                    (start + finalOffsetRot)
                        .RotateAroundPivot(start,
                        rotation.eulerAngles + Vector3.up * angleOffset);
                var rotPointEd = rotPointSt + axisDelta;
                var angleVelocity = rotPointSt - start;

                var hasRotObs =
                    UnityEngine
                        .Physics
                        .CapsuleCast(start,
                        end,
                        radius,
                        angleVelocity,
                        out hitInfo,
                        finalOffsetRot.magnitude,
                        _ObstacleLayers);

                var rotVelocity =
                    Vector3.ProjectOnPlane(velocity, hitInfo.normal);
                var hasNormalObs = CheckNearObstacles(rotVelocity, deltaTime);

                // Root out overlapping situation with CheckCapsule()
                hasRotObs =
                    hasRotObs ||
                    UnityEngine
                        .Physics
                        .CheckCapsule(rotPointSt,
                        rotPointEd,
                        radius,
                        _ObstacleLayers);

                if (_WalkOnGround)
                {
                    var hasWalkableGround =
                        UnityEngine
                            .Physics
                            .CheckCapsule(rotPointSt +
                            Vector3.down * _MaxGroundOffset,
                            rotPointEd,
                            radius,
                            _GroundLayers);

                    // Stop normal projecting when touching border with unwalkable ground
                    hasRotObs = hasRotObs || !hasWalkableGround;
                    hasNormalObs = hasWalkableGround && hasNormalObs;
                }
                blockingResults[i] =
                    new BlockingDetectionResult()
                    {
                        AngleOffset = angleOffset,
                        IsBlocked = hasRotObs,
                        IsNormalBlocked = hasNormalObs,
                        HitInfo = hitInfo,
                        AngleDirection = angleVelocity
                    };
                rotationalBlocked = hasRotObs && rotationalBlocked;
            }
            return (hasNearObs, hasFarObs, rotationalBlocked, blockingResults);
        }

        private bool CheckNearObstacles(Vector3 vel, float deltaTime)
        {
            var direction = new Vector3 { [collider.direction] = 1 };
            var deltaPos = vel * deltaTime;
            var offset = collider.height / 2 - collider.radius;
            var localPoint0 = collider.center - direction * offset;
            var localPoint1 = collider.center + direction * offset;
            var start = transform.TransformPoint(localPoint0);
            var end = transform.TransformPoint(localPoint1);
            var axisDelta = end - start;
            var r =
                transform
                    .TransformVector(collider.radius,
                    collider.radius,
                    collider.radius);
            var radius =
                Enumerable
                    .Range(0, 3)
                    .Select(xyz => xyz == collider.direction ? 0 : r[xyz])
                    .Select(Mathf.Abs)
                    .Max();

            var finalOffsetNear = deltaPos * _CollisionCheckRangeNear;
            var hasNearObs =
                UnityEngine
                    .Physics
                    .CheckCapsule(start + finalOffsetNear,
                    end + finalOffsetNear,
                    radius,
                    _ObstacleLayers);
            return hasNearObs;
        }

        private Collider[] FindColliders()
        {
            var direction = new Vector3 { [collider.direction] = 1 };
            var offset = collider.height / 2 - collider.radius;
            var localPoint0 = collider.center - direction * offset;
            var localPoint1 = collider.center + direction * offset;
            var start = transform.TransformPoint(localPoint0);
            var end = transform.TransformPoint(localPoint1);

            var r =
                transform
                    .TransformVector(collider.radius,
                    collider.radius,
                    collider.radius);
            var radius =
                Enumerable
                    .Range(0, 3)
                    .Select(xyz => xyz == collider.direction ? 0 : r[xyz])
                    .Select(Mathf.Abs)
                    .Max();

            return UnityEngine
                .Physics
                .OverlapCapsule(start, end, radius, _ObstacleLayers);
        }

        private void UpdateMovement(Vector3 moveDirection, float deltaTime)
        {
            var preVelXZ = new Vector3(velocity.x, 0f, velocity.z);

            velocity = _MoveSpeed * moveDirection;

            var velXZ = new Vector3(velocity.x, 0f, velocity.z);
            if (preVelXZ.magnitude > 0 && velXZ.magnitude <= 0)
            {
                OnMoveStop?.Invoke();
            }
            if (preVelXZ.magnitude <= 0 && velXZ.magnitude > 0)
            {
                OnMoveStart?.Invoke();
            }

            StickToGround (moveDirection, deltaTime);

            var (
                isNearBlocked,
                isFarBlocked,
                isRotBlocked,
                rotBlockingResults
            ) = CheckObstacles(velocity, deltaTime);

            // No Collision
            if (!isFarBlocked && !isNearBlocked)
            {
                if (_RotateTowardsVelocity) RotateTowards(velocity, deltaTime);
                collisionFrame = 0;
                ApplyVelocity (velocity, deltaTime);
            }
            else
            {
                // Near end blocked or rotational vector blocked
                if (isRotBlocked)
                {
                    collisionFrame += 1;
                    if (collisionFrame >= _MaxCollisionFrame)
                    {
                        ResolveCollision();
                    }
                }
                else
                {
                    // Far end or near end blocked only
                    var minAvailableIndex = -1;
                    var minOffsetAngle = _RotationalCollisionAngle;
                    var isDirBlocked = false;

                    // Space detected at specific angle, apply rotational movement
                    for (var i = 0; i < rotBlockingResults.Length; i++)
                    {
                        // Find the available probe vector with minimum AngleOffset
                        if (
                            Mathf.Abs(rotBlockingResults[i].AngleOffset) <=
                            minOffsetAngle
                        )
                        {
                            // Blocked direction with normal available
                            if (
                                rotBlockingResults[i].IsBlocked &&
                                !rotBlockingResults[i].IsNormalBlocked
                            )
                            {
                                minAvailableIndex = i;
                                minOffsetAngle =
                                    Mathf
                                        .Abs(rotBlockingResults[i].AngleOffset);
                                isDirBlocked = true;
                                continue;
                            }
                        }
                    }
                    if (minAvailableIndex >= 0)
                    {
                        if (isDirBlocked)
                        {
                            velocity =
                                Vector3
                                    .ProjectOnPlane(velocity,
                                    rotBlockingResults[minAvailableIndex]
                                        .HitInfo
                                        .normal);
                            ApplyVelocity (velocity, deltaTime);
                        }
                        if (_RotateTowardsVelocity)
                            RotateTowards(velocity, deltaTime);
                        return;
                    }
                    collisionFrame += 1;
                }
            }
        }

        private void ResolveCollision()
        {
            collisionFrame = 0;

            // 不处理死锁碰撞
            if (_CollisionResolver.Equals(CollisionResolver.Static)) return;
            var colls = FindColliders();
            var selfTrans = GetTransform();
            var selfSize =
                transform.TransformVector(collider.radius, 0f, collider.radius);
            if (colls.Length < 1) return;
            selfSize.y = selfTrans.position.y;
            var extents =
                new Vector3(transform.lossyScale.x * colls[0].bounds.extents.x,
                    transform.lossyScale.y * colls[0].bounds.extents.y,
                    transform.lossyScale.z * colls[0].bounds.extents.z);

            switch (_CollisionResolver)
            {
                case CollisionResolver.HorizontalAvailable:
                    lastSafePosition =
                        ResolveHorizontal(colls[0].bounds,
                        selfTrans,
                        extents,
                        selfSize);
                    break;
                case CollisionResolver.HeightAvailable:
                    lastSafePosition =
                        ResolveHeight(colls[0].bounds,
                        selfTrans,
                        extents,
                        selfSize);
                    break;
                case CollisionResolver.BestAvailable:
                    var a =
                        ResolveHorizontal(colls[0].bounds,
                        selfTrans,
                        extents,
                        selfSize);
                    var b =
                        ResolveHeight(colls[0].bounds,
                        selfTrans,
                        extents,
                        selfSize);
                    lastSafePosition =
                        Vector3.Distance(a, selfTrans.position) >
                        Vector3.Distance(b, selfTrans.position)
                            ? b
                            : a;
                    break;
            }
            selfTrans.position = lastSafePosition;
        }

        private Vector3
        ResolveHeight(
            Bounds bounds,
            Transform selfTrans,
            Vector3 extents,
            Vector3 selfSize
        )
        {
            return new Vector3(selfTrans.position.x,
                bounds.center.y + extents.y + selfSize.y,
                selfTrans.position.z);
        }

        private Vector3
        ResolveHorizontal(
            Bounds bounds,
            Transform selfTrans,
            Vector3 extents,
            Vector3 selfSize
        )
        {
            var distance = Mathf.Infinity;
            var tempDistance = distance;
            var index = 0;
            var zMaxCenter =
                new Vector3(bounds.center.x,
                    selfTrans.position.y,
                    bounds.center.z + extents.z + selfSize.z);
            tempDistance = Vector3.Distance(zMaxCenter, selfTrans.position);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                index = 0;
            }

            var zMinCenter =
                new Vector3(bounds.center.x,
                    selfTrans.position.y,
                    bounds.center.z - extents.z - selfSize.z);
            tempDistance = Vector3.Distance(zMinCenter, selfTrans.position);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                index = 1;
            }

            var xMaxCenter =
                new Vector3(bounds.center.x + extents.x + selfSize.x,
                    selfTrans.position.y,
                    bounds.center.z);
            tempDistance = Vector3.Distance(xMaxCenter, selfTrans.position);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                index = 2;
            }

            var xMinCenter =
                new Vector3(bounds.center.x - extents.x - selfSize.x,
                    selfTrans.position.y,
                    bounds.center.z);
            tempDistance = Vector3.Distance(xMinCenter, selfTrans.position);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                index = 3;
            }
            return new Vector3[] {
                zMaxCenter,
                zMinCenter,
                xMaxCenter,
                xMinCenter
            }[index];
        }

        private Transform GetTransform()
        {
            switch (_ControlType)
            {
                case ControlType.Rigidbody:
                    return _PlayerBody.transform;
                case ControlType.Transform:
                    return _PosTransform != null ? _PosTransform : transform;
            }
            return null;
        }

        private Transform GetRotTransform()
        {
            return _RotTransform != null ? _RotTransform : transform;
        }

        public virtual Vector3 GetMovementDirection()
        {
            axisInput.x =
                _JoyStick != null
                    ? _JoyStick.Horizontal
                    : Input.GetAxis("Horizontal");
            axisInput.y =
                _JoyStick != null
                    ? _JoyStick.Vertical
                    : Input.GetAxis("Vertical");
            if (_FreezeHorizontal)
            {
                axisInput.x = 0f;
                axisInput.y = 0f;
            }

            Vector3 forward =
                Vector3
                    .ProjectOnPlane(cameraTransform.forward, transform.up)
                    .normalized;
            Vector3 right = Vector3.Cross(transform.up, forward);
            return axisInput.x * right + axisInput.y * forward;
        }

        public virtual void LookAt(Vector3 direction)
        {
            Vector3 dirProj = Vector3.ProjectOnPlane(direction, transform.up);

            if (dirProj.sqrMagnitude < 1E-06f) return;

            switch (_ControlType)
            {
                case ControlType.Rigidbody:
                case ControlType.Transform:
                    var targetTrans = GetRotTransform();
                    var rotation =
                        Quaternion.LookRotation(dirProj, targetTrans.up);
                    targetTrans.rotation = rotation;
                    break;
            }
        }

        private void RotateTowards(Vector3 direction, float deltaTime)
        {
            Vector3 direzioneOrizz =
                Vector3.ProjectOnPlane(direction, transform.up);

            if (direzioneOrizz.sqrMagnitude < 1E-06f) return;

            switch (_ControlType)
            {
                case ControlType.Rigidbody:
                case ControlType.Transform:
                    var targetTrans = GetRotTransform();
                    var rotation =
                        Quaternion.LookRotation(direzioneOrizz, targetTrans.up);
                    targetTrans.rotation =
                        Quaternion
                            .RotateTowards(targetTrans.rotation,
                            rotation,
                            _RotationSpeed * deltaTime);
                    break;
            }
        }

        private void ApplyVelocity(Vector3 appliedVelocity, float deltaTime)
        {
            switch (_ControlType)
            {
                case ControlType.Rigidbody:
                    _PlayerBody
                        .MovePosition(_PlayerBody.transform.position +
                        appliedVelocity * deltaTime);
                    lastSafePosition = _PlayerBody.position;
                    break;
                case ControlType.Transform:
                    var targetTrans =
                        _PosTransform != null ? _PosTransform : transform;
                    targetTrans.Translate(appliedVelocity * deltaTime);
                    lastSafePosition = targetTrans.position;
                    break;
            }
        }

        private void ApplyGravity(float deltaTime)
        {
            switch (_ControlType)
            {
                case ControlType.Rigidbody:
                    _PlayerBody
                        .MovePosition(_PlayerBody.transform.position +
                        gravityVelocity * deltaTime);
                    break;
                case ControlType.Transform:
                    var targetTrans =
                        _PosTransform != null ? _PosTransform : transform;
                    targetTrans.Translate(gravityVelocity * deltaTime);
                    break;
            }
        }

        private void MoveToPosY(float y)
        {
            switch (_ControlType)
            {
                case ControlType.Rigidbody:
                    _PlayerBody
                        .MovePosition(new Vector3(_PlayerBody.position.x,
                            y,
                            _PlayerBody.position.z));
                    break;
                case ControlType.Transform:
                    var targetTrans =
                        _PosTransform != null ? _PosTransform : transform;
                    targetTrans.position =
                        new Vector3(targetTrans.position.x,
                            y,
                            targetTrans.position.z);
                    break;
            }
        }

        public Vector3 GetVelocity()
        {
            return velocity;
        }


#if UNITY_EDITOR
        void OnDrawGizmosSelected()
        {
            collider = GetComponent<CapsuleCollider>();
            var direction = new Vector3 { [collider.direction] = 1 };

            var offset = collider.height / 2 - collider.radius;
            var localPoint0 = collider.center - direction * offset;
            var localPoint1 = collider.center + direction * offset;
            var start = transform.TransformPoint(localPoint0);
            var end = transform.TransformPoint(localPoint1);
            var axisDelta = end - start;
            var colCenter = (start + end) / 2f;

            // Ground Check
            var r =
                transform
                    .TransformVector(collider.radius,
                    collider.radius,
                    collider.radius);
            var radius =
                Enumerable
                    .Range(0, 3)
                    .Select(xyz => xyz == collider.direction ? 0 : r[xyz])
                    .Select(Mathf.Abs)
                    .Max();
            Gizmos.color = Color.blue;
            UnityEditorExt
                .DrawWireCapsule(start + Vector3.down * _SlopeStepOffset,
                end,
                radius);

            // Obstacle Check
            var virtualVelocity = _MoveSpeed * GetTransform().forward;
            var deltaPos = virtualVelocity * Time.fixedDeltaTime;
            var finalOffsetNear = deltaPos * _CollisionCheckRangeNear;
            var finalOffsetFar = deltaPos * _CollisionCheckRangeFar;
            var finalOffsetRot = deltaPos * _CollisionCheckRangeRot;
            Gizmos.color = Color.red;
            UnityEditorExt
                .DrawWireCapsule(start + finalOffsetNear,
                end + finalOffsetNear,
                radius);
            Gizmos.color = Color.magenta;
            UnityEditorExt
                .DrawWireCapsule(start + finalOffsetFar,
                end + finalOffsetFar,
                radius);

            var rotation = GetRotTransform().rotation;
            var playerTransform = GetTransform();

            // Rotational Projection Check
            var halfRotAngles = _RotationalCollisionAngle / 2f;
            Gizmos.color = Color.yellow;
            for (var i = 0; i <= _RotationalCollisionNum; i++)
            {
                var angleOffset =
                    _RotationalCollisionAngle / _RotationalCollisionNum * i -
                    halfRotAngles;
                var rotPointSt =
                    (start + finalOffsetRot)
                        .RotateAroundPivot(start,
                        rotation.eulerAngles + Vector3.up * (angleOffset));
                var rotPointCenter = rotPointSt + axisDelta / 2f;
                Handles
                    .DrawBezier(colCenter,
                    rotPointCenter,
                    colCenter,
                    rotPointCenter,
                    Color.yellow,
                    null,
                    3f);
                Gizmos.DrawSphere(rotPointCenter, 0.1f);
            }
        }
#endif
    }

    public enum ControlType
    {
        Rigidbody,
        Transform
    }

    public enum CollisionResolver
    {
        Static,
        HorizontalAvailable,
        HeightAvailable,
        BestAvailable
    }

    public struct BlockingDetectionResult
    {
        public float AngleOffset;

        public bool IsBlocked;

        public bool IsNormalBlocked;

        public RaycastHit HitInfo;

        public Vector3 AngleDirection;
    }
}
