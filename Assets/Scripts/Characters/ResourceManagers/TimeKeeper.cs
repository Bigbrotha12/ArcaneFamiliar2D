using System;
using UnityEngine;

namespace Environment
{
    public class TimeKeeper : MonoBehaviour
    {
        // Time units stored in Unix Epoch time
        private int _totalPlayTime;
        private float _currentSessionTime = 0;
        private int HOUR = 3600;
        private int MINUTE = 60;

        public int PlayTime { 
            get { return _totalPlayTime + (int)_currentSessionTime; } 
            set 
            {
                if(_totalPlayTime == 0)
                {
                    _totalPlayTime = value;
                }
            }
        }
        
        public void Update()
        {
            _currentSessionTime += Time.deltaTime;
        }

        public string PlayTimeString()
        {
            int totalSeconds = PlayTime;
            int hoursRemainder = totalSeconds % HOUR;
            int hours = (totalSeconds - hoursRemainder) / HOUR;
            int secondsRemainder = hoursRemainder % MINUTE;
            int minutes = (hoursRemainder - secondsRemainder) / MINUTE;
            int seconds = secondsRemainder;

            string time = "";
            if (hours > 0) { time += hours.ToString() + " : "; }
            if (minutes > 0) { time += minutes.ToString() + " : "; }
            time += seconds.ToString();
            return time;
        }
    }
}