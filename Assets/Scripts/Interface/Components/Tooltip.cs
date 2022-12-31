using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class Tooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text tooltip;
    [SerializeField] private string tooltipText;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.text = tooltipText;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {    
        tooltip.text = "";
    }
}