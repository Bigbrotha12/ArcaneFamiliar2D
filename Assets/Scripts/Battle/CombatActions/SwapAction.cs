using UnityEngine;
using Characters;
using System.Collections.Generic;

namespace Battle
{
    public class SwapAction : ICombatAction
    {
        public ActionType Category { get; set; }
        public CombatantSO Combatant { get; set; }
        public int Priority { get; set; }
        public string ActionName { get; set; }
        public List<string> ActionResult { get; set; }

        public SwapAction()
        {
            Category = ActionType.SWAP;
            Priority = -1;
            Combatant = GameManager.Instance.Player;
            ActionName = "Switching";
            ActionResult = new List<string>();
        }

        public void DoAction()
        {
            PlayerSO player = Combatant as PlayerSO;

            ActionResult.Clear();
            ActionResult.Add("Switching to " + player.Familiars.SupportFamiliar.ObjectName + ".");

            player.Familiars.SwapFamiliars();
        }
    }
}