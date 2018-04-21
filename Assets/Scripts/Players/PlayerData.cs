using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData.asset", menuName = "Player/PlayerData", order = 0)]
public class PlayerData : ScriptableObject
{
    public Unit.PlayerTag playerTag;
    public string playerName;
    public Color color;
}
