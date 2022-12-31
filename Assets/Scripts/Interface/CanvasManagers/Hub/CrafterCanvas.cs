using Environment;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Characters;

// Script in charge of managing Craft scene
public class CrafterCanvas : UICanvasBase
{
    [SerializeField] private CrafterSO _craftController;

    public override void OnEnable()
    {
        base.OnEnable();
        _craftController.Greet();
    }

    // index 0: Craft Spell, index 1: Craft item, index 2: exit, index 3: spellCraft, index 4: spellRecipe, 
    // index 5: spellCancel, index 6: itemCraft, index 7: itemRecipe, index 8: itemCancel, 
    // index 9: closeIngredientSelect, index 10: closeRecipeSelect, index 11: spellComp1, index 12: spellComp2,
    // index 13: spellComp3, index 14: spellComp4, index 15: itemComp1, index 16: itemComp2, index 17: itemComp3
    // index 18: spellOutput, index 19: itemOutput
    protected override async void HandleButtonSelect(int index)
    {
        switch(index)
        {
            case 0:
                OpenCraftingPanel(CraftingMode.Spell);
                break;
            case 1:
                OpenCraftingPanel(CraftingMode.Item);
                break;
            case 2:
                ActivateCanvas(false);
                break;
            case 3:
                CanvasButtons[index].interactable = false;
                UpdateCraftResult(await _craftController.ValidateCrafting(CraftingMode.Spell));
                ClearIngredientSlot(CraftingMode.Spell);
                CanvasButtons[index].interactable = true;
                break;
            case 4:
                OpenRecipeSelector(CraftingMode.Spell);
                break;
            case 5:
                CloseCraftingPanel(CraftingMode.Spell);
                break;
            case 6:
                CanvasButtons[index].interactable = false;
                UpdateCraftResult(await _craftController.ValidateCrafting(CraftingMode.ITEM));
                ClearIngredientSlot(CraftingMode.Item);
                CanvasButtons[index].interactable = true;
                break;
            case 7:
                OpenRecipeSelector(CraftingMode.Item);
                break;
            case 8:
                CloseCraftingPanel(CraftingMode.Item);
                break;
            case 9:
                CloseIngredientSelector();
                break;
            case 10:
                CloseRecipeSelector();
                break;
            case 11:
                OpenIngredientSelector(CraftingMode.Spell, 0);
                break;
            case 12:
                OpenIngredientSelector(CraftingMode.Spell, 1);
                break;
            case 13:
                OpenIngredientSelector(CraftingMode.Spell, 2);
                break;
            case 14:
                OpenIngredientSelector(CraftingMode.Spell, 3);
                break;
            case 15:
                OpenIngredientSelector(CraftingMode.Item, 0);
                break;
            case 16:
                OpenIngredientSelector(CraftingMode.Item, 1);
                break;
            case 17:
                OpenIngredientSelector(CraftingMode.Item, 2);
                break;
            case 18:
                // Handled dynamically
                break;
            case 19:
                // Handled dynamically
                break;
            default:
                Debug.Log("Invalid button index.");
                break;
        }
    }

    [SerializeField] private GameObject ingredientSelector;
    [SerializeField] private Transform ingredientContainer;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject recipeSelector;
    [SerializeField] private Transform recipeContainer;
    [SerializeField] private GameObject recipePrefab;
    [SerializeField] private Sprite defaultItemSprite;
    [SerializeField] private Slider settingOneSlider;
    [SerializeField] private Slider settingTwoSlider;

    [SerializeField] private GameObject spellCraftPanel;
    [SerializeField] private Image runeIngredient;
    [SerializeField] private Image spellComponent1;
    [SerializeField] private Image spellComponent2;
    [SerializeField] private Image energyComponent;
    [SerializeField] private Image spellCraftOutput;

    [SerializeField] private GameObject itemCraftPanel;
    [SerializeField] private Image itemComponent1;
    [SerializeField] private Image itemComponent2;
    [SerializeField] private Image catalystComponent;
    [SerializeField] private Image itemCraftOutput;

   
    private List<ItemSO> playerCurrentIngredients = new List<ItemSO>();

    public void Start()
    {
        UpdateCurrentIngredients();
    }

    private void UpdateCurrentIngredients()
    {
        List<ItemSO> ingredients = new List<ItemSO>();
        foreach (KeyValuePair<ItemSO, int> stackedItem in GameManager.Instance.Player.Inventory.Backpack.Stack)
        {
            if(stackedItem.Key.Category == ItemCategory.CRAFTING)
            {
                ingredients.Add(stackedItem.Key);
            }
        }
        foreach (KeyValuePair<ItemSO, int> stackedItem in GameManager.Instance.Player.Inventory.Storage.Stack)
        {
            if(stackedItem.Key.Category == ItemCategory.CRAFTING)
            {
                ingredients.Add(stackedItem.Key);
            }
        }
        playerCurrentIngredients = ingredients;
    }

    private void UpdateCraftResult(IObjectHeader output)
    {
        if (output is null) { return; }
        if(output is ItemSO)
        {
            itemCraftOutput.sprite = output.Icon;
            itemCraftOutput.GetComponentInParent<Button>().onClick.AddListener(() => { itemCraftOutput.sprite = defaultItemSprite; });
        }
        else if(output is SpellSO)
        {
            spellCraftOutput.sprite = output.Icon;
            spellCraftOutput.GetComponentInParent<Button>().onClick.AddListener(() => { itemCraftOutput.sprite = defaultItemSprite; });
        }
    }

    private void OpenCraftingPanel(CraftingMode mode)
    {
        if (mode == CraftingMode.Spell)
        {
            spellCraftPanel.SetActive(true);
        }
        else if (mode == CraftingMode.Item)
        {
            itemCraftPanel.SetActive(true);
        }   
    }

    private void RefundCraftingIngredient(CraftingMode mode)
    {
        if (_craftController.IngredientsHeld is null) { return; }
        foreach (ItemSO item in _craftController.IngredientsHeld)
        {
            GameManager.Instance.Player.Inventory.AddToAny(item);
        }
        _craftController.ClearIngredients();
        ClearIngredientSlot(mode);
        UpdateCurrentIngredients();
    }

    private void ClearIngredientSlot(CraftingMode mode)
    {
        if (mode == CraftingMode.Spell)
        {
            runeIngredient.sprite = defaultItemSprite;
            spellComponent1.sprite = defaultItemSprite;
            spellComponent2.sprite = defaultItemSprite;
            energyComponent.sprite = defaultItemSprite;
        }
        else if(mode == CraftingMode.Item)
        {
            itemComponent1.sprite = defaultItemSprite;
            itemComponent2.sprite = defaultItemSprite;
            catalystComponent.sprite = defaultItemSprite;
        } 
    }

    private void CloseCraftingPanel(CraftingMode mode)
    {
        RefundCraftingIngredient(mode);

        if (mode == CraftingMode.Spell)
        {
            spellCraftPanel.SetActive(false);
        } else if (mode == CraftingMode.Item)
        {
            itemCraftPanel.SetActive(false);
        } 
    }

    private void OpenIngredientSelector(CraftingMode mode, int index)
    {
        // open and populate ingredient inventory
        ingredientSelector.SetActive(true);
        RefreshIngredientContainer(mode, index);
    }

    private void RefreshIngredientContainer(CraftingMode mode, int index)
    {
        foreach (Transform container in ingredientContainer)
        {
            Destroy(ingredientContainer.gameObject);
        }

        foreach (ItemSO item in playerCurrentIngredients)
        {
            GameObject container = GameObject.Instantiate(ingredientPrefab, ingredientContainer);
            container.GetComponent<Button>().onClick.AddListener(() => 
            {
                AddIngredientToSlot(item, mode, index);
                CloseIngredientSelector();
            });
            container.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.Icon;
        }
    }

    // index 0: rune, index 1: spellComp1, index 2: spellComp2, index 3: energy, index 4: itemComp1, 
    // index 5: itemComp2, index 6: catalyst
    private void AddIngredientToSlot(ItemSO item, CraftingMode mode, int index)
    {
        // Refund any existing ingredient
        if (_craftController.IngredientsHeld.Count > index && _craftController.IngredientsHeld[index] is not null) 
        { 
            GameManager.Instance.Player.Inventory.AddToAny(_craftController.IngredientsHeld[index]);
            _craftController.IngredientsHeld.RemoveAt(index);
        }

        // Remove ingredient and add to slot
        if(GameManager.Instance.Player.Inventory.RemoveFromAny(item))
        {
            _craftController.IngredientsHeld.Add(item);
            UpdateCurrentIngredients();
        }

        // Update slot sprite
        if(mode == CraftingMode.Spell)
        {
            switch(index)
            {
                case 0:
                    runeIngredient.sprite = item.Icon;
                    break;
                case 1:
                    spellComponent1.sprite = item.Icon;
                    break;
                case 2:
                    spellComponent2.sprite = item.Icon;
                    break;
                case 3:
                    energyComponent.sprite = item.Icon;
                    break;
                default:
                    Debug.Log("Invalid spell slot index.");
                    break;
            }     
        } 
        else if(mode == CraftingMode.ITEM)
        {
            switch (index)
            {
                case 0:
                    itemComponent1.sprite = item.Icon;
                    break;
                case 1:
                    itemComponent2.sprite = item.Icon;
                    break;
                case 2:
                    catalystComponent.sprite = item.Icon;
                    break;
                default:
                    Debug.Log("Invalid item slot index.");
                    break;
            }
        } 
    }

    private void CloseIngredientSelector()
    {
        for (int i = 0; i < ingredientContainer.childCount; i++)
        {
            Destroy(ingredientContainer.GetChild(i).gameObject);
        }
        ingredientSelector.SetActive(false);
    }

    // index 0: spell, index 1: item
    private void OpenRecipeSelector(CraftingMode mode)
    {
        // open and populate ingredient inventory
        recipeSelector.SetActive(true);
        RefreshRecipeContainer(mode);
    }

    private void RefreshRecipeContainer(CraftingMode mode)
    {
        for (int i = 0; i < recipeContainer.childCount; i++)
        {
            Destroy(recipeContainer.GetChild(i).gameObject);
        }

        foreach (RecipeSO recipe in GameManager.Instance.Player.RecipeBook.RecipesKnown)
        {
            if(mode == CraftingMode.Spell && recipe.CraftType == CraftingMode.Spell && recipe.IsCraftable(playerCurrentIngredients))
            {
                GameObject container = GameObject.Instantiate(recipePrefab, recipeContainer);
                container.GetComponent<Button>().onClick.AddListener(() => 
                {
                    AddRecipeToSlot(recipe, mode);
                    CloseRecipeSelector();
                });
                
                container.transform.Find("Label").GetComponent<TMP_Text>().text = recipe.ObjectName;
                container.transform.Find("Border").Find("RecipeIcon").GetComponent<Image>().sprite = recipe.Product.Icon;
                Transform recipeIngredientContainer = container.transform.Find("Ingredients");
                
                foreach (ItemSO item in recipe.Ingredients)
                {
                    GameObject itemContainer = GameObject.Instantiate(ingredientPrefab, recipeIngredientContainer);
                    itemContainer.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.Icon;
                }
                ItemSO checkSumItem = recipe.GetCheckSumItem();
                GameObject checkSumContainer = GameObject.Instantiate(ingredientPrefab, recipeIngredientContainer);
                checkSumContainer.transform.Find("ItemIcon").GetComponent<Image>().sprite = checkSumItem.Icon;
            }

            if(mode == CraftingMode.Item && recipe.CraftType == CraftingMode.Item && recipe.IsCraftable(playerCurrentIngredients))
            {
                GameObject container = GameObject.Instantiate(recipePrefab, recipeContainer);
                container.GetComponent<Button>().onClick.AddListener(() => 
                {
                    AddRecipeToSlot(recipe, mode);
                    CloseRecipeSelector();
                });

                container.transform.Find("Label").GetComponent<TMP_Text>().text = recipe.ObjectName;
                container.transform.Find("Border").Find("RecipeIcon").GetComponent<Image>().sprite = recipe.Product.Icon;
                Transform recipeIngredientContainer = container.transform.Find("Ingredients");
                
                foreach (ItemSO item in recipe.Ingredients)
                {
                    GameObject itemContainer = GameObject.Instantiate(ingredientPrefab, recipeIngredientContainer);
                    itemContainer.transform.Find("ItemIcon").GetComponent<Image>().sprite = item.Icon;
                }
                ItemSO checkSumItem = recipe.GetCheckSumItem();
                GameObject checkSumContainer = GameObject.Instantiate(ingredientPrefab, recipeIngredientContainer);
                checkSumContainer.transform.Find("ItemIcon").GetComponent<Image>().sprite = checkSumItem.Icon;
            }
        }
    }

    private void AddRecipeToSlot(RecipeSO recipe, CraftingMode mode)
    {
        // Refund existing items
        RefundCraftingIngredient(mode);
        _craftController.ClearIngredients();

        // Transfer items from player inventory, otherwise refund.
        ItemSO checkSumItem = recipe.GetCheckSumItem();
        foreach (ItemSO item in recipe.Ingredients)
        {
            if (GameManager.Instance.Player.Inventory.RemoveFromAny(item))
            {
                _craftController.IngredientsHeld.Add(item);
            }
            else
            {
                RefundCraftingIngredient(mode);
                ClearIngredientSlot(mode);
                return;
            }
        }
        if (GameManager.Instance.Player.Inventory.RemoveFromAny(checkSumItem))
        {
            _craftController.IngredientsHeld.Add(checkSumItem);
        }
        else
        {
            RefundCraftingIngredient(mode);
            ClearIngredientSlot(mode);
            return;
        }      
        UpdateCurrentIngredients();
            
        // Update slot sprites
        if(mode == CraftingMode.Spell)
        {
            runeIngredient.sprite = _craftController.IngredientsHeld[0].Icon;
            spellComponent1.sprite = _craftController.IngredientsHeld[1].Icon;
            spellComponent2.sprite = _craftController.IngredientsHeld[2].Icon;
            energyComponent.sprite = _craftController.IngredientsHeld[3].Icon;
        } 
        else if (mode == CraftingMode.Item)
        {
            itemComponent1.sprite = _craftController.IngredientsHeld[0].Icon;
            itemComponent2.sprite = _craftController.IngredientsHeld[1].Icon;
            catalystComponent.sprite = _craftController.IngredientsHeld[2].Icon;        
        }
    }

    public void CloseRecipeSelector()
    {
        for (int i = 0; i < recipeContainer.childCount; i++)
        {
            Destroy(recipeContainer.GetChild(i).gameObject);
        }
        recipeSelector.SetActive(false);
    }
}