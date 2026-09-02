using UnityEngine;
using UnityEngine.UI;
using SilentDivide.Suspicion;

namespace SilentDivide.UI
{
    /// <summary>
    /// Barra de sospecha. Lee el estado del <see cref="SuspicionSystem"/> cada fotograma; no
    /// contiene lógica de mecánica.
    /// </summary>
    public sealed class SuspicionBar : MonoBehaviour
    {
        [SerializeField] private SuspicionSystem suspicionSystem;
        [SerializeField] private Image fill;

        [Header("Color")]
        [SerializeField] private Color calmColor    = new Color(0.51f, 0.78f, 0.92f);
        [SerializeField] private Color alertColor   = new Color(0.85f, 0.72f, 0.42f);
        [SerializeField] private Color detectedColor = new Color(0.80f, 0.25f, 0.22f);

        private void Awake()
        {
            if (suspicionSystem == null)
                suspicionSystem = FindFirstObjectByType<SuspicionSystem>();
        }

        private void Update()
        {
            if (suspicionSystem == null || fill == null) return;

            float t = suspicionSystem.NormalizedSuspicion;
            fill.fillAmount = t;
            fill.color = suspicionSystem.Detected
                ? detectedColor
                : Color.Lerp(calmColor, alertColor, t);
        }
    }
}
