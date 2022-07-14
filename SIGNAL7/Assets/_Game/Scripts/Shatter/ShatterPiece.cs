using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class ShatterPiece : MonoBehaviour
{
    /**
     * Represents a single piece of the destroyed signal
     **/

    private MeshRenderer m_Renderer;
    private Rigidbody m_Rigidbody;

    private void Awake()
    {
        m_Renderer = GetComponent<MeshRenderer>();
        m_Rigidbody = GetComponent<Rigidbody>();

        m_Renderer.sharedMaterial.SetFloat(LookupTags.DissolveAmount, 0f);
        m_Rigidbody.detectCollisions = false;
    }

    public void ApplyShatter(float explosionStrength, Vector3 explosionPos, float explosionRadius, float dissolveTime)
    {
        // Apply gravity
        m_Rigidbody.useGravity = true;
        m_Rigidbody.detectCollisions = true;

        // Add explosive force
        m_Rigidbody.AddExplosionForce(explosionStrength, explosionPos, explosionRadius);

        // Start dissolving
        StartCoroutine(Dissolve(dissolveTime));

        // Once dissolving, disable gravity. This allows the pieces to kind of float away suspended in air.
        m_Rigidbody.useGravity = false;
    }

    public IEnumerator Dissolve(float dissolveTime)
    {
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / dissolveTime)
        {
            m_Renderer.sharedMaterial.SetFloat(LookupTags.DissolveAmount, t);
            yield return null;
        }

        // Hide after dissolving
        gameObject.SetActive(false);
        m_Renderer.sharedMaterial.SetFloat(LookupTags.DissolveAmount, 0f);
    }
}
