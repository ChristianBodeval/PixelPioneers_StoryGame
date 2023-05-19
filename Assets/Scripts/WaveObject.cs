using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour
{
    [Header("Wave timing")]
    public float waitTimeAfterWave = 1.5f;
    public float timeBetweenMobs = 0.01f;
    
    [Header("Wave composition")]
    [SerializeField] private int meleeAmount = 0;
    [SerializeField] private int rangeAmount = 0;
    [SerializeField] private int chargerAmount = 0;
    [SerializeField] private int wurmAmount = 0;
    [SerializeField] private int bruiserAmount = 0;
    [Range(0,1)][SerializeField] private int bossAmount = 0;

    public enum EnemyType
    {
        Melee,
        Range,
        Charger,
        Wurm,
        Bruiser,
        Boss
    }

    // Constructor
    public WaveObject(int meleeAmount, int rangeAmount, int chargerAmount, int wurmAmount, int bruiserAmount, int bossAmount)
    {
        this.meleeAmount = meleeAmount;
        this.rangeAmount = rangeAmount;
        this.chargerAmount = chargerAmount;
        this.wurmAmount = wurmAmount;
        this.bruiserAmount = bruiserAmount;
        this.bossAmount = bossAmount;
    }

    public int GetNumberOfType(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Melee:
                return meleeAmount;
            case EnemyType.Range:
                return rangeAmount;
            case EnemyType.Charger:
                return chargerAmount;
            case EnemyType.Wurm:
                return wurmAmount;
            case EnemyType.Bruiser:
                return bruiserAmount;
            case EnemyType.Boss:
                return bossAmount;
            default:
                return 0;
        }
    }

    public void SetNumberOfType(EnemyType type, int amount)
    {
        switch (type)
        {
            case EnemyType.Melee:
                meleeAmount = amount;
                break;
            case EnemyType.Range:
                rangeAmount = amount;
                break;
            case EnemyType.Charger:
                chargerAmount = amount;
                break;
            case EnemyType.Wurm:
                wurmAmount = amount;
                break;
            case EnemyType.Bruiser:
                bruiserAmount = amount;
                break;
            case EnemyType.Boss:
                bossAmount = amount;
                break;
            default:
                break;
        }
    }
}
