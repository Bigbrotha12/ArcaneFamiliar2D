using System.Collections.Generic;

namespace Characters
{
    public static class FamiliarType
    {
        public static Dictionary<string, ElementType> FamiliarAffinity = new Dictionary<string, ElementType>(){
            {"Light", ElementType.LIGHT},
            {"Dark", ElementType.DARK},
            {"Fire", ElementType.FIRE},
            {"Water", ElementType.WATER},
            {"Earth", ElementType.EARTH},
            {"Wind", ElementType.WIND}
        };
        public static Dictionary<string, Rarity> FamiliarRarity = new Dictionary<string, Rarity>(){
            {"common", Rarity.COMMON},
            {"uncommon", Rarity.UNCOMMON},
            {"rare", Rarity.RARE},
            {"secret", Rarity.SECRET},
            {"legendary", Rarity.LEGENDARY},
        };
        public static int HP_MAX = 500;
        public static int MP_MAX = 100;
        public static int ATK_MAX = 100;
        public static int DEF_MAX = 100;
        public static int ARC_MAX = 200;
        public static int SPD_MAX = 100;
    }
    
}