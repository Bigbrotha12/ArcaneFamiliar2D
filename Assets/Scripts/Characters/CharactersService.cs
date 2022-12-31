using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Characters
{
    public class CharactersService : ICharacters
    {
        public Preferences defaultPreferences = new Preferences()
        {
            mainFamiliar = 0,
            supportFamiliar = 1,
            equippedSpells = 0,
            musicVolume = 50,
            soundVolume = 50,
            resolutionOption = 0,
            windowMode = 0,
            controls = null
        };
        public Preferences _currentPreferences;

        public CharactersService(Preferences initialPreferences)
        {
            _currentPreferences = initialPreferences is null ? defaultPreferences : initialPreferences;
        }

        public PlayerSO CreatePlayer(UserData gameData, List<Familiars> familiars, WorldAtlas worldAtlas)
        {
            if(gameData is null || familiars is null || worldAtlas is null) 
            {
                Debug.LogError("Invalid input parameters for Create Player.");
                return null;
            }

            List<FamiliarSO> playerFamiliars = new List<FamiliarSO>();
            foreach (Familiars familiar in familiars)
            {
                playerFamiliars.Add(worldAtlas.GetWorldObject<FamiliarSO>(familiar.familiarId));
            }

            CharacterSO playerPrototype = worldAtlas.GetWorldObject<CharacterSO>(1);
            PlayerSO mainCharacter = ScriptableObject.CreateInstance("PlayerSO");
            
            mainCharacter.Inventory = new Inventory(UserDataEncoder.GetPlayerItems(gameData, worldAtlas));
            mainCharacter.SpellBook = new SpellBook(UserDataEncoder.GetPlayerSpells(gameData, worldAtlas), _currentPreferences.equippedSpells);
            mainCharacter.Atlas = new LocationAtlas(UserDataEncoder.GetPlayerLocations(gameData, worldAtlas));
            mainCharacter.RecipeBook = new RecipeBook(UserDataEncoder.GetPlayerRecipes(gameData, worldAtlas));
            mainCharacter.Familiars = new Familiars(playerFamiliars);
            mainCharacter.NewFamiliarProgress = UserDataEncoder.GetPlayerProgress(gameData);
            mainCharacter.Level = new LevelManager(UserDataEncoder.GetPlayerLevel(gameData));
            mainCharacter.Wallet = new Wallet(UserDataEncoder.GetMoneyBalance(gameData));

            mainCharacter.Familiars.BondMainFamiliar(_currentPreferences.mainFamiliar);
            mainCharacter.Familiars.BondSupportFamiliar(_currentPreferences.supportFamiliar);
            mainCharacter.InitializeResources();
            return mainCharacter;
        }

        public CombatantSO CreateCombatant(CombatantSO template)
        {
            CombatantSO newInstance;
            if(template is SpellCasterSO)
            {
                newInstance = ScriptableObject.CreateInstance<SpellCasterSO>();
            }
            else
            {
                newInstance = ScriptableObject.CreateInstance<CombatantSO>();
            }
            return newInstance;
        }

        public List<CombatantSO> CreateCombatant(List<CombatantSO> templates)
        {
            List<CombatantSO> newInstances = new List<CombatantSO>();
            foreach (CombatantSO template in templates)
            {
                if(template is SpellCasterSO)
                {
                    newInstances.Add(ScriptableObject.CreateInstance<SpellCasterSO>());
                }
                else
                {
                    newInstances.Add(ScriptableObject.CreateInstance<CombatantSO>());
                }
            }
            return newInstances;
        }

        

        public void UpdatePreferences(Preferences playerPreferences)
        {
            _currentPreferences = playerPreferences is null ? defaultPreferences : playerPreferences;
        }

        public UserData PrepareSaveData()
        {
            return UserDataEncoder.EncodePlayerData(GameManager.Instance.Player);
        }
    }
}