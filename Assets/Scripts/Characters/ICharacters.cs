using System.Collections.Generic;

namespace Characters
{
    public interface ICharacters
    {
        public PlayerSO CreatePlayer(UserData gameData, List<FamiliarMetadata> familiars, WorldAtlas worldAtlas);
        public CombatantSO CreateCombatant(CombatantSO template);
        public List<CombatantSO> CreateCombatant(List<CombatantSO> templates);
        public UserData PrepareSaveData();
        public void UpdatePreferences(Preferences playerPreference);
    }
}