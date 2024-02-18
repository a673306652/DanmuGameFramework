using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hisao
{
    public static class HisaoList
    {
        public static List<T> ShuffleList<T>(this List<T> targetList)
        {
            for (int i = targetList.Count - 1; i > 0; i--)
            {
                int exchange = Random.Range(0, i + 1);
                (targetList[i], targetList[exchange]) = (targetList[exchange], targetList[i]);
            }

            return targetList;
        }

        public static T[] ShuffleArray<T>(this T[] targetList)
        {
            for (int i = targetList.Length - 1; i > 0; i--)
            {
                int exchange = Random.Range(0, i + 1);
                (targetList[i], targetList[exchange]) = (targetList[exchange], targetList[i]);
            }

            return targetList;
        }
    }
}