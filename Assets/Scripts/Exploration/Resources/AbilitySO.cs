using UnityEngine;
using System.Collections.Generic;
using Characters;

namespace Environment
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Artifacts/New Ability")]
    public class AbilitySO : ScriptableObject, IObjectHeader
    {
        [SerializeField] protected int _id;
        [SerializeField] protected string _objectName;
        [SerializeField] protected string _description;
        [SerializeField] protected Sprite _icon;
        [SerializeField] protected List<EffectSO> _benefits;
        [SerializeField] protected List<EffectSO> _drawbacks;
        [SerializeField] protected ElementType _favoredElement;
        [SerializeField] protected ElementType _unfavoredElement;
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
        
        public List<EffectSO> Benefits { get { return _benefits; } }
        public List<EffectSO> Drawbacks { get { return _drawbacks; } }
    
        public void Activate(CombatantSO target, ElementType element)
        {
            if(element != _unfavoredElement)
            {
                foreach (EffectSO effect in Benefits)
                {
                    new Effect(effect, target);
                }
            }
            if(element != _favoredElement)
            {
                foreach (EffectSO effect in Drawbacks)
                {
                    new Effect(effect, target);
                }
            }
        }
    }
}