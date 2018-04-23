using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Unit m_target = null;
    Player m_player = null;
    float m_damage;
    float m_speed;
    AudioSource m_audioSource;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (m_target != null)
        {
            Vector2 direction = m_target.transform.position - transform.position;
            Vector2 velocity = direction.normalized * m_speed;
            velocity *= Time.deltaTime;
            transform.position += new Vector3(velocity.x, velocity.y);

            float angle = Vector2.Angle(Vector2.right, direction);
            if (direction.y < 0.0f) angle *= -1.0f;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = rotation;
        }
        if (m_player != null)
        {
            Vector2 direction = m_player.transform.position - transform.position;
            Vector2 velocity = direction.normalized * m_speed;
            velocity *= Time.deltaTime;
            transform.position += new Vector3(velocity.x, velocity.y);

            float angle = Vector2.Angle(Vector2.right, direction);
            if (direction.y < 0.0f) angle *= -1.0f;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.rotation = rotation;

            if (direction.magnitude <= 1.0f)
            {
                transform.position = new Vector3(-5000.0f, 0.0f);
                m_player.Health -= m_damage;
                StartCoroutine(Kill());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit unit = collision.GetComponent<Unit>();
        if (unit != null && unit == m_target)
        {
            m_target.TakeDamage(m_damage);
            StartCoroutine(Kill());
        }
    }

    public void Initialize(Vector2 position, Unit target, Player player, float speed, float damage)
    {
        transform.position = position;
        m_target = target;
        m_player = player;
        m_speed = speed;
        m_damage = damage;
    }

    IEnumerator Kill()
    {
        m_audioSource.Play();
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }
}
