using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopIcon : MonoBehaviour
{
    [SerializeField] public Text m_cost = null;
    [SerializeField] public Unit.UnitType m_unitType = Unit.UnitType.DRAGON;

    public void OnClick()
    {
        if (TurnManager.Instance.m_playerTurn.PlayerTag == Unit.PlayerTag.PLAYER_1 || GameMode.Instance.PlayerMode == GameMode.Mode.PLAYER_PLAYER)
        {
            UnitSpawner.Instance.Purchase(m_unitType);
        }
    }
}
