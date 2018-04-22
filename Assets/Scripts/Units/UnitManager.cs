using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : Singleton<UnitManager>
{
    [SerializeField] TurnManager m_turnManager = null;
    [SerializeField] SpriteRenderer m_blueSelector = null;
    [SerializeField] SpriteRenderer m_redSelector = null;

    List<Unit> m_selectedUnits;
    public List<Unit> m_units;

    Timer m_timer;
    Vector2 m_selectionStart;
    Vector2 m_selectionEnd;

    private void Start()
    {
        m_timer = Timer.Instance;
        m_selectedUnits = new List<Unit>();
        m_selectionStart = Vector2.zero;
        m_selectionStart = m_selectionEnd;
    }

    void Update()
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
                        m_selectedUnits[i].SetTargetPosition(mousePos);
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
