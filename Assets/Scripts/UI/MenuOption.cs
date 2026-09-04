using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace SilentDivide.UI
{
    /// <summary>
    /// Fila de Ajustes que recorre una lista de valores: «Pantalla completa · Sí / No»,
    /// «Calidad · Baja / Media / Alta». El nombre va a la izquierda y el valor a la derecha, con
    /// los signos «‹ ›» que aparecen solo cuando la fila tiene el foco.
    ///
    /// Mismo lenguaje visual que <see cref="MenuButton"/>: filete debajo, texto hueso, y el ámbar
    /// de las lámparas para el foco. La diferencia es que aquí Enter avanza al valor siguiente,
    /// igual que la flecha derecha: así la fila se puede usar solo con Enter si hace falta.
    /// </summary>
    public sealed class MenuOption : MenuEntry,
        IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI valueLabel;
        [SerializeField] private Image rule;
        [SerializeField] private Image highlight;
        [SerializeField] private Image hitArea;
        [SerializeField, Min(1f)] private float ruleThickness = 1.5f;

        private string[] values = Array.Empty<string>();
        private int index;
        private bool pressed;

        /// <summary>Avisa del índice elegido. Lo conecta la pantalla de Ajustes.</summary>
        public event Action<int> OnValueChanged;

        public int Index => index;

        /// <summary>Configura la fila. <paramref name="current"/> es el índice ya guardado.</summary>
        public void Configure(string name, string[] options, int current)
        {
            if (nameLabel != null) nameLabel.text = name;
            values = options ?? Array.Empty<string>();
            index  = values.Length == 0 ? 0 : Mathf.Clamp(current, 0, values.Length - 1);
            Repaint();
        }

        /// <summary>Mueve el valor sin volver a disparar el evento. Para refrescar desde fuera.</summary>
        public void SetIndexSilently(int value)
        {
            if (values.Length == 0) return;
            index = Mathf.Clamp(value, 0, values.Length - 1);
            Repaint();
        }

        public override void Adjust(int direction)
        {
            if (!interactable || values.Length == 0) return;

            // Da la vuelta en los dos extremos: son listas cortas y cerradas, y quedarse clavado
            // en el último valor se lee como que la fila dejó de responder.
            index = (index + direction + values.Length) % values.Length;
            Repaint();
            OnValueChanged?.Invoke(index);
        }

        /// <summary>Enter y clic avanzan; no hay «activar» distinto de cambiar el valor.</summary>
        public override void Activate() => Adjust(1);

        protected override void OnFocusLost() => pressed = false;

        public override void Repaint()
        {
            float e = FocusEased;

            Color text;
            Color highlightColor = UITheme.RuleHover;
            float thickness = ruleThickness;

            if (!interactable)   { text = UITheme.ButtonDisabled; e = 0f; }
            else if (pressed)    { text = UITheme.ButtonPressed; highlightColor = UITheme.RulePressed;
                                   thickness = ruleThickness * 2f; e = 1f; }
            else                 { text = Color.Lerp(UITheme.ButtonIdle, UITheme.ButtonHover, e); }

            if (nameLabel != null) nameLabel.color = text;

            if (valueLabel != null)
            {
                string value = values.Length == 0 ? "—" : values[index];
                // Las guías solo con el foco: en reposo la columna de valores queda limpia.
                valueLabel.text  = Focused && interactable ? $"‹  {value}  ›" : value;
                valueLabel.color = text;
            }

            PaintRule(rule, highlight, interactable, thickness, highlightColor, e);

            if (hitArea != null) hitArea.color = new Color(0f, 0f, 0f, 0f);
        }

        // ── Ratón ────────────────────────────────────────────────────────────────────────────

        public void OnPointerExit(PointerEventData e)  { if (interactable) { pressed = false; Repaint(); } }
        public void OnPointerDown(PointerEventData e)  { if (interactable) { pressed = true;  Repaint(); } }
        public void OnPointerUp(PointerEventData e)    { if (interactable) { pressed = false; Repaint(); } }

        public void OnPointerClick(PointerEventData e)
        {
            if (!interactable) return;
            // Clic en la mitad izquierda retrocede, en la derecha avanza: es lo que sugieren los
            // signos «‹ ›» que la fila muestra al enfocarse.
            RectTransform rect = (RectTransform)transform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rect, e.position, e.pressEventCamera, out Vector2 local);
            Adjust(local.x < rect.rect.center.x ? -1 : 1);
        }
    }
}
