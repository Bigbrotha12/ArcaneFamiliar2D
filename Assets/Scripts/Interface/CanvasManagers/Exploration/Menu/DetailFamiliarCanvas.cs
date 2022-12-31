using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Characters;

public class DetailFamiliarCanvas : UICanvasBase
{
    #region Displays
    [SerializeField] private TMP_Text familiarName;
    [SerializeField] private TMP_Text familiarDescription;
    [SerializeField] private Image familiarIcon;

    [SerializeField] private TMP_Text familiarId;
    [SerializeField] private TMP_Text familiarElement;
    [SerializeField] private TMP_Text familiarRarity;
    [SerializeField] private TMP_Text familiarGeneration;

    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Image hpBar;
    [SerializeField] private TMP_Text mpText;
    [SerializeField] private Image mpBar;
    [SerializeField] private TMP_Text atkText;
    [SerializeField] private Image atkBar;
    [SerializeField] private TMP_Text defText;
    [SerializeField] private Image defBar;
    [SerializeField] private TMP_Text arcText;
    [SerializeField] private Image arcBar;
    [SerializeField] private TMP_Text spdText;
    [SerializeField] private Image spdBar;

    [SerializeField] private TMP_Text ab01Name;
    [SerializeField] private TMP_Text ab01Desc;
    [SerializeField] private TMP_Text ab02Name;
    [SerializeField] private TMP_Text ab02Desc;
    [SerializeField] private TMP_Text ab03Name;
    [SerializeField] private TMP_Text ab03Desc;
    [SerializeField] private TMP_Text ab04Name;
    [SerializeField] private TMP_Text ab04Desc;
    #endregion

    // index 0: close
    protected override void HandleButtonSelect(int index)
    {
        switch (index)
        {
            case 0:
                ActivateCanvas(false);
                break;
            default:
                Debug.LogError("Button event not handled: " + index.ToString());
                break;
        }
    }

    public void DisplayFamiliarDetails(FamiliarSO familiar)
    {
        RefreshHeaderPanel(familiar);
        RefreshMetadataPanel(familiar);
        RefreshStatsPanel(familiar);
        RefreshAbilityPanel(familiar);
    }

    private void RefreshHeaderPanel(FamiliarSO familiar)
    {
        familiarName.text = familiar.ObjectName;
        familiarDescription.text = familiar.Description;
        familiarIcon.sprite = familiar.Icon;
    }

    private void RefreshMetadataPanel(FamiliarSO familiar)
    {
        familiarId.text = familiar.Id.ToString();
        familiarElement.text = familiar.ElementType.ToString();
        familiarRarity.text = familiar.Rarity.ToString();
        familiarGeneration.text = familiar.Generation.ToString();
    }

    private void RefreshStatsPanel(FamiliarSO familiar)
    {
        hpText.text = familiar.HP.ToString();
        hpBar.fillAmount = (float) familiar.HP / (float) FamiliarType.HP_MAX;
        mpText.text = familiar.MP.ToString();
        mpBar.fillAmount = (float) familiar.MP / (float) FamiliarType.MP_MAX;
        atkText.text = familiar.Attack.ToString();
        atkBar.fillAmount = (float) familiar.Attack / (float) FamiliarType.ATK_MAX;
        defText.text = familiar.Defense.ToString();
        defBar.fillAmount = (float) familiar.Defense / (float) FamiliarType.DEF_MAX;
        arcText.text = familiar.Arcane.ToString();
        arcBar.fillAmount = (float) familiar.Arcane / (float) FamiliarType.ARC_MAX;
        spdText.text = familiar.Speed.ToString();
        spdBar.fillAmount = (float) familiar.Speed / (float) FamiliarType.SPD_MAX;
    }

    private void RefreshAbilityPanel(FamiliarSO familiar)
    {
        ab01Name.text = familiar.Abilities[0].ObjectName;
        ab01Desc.text = familiar.Abilities[0].Description;
        ab02Name.text = familiar.Abilities[1].ObjectName;
        ab02Desc.text = familiar.Abilities[1].Description;
        ab03Name.text = familiar.Abilities[2].ObjectName;
        ab03Desc.text = familiar.Abilities[2].Description;
        ab04Name.text = familiar.Abilities[3].ObjectName;
        ab04Desc.text = familiar.Abilities[3].Description;
    }
}