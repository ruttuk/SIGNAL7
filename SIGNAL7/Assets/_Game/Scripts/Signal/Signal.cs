using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Signal : MonoBehaviour
{
    /**
     * The signal is the base class for each character in the game (they look kind of like spaceships)
     * A constant trail follows the signal which is deadly if crashed in to. These trails remain there for the entirety of the game.
     **/

    [Header("Signal Settings")]
    [SerializeField] protected Color signalColor;
    [SerializeField] protected float forwardSpeed = 11;
    [SerializeField] protected float turnDuration = 0.2f;
    [SerializeField] protected float postTurnSpeedBump = 2.5f;
    [SerializeField] protected float postTurnAccelerationTime = 1f;

    [Header("Components")]
    [SerializeField] private GameObject glider;
    [SerializeField] private Trail trail1;
    [SerializeField] private Trail trail2;
    [SerializeField] private ParticleSystem trailFX;

    public bool rotating { get; private set; }
    public bool crashed { get; private set; }

    private bool axisInUse = false;

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
        ParticleSystem.MainModule main = trailFX.main;

        // Adjust the particle color
        float pca = 1f;
        float particleAlbedo = 1f;
        Color particleColor = new Color(signalColor.r * pca, signalColor.g * pca, signalColor.b * pca, particleAlbedo);
        ParticleSystem.MinMaxGradient gradient = new ParticleSystem.MinMaxGradient(particleColor);

        main.startColor = gradient;
        fxRend.trailMaterial.color = particleColor;        
    }

    public virtual void Update()
    {
        if(!crashed && GameManager.Instance.IsGameRunning())
        {
            // If we haven't crashed and the game is still running, check for user input.
            CheckForInput();
        }
        else
        {
            // If space is pressed, restart the level.
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(0);
                Time.timeScale = 1f;
            }
        }
    }

    private void CheckForInput()
    {
        // As long as we aren't turning, the signal keeps moving forward.
        if (!rotating)
        {
            transform.Translate(Vector3.forward * forwardSpeed * Time.deltaTime);
        }

        float xInput = Input.GetAxisRaw("Horizontal");

        if (xInput != 0f)
        {
            if (!axisInUse && !rotating)
            {
                axisInUse = true;
                Turn(xInput);
            }
        }
        else
        {
            axisInUse = false;
        }
    }

    public void SignalCrash(bool playerCharacter, bool eliminatedByPlayerCharacter)
    {
        Debug.Log("Signal crashed!");
        crashed = true;

        // Disable the particle trail fx.
        trailFX.gameObject.SetActive(false);
        GameManager.Instance.EliminateSignal(playerCharacter, eliminatedByPlayerCharacter, signalColor);
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

        // After turning, briefly increase forward speed to accelerate out of the turn.
        // This can be used strategically by the player to get an edge on the other signals.
        forwardSpeed += postTurnSpeedBump;

        yield return new WaitForSeconds(postTurnAccelerationTime);

        forwardSpeed -= postTurnSpeedBump;
    }
}
