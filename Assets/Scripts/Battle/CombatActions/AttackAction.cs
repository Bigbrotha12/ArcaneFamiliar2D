using Characters;
using UnityEngine;
using System.Collections.Generic;

namespace Battle
{
    public class AttackAction : ICombatAction
    {
        public ActionType Category { get; set; }
        public CombatantSO Combatant { get; set; }
        public List<CombatantSO> Opponent { get; set; }
        public int Priority { get; set; }
        public string ActionName { get; set; }
        public List<string> ActionResult { get; set; }

        public AttackAction(CombatantSO attacker, List<CombatantSO> opponent)
        {
            Category = ActionType.ATTACK;
            Priority = -1;
            Combatant = attacker;
            Opponent = opponent;
            ActionName = "Attack";
            ActionResult = new List<string>();
        }

        public void DoAction()
        {
            ActionResult.Clear();
            ActionResult.Add(Combatant.ObjectName + " is attacking.");

            int attackPower = Combatant.GetAdjustedStatistic(Statistic.Attack);
            DamageType damageType = DamageType.PHYSICAL;

            foreach (CombatantSO target in Opponent)
            {
                ActionResult.Add(Combatant.ObjectName + " attacked " + target.ObjectName + ".");

                // Raw damage calculation
                int defendPower = target.GetAdjustedStatistic(Statistic.Defense);
                int damage = Mathf.Max(attackPower - defendPower, 0);

                // Resistance modifiers
                ResistanceType damageModifier = target.DamageResistance.GetResistance(damageType);
                
                int modifiedDamage = damage;
                switch (damageModifier)
                {
                    case ResistanceType.None:
                        break;
                    case ResistanceType.VULNERABILITY:
                        modifiedDamage *= 2;
                        break;
                    case ResistanceType.RESISTANCE:
                        modifiedDamage /= 2;
                        break;
                    case ResistanceType.IMMUNITY:
                        modifiedDamage = 0;
                        break;
                    default:
                        break;
                }

                target.ChangeHealth(-modifiedDamage);

                ActionResult.Add(target.ObjectName + " suffered " + modifiedDamage.ToString() + " points of damage.");
            }
        }
    }
}