using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterAI : Shatter
{
    [Header("AI Settings")]
    // Minimum distance to check for turn in front
    [Range(0.05f, 0.5f)]
    [SerializeField] private float sensorMinDistance;
    // Maximum distance to check for turn in front
    [Range(0.5f, 4f)]
    [SerializeField] private float sensorMaxDistance;
    // Minimum time that the signal is vulnerable after turning
    [Range(0.1f, 0.2f)]
    [SerializeField] private float fragileMinTime;
    [Range(0.2f, 1f)]
    [SerializeField] private float fragileMaxTime;
    // How often does the signal conduct random turns
    [Range(0.2f, 0.8f)]
    [SerializeField] private float randomTurnFrequency;
    // The distance to check if a turn will result in collision - lower value actually better
    [SerializeField] private float turnAwarenessDistance;

    // every 3 seconds, we'll roll for a random turn
    private float randomTurnInterval = 5f;

    // The normal hitbox for the AI has two modes.
    // Sensor or Fragile
    // 
    // while in sensor mode, the collider is bigger and out in front of the signal
    // if a collision is detected, the signal AI will turn
    // After turning, the signal enters fragile mode for a brief period
    // during which they are susceptible to crash
    // and then the hitbox is readjusted and the AI re-enters Sensor mode

    private bool isFragile = false;
    private Vector3 initialColliderSize;
    private Vector3 initialColliderCenter;
    private float sensorZOffset;

    private float timeElapsed = 0f;

    private void Start()
    {
        initialColliderSize = m_Collider.size;
        initialColliderCenter = m_Collider.center;
    }

    private void Update()
    {
        if(!isFragile && !signal.crashed && GameManager.Instance.IsGameRunning())
        {
            if (timeElapsed > randomTurnInterval)
            {
                // Roll for turn!
                if (Random.Range(0f, 1f) < randomTurnFrequency)
                {
                    Debug.Log("Rolled for random turn!");
                    CalculatedTurn();
                }
                timeElapsed = 0f;
            }

            timeElapsed += Time.deltaTime;
        }
    }

    protected override void OnTriggerEnter(Collider collision)
    {
        if(!signal.crashed)
        {
            if (collision.CompareTag("Barrier"))
            {
                if (isFragile || signal.rotating)
                {
                    bool eliminatedByPlayer = collision.gameObject.layer == LayerMask.NameToLayer("Player");
                    signal.SignalCrash(false, eliminatedByPlayer);
                    ApplyShatterEffect();
                }
                else
                {
                    Debug.Log("Sensed a barrier!");

                    // Set to Fragile
                    isFragile = true;

                    // Set hitbox back to initial size
                    m_Collider.center = initialColliderCenter;
                    m_Collider.size = initialColliderSize;

                    CalculatedTurn();
                }
            }
        }
    }

    private void CalculatedTurn()
    {
        // Set randomly by default
        float turnDir;

        // Before turning, determine if a our current turn will put us in harms way
        float leftBarrierDistance = CheckTurnForHit(true);
        float rightBarrierDistance = CheckTurnForHit(false);

        // If the distance to barrier on the left is greater than on the right, turn left
        if (leftBarrierDistance > rightBarrierDistance)
        {
            Debug.Log("Turn left!");
            turnDir = -1f;
        }
        else if(rightBarrierDistance > leftBarrierDistance)
        {
            Debug.Log("Turn right!");
            turnDir = 1f;
        }
        else
        {
            // If they are either equal or there are no barriers, pick random direction.
            turnDir = Random.Range(-1f, 1f);
        }

        // Turn
        signal.Turn(turnDir);

        StartCoroutine(BeFragile());
    }

    // Will turning in the given direction result in a hit?
    // If so, return how far away the barrier is
    // If not, return Infinity
    private float CheckTurnForHit(bool left)
    {
        Vector3 dir = left ? -Vector3.right : Vector3.right;

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(dir), out hit, turnAwarenessDistance))
        {
            if (hit.collider.CompareTag("Barrier"))
            {
                return hit.distance;
            }
        }

        return Mathf.Infinity;
    }

    private IEnumerator BeFragile()
    {
        // Wait for a random bit of time before resuming sensor mode
        yield return new WaitForSeconds(Random.Range(fragileMinTime, fragileMaxTime));

        sensorZOffset = Random.Range(sensorMinDistance, sensorMaxDistance);

        // Reset the collider to a new size - only in the Z axis.
        m_Collider.center = new Vector3(m_Collider.center.x, m_Collider.center.y, m_Collider.center.z + sensorZOffset);
        m_Collider.size = new Vector3(m_Collider.size.x, m_Collider.size.y, m_Collider.size.z + sensorZOffset);

        isFragile = false;
    }
}
