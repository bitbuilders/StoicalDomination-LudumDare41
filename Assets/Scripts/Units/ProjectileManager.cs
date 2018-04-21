using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : Singleton<ProjectileManager>
{
    public enum ProjectileType
    {
        DRAGON,
        CATAPULT
    }

    [SerializeField] Transform m_p1ProjectilesLocation = null;
    [SerializeField] Transform m_p2ProjectilesLocation = null;
    [SerializeField] GameObject m_dragonProjectile = null;
    [SerializeField] GameObject m_catapultProjectile = null;
    [SerializeField] [Range(1, 100)] int m_poolSize = 25;

    List<GameObject> m_dragonProjectiles;
    List<GameObject> m_catapultProjectiles;

    void Start()
    {
        if (m_dragonProjectile != null)
        {
            m_dragonProjectiles = new List<GameObject>();
            CreatePool(m_dragonProjectiles, m_dragonProjectile, m_p1ProjectilesLocation);
        }
        if (m_catapultProjectile != null)
        {
            m_catapultProjectiles = new List<GameObject>();
            CreatePool(m_catapultProjectiles, m_catapultProjectile, m_p2ProjectilesLocation);
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

    public GameObject Get(ProjectileType type)
    {
        GameObject projectile = null;
        switch (type)
        {
            case ProjectileType.DRAGON:
                projectile = FindObjectInList(m_dragonProjectiles);
                break;
            case ProjectileType.CATAPULT:
                projectile = FindObjectInList(m_catapultProjectiles);
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
