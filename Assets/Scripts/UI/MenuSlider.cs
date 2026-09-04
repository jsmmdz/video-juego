using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace SilentDivide.UI
{
    /// <summary>
    /// Fila de Ajustes con un valor continuo: los tres volúmenes. Nombre a la izquierda, barra y
    /// porcentaje a la derecha.
    ///
    /// La barra se dibuja con dos rectángulos —canal y relleno—, no con el <c>Slider</c> de Unity:
    /// aquí no hay que arrastrar un tirador, solo mover el valor con las flechas, y así comparte
    /// exactamente el mismo lenguaje visual que <see cref="MenuOption"/>.
    /// </summary>
    public sealed class MenuSlider : MenuEntry,
        IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI nameLabel;
        [SerializeField] private TextMeshProUGUI valueLabel;
        [SerializeField] private Image track;
        [SerializeField] private Image fill;
        [SerializeField] private Image rule;
        [SerializeField] private Image hitArea;
        [SerializeField, Min(1f)] private float ruleThickness = 1.5f;

        [Tooltip("Cuánto se mueve con cada pulsación de flecha. 0,05 = 5 %.")]
        [SerializeField, Range(0.01f, 0.5f)] private float step = 0.05f;

        private float value01;
        private bool pressed;

        /// <summary>Valor entre 0 y 1. Lo conecta la pantalla de Ajustes.</summary>
        public event Action<float> OnValueChanged;

        public float Value => value01;

        public void Configure(string name, float current)
        {
            if (nameLabel != null) nameLabel.text = name;
            value01 = Mathf.Clamp01(current);
            Repaint();
        }

        public override void Adjust(int direction)
        {
            if (!interactable) return;

            // Satura en los extremos en vez de dar la vuelta: pasar de silencio a volumen máximo
            // de un golpe sería un susto, no un ajuste.
            float next = Mathf.Clamp01(value01 + direction * step);
            if (Mathf.Approximately(next, value01)) return;

            value01 = next;
            Repaint();
            OnValueChanged?.Invoke(value01);
        }

        /// <summary>Enter no hace nada aquí: el valor solo se mueve con las flechas o el ratón.</summary>
        public override void Activate() { }

        protected override void OnFocusLost() => pressed = false;

        public override void Repaint()
        {
            Color text, ruleColor, fillColor;
            float thickness = ruleThickness;

            if (!interactable)
            {
                text = UITheme.ButtonDisabled; ruleColor = UITheme.RuleDisabled;
                fillColor = UITheme.ButtonDisabled;
            }
            else if (pressed)
            {
                text = UITheme.ButtonPressed;  ruleColor = UITheme.RulePressed;
                fillColor = UITheme.RulePressed; thickness = ruleThickness * 2f;
            }
            else if (Focused)
            {
                text = UITheme.ButtonHover;    ruleColor = UITheme.RuleHover;
                fillColor = UITheme.RuleHover;
            }
            else
            {
                text = UITheme.ButtonIdle;     ruleColor = UITheme.RuleIdle;
                fillColor = UITheme.RuleIdle;
            }

            if (nameLabel  != null) nameLabel.color = text;
            if (valueLabel != null)
            {
                valueLabel.text  = Mathf.RoundToInt(value01 * 100f) + " %";
                valueLabel.color = text;
            }

            if (track != null) track.color = UITheme.TextMuted;
            if (fill  != null)
            {
                fill.color = fillColor;
                // El relleno se ancla a la izquierda del canal y crece con el valor.
                fill.rectTransform.anchorMin = new Vector2(0f, 0f);
                fill.rectTransform.anchorMax = new Vector2(value01, 1f);
                fill.rectTransform.offsetMin = Vector2.zero;
                fill.rectTransform.offsetMax = Vector2.zero;
            }

            if (rule != null)
            {
                rule.color = ruleColor;
                Vector2 size = rule.rectTransform.sizeDelta;
                rule.rectTransform.sizeDelta = new Vector2(size.x, thickness);
            }

            if (hitArea != null) hitArea.color = new Color(0f, 0f, 0f, 0f);
        }

        // ── Ratón ────────────────────────────────────────────────────────────────────────────

        public void OnPointerExit(PointerEventData e)  { if (interactable) { pressed = false; Repaint(); } }
        public void OnPointerDown(PointerEventData e)  { if (interactable) { pressed = true;  Repaint(); } }
        public void OnPointerUp(PointerEventData e)    { if (interactable) { pressed = false; Repaint(); } }

        /// <summary>Clic sobre el canal: salta al punto pulsado.</summary>
        public void OnPointerClick(PointerEventData e)
        {
            if (!interactable || track == null) return;

            RectTransform rect = track.rectTransform;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    rect, e.position, e.pressEventCamera, out Vector2 local))
                return;

            // Fuera del canal el clic no mueve nada: es una fila ancha y la mayor parte es texto.
            if (!rect.rect.Contains(local)) return;

            value01 = Mathf.Clamp01((local.x - rect.rect.xMin) / rect.rect.width);
            Repaint();
            OnValueChanged?.Invoke(value01);
        }
    }
}
