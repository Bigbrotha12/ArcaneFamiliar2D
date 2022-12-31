using UnityEngine;
using Characters;

public abstract class EffectSO : ScriptableObject, IObjectHeader
{
    public enum ActivationType
    {
        Instant,
        OnTurnStart,
        OnTakingDamage,
        OnDealingDamage
    }
    public enum DurationType
    {
        UntilDeactivated,
        LimitedByUses,
        LimitedByTurns,
        Permanent
    }

    [SerializeField] protected int _id;
    [SerializeField] protected string _objectName;
    [SerializeField] protected string _description;
    [SerializeField] protected Sprite _icon;
    public int Id
    {
        get { return _id; }
        set { _id = _id is 0 ? value : _id; }
    }
    public string ObjectName
    {
        get { return _objectName; }
        set { _objectName = _objectName is null ? value : _objectName; }
    }
    public string Description
    {
        get { return _description; }
        set { _description = _description is null ? value : _description; }
    }
    public Sprite Icon
    {
        get { return _icon; }
        set { _icon = _icon is null ? value : _icon; }
    }
        
    public bool StatusEffect;
    public DurationType durationType;
    public ActivationType activationType;
    public int durationLimit;
    public GameObject visualFX;
    
    public abstract void ActivateEffect(CombatantSO targetOfEffect);
    public abstract void ReverseEffect(CombatantSO targetOfEffect);
}