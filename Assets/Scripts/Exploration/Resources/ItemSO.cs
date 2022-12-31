using UnityEngine;
using System.Collections.Generic;
using Characters;

namespace Environment
{
    // All world objects must be stateless or be ok if state is shared.
    [CreateAssetMenu(fileName = "New Item", menuName = "Artifacts/New Item")]
    public class ItemSO : ScriptableObject, IObjectHeader
    {
        [SerializeField] protected int _id;
        [SerializeField] protected string _objectName;
        [SerializeField] protected string _description;
        [SerializeField] protected Sprite _icon;
        public int Id
        {
            get { return _id; }
            set { _id = _id is 0 ? value : _id; }
        }
        public string ObjectName
        {
            get { return _objectName; }
            set { _objectName = _objectName is null ? value : _objectName; }
        }
        public string Description
        {
            get { return _description; }
            set { _description = _description is null ? value : _description; }
        }
        public Sprite Icon
        {
            get { return _icon; }
            set { _icon = _icon is null ? value : _icon; }
        }

        [SerializeField] protected ItemCategory _category;
        [SerializeField] protected Rarity _rarity;
        [SerializeField] protected int _craftValue;
        [SerializeField] protected int _cost;
        [SerializeField] protected List<EffectSO> _itemEffects;
        [SerializeField] protected IObjectHeader _learnableContent;
        [SerializeField] protected List<string> _flavorDescription;
        [SerializeField] protected Target _itemTarget;
        public ItemCategory Category 
        { 
            get { return _category; }
            set { _category = _category is default(ItemCategory) ? value : _category; }
        }
        public Rarity Rarity 
        { 
            get { return _rarity; } 
            set { _rarity = _rarity is default(Rarity) ? value : _rarity; }
        }
        public int CraftValue
        { 
            get { return _craftValue; }
            set { _craftValue = _craftValue is 0 ? value : _craftValue; }
        }
        public int Cost
        { 
            get { return _cost; }
            set { _cost = _cost is 0 ? value : _cost; }
        }
        public List<EffectSO> ItemEffects 
        {
            get { return _itemEffects; }
            set { _itemEffects = _itemEffects is null ? value : _itemEffects; }
        }
        public IObjectHeader LearnableContent 
        { 
            get { return _learnableContent; }
            set { _learnableContent = _learnableContent is null ? value : _learnableContent; }
        }
        public List<string> FlavorDescription 
        { 
            get { return _flavorDescription; }
            set { _flavorDescription = _flavorDescription is null ? value : _flavorDescription; }
        }
        public Target ItemTarget 
        { 
            get { return _itemTarget; }
            set { _itemTarget = _itemTarget is default(Target) ? value : _itemTarget; }
        }

        public bool Usable()
        {
            if(Category is ItemCategory.None || Category is ItemCategory.CRAFTING || Category is ItemCategory.LEARNABLE)
            {
                return false;
            }

            if(Category is ItemCategory.BATTLE)
            {
                return GameManager.Instance.GetCurrentState() is GlobalStates.BATTLE;
            }

            return true;
        }

        public void Use(CombatantSO target)
        {
            if(Usable())
            {
                foreach (EffectSO effect in ItemEffects)
                {
                    new Effect(effect, target);
                }
            }
        }
    }
    
}