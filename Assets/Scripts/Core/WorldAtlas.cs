using System.Collections.Generic;
using UnityEngine;
using Environment;
using Characters;

// Read-only class holds reference to all world object SOs. Sorted by ID.
public class WorldAtlas : MonoBehaviour
{
    [SerializeField] private List<ItemSO> _itemsRegistry;
    [SerializeField] private List<SpellSO> _spellsRegistry;
    [SerializeField] private List<AbilitySO> _abilitiesRegistry;
    [SerializeField] private List<EffectSO> _effectsRegistry;
    [SerializeField] private List<LocationSO> _locationsRegistry;
    [SerializeField] private List<RecipeSO> _recipesRegistry;
    [SerializeField] private List<CharacterSO> _charactersRegistry;
    [SerializeField] private List<FamiliarSO> _familiarsRegistry;
    public List<ItemSO> ItemsRegistry { get { return _itemsRegistry; }}
    public List<SpellSO> SpellsRegistry { get { return _spellsRegistry; }}
    public List<AbilitySO> AbilitiesRegistry { get { return _abilitiesRegistry; }}
    public List<EffectSO> EffectsRegistry { get { return _effectsRegistry; }}
    public List<LocationSO> LocationsRegistry { get { return _locationsRegistry; }}
    public List<RecipeSO> RecipesRegistry { get { return _recipesRegistry; }}
    public List<CharacterSO> CharactersRegistry { get { return _charactersRegistry; }}
    public List<FamiliarSO> FamiliarsRegistry { get { return _familiarsRegistry; }}

    public T GetWorldObject<T>(int index) where T : class
    {
        if(typeof(T) == typeof(ItemSO)) 
        {
            if (index >= _itemsRegistry.Count) { return _itemsRegistry[0] as T; }
            return _itemsRegistry[index] as T; 
        }
        if(typeof(T) == typeof(SpellSO)) 
        { 
            if (index >= _spellsRegistry.Count) { return _spellsRegistry[0] as T; }
            return _spellsRegistry[index] as T; 
        }
        if(typeof(T) == typeof(AbilitySO)) 
        { 
            if (index >= _abilitiesRegistry.Count) { return _abilitiesRegistry[0] as T; }
            return _abilitiesRegistry[index] as T; 
        }
        if(typeof(T) == typeof(EffectSO)) 
        { 
            if (index >= _effectsRegistry.Count) { return _effectsRegistry[0] as T; }
            return _effectsRegistry[index] as T; 
        }
        if(typeof(T) == typeof(LocationSO)) 
        { 
            if (index >= _locationsRegistry.Count) { return _locationsRegistry[0] as T; }
            return _locationsRegistry[index] as T; 
        }
        if(typeof(T) == typeof(RecipeSO)) 
        { 
            if (index >= _recipesRegistry.Count) { return _recipesRegistry[0] as T; }
            return _recipesRegistry[index] as T; 
        }
        if(typeof(T) == typeof(CharacterSO)) 
        { 
            if (index >= _charactersRegistry.Count) { return _charactersRegistry[0] as T; }
            return _charactersRegistry[index] as T; 
        }
        if(typeof(T) == typeof(FamiliarSO)) 
        { 
            if (index >= _familiarsRegistry.Count) { return _familiarsRegistry[0] as T; }
            return _familiarsRegistry[index] as T; 
        }
        throw new System.Exception("Invalid object type");
    }

    public AbilitySO GetAbilityByName(string name)
    {
        foreach (AbilitySO ability in _abilitiesRegistry)
        {
            if (ability.name == name) 
            {
                return ability;
            }
        }
        return _abilitiesRegistry[0];
    }
}