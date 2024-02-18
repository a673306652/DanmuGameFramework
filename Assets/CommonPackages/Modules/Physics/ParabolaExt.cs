using UnityEngine;
namespace Modules.Physics
{
    public class ParabolaExt
    {
        /// <param name="t">0到1的值，0获取曲线的起点，1获得曲线的终点</param>
        /// <param name="start">曲线的起始位置</param>
        /// <param name="center">决定曲线形状的控制点</param>
        /// <param name="end">曲线的终点</param>
        public static Vector3 GetBezierPoint(float t, Vector3 start, Vector3 center, Vector3 end)
        {
            return (1 - t) * (1 - t) * start + 2 * t * (1 - t) * center + t * t * end;
        }
    }
}