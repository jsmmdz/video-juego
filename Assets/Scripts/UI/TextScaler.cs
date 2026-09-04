using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SilentDivide.UI
{
    /// <summary>
    /// Aplica el tamaño de texto de accesibilidad a todas las etiquetas de un canvas.
    ///
    /// Guarda el tamaño original de cada etiqueta la primera vez y siempre multiplica sobre ese
    /// valor. Si multiplicara sobre el tamaño actual, entrar y salir de Ajustes varias veces iría
    /// agrandando el texto sin parar.
    /// </summary>
    public sealed class TextScaler : MonoBehaviour
    {
        private readonly Dictionary<TextMeshProUGUI, float> baseSizes =
            new Dictionary<TextMeshProUGUI, float>();

        private void OnEnable()
        {
            GameSettings.Changed += Apply;
            Apply();
        }

        private void OnDisable() => GameSettings.Changed -= Apply;

        /// <summary>Hay que llamarlo si se crean etiquetas nuevas después de arrancar.</summary>
        public void Apply()
        {
            float scale = GameSettings.TextScale;

            // includeInactive: la pantalla de Ajustes empieza oculta y también tiene que escalarse.
            foreach (TextMeshProUGUI label in GetComponentsInChildren<TextMeshProUGUI>(true))
            {
                if (!baseSizes.TryGetValue(label, out float baseSize))
                {
                    baseSize = label.fontSize;
                    baseSizes[label] = baseSize;
                }

                label.fontSize = baseSize * scale;
            }
        }
    }
}
