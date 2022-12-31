using UnityEngine;
using Characters;
using System.Collections.Generic;

namespace Battle
{
    public class FleeAction : ICombatAction
    {
        public ActionType Category { get; set; }
        public CombatantSO Combatant { get; set; }
        public int Priority { get; set; }
        public string ActionName { get; set; }
        public List<string> ActionResult { get; set; }

        public FleeAction(CombatantSO runner)
        {
            Category = ActionType.RUN;
            Priority = 2;
            Combatant = runner;
            ActionName = "Fleeing";
            ActionResult = new List<string>();
        }

        public void DoAction()
        {
            ActionResult.Clear();
            ActionResult.Add(Combatant.ObjectName + " is trying to run away.");

            GameObject.FindObjectOfType<BattleService>().RunAway(Combatant);
        }
    }
}