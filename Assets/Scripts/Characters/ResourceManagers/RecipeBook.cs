using System.Collections.Generic;
using Environment;

namespace Characters
{
    [System.Serializable]
    public class RecipeBook
    {
        public List<RecipeSO> RecipesKnown { get; private set; }
        private int CATALYST_OFFSET = 10;

        public RecipeBook(List<RecipeSO> initialRecipes)
        {
            RecipesKnown = initialRecipes is not null ? initialRecipes : new List<RecipeSO>();
        }

        public void AddRecipe(RecipeSO newRecipe)
        {
            if (newRecipe is null) { return; }
            if(!RecipesKnown.Contains(newRecipe))
            {
                RecipesKnown.Add(newRecipe);
            }
        }

        public List<RecipeSO> GetCraftableRecipes(List<ItemSO> ingredients)
        {
            if (ingredients is null) { return null; }
            List<RecipeSO> craftables = new List<RecipeSO>();

            foreach (RecipeSO recipe in RecipesKnown)
            {
                if(IsCraftable(recipe, ingredients))
                {
                    craftables.Add(recipe);
                }
            }
            return craftables;
        }

        public ItemSO GetRequiredCatalyst(RecipeSO recipe)
        {
            if (recipe is null || GameManager.Instance.Player.Atlas.LocationsKnown is null) { return null; }
            // Seed 1
            int s1 = GameManager.Instance.Player.Atlas.LocationsKnown[0].State.GetSeedOne();
            foreach (ItemSO item in recipe.Ingredients)
            {
                s1 += item.CraftValue;
            }

            int lastDigit = s1 % 10;
            int catalystId = (10 - lastDigit) + CATALYST_OFFSET;
            return GameManager.Instance.WorldAtlas.GetWorldObject<ItemSO>(catalystId);
        }

        public bool IsCraftable(RecipeSO recipe, List<ItemSO> ingredients)
        {
            if (recipe is null || ingredients is null) { return false; }
            if (!ingredients.Contains(GetRequiredCatalyst(recipe))) { return false; }

            foreach (ItemSO requiredIngredient in recipe.Ingredients)
            {
                if(!ingredients.Contains(requiredIngredient))
                {
                    return false;
                }
            }
            return true;
        }
    }
}