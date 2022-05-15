using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Shatter : MonoBehaviour
{
    [SerializeField] private Signal signal;
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

    BoxCollider m_GliderCollider;

    private void Awake()
    {
        m_GliderCollider = GetComponent<BoxCollider>();

        // Shatter script applied on the parent of all the cells
        shatterPieces = GetComponentsInChildren<ShatterPiece>();
    }

    private void ApplyShatterEffect()
    {
        Debug.Log($"Shattering {shatterPieces.Length} number of pieces.");

        for(int i = 0; i < shatterPieces.Length; i++)
        {
            explosionPos = shatterPieces[i].transform.position * Random.Range(-explosionJitter, explosionJitter);
            shatterPieces[i].ApplyShatter(explosionStrength, explosionPos, explosionRadius, explosionDissolveTime);
        }

        StartCoroutine(trail1.DissolveTrail(trailDissolveTime));
        StartCoroutine(trail2.DissolveTrail(trailDissolveTime));
    } 
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.tag.Equals("Trail"))
        {
            Debug.Log("Here");
            signal.SignalCrash();
            ApplyShatterEffect();
        }
    }
}
