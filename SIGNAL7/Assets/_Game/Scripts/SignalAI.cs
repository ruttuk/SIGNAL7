 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalAI : Signal
{
    [Header("AI Settings")]
    [SerializeField] private float checkForBarrierInterval = 0.5f;
    [SerializeField] private float maxDistanceCheckedForBarrier = 5f;

    /**
     * SignalAI works the same as signal except does not collect user input
     * Instead, a ray is cast in front of the signalAI on a fixed interval
     * If the ray hits a barrier, a function is performed where the probability of turning increases the closer to the barrier
     * 
     **/
    float timeElapsed = 0f;

    public override void Update()
    {
        if (!rotating && !crashed)
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }

        if(timeElapsed > checkForBarrierInterval)
        {
            CheckForBarrier();
            timeElapsed = 0f;
        }

        timeElapsed += Time.deltaTime;
    }

    private void CheckForBarrier()
    {
        Debug.Log("Checking for barrier!");
        RaycastHit hit;
        // cast from 1 unit in front of signal
        Vector3 forwardPos = transform.position += transform.forward;
        if(Physics.Raycast(forwardPos, transform.TransformDirection(transform.forward), out hit, maxDistanceCheckedForBarrier))
        {
            Debug.Log("Hit a barrier!");
            // Now we roll dice to see if we turn
            // probability of turning is calculated by distance to barrier i.e. if current dist is 3, max is 5
            // prob of turn = (5-3)/5 = 2/5 = 40%
            float turnProbability = (maxDistanceCheckedForBarrier - hit.distance) / maxDistanceCheckedForBarrier;

            if(Random.Range(0f, 1f) < turnProbability)
            {
                Debug.Log("Rolled for turn!");
                // Pick random direction, <0 is left turn, >0 right turn
                xInput = Random.Range(-1f, 1f);
                StartCoroutine(Rotate90());
            }
        }
    }
}
