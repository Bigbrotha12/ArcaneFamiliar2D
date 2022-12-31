using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Environment;
using System.Collections.Generic;

public class SpellCanvas : UICanvasBase
{    
    [SerializeField] private TMP_Text spellsEquipped;
    [SerializeField] private TMP_Text spellsKnown;
    [SerializeField] private Toggle equipFilter;
    [SerializeField] private Transform content;
    [SerializeField] private GameObject containerPrefab;

    [SerializeField] private UICanvasBase DetailSpellCanvas;
    private int equippedCount;

    public override void OnEnable()
    {
        base.OnEnable();
        GameManager.Instance.Player.SpellBook.EquipSpellChange += () => RefreshSpellContainer(equipFilter.isOn);
        GameManager.Instance.Player.SpellBook.KnownSpellChange += () => RefreshSpellContainer(equipFilter.isOn);

        RefreshSpellContainer(equipFilter.isOn);
        equipFilter.onValueChanged.AddListener(RefreshSpellContainer);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        GameManager.Instance.Player.SpellBook.ClearEventListeners();
    }

    // index 0: close button
    protected override void HandleButtonSelect(int index)
    {
        switch (index)
        {
            case 0:
                ActivateCanvas(false);
                break;
            default:
                Debug.LogError("Invalid button index.");
                break;
        }
    }

    private void RefreshSpellContainer(bool equipFilter)
    {
        ClearCanvas();
        spellsEquipped.text = "Equipped: " + GameManager.Instance.Player.SpellBook.SpellsEquipped.Count.ToString();
        spellsKnown.text = "Known: " + GameManager.Instance.Player.SpellBook.SpellsKnown.Count.ToString();
        List<SpellSO> spellsToDisplay = equipFilter ? GameManager.Instance.Player.SpellBook.SpellsEquipped : GameManager.Instance.Player.SpellBook.SpellsKnown;
        
        foreach (SpellSO spell in spellsToDisplay)
        {
            GameObject container = GameObject.Instantiate(containerPrefab, content);
            container.transform.Find("Spell").GetComponent<Button>().onClick.AddListener(() => OnSpellClick(spell));
            container.transform.Find("Spell").Find("SpellFrame").Find("SpellIcon").GetComponent<Image>().sprite = spell.Icon;
            container.transform.Find("Spell").Find("SpellName").GetComponent<TMP_Text>().text = spell.ObjectName;
            container.transform.Find("Spell").Find("SpellCost").GetComponent<TMP_Text>().text = spell.Cost.ToString() + " MP";
        }
    }

    protected void OnSpellClick(SpellSO spell)
    {
        DetailSpellCanvas.ActivateCanvas(true);
        (DetailSpellCanvas as DSpellCanvas).DisplayDSpellCanvas(spell);
    }

    protected void ClearCanvas()
    {
        foreach (Transform container in content)
        {
            Destroy(container.gameObject);
        }
    }
}