using UnityEngine;

[System.Serializable]
public class StatModifiers
{
    private int HPModifiers = 0;
    private int MPModifiers = 0;
    private int AttackModifiers = 0;
    private int DefenseModifiers = 0;
    private int ArcaneModifiers = 0;
    private int SpeedModifiers = 0;

    public int GetModifier(Statistic statistic)
    {
        switch (statistic)
        {
            case Statistic.HP:
                return HPModifiers;
            case Statistic.MP:
                return MPModifiers;
            case Statistic.Attack:
                return AttackModifiers;
            case Statistic.Defense:
                return DefenseModifiers;
            case Statistic.Arcane:
                return ArcaneModifiers;
            case Statistic.Speed:
                return SpeedModifiers;
            default:
                Debug.LogError("Error: Invalid statistic.");
                return 0;
        }
    }

    public void IncreaseStatistic(Statistic toIncrease, int amount)
    {
        switch (toIncrease)
        {
            case Statistic.HP:
                HPModifiers += amount;
                break;
            case Statistic.MP:
                MPModifiers += amount;
                break;
            case Statistic.Attack:
                AttackModifiers += amount;
                break;
            case Statistic.Defense:
                DefenseModifiers += amount;
                break;
            case Statistic.Arcane:
                ArcaneModifiers += amount;
                break;
            case Statistic.Speed:
                SpeedModifiers += amount;
                break;
            default:
                Debug.LogError("Error: Invalid statistic.");
                break;
        }
    }

    public void DecreaseStatistic(Statistic toDecrease, int amount)
    {   
        switch (toDecrease)
        {
            case Statistic.HP:
                HPModifiers -= amount;
                break;
            case Statistic.MP:
                MPModifiers -= amount;
                break;
            case Statistic.Attack:
                AttackModifiers -= amount;
                break;
            case Statistic.Defense:
                DefenseModifiers -= amount;
                break;
            case Statistic.Arcane:
                ArcaneModifiers -= amount;
                break;
            case Statistic.Speed:
                SpeedModifiers -= amount;
                break;
            default:
                Debug.LogError("Error: Invalid statistic.");
                break;
        }
    }

    public void ResetModifiers()
    {
        HPModifiers = 0;
        MPModifiers = 0;
        AttackModifiers = 0;
        DefenseModifiers = 0;
        ArcaneModifiers = 0;
        SpeedModifiers = 0;
    }
}