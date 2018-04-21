using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{
    public struct UnitSpawnData
    {
        public UnitSpawnData(int count, float time, int cost, float spawnTime)
        {
            queueCount = count;
            currentTime = time;
            this.cost = cost;
            this.spawnTime = spawnTime;
        }
        public float currentTime;
        public float spawnTime;
        public int queueCount;
        public int cost;
    }
    public int Crystals { get; set; }
    public float Health { get; private set; }
    public bool CanBuy { get; set; }
    public UnitSpawnData[] m_spawnData;
    Timer m_timer;

    private void Start()
    {
        m_timer = Timer.Instance;
        Health = 500.0f;
        m_spawnData = new UnitSpawnData[3];
        int costRate = 50;
        float spawnRate = 3.0f;
        for (int i = 0; i < m_spawnData.Length; ++i)
        {
            m_spawnData[i] = new UnitSpawnData(0, 0.0f, (i + 1) * costRate, (i + 1) * spawnRate);
        }
    }

    private void Update()
    {
        if (CanBuy && !m_timer.Paused)
        {
            foreach (UnitSpawnData data in m_spawnData)
            {
                if (data.queueCount > 0 && data.spawnTime == 0.0f)
                {

                }
            }
        }
    }

    public bool PurchaseUnit(Unit.UnitType type)
    {
        bool validPurchase = false;
        switch (type)
        {
            case Unit.UnitType.CATAPULT:
                if (m_spawnData[1].cost <= Crystals)
                {
                    Crystals -= m_spawnData[1].cost;
                    m_spawnData[1].queueCount++;
                    m_spawnData[1].currentTime = m_spawnData[1].spawnTime;
                    validPurchase = true;
                }
                break;
            case Unit.UnitType.DRAGON:
                if (m_spawnData[2].cost <= Crystals)
                {
                    Crystals -= m_spawnData[2].cost;
                    m_spawnData[2].queueCount++;
                    m_spawnData[2].currentTime = m_spawnData[2].spawnTime;
                    validPurchase = true;
                }
                break;
        }

        return validPurchase;
    }
}
