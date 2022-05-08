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

    // First position in line will always be the signal position
    // Last position will always be signal starting point

    private void Start()
    {
        //m_TrailRenderer = GetComponent<TrailRenderer>();

        //m_TrailRenderer.alignment = LineAlignment.View;

        m_LineRenderer.alignment = LineAlignment.TransformZ;
        //m_LineRenderer.loop = true;

        // Don't fade out trail
        // m_TrailRenderer.time = float.PositiveInfinity;

        //m_TrailRenderer.positionCount = 2;
        m_LineRenderer.positionCount = 2;
        // Set first position and last position to signal position
        //m_TrailRenderer.SetPosition(0, signal.transform.position);
        m_LineRenderer.SetPosition(0, signal.transform.position);
        m_LineRenderer.SetPosition(1, signal.transform.position);
    }

    void FixedUpdate()
    {
        // Set first point to signal position
        m_LineRenderer.SetPosition(0, signal.transform.position);
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

        StartCoroutine(Rotate90(xInput, turnDuration));
    }

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

}
