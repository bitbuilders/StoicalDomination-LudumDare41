using System.Collections;
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
    [System.Serializable]
    public enum UnitType
    {
        DRAGON,
        CATAPULT,
        ARCHER,
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
    public float MaxHealth { get; set; }
    public float Health { get { return m_health; } set { m_health = value; } }

    public SpriteRenderer m_spriteRenderer;
    ProjectileManager m_projectileManager;
    UnitManager m_unitManager;
    AudioSource m_audioSource;
    protected Unit m_nearestTarget = null;
    protected Player m_targetPlayer;
    Sprite m_originalSprite;
    Vector2 m_positionOffset;
    public Vector2 m_actualPosition;
    Vector2 m_targetPosition;
    Vector2 m_collisionForce;
    State m_state;
    float m_currentSpeed;
    float m_offsetRate;
    float m_attackTime;
    float m_delayTime;
    private bool m_paused = false;
    protected bool m_attackBase = false;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_unitManager = UnitManager.Instance;
        m_projectileManager = ProjectileManager.Instance;
        m_state = State.IDLE;
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_originalSprite = m_spriteRenderer.sprite;
        m_actualPosition = transform.position;
        m_positionOffset = Vector2.zero;
        m_collisionForce = Vector2.zero;
        m_offsetRate = m_bobSpeed;
        m_attackTime = 0.0f;
        m_delayTime = 0.0f;
        m_currentSpeed = m_speed;
        MaxHealth = Health;
        Alive = true;
        foreach (Player player in TurnManager.Instance.m_players)
        {
            if (player.PlayerTag != m_playerTag)
            {
                m_targetPlayer = player;
                break;
            }
        }
    }

    private void Update()
    {
        if (!m_paused && Alive)
        {
            GameObject target = null;
            if (m_nearestTarget != null)
            {
                Vector3 playerPos = m_targetPlayer.transform.position - new Vector3(m_actualPosition.x, m_actualPosition.y);
                Vector3 targetPos = m_nearestTarget.m_actualPosition - m_actualPosition;
                target = playerPos.magnitude < targetPos.magnitude ? m_targetPlayer.gameObject : m_nearestTarget.gameObject;
            }
            else
            {
                target = m_targetPlayer.gameObject;
            }

            Vector2 direction = m_targetPosition - m_actualPosition;
            if (direction.magnitude > 1.0f)
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
            if (m_delayTime >= 2.0f || direction.magnitude <= 0.25f)
            {
                bool valid = m_nearestTarget != null ? m_nearestTarget.Alive : true;
                if (target != null && valid)
                {
                    float distanceFromTarget = (target.transform.position - transform.position).magnitude;
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
                    Attack(target);
                }
                else if (m_state == State.SEEKING && m_nearestTarget != null)
                {
                    m_currentSpeed = m_speed * 0.5f;
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

        CalculateCollisionForce();
        m_actualPosition += m_collisionForce * Time.deltaTime;
        transform.position = new Vector3(m_actualPosition.x, m_actualPosition.y) + new Vector3(m_positionOffset.x, m_positionOffset.y);
    }

    private void Attack(GameObject target)
    {
        m_targetPosition = m_actualPosition;
        if (target == m_targetPlayer.gameObject)
        {
            m_attackBase = true;
        }
        else
        {
            m_attackBase = false;
        }

        m_attackTime += Time.deltaTime;
        Vector2 pos = target.transform.position;
        UpdateRotation(pos - m_actualPosition);
        if (m_attackTime >= m_attackRate)
        {
            m_audioSource.Play();
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

    public void CalculateCollisionForce()
    {
        m_collisionForce = Vector2.zero;
        foreach (Unit unit in m_unitManager.m_units)
        {
            Vector2 direction = m_actualPosition - unit.m_actualPosition;
            if (direction.magnitude <= 0.75f)
            {
                m_collisionForce += direction;
            }
        }
        m_collisionForce = m_collisionForce.normalized * m_speed * 0.5f;
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
        m_unitManager.RemnoveUnit(this);
        Destroy(gameObject);
    }

    private void FindNearestTarget()
    {
        Unit nearest = null;

        float distance = float.MaxValue;
        foreach (Unit unit in m_unitManager.m_units)
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

    public void SetNearestTarget(Unit target)
    {
        m_nearestTarget = target;
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
