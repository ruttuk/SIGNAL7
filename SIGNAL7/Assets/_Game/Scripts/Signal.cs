using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
    [Header("Signal Settings")]
    [SerializeField] private float forwardSpeed = 11;
    [SerializeField] private float turnDuration = 0.2f;

    [Header("Components")]
    [SerializeField] private Trail trail1;
    [SerializeField] private Trail trail2;
    [SerializeField] private ParticleSystem trailFX;

    private float xInput;
    private bool rotating;
    private bool crashed = false; 

    private void Update()
    {
        if(!crashed)
        {
            if (!rotating)
            {
                transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
            }

            xInput = Input.GetAxisRaw("Horizontal");

            if (xInput != 0f && !rotating)
            {
                // Add point to the trail
                trail1.AddLinePoint(transform.position, xInput, turnDuration);
                trail2.AddLinePoint(transform.position, xInput, turnDuration);

                // Rotate left or right 90 degrees
                StartCoroutine(Rotate90());
            }
        }
    }

    public void SignalCrash()
    {
        Debug.Log("Signal crashed!");
        crashed = true;
        trailFX.gameObject.SetActive(false);
    }

    private IEnumerator Rotate90()
    {
        Debug.Log("Rotate 90");

        rotating = true;
        float timeElapsed = 0;
        float turnModifier = xInput > 0 ? 1f : -1f;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = transform.rotation * Quaternion.Euler(0, 90 * turnModifier, 0);

        while (timeElapsed < turnDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / turnDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.rotation = targetRotation;
        rotating = false;
    }
}
