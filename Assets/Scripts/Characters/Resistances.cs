using UnityEngine;

[System.Serializable]
public class Resistances
{
    [SerializeField] private int PhysicalResistance = 0;
    [SerializeField] private int LightResistance = 0;
    [SerializeField] private int DarkResistance = 0;
    [SerializeField] private int FireResistance = 0;
    [SerializeField] private int WaterResistance = 0;
    [SerializeField] private int EarthResistance = 0;
    [SerializeField] private int WindResistance = 0;
    [SerializeField] private int HealResistance = 0;

    public ResistanceType GetResistance(DamageType type)
    {
        switch (type)
        {
            case DamageType.LIGHT:
                if(LightResistance < 0) return ResistanceType.VULNERABILITY;
                if(LightResistance == 0) return ResistanceType.None;
                if(LightResistance == 1) return ResistanceType.RESISTANCE;
                if(LightResistance > 1) return ResistanceType.IMMUNITY;
                break;
            case DamageType.DARK:
                if(LightResistance < 0) return ResistanceType.VULNERABILITY;
                if(LightResistance == 0) return ResistanceType.None;
                if(LightResistance == 1) return ResistanceType.RESISTANCE;
                if(LightResistance > 1) return ResistanceType.IMMUNITY;
                break;
            case DamageType.FIRE:
                if(LightResistance < 0) return ResistanceType.VULNERABILITY;
                if(LightResistance == 0) return ResistanceType.None;
                if(LightResistance == 1) return ResistanceType.RESISTANCE;
                if(LightResistance > 1) return ResistanceType.IMMUNITY;
                break;
            case DamageType.WATER:
                if(LightResistance < 0) return ResistanceType.VULNERABILITY;
                if(LightResistance == 0) return ResistanceType.None;
                if(LightResistance == 1) return ResistanceType.RESISTANCE;
                if(LightResistance > 1) return ResistanceType.IMMUNITY;
                break;
            case DamageType.EARTH:
                if(LightResistance < 0) return ResistanceType.VULNERABILITY;
                if(LightResistance == 0) return ResistanceType.None;
                if(LightResistance == 1) return ResistanceType.RESISTANCE;
                if(LightResistance > 1) return ResistanceType.IMMUNITY;
                break;
            case DamageType.WIND:
                if(LightResistance < 0) return ResistanceType.VULNERABILITY;
                if(LightResistance == 0) return ResistanceType.None;
                if(LightResistance == 1) return ResistanceType.RESISTANCE;
                if(LightResistance > 1) return ResistanceType.IMMUNITY;
                break;
            case DamageType.HEAL:
                if(LightResistance < 0) return ResistanceType.VULNERABILITY;
                if(LightResistance == 0) return ResistanceType.None;
                if(LightResistance == 1) return ResistanceType.RESISTANCE;
                if(LightResistance > 1) return ResistanceType.IMMUNITY;
                break;
            case DamageType.PHYSICAL:
                if(LightResistance < 0) return ResistanceType.VULNERABILITY;
                if(LightResistance == 0) return ResistanceType.None;
                if(LightResistance == 1) return ResistanceType.RESISTANCE;
                if(LightResistance > 1) return ResistanceType.IMMUNITY;
                break;
            default:
                Debug.LogError("Error: Invalid damage type.");
                break;
        }
        return ResistanceType.None;
    }

    public void AddResistance(DamageType type)
    {
        switch (type)
        {
            case DamageType.LIGHT:
                LightResistance += 1;
                break;
            case DamageType.DARK:
                DarkResistance += 1;
                break;
            case DamageType.FIRE:
                FireResistance += 1;
                break;
            case DamageType.WATER:
                WaterResistance += 1;
                break;
            case DamageType.EARTH:
                EarthResistance += 1;
                break;
            case DamageType.WIND:
                WindResistance += 1;
                break;
            case DamageType.HEAL:
                HealResistance += 1;
                break;
            case DamageType.PHYSICAL:
                PhysicalResistance += 1;
                break;
            default:
                break;
        }
    }

    public void RemoveResistance(DamageType type)
    {
        switch (type)
        {
            case DamageType.LIGHT:
                LightResistance -= 1;
                break;
            case DamageType.DARK:
                DarkResistance -= 1;
                break;
            case DamageType.FIRE:
                FireResistance -= 1;
                break;
            case DamageType.WATER:
                WaterResistance -= 1;
                break;
            case DamageType.EARTH:
                EarthResistance -= 1;
                break;
            case DamageType.WIND:
                WindResistance -= 1;
                break;
            case DamageType.HEAL:
                HealResistance -= 1;
                break;
            case DamageType.PHYSICAL:
                PhysicalResistance -= 1;
                break;
            default:
                break;
        }
    }

    public void ResetResistances()
    {
        PhysicalResistance = 0;
        LightResistance = 0;
        DarkResistance = 0;
        FireResistance = 0;
        WaterResistance = 0;
        EarthResistance = 0;
        WindResistance = 0;
        HealResistance = 0;
    }
}