using UnityEngine;
using Characters;

[System.Serializable]
public class ResourceTrigger
{
    public int Id;
    private ResourceType _triggerResource;
    private ResourceType _affectedResource;
    private ComparisonType _comparison;
    private int _percentChangeTrigger;
    private CombatantSO _target;
    private int _amountAdded;

    public ResourceTrigger(int triggerId, CombatantSO target, ResourceType trigger, ResourceType affected, ComparisonType comparer, int changeTrigger, int changeAmount)
    {
        Id = triggerId;
        _triggerResource = trigger;
        _affectedResource = affected;
        _comparison = comparer;
        _percentChangeTrigger = changeTrigger;
        _target = target;
        _amountAdded = changeAmount;
        
        if(_triggerResource is ResourceType.Health) _target.OnHealthChanged += TriggerAction;
        if(_triggerResource is ResourceType.Mana) _target.OnManaChanged += TriggerAction;
    }
    
    public void TriggerAction(int previousValue, int newValue)
    {
        int percentChange;
        if(previousValue == 0) { percentChange = 1; }
        else if (_triggerResource is ResourceType.Health)
        {
            percentChange =
            (int)((float)Mathf.Abs(newValue - previousValue) / (float)_target.GetAdjustedStatistic(Statistic.HP));
        }
        else if (_triggerResource is ResourceType.Mana)
        {
            percentChange =
            (int)((float)Mathf.Abs(newValue - previousValue) / (float)_target.GetAdjustedStatistic(Statistic.MP));
        }
        else
        {
            percentChange = 0;
        }
    
        switch(_comparison)
        {
            case ComparisonType.LessThan:
                if(percentChange < _percentChangeTrigger) 
                {
                    ChangeResource();
                } 
                break;
            case ComparisonType.LessThanOrEqual:
                if(percentChange <= _percentChangeTrigger) 
                {
                    ChangeResource();
                } 
                break;
            case ComparisonType.Equal:
                if(percentChange == _percentChangeTrigger) 
                {
                    ChangeResource();
                } 
                break;
            case ComparisonType.MoreThanOrEqual:
                if(percentChange >= _percentChangeTrigger) 
                {
                    ChangeResource();
                } 
                break;
            case ComparisonType.MoreThan:
                if(percentChange > _percentChangeTrigger) 
                {
                    ChangeResource();
                } 
                break;
            default:
                Debug.LogError("Error: Invalid Comparison type.");
                break;
        }
    }

    private void ChangeResource()
    {
        Debug.Log("TriggerEffect ID: " + Id.ToString() + " activated.");
        if(_affectedResource is ResourceType.Health)
        {
            _target.Health += _amountAdded;
        }
        else if(_affectedResource is ResourceType.Mana)
        {
            _target.Mana += _amountAdded;
        }    
    }

    public void UnsubscribeTrigger()
    {
        if(_triggerResource is ResourceType.Health) _target.OnHealthChanged -= TriggerAction;
        if(_triggerResource is ResourceType.Mana) _target.OnManaChanged -= TriggerAction;
    }
}