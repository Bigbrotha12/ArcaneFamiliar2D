using UnityEngine;
using Environment;
using System.Collections.Generic;

namespace Characters
{
    // Keep same as original but refactor properties that are not common among familiars, enemies, and player.
    [CreateAssetMenu(fileName = "New Combatant", menuName = "Characters/New Combatant", order = 1)]
    public class CombatantSO : CharacterSO
    {   
        public List<AbilitySO> Abilities;
        public List<Effect> ActiveEffects;

        public delegate void HandleResourceChange(int previousValue, int newValue);
        public delegate void CombatantDefeat(CombatantSO combatant);
        public event HandleResourceChange OnHealthChanged;
        public event HandleResourceChange OnManaChanged;
        public event CombatantDefeat OnCombatantDefeat;

        public List<ActionType> AvailableActions = new List<ActionType>();
        protected StatModifiers Modifiers = new StatModifiers();
        public Resistances DamageResistance = new Resistances();
        public List<ResourceTrigger> ResourceTriggers = new List<ResourceTrigger>();

        protected Statistics _statistics = new Statistics();
        public virtual int HP { get { return _statistics.HP; } set { _statistics.HP = Mathf.Max(0, value); } }
        public virtual int MP { get { return _statistics.MP; } set { _statistics.MP = Mathf.Max(0, value); } }
        public virtual int Attack { get { return _statistics.Attack; } set { _statistics.Attack = Mathf.Max(0, value); } }
        public virtual int Defense { get { return _statistics.Defense; } set { _statistics.Defense = Mathf.Max(0, value); } }
        public virtual int Arcane { get { return _statistics.Arcane; } set { _statistics.Arcane = Mathf.Max(0, value); } }
        public virtual int Speed { get { return _statistics.Speed; } set { _statistics.Speed = Mathf.Max(0, value); } }

        public void ChangeHealth(int changeAmount)
        {
            int previousHealth = Health;
            Health += changeAmount;

            OnHealthChanged.Invoke(previousHealth, Health);
            if(Health <= 0)
            {
                OnCombatantDefeat.Invoke(this);
            }
        }

        public void ChangeMana(int changeAmount)
        {
            int previousMana = Mana;
            Mana += changeAmount;

            OnManaChanged.Invoke(previousMana, Mana);
        }

        public int GetAdjustedStatistic(Statistic stat)
        {
            return 1;
            // switch (stat)
            // {
            //     case Statistic.HP:
            //         return HP + Modifiers.GetModifier(stat);
            //     case Statistic.MP:
            //         return MP + Modifiers.GetModifier(stat);
            //     case Statistic.Attack:
            //         return Attack + Modifiers.GetModifier(stat);
            //     case Statistic.Defense:
            //         return Defense + Modifiers.GetModifier(stat);
            //     case Statistic.Arcane:
            //         return Arcane + Modifiers.GetModifier(stat);
            //     case Statistic.Speed:
            //         return Speed + Modifiers.GetModifier(stat);
            //     default:
            //         Debug.LogError("Error: Invalid statistic.");
            //         return 0;
            // }
        }
        
        public void IncreaseStatistic(Statistic modifier, int amount)
        {
            Modifiers.IncreaseStatistic(modifier, amount);
        }

        public void DecreaseStatistic(Statistic modifier, int amount)
        {
            Modifiers.DecreaseStatistic(modifier, amount);
        }

        public void ModifyAvailableActions(ActionType action, bool available)
        {
            if(available)
            {
                if(!AvailableActions.Contains(action))
                {
                    AvailableActions.Add(action);
                }
            }
            else
            {
                if(AvailableActions.Contains(action))
                {
                    AvailableActions.Remove(action);
                }
            }
        }

        public void ModifyResistances(DamageType newResistance, bool add)
        {
            if(add)
            {
                DamageResistance.AddResistance(newResistance);
            }
            else
            {
                DamageResistance.RemoveResistance(newResistance);
            }
        }

        public void AddTrigger(ResourceTrigger newTrigger)
        {
            ResourceTriggers.Add(newTrigger);
        }

        public void RemoveTrigger(int triggerId)
        {
            foreach (ResourceTrigger trigger in ResourceTriggers)
            {
                if(trigger.Id == triggerId)
                {
                    trigger.UnsubscribeTrigger();
                    ResourceTriggers.Remove(trigger);
                }
            }
        }

        public void ResetModifiers()
        {
            Modifiers.ResetModifiers();
        }

        public void ResetActions()
        {
            AvailableActions = new List<ActionType>()
            {
                ActionType.ATTACK,
                ActionType.SPELL,
                ActionType.DEFEND,
                ActionType.SWAP,
                ActionType.ITEM,
                ActionType.RUN
            };
        }

        public void ResetResistances()
        {
            DamageResistance.ResetResistances();
        }

        public void ResetTriggers()
        {
            foreach (ResourceTrigger trigger in ResourceTriggers)
            {
                trigger.UnsubscribeTrigger();
            }
            ResourceTriggers.Clear();
        }

        
    }
}
