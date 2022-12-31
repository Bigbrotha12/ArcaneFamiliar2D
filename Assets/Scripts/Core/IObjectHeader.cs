using UnityEngine;

public interface IObjectHeader
{
    public int Id { get; set; }
    public string ObjectName { get; set; }
    public string Description { get; set; }
    public Sprite Icon { get; set; }
}