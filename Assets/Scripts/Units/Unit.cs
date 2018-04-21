﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    [System.Serializable]
    public enum PlayerTag
    {
        PLAYER_1,
        PLAYER_2,
        COMPUTER
    }
    public enum State
    {
        ATTACKING,
        SEEKING,
        IDLE,
    }

    [SerializeField] public PlayerTag m_playerTag = PlayerTag.PLAYER_1;
    [SerializeField] Sprite m_front = null;
    [SerializeField] Sprite m_back = null;
    [SerializeField] [Range(0.0f, 500.0f)] float m_health = 10.0f;
    [SerializeField] [Range(0.0f, 100.0f)] float m_speed = 10.0f;
    [SerializeField] [Range(0.0f, 100.0f)] float m_bobSpeed = 0.6f;
    [SerializeField] [Range(0.0f, 5.0f)] float m_offsetDelta = 0.3f;
    [SerializeField] [Range(1.0f, 15.0f)] float m_detectionRange = 8.0f;
    [SerializeField] [Range(1.0f, 15.0f)] float m_attackRange = 4.0f;
    [SerializeField] [Range(0.5f, 15.0f)] float m_attackRate = 1.0f;
    [SerializeField] [Range(0.5f, 15.0f)] protected float m_projectileSpeed = 10.0f;
    [SerializeField] [Range(1.0f, 500.0f)] protected float m_projectileDamage = 50.0f;
    [SerializeField] public bool m_isFlying = true;

    public bool IsSelected { get; private set; }
    public bool Alive { get; private set; }

    public SpriteRenderer m_spriteRenderer;
    ProjectileManager m_projectileManager;
    protected Unit m_nearestTarget = null;
    Sprite m_originalSprite;
    Vector2 m_positionOffset;
    Vector2 m_actualPosition;
    Vector2 m_targetPosition;
    State m_state;
    float m_currentSpeed;
    float m_offsetRate;
    float m_attackTime;
    float m_delayTime;
    private bool m_paused = false;

    private void Start()
    {
        m_projectileManager = ProjectileManager.Instance;
        m_state = State.IDLE;
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_originalSprite = m_spriteRenderer.sprite;
        m_actualPosition = transform.position;
        m_targetPosition = m_actualPosition;
        m_positionOffset = Vector2.zero;
        m_offsetRate = m_bobSpeed;
        m_attackTime = 0.0f;
        m_delayTime = 0.0f;
        m_currentSpeed = m_speed;
        Alive = true;
    }

    private void Update()
    {
        if (!m_paused && Alive)
        {
            Vector2 direction = m_targetPosition - m_actualPosition;
            if (direction.magnitude > 0.5f)
            {
                Vector2 velocity = direction.normalized * m_currentSpeed;
                velocity *= Time.deltaTime;
                m_actualPosition += velocity;
                UpdateRotation(velocity);
            }
            else
            {
                m_state = State.IDLE;
            }

            if (m_nearestTarget == null || !m_nearestTarget.Alive)
            {
                FindNearestTarget();
            }
            m_delayTime += Time.deltaTime;
            if (m_delayTime >= 1.0f)
            {
                if (m_nearestTarget != null && m_nearestTarget.Alive)
                {
                    float distanceFromTarget = (m_nearestTarget.transform.position - transform.position).magnitude;
                    if (distanceFromTarget <= m_attackRange)
                    {
                        m_state = State.ATTACKING;
                    }
                    else if (distanceFromTarget <= m_detectionRange)
                    {
                        m_state = State.SEEKING;
                    }
                    else
                    {
                        FindNearestTarget();
                        m_state = State.IDLE;
                        m_targetPosition = m_actualPosition;
                    }
                }

                if (m_state == State.ATTACKING)
                {
                    Attack();
                }
                else if (m_state == State.SEEKING)
                {
                    m_currentSpeed = m_speed / 2.0f;
                    m_targetPosition = m_nearestTarget.transform.position;
                }
            }
            else
            {
                m_currentSpeed = m_speed;
            }
        }

        if ((m_state == State.IDLE || m_state == State.ATTACKING) && m_isFlying)
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
        }
        transform.position = new Vector3(m_actualPosition.x, m_actualPosition.y) + new Vector3(m_positionOffset.x, m_positionOffset.y);
    }

    private void Attack()
    {
        m_targetPosition = m_actualPosition;

        m_attackTime += Time.deltaTime;
        if (m_attackTime >= m_attackRate)
        {
            LaunchAttack(m_projectileManager);
            m_attackTime = 0.0f;
        }
    }
    protected abstract void LaunchAttack(ProjectileManager projectileManager);

    public void Initialize(Vector2 position, Vector2 targetPosition, PlayerTag tag, float health, bool flying)
    {
        m_actualPosition = position;
        SetTargetPosition(targetPosition);
        m_playerTag = tag;
        m_isFlying = flying;
        m_health = health;
    }

    public void TakeDamage(float damage)
    {
        m_health -= damage;
        if (m_health <= 0.0f && Alive)
        {
            Die();
        }
    }

    private void Die()
    {
        Alive = false;
        StartCoroutine(DeathTransition());
    }

    IEnumerator DeathTransition()
    {
        for (float i = 1.0f; i >= 0.0f; i -= Time.deltaTime)
        {
            Color c = m_spriteRenderer.color;
            c.a = i;
            m_spriteRenderer.color = c;
            yield return null;
        }
        UnitManager.Instance.RemnoveUnit(this);
        Destroy(gameObject);
    }

    private void FindNearestTarget()
    {
        Unit nearest = null;

        float distance = float.MaxValue;
        foreach (Unit unit in UnitManager.Instance.m_units)
        {
            float dist = (transform.position - unit.transform.position).magnitude;
            if (dist < distance && m_playerTag != unit.m_playerTag)
            {
                nearest = unit;
                distance = dist;
            }
        }

        m_nearestTarget = nearest;
    }

    private void UpdateRotation(Vector2 velocity)
    {
        if (Vector2.Angle(velocity, Vector2.left) <= 45.0f)
        {
            transform.localScale = new Vector3(-1.0f, 1.0f);
            m_spriteRenderer.sprite = m_originalSprite;
        }
        else if (Vector2.Angle(velocity, Vector2.right) <= 45.0f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f);
            m_spriteRenderer.sprite = m_originalSprite;
        }
        else if (Vector2.Angle(velocity, Vector2.up) <= 45.0f)
        {
            m_spriteRenderer.sprite = m_back;
        }
        else if (Vector2.Angle(velocity, Vector2.down) <= 45.0f)
        {
            m_spriteRenderer.sprite = m_front;
        }
    }

    public void SetTargetPosition(Vector2 position)
    {
        m_delayTime = 0.0f;
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

    public void Pause()
    {
        m_paused = true;
    }

    public void Resume()
    {
        m_paused = false;
    }

    public void Select()
    {
        IsSelected = true;
        UpdateSelector();
    }

    public void DeSelect()
    {
        IsSelected = false;
        UpdateSelector();
    }

    private void UpdateSelector()
    {
        if (IsSelected)
        {
            if (m_playerTag == PlayerTag.PLAYER_1)
            {

            }
            else if (m_playerTag == PlayerTag.PLAYER_2)
            {

            }
            else
            {

            }
        }
        else
        {

        }
    }
}