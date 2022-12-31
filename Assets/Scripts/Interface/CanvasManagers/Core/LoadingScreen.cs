using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image _loadingBar;
    [SerializeField] private TMP_Text _locationName;
    [SerializeField] private Image _locationIcon;
    private bool _isDone;

    // Start is called before the first frame update
    public void OnEnable()
    {
        // Get player's current location
        var loadingLocation = GameManager.Instance.Player.Atlas.CurrentLocation;
        _locationName.text = loadingLocation.ObjectName;
        _locationIcon.sprite = loadingLocation.Icon;
        _loadingBar.fillAmount = 0;
        _isDone = false;
    }

    public void Update()
    {
        if (_isDone) { return; }

        float status = GameManager.Instance.Loader.GetStatus();
        _loadingBar.fillAmount = status;
        if(status >= 1.0f)
        {
            _isDone = true;
            GameManager.Instance.UInterface.FinishLoading();
        }
    }
}