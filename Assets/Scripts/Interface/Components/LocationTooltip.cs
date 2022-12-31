using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Environment;

public class LocationTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TMP_Text _tooltipTextName;
    private TMP_Text _tooltipTextDescription;
    private Image _tooltipIconContainer;
    private string _tooltipText;
    private string _tooltipDescription;
    private Sprite _tooltipIcon;
    private Sprite _defaultIcon;

    public void SetTooltip(LocationSO location, Sprite defaultIcon, TMP_Text nameContainer, TMP_Text descriptionContainer, Image iconContainer)
    {
        _tooltipTextName = nameContainer;
        _tooltipTextDescription = descriptionContainer;
        _tooltipIconContainer = iconContainer;
        _defaultIcon = defaultIcon;
        
        _tooltipText = location.ObjectName;
        _tooltipDescription = location.Description;
        _tooltipIcon = location.Icon;
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _tooltipTextName.text = _tooltipText;
        _tooltipTextDescription.text = _tooltipDescription;
        _tooltipIconContainer.sprite = _tooltipIcon;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {    
        _tooltipTextName.text = "";
        _tooltipTextDescription.text = "";
        _tooltipIconContainer.sprite = _defaultIcon;
        
    }
}