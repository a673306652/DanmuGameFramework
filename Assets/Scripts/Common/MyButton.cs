using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyButton
{
	public bool IsPressing = false;
	public bool OnPressed = false;
	public bool OnReleased = false;
	public bool IsExtending = false;
	public bool IsDelaying = false;

	[Header("==== Settings ====")]
	public float extendingDuration = 1.5f;
	public float delayingDuration = 0.13f;

	private bool curState = false;
	private bool lastState = false;

	private MyTimer extTimer = new MyTimer();
	private MyTimer delayTimer = new MyTimer();
	public void Tick(bool input)
    {
		//StartTimer(extTimer, 1.0f);
		extTimer.Tick();
		delayTimer.Tick();

		curState = input;

		IsPressing = curState;

		OnPressed = false;
		OnReleased = false;
		IsExtending = false;
		IsDelaying = false;

		if (curState != lastState)
        {
			if(curState == true)
            {
				OnPressed = true;
				StartTimer(delayTimer, delayingDuration);

			}
            else
            {
				OnReleased = true;
				StartTimer(extTimer, extendingDuration);
			}
        }
		lastState = curState;

		if (extTimer.state == MyTimer.STATE.RUN) {
			IsExtending = true;
			//主要是这一段，他一直开着
		}

		if (delayTimer.state == MyTimer.STATE.RUN)
        {
			IsDelaying = true;
        }
	}
	public void StartTimer(MyTimer timer,float duration)
    {
		timer.Duration = duration;
		timer.Go();

    }

}