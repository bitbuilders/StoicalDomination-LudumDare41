using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawner : Singleton<UnitSpawner>
{
    [SerializeField] GameObject m_dragonTemplateBlue = null;
    [SerializeField] GameObject m_dragonTemplateRed = null;
    [SerializeField] GameObject m_catapultTemplateBlue = null;
    [SerializeField] GameObject m_catapultTemplateRed = null;
    [SerializeField] GameObject m_archerTemplateBlue = null;
    [SerializeField] GameObject m_archerTemplateRed = null;
    [SerializeField] Transform m_p1Units = null;
    [SerializeField] Transform m_p2Units = null;
    [SerializeField] Player1 m_player1 = null;
    [SerializeField] Player2 m_player2 = null;
    [SerializeField] Text m_unitCount = null;
    [SerializeField] Text m_unitLimit = null;
    [SerializeField] Text m_crystalCount = null;
    [SerializeField] Slider m_progress1 = null;
    [SerializeField] Slider m_progress2 = null;
    [SerializeField] Slider m_progress3 = null;

    TurnManager m_turnManager;
    Timer m_timer;

    private void Start()
    {
        m_timer = Timer.Instance;
        m_turnManager = TurnManager.Instance;
    }

    private void Update()
    {
        if (m_timer.Paused)
        {
            m_player1.CanBuy = false;
            m_player2.CanBuy = false;
        }
        else
        {
            m_player1.CanBuy = true;
            m_player2.CanBuy = true;
        }

        Player player;
        if (m_turnManager.m_playerTurn.PlayerTag == m_player1.PlayerTag) player = m_player1;
        else player = m_player2;
        m_progress1.value = player.m_spawnData[0].queueCount > 0 ? 1.0f - (player.m_spawnData[0].currentTime / player.m_spawnData[0].spawnTime) : 0.0f;
        m_progress2.value = player.m_spawnData[1].queueCount > 0 ? 1.0f - (player.m_spawnData[1].currentTime / player.m_spawnData[1].spawnTime) : 0.0f;
        m_progress3.value = player.m_spawnData[2].queueCount > 0 ? 1.0f - (player.m_spawnData[2].currentTime / player.m_spawnData[2].spawnTime) : 0.0f;

        if (m_turnManager.m_playerTurn.PlayerTag == m_player1.PlayerTag)
        {
            m_unitCount.text = m_p1Units.childCount.ToString();
            m_unitLimit.text = m_player1.UnitLimit.ToString();
            m_crystalCount.text = m_player1.Crystals.ToString();
        }
        else
        {
            m_unitCount.text = m_p2Units.childCount.ToString();
            m_unitLimit.text = m_player2.UnitLimit.ToString();
            m_crystalCount.text = m_player2.Crystals.ToString();
        }
    }

    public void Spawn(Unit.UnitType type, Vector2 position, Vector2 targetPosition, Unit.PlayerTag player)
    {
        GameObject unitObject = null;
        Transform location = player == Unit.PlayerTag.PLAYER_1 ? m_p1Units : m_p2Units;
        switch (type)
        {
            case Unit.UnitType.DRAGON:
                GameObject unit1 = player == Unit.PlayerTag.PLAYER_1 ? m_dragonTemplateBlue : m_dragonTemplateRed;
                unitObject = Instantiate(unit1, position, Quaternion.identity, location);
                break;
            case Unit.UnitType.CATAPULT:
                GameObject unit2 = player == Unit.PlayerTag.PLAYER_1 ? m_catapultTemplateBlue : m_catapultTemplateRed;
                unitObject = Instantiate(unit2, position, Quaternion.identity, location);
                break;
        }
        Unit unit = unitObject.GetComponent<Unit>();
        unit.m_actualPosition = transform.position;
        unit.SetTargetPosition(targetPosition);
        UnitManager.Instance.AddUnit(unit);
    }

    public void Purchase(Unit.UnitType type)
    {
        if (m_turnManager.m_playerTurn.PlayerTag == m_player1.PlayerTag)
        {
            m_player1.PurchaseUnit(type);
        }
        else
        {
            m_player2.PurchaseUnit(type);
        }
    }
}
