using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
namespace Daan
{
    public class MyMath
    {
        public static List<Vector3> standardV3List = new List<Vector3>() { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        public static List<Vector3> diceV3List = new List<Vector3>() { Vector3.up, Vector3.left, Vector3.back, Vector3.right, Vector3.forward, Vector3.down };


        public const double PI = 3.141592653589793D;
        public const float Deg2Rad = 0.0174532924F;

        public class Color
        {
            /// <summary>
            /// color 转换hex
            /// </summary>
            /// <param name="color"></param>
            /// <returns></returns>
            public static string ColorToHex(UnityEngine.Color color)
            {
                int r = Mathf.RoundToInt(color.r * 255.0f);
                int g = Mathf.RoundToInt(color.g * 255.0f);
                int b = Mathf.RoundToInt(color.b * 255.0f);
                int a = Mathf.RoundToInt(color.a * 255.0f);
                string hex = string.Format("{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a);
                return hex;
            }

            public static string GetColorString(string str, UnityEngine.Color color)
            {
                var hex = ColorToHex(color);
                return $"<color=#{hex}>{str}</color>";
            }

            /// <summary>
            /// hex转换到color
            /// </summary>
            /// <param name="hex"></param>
            /// <returns></returns>
            public static UnityEngine.Color HexToColor(string hex, bool withAlpha = false)
            {
                hex = hex.Replace("#", "");
                byte br = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                byte bg = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                byte bb = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);

                float r = br / 255f;
                float g = bg / 255f;
                float b = bb / 255f;
                float a = 1;
                if (withAlpha)
                {
                    byte cc = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
                    a = cc / 255f;
                }

                return new UnityEngine.Color(r, g, b, a);
            }


            public static UnityEngine.Color RandomColor(float a = 1, System.Random random = null)
            {
                if (random != null)
                {
                    return new UnityEngine.Color(random.Next(0, 256) / 255F, random.Next(0, 256) / 255F, random.Next(0, 256) / 255F, a);
                }
                else
                {
                    return new UnityEngine.Color(UnityEngine.Random.Range(0, 256) / 255F, UnityEngine.Random.Range(0, 256) / 255F, UnityEngine.Random.Range(0, 256) / 255F, a);
                }
            }
        }

        public static Vector3[] GenerateParabola(float height, Vector3 pointA, Vector3 pointB, int count, float g = -9.81F)
        {
            var result = new Vector3[count + 1];

            var dir = (pointB - pointA);
            dir.y = 0;
            var shotWidth = dir.magnitude;
            float a, b, c;
            //根据炮弹的发射点、高度、射程计算出炮弹抛物线的三点
            var x1 = 0;
            var y1 = pointA.y;

            var x2 = shotWidth / 2;
            var y2 = height;

            var x3 = shotWidth;
            var y3 = pointB.y;

            b = ((y1 - y3) * (x1 * x1 - x2 * x2) - (y1 - y2) * (x1 * x1 - x3 * x3)) / ((x1 - x3) * (x1 * x1 - x2 * x2) - (x1 - x2) * (x1 * x1 - x3 * x3));
            a = ((y1 - y2) - b * (x1 - x2)) / (x1 * x1 - x2 * x2);
            c = y1 - a * x1 * x1 - b * x1;

            var vect = Vector3.zero;
            for (int i = 0; i < count; i++)
            {
                var per = (shotWidth / count) * i;
                vect = pointA + dir.normalized * per;
                vect.y = a * per * per + b * per + c;
                result[i] = vect;
            }
            result[count] = pointB;

            return result;
        }
        /// <param name="startPosition"></param>
        /// <param name="finalPosition"></param>
        /// <param name="maxHeightOffset"></param>
        /// <param name="rangeOffset">目标落点往后移一些（这样可以保证抛物线的过程中就会碰到敌人，而不是正好抛物线的尽头是敌人的脚下）</param>
        /// <returns></returns>
        public static Vector3 GetParabolaForce(Vector3 startPosition, Vector3 finalPosition, float maxHeightOffset = 0.0f, float rangeOffset = 0.11f)
        {
            // get our return value ready. Default to (0f, 0f, 0f)
            Vector3 newVel = new Vector3();
            // Find the direction vector without the y-component
            /// /找到未经y分量的方向矢量//
            Vector3 direction = new Vector3(finalPosition.x, 0f, finalPosition.z) - new Vector3(startPosition.x, 0f, startPosition.z);
            // Find the distance between the two points (without the y-component)
            //发现这两个点之间的距离（不y分量）//
            float range = direction.magnitude;
            // Add a little bit to the range so that the ball is aiming at hitting the back of the rim.
            // Back of the rim shots have a better chance of going in.
            // This accounts for any rounding errors that might make a shot miss (when we don't want it to).
            range += rangeOffset;
            // Find unit direction of motion without the y component
            Vector3 unitDirection = direction.normalized;
            // Find the max height
            // Start at a reasonable height above the hoop, so short range shots will have enough clearance to go in the basket
            // without hitting the front of the rim on the way up or down.
            float maxYPos = finalPosition.y + maxHeightOffset;
            // check if the range is far enough away where the shot may have flattened out enough to hit the front of the rim
            // if it has, switch the height to match a 45 degree launch angle
            //if (range / 2f > maxYPos)
            //  maxYPos = range / 2f;
            if (maxYPos < startPosition.y)
                maxYPos = startPosition.y;

            // find the initial velocity in y direction
            /// /发现在y方向上的初始速度//
            float ft;


            ft = -2.0f * Physics.gravity.y * (maxYPos - startPosition.y);
            if (ft < 0)
                ft = 0f;

            newVel.y = Mathf.Sqrt(ft);
            // find the total time by adding up the parts of the trajectory
            // time to reach the max
            //发现的总时间加起来的轨迹的各部分//
            //时间达到最大//

            ft = -2.0f * (maxYPos - startPosition.y) / Physics.gravity.y;
            if (ft < 0)
                ft = 0f;

            float timeToMax = Mathf.Sqrt(ft);
            // time to return to y-target
            //时间返回到y轴的目标//

            ft = -2.0f * (maxYPos - finalPosition.y) / Physics.gravity.y;
            if (ft < 0)
                ft = 0f;

            float timeToTargetY = Mathf.Sqrt(ft);
            // add them up to find the total flight time
            //把它们加起来找到的总飞行时间//
            float totalFlightTime;

            totalFlightTime = timeToMax + timeToTargetY;

            // find the magnitude of the initial velocity in the xz direction
            /// /查找的初始速度的大小在xz方向//
            float horizontalVelocityMagnitude = range / totalFlightTime;
            // use the unit direction to find the x and z components of initial velocity
            //使用该单元的方向寻找初始速度的x和z分量//
            newVel.x = horizontalVelocityMagnitude * unitDirection.x;
            newVel.z = horizontalVelocityMagnitude * unitDirection.z;
            return newVel;
        }
        public static Vector3 RotatePointByEulerX(Vector3 point, Vector3 center, float angle)
        {
            float anglehude = angle * Mathf.PI / 180;
            float y = (point.y - center.y) * Mathf.Cos(anglehude) - (point.z - center.z) * Mathf.Sin(anglehude) + center.y;
            float z = (point.y - center.y) * Mathf.Sin(anglehude) + (point.z - center.z) * Mathf.Cos(anglehude) + center.z;
            y = Mathf.Abs(y - 1) < 0.0001F ? 1 : Mathf.Abs(y - 0F) < .0001F ? 0 : y;
            z = Mathf.Abs(z - 1) < 0.0001F ? 1 : Mathf.Abs(z - 0F) < .0001F ? 0 : z;
            return new Vector3(point.x, y, z);
        }
        public static Vector3 RotatePointByEulerY(Vector3 point, Vector3 center, float angle)
        {
            float anglehude = angle * Mathf.PI / 180;
            float x = (point.x - center.x) * Mathf.Cos(anglehude) + (point.z - center.z) * Mathf.Sin(anglehude) + center.x;
            float z = -(point.x - center.x) * Mathf.Sin(anglehude) + (point.z - center.z) * Mathf.Cos(anglehude) + center.z;
            x = Mathf.Abs(x - 1) < 0.0001F ? 1 : Mathf.Abs(x - 0F) < .0001F ? 0 : x;
            z = Mathf.Abs(z - 1) < 0.0001F ? 1 : Mathf.Abs(z - 0F) < .0001F ? 0 : z;
            return new Vector3(x, point.y, z);
        }
        public static Vector3 RotatePointByEulerZ(Vector3 point, Vector3 center, float angle)
        {
            float anglehude = angle * Mathf.PI / 180;
            float x = (point.x - center.x) * Mathf.Cos(anglehude) - (point.y - center.y) * Mathf.Sin(anglehude) + center.x;
            float y = (point.x - center.x) * Mathf.Sin(anglehude) + (point.y - center.y) * Mathf.Cos(anglehude) + center.y;
            x = Mathf.Abs(x - 1) < 0.0001F ? 1 : Mathf.Abs(x - 0F) < .0001F ? 0 : x;
            y = Mathf.Abs(y - 1) < 0.0001F ? 1 : Mathf.Abs(y - 0F) < .0001F ? 0 : y;
            return new Vector3(x, y, point.z);
        }
        public static Matrix4x4 GetPositionMatrix(Vector3 position)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetRow(0, new Vector4(1f, 0f, 0f, position.x));
            matrix.SetRow(1, new Vector4(0f, 1f, 0f, position.y));
            matrix.SetRow(2, new Vector4(0f, 0f, 1f, position.z));
            matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
            return matrix;
        }
        public static Matrix4x4 GetScaleMatrix(Vector3 scale)
        {
            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetRow(0, new Vector4(scale.x, 0f, 0f, 0f));
            matrix.SetRow(1, new Vector4(0f, scale.y, 0f, 0f));
            matrix.SetRow(2, new Vector4(0f, 0f, scale.z, 0f));
            matrix.SetRow(3, new Vector4(0f, 0f, 0f, 1f));
            return matrix;
        }
        public static Matrix4x4 GetRotationMatrix(Vector3 rotation)
        {
            float radX = rotation.x * Mathf.Deg2Rad;
            float radY = rotation.y * Mathf.Deg2Rad;
            float radZ = rotation.z * Mathf.Deg2Rad;
            float sinX = Mathf.Sin(radX);
            float cosX = Mathf.Cos(radX);
            float sinY = Mathf.Sin(radY);
            float cosY = Mathf.Cos(radY);
            float sinZ = Mathf.Sin(radZ);
            float cosZ = Mathf.Cos(radZ);

            Matrix4x4 matrix = new Matrix4x4();
            matrix.SetColumn(0, new Vector4(
                cosY * cosZ,
                cosX * sinZ + sinX * sinY * cosZ,
                sinX * sinZ - cosX * sinY * cosZ,
                0f
            ));
            matrix.SetColumn(1, new Vector4(
                -cosY * sinZ,
                cosX * cosZ - sinX * sinY * sinZ,
                sinX * cosZ + cosX * sinY * sinZ,
                0f
            ));
            matrix.SetColumn(2, new Vector4(
                sinY,
                -sinX * cosY,
                cosX * cosY,
                0f
            ));
            matrix.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
            return matrix;
        }
        public static int Compute(int a, int b, string symbol)
        {
            if (symbol == "-")
            {
                return a - b;
            }
            if (symbol == "=")
            {
                return a = b;
            }
            if (symbol == "*")
            {
                return a * b;
            }
            if (symbol == "/")
            {
                return a / b;
            }
            if (symbol == "+")
            {
                return a + b;
            }
            return a;
        }
        public static float Compute(float a, float b, string symbol)
        {
            if (symbol == "=")
            {
                return a = b;
            }
            if (symbol == "*")
            {
                return a * b;
            }
            if (symbol == "/")
            {
                return a / b;
            }
            if (symbol == "+")
            {
                return a + b;
            }
            if (symbol == "-")
            {
                return a - b;
            }
            return a;
        }
        public static double Compute(double a, double b, string symbol)
        {
            if (symbol == "-")
            {
                return a - b;
            }
            if (symbol == "=")
            {
                return a = b;
            }
            if (symbol == "*")
            {
                return a * b;
            }
            if (symbol == "/")
            {
                return a / b;
            }
            if (symbol == "+")
            {
                return a + b;
            }
            return a;
        }
        public static bool Compare(double a, double b, string symbol)
        {
            if (symbol == "=")
            {
                return a == b;
            }
            if (symbol == ">=")
            {
                return a >= b;
            }
            if (symbol == "<=")
            {
                return a <= b;
            }
            if (symbol == "!=")
            {
                return a != b;
            }
            if (symbol == ">")
            {
                return a > b;
            }
            if (symbol == "<")
            {
                return a < b;
            }
            return false;
        }
        /// <summary>
        /// 获取两点所形成的射线的方程式的ABC
        /// </summary>
        public static Vector3 GetRayABC(Vector2 start, Vector2 end)
        {
            float a = 0;
            float b = 0;
            float c = 0;

            if ((start.x == end.x) && (start.y == end.y))
            {

            }
            else if (start.x == end.x)
            {
                a = 1;
                b = 0;
                c = -start.x;
            }
            else
            {
                a = -(end.y - start.y) / (end.x - start.x);
                b = 1;
                c = start.x * (end.y - start.y) / (end.x - start.x) - start.y;
            }

            return new Vector3(a, b, c);
        }

        public static float GetSqrDistance(Vector3 a, Vector3 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            float dz = a.z - b.z;
            return dx * dx + dy * dy + dz * dz;
        }
        /// <summary>
        /// 获取两条射线之间的交点
        /// abc代表射线函数的方程式ax^2+bx+c
        /// </summary>
        public static Vector2 GetIntersection2D(Vector3 abc1, Vector3 abc2)
        {
            return GetIntersection2D(abc1.x, abc1.y, abc1.z, abc2.x, abc2.y, abc2.z);
        }
        /// <summary>
        /// 获取两条射线之间的交点
        /// abc代表射线函数的方程式ax^2+bx+c
        /// </summary>
        public static Vector2 GetIntersection2D(float a1, float b1, float c1, float a2, float b2, float c2)
        {
            Vector2 result = new Vector2();
            float x = 0;
            float y = 0;
            if ((b1 == 0) && (b2 == 0))
            {
                return result;
            }
            else if (b1 == 0)
            {
                x = -c1;
                y = -(a2 * x + c2) / b2;
            }
            else if (b2 == 0)
            {
                x = -c2;
                y = -(a1 * x + c1) / b1;
            }
            else
            {
                if ((a1 / b1) == (a2 / b1))
                {
                    return result;
                }
                else
                {
                    x = (c1 - c2) / (a2 - a1);
                    y = -(a1 * x) - c1;
                }
            }
            result.x = x;
            result.y = y;
            return result;

        }
        /// <summary>
        /// 贝塞尔曲线
        /// </summary>
        /// <param name="poss">预设点的位置</param>
        /// <param name="precision">想要取的点的数量</param>
        /// <returns>返回所有点的数组</returns>
        public static Vector2[] BezierVector2(Vector2[] poss, int precision)
        {
            //维度，坐标轴数（二维坐标，三维坐标...）
            int dimersion = 2;

            //贝塞尔曲线控制点数（阶数）
            int number = poss.Length;

            //控制点数不小于 2 ，至少为二维坐标系
            if (number < 2 || dimersion < 2)
                return null;

            Vector2[] result = new Vector2[precision];

            //计算杨辉三角
            int[] mi = new int[number];
            mi[0] = mi[1] = 1;
            for (int i = 3; i <= number; i++)
            {

                int[] t = new int[i - 1];
                for (int j = 0; j < t.Length; j++)
                {
                    t[j] = mi[j];
                }

                mi[0] = mi[i - 1] = 1;
                for (int j = 0; j < i - 2; j++)
                {
                    mi[j + 1] = t[j] + t[j + 1];
                }
            }

            //计算坐标点
            for (int i = 0; i < precision; i++)
            {
                float t = (float)i / precision;
                result[i] = new Vector2();
                for (int k = 0; k < number; k++)
                {
                    result[i].x += Mathf.Pow(1 - t, number - k - 1) * poss[k].x * Mathf.Pow(t, k) * mi[k];
                    result[i].y += Mathf.Pow(1 - t, number - k - 1) * poss[k].y * Mathf.Pow(t, k) * mi[k];
                }
            }

            return result;
        }

        public static Vector2[] BezierVector3(Vector2[] poss, int precision)
        {
            //维度，坐标轴数（二维坐标，三维坐标...）
            int dimersion = 2;

            //贝塞尔曲线控制点数（阶数）
            int number = poss.Length;

            //控制点数不小于 2 ，至少为二维坐标系
            if (number < 2 || dimersion < 2)
                return null;

            Vector2[] result = new Vector2[precision];

            //计算杨辉三角
            int[] mi = new int[number];
            mi[0] = mi[1] = 1;
            for (int i = 3; i <= number; i++)
            {

                int[] t = new int[i - 1];
                for (int j = 0; j < t.Length; j++)
                {
                    t[j] = mi[j];
                }

                mi[0] = mi[i - 1] = 1;
                for (int j = 0; j < i - 2; j++)
                {
                    mi[j + 1] = t[j] + t[j + 1];
                }
            }

            //计算坐标点
            for (int i = 0; i < precision; i++)
            {
                float t = (float)i / precision;
                result[i] = new Vector2();
                for (int k = 0; k < number; k++)
                {
                    result[i].x += Mathf.Pow(1 - t, number - k - 1) * poss[k].x * Mathf.Pow(t, k) * mi[k];
                    result[i].y += Mathf.Pow(1 - t, number - k - 1) * poss[k].y * Mathf.Pow(t, k) * mi[k];
                }
            }

            return result;
        }

        /// <summary>
        /// 获取一个点到直线的距离(如果该点无法投影到直线上，则返回-1)
        /// </summary>
        public static double GetPointToLineDistance(Vector2 selfPos, Vector2 start, Vector2 end, ref Vector2 dir)
        {
            dir = end - start;
            float x = selfPos.x;
            float y = selfPos.y;
            float x1 = start.x;
            float y1 = start.y;
            float x2 = end.x;
            float y2 = end.y;
            double cross = (x2 - x1) * (x - x1) + (y2 - y1) * (y - y1);
            if (cross <= 0)
            {
                dir = selfPos - start;
                return Math.Sqrt((x - x1) * (x - x1) + (y - y1) * (y - y1));
            }



            double d2 = (x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1);

            if (cross >= d2)
            {
                dir = selfPos - end;
                return Math.Sqrt((x - x2) * (x - x2) + (y - y2) * (y - y2));
            }



            double r = cross / d2;

            double px = x1 + (x2 - x1) * r;

            double py = y1 + (y2 - y1) * r;
            dir = new Vector2((float)(x - px), (float)(y - py));
            dir.Normalize();
            return System.Math.Sqrt((x - px) * (x - px) + (py - y) * (py - y));


        }
        /// <summary>
        /// 得到一个数字在一个循环中处于第几位
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cycle"></param>
        /// <returns></returns>
        public static int CycleInt(int value, int cycle)
        {
            return value < 0 ? (cycle - Math.Abs(value) % cycle) % cycle : value % cycle;
        }
        public static float LimitAngle(float angle, float target, float limit)
        {
            float var4 = WrapAngle(target - angle);
            if (var4 > limit)
            {
                var4 = limit;
            }

            if (var4 < -limit)
            {
                var4 = -limit;
            }

            float var5 = angle + var4;

            if (var5 < 0.0F)
            {
                var5 += 360.0F;
            }
            else if (var5 > 360.0F)
            {
                var5 -= 360.0F;
            }

            return var5;
        }
        /// <summary>
        /// 传入两点，得到两点间方向
        /// </summary>
        /// <returns></returns>
        public static float LookAt(Vector3 from, Vector3 to)
        {
            double dx = from.x - to.x;
            double dy = from.y - to.y;
            double dz = from.z - to.z;

            return -((float)(Math.Atan2(dz, dx) * 180.0D / MyMath.PI) - 90.0F);
        }


        public static float ClampAngle(float angle, float min = 0, float max = 360)
        {
            return Mathf.Clamp(WrapAngle(angle), min, max);
        }
        public static Vector3 WrapAngle(Vector3 angle)
        {
            angle.x = WrapAngle(angle.x);
            angle.y = WrapAngle(angle.y);
            angle.z = WrapAngle(angle.z);
            return angle;
        }
        public static float WrapAngle(float angle)
        {
            angle %= 360.0f;
            if (angle >= 180F)
                angle -= 360F;
            if (angle < -180F)
                angle += 360F;

            return angle;
        }
        public static float WrapAngle360(float angle)
        {
            if (angle < 0) angle += 360;
            angle %= 360.0f;
            return angle;
        }
        public static int Floor(double d)
        {
            int var2 = (int)d;
            return d < (double)var2 ? var2 - 1 : var2;
        }
        public static int Floor(float value)
        {
            int var1 = (int)value;
            return value < (float)var1 ? var1 - 1 : var1;
        }

        public static long Floor_Double_Long(double d)
        {
            long var2 = (long)d;
            return d < (double)var2 ? var2 - 1L : var2;
        }
        public static double Clamp_Double(double value, double min, double max)
        {
            return value < min ? min : (value > max ? max : value);
        }


        public static int Ceiling_Float_Int(float value)
        {
            int var1 = (int)value;
            return value > (float)var1 ? var1 + 1 : var1;
        }

        public static int BucketFloat(float a, float b)
        {
            if (a > 0)
            {
                return (int)(a / b);
            }
            else
            {
                return Mathf.FloorToInt(a / b);
            }
        }
        public static int BucketInt(int a, int b)
        {
            return a < 0 ? -((-a - 1) / b) - 1 : a / b;
        }

        /// <summary>
        /// 获取所占的位数
        /// </summary>
        public static int GetBitSize(int value)
        {
            for (int i = 8; i >= 0; i--)
            {
                if (value >> i == 1)
                {
                    return i + 1;
                }
            }
            return 0;
        }
        public static int GetBitCount(int value)
        {
            int count = 0;
            while (value > 0)
            {
                count++;
                value &= (value - 1);
            }
            return count;
        }

        /// <summary>
        /// 获得旋转的标准化向量
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        /// <returns></returns>
        public static Vector3 GetLookNormal(float pitch, float yaw)
        {
            float var3 = (float)Math.Cos(yaw * MyMath.Deg2Rad - (float)Math.PI);
            float var4 = (float)Math.Sin(yaw * MyMath.Deg2Rad - (float)Math.PI);

            float var5 = (float)-Math.Cos(-pitch * MyMath.Deg2Rad);
            float var6 = (float)Math.Sin(-pitch * MyMath.Deg2Rad);

            return new Vector3((var4 * var5), var6, (var3 * var5));
        }
        /// <summary>
        /// 向下取余
        /// </summary>
        public static int Remainder_Floor(float value, int r)
        {
            int v1 = Mathf.FloorToInt(value / r);
            return v1;
        }
        /// <summary>
        /// 取整（可按倍数取整）
        /// </summary>
        public static int ToInt(float value, int rate)
        {
            return ((int)value) / rate * rate;
        }
        /// <summary>
        /// 根据除法进行插值
        /// </summary>
        /// <param name="c"></param>
        /// <param name="n"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float Lerp_Division(float c, float n, float v)
        {
            return c + (n - c) / v;
        }
        public static double Lerp_Division(double c, double n, double v)
        {
            return c + (n - c) / v;
        }
        /// <summary>
        /// 按照乘法进行插值
        /// </summary>
        public static double Lerp_Multiplication(double last, double next, double delta)
        {
            return last + (next - last) * delta;
        }

        public static Vector3 RandomEulerAngle()
        {
            return new Vector3(UnityEngine.Random.Range(0, 360F), UnityEngine.Random.Range(0, 360F), UnityEngine.Random.Range(0, 360F));
        }

        public static Vector3 RandomPositionInCircl(float radius, Quaternion rotation)
        {
            return rotation * (UnityEngine.Random.insideUnitCircle * UnityEngine.Random.Range(0, radius));
        }

        public static Vector3 RandomPositionInSphere(float radius)
        {
            return UnityEngine.Random.onUnitSphere * UnityEngine.Random.Range(0, radius);
        }

        public static Vector3 RandomPosition(float range)
        {
            return new Vector3(UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range));
        }

        public static Vector3 GetRandomPosition(Vector3 size)
        {
            var randomX = UnityEngine.Random.Range(-size.x / 2, size.x / 2);
            var randomY = UnityEngine.Random.Range(-size.y / 2, size.y / 2);
            var randomZ = UnityEngine.Random.Range(-size.z / 2, size.z / 2);

            return new Vector3(randomX, randomY, randomZ);
        }

        public static Vector3 RandomPosition(Vector3 size, Quaternion rotation = default)
        {
            var forward = rotation * Vector3.forward;
            var right = rotation * Vector3.right;
            var up = rotation * Vector3.up;
            var randomX = UnityEngine.Random.Range(-size.x / 2, size.x / 2);
            var randomY = UnityEngine.Random.Range(-size.y / 2, size.y / 2);
            var randomZ = UnityEngine.Random.Range(-size.z / 2, size.z / 2);

            return forward * randomZ + up * randomY + right * randomX;
        }

        public static Vector3 RandomPositionXZ(Rect rect)
        {
            return new Vector3(UnityEngine.Random.Range(rect.xMin, rect.xMax), 0, UnityEngine.Random.Range(rect.yMin, rect.yMax));
        }

    }
}
