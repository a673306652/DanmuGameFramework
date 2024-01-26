using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyTimer
{
	public enum STATE
	{
		IDLE,
		RUN,
		FINISHED
	}
	public STATE state;
	public float Duration = 1.0f;

	private float elapsedTime = 0;

	public void Tick()
	{
		if (state == STATE.IDLE)
		{

		}
		else if (state == STATE.RUN)
		{
			elapsedTime += Time.deltaTime;
			if (elapsedTime >= Duration)
			{
				state = STATE.FINISHED;
			}
		}
		else if (state == STATE.FINISHED)
		{

		}
		else
		{
			Debug.Log("MyTimer error");
		}
	}

	public void Go()
	{
		elapsedTime = 0;
		state = STATE.RUN;
	}
}