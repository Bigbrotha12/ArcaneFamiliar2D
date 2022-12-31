using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Characters
{
    [System.Serializable]
    public class Inventory
    {
        public StackableContainer Storage = new StackableContainer(20, 50);
        public StackableContainer Backpack = new StackableContainer(5, 10);
     
        public Inventory(List<ItemSO> initialItems)
        {
            List<ItemSO> items = initialItems is not null ? initialItems : new List<ItemSO>();
            foreach (ItemSO item in items)
            {
                Storage.AddItem(item);
            }
        }

        public void UseItem(ItemSO item, InventoryStack location)
        {
            if (item is null) { return; }
            switch (location)
            {
                case InventoryStack.Storage:
                    // if(Storage.RemoveItem(item))
                    // {
                    //     foreach (Effect effect in item.ItemEffects)
                    //     {
                    //         effect.Activate();
                    //     }
                    // }
                    break;
                case InventoryStack.Pouch:  
                    // if(Backpack.RemoveItem(item))
                    // {
                    //     foreach (Effect effect in item.ItemEffects)
                    //     {
                    //         effect.Activate();
                    //     }
                    // }
                    break;
                default:
                    Debug.LogError("Invalid location index.");
                    break;
            }
        }

        public bool AddItem(ItemSO item, InventoryStack location)
        {
            if (item is null) { return false; }
            switch (location)
            {
                case InventoryStack.Storage:
                    return Storage.AddItem(item);
                case InventoryStack.Pouch:
                    return Backpack.AddItem(item);
                default:
                    return false;
            }
        }

        public bool RemoveItem(ItemSO item, InventoryStack location)
        {
            if (item is null) { return false; }
            switch (location)
            {
                case InventoryStack.Storage:
                    return Storage.RemoveItem(item);
                case InventoryStack.Pouch:  
                    return Backpack.RemoveItem(item);
                default:
                    return false;
            }
        }

        public bool AddToAny(ItemSO item)
        {
            if (item is null) { return false; }
            return (Storage.AddItem(item) || Backpack.AddItem(item));
        }

        // Removes item from backpack or storage
        public bool RemoveFromAny(ItemSO item)
        {
            if (item is null) { return false; }
            return (Backpack.RemoveItem(item) || Storage.RemoveItem(item));
        }

        // int 0: Storage, int 1: Backpack
        public void TransferItem(InventoryStack from, ItemSO item)
        {
            if (item is null) { return; }
            
            switch(from)
            {
                case InventoryStack.Storage:
                    if(Storage.RemoveItem(item)) { Backpack.AddItem(item); }
                    break;
                case InventoryStack.Pouch:
                    if(Backpack.RemoveItem(item)) { Storage.AddItem(item); }
                    break;
                default:
                    break;
            }
        }

        public bool HasItem(ItemSO item)
        {
            foreach (KeyValuePair<ItemSO, int> stackedItem in Backpack.Stack)
            {
                if(stackedItem.Key.Id == item.Id)
                {
                    return true;
                }
            }

            foreach (KeyValuePair<ItemSO, int> stackedItem in Storage.Stack)
            {
                if(stackedItem.Key.Id == item.Id)
                {
                    return true;
                }
            }
            return false;
        }

        public List<ItemSO> GetCombinedInventory()
        {
            List<ItemSO> CombinedInventory = new List<ItemSO>();
            CombinedInventory.AddRange(Storage.GetFlatContainer());
            CombinedInventory.AddRange(Backpack.GetFlatContainer());
            return CombinedInventory;
        }
    }

    public class StackableContainer
    {
        public Dictionary<ItemSO, int> Stack = new Dictionary<ItemSO, int>();
        public int StackLimit { get; private set; }
        public int SlotLimit { get; private set; }

        public StackableContainer(int stackSize, int slotSize)
        {
            StackLimit = Mathf.Max(0, stackSize);
            SlotLimit = Mathf.Max(0, slotSize);
        }

        public bool AddItem(ItemSO item)
        {
            if (item is null) { return false; }
            if(Stack.ContainsKey(item))
            {
                if(Stack[item] < StackLimit)
                {
                    Stack[item]++;
                    return true;
                } else
                {
                    GameManager.Instance.UInterface.QueueAlert("Item max stack size reached.");
                    return false;
                }
            } else if(Stack.Count < SlotLimit)
            {
                Stack.Add(item, 1);
                return true;
            } else
            {
                GameManager.Instance.UInterface.QueueAlert("Inventory size limit reached.");
                return false;
            }
        }

        public bool RemoveItem(ItemSO item)
        {
            if (item is null) { return false; }
            if(Stack.ContainsKey(item))
            {
                if(Stack[item] > 1)
                {
                    Stack[item]--;
                    return true;
                } else
                {
                    return Stack.Remove(item);
                }
            } else
            {
                return false;
            }
        }

        public List<ItemSO> GetFlatContainer()
        {
            List<ItemSO> result = new List<ItemSO>();
            foreach (KeyValuePair<ItemSO, int> unit in Stack)
            {
                for (int i = 0; i < unit.Value; i++)
                {
                    result.Add(unit.Key);
                }
            }
            return result;
        }
    }
}