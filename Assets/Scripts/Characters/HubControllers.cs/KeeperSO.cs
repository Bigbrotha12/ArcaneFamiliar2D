using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Characters
{
    [CreateAssetMenu(fileName = "New SecretKeeper", menuName = "Characters/Hub/New SecretKeeper", order = 0)]
    public class KeeperSO : NPCSO
    {
        // A secret keeper NPC accept three player actions:
        // Talk - Shares a random bit of information about local area.
        // Study - Allows player to review any Learnable items and obtain reward.
        // Work - Allows player to do work in exchange for money.
        [SerializeField] private List<string> secrets;
        [SerializeField] private List<string> workText;
        [SerializeField] private int studyCost;
        [SerializeField] private int workReward;
        [SerializeField] private bool canWork;

        public void Talk()
        {
            Speak(new List<string>(secrets));
            // TODO: Have NPC say random rumor to player.
        }

        public void ChooseWork()
        {
            List<string> speech = new List<string>() { "I prefer field work but sometimes busy work is good too...\n What should I do today?" };
            string[] options = { "Study", "Write Paper", "Never mind" };
            Debug.Log(GameManager.Instance.Player.ObjectName);
            GameManager.Instance.Player.Ask(speech, options, HandleWorkSelection);
        }

        private void HandleWorkSelection(int index)
        {
            switch (index)
            {
                case 0:
                    Study();
                    break;
                case 1:
                    Work();
                    break;
                case 2:
                    break;
                default:
                    Debug.Log("Invalid selection.");
                    break;
            }

        }

        private void Study()
        {
            bool itemFound = false;
            List<string> studyResult = new List<string>()
            {
                "(Let's see what I have to study...)",
            };

            // Check if player has any learnable items
            foreach(ItemSO item in GameManager.Instance.Player.Inventory.GetCombinedInventory())
            {
                if(item.Category is ItemCategory.LEARNABLE)
                {
                    itemFound = true;
                    studyResult.Add("(I have " + item.ObjectName + ")");
                    studyResult.Add("Mhm...");
                    studyResult.AddRange(item.FlavorDescription);

                    GameManager.Instance.Player.Inventory.RemoveFromAny(item);
                    // Add item depending on type
                    if(item.LearnableContent is ItemSO)
                    {
                        GameManager.Instance.Player.Inventory.AddToAny(item.LearnableContent as ItemSO);
                    } 
                    else if(item.LearnableContent is SpellSO)
                    {
                        GameManager.Instance.Player.SpellBook.LearnNewSpell(item.LearnableContent as SpellSO);
                    }
                    else if(item.LearnableContent is RecipeSO)
                    {
                        GameManager.Instance.Player.RecipeBook.AddRecipe(item.LearnableContent as RecipeSO);
                    }
                    else if(item.LearnableContent is LocationSO)
                    {
                        GameManager.Instance.Player.Atlas.AddLocation(item.LearnableContent as LocationSO);
                    }
                    break;
                }
            }

            if(!itemFound)
            {
               studyResult.Add("(Nope, I don't have anything to look at right now.)");
            } 
            
            GameManager.Instance.Player.Speak(studyResult);
        }

        private void Work()
        {
            List<string> workResult = new List<string>()
            {
                "(Let's see what I can do...)",
            };

            if (canWork) 
            { 
                foreach (string text in workText)
                {
                    workResult.Add(text);
                }

                workResult.Add("I earned " + workReward.ToString() + " G");
                GameManager.Instance.Player.Wallet.EarnMoney(workReward);
            }
            else
            {
                workResult.Add("(I don't have any work left to do.");
            }

            GameManager.Instance.Player.Speak(workResult);
        }
    }
}