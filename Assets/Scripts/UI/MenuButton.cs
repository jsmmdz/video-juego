using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace SilentDivide.UI
{
    /// <summary>
    /// Opción del menú de inicio: un rótulo con un filete corto debajo, como en el mockup
    /// definitivo. No lleva caja ni relleno — el fondo es la ilustración y encerrar el texto en un
    /// recuadro la taparía.
    ///
    /// Los cuatro estados del kit se resuelven con dos cosas: el color del texto y el filete. En
    /// reposo el filete es gris tenue; con el foco se enciende en el ámbar de las lámparas y el
    /// texto sube a blanco; al pulsar, el filete además engorda. Deshabilitado apaga los dos.
    ///
    /// La navegación y el foco compartido entre ratón y teclado están en <see cref="MenuEntry"/>.
    /// </summary>
    public sealed class MenuButton : MenuEntry,
        IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI label;

        [Tooltip("Filete bajo el rótulo. Es lo que marca el estado.")]
        [SerializeField] private Image rule;

        [Tooltip("Gráfico transparente que recibe los eventos de ratón en toda la fila.")]
        [SerializeField] private Image hitArea;

        [Tooltip("Grosor del filete en reposo. Al pulsar se multiplica por 2.")]
        [SerializeField, Min(1f)] private float ruleThickness = 1.5f;

        private bool pressed;

        private void Awake()
        {
            if (label == null) label = GetComponentInChildren<TextMeshProUGUI>();
        }

        /// <summary>Texto visible. Lo usa «Volver» y cualquier opción que cambie de rótulo.</summary>
        public string Text
        {
            get => label != null ? label.text : string.Empty;
            set { if (label != null) label.text = value; }
        }

        protected override void OnFocusLost() => pressed = false;

        public override void Repaint()
        {
            Color text, ruleColor;
            float thickness = ruleThickness;

            if (!interactable)
            {
                text      = UITheme.ButtonDisabled;
                ruleColor = UITheme.RuleDisabled;
            }
            else if (pressed)
            {
                text      = UITheme.ButtonPressed;
                ruleColor = UITheme.RulePressed;
                thickness = ruleThickness * 2f;
            }
            else if (Focused)
            {
                text      = UITheme.ButtonHover;
                ruleColor = UITheme.RuleHover;
            }
            else
            {
                text      = UITheme.ButtonIdle;
                ruleColor = UITheme.RuleIdle;
            }

            if (label != null) label.color = text;

            if (rule != null)
            {
                rule.color = ruleColor;
                Vector2 size = rule.rectTransform.sizeDelta;
                rule.rectTransform.sizeDelta = new Vector2(size.x, thickness);
            }

            // Invisible, pero tiene que seguir recibiendo el ratón.
            if (hitArea != null) hitArea.color = new Color(0f, 0f, 0f, 0f);
        }

        // ── Ratón ────────────────────────────────────────────────────────────────────────────

        public void OnPointerExit(PointerEventData e)
        {
            if (!interactable) return;
            pressed = false;
            Repaint();
        }

        public void OnPointerDown(PointerEventData e)
        {
            if (!interactable) return;
            pressed = true;
            Repaint();
        }

        public void OnPointerUp(PointerEventData e)
        {
            if (!interactable) return;
            pressed = false;
            Repaint();
        }

        public void OnPointerClick(PointerEventData e)
        {
            if (!interactable) return;
            RaiseActivated();
        }
    }
}
