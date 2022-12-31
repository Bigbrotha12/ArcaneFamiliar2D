using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Characters
{
    [CreateAssetMenu(fileName = "New SpellCaster", menuName = "Characters/New SpellCaster", order = 4)]
    public class SpellCasterSO : CombatantSO
    {
        [SerializeField] public Familiars SpellCasterFamiliar;
        [SerializeField] public SpellBook SpellCasterSpellBook;
        [SerializeField] public LevelManager Level;
    
        public override int HP { get { return SpellCasterFamiliar.MainFamiliar.HP + SpellCasterFamiliar.SupportFamiliar.HP; } set { base.HP = value; }}
        public override int MP { get { return SpellCasterFamiliar.MainFamiliar.MP + SpellCasterFamiliar.SupportFamiliar.MP; } set { base.MP = value; }}
        public override int Attack { get { return SpellCasterFamiliar.MainFamiliar.Attack; } set { base.Attack = value; }}
        public override int Defense { get { return SpellCasterFamiliar.SupportFamiliar.Defense; } set { base.Defense = value; }}
        public override int Arcane { get { return SpellCasterFamiliar.MainFamiliar.Arcane; } set { base.Arcane = value; }}
        public override int Speed { get { return SpellCasterFamiliar.SupportFamiliar.Speed; } set { base.Speed = value; }}
              
    }
}