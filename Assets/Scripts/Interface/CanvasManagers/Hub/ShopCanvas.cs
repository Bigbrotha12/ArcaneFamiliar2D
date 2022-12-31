using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Characters;
using Environment;

public class ShopCanvas : UICanvasBase
{
    [SerializeField] private MerchantSO _shopController;

    public override void OnEnable()
    {
        base.OnEnable();
        UpdateWalletBalance();
        _shopController.TransactionClosed += UpdateWalletBalance;
        _shopController.TransactionClosed += RefreshBuyContainer;
        _shopController.TransactionClosed += RefreshPlayerContainer;
        _shopController.Greet();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _shopController.TransactionClosed -= UpdateWalletBalance;
    }

    // index 0: Buy, index 1: Sell, index 2: Exit
    protected override void HandleButtonSelect(int index)
    {
        switch(index)
        {
            case 0:
                ActivateBuyWindow();
                break;
            case 1:
                ActivateSellWindow();
                break;
            case 2:
                ActivateCanvas(false);
                break;
            case 3:
                ClearBuyWindow();
                break;
            case 4:
                ClearSellWindow();
                break;
            default:
                Debug.Log("Invalid button index.");
                break;
        }
    }
 
    [SerializeField] private GameObject shopItemContainer;
    [SerializeField] private GameObject playerItemPrefab;
    [SerializeField] private GameObject walletDisplay;

    private void HandleItemClick(bool buying, ItemSO item)
    {
        if(buying)
        {
            _shopController.CheckShopItem(item);
        } else
        {
            _shopController.CheckPlayerItem(item);
        }
    }
   
    private void UpdateWalletBalance()
    {
        shopKeeperMoney.text = _shopController.GetShopWallet().Money.ToString() + " G";
        playerMoney.text = GameManager.Instance.Player.Wallet.Money.ToString() + " G";
    }

    public void ReplenishSupplies()
    {   
        _shopController.GetShopWallet().Money = GameManager.Instance.Player.Level.PlayerLevel * 200 + Random.Range(0, 100);
        _shopController.GetShopInventory();
    }    

    #region Buy Panel
    [SerializeField] private GameObject shopBuyPanel;
    [SerializeField] private Transform shopBuyContainer;
    [SerializeField] private TMP_Text playerMoney;

    public void ActivateBuyWindow()
    {
        shopBuyPanel.SetActive(true);
        walletDisplay.SetActive(true);
        RefreshBuyContainer();
    }

    private void RefreshBuyContainer()
    {
        // Clear container first
        if(shopBuyContainer is not null)
        {
            foreach (Transform container in shopBuyContainer)
            {
                Destroy(container.gameObject);
            }
        }
       

        foreach (KeyValuePair<ItemSO, int> stackedItem in _shopController.GetShopInventory().Storage.Stack)
        {
            int shopPrice = _shopController.GetBuyPrice(stackedItem.Key);
            GameObject container = GameObject.Instantiate(shopItemContainer, shopBuyContainer);
            container.transform.Find("Item").Find("ItemIcon").GetComponent<Image>().sprite = stackedItem.Key.Icon;
            container.transform.Find("Item").Find("ItemName").GetComponent<TMP_Text>().text = stackedItem.Key.ObjectName;
            container.transform.Find("Item").Find("ItemValue").GetComponent<TMP_Text>().text = shopPrice.ToString();

            container.transform.Find("Item").GetComponent<Button>().onClick.AddListener(() => HandleItemClick(true, stackedItem.Key));

            if(stackedItem.Value > 1)
            {
                container.transform.Find("Item").Find("StackPanel").gameObject.SetActive(true);
                container.transform.Find("Item").Find("StackPanel").Find("StackSize").GetComponent<TMP_Text>().text = stackedItem.Value.ToString();
            }
        }
    }
    
    public void ClearBuyWindow()
    {
        if(shopBuyContainer is not null)
        {
            foreach (Transform container in shopBuyContainer)
            {
                Destroy(container.gameObject);
            }
        }
        
        shopBuyPanel.SetActive(false);
        walletDisplay.SetActive(false);
    }
    #endregion

    #region Sell Panel
    [SerializeField] private GameObject shopSellPanel;
    [SerializeField] private Transform playerPouchContainer;
    [SerializeField] private Transform playerStorageContainer;
    [SerializeField] private TMP_Text shopKeeperMoney;

    public void ActivateSellWindow()
    {
        shopSellPanel.SetActive(true);
        walletDisplay.SetActive(true);
        RefreshPlayerContainer();
    }

    private void RefreshPlayerContainer()
    {
        // Clear container first
        foreach (Transform container in playerPouchContainer)
        {
            Destroy(container.gameObject);
        }

        foreach (Transform container in playerStorageContainer)
        {
            Destroy(container.gameObject);
        }
        
        
        // Add pouch items
        Debug.Log("Pouch: "+GameManager.Instance.Player.Inventory.Backpack.Stack.Count.ToString());
        foreach (KeyValuePair<ItemSO, int> stackedItem in GameManager.Instance.Player.Inventory.Backpack.Stack)
        {
            
            int playerPrice = _shopController.GetSellPrice(stackedItem.Key);
            GameObject container = GameObject.Instantiate(playerItemPrefab, playerPouchContainer);
            container.transform.Find("ItemIcon").GetComponent<Image>().sprite = stackedItem.Key.Icon;

            container.transform.GetComponent<Button>().onClick.AddListener(() => HandleItemClick(false, stackedItem.Key));

            if(stackedItem.Value > 1)
            {
                container.transform.Find("Stack").gameObject.SetActive(true);
                container.transform.Find("Stack").Find("StackSize").GetComponent<TMP_Text>().text = stackedItem.Value.ToString();
            }
        }

        // Add Storage items
        Debug.Log("Pouch: "+GameManager.Instance.Player.Inventory.Storage.Stack.Count.ToString());
        foreach (KeyValuePair<ItemSO, int> stackedItem in GameManager.Instance.Player.Inventory.Storage.Stack)
        {
            int playerPrice = _shopController.GetSellPrice(stackedItem.Key);
            GameObject container = GameObject.Instantiate(playerItemPrefab, playerStorageContainer);
            container.transform.Find("ItemIcon").GetComponent<Image>().sprite = stackedItem.Key.Icon;
       
            container.transform.GetComponent<Button>().onClick.AddListener(() => HandleItemClick(false, stackedItem.Key));

            if(stackedItem.Value > 1)
            {
                container.transform.Find("Stack").gameObject.SetActive(true);
                container.transform.Find("Stack").Find("StackSize").GetComponent<TMP_Text>().text = stackedItem.Value.ToString();
            }
        }
    }
    
    public void ClearSellWindow()
    {
        foreach (Transform container in playerPouchContainer)
        {
            Destroy(container.gameObject);
        }

        foreach (Transform container in playerStorageContainer)
        {
            Destroy(container.gameObject);
        }
        

        shopSellPanel.SetActive(false);
        walletDisplay.SetActive(false);
    }
    #endregion

  
}