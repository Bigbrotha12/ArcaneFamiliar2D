using UnityEngine;

namespace Characters
{
    [System.Serializable]
    public class Wallet
    {
        private int moneyLimit = 9999;
        private int _money = -1;
        public int Money 
        { 
            get { return Mathf.Min(_money, moneyLimit); } 
            set 
            {
                if(_money == -1)
                {
                    _money = value;
                }
            }
        }

        public Wallet(int intialBalance)
        {
            Money = intialBalance;
        }

        public bool SpendMoney(int cost)
        {
            if(cost > 0 && cost > _money)
            {
                Debug.Log("Cannot afford this.");
                return false;
            }
            _money = _money - cost;
            return true;
        }

        public void EarnMoney(int earning)
        {
            if(earning < 0) { return; }
            _money += earning;
        }
    }
}