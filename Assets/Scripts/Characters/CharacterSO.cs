using UnityEngine;
using System.Collections.Generic;

namespace Characters
{
    [CreateAssetMenu(fileName = "New Character", menuName = "Characters/New Character", order = 0)]
    public class CharacterSO : ScriptableObject, IObjectHeader
    {
        [SerializeField] private int _id = 0;
        [SerializeField] private string _objectName;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;
        [SerializeField] private GameObject _prefab;
        [SerializeField] private float talkingSpeed;

        public int Id 
        { 
            get { return _id; } 
            set { 
                if(_id == 0)
                {
                    _id = value;
                } else 
                {
                    Debug.LogError("ID has already been set.");
                }
            } 
        }
        public string ObjectName 
        { 
            get { return _objectName; } 
            set 
            {
                if(_objectName == null)
                {
                    _objectName = value;
                } else 
                {
                    Debug.LogError("Object Name has already been set.");
                }
            }
        }
        public string Description
        {
            get { return _description; }
            set 
            {
                if(_description == null)
                {
                    _description = value;
                } else
                {
                    Debug.LogError("Description has already been set");
                }
            }
        }
        public Sprite Icon
        {
            get { return _icon; }
            set
            {
                if(_icon == null)
                {
                    _icon = value;
                } else
                {
                    Debug.LogError("Icon has already been set.");
                }
            }
        }
        public GameObject Prefab
        {
            get { return _prefab; }
            set
            {
                if(_prefab == null)
                {
                    _prefab = value;
                } else
                {
                    Debug.LogError("Prefab has already been set.");
                }
            }
        }

        public void Speak(List<string> message)
        {
            Debug.Log("CharacterSO " + this.ObjectName);
            GameManager.Instance.UInterface.QueueConversation(this, message, talkingSpeed);
        }

        public void Ask(List<string> message, string[] choices, Response replyCallback)
        {
            GameManager.Instance.UInterface.QueueConversation(this, message, choices, replyCallback, talkingSpeed);
        }
    }
}