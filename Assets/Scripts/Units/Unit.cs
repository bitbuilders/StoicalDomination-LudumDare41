using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Unit : MonoBehaviour
{
    [SerializeField] public bool m_isFlying = true;
    [SerializeField] [Range(0.0f, 100.0f)] float m_speed = 10.0f;
    [SerializeField] [Range(0.0f, 100.0f)] float m_bobSpeed = 0.6f;
    [SerializeField] [Range(0.0f, 5.0f)] float m_offsetDelta = 0.3f;

    public bool IsSelected { get; private set; }

    Vector2 m_positionOffset;
    Vector2 m_actualPosition;
    Vector2 m_targetPosition;
    float m_offsetRate;

    private void Start()
    {
        m_actualPosition = transform.position;
        m_targetPosition = m_actualPosition;
        m_positionOffset = Vector2.zero;
        m_offsetRate = m_bobSpeed;
    }

    private void Update()
    {
        Vector2 direction = m_targetPosition - m_actualPosition;
        if (direction.magnitude > 0.5f)
        {
            Vector2 velocity = direction.normalized * m_speed;
            velocity *= Time.deltaTime;
            m_actualPosition += velocity;
        }

        if (m_isFlying)
        {
            float multiplier = 1.0f - (m_positionOffset.y / m_offsetDelta);
            m_positionOffset.y += (m_offsetRate * multiplier) * Time.deltaTime;

            if (m_positionOffset.y >= m_offsetDelta - 0.1f)
            {
                m_offsetRate *= -1.0f;
                m_positionOffset.y = m_offsetDelta - 0.1f;
            }
            else if (m_positionOffset.y <= -m_offsetDelta)
            {
                m_offsetRate *= -1.0f;
                m_positionOffset.y = -m_offsetDelta;
            }

            transform.position = new Vector3(m_actualPosition.x, m_actualPosition.y) + new Vector3(m_positionOffset.x, m_positionOffset.y);
        }
    }

    public void SetTargetPosition(Vector2 position)
    {
        m_targetPosition = position;
    }

    public Vector2 ActualPosition()
    {
        return m_actualPosition + m_positionOffset;
    }

    protected void Move(Vector2 amount)
    {
        m_actualPosition += amount;
    }

    public void Select()
    {
        IsSelected = true;
    }

    public void DeSelect()
    {
        IsSelected = false;
    }
}
