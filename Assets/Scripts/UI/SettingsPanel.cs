using System.Collections.Generic;
using UnityEngine;

namespace SilentDivide.UI
{
    /// <summary>
    /// Ajustes de Sistema: gráficos, audio, controles y accesibilidad (pitch, pág. 5).
    ///
    /// Es una pantalla dentro de la misma escena del menú, no una escena aparte: así la ilustración
    /// de fondo no se recarga al entrar y salir, y la transición no parpadea.
    ///
    /// Las filas las construye <c>MainMenuSceneBuilder</c> y se registran aquí. Este componente solo
    /// las conecta con <see cref="GameSettings"/> y gestiona el foco y la salida con Escape.
    /// </summary>
    public sealed class SettingsPanel : MonoBehaviour
    {
        [Header("Filas, en orden de navegación")]
        [SerializeField] private List<MenuItem> items = new List<MenuItem>();

        [Header("Gráficos")]
        [SerializeField] private MenuOption fullscreenOption;
        [SerializeField] private MenuOption qualityOption;

        [Header("Audio")]
        [SerializeField] private MenuSlider masterSlider;
        [SerializeField] private MenuSlider musicSlider;
        [SerializeField] private MenuSlider effectsSlider;

        [Header("Accesibilidad")]
        [SerializeField] private MenuOption textScaleOption;
        [SerializeField] private MenuOption reduceFlashOption;

        [Header("Salida")]
        [SerializeField] private MenuButton backButton;

        /// <summary>Lo escucha el menú principal para volver a tomar el foco.</summary>
        public event System.Action OnClosed;

        /// <summary>Multiplicadores del tamaño de texto, en el orden de las etiquetas.</summary>
        private static readonly float[] TextScales = { 1f, 1.15f, 1.3f };
        private static readonly string[] TextScaleLabels = { "Normal", "Grande", "Muy grande" };
        private static readonly string[] YesNo = { "No", "Sí" };

        private MenuNavigator navigator;

        // La escena se construye con esta pantalla desactivada, así que Awake no corre al
        // arrancar: corre la primera vez que Open() la activa. Por eso Open() activa antes de
        // tocar el navegador.
        private void Awake()
        {
            navigator = new MenuNavigator(items);
            Bind();
        }

        private void OnDestroy() => navigator?.Dispose();

        private void Update()
        {
            navigator.HandleInput();

            // Escape sale de Ajustes. Es la única tecla que no delega en el navegador: cerrar la
            // pantalla no es navegar dentro de ella.
            if (Input.GetKeyDown(KeyCode.Escape)) Close();
        }

        // ── Apertura y cierre ────────────────────────────────────────────────────────────────

        public void Open()
        {
            gameObject.SetActive(true);   // dispara Awake la primera vez: deja el navegador listo
            Refresh();
            navigator.FocusFirstAvailable();
        }

        public void Close()
        {
            GameSettings.Save();
            gameObject.SetActive(false);
            OnClosed?.Invoke();
        }

        // ── Conexión con los ajustes ─────────────────────────────────────────────────────────

        private void Bind()
        {
            if (fullscreenOption != null)
                fullscreenOption.OnValueChanged += i => GameSettings.Fullscreen = i == 1;

            if (qualityOption != null)
                qualityOption.OnValueChanged += i => GameSettings.QualityLevel = i;

            if (masterSlider  != null) masterSlider.OnValueChanged  += v => GameSettings.MasterVolume  = v;
            if (musicSlider   != null) musicSlider.OnValueChanged   += v => GameSettings.MusicVolume   = v;
            if (effectsSlider != null) effectsSlider.OnValueChanged += v => GameSettings.EffectsVolume = v;

            if (textScaleOption != null)
                textScaleOption.OnValueChanged += i => GameSettings.TextScale = TextScales[i];

            if (reduceFlashOption != null)
                reduceFlashOption.OnValueChanged += i => GameSettings.ReduceFlashing = i == 1;

            if (backButton != null) backButton.OnActivated += Close;
        }

        /// <summary>Vuelca los valores guardados a las filas. Se llama cada vez que se abre.</summary>
        private void Refresh()
        {
            if (fullscreenOption != null)
                fullscreenOption.Configure("Pantalla completa", YesNo, GameSettings.Fullscreen ? 1 : 0);

            if (qualityOption != null)
                qualityOption.Configure("Calidad", QualitySettings.names, GameSettings.QualityLevel);

            if (masterSlider  != null) masterSlider.Configure("Volumen general", GameSettings.MasterVolume);
            if (musicSlider   != null) musicSlider.Configure("Música",           GameSettings.MusicVolume);
            if (effectsSlider != null) effectsSlider.Configure("Efectos",        GameSettings.EffectsVolume);

            if (textScaleOption != null)
                textScaleOption.Configure("Tamaño del texto", TextScaleLabels,
                                          NearestTextScale(GameSettings.TextScale));

            if (reduceFlashOption != null)
                reduceFlashOption.Configure("Reducir destellos", YesNo,
                                            GameSettings.ReduceFlashing ? 1 : 0);
        }

        /// <summary>
        /// El valor guardado es un multiplicador, no un índice: si alguna vez cambian los tramos,
        /// se elige el más cercano en vez de reiniciar la preferencia del jugador.
        /// </summary>
        private static int NearestTextScale(float scale)
        {
            int best = 0;
            float bestDistance = float.MaxValue;

            for (int i = 0; i < TextScales.Length; i++)
            {
                float distance = Mathf.Abs(TextScales[i] - scale);
                if (distance < bestDistance) { bestDistance = distance; best = i; }
            }

            return best;
        }
    }
}
