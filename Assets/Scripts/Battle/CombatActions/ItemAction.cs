using UnityEngine;
using Characters;
using Environment;
using System.Collections.Generic;

namespace Battle
{
    public class ItemAction : ICombatAction
    {
        public ActionType Category { get; set; }
        public CombatantSO Combatant { get; set; }
        public List<CombatantSO> Opponent { get; set; }
        public int Priority { get; set; }
        public ItemSO Item;
        public string ActionName { get; set; }
        public List<string> ActionResult { get; set; }

        public ItemAction(List<CombatantSO> targets, ItemSO item)
        {
            Category = ActionType.ITEM;
            Priority = 0;
            Combatant = GameManager.Instance.Player;
            Opponent = targets;
            Item = item;
            ActionName = item.ObjectName;
            ActionResult = new List<string>();
        }

        public void DoAction()
        {
            ActionResult.Clear();
            ActionResult.Add(Combatant.ObjectName + " is using " + Item.ObjectName + ".");

            PlayerSO player = Combatant as PlayerSO;
            if (player.Inventory.RemoveItem(Item, InventoryStack.POUCH))
            {
                foreach (EffectSO itemEffect in Item.ItemEffects)
                {
                    foreach (CombatantSO target in Opponent)
                    {
                        new Effect(itemEffect, target);
                    }
                }
            }
        }
    }
}