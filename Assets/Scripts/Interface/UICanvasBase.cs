using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Characters;

public delegate void Response(int index);
public delegate void ObjectResponse(IObjectHeader item, int index);

// Base class to be inherited by all UI canvas.
public abstract class UICanvasBase : MonoBehaviour
{
    [SerializeField] protected List<Button> CanvasButtons = new List<Button>();

    // Display/Remove itself
    public virtual void ActivateCanvas(bool state)
    {
        gameObject.SetActive(state);
    }

    // Handle canvas events 
    protected abstract void HandleButtonSelect(int index);

    public virtual void OnEnable()
    {
        for (int i = 0; i < CanvasButtons.Count; i++)
        {
            int index = i;
            CanvasButtons[index].onClick.AddListener(() => HandleButtonSelect(index));
        }
    }

    public virtual void OnDisable()
    {
        for (int i = 0; i < CanvasButtons.Count; i++)
        {
            int index = i;
            CanvasButtons[index].onClick.RemoveAllListeners();
        }
    }
}

public class Dialog
{
    private Queue<string> _lines;
    private string[] _options;
    private int _maxOptions = 4;
    private float _delay = 0.05f;
    
    public DialogType Type { get; private set; }
    public CharacterSO Speaker { get; private set; }
    public IObjectHeader DisplayableObject { get; private set; }
    public string[] Options 
    { 
        get { return _options; } 
        set 
        {
            if(value.Length > _maxOptions)
            {
                string[] slicedOption = new string[_maxOptions];
                for (int i = 0; i < _maxOptions; i++)
                {
                    slicedOption[i] = value[i];
                }
                _options = slicedOption;
            } else
            {
                _options = value;
            }
        }
    }
    public Response Reply;
    public ObjectResponse ObjectReply;
    public float Delay 
    { 
        get { return _delay; }
        private set { _delay = value; }
    }

    public void AddAlertOptions(string message, float timeout = 0)
    {
        Type = DialogType.ALERT;
        _lines = new Queue<string>();
       _lines.Enqueue(message);
        Delay = timeout > 0 ? timeout : Delay;
    }

    public void AddObjectOptions(IObjectHeader targetObject, float timeout = 0)
    {
        Type = DialogType.OBJECT;
        DisplayableObject = targetObject;
        Delay = timeout > 0 ? timeout : Delay;
    }

    public void AddConversationOptions(CharacterSO character, List<string> speech, float speed = 0)
    {
        Type = DialogType.CONVERSATION;
        Speaker = character;
        _lines = new Queue<string>();
       foreach (string line in speech)
       {
            _lines.Enqueue(line);
        }
        Delay = speed > 0 ? speed : Delay;
    }

    public int GetLineCount()
    {
        return _lines is not null ? _lines.Count : 0;
    }

    public string GetNextLine()
    {
        if(_lines is not null && _lines.Count > 0)
        {
            string nextLine = _lines.Dequeue();
            return nextLine;
        }
        return null;
    }

    public void SendReply(int index)
    {
        switch (Type)
        {
            case DialogType.ALERT:
                if (Reply is null) { return; }
                Reply(index);
                break;
            case DialogType.OBJECT:
                if (ObjectReply is null || DisplayableObject is null) { return; }
                ObjectReply(DisplayableObject, index);
                break;
            case DialogType.CONVERSATION:
                if (Reply is null) { return; }
                Reply(index);
                break;
            default:
                Debug.LogError("Unhandled Dialog Type.");
                break;
        }
    }
}

public enum DialogType
{
    ALERT,
    OBJECT,
    CONVERSATION
}

