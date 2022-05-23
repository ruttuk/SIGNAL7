 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalAI : Signal
{
    public override void Update()
    {
        if (!rotating && !crashed && GameManager.Instance.IsGameRunning())
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }
    }
}
