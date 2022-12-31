using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Characters;

public class StatusCanvas : UICanvasBase
{
    // Familiars
    [SerializeField] private Image mainIcon;
    [SerializeField] private Image supportIcon;

    // Statistics
    [SerializeField] private Image mainHPBar;
    [SerializeField] private Image mainMPBar;
    [SerializeField] private Image mainATKBar;
    [SerializeField] private Image mainDEFBar;
    [SerializeField] private Image mainARCBar;
    [SerializeField] private Image mainSPDBar;
    [SerializeField] private Image supportHPBar;
    [SerializeField] private Image supportMPBar;
    [SerializeField] private Image supportATKBar;
    [SerializeField] private Image supportDEFBar;
    [SerializeField] private Image supportARCBar;
    [SerializeField] private Image supportSPDBar;
    [SerializeField] private TMP_Text HPValue;
    [SerializeField] private TMP_Text MPValue;
    [SerializeField] private TMP_Text ATKValue;
    [SerializeField] private TMP_Text DEFValue;
    [SerializeField] private TMP_Text ARCValue;
    [SerializeField] private TMP_Text SPDValue;

    // Current Status
    [SerializeField] private Image currentHPBar;
    [SerializeField] private Image currentMPBar;
    [SerializeField] private TMP_Text currentHPValue;
    [SerializeField] private TMP_Text currentMPValue;
    [SerializeField] private Image currentXPFrame;
    [SerializeField] private TMP_Text currentLevel;
    [SerializeField] private Image currentAvatar;

    // Passive abilities
    [SerializeField] private TMP_Text ability_1;
    [SerializeField] private TMP_Text ability_2;
    [SerializeField] private TMP_Text ability_3;
    [SerializeField] private TMP_Text ability_4;
    [SerializeField] private TMP_Text ability_5;
    [SerializeField] private TMP_Text ability_6;
    [SerializeField] private TMP_Text ability_7;
    [SerializeField] private TMP_Text ability_8;

    public override void OnEnable()
    {
        base.OnEnable();
        PopulateFamiliarPanel();
        PopulateStatisticsPanel();
        PopulateStatusPanel();
        PopulateAbilityPanel();
    }

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

    private void PopulateFamiliarPanel()
    {
        mainIcon.sprite = GameManager.Instance.Player.Familiars.MainFamiliar.Icon;
        supportIcon.sprite = GameManager.Instance.Player.Familiars.SupportFamiliar.Icon;
    }

    private void PopulateStatisticsPanel()
    {
        float MMHP = ((float)GameManager.Instance.Player.Familiars.MainFamiliar.HP) / FamiliarType.HP_MAX / 2;
        mainHPBar.fillAmount = MMHP;
        supportHPBar.fillAmount = ((float) GameManager.Instance.Player.Familiars.SupportFamiliar.HP) / FamiliarType.HP_MAX / 2 + MMHP;
        HPValue.text = GameManager.Instance.Player.HP.ToString();

        float MMMP = ((float) GameManager.Instance.Player.Familiars.MainFamiliar.MP) / FamiliarType.MP_MAX / 2;
        mainMPBar.fillAmount = MMMP;
        supportMPBar.fillAmount = ((float) GameManager.Instance.Player.Familiars.SupportFamiliar.MP) / FamiliarType.MP_MAX / 2 + MMMP;
        MPValue.text = GameManager.Instance.Player.MP.ToString();

        float MATK = ((float) GameManager.Instance.Player.Familiars.MainFamiliar.Attack) / FamiliarType.ATK_MAX / 2;
        mainATKBar.fillAmount = MATK;
        supportATKBar.fillAmount = ((float) GameManager.Instance.Player.Familiars.SupportFamiliar.Attack) / FamiliarType.ATK_MAX / 2 + MATK;
        ATKValue.text = GameManager.Instance.Player.Attack.ToString();

        float MDEF = ((float) GameManager.Instance.Player.Familiars.MainFamiliar.Defense) / FamiliarType.DEF_MAX / 2;
        mainDEFBar.fillAmount = MDEF;
        supportDEFBar.fillAmount = ((float) GameManager.Instance.Player.Familiars.SupportFamiliar.Defense) / FamiliarType.DEF_MAX / 2 + MDEF;
        DEFValue.text = GameManager.Instance.Player.Defense.ToString();

        float MARC = ((float) GameManager.Instance.Player.Familiars.MainFamiliar.Arcane) / FamiliarType.ARC_MAX / 2;
        mainARCBar.fillAmount = MARC;
        supportARCBar.fillAmount = ((float) GameManager.Instance.Player.Familiars.SupportFamiliar.Arcane) / FamiliarType.ARC_MAX / 2 + MARC;
        ARCValue.text = GameManager.Instance.Player.Arcane.ToString();

        float MSPD = ((float) GameManager.Instance.Player.Familiars.MainFamiliar.Speed) / FamiliarType.SPD_MAX / 2;
        mainSPDBar.fillAmount = MSPD;
        supportSPDBar.fillAmount = ((float) GameManager.Instance.Player.Familiars.SupportFamiliar.Speed) / FamiliarType.SPD_MAX / 2 + MSPD;
        SPDValue.text = GameManager.Instance.Player.Speed.ToString();
    }

    private void PopulateStatusPanel()
    {
        currentHPBar.fillAmount = (float) GameManager.Instance.Player.Health / (float) GameManager.Instance.Player.HP;
        currentHPValue.text = GameManager.Instance.Player.Health.ToString() + " / " + GameManager.Instance.Player.HP.ToString();
        currentMPBar.fillAmount = (float) GameManager.Instance.Player.Health / (float) GameManager.Instance.Player.MP;
        currentMPValue.text = GameManager.Instance.Player.Health.ToString() + " / " + GameManager.Instance.Player.MP.ToString();

        currentXPFrame.fillAmount = (float) GameManager.Instance.Player.Level.LevelProgress;
        currentLevel.text = GameManager.Instance.Player.Level.PlayerLevel.ToString();
        currentAvatar.sprite = GameManager.Instance.Player.Icon;
    }

    private void PopulateAbilityPanel()
    {
        ability_1.text = GameManager.Instance.Player.Familiars.MainFamiliar.Abilities[0].ObjectName;
        ability_2.text = GameManager.Instance.Player.Familiars.MainFamiliar.Abilities[1].ObjectName;
        ability_3.text = GameManager.Instance.Player.Familiars.MainFamiliar.Abilities[2].ObjectName;
        ability_4.text = GameManager.Instance.Player.Familiars.MainFamiliar.Abilities[3].ObjectName;
        ability_5.text = GameManager.Instance.Player.Familiars.SupportFamiliar.Abilities[0].ObjectName;
        ability_6.text = GameManager.Instance.Player.Familiars.SupportFamiliar.Abilities[1].ObjectName;
        ability_7.text = GameManager.Instance.Player.Familiars.SupportFamiliar.Abilities[2].ObjectName;
        ability_8.text = GameManager.Instance.Player.Familiars.SupportFamiliar.Abilities[3].ObjectName;
    }
}