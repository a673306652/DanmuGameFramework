using System;
using System.Collections;
using System.Collections.Generic;
using Hisao;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[ClassAvatar("Icon_Editor_Human", "基础控制单位")]
//一会加个标签,我找找看
public class UnitAI : HisaoMono
{
    protected string myID;
    public Bloc myBloc;

    //Skin and Anim

    [SerializeField] public Animator myAnim;
    [SerializeField] protected SkinAgent ActiveSkin;
    [TabGroup("引用类")] [SerializeField] protected SkinAgentMediator SAM;

    //Other static reference

    [TabGroup("引用类")] [SerializeField] public EffectPool effects;
    [TabGroup("引用类")] [SerializeField] protected Image hpFiller;

    [TabGroup("引用类")] [SerializeField] protected TalkBubble talkBubble;

    //States
    [SerializeField] public AIState myState;
    protected BaseAttackState myAttackState;

    //dynamic reference

    [SerializeField] protected UnitAI targetEnemy;
    protected float attackTick;
    protected float targetEnemyRange = 6f;
    public UnitSkillMediator USM;
    public BuffManager BM;


    #region Properties

    //propertiesBase

    protected PropertiesMediator PM;

    public double _AttackBase;
    public double _AttackSpeedBase;
    public double _HPBase;
    public double _BaseDef;
    public float _BaseMoveSpeed;
    public double ShieldValue;

    public double _AttackBaseAdd;
    public double _AttackSpeedBaseAdd;
    public double _HPBaseAdd;
    public double _BaseDefAdd;
    public float _BaseMoveSpeedAdd;

    protected float _AttackRange = 1f;

    protected double price;

    public bool isStatic;

    //propertiesGetter
    public double _MaxHP;
    public double _CurrentHP;

    public virtual double _GetMoveSpeed()
    {
        return _BaseMoveSpeed + _BaseMoveSpeedAdd + PM.GetSpeed() + myBloc.pm.GetSpeed();
    }

    public virtual double _GetOriginHP()
    {
        var m = _HPBase + _HPBaseAdd;
        m += PM.GetAdditionHP() + myBloc.pm.GetAdditionHP();
        m = m * (1 + PM.GetHPMul() + myBloc.pm.GetHPMul());
        return m;
    }

    public virtual double _AttackSpeed()
    {
        var originSP = _AttackSpeedBase + _AttackSpeedBaseAdd;

        var addSP = PM.GetAttackSpeedMul() + myBloc.pm.GetAttackSpeedMul();

        var right = (1 - (addSP / (addSP + 1f)));
        if (addSP <= 0)
        {
            right = 1 + Mathf.Abs((float)addSP);
        }

        var fianl = originSP * right;


        var s = Mathf.Clamp((float)fianl, 0.02f, 111);
        return s;
    }

    public virtual double _GetCurrentAtk()
    {
        var originATK = _AttackBase + _AttackBaseAdd +
                        (_GetOriginHP() / 100) * (PM.GetHPTATK() + myBloc.pm.GetHPTATK());
        originATK += PM.GetAdditionAttack();
        originATK += myBloc.pm.GetAdditionAttack();
        originATK *= (1 + PM.GetAttackMul() + myBloc.pm.GetAttackMul());
        originATK *= (1 + PM.GetDMGMul() + myBloc.pm.GetDMGMul());

        return originATK;
    }

    public virtual double _GetCurrentDef()
    {
        var originDef = _BaseDef + _BaseDefAdd;
        originDef += PM.GetAdditionDef() + myBloc.pm.GetAdditionDef();
        originDef *= (1 + PM.GetDefMul() + myBloc.pm.GetDefMul());
        return originDef;
    }

    #endregion

    //理论上的怪物加载应该基于表进行来的
    //我去把表结构也做了?算了直接写死吧先
    public void Init(Bloc myBloc, string id, BaseAttackState attackState, int skinID, double baseATK, double baseDef,
        double baseHp,
        float baseMoveSpeed, float baseAttackRange, float BaseAttackSpeed, float price)
    {
        this.myBloc = myBloc;
        PM = new PropertiesMediator();
        PM.Init(this);
        USM = new UnitSkillMediator();
        USM.Init(this);
        BM = new BuffManager();
        BM.Init(this);
        myAttackState = attackState;
        myID = id;
        this._AttackBase = baseATK;
        this._BaseDef = baseDef;
        this._HPBase = baseHp;
        this._BaseMoveSpeed = baseMoveSpeed;
        this._AttackRange = baseAttackRange;
        this._AttackSpeedBase = BaseAttackSpeed;
        this._CurrentHP = _GetOriginHP();
        this._MaxHP = _GetOriginHP();
        _AttackBaseAdd = 0;
        _BaseDefAdd = 0;
        _AttackSpeedBaseAdd = 0;
        _BaseMoveSpeedAdd = 0;
        _HPBaseAdd = 0;
        this.price = price;
        myState = AIState.Idle;

        if (PM.GetShield() + this.myBloc.pm.GetShield() > 0)
        {
            ShieldValue = _GetOriginHP() * (PM.GetShield() + this.myBloc.pm.GetShield());
        }

        SetSKin(skinID);
        InterruptMoveTask();
    }

    protected void SetSKin(int id)
    {
        ActiveSkin = SAM.GetSkinByID(id);
        myAnim = ActiveSkin.Anim;
        SAM.OpenSkin(id);
    }


    private void Update()
    {
        var delta = Time.deltaTime * GLOBAL_TIME * LOCAL_TIME;
        ProcessState(delta);
        UpdateUI(delta);
        UpdateAnim();
        USM.OnUpdate(delta);
        PM.OnUpdate(delta);
        BM.UpdateBuffs(delta);
        if (ShieldValue > 0)
        {
             ShieldObject.gameObject.SetActive(true);
        }
        else
        {
            ShieldObject.gameObject.SetActive(false);
        }
    }

    [SerializeField] public GameObject ShieldObject;

    protected virtual float GetMovePowerBySpeed()
    {
        var baseValue = 0.5f;

        baseValue += (float)_GetMoveSpeed() / 5f;
        return baseValue;
    }

    protected virtual void UpdateAnim()
    {
        myAnim.SetFloat("AttackSpeed", (float)(_AttackSpeedBase / _AttackSpeed()));
        switch (myState)
        {
            case AIState.Moving:
                myAnim.SetFloat("WalkPower", Mathf.Lerp(myAnim.GetFloat("WalkPower"), GetMovePowerBySpeed(), 0.1f));
                break;
            default:
                myAnim.SetFloat("WalkPower", Mathf.Lerp(myAnim.GetFloat("WalkPower"), 0f, 0.1f));
                break;
        }
    }

    protected virtual void UpdateUI(float delta)
    {
        if (null != hpFiller)
        {
            hpFiller.transform.parent.gameObject.SetActive(!myState.Equals(AIState.Die));
            hpFiller.fillAmount = (float)(_CurrentHP / _MaxHP);
            if (hpFiller.GetComponent<TextMeshProUGUI>())
            {
                hpFiller.GetComponent<TextMeshProUGUI>().text = _CurrentHP + "/" + _MaxHP;
            }
        }
    }

    protected virtual void ProcessState(float delta)
    {
        if (isStatic)
        {
            return;
        }
        if (null == targetEnemy)
        {
            TryFindEnemy();
            //会出现没有敌人的散步情况， 应该是持续索敌
            //根据是否有敌人的情况处理状态
        }
        else
        {
            if (null != targetEnemy)
            {
                if (targetEnemy.myState.Equals(AIState.Die))
                {
                    targetEnemy = null;
                }
            }
        }

        TickAttackTime(delta);


        switch (myState)
        {
            case AIState.Idle:
                if (null != targetEnemy)
                {
                    //攻击结束后的闲置状态，如果暂时等待攻击状态，判断是否需要移动，如果需要移动就移动
                    if (Vector3.Distance(targetEnemy.transform.position, transform.position) <
                        _AttackRange - _AttackRange * 0.1f)
                    {
                        //进入攻击范围了，不移动了
                        InterruptMoveTask();
                        myState = AIState.TryAttack;
                        break;
                    }
                }

                myState = AIState.Move;
                //如果没有敌人，且在待机，就转移动

                break;
            case AIState.OnSkill:
                break;
            case AIState.Move:
                //移动应该分有敌人和没敌人的两种情况
                if (null == targetEnemy)
                {
                    //随机往自己的一个圆环点移动吧
                    var p = new Vector3[]
                    {
                        this.transform.position,
                        (transform.position + RandomPointHelper.RandomPointOnCircle((float)_GetMoveSpeed() * 3f))
                    };
                    DoMove(p, 3f, () => { myState = AIState.Idle; });
                    myState = AIState.Moving;
                }
                else
                {
                    //向敌人方向的攻击范围移动，如果足够靠近敌人就会自动中断这次移动
                    //如果移动结束后还不达到攻击范围，则继续移动
                    var targetPoint = (targetEnemy.transform.position);
                    //目标距离
                    var totaldis = Vector3.Distance(this.transform.position, targetPoint
                    );


                    bool fullMove = (float)_GetMoveSpeed() * 0.5f > totaldis;

                    var p = new Vector3[]
                    {
                        this.transform.position,
                        this.transform.position + (targetPoint - transform.position).normalized *
                        (fullMove ? totaldis : (float)_GetMoveSpeed() * 0.5f)
                    };
                    //那这里就也分段走，不走到底
                    //这个一定是直线运动，所以可以以自己的速度乘时间/总距离算出来应该移动多少码
                    DoMove(p, fullMove ? totaldis / (float)_GetMoveSpeed() : 0.5f,
                        () => { myState = AIState.Idle; });
                    myState = AIState.Moving;
                }

                break;
            case AIState.Moving:
                //移动中每帧都会检测，这里做判定
                if (targetEnemy != null)
                {
                    if (Vector3.Distance(targetEnemy.transform.position, transform.position) <
                        _AttackRange - _AttackRange * 0.1f)
                    {
                        //进入攻击范围了，不移动了
                        InterruptMoveTask();
                        myState = AIState.TryAttack;
                    }
                }

                break;
            case AIState.TryAttack:
                if (attackTick <= 0)
                {
                    if (targetEnemy != null && targetEnemy.myState != AIState.Die)
                    {
                        Attack();
                        myAnim.SetTrigger("Attack");
                        attackTick = (float)_AttackSpeed();

                        InterruptMoveTask();
                        myState = AIState.WaitingAttack;
                        USM.OnAttack();
                        PM.OnAttack();
                    }
                    else
                    {
                        myState = AIState.Idle;
                    }
                }
                else
                {
                    myState = AIState.Idle;
                }

                break;
            case AIState.WaitingAttack:
                if (attackTick <= 0)
                {
                    myState = AIState.Idle;
                }

                if (null != targetEnemy)
                {
                    if (currentMoveTask == null)
                    {
                        SAM.transform.forward = Vector3.Lerp(SAM.transform.forward,
                            (targetEnemy.transform.position - transform.position).normalized, 0.2f);
                    }
                }

                break;
            case AIState.Die:

                break;
        }
    }

    protected virtual void TickAttackTime(float delta)
    {
        if (attackTick > 0)
        {
            attackTick -= delta;
        }
        else
        {
            attackTick = 0;
        }
    }

    protected virtual void ProcessAtkCoin(double dmg)
    {
        if (PM.GetCoinPerAttack() + myBloc.pm.GetCoinPerAttack() > 0)
        {
            var x = Mathf.Clamp((int)(PM.GetCoinPerAttack() + myBloc.pm.GetCoinPerAttack()), 0, 99999);
            myBloc.Coin += x;
        }
    }

    protected virtual void ProcessToxic(double dmg)
    {
        if (PM.GetToxicAtk() + myBloc.pm.GetToxicAtk() > 0)
        {
            var d = dmg * (PM.GetToxicAtk() + myBloc.pm.GetToxicAtk());
            targetEnemy.BM.RegistDot((float)d, 10f, myBloc.GetKey(), "T", this);

            if (PM.GetToxicAtk2() + myBloc.pm.GetToxicAtk2() > 0)
            {
                var dotPool = targetEnemy.BM.cDots[myBloc.GetKey()].DotDmg;
                var dd = dotPool * PM.GetToxicAtk2() + myBloc.pm.GetToxicAtk2();
                targetEnemy.OnHit(dd, this);
                DNTest.Instance.t(transform.position, "毒爆<color=#00ff00>" + dd.ToString("F0") + "</color>");
            }

            if (PM.GetToxicHealing() + myBloc.pm.GetToxicHealing() > 0)
            {
                var dotPool = targetEnemy.BM.cDots[myBloc.GetKey()].DotDmg;
                var dd = dotPool * PM.GetToxicHealing() + myBloc.pm.GetToxicHealing();
                this.Healing(dd, 0, false);
                DNTest.Instance.t(transform.position, "毒性营养<color=#00ff00>" + dd.ToString("F0") + "</color>");
            }
        }
    }

    protected virtual void Attack()
    {
        var dmg = _GetCurrentAtk();

        if (PM.GetAdditionDoubleDMG() + myBloc.pm.GetAdditionDoubleDMG() > 0)
        {
            var seed = Random.Range(0, 1f);
            if (seed < PM.GetAdditionDoubleDMG() + myBloc.pm.GetAdditionDoubleDMG())
            {
                dmg *= 2f;
            }
        }

        double dmged = 0;

        switch (myAttackState)
        {
            case BaseAttackState.Near:

                //近战
                //直接造成伤害吧先

                if (null != targetEnemy)
                {
                    dmged = targetEnemy.OnHit(dmg, this);
                }

                break;
            case BaseAttackState.Far:
                //远程
                Exec((a) =>
                {
                    if (null != targetEnemy)
                    {
                        dmged = targetEnemy.OnHit(dmg, this);
                    }
                }, 0.3f);

                break;
        }


        if (dmged > 0)
        {
            if (PM.GetATKHealing() > 0)
            {
                var h = dmged * PM.GetATKHealing();
                Healing(h, 0, false);
            }
        }

        ProcessToxic(dmged);
        ProcessAtkCoin(dmged);
    }

    protected virtual void TryFindEnemy()
    {
        
        //这里负责索敌
        
        //新项目应该有其他规则

        targetEnemy = null;
    }

    protected virtual void OnKillEnemy()
    {
    }

    protected virtual void SupportDPS(double dmg)
    {
        myBloc.total += dmg;
        //这里是直接给总的阵营加总伤，弹幕模块应该是以player为单位
    }

    protected virtual double ProcessDMG(double dmg)
    {
        var def = _GetCurrentDef();

        dmg -= def;
        dmg = Mathf.Clamp((float)dmg, 0, 9999999);
        //这个项目是防御固定减伤，下面注释的这段是百分比减伤的公式噢
        //
        // var present = def / ((def + 100));
        // if (def == 0)
        // {
        //     present = 0;
        // }

        return dmg;
//        return dmg * (1f - present);
    }

    protected virtual void ProcessShieldDMG(double dmg, UnitAI dmgFrom)
    {
        if (this.PM.GetShieldDMG() > 0)
        {
            dmgFrom.OnHit(_GetCurrentAtk() * PM.GetShieldDMG(), this);
            DNTest.Instance.t(dmgFrom.transform.position,
                $"<color=#888888>盾反:-{(_GetCurrentAtk() * PM.GetShieldDMG()).ToString("F1")}</color>");
        }
    }

    public virtual double OnHit(double dmg, UnitAI dmgFrom)
    {
        if (myState.Equals(AIState.Die))
        {
            return 0;
        }

        PM.OnBeHited();
        USM.OnBeHited();

        if (ShieldValue > 0)
        {
            var cast = Mathf.Min((float)dmg, (float)ShieldValue);
            ShieldValue -= cast;
            dmg -= cast;
            if (dmg <= 0)
            {
                dmg = 0;
            }
        }

        ProcessShieldDMG(dmg, dmgFrom);
        dmg = ProcessDMG(dmg);
        _CurrentHP -= dmg;
        dmgFrom.SupportDPS(dmg);
        if (!myID.Equals("hero"))
        {
            DNTest.Instance.t(transform.position, $"-{dmg.ToString("F1")}");
        }
        else
        {
            DNTest.Instance.t2(transform.position, $"-{dmg.ToString("F1")}");
        }

        if (ActiveSkin.GetComponent<MeshShining>())
        {
            ActiveSkin.GetComponent<MeshShining>().Shining();
        }

        effects.Play(EffectID.HealingEffect);

        if (_CurrentHP <= 0)
        {
            dmgFrom.OnKillEnemy();
            OnDead();
        }


        return dmg;
    }

    public double totalDMG;

    // public void ClearDPS()
    // {
    //     totalDMG = 0;
    // }

    public virtual void ForceDead()
    {
        InterruptMoveTask();
        myState = AIState.Die;
        myBloc.OnActorDie(this);
        USM.OnDead();
        PM.OnDead();
        myAnim.Play("Dead");
        SuperGC.Instance.EnqueueShitGC(gameObject.transform.parent.name, gameObject, 0f);
    }
    protected virtual void OnDead()
    {
        if (myState == AIState.Die)
        {
            return;
        }

        InterruptMoveTask();
        myState = AIState.Die;
        myBloc.OnActorDie(this);
        USM.OnDead();
        PM.OnDead();

        var p = price + PM.GetDeadPerCoin() + myBloc.pm.GetDeadPerCoin();

        myBloc.Coin += p;

        if (myID.Equals("GL"))
        {
            if (PM.GetGL() + myBloc.pm.GetGL() < 0)
            {
                var mm = myBloc.GetActors();

                for (var i = 0; i < mm.Count; i++)
                {
                    mm[i]._AttackBaseAdd += _AttackBase * PM.GetGL() + myBloc.pm.GetGL();
                    mm[i]._BaseDefAdd += _BaseDef * PM.GetGL() + myBloc.pm.GetGL();
                    mm[i]._AttackSpeedBaseAdd += _AttackSpeedBase * PM.GetGL() + myBloc.pm.GetGL();
                    mm[i]._BaseMoveSpeedAdd += _BaseMoveSpeed * (float)(PM.GetGL() + myBloc.pm.GetGL());
                    mm[i]._HPBaseAdd += _HPBase * PM.GetGL() + myBloc.pm.GetGL();
                    mm[i].Healing(_HPBase * PM.GetGL() + myBloc.pm.GetGL(), 0, true);
                }
            }
        }

        if (PM.GetDeadDMG() + myBloc.pm.GetDeadDMG() > 0)
        {
            LayerMask l = 1 << 0;

            var x = Physics.OverlapSphere(transform.position, 4f, l);

            if (x.Length > 0)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i].GetComponent<UnitAI>())
                    {
                        x[i].GetComponent<UnitAI>().OnHit(_GetOriginHP() * (PM.GetDeadDMG() + myBloc.pm.GetDeadDMG()),
                            this);
                    }
                }
            }
        }

        if (PM.GetDeadDMG2() + myBloc.pm.GetDeadDMG2() > 0)
        {
            LayerMask l = 1 << 0;

            var x = Physics.OverlapSphere(transform.position, 4f, l);

            if (x.Length > 0)
            {
                for (int i = 0; i < x.Length; i++)
                {
                    if (x[i].GetComponent<UnitAI>())
                    {
                        x[i].GetComponent<UnitAI>().OnHit(
                            _GetCurrentAtk() * (PM.GetDeadDMG2() + myBloc.pm.GetDeadDMG2()),
                            this);
                    }
                }
            }
        }

        myAnim.Play("Dead");
        SuperGC.Instance.EnqueueShitGC(gameObject.transform.parent.name, gameObject, 2f);
    }

    [Button()]
    protected void TestTalk()
    {
        Talk("哈哈");
    }
    protected void Talk(string msg)
    {
        if (null != talkBubble)
        {
            talkBubble.RequestBubble(msg);
        }
    }

    public virtual void Healing(double value, float percent, bool effect = true)
    {
        var castValue = _MaxHP - _CurrentHP;
        var healingValue = Mathf.Min((float)value + (float)_MaxHP * percent, (float)castValue);
        if (healingValue > 0)
        {
            _CurrentHP += healingValue; 
            if (effect)
            {
                effects.Play(4);
            }
        }
    }

    protected virtual void Respawn()
    {
    }

    protected Coroutine currentMoveTask;

    public void DoMove(Vector3[] path, float t, UnityAction oc = null)
    {
        currentMoveTask = StartCoroutine(MoveTast(path, t, oc));
    }

    public virtual void InterruptMoveTask()
    {
        if (null != currentMoveTask)
        {
            StopCoroutine(currentMoveTask);
            currentMoveTask = null;
        }
    }

    protected IEnumerator MoveTast(Vector3[] path, float t, UnityAction oc)
    {
        var m = 0f;

        var cIndex = 0;
        var totalDistannce = 0f;

        for (var i = 0; i < path.Length; i++)
        {
            if (i < path.Length - 1)
            {
                totalDistannce += Vector3.Distance(path[i], path[i + 1]);
            }
        }

        while (cIndex < path.Length - 1)
        {
            var targetDistance = Vector3.Distance(path[cIndex], path[cIndex + 1]);
            var targetTime = (targetDistance / totalDistannce) * t;
            m = 0;
            while (m < targetTime)
            {
                m += Time.fixedDeltaTime * GLOBAL_TIME * LOCAL_TIME;
                transform.position = Vector3.Lerp(path[cIndex], path[cIndex + 1], m / targetTime);
                SAM.transform.forward = Vector3.Lerp(SAM.transform.forward,
                    (path[cIndex + 1] - path[cIndex]).normalized, 0.2f);
                USM.OnMove();
                PM.OnMove();
                yield return new WaitForFixedUpdate();
            }

            cIndex++;
        }

        currentMoveTask = null;
        oc?.Invoke();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var c = Color.blue;
        c.a = 0.2f;
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, targetEnemyRange);
        c = Color.red;
        c.a = 0.2f;
        Gizmos.color = c;
        Gizmos.DrawSphere(transform.position, _AttackRange);
    }
#endif


    public enum AIState
    {
        Idle,
        Move,
        OnSkill,
        TryAttack,
        WaitingAttack,
        Die,
        Moving
    }

    public enum BaseAttackState
    {
        Near,
        Far
    }
}

public class PropertiesMediator
{
    public List<UnitProperties> properties;
    public UnitAI owner;

    public void Init(UnitAI owner = null)
    {
        properties = new List<UnitProperties>();
        this.owner = owner;
    }

    public void AddProperties(UnitProperties p)
    {
        properties.Add(p);
        p.OnGetSkill?.Invoke(owner);
    }

    public void Clear()
    {
        if (null != properties)
        {
            properties.Clear();
        }
    }

    public double GetCoinPerAttack()
    {
        var m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].GetCoinPerAttack;
        }

        return m;
    }

    public double GetAttackMul()
    {
        var m = 0f;
        for (int i = 0; i < properties.Count; i++)
        {
            m += (float)properties[i].AttackMul;
        }

        return m;
    }

    public double GetSlimAdditionPercent()
    {
        var m = 0f;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].SlimAdditionPercent;
        }

        return m;
    }

    public float GetAdditionDoubleDMG()
    {
        var m = 0f;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].AdditionDoubleDMG;
        }

        return m;
    }

    public double GetDefMul()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].DefMul;
        }

        return m;
    }

    public double GetHPMul()
    {
        double m = 0f;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].HPMul;
        }

        return m;
    }

    public double GetAdditionDef()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].AdditionDef;
        }

        return m;
    }

    public double GetAdditionHP()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].AdditionHP;
        }

        return m;
    }

    public double GetAdditionAttack()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].AdditionAttack;
        }

        return m;
    }

    public double GetDeadDMG()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].DeadDMG;
        }

        return m;
    }

    public double GetSpecialCall()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].SpecialCall;
        }

        return m;
    }

    public double GetDMGMul()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].DMGMul;
        }

        return m;
    }

    public double GetAttackSpeedMul()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].AttackSpeedMul;
        }

        return m;
    }

    public double GetShieldDMG()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].ShieldDMG;
        }

        return m;
    }

    public double GetATKHealing()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].ATKHealing;
        }

        return m;
    }

    public double GetToxicAtk()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].ToxicAttack;
        }

        return m;
    }

    public double GetToxicHealing()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].ToxicHeal;
        }

        return m;
    }

    public double GetToxicAtk2()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].ToxicAttack2;
        }

        return m;
    }

    public double GetDeadDMG2()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].DeadDMG2;
        }

        return m;
    }


    public double GetSpeed()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].speed;
        }

        return m;
    }

    public double GetDeadPerCoin()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].DeadPerCoin;
        }

        return m;
    }

    public double GetGL()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].GLProperties;
        }

        return m;
    }

    public double GetYouHui()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].YouHui;
        }

        return m;
    }

    public double GetShield()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].Shield;
        }

        return m;
    }

    public double GetHPTATK()
    {
        double m = 0;
        for (int i = 0; i < properties.Count; i++)
        {
            m += properties[i].HPTransformATK;
        }

        return m;
    }

    public void OnDead()
    {
        for (var i = 0; i < properties.Count; i++)
        {
            properties[i].OnDead?.Invoke(owner);
        }
    }

    public void OnAttack()
    {
        for (var i = 0; i < properties.Count; i++)
        {
            properties[i].OnAttack?.Invoke(owner);
        }
    }

    public void OnBeHited()
    {
        for (var i = 0; i < properties.Count; i++)
        {
            properties[i].OnBeHited?.Invoke(owner);
        }
    }

    public void OnMove()
    {
        for (var i = 0; i < properties.Count; i++)
        {
            properties[i].OnMove?.Invoke(owner);
        }
    }

    public void OnUpdate(float delta)
    {
        for (var i = 0; i < properties.Count; i++)
        {
            properties[i].OnUpdate?.Invoke(owner, delta);
        }
    }
}

public class UnitProperties
{
    public int GetCoinPerAttack = 0;
    public float AttackMul = 0;
    public float SlimAdditionPercent = 0;
    public float AdditionDoubleDMG = 0f;
    public double DefMul = 0;
    public double HPMul = 0;
    public double AdditionDef = 0;
    public double AdditionHP = 0;
    public double AdditionAttack = 0;
    public double DeadDMG = 0;
    public double DeadDMG2 = 0;
    public double SpecialCall = 0;
    public double DMGMul = 0;
    public double AttackSpeedMul = 0;
    public double ShieldDMG = 0;
    public float ATKHealing = 0;
    public float ToxicAttack = 0;
    public float ToxicHeal = 0;
    public float ToxicAttack2 = 0;
    public float speed = 0;
    public double DeadPerCoin = 0;
    public double GLProperties = 0;
    public double YouHui = 0;
    public double Shield = 0;
    public double HPTransformATK = 0;
    public UnityAction<UnitAI> OnDead;
    public UnityAction<UnitAI> OnGetSkill;
    public UnityAction<UnitAI> OnAttack;
    public UnityAction<UnitAI> OnBeHited;
    public UnityAction<UnitAI> OnMove;
    public UnityAction<UnitAI, float> OnUpdate;
}

public class UnitSkillMediator
{
    public List<UnitSkill> skills;
    public UnitAI owner;

    public void AddSkill(UnitSkill skill)
    {
        skills.Add(skill);
        skill.OnGetSkill?.Invoke(owner);
    }

    public void Init(UnitAI owner)
    {
        skills = new List<UnitSkill>();
        this.owner = owner;
    }

    public void OnDead()
    {
        for (var i = 0; i < skills.Count; i++)
        {
            skills[i].OnDead?.Invoke(owner);
        }
    }

    public void OnAttack()
    {
        for (var i = 0; i < skills.Count; i++)
        {
            skills[i].OnAttack?.Invoke(owner);
        }
    }

    public void OnBeHited()
    {
        for (var i = 0; i < skills.Count; i++)
        {
            skills[i].OnBeHited?.Invoke(owner);
        }
    }

    public void OnMove()
    {
        for (var i = 0; i < skills.Count; i++)
        {
            skills[i].OnMove?.Invoke(owner);
        }
    }

    public void OnUpdate(float delta)
    {
        for (var i = 0; i < skills.Count; i++)
        {
            skills[i].OnUpdateCD?.Invoke(owner, delta);
            skills[i].OnUpdate?.Invoke(owner, delta);
        }
    }
}

public class UnitSkill
{
    public UnityAction<UnitAI> OnDead;
    public UnityAction<UnitAI> OnGetSkill;
    public UnityAction<UnitAI> OnAttack;
    public UnityAction<UnitAI> OnBeHited;
    public UnityAction<UnitAI> OnMove;
    public UnityAction<UnitAI, float> OnUpdateCD;
    public UnityAction<UnitAI, float> OnUpdate;

    public float temp;
    public float temp2;
    public int temp3;
    public float cd;
}