using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SilentDivide.UI
{
    /// <summary>
    /// Pantalla de inicio. El mockup definitivo del kit de UX-UI muestra dos opciones —«Jugar» y
    /// «Ajustes»— sobre la ilustración del callejón de Umbria.
    ///
    /// El pitch (pág. 5) nombra además «Nueva Partida» y «Continuar». Están **pendientes de
    /// decidir**: el mockup no las coloca, y no se inventa aquí dónde van. Mientras tanto «Jugar»
    /// entra al escenario, y la detección de partida guardada ya está escrita para cuando esas dos
    /// opciones tengan sitio.
    ///
    /// Coordina las dos pantallas de la escena: solo la visible procesa entrada, para que las
    /// flechas no muevan dos focos a la vez.
    /// </summary>
    public sealed class MainMenuController : MonoBehaviour
    {
        [Header("Opciones, en orden de navegación")]
        [SerializeField] private List<MenuEntry> items = new List<MenuEntry>();

        [Header("Referencias")]
        [SerializeField] private MenuButton playButton;
        [SerializeField] private MenuButton settingsButton;

        [Header("Pantallas")]
        [SerializeField] private GameObject rootScreen;
        [SerializeField] private SettingsPanel settingsPanel;

        [Header("Escenas")]
        [Tooltip("Escena a cargar al empezar. Debe estar en Build Settings.")]
        [SerializeField] private string gameplayScene = "Prototipo";

        /// <summary>Clave provisional de guardado, hasta que exista el sistema real.</summary>
        private const string SaveKey = "SilentDivide.HasSave";

        private MenuNavigator navigator;

        private void Start()
        {
            // Antes de dibujar nada: la primera pantalla ya sale con lo que el jugador dejó puesto.
            GameSettings.Apply();

            navigator = new MenuNavigator(items);

            if (playButton     != null) playButton.OnActivated     += Play;
            if (settingsButton != null) settingsButton.OnActivated += OpenSettings;
            if (settingsPanel  != null) settingsPanel.OnClosed     += ReturnFromSettings;

            navigator.FocusFirstAvailable();
        }

        private void OnDestroy()
        {
            navigator?.Dispose();

            if (playButton     != null) playButton.OnActivated     -= Play;
            if (settingsButton != null) settingsButton.OnActivated -= OpenSettings;
            if (settingsPanel  != null) settingsPanel.OnClosed     -= ReturnFromSettings;
        }

        private void Update()
        {
            // Con Ajustes abierto la entrada es suya: este menú queda visible detrás pero inerte.
            if (settingsPanel != null && settingsPanel.gameObject.activeSelf) return;

            navigator.HandleInput();
        }

        // ── Acciones ─────────────────────────────────────────────────────────────────────────

        /// <summary>Cierto si hay partida guardada. Lo usará «Continuar» cuando exista.</summary>
        public static bool HasSavedGame() => PlayerPrefs.GetInt(SaveKey, 0) == 1;

        private void Play()
        {
            // Al iniciar partida nueva el pitch intercala la elección de Origen del Perfil y
            // Especialismo de Infiltración. Están fuera del alcance del prototipo, así que por ahora
            // se entra directo al escenario.
            if (string.IsNullOrEmpty(gameplayScene))
            {
                Debug.LogError("[Menú] No hay escena de juego configurada.");
                return;
            }

            if (!Application.CanStreamedLevelBeLoaded(gameplayScene))
            {
                Debug.LogError($"[Menú] La escena '{gameplayScene}' no está en Build Settings. " +
                               "Añádela en File ▸ Build Settings.");
                return;
            }

            SceneManager.LoadScene(gameplayScene);
        }

        private void OpenSettings()
        {
            if (settingsPanel == null)
            {
                Debug.LogWarning("[Menú] No hay panel de Ajustes conectado.");
                return;
            }

            // Se oculta la columna del menú, no el canvas entero: la ilustración de fondo
            // permanece montada, así que entrar en Ajustes no la recarga ni parpadea.
            navigator.ClearHighlight();
            if (rootScreen != null) rootScreen.SetActive(false);
            settingsPanel.Open();
        }

        private void ReturnFromSettings()
        {
            if (rootScreen != null) rootScreen.SetActive(true);
            navigator.RestoreHighlight();
        }
    }
}
