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
    /// El realce **no conmuta, se anima**: un segundo filete ámbar recorre el gris de izquierda a
    /// derecha, y el rótulo se desplaza unos píxeles a la vez que aclara. El filete de reposo no se
    /// toca, así que la pantalla en reposo sigue siendo exactamente la del mockup.
    ///
    /// La navegación y el foco compartido entre ratón y teclado están en <see cref="MenuEntry"/>.
    /// </summary>
    public sealed class MenuButton : MenuEntry,
        IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI label;

        [Tooltip("Filete bajo el rótulo. Es lo que marca el estado.")]
        [SerializeField] private Image rule;

        [Tooltip("Filete ámbar que recorre al anterior al ganar el foco. Se dibuja encima.")]
        [SerializeField] private Image highlight;

        [Tooltip("Gráfico transparente que recibe los eventos de ratón en toda la fila.")]
        [SerializeField] private Image hitArea;

        [Tooltip("Grosor del filete en reposo. Al pulsar se multiplica por 2.")]
        [SerializeField, Min(1f)] private float ruleThickness = 1.5f;

        [Tooltip("Cuánto se desplaza el rótulo al enfocarse, en píxeles.")]
        [SerializeField] private float labelNudge = 6f;

        private bool pressed;
        private Vector2 labelRestPosition;

        private void Awake()
        {
            if (label == null) label = GetComponentInChildren<TextMeshProUGUI>();
            // El desplazamiento se mide siempre desde aquí, no desde la posición actual: si no,
            // cada pasada del ratón lo iría corriendo un poco más.
            if (label != null) labelRestPosition = label.rectTransform.anchoredPosition;
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
            float e = FocusEased;

            Color text;
            Color highlightColor = UITheme.RuleHover;
            float thickness = ruleThickness;

            if (!interactable)
            {
                text = UITheme.ButtonDisabled;
                e = 0f;                       // una fila apagada no se realza aunque tuviera foco
            }
            else if (pressed)
            {
                // El pulsado es instantáneo a propósito: animar la respuesta al clic la vuelve
                // blanda, y es el único estado que el jugador provoca directamente.
                text = UITheme.ButtonPressed;
                highlightColor = UITheme.RulePressed;
                thickness = ruleThickness * 2f;
                e = 1f;
            }
            else
            {
                text = Color.Lerp(UITheme.ButtonIdle, UITheme.ButtonHover, e);
            }

            if (label != null)
            {
                label.color = text;
                label.rectTransform.anchoredPosition =
                    labelRestPosition + new Vector2(labelNudge * e, 0f);
            }

            PaintRule(rule, highlight, interactable, thickness, highlightColor, e);

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
