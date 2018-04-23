using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crystals : MonoBehaviour
{
    [SerializeField] Unit.PlayerTag m_player = Unit.PlayerTag.PLAYER_1;
    [SerializeField] [Range(0.0f, 5.0f)] float m_dropRate = 0.1f;
    [SerializeField] [Range(1, 500)] int m_crystalsDropped = 5;
    [SerializeField] SpriteRenderer m_crystalsEarned = null;

    TurnManager m_turnManager;
    GameMode m_mode;
    Timer m_timer;
    AudioSource m_audioSource;
    ParticleSystem m_particleSystem;
    float m_timeSinceLastDrop = 5.0f;

    private void Start()
    {
        m_audioSource = GetComponent<AudioSource>();
        m_mode = GameMode.Instance;
        m_timer = Timer.Instance;
        m_turnManager = TurnManager.Instance;
        m_particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        m_timeSinceLastDrop += Time.deltaTime;
    }

    private void OnMouseDown()
    {
        if (m_timeSinceLastDrop >= m_dropRate && (m_mode.PlayerMode == GameMode.Mode.PLAYER_PLAYER || m_turnManager.m_playerTurn.PlayerTag == Unit.PlayerTag.PLAYER_1))
        {
            if (m_turnManager.m_playerTurn.PlayerTag == m_player && !m_timer.Paused)
            {
                m_timeSinceLastDrop = 0.0f;
                m_turnManager.m_playerTurn.Crystals += m_crystalsDropped;
                m_particleSystem.Play();
                SpriteRenderer sprite = Instantiate(m_crystalsEarned, Vector3.zero, Quaternion.identity, transform);
                sprite.transform.position = Random.insideUnitCircle.normalized * 0.5f;
                sprite.transform.position += transform.position;
                m_audioSource.Play();
                StartCoroutine(FadeText(sprite));
            }
        }
    }

    IEnumerator FadeText(SpriteRenderer sprite)
    {
        for (float i = 1.0f; i >= 0.0f; i -= Time.deltaTime)
        {
            Color c = sprite.color;
            c.a = i;
            sprite.color = c;
            sprite.transform.position += Vector3.up * 1.0f * Time.deltaTime;
            yield return null;
        }

        Destroy(sprite.gameObject);
    }
}
