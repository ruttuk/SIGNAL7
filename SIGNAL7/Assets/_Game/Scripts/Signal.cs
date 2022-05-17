using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Signal : MonoBehaviour
{
    [Header("Signal Settings")]
    [SerializeField] protected Color signalColor;
    [SerializeField] protected float forwardSpeed = 11;
    [SerializeField] protected float turnDuration = 0.2f;

    [Header("Components")]
    [SerializeField] private GameObject glider;
    [SerializeField] private Trail trail1;
    [SerializeField] private Trail trail2;
    [SerializeField] private ParticleSystem trailFX;

    protected float xInput;
    protected bool rotating;
    protected bool crashed = false;

    private void Awake()
    {
        SetSignalColor();
    }

    private void SetSignalColor()
    {
        // Set color for each piece of the glider
        Renderer[] gliderPieces = glider.GetComponentsInChildren<Renderer>();

        for (int i = 0; i < gliderPieces.Length; i++)
        {
            gliderPieces[i].material.color = signalColor;
        }

        // Set color for each trail
        trail1.SetColor(signalColor);
        trail2.SetColor(signalColor);

        // Set particle fx color
        ParticleSystemRenderer fxRend = trailFX.GetComponent<ParticleSystemRenderer>();

        // particle color adjuster
        float pca = 1f;
        float particleAlbedo = 1f;
        Color particleColor = new Color(signalColor.r * pca, signalColor.g * pca, signalColor.b * pca, particleAlbedo);

        fxRend.material.color = particleColor;
        fxRend.trailMaterial.color = particleColor;
    }

    public virtual void Update()
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
        //Debug.Log("Signal crashed!");
        crashed = true;
        trailFX.gameObject.SetActive(false);
    }

    protected IEnumerator Rotate90()
    {
        //Debug.Log("Rotate 90");

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
