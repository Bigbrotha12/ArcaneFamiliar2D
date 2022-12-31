using UnityEngine;
using Characters;
using Environment;
using System.Collections.Generic;

namespace Battle
{
    public class SpellAction : ICombatAction
    {
        public ActionType Category { get; set; }
        public CombatantSO Combatant { get; set; }
        public List<CombatantSO> Opponent { get; set; }
        public int Priority { get; set; }
        public SpellSO Spell;
        public string ActionName { get; set; }
        public List<string> ActionResult { get; set; }

        public SpellAction(CombatantSO caster, List<CombatantSO> targets, SpellSO spell)
        {
            Category = ActionType.SPELL;
            Priority = Mathf.Clamp(spell.Priority, -2, 2);
            Combatant = caster;
            Opponent = targets;
            Spell = spell;
            ActionName = spell.ObjectName;
            ActionResult = new List<string>();
        }

        public void DoAction()
        {

            if (Combatant is not SpellCasterSO)
            {
                ActionResult.Add(Combatant.ObjectName + " tried to cast a spell but failed.");
                return;
            }

            ActionResult.Add(Combatant.ObjectName + " is casting " + Spell.ObjectName + ".");

            SpellCasterSO caster = Combatant as SpellCasterSO;

            if (Spell.Power != 0)
            {
                // Damage calculation
                int attackPower = (int)((float)caster.GetAdjustedStatistic(Statistic.Attack) * ((float)caster.GetAdjustedStatistic(Statistic.Arcane) / 100.0f)) + Spell.Power;
                DamageType damageType = (DamageType)caster.SpellCasterFamiliar.MainFamiliar.ElementType;
                ActionResult.Add("Spell's energy was transmuted to " + damageType.ToString() + ".");
                
                // Apply damage and effects
                foreach (CombatantSO target in Opponent)
                {
                    int defendPower = (int)((float)caster.GetAdjustedStatistic(Statistic.Defense) * ((float)caster.GetAdjustedStatistic(Statistic.Arcane) / 100.0f));
                    int damage = Mathf.Max(0, attackPower - defendPower);
                    
                    // Resistance modifiers
                    ResistanceType damageModifier = target.DamageResistance.GetResistance(damageType);
                    
                    int modifiedDamage = damage;
                    switch (damageModifier)
                    {
                        case ResistanceType.None:
                            break;
                        case ResistanceType.VULNERABILITY:
                            ActionResult.Add("It's extremely effective!");
                            modifiedDamage *= 2;
                            break;
                        case ResistanceType.RESISTANCE:
                            ActionResult.Add("It's not very effective.");
                            modifiedDamage /= 2;
                            break;
                        case ResistanceType.IMMUNITY:
                            ActionResult.Add("It didn't do anything...");
                            modifiedDamage = 0;
                            break;
                        default:
                            break;
                    }
                    target.ChangeHealth(-modifiedDamage);

                    ActionResult.Add(target.ObjectName + " suffered " + modifiedDamage.ToString() + " points of damage.");

                    foreach (EffectSO spellEffect in Spell.SpellEffects)
                    {
                        new Effect(spellEffect, target);
                    }
                }
            }
            else
            {
                // Apply effects
                foreach (CombatantSO target in Opponent)
                {
                    foreach (EffectSO spellEffect in Spell.SpellEffects)
                    {
                        new Effect(spellEffect, target);
                    }
                }
            }
        }
    }
}