using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace Environment
{
    [CreateAssetMenu(fileName = "New Location", menuName = "Artifacts/New Location")]
    public class LocationSO : ScriptableObject, IObjectHeader
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

        [SerializeField] protected ElementType _plane;
        [SerializeField] protected GlobalStates _locationType;
        [SerializeField] protected List<LocationSO> _neighbors;
        [SerializeField] protected Vector2 _mapCoordinates;
        [SerializeField] protected int _travelCost;
        [SerializeField] protected List<AssetReference> _sceneReference;

        public List<LocationSO> Neighbors
        {
            get { return _neighbors; }
            set { _neighbors = _neighbors is null ? value : _neighbors; }
        }
        public Vector2 MapCoordinate
        {
            get { return _mapCoordinates; }
            set { _mapCoordinates = _mapCoordinates == null ? value : _mapCoordinates; }
        }
        public ElementType Plane
        {
            get { return _plane; }
            set { _plane = _plane is default(ElementType) ? value : _plane; }
        }
        public GlobalStates LocationType 
        {
            get { return _locationType; }
            set { _locationType = _locationType is default(GlobalStates) ? value : _locationType; }
        }
        public List<AssetReference> SceneReference
        {
            get { return _sceneReference; }
            set { _sceneReference = _sceneReference is null ? value : _sceneReference; }
        }
        public int TravelCost
        {
            get { return _travelCost; }
            set { _travelCost = _travelCost is 0 ? value : _travelCost; }
        }
        public LocationState State;

        public void LoadFirstScene()
        {
            if (SceneReference is null || SceneReference.Count == 0) 
            { 
                Debug.LogError("Error: Missing scene references.");
                return; 
            }
            GameManager.Instance.Loader.LoadRemoteScene(SceneReference[0]);
        }

        public void OnSceneExit()
        {
            Debug.Log("Unloading scene ID: " + Id.ToString());
            // IntroScreen [index 1] should not be unloaded via Addressables.
            if(LocationType is GlobalStates.PREGAME)
            {
                return;
            }
            GameManager.Instance.Loader.UnloadActiveScene();
        }

        public void LoadSubScene(int index)
        {
            if (SceneReference is null || SceneReference.Count <= index) 
            { 
                Debug.LogError("Error: Missing scene references.");
                return; 
            }
            GameManager.Instance.Loader.LoadRemoteScene(SceneReference[index]);
        }

        // Find route between this location and target location, if it exist.
        // Uses BFS algorithm.
        public LocationSO[] FindRouteBFS(LocationSO end, List<LocationSO> knownLocation)
        {
            // Check self-referencing
            if (end is null || end.Id == this.Id) { return null; }

            // Initialize revisit guard, route list, and search queue.
            List<LocationSO> searchedLocations = new List<LocationSO>(){ this };
            List<LocationSO> route = new List<LocationSO>() { this };
            List<(List<LocationSO> route, LocationSO locale)> SearchList = new List<(List<LocationSO>, LocationSO)>();

            // Add start location's neighbors as first layer to search
            foreach (LocationSO location in this.Neighbors)
            {
                if(knownLocation.Contains(location))
                {
                    SearchList.Add((route, location));
                }
            }

            // Main search loop
            while(SearchList.Count > 0)
            {
                // Get location to search in queue
                LocationSO location = SearchList[0].locale;

                // Check it hasn't been visited
                if(searchedLocations.Contains(location))
                {
                    SearchList.RemoveAt(0);
                    continue;
                }

                // Check if it's the target location
                if(location.Id == end.Id)
                {
                    SearchList[0].route.Add(location);
                    return SearchList[0].route.ToArray();
                }

                // If not target location, generate new potential route based on location neighbors
                List<LocationSO> potentialRoute = new List<LocationSO>(SearchList[0].route);
                potentialRoute.Add(location);

                foreach (LocationSO neighbor in location.Neighbors)
                {
                    if (knownLocation.Contains(neighbor))
                    {
                        SearchList.Add((potentialRoute, neighbor));
                    }
                }

                // Add location to list of searched locations and remove it from search list.
                searchedLocations.Add(location);
                SearchList.RemoveAt(0);
            }

            // If loop completes without finding target location, return null
            return null;
        }
    }

    public class LocationState
    {
        public string CurrentState { get; private set; }

        public LocationState(string initialState)
        {
            CurrentState = initialState is not null ? initialState : "00000";
        }

        /**
        Location data is encoded as string of base16 as follow
        State: 
        0 = seed 1, size = 1
        1 = seed 2, size = 1
        2 = quest progression, size = 2
        3 = NPC number, size = 1
        4 = NPC states, size = 1 * NPC number
        */
        private struct StateStruct
        {
            public int s1_size;
            public int s2_size;
            public int quest_size;
            public int numNPC_size;
            public int stateNPC_size;
        }
        private static int baseEncoding = 16;
        private static StateStruct layout = new StateStruct()
        {
            s1_size = 1,
            s2_size = 1,
            quest_size = 2,
            numNPC_size = 1,
            stateNPC_size = 2
        };

        public static string EncodeLocationState(int s1, int s2, int quest, int[] npcStates)
        {
            // Allocate memory for return value
            string encodedState = "";

            // Sanitize inputs
            int seedOne = Mathf.Clamp(s1, 0, layout.s1_size * baseEncoding);
            int seedTwo = Mathf.Clamp(s2, 0, layout.s2_size * baseEncoding);
            int questProgress = Mathf.Clamp(quest, 0, layout.quest_size * baseEncoding);
            int npcStateLength = Mathf.Clamp(npcStates.Length, 0, layout.numNPC_size * baseEncoding);

            // Encode data
            encodedState += EncodeIntToBase16(seedOne, layout.s1_size);
            encodedState += EncodeIntToBase16(seedTwo, layout.s2_size);
            encodedState += EncodeIntToBase16(questProgress, layout.quest_size);
            encodedState += EncodeIntToBase16(npcStates.Length, layout.numNPC_size);
            foreach (int state in npcStates)
            {
                int data = Mathf.Clamp(state, 0, layout.stateNPC_size * baseEncoding);
                encodedState += EncodeIntToBase16(data, layout.stateNPC_size);
            }
            return encodedState;
        }

        private static string EncodeIntToBase16(int input, int size)
        {
            if(size == 1)
            {
                return input.ToString("X");
            } else
            {
                return input.ToString("X" + size.ToString());
            } 
        }

        private static int DecodeBase16ToInt(string input)
        {
            return System.Convert.ToInt32(input, 16);
        }

        public int GetSeedOne()
        {
            return DecodeBase16ToInt(CurrentState.Substring(layout.s1_size - 1, layout.s1_size));
        }

        public int GetSeedTwo()
        {
            return DecodeBase16ToInt(CurrentState.Substring(layout.s1_size + layout.s2_size - 1, layout.s2_size));
        }

        public int DecodeQuestProgress()
        {
            return DecodeBase16ToInt(CurrentState.Substring(layout.s1_size + layout.s2_size + layout.quest_size - 1, layout.quest_size));
        }

        public int GetNPCSize()
        {
            return DecodeBase16ToInt(CurrentState.Substring(layout.s1_size + layout.s2_size + layout.quest_size + layout.numNPC_size - 1, layout.numNPC_size));
        }

        private void SetNPCSize(int newSize)
        {
            // Sanitize input
            if(newSize >= layout.numNPC_size * baseEncoding || newSize < 0) 
            {
                Debug.LogError("Size out of range.");
                return;
            }

            int sizeIndex = layout.s1_size + layout.s2_size + layout.quest_size;
            string newStateValue = EncodeIntToBase16(newSize, layout.numNPC_size);
            CurrentState = CurrentState.Substring(0, sizeIndex) + newStateValue + CurrentState.Substring(sizeIndex + layout.numNPC_size);
        }

        public int[] GetNPCProgress()
        {
            int size = GetNPCSize();
            int[] result = new int[size];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = DecodeBase16ToInt
                (
                    CurrentState.Substring
                    (
                        layout.s1_size + layout.s2_size + layout.quest_size + layout.numNPC_size + (layout.stateNPC_size * i) - 1, 
                        layout.stateNPC_size
                    )
                );
            }
            return result;
        }

        public void UpdateQuestState(int newState)
        {
            // Sanitize input
            int sNewState = Mathf.Clamp(newState, 0, layout.quest_size * baseEncoding);
        
            int questIndex = layout.s1_size + layout.s2_size;
            string newStateValue = EncodeIntToBase16(sNewState, layout.quest_size);
            CurrentState = CurrentState.Substring(0, questIndex) + newStateValue + CurrentState.Substring(questIndex + layout.quest_size);
        }

        public void UpdateNPCState(int newState, int npcIndex)
        {
            // Sanitize input
            int size = GetNPCSize();
            if(npcIndex < 0 || npcIndex >= size) 
            {
                Debug.LogError("Invalid NPC index");
                return;
            }
            int sNewState = Mathf.Clamp(newState, 0, layout.stateNPC_size * baseEncoding);
            
            int NPCStateIndex = layout.s1_size + layout.s2_size + layout.quest_size + layout.numNPC_size + (layout.stateNPC_size * npcIndex);
            string newStateValue = EncodeIntToBase16(newState, layout.stateNPC_size);
            CurrentState = CurrentState.Substring(0, NPCStateIndex) + newStateValue + CurrentState.Substring(NPCStateIndex + layout.stateNPC_size);
        }

        public void AddNPC(int initialState)
        {
            // Sanitize input
            if(initialState >= layout.stateNPC_size * baseEncoding)
            {
                Debug.LogError("Invalid initial state.");
                return;
            }

            int size = GetNPCSize();
            if(size + 1 >= layout.numNPC_size * baseEncoding)
            {
                Debug.LogError("No more space for NPC.");
                return;
            }

            SetNPCSize(size + 1);
            UpdateNPCState(initialState, size);
        }

        public void RemoveNPC(int npcIndex)
        {
            int size = GetNPCSize();
            // Sanitize input
            if(npcIndex < 0 || npcIndex >= size || size == 0)
            {
                Debug.LogError("Invalid NPC index.");
                return;
            }

            SetNPCSize(size - 1);
            int NPCStateIndex = layout.s1_size + layout.s2_size + layout.quest_size + layout.numNPC_size + (layout.stateNPC_size * npcIndex);
            CurrentState = CurrentState.Substring(0, NPCStateIndex) + CurrentState.Substring(NPCStateIndex + layout.stateNPC_size);
        }
    }

}