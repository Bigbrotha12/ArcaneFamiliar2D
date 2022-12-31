using UnityEngine.Events;

namespace Characters
{
    [System.Serializable]
    public class LevelManager
    {
        // Player level will go from 1 to 20. 
        // _level holds current level (int part) + progress (4 digit decimals).
        private int _totalEXP;
        
        public int PlayerLevel 
        { 
            get { return (int) Math.Floor(Math.Max(CalculateLevel(_totalEXP), 1)); }
        }
        public double LevelProgress 
        {
            get { return CalculateLevel(_totalEXP) % 1; }
        }
        public int TotalEXP 
        {
            get { return _totalEXP;  } 
            private set { _totalEXP = value; }
        }
        public UnityEvent LevelUp;

        public LevelManager(int totalEXP)
        {
            TotalEXP = totalEXP;
        }

        public void AddExperience(int exp)
        {
            int currentLevel = PlayerLevel;
            TotalEXP += exp;
            if (PlayerLevel > currentLevel) { LevelUp.Invoke(); }
        }

        private double CalculateLevel(int exp)
        {
            // Takes the earned EXP, calculates the level points
            // to assign to _level on logarithmic scale. Scale designed to take
            // 10,000 EXP to reach cap level 20.
            return Math.Round(Math.Log(exp + 1) + (exp / 1000) + 1, 4);
        }
    }
}