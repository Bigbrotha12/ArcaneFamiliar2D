using Characters;
using System.Collections.Generic;

namespace Battle
{
    public interface ICombatAction
    {
        public void DoAction();
        public ActionType Category { get; set; }
        public int Priority { get; set; }
        public CombatantSO Combatant { get; set; }
        public string ActionName { get; set; }
        public List<string> ActionResult { get; set; }
    }
}