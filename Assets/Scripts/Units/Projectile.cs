using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    Unit m_target = null;
    float m_damage;
    float m_speed;

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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit unit = collision.GetComponent<Unit>();
        if (unit == m_target)
        {
            m_target.TakeDamage(m_damage);
            gameObject.SetActive(false);
        }
    }

    public void Initialize(Vector2 position, Unit target, float speed, float damage)
    {
        transform.position = position;
        m_target = target;
        m_speed = speed;
        m_damage = damage;
    }
}
