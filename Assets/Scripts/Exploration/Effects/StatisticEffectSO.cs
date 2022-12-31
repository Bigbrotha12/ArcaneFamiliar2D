using UnityEngine;
using Characters;

[CreateAssetMenu(fileName = "New Statistic Effect", menuName = "New Effect/Statistic Modifier", order = 3)]
public class StatisticEffectSO : EffectSO
{
    [SerializeField] Statistic statModifier;
    [SerializeField] int amount;

    public override void ActivateEffect(CombatantSO targetOfEffect)
    {
        if(amount >= 0)
        {
            targetOfEffect.IncreaseStatistic(statModifier, amount);
        }
        else
        {
            targetOfEffect.DecreaseStatistic(statModifier, -amount);
        }  
    }

    public override void ReverseEffect(CombatantSO targetOfEffect)
    {
        if(amount >= 0)
        {
            targetOfEffect.DecreaseStatistic(statModifier, amount);
        }
        else
        {
            targetOfEffect.IncreaseStatistic(statModifier, -amount);
        }  
    }
}