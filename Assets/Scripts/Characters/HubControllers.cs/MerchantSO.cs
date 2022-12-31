
using System;
using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Characters
{
    [CreateAssetMenu(fileName = "New Merchant", menuName = "Characters/Hub/New Merchant", order = 0)]
    public class MerchantSO : NPCSO
    {
        private enum PurchaseSelection
        {
            NONE,
            BUY,
            SELL
        }
        // A shop keeper NPC accept two player actions:
        // Buy - Lets player purchase from shop inventory.
        // Sell - Allow player to sell item to merchant.
        [SerializeField] private List<ItemSO> shopInventory;
        [SerializeField] private bool canBuy;
        [SerializeField] private bool canSell;
        [SerializeField] private float _merchantSpread;
        public float MerchantSpread 
        {
            get { return _merchantSpread; }
            set {_merchantSpread = Mathf.Clamp(value, 0, 1.0f); }
        }
        public Wallet ShopWallet;
        private Inventory managedInventory;
        public event Action TransactionClosed;

        public Inventory GetShopInventory()
        {
            if(managedInventory is null)
            {
                managedInventory = new Inventory(shopInventory);
            }
            return managedInventory;
        }

        public Wallet GetShopWallet()
        {
            if(ShopWallet is null)
            {
                ShopWallet = new Wallet(GameManager.Instance.Player.Level.PlayerLevel * 200 + UnityEngine.Random.Range(0, 100));
            }
            return ShopWallet;
        }

        public void CheckShopItem(ItemSO item)
        {
            string[] options = { "Buy", "Cancel" };
            GameManager.Instance.UInterface.QueueObject(item, options, (item, index) => HandlePurchaseSelection(item as ItemSO, index, PurchaseSelection.BUY));
        }

        public void CheckPlayerItem(ItemSO item)
        {
            if(item.Category is ItemCategory.PERMANENT || item.Category is ItemCategory.LEARNABLE)
            {
                Speak(new List<string>() { "Sorry, I don't deal on those items." });
                return;
            }

            int offer = GetSellPrice(item);
            Speak(new List<string>() { "For that, the best I can offer you is " + offer.ToString() + " G."});
            string[] options = { "Sell", "Cancel" };
            GameManager.Instance.UInterface.QueueObject(item, options, (item, index) => HandlePurchaseSelection(item as ItemSO, index, PurchaseSelection.SELL));
        }

        public int GetBuyPrice(ItemSO item)
        {
            return (int)((float) item.Cost * (1.0f + MerchantSpread));
        } 

        //
        public int GetSellPrice(ItemSO item)
        {
            return (int)((float) item.Cost * (1.0f - MerchantSpread));
        }

        // index 1: Cancel
        private void HandlePurchaseSelection(ItemSO item, int index, PurchaseSelection choice)
        {
            if (index == 1) { return; }
            switch (choice)
            {
                case PurchaseSelection.BUY:
                    SellToPlayer(item);
                    break;
                case PurchaseSelection.SELL:
                    BuyFromPlayer(item);
                    break;
                default:
                    Debug.LogError("Invalid purchase choice.");
                    break;
            }
        }

        private void SellToPlayer(ItemSO item)
        {
            if(ShopWallet is null || managedInventory is null)
            {
                Debug.LogError("Error: Shop Keep not initialized.");
            }

            // Check funds
            int price = GetBuyPrice(item);

            if(price > GameManager.Instance.Player.Wallet.Money)
            {
                Speak(new List<string>() { "Looks like you don't have enough money." });
                return;
            }

            if(GameManager.Instance.Player.Inventory.AddToAny(item))
            {
                managedInventory.RemoveFromAny(item);
                GameManager.Instance.Player.Wallet.SpendMoney(price);
                ShopWallet.EarnMoney(price);
                TransactionClosed.Invoke();
            }
            else
            {
                Speak(new List<string>() { "Seems you can't carry anymore of this item." });
                return;
            }
        }

        private void BuyFromPlayer(ItemSO item)
        {
            if(ShopWallet is null || managedInventory is null)
            {
                Debug.LogError("Error: Shop Keep not initialized.");
            }

            // Check funds
            int price = GetSellPrice(item);

            if(price > ShopWallet.Money)
            {
                Speak(new List<string>() { "I can't afford this..." });
                return;
            }

            if(GameManager.Instance.Player.Inventory.RemoveFromAny(item))
            {
                managedInventory.AddToAny(item);
                GameManager.Instance.Player.Wallet.EarnMoney(price);
                ShopWallet.SpendMoney(price);
                TransactionClosed.Invoke();
            }
            else
            {
                Speak(new List<string>() { "Odd... You don't seem to have this item." });
                return;
            }

        }
    }
}