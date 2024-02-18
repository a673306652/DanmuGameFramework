using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class BuffManager
{
    public Dictionary<string, BuffAgent> buffsDict;
    public UnitAI owner;
    public PropertiesMediator pm;
    public Dictionary<string, DotAgent> cDots;

    public void Init(UnitAI owner)
    {
        this.owner = owner;
        buffsDict = new Dictionary<string, BuffAgent>();
        pm = new PropertiesMediator();
        cDots = new Dictionary<string, DotAgent>();
        pm.Init();
    }

    private float DotClock;

    public void UpdateBuffs(float delta)
    {
        var x = buffsDict.ToList();
        List<string> removedList = new List<string>();
        foreach (var keyValuePair in x)
        {
            var id = keyValuePair.Key;
            var agent = keyValuePair.Value;
            agent.OnBuffUpdate.Invoke(agent);
            agent.timeTick += delta;
            if (agent.timeTick >= agent.duration)
            {
                removedList.Add(id);
            }
        }

        for (var i = 0; i < removedList.Count; i++)
        {
            RemoveBuff(removedList[i]);
        }


        var keyss = cDots.Keys.ToList();

        DotClock += delta;

        if (DotClock >= 0.1f)
        {
            DotClock = 0;
            for (int i = 0; i < keyss.Count; i++)
            {
                var pc = cDots[keyss[i]];
                var currentDotDMG = Mathf.Min(pc.DotDmg * (0.1f / pc.duration), pc.DotDmg);
                pc.DotDmg -= currentDotDMG;
                owner.OnHit(currentDotDMG, pc.DotDmgForm);
                if (null != pc.DotDmgForm)
                {
                    DNTest.Instance.t(owner.transform.position,
                        "中毒<color=#00ff00>" + currentDotDMG.ToString("F0") + "</color>");
                }

                if (pc.DotDmg <= 0)
                {
                    cDots.Remove(pc.DotFormKey);
                }
            }
        }
    }

    public void AddBuff(UnitAI owner, string buffID, float duration, UnityAction<BuffAgent> onBuffAdd,
        UnityAction<BuffAgent> onBuffUpdate, UnityAction<BuffAgent> onBuffUnload)
    {
        if (buffsDict.ContainsKey(buffID))
        {
            buffsDict[buffID].timeTick = 0;
            return;
        }

        var m = new BuffAgent();
        m.owner = owner;
        m.buffID = buffID;
        m.duration = duration;
        m.OnBuffAdd = onBuffAdd;
        m.OnBuffUpdate = onBuffUpdate;
        m.OnBuffUnload = onBuffUnload;
        m.BuffProperties = new UnitProperties();
        pm.properties.Add(m.BuffProperties);
        buffsDict.Add(buffID, m);
        m.OnBuffAdd.Invoke(m);
    }

    public void RemoveBuff(string id)
    {
        if (buffsDict.ContainsKey(id))
        {
            buffsDict[id].OnBuffUnload.Invoke(buffsDict[id]);
            buffsDict.Remove(id);
        }
    }

    public void RegistDot(float dmg, float duration, string key, string DMGType, UnitAI form)
    {
        if (cDots.ContainsKey(key))
        {
            var pc = cDots[key];
            pc.DotDmg += dmg;
        }
        else
        {
            var d = new DotAgent();
            d.duration = duration;
            d.DMGType = DMGType;
            d.DotDmg = dmg;
            d.DotDmgForm = form;
            d.DotDmgKey = key;
            d.DotFormKey = key;

            cDots.Add(d.DotFormKey, d);
        }
    }
}

public class DotAgent
{
    public string DotDmgKey;
    public float DotDmg;
    public UnitAI DotDmgForm;
    public float duration;
    public string DotFormKey;
    public string DMGType;
}

public class BuffAgent
{
    public string buffID;
    public UnitProperties BuffProperties;
    public float duration;
    public float timeTick;
    public UnitAI owner;
    public UnityAction<BuffAgent> OnBuffAdd;
    public UnityAction<BuffAgent> OnBuffUpdate;
    public UnityAction<BuffAgent> OnBuffUnload;
}