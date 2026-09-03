using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SpellCardDev.UI;

namespace SpellCardDev.Editor
{
    public class MainMenuSceneSetup : EditorWindow
    {
        [MenuItem("SpellCardDev/Setup Main Menu Scene")]
        public static void CreateMainMenuScene()
        {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            Camera cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 8f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.05f, 0.06f, 0.1f, 1f);
            cam.allowHDR = true;
            camGo.AddComponent<AudioListener>();
            camGo.transform.position = new Vector3(0, 0, -10f);

            GameObject canvasGo = new GameObject("Canvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 960f);
            scaler.matchWidthOrHeight = 0.5f;
            canvasGo.AddComponent<GraphicRaycaster>();

            GameObject eventSystemGo = new GameObject("EventSystem");
            eventSystemGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            GameObject bgGo = new GameObject("Background");
            bgGo.transform.SetParent(canvasGo.transform, false);
            RawImage bgImage = bgGo.AddComponent<RawImage>();
            bgImage.color = Color.white;
            RectTransform bgRect = bgGo.GetComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;

            GameObject overlayGo = new GameObject("DarkOverlay");
            overlayGo.transform.SetParent(canvasGo.transform, false);
            Image overlay = overlayGo.AddComponent<Image>();
            overlay.color = new Color(0f, 0f, 0f, 0.35f);
            overlay.raycastTarget = false;
            RectTransform overlayRect = overlayGo.GetComponent<RectTransform>();
            overlayRect.anchorMin = Vector2.zero;
            overlayRect.anchorMax = Vector2.one;
            overlayRect.offsetMin = Vector2.zero;
            overlayRect.offsetMax = Vector2.zero;

            GameObject mainPanel = CreateMainPanel(canvasGo.transform);
            GameObject helpPanel = CreateHelpPanel(canvasGo.transform);
            GameObject configPanel = CreateConfigPanel(canvasGo.transform);

            helpPanel.SetActive(false);
            configPanel.SetActive(false);

            GameObject controllerGo = new GameObject("MainMenuController");
            controllerGo.transform.SetParent(canvasGo.transform, false);
            SpellCardDev.UI.MainMenuController controller = controllerGo.AddComponent<SpellCardDev.UI.MainMenuController>();

            SerializedObject so = new SerializedObject(controller);
            so.FindProperty("mainPanel").objectReferenceValue = mainPanel;
            so.FindProperty("helpPanel").objectReferenceValue = helpPanel;
            so.FindProperty("configPanel").objectReferenceValue = configPanel;
            so.ApplyModifiedProperties();

            string scenePath = "Assets/_Project/Scenes/MainMenu.unity";
            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("[SpellCardDev] MainMenu scene created at: " + scenePath);
        }

        private static GameObject CreateMainPanel(Transform parent)
        {
            GameObject panel = new GameObject("MainPanel");
            panel.transform.SetParent(parent, false);
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            string[] labels = { "PLAY", "EXTRA STAGE", "OPTIONS", "EXIT" };
            for (int i = 0; i < labels.Length; i++)
            {
                GameObject itemGo = new GameObject(labels[i]);
                itemGo.transform.SetParent(panel.transform, false);
                RectTransform itemRect = itemGo.AddComponent<RectTransform>();
                itemRect.anchorMin = new Vector2(0f, 1f);
                itemRect.anchorMax = new Vector2(0f, 1f);
                itemRect.pivot = new Vector2(0f, 1f);
                itemRect.anchoredPosition = new Vector2(120f, -240f - i * 70f);
                itemRect.sizeDelta = new Vector2(400f, 60f);

                Text txt = itemGo.AddComponent<Text>();
                txt.text = labels[i];
                txt.fontSize = 42;
                txt.fontStyle = FontStyle.Bold;
                txt.color = new Color(0.85f, 0.78f, 0.45f);
                txt.alignment = TextAnchor.MiddleLeft;

                Shadow shadow = itemGo.AddComponent<Shadow>();
                shadow.effectColor = new Color(0f, 0f, 0f, 0.9f);
                shadow.effectDistance = new Vector2(3f, -3f);

                Outline outline = itemGo.AddComponent<Outline>();
                outline.effectColor = new Color(0f, 0f, 0f, 0.8f);
                outline.effectDistance = new Vector2(2f, -2f);

                Button btn = itemGo.AddComponent<Button>();
                ColorBlock colors = btn.colors;
                colors.normalColor = Color.white;
                colors.highlightedColor = new Color(1f, 0.9f, 0.5f);
                colors.pressedColor = new Color(1f, 0.4f, 0.4f);
                btn.colors = colors;
                btn.targetGraphic = txt;
            }

            return panel;
        }

        private static GameObject CreateHelpPanel(Transform parent)
        {
            GameObject panel = new GameObject("HelpPanel");
            panel.transform.SetParent(parent, false);
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.1f, 0.05f);
            rt.anchorMax = new Vector2(0.9f, 0.95f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.02f, 0.02f, 0.08f, 0.88f);

            GameObject titleGo = new GameObject("Title");
            titleGo.transform.SetParent(panel.transform, false);
            RectTransform titleRect = titleGo.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0.85f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(20f, 0f);
            titleRect.offsetMax = new Vector2(-20f, 0f);
            Text titleTxt = titleGo.AddComponent<Text>();
            titleTxt.text = "HOW TO PLAY";
            titleTxt.fontSize = 36;
            titleTxt.fontStyle = FontStyle.Bold;
            titleTxt.color = new Color(1f, 0.85f, 0.3f);
            titleTxt.alignment = TextAnchor.MiddleCenter;

            string helpContent =
                "MOVEMENT         Arrow Keys\n" +
                "FOCUS (Slow)     Left Shift   -   Reveals collision point\n" +
                "SHOOT            Z\n" +
                "CHARGED SHOT     X (Hold & Release)\n" +
                "BOMB             C   -   Sweeping ring wave, clears bullets\n\n" +
                "You have 3 lives. Touching an enemy bullet costs one life.\n" +
                "Your real hitbox is a tiny central dot — use Focus to see it.\n" +
                "Graze enemy bullets to feel alive.";

            GameObject contentGo = new GameObject("Content");
            contentGo.transform.SetParent(panel.transform, false);
            RectTransform contentRect = contentGo.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 0.2f);
            contentRect.anchorMax = new Vector2(1f, 0.85f);
            contentRect.offsetMin = new Vector2(40f, 0f);
            contentRect.offsetMax = new Vector2(-40f, 0f);
            Text contentTxt = contentGo.AddComponent<Text>();
            contentTxt.text = helpContent;
            contentTxt.fontSize = 24;
            contentTxt.color = new Color(0.9f, 0.9f, 0.9f);
            contentTxt.alignment = TextAnchor.UpperLeft;
            contentTxt.lineSpacing = 1.4f;

            CreateTextButton("BACK", panel.transform,
                new Vector2(0.35f, 0.02f), new Vector2(0.65f, 0.15f),
                new Color(0.85f, 0.78f, 0.45f), 28);

            return panel;
        }

        private static GameObject CreateConfigPanel(Transform parent)
        {
            GameObject panel = new GameObject("ConfigPanel");
            panel.transform.SetParent(parent, false);
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.2f, 0.15f);
            rt.anchorMax = new Vector2(0.8f, 0.85f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Image bg = panel.AddComponent<Image>();
            bg.color = new Color(0.02f, 0.02f, 0.08f, 0.88f);

            GameObject titleGo = new GameObject("Title");
            titleGo.transform.SetParent(panel.transform, false);
            RectTransform titleRect = titleGo.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 0.8f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.offsetMin = new Vector2(20f, 0f);
            titleRect.offsetMax = new Vector2(-20f, 0f);
            Text titleTxt = titleGo.AddComponent<Text>();
            titleTxt.text = "OPTIONS";
            titleTxt.fontSize = 36;
            titleTxt.fontStyle = FontStyle.Bold;
            titleTxt.color = new Color(1f, 0.85f, 0.3f);
            titleTxt.alignment = TextAnchor.MiddleCenter;

            GameObject volLabelGo = new GameObject("VolumeLabel");
            volLabelGo.transform.SetParent(panel.transform, false);
            RectTransform volLabelRect = volLabelGo.AddComponent<RectTransform>();
            volLabelRect.anchorMin = new Vector2(0.05f, 0.6f);
            volLabelRect.anchorMax = new Vector2(0.45f, 0.75f);
            volLabelRect.offsetMin = Vector2.zero;
            volLabelRect.offsetMax = Vector2.zero;
            Text volLabel = volLabelGo.AddComponent<Text>();
            volLabel.text = "VOLUME";
            volLabel.fontSize = 26;
            volLabel.color = new Color(0.9f, 0.9f, 0.9f);
            volLabel.alignment = TextAnchor.MiddleLeft;

            GameObject sliderGo = new GameObject("VolumeSlider");
            sliderGo.transform.SetParent(panel.transform, false);
            RectTransform sliderRect = sliderGo.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.45f, 0.6f);
            sliderRect.anchorMax = new Vector2(0.95f, 0.75f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            Slider slider = sliderGo.AddComponent<Slider>();
            slider.minValue = 0f;
            slider.maxValue = 1f;
            slider.value = 1f;

            GameObject fsLabelGo = new GameObject("FullscreenLabel");
            fsLabelGo.transform.SetParent(panel.transform, false);
            RectTransform fsLabelRect = fsLabelGo.AddComponent<RectTransform>();
            fsLabelRect.anchorMin = new Vector2(0.05f, 0.42f);
            fsLabelRect.anchorMax = new Vector2(0.7f, 0.57f);
            fsLabelRect.offsetMin = Vector2.zero;
            fsLabelRect.offsetMax = Vector2.zero;
            Text fsLabel = fsLabelGo.AddComponent<Text>();
            fsLabel.text = "FULLSCREEN";
            fsLabel.fontSize = 26;
            fsLabel.color = new Color(0.9f, 0.9f, 0.9f);
            fsLabel.alignment = TextAnchor.MiddleLeft;

            GameObject toggleGo = new GameObject("FullscreenToggle");
            toggleGo.transform.SetParent(panel.transform, false);
            RectTransform toggleRect = toggleGo.AddComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0.72f, 0.42f);
            toggleRect.anchorMax = new Vector2(0.95f, 0.57f);
            toggleRect.offsetMin = Vector2.zero;
            toggleRect.offsetMax = Vector2.zero;
            Toggle toggle = toggleGo.AddComponent<Toggle>();

            CreateTextButton("BACK", panel.transform,
                new Vector2(0.25f, 0.04f), new Vector2(0.75f, 0.17f),
                new Color(0.85f, 0.78f, 0.45f), 28);

            return panel;
        }

        private static GameObject CreateTextButton(string label, Transform parent,
            Vector2 anchorMin, Vector2 anchorMax, Color textColor, int fontSize)
        {
            GameObject go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);
            RectTransform rt = go.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            Text txt = go.AddComponent<Text>();
            txt.text = label;
            txt.fontSize = fontSize;
            txt.fontStyle = FontStyle.Bold;
            txt.color = textColor;
            txt.alignment = TextAnchor.MiddleCenter;

            Shadow shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.9f);
            shadow.effectDistance = new Vector2(2f, -2f);

            go.AddComponent<Button>();
            return go;
        }
    }
}
