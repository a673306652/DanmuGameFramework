using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public static class SkillModule
{
    public static UnitSkill ShieldAttack()
    {
        var m = new UnitSkill();

        m.cd = 10;
        m.OnUpdateCD = CDEvent(m);
        LayerMask enemyLayer = 1 << 1;
        m.OnAttack = agent =>
        {
            var seed = Random.Range(0, 1f);
            if (seed <= 0.03f)
            {
                if (m.temp2 > 0)
                {
                    return;
                }
                else
                {
                    m.temp2 = 5f;
                    m.temp = 0;
                    agent.effects.Play(1);
                    DNTest.Instance.t(agent.transform.position, "火焰结界！");
                }
            }
        };
        m.OnUpdate = (agent, delta) =>
        {
            if (m.temp2 > 0)
            {
                m.temp2 -= delta;
                if (m.temp2 <= 0)
                {
                    m.temp2 = 0;
                }

                m.temp += delta;

                var p = Physics.OverlapSphere(agent.transform.position, 4.5f, enemyLayer);
                if (p.Length > 0)
                {
                    foreach (var enemy in p)
                    {
                        if (enemy.GetComponent<UnitAI>())
                        {
                            var d = (enemy.transform.position - agent.transform.position).normalized;
                            enemy.transform.position += d * 1.5f * delta;
                        }
                    }
                }

                if (m.temp >= 0.2f)
                {
                    m.temp = 0;
                    if (p.Length > 0)
                    {
                        foreach (var enemy in p)
                        {
                            if (enemy.GetComponent<UnitAI>())
                            {
                                enemy.GetComponent<UnitAI>().OnHit(agent._GetCurrentAtk() * 0.2f, agent);
                            }
                        }
                    }
                }
            }
        };

        return m;
    }

    public static UnitSkill CrushAtk()
    {
        var m = new UnitSkill();


        m.OnUpdateCD = CDEvent(m);
        LayerMask enemyLayer = 1 << 1;
        m.OnAttack = agent =>
        {
            if (m.cd > 0)
            {
                return;
            }

            var seed = Random.Range(0, 1f);
            if (seed <= 0.1f)
            {
                if (m.temp2 > 0)
                {
                    return;
                }
                else
                {
                    LayerMask enemyLayer = 1 << 1;
                    m.cd = 30;
                    //筛选出最多怪物的
                    agent.myState = UnitAI.AIState.OnSkill;
                    var l = new List<(UnitAI, int)>();
                    var lm = new List<UnitAI>();
                    //这里什么都拿不到，因为没写，这里的代码只作为演示
                    for (int i = 0; i < lm.Count; i++)
                    {
                        var x = Physics.OverlapSphere(lm[i].transform.position, 4, enemyLayer);
                        l.Add((lm[i], x.Length));
                    }

                    l.Sort((a, b) => { return b.Item2.CompareTo(a.Item2); });

                    agent.myAnim.SetTrigger("Attack2");
                    agent.InterruptMoveTask();

                    agent.DoMove(new Vector3[] { agent.transform.position, l[0].Item1.transform.position }, 1.8f,
                        () =>
                        {
                            var x = Physics.OverlapSphere(agent.transform.position, 4, enemyLayer);
                            ShakeCamera.Instance.Shake(0.5f, 2f);
                            for (var i = 0; i < x.Length; i++)
                            {
                                if (x[i].GetComponent<UnitAI>())
                                {
                                    x[i].GetComponent<UnitAI>().OnHit(agent._GetCurrentAtk() * 2, agent);
                                }
                            }

                            agent
                                .effects.Play(2);
                        });
                    agent.Exec((a) =>
                    {
                        agent.myState = UnitAI.AIState.Idle;
                        agent.myAnim.Play("Idle");
                    }, 2.1f);
                }
            }
        };


        return m;
    }

    public static UnitSkill Swallow()
    {
        var m = new UnitSkill();


        m.OnUpdateCD = CDEvent(m);
        LayerMask enemyLayer = 1 << 1;
        m.OnAttack = agent =>
        {
            if (m.cd > 0)
            {
                return;
            }

            var seed = Random.Range(0, 1f);
            if (seed <= 1f)
            {
                if (m.temp2 > 0)
                {
                    return;
                }
                else
                {
                    if (agent.myState == UnitAI.AIState.OnSkill)
                    {
                        return;
                    }

                    LayerMask enemyLayer = 1 << 1;

                    //筛选出最多怪物的
                    var x = Physics.OverlapSphere(agent.transform.position, 3f, enemyLayer);


                    if (x.Length > 0)
                    {
                        m.cd = 15;
                        agent.myAnim.SetTrigger("Attack3");
                        agent.InterruptMoveTask();
                        agent.myState = UnitAI.AIState.OnSkill;
                        agent.Exec((a) =>
                        {
                            for (int i = 0; i < x.Length; i++)
                            {
                                if (x[i].GetComponent<UnitAI>())
                                {
                                    if (x[i].GetComponent<UnitAI>().myState != UnitAI.AIState.Die)
                                    {
                                        x[0].GetComponent<UnitAI>().OnHit(99999, agent);
                                        DNTest.Instance.t(x[0].transform.position, "吞噬!");
                                        x[0].GetComponent<UnitAI>().effects.Play(3);
                                        agent.myState = UnitAI.AIState.Idle;
                                        agent.Healing(x[0].GetComponent<UnitAI>()._GetOriginHP(), 0, true);
                                    }
                                    break;
                                }
                            }
                            agent.myState = UnitAI.AIState.Idle;
                        }, 1f);
                    }
                    else
                    {
                    }
                }
            }
        };


        return m;
    }

    public static UnityAction<UnitAI, float> CDEvent(UnitSkill m)
    {
        return (agent, delta) =>
        {
            if (m.cd > 0)
            {
                m.cd -= delta;
            }
            else
            {
                m.cd = 0;
            }
        };
    }
}