using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField]
    private Signal signal;

    [SerializeField]
    private LineRenderer m_LineRenderer;

    [SerializeField]
    private BoxCollider colliderPrefab;

    [SerializeField]
    private Transform colliderContainer;

    [SerializeField]
    private bool isColliderActive = false;

    BoxCollider m_CurrentColliderSegment;
    List<BoxCollider> colliderSegments;

    // are we traveling along x or z axis? by default z.
    private bool movingXward = false;
    private bool dissolving = false;

    private void Start()
    {
        // If the starting rotation for signal is either -90f or 90f, we're moving on the X axis
        //Debug.Log($"{gameObject.name} Starting y rotation: {signal.transform.rotation.eulerAngles.y}");
        //movingXward = (signal.transform.rotation.eulerAngles.y / 90f) % 2 == 0;
        m_LineRenderer.alignment = LineAlignment.TransformZ;
        m_LineRenderer.startWidth = 0.7f;
        m_LineRenderer.endWidth = 0.7f;

        m_LineRenderer.positionCount = 2;

        m_LineRenderer.SetPosition(0, signal.transform.position);
        m_LineRenderer.SetPosition(1, signal.transform.position);

        if(isColliderActive)
        {
            colliderSegments = new List<BoxCollider>();
            CreateColliderSegment();
        }
    }

    public void SetColor(Color c)
    {
        m_LineRenderer.material.color = c;
        m_LineRenderer.material.SetColor("_EmissionColor", c);
    }

    void Update()
    {
        if(!dissolving)
        {
            // Set first point to signal position
            m_LineRenderer.SetPosition(0, signal.transform.position);

            if (isColliderActive)
            {
                UpdateColliderSizeAndPos();
            }
        }
    }

    /// <summary>
    /// Both length and position of collider need to be updated every frame.
    /// 
    /// To get length l of trail in either direction d (x or z), l = abs(index 0d - index 1d)
    /// The current size of the collider on axis d will be set to l.
    /// 
    /// The position of the collider will be set to the midway point m of the current segment
    /// m = (index 1d) + l * 0.5
    /// </summary>
    private void UpdateColliderSizeAndPos()
    {
        Vector3 segHead = m_LineRenderer.GetPosition(0);
        Vector3 segTail = m_LineRenderer.GetPosition(1);

        float segLength;
        float segMid;
        float diffX = segHead.x - segTail.x;
        float diffZ = segHead.z - segTail.z;

        // Update size and pos depending on current direction
        if (movingXward)
        {
            segLength = Mathf.Abs(diffX);
            segMid = segTail.x + (diffX * 0.5f);

            m_CurrentColliderSegment.size = new Vector3(segLength, m_CurrentColliderSegment.size.y, m_CurrentColliderSegment.size.z);
            m_CurrentColliderSegment.transform.position = new Vector3(segMid, m_CurrentColliderSegment.transform.position.y, m_CurrentColliderSegment.transform.position.z);
        }
        else
        {
            segLength = Mathf.Abs(diffZ);
            segMid = segTail.z + (diffZ * 0.5f);

            m_CurrentColliderSegment.size = new Vector3(m_CurrentColliderSegment.size.x, m_CurrentColliderSegment.size.y, segLength);
            m_CurrentColliderSegment.transform.position = new Vector3(m_CurrentColliderSegment.transform.position.x, m_CurrentColliderSegment.transform.position.y, segMid);
        }
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

        if(isColliderActive)
        {
            // Add new collider segment
            CreateColliderSegment();
            // Update direction
            movingXward = !movingXward;
        }

        StartCoroutine(Rotate90(xInput, turnDuration));
    }

    // Each line segment has an associated box collider.
    // The scale of the collider is updated every frame.
    private void CreateColliderSegment()
    {
        // Instantiate new collider segment at the current trail pos, set trail as parent
        m_CurrentColliderSegment = Instantiate(colliderPrefab, signal.transform.position, Quaternion.identity, colliderContainer);
        colliderSegments.Add(m_CurrentColliderSegment);
    }

    private IEnumerator Rotate90(float xInput, float turnDuration)
    {
        // Debug.Log("Rotate 90");

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

    public IEnumerator DissolveTrail(float dissolveTime)
    {
        dissolving = true;

        if (isColliderActive)
        {
            DestroyColliderSegments();
        }

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

    private void DestroyColliderSegments()
    {
        foreach (BoxCollider collider in colliderSegments)
        {
            Destroy(collider);
        }
    }

}
