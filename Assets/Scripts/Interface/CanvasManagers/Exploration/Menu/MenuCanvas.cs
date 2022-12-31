using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Characters;

public class MenuCanvas : UICanvasBase
{
    [SerializeField] private UICanvasBase _inventoryCanvas;
    [SerializeField] private UICanvasBase _familiarCanvas;
    [SerializeField] private UICanvasBase _spellCanvas;
    [SerializeField] private UICanvasBase _statusCanvas;
    [SerializeField] private UICanvasBase _settingsCanvas;
    private UICanvasBase _activeCanvas;

    [SerializeField] private Image _currentHP;
    [SerializeField] private Image _currentMP;
    [SerializeField] private Image _currentXP;
    [SerializeField] private Image _playerAvatar;
    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _playerLevel;
    [SerializeField] private TMP_Text _playerMoney;
    [SerializeField] private TMP_Text _playerTime;
    [SerializeField] private TMP_Text _playerLocation;

    // 0: Inventory, 1: Familiars, 2: Spells, 3: Status, 4: Settings
    protected override void HandleButtonSelect(int index)
    {
        switch (index)
        {
            case 0:
                if (_activeCanvas != null) { _activeCanvas.ActivateCanvas(false); }
                _activeCanvas = _inventoryCanvas;
                _inventoryCanvas.ActivateCanvas(true);
                break;
            case 1:
                if (_activeCanvas != null) { _activeCanvas.ActivateCanvas(false); }
                _activeCanvas = _familiarCanvas;
                _familiarCanvas.ActivateCanvas(true);
                break;
            case 2:
                if (_activeCanvas != null) { _activeCanvas.ActivateCanvas(false); }
                _activeCanvas = _spellCanvas;
                _spellCanvas.ActivateCanvas(true);
                break;
            case 3:
                if (_activeCanvas != null) { _activeCanvas.ActivateCanvas(false); }
                _activeCanvas = _statusCanvas;
                _statusCanvas.ActivateCanvas(true);
                break;
            case 4:
                if (_activeCanvas != null) { _activeCanvas.ActivateCanvas(false); }
                _activeCanvas = _settingsCanvas;
                _settingsCanvas.ActivateCanvas(true);
                break;
            default:
                if (_activeCanvas != null) { _activeCanvas.ActivateCanvas(false); }
                _activeCanvas = null;
                break;
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        UpdateDisplayedData();
    }

    public override void OnDisable()
    {
        base.OnDisable();
        if (_activeCanvas != null) { _activeCanvas.ActivateCanvas(false); }
        _activeCanvas = null;
    }

    public void UpdateDisplayedData()
    {
        _currentHP.fillAmount = (float) GameManager.Instance.Player.Health / (float) GameManager.Instance.Player.HP;
        _currentMP.fillAmount = (float) GameManager.Instance.Player.Mana / (float) GameManager.Instance.Player.MP;
        _currentXP.fillAmount = (float) GameManager.Instance.Player.Level.LevelProgress;
        _playerAvatar.sprite = GameManager.Instance.Player.Icon;
        _playerName.text = GameManager.Instance.Player.ObjectName;
        _playerLevel.text = "Level: " + GameManager.Instance.Player.Level.PlayerLevel.ToString();
        _playerMoney.text = GameManager.Instance.Player.Wallet.Money.ToString() + " G";
        _playerTime.text = GameManager.Instance.playTime.PlayTimeString();
        _playerLocation.text = GameManager.Instance.Player.Atlas.CurrentLocation.ObjectName;
    }

    public void Update()
    {
        // Only update the play time display if needed.
        string currentPlayTime = GameManager.Instance.playTime.PlayTimeString();
        if(_playerTime.text != currentPlayTime)
        {
            _playerTime.text = currentPlayTime;
        } 
    }
}