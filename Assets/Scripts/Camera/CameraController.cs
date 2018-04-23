using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform m_minBoundary = null;
    [SerializeField] Transform m_maxBoundary = null;
    [SerializeField] Transform m_minScrollBounds = null;
    [SerializeField] Transform m_maxScrollBounds = null;
    [SerializeField] [Range(1.0f, 50.0f)] float m_speed = 5.0f;

    Vector3 m_targetPosition;
    float m_time;
    bool m_canControl = true;

    void Update()
    {
        if (m_canControl)
        {
            UpdateCamera();
        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, m_targetPosition, (Time.deltaTime / m_time) * 2.0f);
        }
    }

    private void UpdateCamera()
    {
        Vector2 mousePos = Input.mousePosition;
        Vector2 velocity = Vector2.zero;

        velocity.x = Input.GetAxis("Horizontal");
        velocity.y = Input.GetAxis("Vertical");
        if (mousePos.x <= m_minScrollBounds.position.x)
        {
            velocity.x = -1.0f;
        }
        else if (mousePos.x >= m_maxScrollBounds.position.x)
        {
            velocity.x = 1.0f;
        }
        if (mousePos.y <= m_minScrollBounds.position.y)
        {
            velocity.y = -1.0f;
        }
        else if (mousePos.y >= m_maxScrollBounds.position.y)
        {
            velocity.y = 1.0f;
        }

        velocity *= m_speed * Time.deltaTime;
        transform.position += new Vector3(velocity.x, velocity.y);

        if (transform.position.x <= m_minBoundary.position.x)
        {
            transform.position += Vector3.right * (m_minBoundary.position.x - transform.position.x);
        }
        else if (transform.position.x >= m_maxBoundary.position.x)
        {
            transform.position += Vector3.right * (m_maxBoundary.position.x - transform.position.x);
        }
        if (transform.position.y <= m_minBoundary.position.y)
        {
            transform.position += Vector3.up * (m_minBoundary.position.y - transform.position.y);
        }
        else if (transform.position.y >= m_maxBoundary.position.y)
        {
            transform.position += Vector3.up * (m_maxBoundary.position.y - transform.position.y);
        }
    }

    public void SwitchToOtherSide(float time, Vector2 position)
    {
        m_canControl = false;
        m_time = time;
        m_targetPosition = position;
        m_targetPosition.z = -10.0f;
        StartCoroutine(ControlTimer());
    }

    IEnumerator ControlTimer()
    {
        yield return new WaitForSeconds(m_time);
        m_canControl = true;
    }
}
