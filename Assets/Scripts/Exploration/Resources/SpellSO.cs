namespace Environment
{
    using UnityEngine;
    using System.Collections.Generic;

    [CreateAssetMenu(fileName = "New Spell", menuName = "Artifacts/New Spell")]
    public class SpellSO : ScriptableObject, IObjectHeader
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

        public List<EffectSO> SpellEffects { get; set; }
        public int Priority;
        public GameObject spellVisualEffect;
        public int Cost;
        public int Power;
        public Target SpellTarget;
    }
}