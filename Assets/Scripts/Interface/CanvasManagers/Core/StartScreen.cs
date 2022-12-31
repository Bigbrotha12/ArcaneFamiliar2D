using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StartScreen : UICanvasBase
{
    [SerializeField] private TMP_Text startText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private TMP_Text playerName;
    [SerializeField] private UICanvasBase SettingsCanvas;
    [SerializeField] private GameObject _credits;

    public void Start()
    {
        StartCoroutine("InitialSequence");

        if (GameManager.Instance.IsLoggedIn) 
        {
            playerName.text = GameManager.Instance.Player.ObjectName;
            startText.text = "Continue";
        }
    }

    private IEnumerator InitialSequence()
    {
        _credits.SetActive(true);
        GameManager.Instance.UInterface.StartFade(true, 1f);
        yield return new WaitForSeconds(3);
        GameManager.Instance.UInterface.StartFade(false, 1f);
        yield return new WaitForSeconds(1f);
        _credits.SetActive(false);
        GameManager.Instance.UInterface.StartFade(true, 1f);
    }

    // index 0: Login, index 1: Settings, index 2: Exit
    protected async override void HandleButtonSelect(int index)
    {
        switch (index)
        {
            case 0:
                if(GameManager.Instance.IsLoggedIn)
                {
                    Debug.Log("Traveling to Hub.");
                    GameManager.Instance.LoadIntro(false);
                    GameManager.Instance.Player.Atlas.TravelToLocation(GameManager.Instance.Player.Atlas.HubLocation);
                } else
                {
                    CanvasButtons[0].interactable = false;
                    #if UNITY_EDITOR == false
                    await GameManager.Instance.InitializationRoutine();
                    #endif
                    if (GameManager.Instance.IsLoggedIn == true)
                    {
                        playerName.text = GameManager.Instance.Player.ObjectName;
                        startText.text = "Continue";
                        GameManager.Instance.Player.Atlas.TravelToLocation(GameManager.Instance.Player.Atlas.HubLocation);
                    }
                    
                    CanvasButtons[0].interactable = true;
                }
                break;
            case 1:
                SettingsCanvas.ActivateCanvas(true);
                break;
            case 2:
                GameManager.Instance.ExitGame();
                break;
            default:
                throw new System.Exception("Invalid button index");
        }
    }
}