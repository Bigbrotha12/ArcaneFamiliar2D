using UnityEngine;
using Characters;

[CreateAssetMenu(fileName = "New Resource Effect", menuName = "New Effect/Resource Modifier", order = 2)]
public class ResourceEffectSO : EffectSO
{
    [System.Serializable]
    public struct ResourceModifier
    {
        public ResourceType resourceToModify;
        public int amount;
    }
    [SerializeField] ResourceModifier[] resourcesToChange;

    public override void ActivateEffect(CombatantSO targetOfEffect)
    {
        foreach (ResourceModifier modifier in resourcesToChange)
        {
            switch (modifier.resourceToModify)
            {   
                case ResourceType.Health:
                    targetOfEffect.ChangeHealth(modifier.amount);
                    break;
                case ResourceType.Mana:
                    targetOfEffect.ChangeMana(modifier.amount);
                    break;
                default:
                    Debug.LogError("Error: Invalid resource type.");
                    break;
            }
        }
    }

    public override void ReverseEffect(CombatantSO targetOfEffect)
    {
    }
}