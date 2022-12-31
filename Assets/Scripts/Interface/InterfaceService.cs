using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Characters;

namespace UserInterface
{
    public class InterfaceService : MonoBehaviour, IUserInterface
    {
        #region Common
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private GameObject faderCanvas;
        [SerializeField] private Image fader;
        [SerializeField] private GameObject loadingScreen;
        private float fadeDuration = 0.5f;
        private Dialog activeDialog;
        private Queue<Dialog> dialogQueue = new Queue<Dialog>();
        private bool isDoneDisplayingText = true;
        #endregion

        #region Alert Canvas
        [SerializeField] private GameObject messageCanvas;
        [SerializeField] private TMP_Text alertText;
        [SerializeField] private Transform buttonPanel;
        [SerializeField] private GameObject nextAlertArrow;
        #endregion

        #region Object Canvas
        [SerializeField] private GameObject objectCanvas;
        [SerializeField] private Transform objectButtonPanel;
        [SerializeField] private Image objectIcon;
        [SerializeField] private TMP_Text objectName;
        [SerializeField] private TMP_Text objectDescription;
        [SerializeField] private GameObject nextObjectArrow;
        #endregion

        #region Conversation Canvas
        [SerializeField] private GameObject conversationCanvas;
        [SerializeField] private GameObject optionCanvas;
        [SerializeField] private Transform convoOptionPanel;
        [SerializeField] private GameObject convoButtonPrefab;
        [SerializeField] private GameObject speakerPanel;
        [SerializeField] private GameObject speakerIconFrame;
        [SerializeField] private Image speakerIcon;
        [SerializeField] private TMP_Text speakerText;
        [SerializeField] private TMP_Text speechText;
        [SerializeField] private GameObject nextLineArrow;
        #endregion
        
        public void QueueAlert(string message, float delay = 0)
        {
            Dialog newDialog = new Dialog();
            newDialog.AddAlertOptions(message, delay);
            dialogQueue.Enqueue(newDialog);
        }

        public void QueueAlert(string message, string[] options, Response playerReply)
        {
            if (options.Length < 1) { return; }

            // Create new dialog object and enqueue it
            Dialog newDialog = new Dialog();
            newDialog.AddAlertOptions(message);
            newDialog.Options = options;
            newDialog.Reply = playerReply;
            dialogQueue.Enqueue(newDialog);
        }

        public void QueueObject(IObjectHeader inGameObject, float delay = 0)
        {
            Dialog newDialog = new Dialog();
            newDialog.AddObjectOptions(inGameObject, delay);
            dialogQueue.Enqueue(newDialog);
        }

        public void QueueObject(IObjectHeader inGameObject, string[] options, ObjectResponse playerReply)
        {
            if (options.Length < 1) { return; }

            Dialog newDialog = new Dialog();
            newDialog.AddObjectOptions(inGameObject);
            newDialog.Options = options;
            newDialog.ObjectReply = playerReply;
            dialogQueue.Enqueue(newDialog);
        }
        
        public void QueueConversation(CharacterSO speaker, List<string> speech, float speed = 0)
        {
            Dialog conversation = new Dialog();
            conversation.AddConversationOptions(speaker, speech, speed);
            dialogQueue.Enqueue(conversation);
        }
        
        public void QueueConversation(CharacterSO speaker, List<string> speech, string[] options, Response playerReply, float speed = 0)
        {
            Dialog conversation = new Dialog();
            conversation.AddConversationOptions(speaker, speech, speed);
            conversation.Options = options;
            conversation.Reply = playerReply;
            dialogQueue.Enqueue(conversation);
        }
        
        private void StartDialog()
        {
            activeDialog = dialogQueue.Dequeue();
            StartCoroutine("DisplayText");   
        }

        private IEnumerator DisplayText()
        {
            isDoneDisplayingText = false;
            
            if(activeDialog.Type is DialogType.CONVERSATION)
            {
                // Display line character by character
                speechText.text = "";
                conversationCanvas.SetActive(true);
                nextLineArrow.SetActive(false);
                
                if(activeDialog.Speaker is null) 
                {
                    speakerPanel.SetActive(false);
                    speakerIconFrame.SetActive(false);
                } else
                {
                    speakerPanel.SetActive(true);
                    speakerIconFrame.SetActive(true);
                    speakerText.text = activeDialog.Speaker.ObjectName;
                    speakerIcon.sprite = activeDialog.Speaker.Icon;
                }
                
                string line = activeDialog.GetNextLine();
                if (line is not null)
                {
                    foreach (char character in line.ToCharArray())
                    {
                        // if conversation has been requested to end, finish routine.
                        if (activeDialog == null) { StopCoroutine("DisplayText"); }
                        if (isDoneDisplayingText)
                        {
                            speechText.text = line;
                            break;
                        }

                        speechText.text += character.ToString();
                        yield return new WaitForSeconds(activeDialog.Delay);
                    }
                }
            } 
            else if (activeDialog.Type is DialogType.ALERT)
            {
                messageCanvas.SetActive(true);
                nextAlertArrow.SetActive(false);
                alertText.text = activeDialog.GetNextLine();
                yield return new WaitForSeconds(activeDialog.Delay);
            } 
            else if (activeDialog.Type is DialogType.OBJECT)
            {
                objectCanvas.SetActive(true);
                nextObjectArrow.SetActive(false);
                objectIcon.sprite = activeDialog.DisplayableObject.Icon;
                objectName.text = activeDialog.DisplayableObject.ObjectName;
                objectDescription.text = activeDialog.DisplayableObject.Description;
            }
            
            isDoneDisplayingText = true;
            DisplayOptions();
        }

        private void DisplayOptions()
        {
            if(activeDialog?.GetLineCount() == 0 && activeDialog?.Options is not null)
            {
                switch (activeDialog.Type)
                {
                    case DialogType.ALERT:
                        // Make space for the option buttons
                        alertText.gameObject.GetComponent<RectTransform>().localPosition += Vector3.up * 20.0f;
                        for (int i = 0; i < activeDialog.Options.Length; i++)
                        {
                            int index = i;
                            GameObject Button = GameObject.Instantiate(buttonPrefab, buttonPanel);
                            Button.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                activeDialog.SendReply(index);
                                alertText.gameObject.GetComponent<RectTransform>().localPosition -= Vector3.up * 20.0f;
                                EndDialog();
                            });
                            Button.transform.Find("OptionButtonText").GetComponent<TMP_Text>().text = activeDialog.Options[index];
                        }
                        break;
                    case DialogType.OBJECT:
                        for (int i = 0; i < activeDialog.Options.Length; i++)
                        {
                            int index = i;
                            GameObject Button = GameObject.Instantiate(buttonPrefab, objectButtonPanel);
                            Button.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                activeDialog.SendReply(index);
                                EndDialog();
                            });
                            Button.transform.Find("OptionButtonText").GetComponent<TMP_Text>().text = activeDialog.Options[index];
                        }
                        break;
                    case DialogType.CONVERSATION:
                        optionCanvas.SetActive(true);
                        for (int i = 0; i < activeDialog.Options.Length; i++)
                        {
                            int index = i;
                            GameObject Button = GameObject.Instantiate(convoButtonPrefab, convoOptionPanel);
                            Button.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                activeDialog.Reply(index);
                                EndDialog();
                            });
                            Button.transform.Find("OptionButtonText").GetComponent<TMP_Text>().text = activeDialog.Options[index];
                        }
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (activeDialog.Type)
                {
                    case DialogType.ALERT:
                        nextAlertArrow.SetActive(true);
                        break;
                    case DialogType.OBJECT:
                        nextObjectArrow.SetActive(true);
                        break;
                    case DialogType.CONVERSATION:
                        nextLineArrow.SetActive(true);
                        break;
                    default:
                        break;
                }
            }
        }

        private void EndDialog()
        {
            switch (activeDialog.Type)
            {
                case DialogType.ALERT:
                    foreach (Transform button in buttonPanel) { Destroy(button.gameObject); }
                    nextAlertArrow.SetActive(false);
                    messageCanvas.SetActive(false);
                    break;
                case DialogType.OBJECT:
                    foreach (Transform button in objectButtonPanel) { Destroy(button.gameObject); }
                    nextObjectArrow.SetActive(false);
                    objectCanvas.SetActive(false);
                    break;
                case DialogType.CONVERSATION:
                    foreach (Transform button in convoOptionPanel) { Destroy(button.gameObject); }
                    nextLineArrow.SetActive(false);
                    conversationCanvas.SetActive(false);
                    optionCanvas.SetActive(false);
                    break;
                default:
                    break;
            }

            // Ready for next dialog
            activeDialog = null;
        }

        public void StartLoading()
        {
            StartFade(false, 0);
            
            loadingScreen.SetActive(true);
            StartFade(true, fadeDuration);
        }

        public void FinishLoading()
        {
            StartCoroutine("TransitionOutLoadingScreen");
        }

        public IEnumerator TransitionOutLoadingScreen()
        {
            yield return new WaitForSeconds(2.0f);
            StartFade(false, fadeDuration);
            yield return new WaitForSeconds(fadeDuration);
            loadingScreen.SetActive(false);
            StartFade(true, fadeDuration);
        }

        public void StartFade(bool fadeIn, float duration)
        {
            if(fadeIn)
            {
                faderCanvas.SetActive(true);
                fader.color = new Color(0, 0, 0, 1);
                fader.CrossFadeAlpha(0, duration, false);
            } else
            {
                faderCanvas.SetActive(true);
                fader.color = new Color(0, 0, 0, 1);
                fader.CrossFadeAlpha(1, duration, false);
            }
        }

        public void Update()
        {
            // Check conversation queue
            if (activeDialog is null && dialogQueue.Count > 0) { StartDialog(); }

            // Request full line to be displayed or advance new line.
            if (activeDialog is not null && Input.GetKeyDown(KeyCode.E))
            {
                if (isDoneDisplayingText)
                {
                    if (activeDialog.GetLineCount() > 0) { StartCoroutine("DisplayText"); }
                    else if (activeDialog.Options != null) { return; }
                    else { EndDialog(); }
                }
                else
                {
                    isDoneDisplayingText = true;
                }
            }
        
        }
    }
}
