using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public  class KeyboradInput  : MonoBehaviour
{
    public MyButton W = new MyButton();
    public MyButton S = new MyButton();
    public MyButton A = new MyButton();
    public MyButton D = new MyButton();

    public MyButton space = new MyButton();
    public MyButton shift = new MyButton();

    public MyButton mouse0 = new MyButton();
    public MyButton mouse1 = new MyButton();
    public MyButton mouse2 = new MyButton();
    
    public MyButton C = new MyButton();
    public MyButton Q = new MyButton();
    public MyButton E = new MyButton();
    public MyButton F = new MyButton();
    public MyButton R = new MyButton();
    public MyButton ESC = new MyButton();

    public bool joystickMode;

    private Vector2 Dup;
    public Vector2 JoyDup;
 

    public Vector2 GetDup()
    {
        return Dup;
    }

    public void SetDup()
    {
        if (!joystickMode)
        {
            Dup = new Vector2((W.IsPressing ? 1 : 0) - (S.IsPressing ? 1 : 0), (D.IsPressing ? 1 : 0) - (A.IsPressing ? 1 : 0));
        }
        else
        {
            Dup = new Vector2(JoyDup.y, JoyDup.x);
        }
    }

    public void TickKey()
    {
        W.Tick(Input.GetKey(KeyCode.W));
        S.Tick(Input.GetKey(KeyCode.S));
        A.Tick(Input.GetKey(KeyCode.A));
        D.Tick(Input.GetKey(KeyCode.D));
        space.Tick(Input.GetKey(KeyCode.Space));
        mouse0.Tick(Input.GetKey(KeyCode.Mouse0));
        mouse1.Tick(Input.GetKey(KeyCode.Mouse1));
        mouse2.Tick(Input.GetKey(KeyCode.Mouse2));
        shift.Tick(Input.GetKey(KeyCode.LeftShift));
        E.Tick(Input.GetKey(KeyCode.E));
        Q.Tick(Input.GetKey(KeyCode.Q));
        R.Tick(Input.GetKey(KeyCode.R));
        ESC.Tick(Input.GetKey(KeyCode.Escape));
        C.Tick(Input.GetKey(KeyCode.C));
    }


}
