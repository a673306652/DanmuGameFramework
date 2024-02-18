using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FPSText : MonoBehaviour
{
    private Text text;

    private int count;
    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        this.text = this.GetComponentInChildren<Text>();
        this.StartCoroutine(this.FPS());
    }

    // Update is called once per frame
    void Update()
    {
        this.count++;
        this.timer += Time.deltaTime;

    }


    IEnumerator FPS()
    {

        while (true)
        {
            var fps = (this.count / this.timer);
            this.text.text = $"{fps.ToString("f0")} FPS";
            if (fps < 20)
            {
                this.text.color = Color.red;
            }
            else if (fps < 50)
            {
                this.text.color = Color.yellow;
            }
            else
            {
                this.text.color = Color.green;
            }
            this.count = 0;
            this.timer = 0;
            yield return new WaitForSeconds(0.5F);
        }

    }
}
