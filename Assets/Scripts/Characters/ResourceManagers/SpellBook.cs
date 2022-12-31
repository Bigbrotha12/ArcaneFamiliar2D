using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System;
using Environment;

namespace Characters
{
    [System.Serializable]
    public class SpellBook
    {
        public List<SpellSO> SpellsKnown = new List<SpellSO>();
        public List<SpellSO> SpellsEquipped = new List<SpellSO>();
        public event Action EquipSpellChange;
        public event Action KnownSpellChange;

        public SpellBook(List<SpellSO> knownSpells, int preferences)
        {
            SpellsKnown = knownSpells is not null ? knownSpells : new List<SpellSO>();

            int toEquip = Mathf.Min(preferences, SpellsKnown.Count);
            for (int i = 0; i < toEquip; i++)
            {
                SpellsEquipped.Add(SpellsKnown[i]);
            }
        }

        public bool isEquipped(SpellSO spell)
        {
            if (spell is null) { return false; }
            return SpellsEquipped.Contains(spell);
        }

        public bool isKnown(SpellSO spell)
        {
            if (spell is null) { return false; }
            return SpellsKnown.Contains(spell);
        }

        public void EquipSpell(SpellSO spell)
        {
            if (spell is null) { return; }
            if(isKnown(spell))
            {
                if(!isEquipped(spell))
                {
                    SpellsEquipped.Add(spell);
                    EquipSpellChange.Invoke();
                    Debug.Log("Spell Equipped.");
                } else
                {
                    GameManager.Instance.UInterface.QueueAlert("This spell is already equipped.");
                }
            } else
            {
                GameManager.Instance.UInterface.QueueAlert("You do not know this spell.");
            }
        }

        public void UnEquipSpell(SpellSO spell)
        {
            if (spell is null) { return; }
            SpellsEquipped.Remove(spell);
            EquipSpellChange.Invoke();
        }

        public void LearnNewSpell(SpellSO spell)
        {
            if (spell is null) { return; }
            if(isKnown(spell))
            {
                Debug.LogError("You already know this spell.");
            } else
            {
                SpellsKnown.Add(spell);
                KnownSpellChange.Invoke();
            }
        }

        public void RemoveSpell(SpellSO spell)
        {
            if (spell is null) { return; }
            SpellsKnown.Remove(spell);
            KnownSpellChange.Invoke();
        }

        public void ClearEventListeners()
        {
            EquipSpellChange = null;
            KnownSpellChange = null;
        }
    }
}