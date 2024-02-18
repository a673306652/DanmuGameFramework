using System.Linq;
using UnityEngine;
namespace Modules.Physics
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class GroundDetector : MonoBehaviour
    {
        [Header("Properties")]
        [SerializeField] protected LayerMask _GroundLayers;
        [SerializeField] protected float _SlopeStepOffset = 0.0001f;

        protected CapsuleCollider collider;

        void Awake()
        {
            collider = GetComponent<CapsuleCollider>();
        }

        public void StickToGround()
        {
            Ray downwardRay = new Ray(transform.position, transform.up * -1);
            var (inCol, nxtInCol) = CheckGroundColliders();
            if ((inCol && nxtInCol) || (inCol && !nxtInCol))
            {
                // 与地面完全接触
                MovePositionY(transform.position.y + _SlopeStepOffset / 2f);
            }
            else if (!inCol && !nxtInCol)
            {
                // 与地面无任何接触
                if (UnityEngine.Physics.Raycast(downwardRay, out RaycastHit hitInfo, Mathf.Infinity, _GroundLayers))
                {
                    MovePositionY(hitInfo.point.y);
                }
            }
            else
            {
                // 合理接触 !inCol && nxtInCol
            }
        }

        private void MovePositionY(float y)
        {
            var targetTrans = transform;
            targetTrans.position = new Vector3(targetTrans.position.x, y, targetTrans.position.z);
        }

        /// <summary>
        /// Check collision with ground
        /// </summary>
        /// <returns>(isInCollision, isNextStepInCollision)</returns>
        private (bool, bool) CheckGroundColliders()
        {
            var direction = new Vector3
            {
                [collider.direction] = 1
            };

            var offset = collider.height / 2 - collider.radius;
            var localPoint0 = collider.center - direction * offset;
            var localPoint1 = collider.center + direction * offset;
            var start = transform.TransformPoint(localPoint0);
            var end = transform.TransformPoint(localPoint1);

            var r = transform.TransformVector(collider.radius, collider.radius, collider.radius);
            var radius = Enumerable.Range(0, 3).Select(xyz => xyz == collider.direction ? 0 : r[xyz])
                .Select(Mathf.Abs).Max();
            return (UnityEngine.Physics.CheckCapsule(start, end, radius, _GroundLayers), UnityEngine.Physics.CheckCapsule(start + Vector3.down * _SlopeStepOffset, end, radius, _GroundLayers));
        }
    }
}