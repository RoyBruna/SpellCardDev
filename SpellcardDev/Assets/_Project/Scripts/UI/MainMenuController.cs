using System.Collections;
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
        [SerializeField] private RectTransform[] menuItems;

        [SerializeField] private Slider volumeSlider;
        [SerializeField] private Toggle fullscreenToggle;

        [SerializeField] private string gameSceneName = "Game";
        [SerializeField] private string extraStageSceneName = "Game";

        [SerializeField] private Color itemDefaultColor = new Color(0.85f, 0.78f, 0.45f);
        [SerializeField] private Color itemSelectedColor = new Color(1f, 0.25f, 0.25f);

        [SerializeField] private float cursorMoveSpeed = 12f;

        private int _selectedIndex = 0;
        private bool _inSubPanel = false;
        private Text[] _menuTexts;

        private void Start()
        {
            ShowMainPanel();
            AudioListener.volume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            if (volumeSlider != null)
                volumeSlider.value = AudioListener.volume;
            if (fullscreenToggle != null)
                fullscreenToggle.isOn = Screen.fullScreen;

            _menuTexts = new Text[menuItems.Length];
            for (int i = 0; i < menuItems.Length; i++)
            {
                _menuTexts[i] = menuItems[i].GetComponentInChildren<Text>();
            }

            UpdateVisualSelection();
        }

        private void Update()
        {
            if (_inSubPanel) return;

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                _selectedIndex = (_selectedIndex - 1 + menuItems.Length) % menuItems.Length;
                UpdateVisualSelection();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                _selectedIndex = (_selectedIndex + 1) % menuItems.Length;
                UpdateVisualSelection();
            }
            else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
            {
                ExecuteSelected();
            }

            if (cursorIndicator != null && menuItems.Length > 0)
            {
                Vector3 target = menuItems[_selectedIndex].position + Vector3.left * 60f;
                cursorIndicator.position = Vector3.Lerp(cursorIndicator.position, target, Time.deltaTime * cursorMoveSpeed);
            }
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

        public void OnPlayClicked()
        {
            SceneManager.LoadScene(gameSceneName);
        }

        public void OnExtraStageClicked()
        {
            SceneManager.LoadScene(extraStageSceneName);
        }

        public void OnOptionsClicked()
        {
            _inSubPanel = true;
            mainPanel.SetActive(false);
            configPanel.SetActive(true);
        }

        public void OnHelpClicked()
        {
            _inSubPanel = true;
            mainPanel.SetActive(false);
            helpPanel.SetActive(true);
        }

        public void OnExitClicked()
        {
            Application.Quit();
        }

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

        public void OnVolumeChanged(float value)
        {
            AudioListener.volume = value;
        }

        public void OnFullscreenToggled(bool value)
        {
            Screen.fullScreen = value;
        }

        private void ShowMainPanel()
        {
            mainPanel.SetActive(true);
            helpPanel.SetActive(false);
            configPanel.SetActive(false);
        }
    }
}
