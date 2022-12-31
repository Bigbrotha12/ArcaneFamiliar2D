using UnityEngine;
using System.Collections.Generic;

namespace Characters
{
    [CreateAssetMenu(fileName = "New Familiar", menuName = "Characters/New Familiar", order = 3)]
    public class FamiliarSO : CombatantSO
    {
        public ElementType ElementType;
        public Rarity Rarity;
        public int Generation;
    }
}