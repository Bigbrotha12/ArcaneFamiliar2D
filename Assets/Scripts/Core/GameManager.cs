using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using UnityEngine;

using Environment;
using Characters;
using InterOperability;
using UserInterface;

#if UNITY_EDITOR == false
public class GameManager : Singleton<GameManager>
{
    private GameStateManager _globalState;
    private IInterOperable _interOperation;
    public ICharacters Characters;
    public IUserInterface UInterface;
    private bool _isRegistered;

    public PlayerSO Player { get; private set; }
    public TimeKeeper playTime { get; private set; }
    public WorldAtlas WorldAtlas { get; private set; }
    public SceneAssetLoader Loader { get; private set; }
    public bool IsLoggedIn { get { return _interOperation.LogInStatus(); } }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        Loader = new SceneAssetLoader();
        LoadIntro(true);
        OPS.AntiCheat.Detector.FieldCheatDetector.OnFieldCheatDetected += CheatDetected;
    }

    protected async void Start()
    {
        _globalState = gameObject.GetComponent<GameStateManager>();
        _interOperation = new InterOperabilityService();
        InitialData initData = await _interOperation.RequestInitData();
        Characters = new CharactersService(initData.playerPreferences);
        _isRegistered = initData.isRegistered;
    }

    public void LoadIntro(bool loadFlag)
    {
        if(loadFlag)
        {
            SceneManager.LoadSceneAsync("IntroScreen");
        } 
        else
        {
            SceneManager.UnloadScene("IntroScreen");
        }
    }

    #region Session Management
    // Starts session authentication, and player instantiation.
    // Runs after users clicks "Login" on start Menu.
    public async Task InitializationRoutine()
    {
        // 1. Check initial data to determine if new user or returning.
        if(_isRegistered)
        {
            bool logInResult = await _interOperation.LogIn();
            if (!logInResult) 
            { 
                Debug.LogError("Could not log in user");
                UInterface.QueueAlert("Login Attempt Failed."); 
            } else
            {
                UInterface.QueueAlert("Login Successful.");
            }
        } else
        {
            bool registrationResult = await _interOperation.Register();
            if (!registrationResult) 
            { 
                Debug.LogError("Could not register user"); 
                
            }
        }

        // 2. Once logged in, get player save data, and owned familiars from server
        Task<UserData> dataLoad = _interOperation.LoadData();
        Task<List<Familiars>> familiarFetch = _interOperation.GetOwnedFamiliars();
        UserData gameData = await dataLoad;
        List<Familiars> playerFamiliars = await familiarFetch;

        // 3. Create PlayerSO and initialize player world assets
        playTime = gameObject.GetComponent<TimeKeeper>();
        WorldAtlas = gameObject.GetComponent<WorldAtlas>();
        Player = Characters.CreatePlayer(gameData, playerFamiliars, WorldAtlas);
    }

    public async Task Login()
    {
        bool _isLoggedIn = await _interOperation.LogIn();
    
        if (_isLoggedIn) 
        {
            Debug.Log("You are logged in!"); 
        }
        else 
        {
            Debug.Log("Login Failed..."); 
        }
    }

    public async Task<bool> Logout()
    {
        return await _interOperation.LogOut();
    }

    public async Task<bool> SaveGame(bool progressFlag)
    {
        UserData gameData = _characters.PrepareSaveData();
        return await _interOperation.SaveData(gameData, progressFlag);
    }
    #endregion

    #region State Management
    private void ChangeState(GlobalStates newState)
    {
        _globalState.UpdateState(newState);
    }

    public GlobalStates GetCurrentState()
    {
        return _globalState.CurrentGameState;
    }

    private void CheatDetected()
    {
        Debug.Log("Cheating attempt detected.");
    }

    public void ExitGame()
    {
        _globalState.QuitGame();
    }
    #endregion
}
#else
// TEST GameManager
public class GameManager : Singleton<GameManager>
{
    private GameStateManager _globalState;
    public ICharacters Characters;

    public SceneAssetLoader Loader { get; private set; }
    public IUserInterface UInterface;
    public PlayerSO Player { get; private set; }
    public TimeKeeper playTime { get; private set; }
    public WorldAtlas WorldAtlas { get; private set; }
    public bool IsLoggedIn { get; private set; }
    public bool InitializeTestPlayer;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);
        if(!InitializeTestPlayer) LoadIntro(true);
        _globalState = gameObject.GetComponent<GameStateManager>();
        Loader = new SceneAssetLoader();
        Characters = new CharactersService(null);

        if (InitializeTestPlayer) { Initialization(); }
    }

    public void LoadIntro(bool loadFlag)
    {
        if(loadFlag)
        {
            SceneManager.LoadSceneAsync("IntroScene", LoadSceneMode.Single);
        } 
        else
        {
            SceneManager.UnloadSceneAsync("IntroScene");
        }
    }

#region Session Management
    // Starts session authentication, and player instantiation.
    // Runs after users clicks "Login" on start Menu.
    public void Initialization()
    {
        //await Task.Delay(0);
        // Fake User Data
        UserData gameData = new UserData();
        gameData.name = "Fake Player";
        gameData.level = 10000;
        gameData.playTime = 0;
        gameData.money = 20000;
        gameData.items = new string[] {"1","2","2","3"};
        gameData.recipes = new string[] {"1"};
        gameData.spells = new string[] {"1"};
        gameData.locations = new Dictionary<string, string>()
        {
            {"hub", "001000"},
            {"10", "00010"},
            {"11", "00000"}
        };
        gameData.progress = new int[] {1, 1, 0, 0}; 

        // Fake Familiar Data
        List<FamiliarMetadata> playerFamiliars = new List<FamiliarMetadata>()
        {
            new FamiliarMetadata()
            {
                _id = 0,
                familiarId = 1,
                name = "White Dog",
                description = "This is the first familiar",
                image = "SomeURL.com",
                image_url = "SomeURL.com",
                affinity = "Fire",
                HP = 140,
                MP = 55,
                attack = 33,
                defense = 63,
                arcane = 119,
                speed = 55,
                ability_1 = "Brave",
                ability_2 = "Sturdy",
                ability_3 = "None",
                ability_4 = "None",
                rarity = "common",
                generation = 1
            },
            new FamiliarMetadata()
            {
                _id = 1,
                familiarId = 2,
                name = "Yellow Fighter",
                description = "This is the second familiar",
                image = "SomeURL.com",
                image_url = "SomeURL.com",
                affinity = "Earth",
                HP = 160,
                MP = 23,
                attack = 67,
                defense = 63,
                arcane = 88,
                speed = 70,
                ability_1 = "Quick Step",
                ability_2 = "Sturdy",
                ability_3 = "None",
                ability_4 = "None",
                rarity = "common",
                generation = 1
            }
        };

        // Create Fake Player
        playTime = gameObject.GetComponent<TimeKeeper>();
        WorldAtlas = gameObject.GetComponent<WorldAtlas>();
        Player = Characters.CreatePlayer(gameData, playerFamiliars, WorldAtlas);
        IsLoggedIn = true;
    }

    public async Task Login()
    {
        await Task.Run(() => { IsLoggedIn = true; });
    }

    public async Task<bool> Logout()
    {
        return await Task.Run(() => 
        {
            IsLoggedIn = false;
            return true; 
        });
    }

    public async Task<bool> SaveGame(bool progressFlag)
    {
        return await Task.Run(() => { return true; });
    }
#endregion

#region State Management
    private void ChangeState(GlobalStates newState)
    {
        _globalState.UpdateState(newState);
    }

    public GlobalStates GetCurrentState()
    {
        return _globalState.CurrentGameState;
    }

    public void ExitGame()
    {
        _globalState.QuitGame();
    }
#endregion
}
#endif