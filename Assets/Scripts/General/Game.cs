using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : Singleton<Game>
{
    [SerializeField] GameObject m_pauseScreen = null;

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (isPaused())
            {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }

    bool isPaused()
    {
        return m_pauseScreen.activeInHierarchy;
    }

    public void Pause()
    {
        m_pauseScreen.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void Unpause()
    {
        m_pauseScreen.SetActive(false);
        Time.timeScale = 1.0f;
    }

    public void LoadScene(string scene)
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(scene);
    }

    public void FightComp()
    {
        GameMode.Instance.PlayVsComputer();
    }

    public void FightPlayer()
    {
        GameMode.Instance.PlayVsPlayer();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
