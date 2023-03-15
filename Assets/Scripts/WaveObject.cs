using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour
{
    [Header("Enemies in Wave")]
    [SerializeField] private int meleeAmount = 0;
    [SerializeField] private int rangeAmount = 0;

    public enum EnemyType
    {
        Melee,
        Range
    }

    // Constructor
    public WaveObject(int meleeAmount, int rangeAmount)
    {
        this.meleeAmount = meleeAmount;
        this.rangeAmount = rangeAmount;
    }

    public int GetNumberOfType(EnemyType type)
    {
        switch (type)
        {
            case EnemyType.Melee:
                return meleeAmount;
            case EnemyType.Range:
                return rangeAmount;
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
            default:
                break;
        }
    }
}
