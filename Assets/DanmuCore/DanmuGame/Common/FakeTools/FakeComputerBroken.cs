using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class FakeComputerBroken : MonoBehaviour
{
    public Vector2Int value;
    public bool cancel = false;
    private Coroutine co;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("j"))
        {
            //ע�⣬��һ�¾�ֱ�ӿ����ˣ���������װ��ɱ����
            //Thread.Sleep(10000000);  ���У��߳����߲���δ��Ӧ��δ��Ӧ��ˬ
            while (true)
            {

            }
        }
        if (Input.GetKeyDown("k"))
        {
            this.cancel = false;
            this.value *= 2;
            if (this.co == null)
            {
                this.co = StartCoroutine(this.Ka());
            }
        }



        if (Input.GetKeyDown("l"))
        {
            this.cancel = true;
        }
    }

    IEnumerator Ka()
    {
        while (true)
        {
            if (this.cancel)
            {
                this.cancel = false;
                StopAllCoroutines();
                this.co = null;
            }
            Thread.Sleep(Random.Range(this.value.x, this.value.y));
            yield return 5;
        }
        yield return null;
    }
}
