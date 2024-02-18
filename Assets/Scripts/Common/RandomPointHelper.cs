using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RandomPointHelper 
{
    public static Vector3 RandomPointOnCircle(float radius)
    {
        var m = Random.insideUnitCircle.normalized * radius;
        return new Vector3(m.x, 0, m.y);
    }

    public static Vector3 RandomPointInRing(float radius,float orientation)
    {
        //item2 表示向正前方顺时针偏移多少角度，也就是如果我指向他，就需要我的向量转欧拉的y值
        return CreatRingRandomPoint(Vector3.zero, radius, 0, 180, Vector3.up,orientation);
    }
    
    /// <summary>
    /// 生成扇环/圆环内随机一点
    /// </summary>
    /// <param name="center">中心点</param>
    /// <param name="ringRadius">外圈半径（包括内圈半径）</param>
    /// <param name="insideRadius">内圈半径</param>
    /// <param name="angle">角度，扇形圆心角,0-360</param>
    /// <param name="axis">轴向</param>
    /// <param name="orientation">朝向，默认是朝向正前方的一个扇形</param>
    /// <returns></returns>
    public static Vector3 CreatRingRandomPoint(Vector3 center, float ringRadius, float insideRadius, float angle, Vector3 axis, float orientation = 0)
    {
        Vector3 pos = Vector3.zero;
        if (angle < 0 || angle > 360)
        {
            return pos;
        }
        if (ringRadius <= insideRadius || ringRadius <= 0 || insideRadius < 0)
        {
            return pos;
        }
        float posz = Random.Range(insideRadius, ringRadius);
        float randomAngle = Random.Range(-angle / 2, angle / 2);
        //如果是绕前方轴的话需要更改一下旋转点的初始位置
        if (axis == Vector3.forward)
        {
            pos = new Vector3(posz, 0, 0);
        }
        else
        {
            pos = new Vector3(0, 0, posz);
        }
        return RotateRound(pos, Vector3.zero, axis, randomAngle + orientation) + center;
    }
 
    /// <summary>
    /// 向量绕某点旋转一定角度
    /// </summary>
    /// <param name="position">旋转的向量</param>
    /// <param name="center">中心点</param>
    /// <param name="axis">旋转轴</param>
    /// <param name="angle">旋转角度</param>
    /// <returns></returns>
    public static Vector3 RotateRound(Vector3 position, Vector3 center, Vector3 axis, float angle)
    {
        Vector3 point = Quaternion.AngleAxis(angle, axis) * (position - center);
        Vector3 resultVec3 = center + point;
        return resultVec3;
    }
}
