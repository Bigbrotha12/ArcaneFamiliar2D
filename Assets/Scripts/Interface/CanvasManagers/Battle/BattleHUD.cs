using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Characters;
using Environment;
using Battle;

public class BattleHUD : UICanvasBase
{
    private List<ICombatAction> _actionList;
    private List<CombatantSO> _targets;
    private event Action _onTargetsReady;
    [SerializeField] private BattleService _battleController;
    [SerializeField] private int _maxActionCount;

    // Status
    [SerializeField] private Image _statusPanelBorder;
    [SerializeField] private TMP_Text _HPValue;
    [SerializeField] private Image _HPBar;
    [SerializeField] private TMP_Text _MPValue;
    [SerializeField] private Image _MPBar;
    [SerializeField] private TMP_Text _status;
    [SerializeField] private TMP_Text _mainName;
    [SerializeField] private TMP_Text _supportName;
    [SerializeField] private Image _playerIcon;

    // Info
    [SerializeField] private TMP_Text _environment;
    [SerializeField] private TMP_Text _actionName;
    [SerializeField] private TMP_Text _battleInfo;
    
    // Enemy list
    [SerializeField] private Transform _enemyContainer;
    [SerializeField] private GameObject _enemyPrefab;

    // Action Panel
    [SerializeField] private GameObject _actionPanel;
    [SerializeField] private TMP_Text _actionQueue;
    [SerializeField] private GameObject _actionEndButton;

    // SelectionPanels
    [SerializeField] private GameObject _pouchPanel;
    [SerializeField] private Transform _pouchContainer;
    [SerializeField] private GameObject _itemPrefab;
    [SerializeField] private GameObject _spellBookPanel;
    [SerializeField] private Transform _spellContainer;
    [SerializeField] private GameObject _spellPrefab;
    [SerializeField] private GameObject _targetPanel;
    [SerializeField] private Transform _targetContainer;
    [SerializeField] private GameObject _targetPrefab;

    // index 0: attack, index 1: spell, index 2: defend, index 3: swap, 
    // index 4: items, index 5: run, index 6: end, index 7: pouchCancel,
    // index 8: spellCancel, index 9: targetConfirm, index 10: targetCancel
    public override void OnEnable()
    {
        base.OnEnable();
        if (CanvasButtons is null || CanvasButtons.Count < 5) { Debug.LogError("Error: Invalid button configuration"); return; }
        RefreshStatusHUD();
        RefreshEnemyList();
        _battleController.OnBattleInfo += RefreshInfoHUD;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        _battleController.OnBattleInfo -= RefreshInfoHUD;
    }

    protected override void HandleButtonSelect(int index)
    {
        switch (index)
        {
            case 0:
                HandleActionSelection(ActionType.ATTACK);
                break;
            case 1:
                HandleActionSelection(ActionType.SPELL);
                break;
            case 2:
                HandleActionSelection(ActionType.DEFEND);
                break;
            case 3:
                HandleActionSelection(ActionType.SWAP);
                break;
            case 4:
                HandleActionSelection(ActionType.ITEM);
                break;
            case 5:
                HandleActionSelection(ActionType.RUN);
                break;
            case 6:
                EndActionSelect();
                break;
            default:
                break;
        }
    }

    private void RefreshStatusHUD()
    {
        _HPValue.text = GameManager.Instance.Player.Health.ToString() + " / " + GameManager.Instance.Player.GetAdjustedStatistic(Statistic.HP).ToString();
        _HPBar.fillAmount = (float)GameManager.Instance.Player.Health / (float)GameManager.Instance.Player.GetAdjustedStatistic(Statistic.HP);
        _MPValue.text = GameManager.Instance.Player.Mana.ToString() + " / " + GameManager.Instance.Player.GetAdjustedStatistic(Statistic.MP).ToString();
        _MPBar.fillAmount = (float)GameManager.Instance.Player.Mana / (float)GameManager.Instance.Player.GetAdjustedStatistic(Statistic.MP);

        StopCoroutine("DisplayStatus");
        List<string> statusEffects = new List<string>();
        foreach(Effect effect in GameManager.Instance.Player.ActiveEffects)
        {
            if(effect.EffectSO.StatusEffect)
            {
                statusEffects.Add(effect.EffectSO.ObjectName);
            }
        }
        StartCoroutine("DisplayStatus", statusEffects);

        _mainName.text = GameManager.Instance.Player.Familiars.MainFamiliar.ObjectName;
        _supportName.text = GameManager.Instance.Player.Familiars.SupportFamiliar.ObjectName;
        _playerIcon.sprite = GameManager.Instance.Player.Icon;
        _environment.text = "Environment: " + GameManager.Instance.Player.Atlas.CurrentLocation.Plane.ToString();
    }

    private IEnumerator DisplayStatus(List<string> activeStatus)
    {
        while(activeStatus.Count > 0)
        {
            foreach (string status in activeStatus)
            {
                _status.text = status;
                yield return new WaitForSeconds(3.0f);
            }
        }
    }

    public void RefreshInfoHUD(string actionName, List<string> battleInfo)
    {
        StartCoroutine("DisplayBattleInfo", battleInfo);
        _actionName.text = actionName;
    }

    private IEnumerator DisplayBattleInfo(List<string> actionInfo)
    {
        Queue<string> infoQueue = new Queue<string>();
        foreach (string info in actionInfo)
        {
            infoQueue.Enqueue(info);
        }
        
        while(infoQueue.Count > 0)
        {
            _battleInfo.text = infoQueue.Dequeue();
            yield return new WaitForSeconds(1.5f);
        }
        _battleInfo.text = "";
    }

    private void RefreshEnemyList()
    {
        foreach (Transform container in _enemyContainer)
        {
            Destroy(container.gameObject);
        }

        foreach(CombatantSO enemy in _battleController.Enemies)
        {
            GameObject container = GameObject.Instantiate(_enemyPrefab, _enemyContainer);
            container.transform.Find("EnemyName").GetComponent<TMP_Text>().text = enemy.ObjectName;
            container.transform.Find("EnemyHealth").GetComponent<Image>().fillAmount = (float)enemy.Health / (float)enemy.GetAdjustedStatistic(Statistic.HP);
        }
    }

    private void StartActionSelection()
    {
        if(_actionList.Count >= _maxActionCount)
        {
            EndActionSelect();
            return;
        }
        _actionPanel.SetActive(true);
        _actionQueue.text = (_actionList.Count + 1).ToString() + " / " + _maxActionCount.ToString();

        // Disable any unavailable actions
        CanvasButtons[0].interactable = GameManager.Instance.Player.AvailableActions.Contains(ActionType.ATTACK);
        CanvasButtons[1].interactable = GameManager.Instance.Player.AvailableActions.Contains(ActionType.SPELL);
        CanvasButtons[2].interactable = GameManager.Instance.Player.AvailableActions.Contains(ActionType.DEFEND);
        CanvasButtons[3].interactable = GameManager.Instance.Player.AvailableActions.Contains(ActionType.SWAP);
        CanvasButtons[4].interactable = GameManager.Instance.Player.AvailableActions.Contains(ActionType.ITEM);
        CanvasButtons[5].interactable = GameManager.Instance.Player.AvailableActions.Contains(ActionType.RUN);

        if (_actionList.Count > 0)
        {
            _actionEndButton.SetActive(true);
        }
        else
        {
            _actionEndButton.SetActive(false);
        }
    }

    private void HandleActionSelection(ActionType actionType)
    {
        switch (actionType)
        {
            case ActionType.ATTACK:
                _actionPanel.SetActive(false);
                AttackSelection();
                break;
            case ActionType.SPELL:
                _actionPanel.SetActive(false);
                SpellSelection();
                break;
            case ActionType.DEFEND:
                _actionPanel.SetActive(false);
                AddDefendAction();
                break;
            case ActionType.SWAP:
                _actionPanel.SetActive(false);
                AddSwapAction();
                break;
            case ActionType.ITEM:
                _actionPanel.SetActive(false);
                ItemSelection();
                break;
            case ActionType.RUN:
                _actionPanel.SetActive(false);
                AddRunAction();
                break;
            default:
                Debug.LogError("Error: Invalid action type.");
                break;
        }
    }

    public void AddAttackAction(List<CombatantSO> targets)
    {
        ICombatAction attackAction = new AttackAction(GameManager.Instance.Player, targets);
        _actionList.Add(attackAction);
        
    }

    public void AddSpellAction(List<CombatantSO> targets, SpellSO spell)
    {
        ICombatAction spellAction = new SpellAction(GameManager.Instance.Player, targets, spell);
        _actionList.Add(spellAction);
        StartActionSelection();
    }

    public void AddDefendAction()
    {
        ICombatAction defendAction = new DefendAction(GameManager.Instance.Player);
        _actionList.Add(defendAction);
        StartActionSelection();
    }

    public void AddSwapAction()
    {
        ICombatAction swapAction = new SwapAction();
        _actionList.Add(swapAction);
        StartActionSelection();
    }

    public void AddItemAction(List<CombatantSO> targets, ItemSO item)
    {
        ICombatAction itemAction = new ItemAction(targets, item);
        _actionList.Add(itemAction);
        StartActionSelection();
    }

    public void AddRunAction()
    {
        ICombatAction itemAction = new FleeAction(GameManager.Instance.Player);
        _actionList.Add(itemAction);
        StartActionSelection();
    }

    public void AttackSelection()
    {
        Action AddAction = null;
        AddAction = delegate ()
        {
            AddAttackAction(_targets);
            _onTargetsReady -= AddAction;
        };
        _onTargetsReady += AddAction;

        TargetSelection(Target.SINGLE);
    }

    public void ItemSelection()
    {
        _pouchPanel.SetActive(true);
        foreach (Transform container in _pouchContainer)
        {
            Destroy(container.gameObject);
        }

        foreach (KeyValuePair<ItemSO, int> itemStack in GameManager.Instance.Player.Inventory.Backpack.Stack)
        {
            GameObject container = GameObject.Instantiate(_itemPrefab, _pouchContainer);
            container.transform.Find("ItemIcon").GetComponent<Image>().sprite = itemStack.Key.Icon;
            container.transform.Find("ItemStackSize").GetComponent<TMP_Text>().text = itemStack.Value.ToString();
            container.GetComponent<Button>().onClick.AddListener(() => 
            { 
                string[] options = { "Use", "Cancel" };
                GameManager.Instance.UInterface.QueueObject(itemStack.Key, options, UseItem);
            });
        }
    }

    public void UseItem(IObjectHeader item, int index)
    {
        _pouchPanel.SetActive(false);

        if (index != 0) { return; }

        ItemSO returnedItem = item as ItemSO;
        Action AddAction = null;
        AddAction = delegate ()
        {
            AddItemAction(_targets, returnedItem);
            _onTargetsReady -= AddAction;
        };
        _onTargetsReady += AddAction;

        TargetSelection(returnedItem.ItemTarget);
    }

    public void SpellSelection()
    {
        _spellBookPanel.SetActive(true);
        foreach (Transform container in _spellContainer)
        {
            Destroy(container.gameObject);
        }

        foreach (SpellSO spell in GameManager.Instance.Player.SpellBook.SpellsEquipped)
        {
            GameObject container = GameObject.Instantiate(_spellPrefab, _spellContainer);
            container.transform.Find("SpellFrame").Find("SpellIcon").GetComponent<Image>().sprite = spell.Icon;
            container.transform.Find("SpellName").GetComponent<TMP_Text>().text = spell.ObjectName;
            container.transform.Find("SpellCost").GetComponent<TMP_Text>().text = spell.Cost.ToString();
            container.GetComponent<Button>().onClick.AddListener(() => 
            { 
                string[] options = { "Cast", "Cancel" };
                GameManager.Instance.UInterface.QueueObject(spell, options, UseItem);
            });
        }
    }

    public void UseSpell(IObjectHeader spell, int index)
    {
        _spellBookPanel.SetActive(false);

        if (index != 0) { return; }

        SpellSO returnedSpell = spell as SpellSO;
        Action AddAction = null;
        AddAction = delegate ()
        {
            AddSpellAction(_targets, returnedSpell);
            _onTargetsReady -= AddAction;
        };
        _onTargetsReady += AddAction;

        TargetSelection(returnedSpell.SpellTarget);
    }

    public void TargetSelection(Target targetType)
    {
        _targets.Clear();
        switch (targetType)
        {
            case Target.SELF:
                SelectSelfTarget();
                break;
            case Target.SINGLE:
                SelectSingleTarget();
                break;
            case Target.MULTIPLE:
                SelectMultipleTargets();
                break;
            case Target.ALL:
                SelectAllTargets();
                break;
            default:
                Debug.LogError("Error: Invalid target option.");
                break;
        }
        StartCoroutine("FlashTargets");
    }

    public void SelectSelfTarget()
    {
        _targetPanel.SetActive(true);
        AddTarget(GameManager.Instance.Player);
        CanvasButtons[9].interactable = true;
    }

    public void SelectSingleTarget()
    {
        _targetPanel.SetActive(true);
        foreach (CombatantSO enemy in _battleController.Enemies)
        {
            GameObject container = GameObject.Instantiate(_targetPrefab, _targetContainer);
            container.transform.Find("IconFrame").Find("TargetIcon").GetComponent<Image>().sprite = enemy.Icon;
            container.transform.Find("TargetName").GetComponent<TMP_Text>().text = enemy.ObjectName;
            int maxHP = enemy.GetAdjustedStatistic(Statistic.HP);
            container.transform.Find("HPBar").GetComponent<Image>().fillAmount = (float) enemy.Health / (float) maxHP;
            container.transform.Find("HPValue").GetComponent<TMP_Text>().text = enemy.Health.ToString() + " / " + maxHP.ToString();
            container.GetComponent<Button>().onClick.AddListener(() => {
                _targets.Clear();
                AddTarget(enemy);
                CanvasButtons[9].interactable = true;
            });
        }
    }

    public void SelectAllTargets()
    {
        _targetPanel.SetActive(true);
        AddTarget(GameManager.Instance.Player);
        foreach (CombatantSO enemy in _battleController.Enemies)
        {
            GameObject container = GameObject.Instantiate(_targetPrefab, _targetContainer);
            container.transform.Find("IconFrame").Find("TargetIcon").GetComponent<Image>().sprite = enemy.Icon;
            container.transform.Find("TargetName").GetComponent<TMP_Text>().text = enemy.ObjectName;
            int maxHP = enemy.GetAdjustedStatistic(Statistic.HP);
            container.transform.Find("HPBar").GetComponent<Image>().fillAmount = (float) enemy.Health / (float) maxHP;
            container.transform.Find("HPValue").GetComponent<TMP_Text>().text = enemy.Health.ToString() + " / " + maxHP.ToString();
            AddTarget(enemy);
        }
        CanvasButtons[9].interactable = true;
    }

    public void SelectMultipleTargets()
    {
        _targetPanel.SetActive(true);
        foreach (CombatantSO enemy in _battleController.Enemies)
        {
            GameObject container = GameObject.Instantiate(_targetPrefab, _targetContainer);
            container.transform.Find("IconFrame").Find("TargetIcon").GetComponent<Image>().sprite = enemy.Icon;
            container.transform.Find("TargetName").GetComponent<TMP_Text>().text = enemy.ObjectName;
            int maxHP = enemy.GetAdjustedStatistic(Statistic.HP);
            container.transform.Find("HPBar").GetComponent<Image>().fillAmount = (float) enemy.Health / (float) maxHP;
            container.transform.Find("HPValue").GetComponent<TMP_Text>().text = enemy.Health.ToString() + " / " + maxHP.ToString();
            AddTarget(enemy);
        }
        CanvasButtons[9].interactable = true;
    }

    public void AddTarget(CombatantSO target)
    {
        if(!_targets.Contains(target))
        {
            _targets.Add(target);
        }
    }

    public void CancelTargetSelect()
    {
        CanvasButtons[9].interactable = false;
        foreach (Transform container in _targetContainer)
        {
            Destroy(container.gameObject);
        }

        _targets.Clear();
        _onTargetsReady = null;
        _targetPanel.SetActive(false);
        StopCoroutine("FlashTargets");
    }

    public void ConfirmTargetSelect()
    {
        CanvasButtons[9].interactable = false;
        _targetPanel.SetActive(false);
        StopCoroutine("FlashTargets");

        if(_targets.Count == 0)
        {
            Debug.LogError("Error: Invalid target selection.");
            return;
        }

        _onTargetsReady?.Invoke();
    }

    public IEnumerator FlashTargets()
    {
        if(_targets.Contains(GameManager.Instance.Player))
        {
            _statusPanelBorder.color = new Color(200, 100, 100, 0.8f);
        }

        foreach (CombatantSO target in _targets)
        {
            int index = _battleController.Enemies.IndexOf(target);
            _enemyContainer.GetChild(index).GetComponent<Image>().color = new Color(200, 100, 100, 0.8f);
        }
        yield return new WaitForSeconds(0.5f);
    }

    private void EndActionSelect()
    {
        if(_actionList.Count == 0)
        {
            Debug.LogError("Error: Missing action list.");
            return;
        }

        _battleController.AddPlayerActions(_actionList);
        _actionList.Clear();
        _actionPanel.SetActive(false);
    }
    
}