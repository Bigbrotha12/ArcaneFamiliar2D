using System.Collections.Generic;
using UnityEngine;

namespace Characters
{
    public class NPCSO : CharacterSO
    {
        [SerializeField] protected List<string> Greeting;

        public void Greet()
        {
            List<string> greeting = new List<string>(Greeting);
            Speak(greeting);
        }
    }
}