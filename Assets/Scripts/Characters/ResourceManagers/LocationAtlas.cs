using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Environment;

namespace Characters
{
    [System.Serializable]
    public class LocationAtlas
    {
        public LocationState HubInitData { get; private set; }
        public List<LocationSO> LocationsKnown { get; private set; }
        public LocationSO CurrentLocation { get; private set; }
        public int SubLocationIndex { get; private set; }
        public LocationSO HubLocation { get; private set; }

        public LocationAtlas(List<LocationSO> initialLocations)
        {
            // Initial location is title screen [index 1]
            CurrentLocation = GameManager.Instance.WorldAtlas.GetWorldObject<LocationSO>(1);
            SubLocationIndex = 0;

            LocationsKnown = initialLocations is not null ? initialLocations : new List<LocationSO>();
            if(LocationsKnown.Count > 0)
            {
                // Get Hub index from hub state date
                HubInitData = LocationsKnown[0].State;
                LocationsKnown.RemoveAt(0);

                // Set default location to Guild Academy [index 10]
                HubLocation = GameManager.Instance.WorldAtlas.GetWorldObject<LocationSO>(10);

                // Set current location to indexed hub location
                int hubId = HubInitData.DecodeQuestProgress();
                foreach (LocationSO location in LocationsKnown)
                {
                    if(location.Id == hubId)
                    {
                        Debug.Log("Found hub location: " + location.ObjectName);
                        Debug.Log("Matching Quest Code: " + hubId.ToString());
                        HubLocation = location;
                        break;
                    }
                }
            } else
            {
                Debug.LogError("Missing initial location.");
            }  
        }
    
        public void TravelToLocation(LocationSO newLocation)
        {
            if (newLocation == CurrentLocation) { return; }
            
            LocationSO previousLocation = CurrentLocation;
            int previousSubLocation = SubLocationIndex;
            CurrentLocation = newLocation;
            SubLocationIndex = 0;
            GameManager.Instance.UInterface.StartLoading();

            // Update hub init data if applicable
            if(newLocation.LocationType is GlobalStates.PREPARATION)
            {
                HubInitData.UpdateQuestState(newLocation.Id);
            }

            // Unload current location
            if(previousLocation.Id != 1)
            {
                previousLocation.OnSceneExit();
            }
            
            // Load new location
            newLocation.LoadFirstScene();
            
        }

        public void MoveSubLocation(int locationIndex)
        {
            int previousSubLocation = SubLocationIndex;
            SubLocationIndex = locationIndex;

            CurrentLocation.OnSceneExit();
            CurrentLocation.LoadSubScene(locationIndex);
        }

        public void AddLocation(LocationSO location)
        {
            if(!LocationsKnown.Contains(location))
            {
                location.State = new LocationState("0000");
                LocationsKnown.Add(location);
            }
        }

        public void RemoveLocation(LocationSO location)
        {
            LocationsKnown.Remove(location);
        }
    }
}