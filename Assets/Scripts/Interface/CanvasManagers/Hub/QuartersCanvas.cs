
using UnityEngine;
using Environment;

public class QuartersCanvas : UICanvasBase
{
    // index 0: Summon, index 1: View, index 2: Exit
    protected override void HandleButtonSelect(int index)
    {
        switch(index)
        {
            case 0:
                
                break;
            case 1:
                
                break;
            case 2:
                ActivateCanvas(false);
                break;
            default:
                Debug.Log("Invalid button index.");
                break;
        }
    }

    public void ViewLocations()
    {
        // TODO: Display map and known locations
        //DisplayLocationsKnown();
    }

    public void ViewRecipes()
    {
        // TODO: Display recipes known
        // DisplayRecipes();
    }

    public void ViewAbilities()
    {
        // TODO: Display all passive abilities known
    }
}