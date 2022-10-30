using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiter
{
    public float StepDuration { set { stepDuration = value; } }
    private float stepDuration;
    private float startTime;

    public Waiter(float stepDuration)
    {
        this.stepDuration = stepDuration;
        startTime = CurrentTime();
    }
    public bool CheckTime()
    {
        if (CurrentTime() > startTime+stepDuration)
        {
            startTime = CurrentTime();
            return true;
        }
        return false;
    }
    private float CurrentTime() => Time.realtimeSinceStartup;
}
