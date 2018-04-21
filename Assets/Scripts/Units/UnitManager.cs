using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    Unit m_selectedUnit = null;
    public List<Unit> m_units;

    private void Start()
    {
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            bool selectedAUnit = false;
            foreach (Unit unit in m_units)
            {
                if (Intersects(unit))
                {
                    m_selectedUnit = unit;
                    unit.Select();
                    selectedAUnit = true;
                    break;
                }
            }
            if (!selectedAUnit && m_selectedUnit != null)
            {
                m_selectedUnit.DeSelect();
                m_selectedUnit = null;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (m_selectedUnit != null)
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                m_selectedUnit.SetTargetPosition(mousePos);
            }
        }
    }

    public void AddUnit(Unit unit)
    {
        m_units.Add(unit);
    }

    public void RemnoveUnit(Unit unit)
    {
        m_units.Remove(unit);
    }

    private bool Intersects(Unit unit)
    {
        bool intersects = false;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        SpriteRenderer renderer = unit.GetComponent<SpriteRenderer>();
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
