using System;
using System.Collections.Generic;
using Environment;
using Characters;
// Decodes and Encodes User game data to initialize ScriptableObjects
public static class UserDataEncoder
{
    public static UserData EncodePlayerData(PlayerSO player)
    {
        UserData saveData = new UserData();
        saveData.name = player.ObjectName;
        saveData.level = player.Level.TotalEXP;
        saveData.playTime = GameManager.Instance.playTime.PlayTime;
        saveData.money = player.Wallet.Money;

        #region Items Encoding
        List<string> items = new List<string>();
        foreach (ItemSO item in player.Inventory.GetCombinedInventory())
        {
            items.Add(item.Id.ToString());
        }
        saveData.items = items.ToArray() is not null ? items.ToArray() : new string[0];
        #endregion

        #region Spells Encoding
        List<string> spells = new List<string>();
        foreach (SpellSO spell in player.SpellBook.SpellsKnown)
        {
            spells.Add(spell.Id.ToString());
        }
        saveData.spells = spells.ToArray() is not null ? spells.ToArray() : new string[0];
        #endregion

        #region Recipes Encoding
        List<string> recipes = new List<string>();
        foreach (RecipeSO recipe in player.RecipeBook.RecipesKnown)
        {
            recipes.Add(recipe.Id.ToString());
        }
        saveData.recipes = recipes.ToArray() is not null ? recipes.ToArray() : new string[0];
        #endregion

        #region Location Encoding
        Dictionary<string, string> locations = new Dictionary<string, string>();
        // handle hub special case
        locations.Add("hub", player.Atlas.HubInitData.CurrentState);

        // handle remaining locations
        for (int i = 1; i < player.Atlas.LocationsKnown.Count; i++)
        {
            locations.Add(player.Atlas.LocationsKnown[i].Id.ToString(), player.Atlas.LocationsKnown[i].State.CurrentState);
        }
        saveData.locations = locations;
        #endregion

        return saveData;
    }

    public static string GetPlayerName(UserData data)
    {
        return data.name;
    }

    public static int GetPlayerLevel(UserData data)
    {
        return data.level;
    }

    public static int GetTotalPlayTime(UserData data)
    {
        return data.playTime;
    }

    public static int GetMoneyBalance(UserData data)
    {
        return data.money;
    }

    public static List<ItemSO> GetPlayerItems(UserData data, WorldAtlas worldAtlas)
    {
        List<ItemSO> playerItems = new List<ItemSO>();
        foreach (string itemId in data.items)
        {
            if(Int32.TryParse(itemId, out int id))
            {
                playerItems.Add(worldAtlas.GetWorldObject<ItemSO>(id));
            }
        }
        return playerItems;
    }

    public static List<SpellSO> GetPlayerSpells(UserData data, WorldAtlas worldAtlas)
    {
        List<SpellSO> playerSpells = new List<SpellSO>();
        foreach (string spellId in data.spells)
        {
            if(Int32.TryParse(spellId, out int id))
            {
                playerSpells.Add(worldAtlas.GetWorldObject<SpellSO>(id));
            }
        }
        return playerSpells;
    }

    public static List<RecipeSO> GetPlayerRecipes(UserData data, WorldAtlas worldAtlas)
    {
        List<RecipeSO> playerRecipes = new List<RecipeSO>();
        foreach (string recipeId in data.recipes)
        {
            if(Int32.TryParse(recipeId, out int id))
            {
                playerRecipes.Add(worldAtlas.GetWorldObject<RecipeSO>(id));
            }
        }
        return playerRecipes;
    }

    public static List<LocationSO> GetPlayerLocations(UserData data, WorldAtlas worldAtlas)
    {
        List<LocationSO> playerLocations = new List<LocationSO>();
        
        // Player data always contains special "hub" location data at index 0.
        LocationSO hub = worldAtlas.GetWorldObject<LocationSO>(1);
        hub.State = new LocationState(data.locations["hub"]);
        playerLocations.Add(hub);

        // Rest of locations
        foreach (KeyValuePair<string, string> pair in data.locations)
        {
            if(Int32.TryParse(pair.Key, out int id))
            {
                LocationSO baseLocation = worldAtlas.GetWorldObject<LocationSO>(id);
                baseLocation.State = new LocationState(pair.Value);
                playerLocations.Add(baseLocation);
            }
        }
        return playerLocations;
    }

    public static int GetPlayerProgress(UserData data)
    {
        int count = 0;
        foreach (int progress in data.progress)
        {
            if(progress != 0)
            {
                count++;
            }
        }
        return count;
    }
    
}