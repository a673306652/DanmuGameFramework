namespace Modules.Physics
{
    using System.Collections.Generic;
    using UnityEngine;

    public static class BezierCurveExt
    {
        public static float SimpleInterpolate(float p0, float p1, float t)
        {
            return (1 - t) * p0 + t * p1;
        }

        public static float SquareInterpolate(float p0, float p1, float p2, float t)
        {
            float p0p1 = (1 - t) * p0 + t * p1;
            float p1p2 = (1 - t) * p1 + t * p2;
            return (1 - t) * p0p1 + t * p1p2; ;
        }

        public static float CubicInterpolate(float a, float b, float c, float d, float t)
        {
            var tt = t * t;
            var ttt = tt * t;

            var e = a - b;
            var f = d - c;
            var g = f - e;
            var h = e - g;
            var i = c - a;

            return g * ttt + h * tt + i * t + b;
        }

        /// <summary>
        /// Gets a point along a line defined by the two ends p1 and p2, with the interpolant t
        /// </summary>
        public static Vector3 LineLerpV3(Vector3 p1, Vector3 p2, float t)
        {
            float x = Mathf.Lerp(p1.x, p2.x, t);
            float y = Mathf.Lerp(p1.y, p2.y, t);
            float z = Mathf.Lerp(p1.z, p2.z, t);

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Interpolates between three control points with a quadratic bezier curve, with the interpolant t
        /// </summary>
        public static Vector3 QuadraticInterpolateV3(Transform p1, Transform p2, Transform p3, float t)
        {
            Vector3 a = LineLerpV3(p1.position, p2.position, t);
            Vector3 b = LineLerpV3(p2.position, p3.position, t);

            return LineLerpV3(a, b, t);
        }

        /// <summary>
        /// Interpolates between four control points with a cubic bezier curve, with the interpolant t
        /// </summary>
        public static Vector3 CubicInterpolateV3(Transform p1, Transform p2, Transform p3, Transform p4, float t)
        {
            Vector3 a = LineLerpV3(p1.position, p2.position, t);
            Vector3 b = LineLerpV3(p2.position, p3.position, t);
            Vector3 c = LineLerpV3(p3.position, p4.position, t);

            Vector3 d = LineLerpV3(a, b, t);
            Vector3 e = LineLerpV3(b, c, t);

            return LineLerpV3(d, e, t);
        }

        /// <summary>
        /// Interpolates between any number of control points in the points list, using a bezier curve and the interpolant, t.
        /// </summary>
        public static Vector3 NOrderInterpolateV3(List<Transform> points, float t)
        {
            if (points.Count < 2)
                throw new System.Exception("Bezier Curve needs at least 3 points, or 2 for a linear interpolation");

            //Convert the list of Transform's to a list of Vector3
            List<Vector3> vecp = new List<Vector3>();

            foreach (Transform p in points)
            {
                vecp.Add(p.position);
            }

            return NOrder_R(vecp, t);
        }


        /// <summary>
        /// Interpolates between any number of control points in the points list, using a bezier curve and the interpolant, t.
        /// </summary>
        public static Vector3 NOrderInterpolateV3(List<Vector3> points, float t)
        {
            if (points.Count < 2)
                throw new System.Exception("Bezier Curve needs at least 3 points, or 2 for a linear interpolation");

            //Convert the list of Transform's to a list of Vector3
            List<Vector3> vecp = new List<Vector3>();

            foreach (Vector3 p in points)
            {
                vecp.Add(p);
            }

            return NOrder_R(vecp, t);
        }

        /// <summary>
        /// Underlying recursive function to calculate n order bezier curves
        /// </summary>
        private static Vector3 NOrder_R(List<Vector3> points, float t)
        {
            if (points.Count == 2)
            {
                return LineLerpV3(points[0], points[1], t);
            }

            List<Vector3> lines = new List<Vector3>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                Vector3 line = LineLerpV3(points[i], points[i + 1], t);
                lines.Add(line);
            }

            return NOrder_R(lines, t);
        }
    }
}