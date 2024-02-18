using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class RollTool  
{
    public static Dictionary<string, SkillPool> skillPools;

    public static void Init()
    {
        skillPools = new Dictionary<string, SkillPool>();

        skillPools.Add("Test", new SkillPool(new List<(string, float)>()
        {
            ("H1001", 1f), ("H1002", 1f), ("H1003", 1f), ("H1004", 1f), ("H1005", 1f), ("H1007", 1f),
            ("H1008", 1f), ("H1009", 1f),
            ("H1010", 1f), ("H1011", 1f), ("H1012", 1f), ("H1013", 1f), ("H1014", 1f), ("H1016", 1f), ("H1017", 1f),
            ("H1018", 1f), ("H1019", 1f),
            ("H1020", 1f), ("H1021", 1f), ("H1022", 1f), ("H1023", 1f), ("H1024", 1f), ("H1025", 1f), ("H1026", 1f),
        }));
    }
}

public class SkillPool
{
    public string PoolKey;
    public Dictionary<string, float> SkillDict;
    public List<SkillRollNumber> skillRandomPool;

    public Dictionary<string, int> playerProtect;
    public int protectCount;
    public List<(string, float)> originSkillKey;

    public void ChangePercent(string id, float percent)
    {
        if (SkillDict.ContainsKey(id))
        {
            var m = new List<(string, float)>();
            SkillDict[id] *= 1 + percent;
            foreach (var keyValuePair in SkillDict)
            {
                m.Add((keyValuePair.Key, keyValuePair.Value));
            }

            Init(m);
        }
    }

    public SkillPool(List<(string, float)> skills)
    {
        Init(skills);
        originSkillKey = skills;
    }

    public List<string> protectObjects;

    public SkillPool AddProtectContent(List<string> keys)
    {
        protectObjects = keys;
        return this;
    }

    public SkillPool SetProtectCount(int c)
    {
        protectCount = c;

        return this;
    }

    public void Init(List<(string, float)> skills)
    {
        SkillDict = new Dictionary<string, float>();

        for (var i = 0; i < skills.Count; i++)
        {
            SkillDict.Add(skills[i].Item1, skills[i].Item2);
        }

        var n = SkillDict.Keys.ToList();
        var total = 0f;
        skillRandomPool = new List<SkillRollNumber>();
        //total作为抽卡总值，roll其中的数值。
        for (var i = 0; i < n.Count; i++)
        {
            //我们规定 大于等于起始值，小于结束值 即抽中该卡
            var xx = new SkillRollNumber();
            xx.start = total;
            xx.end = total + SkillDict[n[i]];
            xx.key = n[i];
            total += SkillDict[n[i]];
            skillRandomPool.Add(xx);
        }

        totalRollKey = total;
        playerProtect = new Dictionary<string, int>();
    }

    public string RollOnce(string playerID)
    {
        if (!playerProtect.ContainsKey(playerID))
        {
            playerProtect.Add(playerID, 0);
        }
        else
        {
            playerProtect[playerID] = playerProtect[playerID] + 1;
        }

        if (playerProtect[playerID] >= protectCount)
        {
            playerProtect[playerID] = 0;

            if (protectObjects != null && protectObjects.Count > 0)
            {
                var r = Random.Range(0, protectObjects.Count);
                return protectObjects[r];
            }
        }

        var b = Random.Range(0f, totalRollKey);

        for (var i = 0; i < skillRandomPool.Count; i++)
        {
            if (b >= skillRandomPool[i].start && b < skillRandomPool[i].end)
            {
                return skillRandomPool[i].key;
            }
        }

        Debug.LogError("RollFailed??");
        return skillRandomPool[0].key;
    }


    public float totalRollKey;
}

public class SkillRollNumber
{
    public string key;
    public float start;
    public float end;
}