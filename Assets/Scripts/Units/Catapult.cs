﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : Unit
{
    [SerializeField] UnitType m_type = UnitType.CATAPULT;
    
    protected override void LaunchAttack(ProjectileManager projectileManager)
    {
        GameObject projectile = projectileManager.Get(m_type);
        projectile.GetComponent<Projectile>().Initialize(transform.position, m_nearestTarget, m_projectileSpeed, m_projectileDamage);
    }
}