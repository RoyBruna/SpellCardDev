using UnityEngine;

namespace SpellCardDev.Background
{
    public class BackgroundVideoPlayer : MonoBehaviour
    {
        [SerializeField] private Color backgroundColor = new Color(0.05f, 0.06f, 0.1f, 1f);

        private void Awake()
        {
            ApplyBackgroundColor();
        }

        private void OnValidate()
        {
            ApplyBackgroundColor();
        }

        private void ApplyBackgroundColor()
        {
            Camera cam = Camera.main;
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = backgroundColor;
            }
        }
    }
}
