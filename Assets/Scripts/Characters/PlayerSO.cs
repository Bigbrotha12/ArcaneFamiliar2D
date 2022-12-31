using UnityEngine;
using Environment;
using System.Collections.Generic;

namespace Characters
{
    public class PlayerSO : CombatantSO
    {   
        public Inventory Inventory;
        public Familiars Familiars;
        public SpellBook SpellBook;
        public RecipeBook RecipeBook;
        public LocationAtlas Atlas;
        public LevelManager Level;
        public Wallet Wallet;
        public TimeKeeper Timer;
        public int NewFamiliarProgress;

        
    }   
}