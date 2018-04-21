using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData.asset", menuName = "Player/PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    [System.Serializable]
    public enum PlayerTag
    {
        PLAYER_1,
        PLAYER_2
    }

    public PlayerTag playerTag;
    public string playerName;
}
