using System;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : Singleton<GameStateManager>
{
    public UnityEvent<GlobalStates, GlobalStates> OnGameStateChanged = new UnityEvent<GlobalStates, GlobalStates>();

    GlobalStates _currentGameState = GlobalStates.PREGAME;
    GlobalStates _previousState = GlobalStates.PREGAME;

    public GlobalStates CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (_currentGameState == GlobalStates.PREGAME)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        _previousState = CurrentGameState;
        UpdateState(_currentGameState is not GlobalStates.PAUSED ? GlobalStates.PAUSED : _previousState);
    }

    public void onGameOver()
    {
        UpdateState(GlobalStates.ENDGAME);
    }

    public void QuitGame()
    {
        Debug.Log("Game Manager: Quitting game");

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void UpdateState(GlobalStates state)
    {
        Debug.Log("Exiting State: " + _previousState.ToString());
        _previousState = _currentGameState;
        _currentGameState = state;
        Debug.Log("Entering State: " + state.ToString());

        switch (_currentGameState)
        {
            case GlobalStates.PREGAME:
                Time.timeScale = 1.0f;
                break;
            case GlobalStates.PREPARATION:
                Time.timeScale = 1.0f;
                break;
            case GlobalStates.EXPLORATION:
                Time.timeScale = 1.0f;
                break;
            case GlobalStates.BATTLE:
            Time.timeScale = 1.0f;
                break;
            case GlobalStates.PAUSED:
                Time.timeScale = 0.0f;
                break;
            case GlobalStates.ENDGAME:
                Time.timeScale = 1.0f;
                break;
            default:
                break;
        }
        OnGameStateChanged.Invoke(_currentGameState, _previousState);
    }
}