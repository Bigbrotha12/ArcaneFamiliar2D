using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Environment;
using System.Collections.Generic;

public class InventoryCanvas : UICanvasBase
{
    [SerializeField] private TMP_Text numPouchItems;
    [SerializeField] private Transform pouchContent;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject itemPanel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDesc;

    public override void OnEnable()
    {
        base.OnEnable();
        RefreshPouchBox();
    }

    // index 0: close button, index 1: use button, index 2: discard button
    protected override void HandleButtonSelect(int index)
    {
        switch (index)
        {
            case 0:
                ActivateCanvas(false);
                break;
            case 1:
                // Handled via script below
                break;
            case 2:
                // Handled via script below
                break;
            default:
                Debug.LogError("Button event not handled: " + index.ToString());
                break;
        }
    }

    private void OnItemClick(ItemSO item)
    {
        itemPanel.SetActive(true);
        itemIcon.sprite = item.Icon;
        itemName.text = item.ObjectName;
        itemDesc.text = item.Description;
        CanvasButtons[1].onClick.AddListener(() => 
        { 
            GameManager.Instance.Player.Inventory.UseItem(item, InventoryStack.POUCH);
            RefreshPouchBox();
        });
        CanvasButtons[2].onClick.AddListener(() => 
        { 
            GameManager.Instance.Player.Inventory.RemoveItem(item, InventoryStack.POUCH);
            RefreshPouchBox();
        });
    }

    private void RefreshPouchBox()
    {
        ClearCanvas();
        int pouchItem = GameManager.Instance.Player.Inventory.Backpack.Stack.Count;
        int pouchLimit = GameManager.Instance.Player.Inventory.Backpack.SlotLimit;
        numPouchItems.text = "Slots: " + pouchItem.ToString() + " / " + pouchLimit.ToString();

        foreach (KeyValuePair<ItemSO, int> item in GameManager.Instance.Player.Inventory.Backpack.Stack)
        {
            GameObject container = GameObject.Instantiate(itemPrefab, pouchContent);
            container.transform.GetComponent<Button>().onClick.AddListener(() => { OnItemClick(item.Key); });
            container.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.Key.Icon;
            
            if(item.Value > 1)
            {
                container.transform.Find("Stack").Find("StackSize").GetComponent<TMP_Text>().text = item.Value.ToString();
                container.transform.Find("Stack").gameObject.SetActive(true);
            }
        }

        // fill remaining slots with empty boxes
        for (int i = 0; i < pouchLimit - pouchItem; i++)
        {
            GameObject.Instantiate(itemPrefab, pouchContent);
        }
    }

    private void ClearCanvas()
    {
        // Destroy item boxes
        foreach (Transform container in pouchContent)
        {
            Destroy(container.gameObject);
        }

        // Disable item panel
        itemPanel.SetActive(false);

    }
}