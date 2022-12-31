using UnityEngine;
using Characters;

[CreateAssetMenu(fileName = "New Action Effect", menuName = "New Effect/Action Modifier", order = 0)]
public class ActionEffectSO : EffectSO
{
    [System.Serializable]
    public struct ActionModifier
    {
        public ActionType actionToModify;
        public bool available;
    }
    [SerializeField] ActionModifier[] actionsToDisable;

    public override void ActivateEffect(CombatantSO targetOfEffect)
    {
        foreach (ActionModifier action in actionsToDisable)
        {
            targetOfEffect.ModifyAvailableActions(action.actionToModify, action.available);   
        }
    }

    public override void ReverseEffect(CombatantSO targetOfEffect)
    {
        foreach (ActionModifier action in actionsToDisable)
        {
            targetOfEffect.ModifyAvailableActions(action.actionToModify, !action.available);
        }
    }
}