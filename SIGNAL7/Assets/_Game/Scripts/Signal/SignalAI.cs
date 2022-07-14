 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalAI : Signal
{
    /**
     * The SignalAI functions like the Signal except is always moves forward and does not accept input. 
     **/

    public override void Update()
    {
        if (!rotating && !crashed && GameManager.Instance.IsGameRunning())
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }
    }
}
