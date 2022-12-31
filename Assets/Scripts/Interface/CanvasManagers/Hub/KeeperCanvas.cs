using UnityEngine;
using Environment;
using System.Collections;
using System.Collections.Generic;
using Characters;

// Library location NPC / Manager. They manage their own canvas
public class KeeperCanvas : UICanvasBase
{
    [SerializeField] private KeeperSO _keeperController;

    public override void OnEnable()
    {
        base.OnEnable();
        _keeperController.Greet();
    }

    // index 0: Talk, index 1: Work, index 2: ToHub
    protected override void HandleButtonSelect(int index)
    {
        switch(index)
        {
            case 0:
                _keeperController.Talk();
                break;
            case 1:
                _keeperController.ChooseWork();
                break;
            case 2:
                ActivateCanvas(false);
                break;
            default:
                Debug.Log("Invalid button index.");
                break;
        }
    }
}