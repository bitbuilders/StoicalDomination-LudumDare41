using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGame : MonoBehaviour
{
    [SerializeField] Player m_p1 = null;
    [SerializeField] Player m_p2 = null;
    [SerializeField] Text m_endText = null;
    [SerializeField] GameObject m_endScreen = null;

    void Update()
    {
        if (m_p1.Health <= 0.0f)
        {
            string text = "";
            if (GameMode.Instance.PlayerMode == GameMode.Mode.PLAYER_COMPUTER)
            {
                text = "Computer Wins :(";
            }
            else
            {
                text = "Player 2 Wins!";
            }
            DoEndStuff(text);
        }
        else if (m_p2.Health <= 0.0f)
        {
            string text = "";
            if (GameMode.Instance.PlayerMode == GameMode.Mode.PLAYER_COMPUTER)
            {
                text = "You Win!";
            }
            else
            {
                text = "Player 1 Wins!";
            }
            DoEndStuff(text);
        }
    }

    private void DoEndStuff(string text)
    {
        m_endText.text = text;
        m_endScreen.SetActive(true);
        Timer.Instance.Pause();
    }
}
