using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMode : Singleton<GameMode>
{
    public enum Mode
    {
        PLAYER_COMPUTER,
        PLAYER_PLAYER,
    }

    public Mode PlayerMode { get; private set; }

    static GameMode ms_gameMode = null;
    private void Start()
    {
        if (ms_gameMode == null)
        {
            ms_gameMode = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayVsComputer()
    {
        PlayerMode = Mode.PLAYER_COMPUTER;
    }

    public void PlayVsPlayer()
    {
        PlayerMode = Mode.PLAYER_PLAYER;
    }
}
