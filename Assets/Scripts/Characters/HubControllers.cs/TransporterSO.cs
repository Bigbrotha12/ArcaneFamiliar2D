using System.Collections.Generic;
using UnityEngine;
using Environment;

namespace Characters
{
    [CreateAssetMenu(fileName = "New Transporter", menuName = "Characters/Hub/New Transporter", order = 0)]
    public class TransporterSO : NPCSO
    {
        [SerializeField] private int drivingFee;
        [SerializeField] private List<LocationSO> forbiddenLocations;

        public LocationSO[] FindRoute(LocationSO destination)
        {
            if (destination is null || forbiddenLocations.Contains(destination)) 
            {
                Speak(new List<string>() { "I can't get you there. Sorry." });
                return null; 
            }
            else if (destination == GameManager.Instance.Player.Atlas.CurrentLocation)
            {
                Speak(new List<string>() { "That's where we are right now." });
                return null; 
            }

            // Get route
            return 
                GameManager.Instance.Player.Atlas.CurrentLocation.FindRouteBFS
                (destination as LocationSO, GameManager.Instance.Player.Atlas.LocationsKnown);
        }

        public int RouteCost(LocationSO[] route)
        {
            if (route is null) { return 0; }
            int cost = drivingFee;
            foreach (LocationSO locale in route)
            {
                cost += locale.TravelCost;
            }
            return cost;
        }

        public void StartTravel(LocationSO destination, int cost)
        {
            if(destination is null || !GameManager.Instance.Player.Atlas.LocationsKnown.Contains(destination)) 
            {
                Debug.LogError("Invalid destination.");
                return;
            }

            // Player funds check
            if(GameManager.Instance.Player.Wallet.Money < cost)
            {
                Speak(new List<string>(){"Sorry, you cannot afford this trip."});
                return;
            }

            GameManager.Instance.Player.Wallet.SpendMoney(cost);
            GameManager.Instance.Player.Atlas.TravelToLocation(destination);
        }
    }
}