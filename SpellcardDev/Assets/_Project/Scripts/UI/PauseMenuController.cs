using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SpellCardDev.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private bool _isPaused = false;

        private void Start()
        {
            AutoWireButtons();
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }

        private void AutoWireButtons()
        {
            if (pausePanel == null) return;
            Button[] buttons = pausePanel.GetComponentsInChildren<Button>(true);
            foreach (Button btn in buttons)
            {
                string name = btn.gameObject.name;
                btn.onClick.RemoveAllListeners();
                if (name == "RESUME") btn.onClick.AddListener(Resume);
                else if (name == "RETRY") btn.onClick.AddListener(Retry);
                else if (name == "QUIT TO MENU") btn.onClick.AddListener(QuitToMenu);
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_isPaused) Resume();
                else Pause();
            }
        }

        public void Pause()
        {
            _isPaused = true;
            Time.timeScale = 0f;
            if (pausePanel != null)
                pausePanel.SetActive(true);
        }

        public void Resume()
        {
            _isPaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null)
                pausePanel.SetActive(false);
        }

        public void Retry()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void QuitToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}
