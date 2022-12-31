using UnityEngine;

public class Statistics
        {
            [Header("Statistics:")]
            [Range(0, 500), SerializeField] public int HP;
            [Range(0, 100), SerializeField] public int MP;
            [Range(0, 100), SerializeField] public int Attack;
            [Range(0, 100), SerializeField] public int Defense;
            [Range(0, 200), SerializeField] public int Arcane;
            [Range(0, 100), SerializeField] public int Speed;
        }