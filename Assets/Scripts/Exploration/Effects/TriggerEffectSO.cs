using UnityEngine;
using Characters;

[CreateAssetMenu(fileName = "New Trigger Effect", menuName = "New Effect/Resource Trigger", order = 4)]
public class TriggerEffectSO : EffectSO
{
    [SerializeField] private ResourceType resourcesTrigger;
    [SerializeField] private ResourceType resourcesAffected;
    [SerializeField] private ComparisonType comparison;
    [SerializeField] private int percentChangeTrigger;
    [SerializeField] private int amountAdded;

    public override void ActivateEffect(CombatantSO targetOfEffect)
    {
        if(resourcesTrigger is default(ResourceType) || comparison is default(ComparisonType) || resourcesAffected is default(ResourceType))
        {
            Debug.LogError("Error: Unassigned fields.");
        }

        ResourceTrigger trigger = new ResourceTrigger(Id, targetOfEffect, resourcesTrigger, resourcesAffected, comparison, percentChangeTrigger, amountAdded);
        targetOfEffect.AddTrigger(trigger);
    }

    public override void ReverseEffect(CombatantSO targetOfEffect)
    {
        targetOfEffect.RemoveTrigger(Id);
    }
}