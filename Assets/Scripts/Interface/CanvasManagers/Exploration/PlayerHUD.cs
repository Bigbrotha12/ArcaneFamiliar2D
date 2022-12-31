namespace UserInterface
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private Image playerAvatar;
        [SerializeField] private TMP_Text playerLevel;
        [SerializeField] private Image playerLevelProgress;

        [SerializeField] private Image currentHPBar;
        [SerializeField] private TMP_Text currentHPValue;
        [SerializeField] private Image currentMPBar;
        [SerializeField] private TMP_Text currentMPValue;

        [SerializeField] private Image mainAvatar;
        [SerializeField] private Image supportAvatar;

        [SerializeField] private Image miniMap;

        private bool menuToggle = false;
        [SerializeField] private UICanvasBase menuCanvas;
        

        public void Start()
        {
            RefreshHUD();
        }

        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                Debug.Log("Activating menu...");
                menuToggle = !menuToggle;
                menuCanvas.ActivateCanvas(menuToggle);
                RefreshHUD();
            }
        }  

        public void RefreshHUD()
        {
            // Status bars
            playerAvatar.sprite = GameManager.Instance.Player.Icon;
            playerLevel.text = GameManager.Instance.Player.Level.PlayerLevel.ToString();
            playerLevelProgress.fillAmount = (float) GameManager.Instance.Player.Level.LevelProgress;
            currentHPBar.fillAmount = GameManager.Instance.Player.Health/ GameManager.Instance.Player.HP;
            currentHPValue.text = GameManager.Instance.Player.Health.ToString() + " / " + GameManager.Instance.Player.HP.ToString();
            currentMPBar.fillAmount = GameManager.Instance.Player.Mana / GameManager.Instance.Player.MP;
            currentMPValue.text = GameManager.Instance.Player.Mana.ToString() + " / " + GameManager.Instance.Player.MP.ToString();

            // Familiar section
            mainAvatar.sprite = GameManager.Instance.Player.Familiars.MainFamiliar.Icon;
            supportAvatar.sprite = GameManager.Instance.Player.Familiars.SupportFamiliar.Icon;

            // Minimap
            // TODO
        }
    }
}
