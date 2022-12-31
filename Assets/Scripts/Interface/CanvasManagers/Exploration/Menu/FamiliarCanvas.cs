using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Characters;
using System.Collections.Generic;

public class FamiliarCanvas : UICanvasBase
{
    [SerializeField] private Image mainIcon;
    [SerializeField] private TMP_Text mainName;
    [SerializeField] private TMP_Text mainElement;
    [SerializeField] private Image supportIcon;
    [SerializeField] private TMP_Text supportName;
    [SerializeField] private TMP_Text supportElement;
    [SerializeField] private TMP_Text numFamiliars;

    [SerializeField] private Transform content;
    [SerializeField] private GameObject containerPrefab;
    [SerializeField] private GameObject emptyPrefab;

    [SerializeField] private UICanvasBase familiarDetailCanvas;

    public override void OnEnable()
    {
        base.OnEnable();
        UpdateBondedFamiliarData();
        RefreshFamiliarContainer();
    }

    // index 0: Close button, index 1: Swap Familiars
    protected override void HandleButtonSelect(int index)
    {
        switch (index)
        {
            case 0:
                ActivateCanvas(false);
                break;
            case 1:
                GameManager.Instance.Player.Familiars.SwapFamiliars();
                UpdateBondedFamiliarData();
                break;
            default:
                Debug.LogError("Invalid button index.");
                break;
        }
    }

    protected void OnFamiliarClick(FamiliarSO familiar)
    {
        familiarDetailCanvas.ActivateCanvas(true);
        (familiarDetailCanvas as DetailFamiliarCanvas).DisplayFamiliarDetails(familiar);
    }

    private void UpdateBondedFamiliarData()
    {
        FamiliarSO mainFamiliar = GameManager.Instance.Player.Familiars.MainFamiliar;
        FamiliarSO supportFamiliar = GameManager.Instance.Player.Familiars.SupportFamiliar;
        mainIcon.sprite = mainFamiliar.Icon;
        mainName.text = mainFamiliar.ObjectName;
        mainElement.text = mainFamiliar.ElementType.ToString();
        supportIcon.sprite = supportFamiliar.Icon;
        supportName.text = supportFamiliar.ObjectName;
        supportElement.text = supportFamiliar.ElementType.ToString();
    }

    private void RefreshFamiliarContainer()
    {
        ClearCanvas();
        List<FamiliarSO> familiars = GameManager.Instance.Player.Familiars.FamiliarPacts;
        numFamiliars.text = "Pacts: " + familiars.Count.ToString();
        foreach (FamiliarSO familiar in familiars)
        {
            GameObject container = GameObject.Instantiate(containerPrefab, content);
            container.transform.Find("Familiar").GetComponent<Button>().onClick.AddListener(() => OnFamiliarClick(familiar));
            container.transform.Find("Familiar").Find("FamiliarIcon").GetComponent<Image>().sprite = familiar.Icon;
        }

    }

    protected void ClearCanvas()
    {
        foreach (Transform container in content)
        {
            Destroy(container.gameObject);
        }
    }
}