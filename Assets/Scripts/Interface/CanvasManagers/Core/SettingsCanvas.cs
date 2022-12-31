using UnityEngine;

public class SettingsCanvas : UICanvasBase
{    
    // index 0: close, index 1: Controls, index 2: defaults
    protected override void HandleButtonSelect(int index)
    {
        switch (index)
        {
            case 0:
                ActivateCanvas(false);
                break;
            case 1:
                break;
            case 2:
                break;
            default:
                Debug.LogError("Button event not handled: " + index.ToString());
                break;
        }
    }
}