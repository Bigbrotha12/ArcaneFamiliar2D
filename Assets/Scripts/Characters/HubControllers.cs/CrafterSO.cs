using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Environment;

namespace Characters
{
    [CreateAssetMenu(fileName = "New Crafter", menuName = "Characters/Hub/New Crafter", order = 0)]
    public class CrafterSO : NPCSO
    {
        private List<ItemSO> _ingredientsHeld;
        public List<ItemSO> IngredientsHeld
        {
            get 
            { 
                if(_ingredientsHeld is null) 
                {
                    _ingredientsHeld = new List<ItemSO>();
                }
                return _ingredientsHeld;
            }
            private set { _ingredientsHeld = value; }
        }
        [SerializeField] private bool _allowVariants;
        public int itemSlots;
        public int spellSlots;

        private int GetKey()
        {
            return GameManager.Instance.Player.Atlas.HubInitData.GetSeedOne();
        }

        public void ClearIngredients()
        {
            IngredientsHeld.Clear();
        }

        public async Task<IObjectHeader> ValidateCrafting(CraftingMode mode, int option1 = 0, int option2 = 0)
        {
            // Check ingredients available
            if((mode == CraftingMode.Spell && IngredientsHeld.Count < spellSlots) || (mode == CraftingMode.Item && IngredientsHeld.Count < itemSlots))
            {
                Speak(new List<string>(){"You're missing some components. That won't do."});
                return null;
            }

            GameManager.Instance.UInterface.QueueAlert("Crafting in progress...", 0);

            // Generate Variant
            int variant = 2 * option1 + option2;

            // Get Craft values
            int craftValue = 0;
            foreach (ItemSO item in _ingredientsHeld) { craftValue += item.CraftValue; }

            // Checksum
            int key = GetKey();
            if ((craftValue + key) % 10 != 0) 
            {
                Debug.Log("Checksum check failed.\nValue: " + craftValue.ToString() + "key: " + key.ToString());
                return await ExecuteCrafting(mode, false); 
            }

            // Generate Item
            int baseCraftValue = craftValue - _ingredientsHeld[_ingredientsHeld.Count-1].CraftValue;
            IObjectHeader newObject = null;
            foreach (RecipeSO recipe in GameManager.Instance.WorldAtlas.RecipesRegistry)
            {
                if(recipe.CraftValue == baseCraftValue)
                {
                    newObject = recipe.Product as IObjectHeader;
                    IObjectHeader variantProduct = recipe.GetVariant(variant, mode);
                    if (_allowVariants && variantProduct is not null) { newObject = variantProduct; }
                    break;
                }
            }
            if(newObject is null || newObject.Id == 0) 
            { 
                return await ExecuteCrafting(mode, false); 
            }
            return await ExecuteCrafting(mode, true, newObject); 
        }

        private async Task<IObjectHeader> ExecuteCrafting(CraftingMode mode, bool success, IObjectHeader newObject = null)
        {
            if(success)
            {
                if (mode == CraftingMode.Spell && newObject is SpellSO) 
                { 
                    GameManager.Instance.Player.SpellBook.LearnNewSpell(newObject as SpellSO); 
                    if(await GameManager.Instance.SaveGame(false))
                    {
                        GameManager.Instance.UInterface.QueueAlert("You created a new spell. \n" + newObject.ObjectName, 0);
                        return newObject;
                    } 
                    else
                    {
                        GameManager.Instance.UInterface.QueueAlert("Error: Could not establish communication with server./nEnsure you're connected to a network and try again.", 0);
                        GameManager.Instance.Player.SpellBook.RemoveSpell(newObject as SpellSO);
                        foreach (ItemSO item in _ingredientsHeld)
                        {
                            GameManager.Instance.Player.Inventory.AddToAny(item);
                        }
                    }
                }
                else if (mode == CraftingMode.Item && newObject is ItemSO) 
                { 
                    GameManager.Instance.Player.Inventory.AddToAny(newObject as ItemSO);
                    if(await GameManager.Instance.SaveGame(false))
                    {
                        GameManager.Instance.UInterface.QueueAlert("You created a new item. \n" + newObject.ObjectName, 0);
                        return newObject;
                    } 
                    else
                    {
                        GameManager.Instance.UInterface.QueueAlert("Error: Could not establish communication with server./nEnsure you're connected to a network and try again.", 0);
                        GameManager.Instance.Player.Inventory.Storage.RemoveItem(newObject as ItemSO);
                        foreach (ItemSO item in _ingredientsHeld)
                        {
                            GameManager.Instance.Player.Inventory.AddToAny(item);
                        } 
                    } 
                }
            } 
            else
            {
                if(await GameManager.Instance.SaveGame(false))
                {
                    GameManager.Instance.Player.Speak(new List<string>(){"Oops, that didn't work as expected."});
                    GameManager.Instance.UInterface.QueueAlert("Crafting attempt failed.", 0);
                } 
                else
                {
                    GameManager.Instance.UInterface.QueueAlert("Error: Could not establish communication with server./nEnsure you're connected to a network and try again.", 0);
                    
                    foreach (ItemSO item in _ingredientsHeld)
                    {
                        GameManager.Instance.Player.Inventory.AddToAny(item);
                    } 
                }   
            }
            _ingredientsHeld.Clear();
            return null;
        }

    }
}