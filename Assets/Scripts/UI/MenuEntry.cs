using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SilentDivide.UI
{
    /// <summary>
    /// Base de todo lo seleccionable del menú: botones, selectores de valor y barras.
    ///
    /// Existe porque la pantalla de Ajustes mezcla los tres tipos en una misma columna y el foco
    /// tiene que recorrerlos por igual. La navegación (<see cref="MenuNavigator"/>) trabaja contra
    /// esta clase y no necesita saber qué es cada fila.
    ///
    /// El foco es uno solo y lo comparten ratón y teclado: apuntar con el ratón mueve la selección,
    /// igual que hacerlo con las flechas. Sin eso, un menú de teclado y ratón acaba mostrando dos
    /// filas resaltadas a la vez.
    /// </summary>
    public abstract class MenuEntry : MonoBehaviour, IPointerEnterHandler
    {
        [Tooltip("Deshabilitado: no responde y se dibuja apagado. Por ejemplo «Continuar» cuando " +
                 "no hay partida guardada.")]
        [SerializeField] protected bool interactable = true;

        [Tooltip("Duración de la transición al ganar o perder el foco, en segundos.")]
        [SerializeField, Min(0f)] protected float transitionSeconds = 0.14f;

        /// <summary>Se dispara al activarlo, con ratón o con teclado.</summary>
        public event System.Action OnActivated;

        /// <summary>Avisa al navegador de que el ratón tomó el foco, para mover la selección.</summary>
        public event System.Action<MenuEntry> OnFocusRequested;

        /// <summary>Cierto mientras esta fila es la seleccionada.</summary>
        protected bool Focused { get; private set; }

        /// <summary>
        /// Cuánto foco tiene la fila ahora mismo, de 0 a 1, **animado**. Es lo que usan las
        /// subclases al repintar, en vez del booleano: así el realce entra y sale en vez de saltar.
        /// </summary>
        protected float FocusAmount { get; private set; }

        /// <summary>
        /// El mismo valor con una curva suave. En una transición tan corta la interpolación lineal
        /// se nota dura al arrancar y al frenar.
        /// </summary>
        protected float FocusEased => FocusAmount * FocusAmount * (3f - 2f * FocusAmount);

        public bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;
                if (!value) { Focused = false; OnFocusLost(); }
                Repaint();
            }
        }

        protected virtual void OnEnable()
        {
            // Al reactivar la pantalla el realce arranca de cero: si se conservara, la fila que
            // tenía el foco al salir reaparecería ya encendida antes de que nadie la seleccione.
            FocusAmount = Focused && interactable ? 1f : 0f;
            Repaint();
        }

        protected virtual void Update()
        {
            float target = Focused && interactable ? 1f : 0f;
            if (Mathf.Approximately(FocusAmount, target)) return;

            // Sin escalar por Time.timeScale: el menú tiene que animarse igual con el juego en
            // pausa, que es justo cuando se abren los Ajustes.
            FocusAmount = transitionSeconds <= 0f
                ? target
                : Mathf.MoveTowards(FocusAmount, target, Time.unscaledDeltaTime / transitionSeconds);

            Repaint();
        }

        /// <summary>Lo llama el navegador cuando esta fila pasa a ser la seleccionada.</summary>
        public void SetFocused(bool value)
        {
            if (!interactable) return;
            Focused = value;
            if (!value) OnFocusLost();
            Repaint();
        }

        /// <summary>Activación con Enter, Espacio o clic.</summary>
        public virtual void Activate()
        {
            if (!interactable) return;
            OnActivated?.Invoke();
        }

        /// <summary>
        /// Flechas izquierda y derecha sobre la fila enfocada. Los botones no la usan; los
        /// selectores y las barras cambian su valor con ella.
        /// </summary>
        /// <param name="direction">−1 izquierda, +1 derecha.</param>
        public virtual void Adjust(int direction) { }

        /// <summary>Redibuja según estado. Es donde vive el kit de UX-UI de cada tipo de fila.</summary>
        public abstract void Repaint();

        /// <summary>Gancho para limpiar estado transitorio, como el «pulsado» de un botón.</summary>
        protected virtual void OnFocusLost() { }

        protected void RaiseActivated() => OnActivated?.Invoke();

        /// <summary>
        /// Pinta el filete de una fila y su realce. Lo comparten los tres tipos porque el gesto es
        /// el mismo: el filete de reposo no cambia de color nunca —es el del mockup— y encima suyo
        /// crece un segundo filete ámbar que lo barre de izquierda a derecha.
        /// </summary>
        /// <param name="amount">Cuánto realce, de 0 a 1. Es el ancho del barrido.</param>
        protected static void PaintRule(Image rule, Image highlight, bool interactable,
                                        float thickness, Color highlightColor, float amount)
        {
            if (rule != null)
            {
                rule.color = interactable ? UITheme.RuleIdle : UITheme.RuleDisabled;
                Vector2 size = rule.rectTransform.sizeDelta;
                rule.rectTransform.sizeDelta = new Vector2(size.x, thickness);
            }

            if (highlight == null) return;

            highlight.color = highlightColor;

            RectTransform rect = highlight.rectTransform;
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(amount, 1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        public void OnPointerEnter(PointerEventData e)
        {
            if (!interactable) return;
            OnFocusRequested?.Invoke(this);   // el navegador mueve aquí la selección
        }
    }
}
