using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SpellCardDev.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject mainPanel;
        [SerializeField] private GameObject helpPanel;
        [SerializeField] private GameObject configPanel;

        [SerializeField] private RectTransform cursorIndicator;

        [SerializeField] private string gameSceneName = "Game";
        [SerializeField] private string extraStageSceneName = "Game";

        [SerializeField] private Color itemDefaultColor = new Color(0.85f, 0.78f, 0.45f);
        [SerializeField] private Color itemSelectedColor = new Color(1f, 0.25f, 0.25f);

        [SerializeField] private float cursorMoveSpeed = 12f;

        private int _selectedIndex = 0;
        private bool _inSubPanel = false;
        private RectTransform[] _menuItems;
        private Text[] _menuTexts;
        private readonly Vector3[] _itemCorners = new Vector3[4];

        private void Start()
        {
            AutoWireButtons();
            BuildMenuItemList();
            ShowMainPanel();

            float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            AudioListener.volume = savedVolume;

            Slider volSlider = FindChildByName("VolumeSlider")?.GetComponent<Slider>();
            if (volSlider != null)
            {
                volSlider.value = savedVolume;
                volSlider.onValueChanged.AddListener(OnVolumeChanged);
            }

            Toggle fsToggle = FindChildByName("FullscreenToggle")?.GetComponent<Toggle>();
            if (fsToggle != null)
            {
                fsToggle.isOn = Screen.fullScreen;
                fsToggle.onValueChanged.AddListener(OnFullscreenToggled);
            }

            if (cursorIndicator != null)
            {
                Image cursorImg = cursorIndicator.GetComponent<Image>();
                if (cursorImg != null)
                    cursorImg.raycastTarget = false;

                Vector3 initialTarget = GetCursorTargetPosition();
                if (initialTarget != Vector3.zero)
                    cursorIndicator.position = initialTarget;
            }

            UpdateVisualSelection();
        }

        private void AutoWireButtons()
        {
            WireButton("PLAY", OnPlayClicked);
            WireButton("EXTRA STAGE", OnExtraStageClicked);
            WireButton("OPTIONS", OnOptionsClicked);
            WireButton("EXIT", OnExitClicked);
            WireButton("BACKButton", OnBackFromHelp);
            WireButton("BACKButton", OnBackFromConfig);
        }

        private void WireButton(string goName, UnityEngine.Events.UnityAction action)
        {
            GameObject go = FindChildByName(goName);
            if (go == null) return;
            Button btn = go.GetComponent<Button>();
            if (btn == null) return;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);

            var trigger = go.GetComponent<UnityEngine.EventSystems.EventTrigger>();
            if (trigger == null) trigger = go.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            var entry = new UnityEngine.EventSystems.EventTrigger.Entry();
            entry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            int idx = System.Array.IndexOf(new string[] { "PLAY", "EXTRA STAGE", "OPTIONS", "EXIT" }, goName);
            if (idx >= 0)
            {
                int captured = idx;
                entry.callback.AddListener(_ =>
                {
                    _selectedIndex = captured;
                    UpdateVisualSelection();
                });
                trigger.triggers.Add(entry);
            }
        }

        private void BuildMenuItemList()
        {
            string[] names = { "PLAY", "EXTRA STAGE", "OPTIONS", "EXIT" };
            _menuItems = new RectTransform[names.Length];
            _menuTexts = new Text[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                GameObject go = FindChildByName(names[i]);
                if (go != null)
                {
                    _menuItems[i] = go.GetComponent<RectTransform>();
                    _menuTexts[i] = go.GetComponent<Text>();
                    if (_menuTexts[i] == null)
                        _menuTexts[i] = go.GetComponentInChildren<Text>();
                }
            }
        }

        private void Update()
        {
            if (_inSubPanel) return;
            if (_menuItems == null || _menuItems.Length == 0) return;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _selectedIndex = (_selectedIndex - 1 + _menuItems.Length) % _menuItems.Length;
                UpdateVisualSelection();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _selectedIndex = (_selectedIndex + 1) % _menuItems.Length;
                UpdateVisualSelection();
            }
            else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            {
                ExecuteSelected();
            }
            else if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape))
            {
                OnExitClicked();
            }

            if (cursorIndicator != null)
            {
                Vector3 target = GetCursorTargetPosition();
                if (target != Vector3.zero)
                {
                    cursorIndicator.position = Vector3.Lerp(cursorIndicator.position, target, Time.deltaTime * cursorMoveSpeed);
                }
            }
        }

        private Vector3 GetCursorTargetPosition()
        {
            if (cursorIndicator == null || _menuItems == null || _selectedIndex < 0 || _selectedIndex >= _menuItems.Length || _menuItems[_selectedIndex] == null)
                return Vector3.zero;

            _menuItems[_selectedIndex].GetWorldCorners(_itemCorners);
            // Center vertically with the button and place to the left
            Vector3 leftCenter = (_itemCorners[0] + _itemCorners[1]) * 0.5f;
            float offset = (cursorIndicator.rect.width * 0.5f) + 16f;
            return leftCenter + Vector3.left * offset;
        }

        private void UpdateVisualSelection()
        {
            if (_menuTexts == null) return;
            for (int i = 0; i < _menuTexts.Length; i++)
            {
                if (_menuTexts[i] != null)
                    _menuTexts[i].color = (i == _selectedIndex) ? itemSelectedColor : itemDefaultColor;
            }
        }

        private void ExecuteSelected()
        {
            switch (_selectedIndex)
            {
                case 0: OnPlayClicked(); break;
                case 1: OnExtraStageClicked(); break;
                case 2: OnOptionsClicked(); break;
                case 3: OnExitClicked(); break;
            }
        }

        public void OnPlayClicked() => SceneManager.LoadScene(gameSceneName);

        public void OnExtraStageClicked() => SceneManager.LoadScene(extraStageSceneName);

        public void OnOptionsClicked()
        {
            _inSubPanel = true;
            mainPanel.SetActive(false);
            configPanel.SetActive(true);
        }

        public void OnExitClicked() => Application.Quit();

        public void OnBackFromHelp()
        {
            _inSubPanel = false;
            ShowMainPanel();
        }

        public void OnBackFromConfig()
        {
            _inSubPanel = false;
            PlayerPrefs.SetFloat("MasterVolume", AudioListener.volume);
            PlayerPrefs.Save();
            ShowMainPanel();
        }

        public void OnVolumeChanged(float value) => AudioListener.volume = value;

        public void OnFullscreenToggled(bool value) => Screen.fullScreen = value;

        private void ShowMainPanel()
        {
            if (mainPanel != null) mainPanel.SetActive(true);
            if (helpPanel != null) helpPanel.SetActive(false);
            if (configPanel != null) configPanel.SetActive(false);
        }

        private GameObject FindChildByName(string name)
        {
            Transform[] all = GetComponentsInParent<Transform>(true);
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) return null;
            Transform[] allChildren = canvas.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in allChildren)
            {
                if (t.gameObject.name == name)
                    return t.gameObject;
            }
            return null;
        }
    }
}
