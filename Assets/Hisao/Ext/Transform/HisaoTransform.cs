using UnityEngine;

namespace Hisao
{
    public static class HisaoTransform
    {
        public static Vector3 x(this Vector3 pos)
        {
            return new Vector3(pos.x, 0, 0);
        }

        public static Vector3 y(this Vector3 pos)
        {
            return new Vector3(0, pos.y, 0);
        }

        public static Vector3 z(this Vector3 pos)
        {
            return new Vector3(0, 0, pos.z);
        }

        public static Vector3 xy(this Vector3 pos)
        {
            return new Vector3(pos.x, pos.y, 0);
        }

        public static Vector3 xz(this Vector3 pos)
        {
            return new Vector3(pos.x, 0, pos.z);
        }

        public static Vector3 yz(this Vector3 pos)
        {
            return new Vector3(0, pos.y, pos.z);
        }

        public static Vector3 xyz(this Vector3 pos)
        {
            return pos;
        }
        
        //TransA2B

        public static Vector3 Euler2Dir(this Vector3 pos)
        {
            return (Quaternion.Euler(pos) * Vector3.forward).normalized;
        }

        public static Quaternion Euler2Quaternion(this Vector3 pos)
        {
            return Quaternion.Euler(pos);
        }
        public static Vector3 Dir2Euler(this Vector3 pos)
        {
            return Quaternion.LookRotation(pos).eulerAngles;
        }

        public static Quaternion Dir2Quaternion(this Vector3 pos)
        {
            return Quaternion.LookRotation(pos);
        }
        public static Vector3 GlobalDir2LocalDir2D(this Vector3 GDir, Vector3 localDir)
        {
            var rotation = Quaternion.Euler(0, 90, 0);
            var right = rotation * GDir;
            return  (GDir * localDir.z) + (right * localDir.x);
        }

        public static Vector3 Screen2WorldPos(Vector3 Screen,Camera cam, float depth)
        {
            var pos = cam.ScreenToWorldPoint(Screen.xy() + new Vector3(0, 0, depth));
            return pos;
        }

        public static Vector3 World2Screen(Vector3 pos, Camera cam)
        {
            return cam.WorldToScreenPoint(pos);
        }
        
        
    }
}