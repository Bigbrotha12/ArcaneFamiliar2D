using UnityEngine;
using Characters;
using Battle;

[System.Serializable]
public class Effect
{
    [SerializeField] 
    EffectSO effectSO;
    CombatantSO targetOfEffect;
    int counter = 0;
    public Effect(EffectSO effectSO, CombatantSO targetOfEffect)
    {
        this.effectSO = effectSO;
        this.targetOfEffect = targetOfEffect;
        Initialise();
    }

    public EffectSO EffectSO { get { return effectSO; } }

    void Initialise()
    {
        switch (effectSO.activationType)
        {
            case EffectSO.ActivationType.Instant:
                Activate();
                break;
            case EffectSO.ActivationType.OnTurnStart:
                targetOfEffect.ActiveEffects.Add(this);
                //GameObject.FindObjectOfType<BattleService>().OnStartTurn+= Activate;
                break;
            default:
                break;
        }

        switch (effectSO.durationType)
        {
            case EffectSO.DurationType.LimitedByTurns:
                //GameObject.FindObjectOfType<BattleService>().OnStartTurn += IncrementCounter;
                break;
        }
    }

    public void Activate()
    {
        effectSO.ActivateEffect(targetOfEffect);
        

        if (effectSO.durationType == EffectSO.DurationType.LimitedByUses)
        {
            IncrementCounter();
        }
    }

    void IncrementCounter()
    {
        if (counter >= effectSO.durationLimit)
        {
            Deactivate();
        }
        counter++;
    }

    void ReverseEffect()
    {
        effectSO.ReverseEffect(targetOfEffect);
    }

    public void Deactivate()
    {
        if (effectSO.durationType == EffectSO.DurationType.Permanent) return;

        ReverseEffect();

        RemoveListeners();

        if (targetOfEffect.ActiveEffects.Contains(this))
        {
            targetOfEffect.ActiveEffects.Remove(this);
        }
    }

    void RemoveListeners()
    {
        if(effectSO.activationType == EffectSO.ActivationType.OnTurnStart)
        {
            //GameObject.FindObjectOfType<BattleService>().OnStartTurn -= Activate;
        }
        if(effectSO.durationType == EffectSO.DurationType.LimitedByTurns)
        {
            //GameObject.FindObjectOfType<BattleService>().OnStartTurn -= IncrementCounter;
        }
    }
}