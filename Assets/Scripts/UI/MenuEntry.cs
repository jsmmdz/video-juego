using UnityEngine;
using UnityEngine.EventSystems;

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

        /// <summary>Se dispara al activarlo, con ratón o con teclado.</summary>
        public event System.Action OnActivated;

        /// <summary>Avisa al navegador de que el ratón tomó el foco, para mover la selección.</summary>
        public event System.Action<MenuEntry> OnFocusRequested;

        /// <summary>Cierto mientras esta fila es la seleccionada.</summary>
        protected bool Focused { get; private set; }

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

        protected virtual void OnEnable() => Repaint();

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

        public void OnPointerEnter(PointerEventData e)
        {
            if (!interactable) return;
            OnFocusRequested?.Invoke(this);   // el navegador mueve aquí la selección
        }
    }
}
