using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : Singleton<ProjectileManager>
{
    [SerializeField] Transform m_projectilesLocation = null;
    [SerializeField] GameObject m_dragonProjectile = null;
    [SerializeField] GameObject m_catapultProjectile = null;
    [SerializeField] GameObject m_archerProjectile = null;
    [SerializeField] [Range(1, 100)] int m_poolSize = 25;

    List<GameObject> m_dragonProjectiles;
    List<GameObject> m_catapultProjectiles;
    List<GameObject> m_archerProjectiles;

    void Start()
    {
        if (m_dragonProjectile != null)
        {
            m_dragonProjectiles = new List<GameObject>();
            CreatePool(m_dragonProjectiles, m_dragonProjectile, m_projectilesLocation);
        }
        if (m_catapultProjectile != null)
        {
            m_catapultProjectiles = new List<GameObject>();
            CreatePool(m_catapultProjectiles, m_catapultProjectile, m_projectilesLocation);
        }
        if (m_archerProjectile != null)
        {
            m_archerProjectiles = new List<GameObject>();
            CreatePool(m_archerProjectiles, m_archerProjectile, m_projectilesLocation);
        }
    }

    private void CreatePool(List<GameObject> list, GameObject projectile, Transform location)
    {
        for (int i = 0; i < m_poolSize; ++i)
        {
            GameObject proj = Instantiate(projectile, Vector3.zero, Quaternion.identity, location);
            list.Add(proj);
            proj.SetActive(false);
        }
    }

    public GameObject Get(Unit.UnitType type)
    {
        GameObject projectile = null;
        switch (type)
        {
            case Unit.UnitType.DRAGON:
                projectile = FindObjectInList(m_dragonProjectiles);
                break;
            case Unit.UnitType.CATAPULT:
                projectile = FindObjectInList(m_catapultProjectiles);
                break;
            case Unit.UnitType.ARCHER:
                projectile = FindObjectInList(m_archerProjectiles);
                break;
        }

        return projectile;
    }

    private GameObject FindObjectInList(List<GameObject> list)
    {
        GameObject projectile = null;
        foreach (GameObject obj in list)
        {
            if (!obj.activeInHierarchy)
            {
                projectile = obj;
                obj.SetActive(true);
                break;
            }
        }

        return projectile;
    }
}
