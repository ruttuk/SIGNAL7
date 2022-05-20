using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public bool rotating { get; private set; }
    public bool crashed { get; private set; }

    private void Awake()
    {
        crashed = false;
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
        if(!crashed && !GameManager.Instance.gameOver)
        {
            if (!rotating)
            {
                transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
            }

            float xInput = Input.GetAxisRaw("Horizontal");

            if (xInput != 0f && !rotating)
            {
                Turn(xInput);
            }
        }
        else
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
                Time.timeScale = 1f;
            }
        }
    }

    public void SignalCrash(bool playerCharacter)
    {
        //Debug.Log("Signal crashed!");
        crashed = true;
        trailFX.gameObject.SetActive(false);
        GameManager.Instance.EliminateSignal(playerCharacter);
    }

    public void Turn(float xInput)
    {
        // Add point to the trail
        trail1.AddLinePoint(transform.position, xInput, turnDuration);
        trail2.AddLinePoint(transform.position, xInput, turnDuration);

        // Rotate left or right 90 degrees
        StartCoroutine(Rotate90(xInput));
    }

    protected IEnumerator Rotate90(float xInput)
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
