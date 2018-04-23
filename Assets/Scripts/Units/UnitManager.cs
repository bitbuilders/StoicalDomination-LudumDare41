using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    [SerializeField] TurnManager m_turnManager = null;
    [SerializeField] SpriteRenderer m_blueSelector = null;
    [SerializeField] SpriteRenderer m_redSelector = null;
    [SerializeField] Transform m_minBounds = null;
    [SerializeField] Transform m_maxBounds = null;
    [SerializeField] [Range(0.0f, 5.0f)] float m_decisionRate = 1.0f;

    List<Unit> m_selectedUnits;
    public List<Unit> m_units;

    Timer m_timer;
    GameMode m_mode;
    UnitSpawner m_spawner;
    Vector2 m_selectionStart;
    Vector2 m_selectionEnd;
    float m_decisionTime = 0.0f;
    int m_budget = 50;

    private void Start()
    {
        m_mode = GameMode.Instance;
        m_timer = Timer.Instance;
        m_spawner = UnitSpawner.Instance;
        m_selectedUnits = new List<Unit>();
        m_selectionStart = Vector2.zero;
        m_selectionStart = m_selectionEnd;
    }

    void Update()
    {
        if (m_mode.PlayerMode == GameMode.Mode.PLAYER_PLAYER || (m_turnManager.m_playerTurn.PlayerTag == Unit.PlayerTag.PLAYER_1))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            m_selectionEnd = mousePos;

            if (Input.GetMouseButtonDown(0))
            {
                m_selectionStart = mousePos;
                if (!m_timer.Paused)
                {
                    bool selectedAUnit = false;
                    foreach (Unit unit in m_units)
                    {
                        if (Intersects(unit) && m_turnManager.m_playerTurn.PlayerTag == unit.m_playerTag)
                        {
                            m_selectedUnits.Add(unit);
                            unit.Select();
                            selectedAUnit = true;
                        }
                    }
                    if (!selectedAUnit)
                    {
                        DeSelectEverything();
                    }
                }
            }
            else if (Input.GetMouseButtonUp(0) && !m_timer.Paused)
            {
                float selectionMinX = m_selectionStart.x < m_selectionEnd.x ? m_selectionStart.x : m_selectionEnd.x;
                float selectionMaxX = m_selectionStart.x < m_selectionEnd.x ? m_selectionEnd.x : m_selectionStart.x;
                float selectionMinY = m_selectionStart.y < m_selectionEnd.y ? m_selectionStart.y : m_selectionEnd.y;
                float selectionMaxY = m_selectionStart.y < m_selectionEnd.y ? m_selectionEnd.y : m_selectionStart.y;
                foreach (Unit unit in m_units)
                {
                    if (unit.m_playerTag == m_turnManager.m_playerTurn.PlayerTag)
                    {
                        if (UnitWithinSelection(unit, selectionMinX, selectionMaxX, selectionMinY, selectionMaxY))
                        {
                            unit.Select();
                            m_selectedUnits.Add(unit);
                        }
                    }
                }
                m_selectionEnd = m_selectionStart;
            }
            else if (Input.GetMouseButtonDown(1) && !m_timer.Paused)
            {
                Unit target = null;
                foreach (Unit unit in m_units)
                {
                    if (unit.m_playerTag != m_turnManager.m_playerTurn.PlayerTag)
                    {
                        if (Intersects(unit))
                        {
                            target = unit;
                        }
                    }
                }
                if (m_selectedUnits.Count > 0)
                {
                    for (int i = 0; i < m_selectedUnits.Count; ++i)
                    {
                        if (target == null)
                        {
                            m_selectedUnits[i].SetTargetPosition(LimitMove(mousePos));
                        }
                        else
                        {
                            m_selectedUnits[i].SetNearestTarget(target);
                        }
                    }
                }
            }
            DrawSelection();
        }
        else
        {
            BlueSelector(false);
            RedSelector(false);

            if (!m_timer.Paused)
            {
                m_decisionTime += Time.deltaTime;
                if (m_decisionTime >= m_decisionRate)
                {
                    m_decisionTime = 0.0f;

                    Player computer = m_turnManager.m_playerTurn;
                    List<Unit> friendlies = GetFriendlies(computer.PlayerTag);
                    int min = 0;
                    int max = 5;
                    
                    if (computer.Crystals < 50 || computer.Crystals < m_budget) min++;
                    if (friendlies.Count <= 0) max--;
                    int decision = Random.Range(min, max);

                    if (decision == 0)
                    {
                        int range = 3;
                        if (computer.Crystals < 150) range--;
                        if (computer.Crystals < 100) range--;
                        if (computer.Crystals < 50) range--;

                        if (range >= 1)
                        {
                            int x = Random.Range(0, range);
                            if (x == 0) m_spawner.Purchase(Unit.UnitType.ARCHER);
                            if (x == 1) m_spawner.Purchase(Unit.UnitType.CATAPULT);
                            if (x == 2) m_spawner.Purchase(Unit.UnitType.DRAGON);
                        }

                        m_budget = Random.Range(50, 250);

                        m_decisionRate = 1.0f;
                    }
                    else if (decision >= 1 && decision <= 3)
                    {
                        computer.Crystals += 5;
                        m_decisionRate = 0.2f;
                    }
                    else
                    {
                        m_decisionRate = 1.0f;

                        int y = Random.Range(0, friendlies.Count);
                        Unit chosen = friendlies[y];
                        Vector2 targetPosition = Random.insideUnitCircle.normalized * 10.0f;
                        targetPosition = LimitMove(targetPosition);
                        chosen.SetTargetPosition(targetPosition);
                    }
                }
            }
        }
        
    }

    public bool UnitWithinSelection(Unit unit, float minX, float maxX, float minY, float maxY)
    {
        bool within = false;
        

        if (unit.transform.position.x < maxX && unit.transform.position.x > minX)
        {
            if (unit.transform.position.y < maxY && unit.transform.position.y > minY)
            {
                within = true;
            }
        }

        return within;
    }

    private Vector2 LimitMove(Vector2 targetPosition)
    {
        Vector2 newPosition = targetPosition;
        if (targetPosition.x < m_minBounds.position.x)
        {
            newPosition.x = m_minBounds.position.x;
        }
        else if (targetPosition.x > m_maxBounds.position.x)
        {
            newPosition.x = m_maxBounds.position.x;
        }
        if (targetPosition.y < m_minBounds.position.y)
        {
            newPosition.y = m_minBounds.position.y;
        }
        else if (targetPosition.y > m_maxBounds.position.y)
        {
            newPosition.y = m_maxBounds.position.y;
        }

        return newPosition;
    }

    private List<Unit> GetFriendlies(Unit.PlayerTag tag)
    {
        List<Unit> units = new List<Unit>();

        foreach (Unit unit in m_units)
        {
            if (unit.m_playerTag == tag)
            {
                units.Add(unit);
            }
        }

        return units;
    }

    private void DrawSelection()
    {
        if (Input.GetMouseButton(0))
        {
            if (m_turnManager.m_playerTurn.PlayerTag == Unit.PlayerTag.PLAYER_1)
            {
                BlueSelector(true);
            }
            else if (m_turnManager.m_playerTurn.PlayerTag == Unit.PlayerTag.PLAYER_2)
            {
                RedSelector(true);
            }
            else
            {
                BlueSelector(false);
                RedSelector(false);
            }
            Vector2 size = m_blueSelector.size;
            size.x = m_selectionEnd.x - m_selectionStart.x;
            size.y = m_selectionEnd.y - m_selectionStart.y;
            m_blueSelector.size = size;
            m_redSelector.size = size;
            Vector2 center = new Vector2(size.x / 2.0f, size.y / 2.0f) + m_selectionStart;
            m_blueSelector.transform.position = center;
            m_redSelector.transform.position = center;
        }
        else
        {
            BlueSelector(false);
            RedSelector(false);
        }
    }

    private void BlueSelector(bool show)
    {
        float color = 0.0f;
        if (show)
        {
            color = 1.0f;
            RedSelector(false);
        }
        else
        {
            color = 0.0f;
        }
        m_blueSelector.color = new Color(m_blueSelector.color.r, m_blueSelector.color.g, m_blueSelector.color.b, color);
    }

    private void RedSelector(bool show)
    {
        float color = 0.0f;
        if (show)
        {
            color = 1.0f;
            BlueSelector(false);
        }
        else
        {
            color = 0.0f;
        }
        m_redSelector.color = new Color(m_redSelector.color.r, m_redSelector.color.g, m_redSelector.color.b, color);
    }

    public void DeSelectEverything()
    {
        foreach (Unit unit in m_selectedUnits)
        {
            unit.DeSelect();
        }
        m_selectedUnits.Clear();
    }

    public void PauseEverything()
    {
        foreach (Unit unit in m_units)
        {
            unit.SetTargetPosition(unit.m_actualPosition);
            unit.Pause();
        }
    }

    public void ResumeEverything()
    {
        foreach (Unit unit in m_units)
        {
            unit.Resume();
        }
    }

    public void AddUnit(Unit unit)
    {
        m_units.Add(unit);
    }

    public void RemnoveUnit(Unit unit)
    {
        m_units.Remove(unit);
        m_selectedUnits.Remove(unit);
    }

    private bool Intersects(Unit unit)
    {
        bool intersects = false;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SpriteRenderer renderer = unit.m_spriteRenderer;
        Vector2 size = renderer.size;
        if (mousePos.x > unit.ActualPosition().x - size.x && mousePos.x < unit.ActualPosition().x + size.x)
        {
            if (mousePos.y > unit.ActualPosition().y - size.y && mousePos.y < unit.ActualPosition().y + size.y)
            {
                intersects = true;
            }
        }

        return intersects;
    }
}
