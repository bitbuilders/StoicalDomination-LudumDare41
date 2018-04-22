using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TurnManager : Singleton<TurnManager>
{
    [SerializeField] Text m_playerTurnText = null;
    [SerializeField] Text m_bannerText = null;
    [SerializeField] [Range(0.5f, 5.0f)] float m_turnTransitionTime = 1.0f;

    [SerializeField] List<Player> m_players;

    public Player m_playerTurn;
    UnitManager m_unitManager;
    Timer m_timer;
    private int m_turn = 0;

    private void Start()
    {
        m_unitManager = UnitManager.Instance;
        m_timer = Timer.Instance;
        m_playerTurn = m_players[m_turn];
        UpdateName();
    }

    private void Update()
    {
        if (m_timer.Finished)
        {
            m_timer.Finished = false;
            m_unitManager.DeSelectEverything();
            m_unitManager.PauseEverything();
            StartCoroutine(TurnTransition());
        }
    }

    IEnumerator TurnTransition()
    {
        yield return new WaitForSeconds(m_turnTransitionTime);
        SwitchTurn();
        m_unitManager.ResumeEverything();
        m_timer.Resume();
    }

    private void SwitchTurn()
    {
        ++m_turn;
        m_turn = m_turn % m_players.Count;
        m_playerTurn = m_players[m_turn];

        UpdateName();
    }

    private void UpdateName()
    {
        m_playerTurnText.text = m_playerTurn.PlayerData.playerName.ToLower()[m_playerTurn.PlayerData.playerName.Length - 1] == 's' ? 
            m_playerTurn.PlayerData.playerName + "'" : m_playerTurn.PlayerData.playerName + "'s";

        m_playerTurnText.color = m_playerTurn.PlayerData.color;
        m_bannerText.color = m_playerTurn.PlayerData.color;
    }
}
