using UnityEngine;

namespace SpellCardDev.EditorUtils
{
    public class HitboxGizmo : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private float hitboxRadius = 0.06f;
        [SerializeField] private Color gizmoColor = new Color(1f, 0.1f, 0.1f, 0.9f);
        [SerializeField] private Color gizmoSelectedColor = new Color(1f, 0.4f, 0.0f, 1.0f);

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, hitboxRadius);

            float crossSize = hitboxRadius * 0.5f;
            Gizmos.DrawLine(
                transform.position + Vector3.left * crossSize,
                transform.position + Vector3.right * crossSize);
            Gizmos.DrawLine(
                transform.position + Vector3.down * crossSize,
                transform.position + Vector3.up * crossSize);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = gizmoSelectedColor;
            Gizmos.DrawWireSphere(transform.position, hitboxRadius * 1.3f);
        }
#endif
    }
}
