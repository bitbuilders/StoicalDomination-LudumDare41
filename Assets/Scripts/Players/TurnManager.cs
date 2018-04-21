using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] Timer m_timer = null;
    [SerializeField] Text m_playerTurnText = null;

    [SerializeField] List<PlayerData> m_players;

    public PlayerData m_playerTurn;
    private int m_turn = 0;

    private void Start()
    {
        m_playerTurn = m_players[m_turn];
        m_timer.Resume();
        UpdateName();
    }

    private void Update()
    {
        if (m_timer.Finished)
        {
            SwitchTurn();
            m_timer.Resume();
        }
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
        m_playerTurnText.text = m_playerTurn.playerName.ToLower()[m_playerTurn.playerName.Length - 1] == 's' ? m_playerTurn.playerName + "'" : m_playerTurn.playerName + "'s";
    }
}
