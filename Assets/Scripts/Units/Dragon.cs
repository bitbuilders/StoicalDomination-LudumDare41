using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Unit
{
    [SerializeField] UnitType m_type = UnitType.DRAGON;
    
    protected override void LaunchAttack(ProjectileManager projectileManager)
    {
        GameObject projectile = projectileManager.Get(m_type);
        Unit unit = m_attackBase ? null : m_nearestTarget;
        Player player = m_attackBase ? m_targetPlayer : null;
        projectile.GetComponent<Projectile>().Initialize(transform.position, unit, player, m_projectileSpeed, m_projectileDamage);
    }
}
