using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class TurnManager : Singleton<TurnManager>
{
    [SerializeField] Text m_playerTurnText = null;
    [SerializeField] Text m_bannerText = null;
    [SerializeField] [Range(0.5f, 5.0f)] float m_turnTransitionTime = 1.0f;
    [SerializeField] CameraController m_camera = null;

    [SerializeField] public List<Player> m_players;

    public Player m_playerTurn;
    UnitManager m_unitManager;
    Timer m_timer;
    private int m_turn = 0;

    private void Start()
    {
        m_unitManager = UnitManager.Instance;
        m_timer = Timer.Instance;
        m_playerTurn = m_players[m_turn];
        if (GameMode.Instance.PlayerMode == GameMode.Mode.PLAYER_COMPUTER)
        {
            m_players[0].PlayerData.playerName = "Your";
            m_players[1].PlayerData.playerName = "Computer";
        }
        else
        {
            m_players[0].PlayerData.playerName = "Player 1";
            m_players[1].PlayerData.playerName = "Player 2";
        }
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
        Player p = m_turn == 0 ? m_players[1] : m_players[0];
        m_camera.SwitchToOtherSide(m_turnTransitionTime, new Vector2(0.0f, p.transform.position.y));
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
        m_playerTurnText.text = (GameMode.Instance.PlayerMode == GameMode.Mode.PLAYER_COMPUTER && m_playerTurn.PlayerTag == Unit.PlayerTag.PLAYER_1) ? "Your" : m_playerTurnText.text;

        m_playerTurnText.color = m_playerTurn.PlayerData.color;
        m_bannerText.color = m_playerTurn.PlayerData.color;
    }
}
