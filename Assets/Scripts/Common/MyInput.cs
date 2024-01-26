using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyInput : MonoBehaviour
{
    public Vector2 tempMousePos;
    public MyButton btn1 = new MyButton();
    public Vector2 tempPos;
    public float Distance;
    [SerializeField] private Vector2 Axis;
    public float mouseX;
    public static MyInput instance;

    public Vector2 centerPos;
    public Vector2 joysDir;

    public GameObject JoysTick;
    public GameObject JoysHandle;


    public float targetdis;

    public float joysTickDis;
    public float disClamp;
    public Vector3 fixedDir;
    public float fixedDis;

    public Vector3 intDir;

    public Vector2 GetMousePos()
    {
        {
            Vector2 v;
            v = new Vector2(tempMousePos.x, tempMousePos.y) + new Vector2(Screen.width / 2, Screen.height / 2);
            return v;
        }
    }

    public void UpdateMousePos()
    {
        tempMousePos = Input.mousePosition;
    }

    private void Update()
    {
        UpdateMousePos();
        btn1.Tick(Input.GetKey(KeyCode.Mouse0));
        tempPos = Vector2.Lerp(tempPos, GetMousePos(), 0.1f);
        mouseX = ReMapNumber(Input.mousePosition.x, 0, Screen.width, -1, 1);


        Axis.x = float.Parse((((GetMousePos().x - tempPos.x) / 1080 * 2)).ToString("#0.00"));
        Axis.y = float.Parse((((GetMousePos().y - tempPos.y) / 1920 * 2)).ToString("#0.00"));

        if (btn1.OnPressed)
        {
            centerPos = GetMousePos();
            JoysTick.transform.position = Input.mousePosition;
        }

        joysDir = (GetMousePos() - centerPos).normalized;

        joysTickDis = Vector2.Distance(centerPos, GetMousePos());
        joysTickDis = Mathf.Clamp(joysTickDis, -JoysTick.GetComponent<RectTransform>().rect.width / 2,
            JoysTick.GetComponent<RectTransform>().rect.width / 2);

        targetdis = ReMapNumber(joysTickDis, -JoysTick.GetComponent<RectTransform>().rect.width / 2,
            JoysTick.GetComponent<RectTransform>().rect.width / 2, -1, 1);
        JoysHandle.transform.position = JoysTick.transform.position +
                                        new Vector3((joysDir * joysTickDis).x, (joysDir * joysTickDis).y, 0);
        if (btn1.IsPressing)
        {
            JoysTick.SetActive(true);
            fixedDis = Mathf.Lerp(fixedDis, targetdis, 0.5f);
        }
        else
        {
            JoysTick.SetActive(false);
            fixedDis = Mathf.Lerp(fixedDis, 0, 0.5f);
        }

        fixedDir = new Vector3(joysDir.x, 0, joysDir.y);

        if (fixedDir.x > -0.7f && fixedDir.z < -0.7f)
        {
            intDir = Vector3.back;
        }
        else if (fixedDir.x < -0.7f && fixedDir.z > -0.7f)
        {
            intDir = Vector3.left;
        }
        else if (fixedDir.x > 0.7f && fixedDir.z < 0.7f)
        {
            intDir = Vector3.right;
        }
        else if (fixedDir.x > -0.7f && fixedDir.z > 0.7f)
        {
            intDir = Vector3.forward;
        }
    }

    public Vector2 GetAxis()
    {
        return Axis;
    }

    public Vector3 GetIntDir()
    {
        return intDir;
    }
 
    void Awake()
    {
        tempPos = GetMousePos();
        instance = this;
        JoysTick = Instantiate(JoysTick, this.transform);
        JoysTick.transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
        JoysHandle = JoysTick.transform.GetChild(0).gameObject;
    }
 
    public float ReMapNumber(float oXY, float oMin, float oMax, float nMin, float nMax)
    {
        float result = 0;
        result = (nMax - nMin) / (oMax - oMin) * (oXY - oMin) + nMin;
        return result;
    }
}