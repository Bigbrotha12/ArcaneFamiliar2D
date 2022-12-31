using UnityEngine;
using Characters;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Resistance Effect", menuName = "New Effect/Resistance Modifier", order = 1)]
public class ResistanceEffectSO : EffectSO
{
    [SerializeField] DamageType damageType;
    [SerializeField] bool AddResistance;

    public override void ActivateEffect(CombatantSO targetOfEffect)
    {
        targetOfEffect.ModifyResistances(damageType, AddResistance);
    }

    public override void ReverseEffect(CombatantSO targetOfEffect)
    {
        targetOfEffect.ModifyResistances(damageType, !AddResistance);
    }
}