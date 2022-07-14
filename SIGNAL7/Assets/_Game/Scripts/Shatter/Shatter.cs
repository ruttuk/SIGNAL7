using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Shatter : MonoBehaviour
{
    /**
     * Shatter is the base collider class for determining if the signal has been hit.
     * It also handles the shatter VFX that occur post-collision.
     **/

    [Header("Shatter Components")]
    [SerializeField] protected Signal signal;
    [SerializeField] private Trail trail1;
    [SerializeField] private Trail trail2;

    [Header("Explosion Settings")]
    [SerializeField] private float explosionStrength;
    [SerializeField] private float explosionRadius;
    [SerializeField] private float explosionJitter;
    [SerializeField] private float explosionDissolveTime;
    [SerializeField] private float trailDissolveTime;

    ShatterPiece[] shatterPieces;
    Vector3 explosionPos;

    protected BoxCollider m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<BoxCollider>();

        // Shatter script applied on the parent of all the cells
        shatterPieces = GetComponentsInChildren<ShatterPiece>();
    }

    protected void ApplyShatterEffect()
    {
        Debug.Log($"Shattering {shatterPieces.Length} number of pieces.");

        for(int i = 0; i < shatterPieces.Length; i++)
        {
            explosionPos = shatterPieces[i].transform.position * Random.Range(-explosionJitter, explosionJitter);
            shatterPieces[i].ApplyShatter(explosionStrength, explosionPos, explosionRadius, explosionDissolveTime);
        }

        // Dissolve both sides of the trail.
        StartCoroutine(trail1.DissolveTrail(trailDissolveTime));
        StartCoroutine(trail2.DissolveTrail(trailDissolveTime));
    } 
    protected virtual void OnTriggerEnter(Collider collision)
    {
        if(collision.tag.Equals(LookupTags.Barrier))
        {
            signal.SignalCrash(true, false);
            ApplyShatterEffect();
        }
    }
}
