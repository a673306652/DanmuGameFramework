using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bloc
{
    public double Coin;

    public double EXP;

    public int level;

    public double total;
    
    private float intTicker;

    public PropertiesMediator pm;

    public RollPercent CallPercent;
    public RollPercent CardPercent;
    public float SLMPercent = 1;

    public class RollPercent
    {
        public float white = 1;
        public float green = 1;
        public float blue = 1;
        public float purple = 1;
        public float orange = 1;
    }
    
    public virtual int GetBuyCoin()
    {
        switch (level)
        {
            case 0:
                return 2;
            case 1:
                return 4;
            case 2:
                return 32;
            case 3:
                return 64;
            case 4:
                return 127;
            case 5:
                return 256;
            case 6:
                return 512;
            case 7:
                return 1024;
            case 8:
                return 1024;
        }

        return 0;
    }

    public double GetCoinPerLevel()
    {
        switch (level)
        {
            case 0:
                return 2;
            case 1:
                return 4;
            case 2:
                return 8;
            case 3:
                return 16;
            case 4:
                return 32;
            case 5:
                return 64;
            case 6:
                return 128;
            case 7:
                return 256;
        }

        return 0;
    }

    public double GetNextLevelEXP()
    {
        switch (level)
        {
            case 0:
                return 200;

            case 1:
                return 400;
            case 2:
                return 3200;
            case 3:
                return 6400;
            case 4:
                return 12800;
            case 5:
                return 25600;
            case 6:
                return 51200;
            case 7:
                return 102400;
        }

        return 99999999999999;
    }

    //后面阵营的getter都写这里

    protected string Key;

    public string GetKey()
    {
        return Key;
    }

    protected List<UnitAI> blocActors;

    public List<UnitAI> GetActors()
    {
        var m = blocActors.ToArray().ToList();
        return m;
    }

    public string autoCreateKey;
    public float autoCreatePerTime;

    private float autoCreateTick;

    public double GetBuyEXP()
    {
        if (GetNextLevelEXP() > 0)
        {
            return Mathf.Floor((int)GetNextLevelEXP() / 20f);
        }

        return 0;
    }

    public void BuyExp()
    {
        var cast = GetBuyEXP();
        if (Coin > cast)
        {
            Coin -= cast;
            GetExp(cast);
        }
    }

    public void GetExp(double exp)
    {
        this.EXP += exp;

        if (EXP >= GetNextLevelEXP())
        {
            EXP -= GetNextLevelEXP();
            level += 1;
            OnLevelUp();
        }
    }

    public void OnLevelUp()
    {
        switch (level)
        {
            case 0:
                autoCreateTick = 2;
                break;

            case 1:
                autoCreateTick = 1.8f;
                break;
            case 2:
                autoCreateTick = 1.7f;
                break;
            case 3:
                autoCreateTick = 1.6f;
                break;
            case 4:
                autoCreateTick = 1.4f;
                break;
            case 5:
                autoCreateTick = 1.3f;
                break;
            case 6:
                autoCreateTick = 1.2f;
                break;
            case 7:
                autoCreateTick = 1.1f;
                break;
            case 9:
                autoCreateTick = 1f;
                break;
        }
    }

    public virtual void UpdateBloc(float delta)
    {
        intTicker += delta;
        if (intTicker >= 0.3f)
        {
            intTicker = 0;
            Coin += GetCoinPerLevel();
        }

        if (autoCreatePerTime > 0)
        {
            autoCreateTick += delta;
            if (autoCreateTick >= autoCreatePerTime)
            {
                CreateOneActor();
                autoCreateTick = 0;
            }
        }

        pm.OnUpdate(delta);
    }

    protected virtual void CreateOneActor()
    {
        
    }

    public void Init(string key, float autoCreateTimeBase)
    {
        this.Key = key;
        blocActors = new List<UnitAI>();
        pm = new PropertiesMediator();
        pm.Init();
        autoCreatePerTime = autoCreateTimeBase;
        SLMPercent = 0;
        OnInit();
    }

    protected virtual void OnInit()
    {
        
    }

    public virtual void OnActorDie(UnitAI actor)
    {
        blocActors.Remove(actor);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="id"></param>
    /// <param name="attackState"></param>
    /// <param name="skinID"></param>
    /// <param name="baseATK"></param>
    /// <param name="baseDef"></param>
    /// <param name="baseHp"></param>
    /// <param name="baseMoveSpeed"></param>
    /// <param name="baseAttackRange"></param>
    public void CreateOneActorCustom(Vector3 pos, string id, UnitAI.BaseAttackState attackState, int skinID,
        double baseATK,
        double baseDef,
        double baseHp,
        float baseMoveSpeed, float baseAttackRange, float baseAttackSpeed, float price,string customID)
    {
        var ai = HisaoResourcesPool.Instance.Load("Prefabs/Actor/" + id, 100);
        if (null != ai)
        {
            ai.transform.position = pos;
            var agent = ai.GetComponent<UnitAI>();
            agent.Init(this, customID, attackState, skinID, baseATK, baseDef, baseHp, baseMoveSpeed, baseAttackRange,
                baseAttackSpeed, price);
            blocActors.Add(agent);
        }
    }

 
}