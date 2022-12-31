using Characters;
using System.Collections.Generic;

namespace Battle
{
    public class DefendAction : ICombatAction
    {
        public ActionType Category { get; set; }
        public CombatantSO Combatant { get; set; }
        public int Priority { get; set; }
        public string ActionName { get; set; }
        public List<string> ActionResult { get; set; }

        public DefendAction(CombatantSO combatant)
        {
            Category = ActionType.DEFEND;
            Priority = -2;
            Combatant = combatant;
            ActionName = "Defending";
            ActionResult = new List<string>();
        }

        public void DoAction()
        {
            ActionResult.Clear();
            ActionResult.Add(Combatant.ObjectName + " is taking a defensive position.");

            // Defend EffectSO
            EffectSO defend = GameManager.Instance.WorldAtlas.GetWorldObject<EffectSO>(1);
            new Effect(defend, Combatant);
        }
    }
}