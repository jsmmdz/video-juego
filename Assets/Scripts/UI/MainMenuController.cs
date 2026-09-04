using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SilentDivide.UI
{
    /// <summary>
    /// Menú principal. Las opciones vienen del pitch (PDF pág. 5): Nueva Partida, Continuar y
    /// Ajustes de Sistema; el kit de UX-UI añade Salir.
    ///
    /// «Continuar» solo está disponible si hay partida guardada — por eso el kit define un estado
    /// deshabilitado.
    /// </summary>
    public sealed class MainMenuController : MonoBehaviour
    {
        [Header("Botones, en orden de navegación")]
        [SerializeField] private List<MenuButton> buttons = new List<MenuButton>();

        [Header("Referencias")]
        [SerializeField] private MenuButton newGameButton;
        [SerializeField] private MenuButton continueButton;
        [SerializeField] private MenuButton settingsButton;
        [SerializeField] private MenuButton quitButton;

        [Header("Escenas")]
        [Tooltip("Escena a cargar al empezar. Debe estar en Build Settings.")]
        [SerializeField] private string gameplayScene = "Prototipo";

        /// <summary>Clave provisional de guardado, hasta que exista el sistema real.</summary>
        private const string SaveKey = "SilentDivide.HasSave";

        private int focusedIndex = -1;

        private void Start()
        {
            foreach (MenuButton button in buttons)
            {
                if (button == null) continue;
                button.OnFocusRequested += Focus;
            }

            if (newGameButton  != null) newGameButton.OnActivated  += StartNewGame;
            if (continueButton != null) continueButton.OnActivated += ContinueGame;
            if (settingsButton != null) settingsButton.OnActivated += OpenSettings;
            if (quitButton     != null) quitButton.OnActivated     += Quit;

            if (continueButton != null)
                continueButton.Interactable = HasSavedGame();

            FocusFirstAvailable();
        }

        private void OnDestroy()
        {
            foreach (MenuButton button in buttons)
            {
                if (button == null) continue;
                button.OnFocusRequested -= Focus;
            }

            if (newGameButton  != null) newGameButton.OnActivated  -= StartNewGame;
            if (continueButton != null) continueButton.OnActivated -= ContinueGame;
            if (settingsButton != null) settingsButton.OnActivated -= OpenSettings;
            if (quitButton     != null) quitButton.OnActivated     -= Quit;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                Step(1);
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                Step(-1);
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)
                     || Input.GetKeyDown(KeyCode.Space))
                ActivateFocused();
        }

        // ── Navegación ───────────────────────────────────────────────────────────────────────

        private void Focus(MenuButton button)
        {
            int index = buttons.IndexOf(button);
            if (index >= 0) SetFocus(index);
        }

        private void SetFocus(int index)
        {
            for (int i = 0; i < buttons.Count; i++)
                if (buttons[i] != null)
                    buttons[i].SetFocused(i == index);

            focusedIndex = index;
        }

        /// <summary>
        /// Avanza saltando los botones deshabilitados. Recorre como mucho una vuelta completa, para
        /// no colgarse si ninguno es seleccionable.
        /// </summary>
        private void Step(int direction)
        {
            if (buttons.Count == 0) return;

            int index = focusedIndex;
            for (int i = 0; i < buttons.Count; i++)
            {
                index = (index + direction + buttons.Count) % buttons.Count;
                if (buttons[index] != null && buttons[index].Interactable)
                {
                    SetFocus(index);
                    return;
                }
            }
        }

        private void FocusFirstAvailable()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                if (buttons[i] != null && buttons[i].Interactable)
                {
                    SetFocus(i);
                    return;
                }
            }
        }

        private void ActivateFocused()
        {
            if (focusedIndex < 0 || focusedIndex >= buttons.Count) return;
            if (buttons[focusedIndex] != null) buttons[focusedIndex].Activate();
        }

        // ── Acciones ─────────────────────────────────────────────────────────────────────────

        private static bool HasSavedGame() => PlayerPrefs.GetInt(SaveKey, 0) == 1;

        private void StartNewGame()
        {
            // Al iniciar partida nueva el pitch intercala la elección de Origen del Perfil y
            // Especialismo de Infiltración. Están fuera del alcance del prototipo, así que por ahora
            // se entra directo al escenario.
            LoadGameplay();
        }

        private void ContinueGame()
        {
            // Retomará el último punto guardado con el inventario intacto. Mientras no exista el
            // sistema de guardado, carga la misma escena.
            LoadGameplay();
        }

        private void LoadGameplay()
        {
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
            // Gráficos, audio, controles y accesibilidad (PDF pág. 5). Pendiente de implementar.
            Debug.Log("[Menú] Ajustes de Sistema: pendiente.");
        }

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
