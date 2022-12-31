using UnityEngine;
using UnityEngine.UI;
using Environment;
using TMPro;

public class DSpellCanvas : UICanvasBase
{
    [SerializeField] private Image spellIcon;
    [SerializeField] private TMP_Text spellName;
    [SerializeField] private TMP_Text spellCost;
    [SerializeField] private TMP_Text spellPower;
    [SerializeField] private TMP_Text spellTarget;
    [SerializeField] private TMP_Text spellDescription;

    [SerializeField] private Transform effectContainer;
    [SerializeField] private GameObject effectPanel;

    // index 0: Close, index 1: Equip/UnEquip, index 2: Cancel
    protected override void HandleButtonSelect(int index)
    {
        switch (index)
        {
            case 0:
                ActivateCanvas(false);
                break;
            case 1:
                break;
            case 2:
                ActivateCanvas(false);
                break;
            default:
                Debug.LogError("Button event not handled: " + index.ToString());
                break;
        }
    }

    public void DisplayDSpellCanvas(SpellSO spell)
    {
        DisplaySpellData(spell);
        DisplaySpellEffect(spell);
    }

    private void DisplaySpellData(SpellSO spell)
    {
        spellIcon.sprite = spell.Icon;
        spellName.text = spell.ObjectName;
        spellCost.text = spell.Cost.ToString() + " MP";
        spellPower.text = spell.Power.ToString();
        spellTarget.text = spell.SpellTarget.ToString(); 
        spellDescription.text = spell.Description;

        bool isEquipped = GameManager.Instance.Player.SpellBook.isEquipped(spell);
        CanvasButtons[1].transform.Find("OptionButtonText").GetComponent<TMP_Text>().text = isEquipped ? "UnEquip" : "Equip";
        if (isEquipped)
        {
            CanvasButtons[1].onClick.AddListener(() => 
            { 
                GameManager.Instance.Player.SpellBook.UnEquipSpell(spell);
                ActivateCanvas(false);
            });
        } else
        {
            CanvasButtons[1].onClick.AddListener(() => 
            { 
                GameManager.Instance.Player.SpellBook.EquipSpell(spell);
                ActivateCanvas(false); 
            });
        }
        
    }

    private void DisplaySpellEffect(SpellSO spell)
    {
        // foreach (Effect effect in spell.SpellEffects)
        // {
        //     GameObject container = GameObject.Instantiate(effectPanel, effectContainer);
        //     //container.Find("EffectLabel").GetComponent<TMP_Text>().text = effect.ObjectName;
        //     //container.Find("DescPanel").Find("EffectDesc").GetComponent<TMP_Text>().text = effect.Description;
        // }
    }
}