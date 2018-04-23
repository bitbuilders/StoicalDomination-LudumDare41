using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField] Unit m_unit = null;
    [SerializeField] Player m_player = null;
    [SerializeField] SpriteRenderer m_BG = null;
    [SerializeField] SpriteRenderer m_fill = null;

    void Update()
    {
        float scale = 1.0f;
        if (m_unit != null) scale = m_unit.Health / m_unit.MaxHealth;
        else if (m_player != null) scale = m_player.Health / m_player.MaxHealth;
        m_fill.transform.localScale = new Vector3(scale * 4.0f, 1.0f, 1.0f);
        Vector2 offset = (1.0f - scale) * m_BG.size;
        m_fill.transform.position = m_BG.transform.position - new Vector3(offset.x * 0.5f, 0.0f);
    }
}
