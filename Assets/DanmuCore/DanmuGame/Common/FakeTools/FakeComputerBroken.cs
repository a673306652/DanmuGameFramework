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
            //注意，按一下就直接卡死了，主播必须装作杀进程
            //Thread.Sleep(10000000);  不行，线程休眠不会未响应，未响应才爽
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
