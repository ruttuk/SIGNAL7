using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField]
    private Signal signal;

    //TrailRenderer m_TrailRenderer;

    [SerializeField]
    private LineRenderer m_LineRenderer;

    [SerializeField]
    private BoxCollider colliderPrefab;

    [SerializeField]
    private bool isColliderActive = false;

    BoxCollider m_CurrentColliderSegment;

    // are we traveling along x or z axis? by default z.
    private bool movingXward = false;

    private void Start()
    {
        /*
        m_Collider = GetComponent<PolygonCollider2D>();

        colliderPoints = new List<Vector2>();

        colliderPoints.Add(new Vector2(signal.transform.position.x, signal.transform.position.z));
        colliderPoints.Add(new Vector2(signal.transform.position.x - 1, signal.transform.position.z - 1));

        m_Collider.points = colliderPoints.ToArray();
        */
        m_LineRenderer.alignment = LineAlignment.TransformZ;
        m_LineRenderer.startWidth = 0.7f;
        m_LineRenderer.endWidth = 0.7f;

        m_LineRenderer.positionCount = 2;

        m_LineRenderer.SetPosition(0, signal.transform.position);
        m_LineRenderer.SetPosition(1, signal.transform.position);

        if(isColliderActive)
        {
            CreateColliderSegment();
        }
    }

    void FixedUpdate()
    {
        // Set first point to signal position
        m_LineRenderer.SetPosition(0, signal.transform.position);

        if(isColliderActive)
        {
            // To get the midpoint of the trail, get the current head first
            // Then figure out which direction the trail is moving in and cut in half.
            Vector3 midpointOfTrailSegment = m_LineRenderer.GetPosition(0);

            // Update the size of the box as well, depending on current direction
            if(movingXward)
            {
                m_CurrentColliderSegment.size = new Vector3(midpointOfTrailSegment.x, m_CurrentColliderSegment.size.y, m_CurrentColliderSegment.size.z);
                midpointOfTrailSegment.x *= 0.5f;
            }
            else
            {
                m_CurrentColliderSegment.size = new Vector3(m_CurrentColliderSegment.size.x, m_CurrentColliderSegment.size.y, midpointOfTrailSegment.z);
                midpointOfTrailSegment.z *= 0.5f;
            }

            m_CurrentColliderSegment.transform.position = midpointOfTrailSegment;
        }
    }

    public void AddPoint(Vector3 point)
    {
        //m_TrailRenderer.AddPosition(point);
    }

    
    public void AddLinePoint(Vector3 point, float xInput, float turnDuration)
    {
        Debug.Log("Adding point " + point);
        // Add one more point to count
        m_LineRenderer.positionCount++;

        // move everything down 1 i.e m_Line[2] -> m_Line[3];
        for(int i = m_LineRenderer.positionCount - 1; i > 0; --i)
        {
            m_LineRenderer.SetPosition(i, m_LineRenderer.GetPosition(i - 1));
        }

        // Set the new point at index 1
        m_LineRenderer.SetPosition(1, point);

        // Add collider point
        // AddColliderPoint(point);

        StartCoroutine(Rotate90(xInput, turnDuration));
    }

    /*
    private void AddColliderPoint(Vector3 point3)
    {
        colliderPoints.Add(new Vector2(point3.x, point3.z));
        m_Collider.points = colliderPoints.ToArray();
    }
    */

    private IEnumerator Rotate90(float xInput, float turnDuration)
    {
        Debug.Log("Rotate 90");

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
    }

    // Each line segment has an associated box collider.
    // The scale of the collider is updated every frame.
    private void CreateColliderSegment()
    {
        // Instantiate new collider segment at the current trail pos, set trail as parent
        m_CurrentColliderSegment = Instantiate(colliderPrefab, signal.transform.position, Quaternion.identity, transform);

        // Adjust segment position 3 units on the y axis
        // m_CurrentColliderSegment.transform.position += Vector3.up * 3f;
    }
    public IEnumerator DissolveTrail(float dissolveTime)
    {
        float alpha = m_LineRenderer.material.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / dissolveTime)
        {
            Color newColor = new Color(1, 1, 1, 1 - t);
            m_LineRenderer.material.color = newColor;
            yield return null;
        }

        // Hide after dissolving
        gameObject.SetActive(false);
    }

}
