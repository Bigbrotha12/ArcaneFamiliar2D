using Characters;
using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    [System.Serializable]
    public class Familiars
    {
        public List<FamiliarSO> FamiliarPacts { get; private set; }
        public FamiliarSO MainFamiliar 
        { 
            get
            {
                return MainFamiliar is null ? GameManager.Instance.WorldAtlas.GetWorldObject<FamiliarSO>(0) : MainFamiliar;
            } 
            private set
            {
                MainFamiliar = value;
            } 
        }
        public FamiliarSO SupportFamiliar
        { 
            get
            {
                return SupportFamiliar is null ? GameManager.Instance.WorldAtlas.GetWorldObject<FamiliarSO>(0) : SupportFamiliar;
            } 
            private set
            {
                SupportFamiliar = value;
            } 
        }

        public Familiars(List<FamiliarSO> initialFamiliars, int main, int support)
        {
            FamiliarPacts = initialFamiliars is not null ? initialFamiliars : new List<FamiliarSO>();
            
            if(FamiliarPacts.Count > main) { MainFamiliar = FamiliarPacts[main]; }
            if (FamiliarPacts.Count > support) { SupportFamiliar = FamiliarPacts[support]; }

        }

        public void SwapFamiliars()
        {
            FamiliarSO placeholder = SupportFamiliar;
            SupportFamiliar = MainFamiliar;
            MainFamiliar = placeholder;
        }

        public void BondMainFamiliar(FamiliarSO familiar)
        {
            if (familiar is null) { return; }
            if(!FamiliarPacts.Contains(familiar)) 
            {
                GameManager.Instance.UInterface.QueueAlert("You do not have a pact with this familiar.", 0);
                return;
            }
            if(familiar == SupportFamiliar)
            {
                SwapFamiliars();
            } else
            {
                MainFamiliar = familiar;
            }
        }

        public void BondSupportFamiliar(FamiliarSO familiar)
        {
            if (familiar is null) { return; }
            if(!FamiliarPacts.Contains(familiar)) 
            {
                GameManager.Instance.UInterface.QueueAlert("You do not have a pact with this familiar.", 0);
                return;
            }
            if(familiar == MainFamiliar)
            {
                SwapFamiliars();
            } else
            {
                SupportFamiliar = familiar;
            }
        }

        public async void LearnFamiliarName()
        {
            if(await GameManager.Instance.SaveGame(true))
            {
                GameManager.Instance.Player.NewFamiliarProgress++;
            }
            if(GameManager.Instance.Player.NewFamiliarProgress >= 4)
            {
                GameManager.Instance.UInterface.QueueAlert("New Familiar Spirit Available for summoning.", 0);
            }
        }

        public void MakeFamiliarPact()
        {
            // TODO: Implement GameManager call to Mint function
        }

        public void AddFamiliar(FamiliarSO familiar)
        {
            FamiliarPacts.Add(familiar);
        }

    }
}