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
    [SerializeField] [Range(0.0f, 500.0f)] public float Health = 500.0f;
    [SerializeField] public PlayerData PlayerData ;
    public Unit.PlayerTag PlayerTag { get; set; }
    public float MaxHealth { get; set; }
    public int Crystals { get; set; }
    public int UnitLimit { get; set; }
    public bool CanBuy { get; set; }
    public UnitSpawnData[] m_spawnData;
    Timer m_timer;
    UnitSpawner m_unitSpawner;

    private void Awake()
    {
        CanBuy = true;
        Crystals = 200;
        UnitLimit = 75;
        m_unitSpawner = UnitSpawner.Instance;
        m_timer = Timer.Instance;
        Health = 500.0f;
        MaxHealth = Health;
        m_spawnData = new UnitSpawnData[3];
        int costRate = 50;
        float spawnRate = 2.0f;
        for (int i = 0; i < m_spawnData.Length; ++i)
        {
            m_spawnData[i] = new UnitSpawnData(0, 0.0f, (i + 1) * costRate, (i + 1) * spawnRate);
        }
    }

    private void Start()
    {
        MaxHealth = Health;
    }

    private void Update()
    {
        if (CanBuy && !m_timer.Paused)
        {
            for (int i = 0; i < m_spawnData.Length; ++i)
            {
                if (m_spawnData[i].queueCount > 0 && m_spawnData[i].currentTime <= 0.0f)
                {
                    Unit.UnitType type;
                    if (i == 0) type = Unit.UnitType.ARCHER;
                    else if (i == 1) type = Unit.UnitType.CATAPULT;
                    else type = Unit.UnitType.DRAGON;
                    m_spawnData[i].queueCount -= 1;
                    m_spawnData[i].currentTime = m_spawnData[i].spawnTime;
                    Spawn(type);
                }
                else if (m_spawnData[i].queueCount > 0)
                {
                    m_spawnData[i].currentTime -= Time.deltaTime;
                }
            }
        }
    }

    private void Spawn(Unit.UnitType type)
    {
        Vector2 position = Random.insideUnitCircle.normalized * 3.0f;
        position = position + new Vector2(transform.position.x, transform.position.y);

        m_unitSpawner.Spawn(type, transform.position, position, PlayerTag);
    }

    public bool PurchaseUnit(Unit.UnitType type)
    {
        bool validPurchase = false;
        switch (type)
        {
            case Unit.UnitType.ARCHER:
                if (m_spawnData[0].cost <= Crystals)
                {
                    Crystals -= m_spawnData[0].cost;
                    m_spawnData[0].queueCount++;
                    if (m_spawnData[0].queueCount <= 1) m_spawnData[0].currentTime = m_spawnData[0].spawnTime;
                    validPurchase = true;
                }
                break;
            case Unit.UnitType.CATAPULT:
                if (m_spawnData[1].cost <= Crystals)
                {
                    Crystals -= m_spawnData[1].cost;
                    m_spawnData[1].queueCount++;
                    if (m_spawnData[1].queueCount <= 1) m_spawnData[1].currentTime = m_spawnData[1].spawnTime;
                    validPurchase = true;
                }
                break;
            case Unit.UnitType.DRAGON:
                if (m_spawnData[2].cost <= Crystals)
                {
                    Crystals -= m_spawnData[2].cost;
                    m_spawnData[2].queueCount++;
                    if (m_spawnData[2].queueCount <= 1) m_spawnData[2].currentTime = m_spawnData[2].spawnTime;
                    validPurchase = true;
                }
                break;
        }

        return validPurchase;
    }
}
