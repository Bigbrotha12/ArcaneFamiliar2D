using UnityEngine;
using System.Collections.Generic;

namespace Environment
{
    [CreateAssetMenu(fileName = "New Recipe", menuName = "Artifacts/New Recipe")]
    public class RecipeSO : ScriptableObject, IObjectHeader
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

        private readonly int CHECKSUMOFFSET = 10;
        private ItemSO checkSumItem;
        [SerializeField] protected CraftingMode _type;
        [SerializeField] protected List<ItemSO> _ingredients;
        [SerializeField] protected ItemSO _itemProduct;
        [SerializeField] protected ItemSO[] _itemVariant;
        [SerializeField] protected SpellSO _spellProduct;
        [SerializeField] protected SpellSO[] _spellVariant;
        public CraftingMode CraftType { get { return _type; } }
        public List<ItemSO> Ingredients
        {
            get { return _ingredients; }
            set { _ingredients = _ingredients is null ? value : _ingredients; }
        }
        public IObjectHeader Product
        {
            get { return _itemProduct is not null ? _itemProduct : _spellProduct; }
            set 
            {
                _itemProduct = value is ItemSO ? value as ItemSO : null;
                _spellProduct = value is SpellSO ? value as SpellSO : null; 
            }
        }
        public int CraftValue 
        { 
            get 
            {
                int result = 0;
                foreach (ItemSO item in _ingredients)
                {
                    result += item.CraftValue;
                }
                return result;
            } 
        }

        public IObjectHeader GetVariant(int variant, CraftingMode type)
        {
            if(type is CraftingMode.Spell)
            {
                if(_spellVariant.Length > variant)
                {
                    return _spellVariant[variant];
                }
            } 
            else if(type is CraftingMode.Item)
            {
                if(_itemVariant.Length > variant)
                {
                    return _itemVariant[variant];
                }
            }
            return null;
        }

        private int GetKey()
        {
            return GameManager.Instance.Player.Atlas.HubInitData.GetSeedOne();
        }

        public bool IsCraftable(List<ItemSO> ingredients)
        {
            Debug.Log("Checking if " + this.ObjectName + " is craftable.");
            if(this.Ingredients is not null)
            {
                foreach (ItemSO requiredIngredient in this.Ingredients)
                {
                    if(!ingredients.Contains(requiredIngredient))
                    {
                        Debug.Log("Missing required ingredient " + requiredIngredient.ObjectName);
                        return false;
                    }
                }
                return ingredients.Contains(this.GetCheckSumItem());
            }
            return false;
        }
        public ItemSO GetCheckSumItem()
        {
            if (checkSumItem is not null) { return checkSumItem; }

            int digit = GetKey();
            foreach (ItemSO item in this.Ingredients)
            {
                digit += item.CraftValue;
            }

            int offsetItem = (10 - (digit % 10)) + CHECKSUMOFFSET;
            return GameManager.Instance.WorldAtlas.GetWorldObject<ItemSO>(offsetItem);
        }
    }
}