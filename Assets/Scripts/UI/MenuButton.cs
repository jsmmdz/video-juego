using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace SilentDivide.UI
{
    /// <summary>
    /// Botón del menú, con los cuatro estados del kit de UX-UI: reposo, foco, pulsado y
    /// deshabilitado.
    ///
    /// El foco es uno solo y lo comparten ratón y teclado: apuntar con el ratón mueve la selección,
    /// igual que hacerlo con las flechas. Sin eso, un menú de teclado y ratón acaba mostrando dos
    /// botones resaltados a la vez.
    /// </summary>
    [RequireComponent(typeof(ChamferedPanel))]
    public sealed class MenuButton : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler,
        IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private ChamferedPanel panel;

        [Tooltip("Deshabilitado: no responde y se dibuja apagado. Por ejemplo «Continuar» cuando " +
                 "no hay partida guardada.")]
        [SerializeField] private bool interactable = true;

        /// <summary>Se dispara al activarlo, con ratón o con teclado.</summary>
        public event System.Action OnActivated;

        /// <summary>Avisa al controlador de que el ratón tomó el foco, para mover la selección.</summary>
        public event System.Action<MenuButton> OnFocusRequested;

        private bool focused;
        private bool pressed;

        public bool Interactable
        {
            get => interactable;
            set { interactable = value; if (!value) { focused = pressed = false; } Repaint(); }
        }

        private void Awake()
        {
            if (panel == null) panel = GetComponent<ChamferedPanel>();
            if (label == null) label = GetComponentInChildren<TextMeshProUGUI>();
        }

        private void OnEnable() => Repaint();

        /// <summary>Lo llama el controlador cuando este botón pasa a ser el seleccionado.</summary>
        public void SetFocused(bool value)
        {
            if (!interactable) return;
            focused = value;
            if (!value) pressed = false;
            Repaint();
        }

        /// <summary>Activación por teclado. El controlador la usa con Enter o Espacio.</summary>
        public void Activate()
        {
            if (!interactable) return;
            OnActivated?.Invoke();
        }

        private void Repaint()
        {
            if (panel == null) return;

            Color border, fill, text;

            if (!interactable)
            {
                border = UITheme.ButtonDisabled;
                fill   = Color.clear;
                text   = UITheme.ButtonDisabled;
            }
            else if (pressed)
            {
                border = UITheme.ButtonPressed;
                fill   = UITheme.ButtonPressedFill;
                text   = UITheme.TextPrimary;
            }
            else if (focused)
            {
                border = UITheme.ButtonHover;
                fill   = Color.clear;
                text   = UITheme.ButtonHover;
            }
            else
            {
                border = UITheme.ButtonIdle;
                fill   = Color.clear;
                text   = UITheme.TextPrimary;
            }

            panel.BorderColor = border;
            panel.color = fill;
            if (label != null) label.color = text;
        }

        // ── Ratón ────────────────────────────────────────────────────────────────────────────

        public void OnPointerEnter(PointerEventData e)
        {
            if (!interactable) return;
            OnFocusRequested?.Invoke(this);   // el controlador mueve aquí la selección
        }

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
            OnActivated?.Invoke();
        }
    }
}
