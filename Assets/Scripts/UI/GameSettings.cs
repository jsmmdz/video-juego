using UnityEngine;

namespace SilentDivide.UI
{
    /// <summary>
    /// Ajustes de Sistema: gráficos, audio, controles y accesibilidad (pitch, pág. 5).
    ///
    /// Guarda en <c>PlayerPrefs</c>, que es suficiente para preferencias de máquina y no tiene nada
    /// que ver con el guardado de partida —ese sistema aún no existe—. Los valores se aplican al
    /// cambiarlos y se vuelven a aplicar al arrancar, para que la primera pantalla ya salga con lo
    /// que el jugador dejó puesto.
    /// </summary>
    public static class GameSettings
    {
        private const string KeyFullscreen  = "SilentDivide.Fullscreen";
        private const string KeyQuality     = "SilentDivide.Quality";
        private const string KeyMaster      = "SilentDivide.Volume.Master";
        private const string KeyMusic       = "SilentDivide.Volume.Music";
        private const string KeyEffects     = "SilentDivide.Volume.Effects";
        private const string KeyTextScale   = "SilentDivide.TextScale";
        private const string KeyReduceFlash = "SilentDivide.ReduceFlash";

        /// <summary>Se dispara cuando cambia cualquier ajuste, para que la interfaz se redibuje.</summary>
        public static event System.Action Changed;

        // ── Gráficos ─────────────────────────────────────────────────────────────────────────

        public static bool Fullscreen
        {
            get => PlayerPrefs.GetInt(KeyFullscreen, Screen.fullScreen ? 1 : 0) == 1;
            set { PlayerPrefs.SetInt(KeyFullscreen, value ? 1 : 0); Screen.fullScreen = value; Notify(); }
        }

        /// <summary>Índice dentro de <c>QualitySettings.names</c>.</summary>
        public static int QualityLevel
        {
            get => Mathf.Clamp(PlayerPrefs.GetInt(KeyQuality, QualitySettings.GetQualityLevel()),
                               0, QualitySettings.names.Length - 1);
            set
            {
                int level = Mathf.Clamp(value, 0, QualitySettings.names.Length - 1);
                PlayerPrefs.SetInt(KeyQuality, level);
                // applyExpensiveChanges en falso: cambiar calidad desde un menú no debe provocar
                // el tirón de recargar texturas mientras el jugador recorre las opciones.
                QualitySettings.SetQualityLevel(level, false);
                Notify();
            }
        }

        // ── Audio ────────────────────────────────────────────────────────────────────────────
        // Solo el volumen general llega hoy al motor: no hay AudioMixer todavía, así que música y
        // efectos se guardan y esperan a que existan sus buses. Se exponen igual porque la pantalla
        // de Ajustes del pitch los lista, y así el jugador no ve la opción aparecer más tarde.

        public static float MasterVolume
        {
            get => PlayerPrefs.GetFloat(KeyMaster, 1f);
            set { float v = Mathf.Clamp01(value); PlayerPrefs.SetFloat(KeyMaster, v);
                  AudioListener.volume = v; Notify(); }
        }

        public static float MusicVolume
        {
            get => PlayerPrefs.GetFloat(KeyMusic, 0.8f);
            set { PlayerPrefs.SetFloat(KeyMusic, Mathf.Clamp01(value)); Notify(); }
        }

        public static float EffectsVolume
        {
            get => PlayerPrefs.GetFloat(KeyEffects, 1f);
            set { PlayerPrefs.SetFloat(KeyEffects, Mathf.Clamp01(value)); Notify(); }
        }

        // ── Accesibilidad ────────────────────────────────────────────────────────────────────

        /// <summary>Multiplicador del tamaño de texto de interfaz. Lo aplica <see cref="TextScaler"/>.</summary>
        public static float TextScale
        {
            get => PlayerPrefs.GetFloat(KeyTextScale, 1f);
            set { PlayerPrefs.SetFloat(KeyTextScale, Mathf.Clamp(value, 1f, 1.5f)); Notify(); }
        }

        /// <summary>
        /// Reduce los parpadeos. Lo leerá el overlay de alerta de la fase 2, que es el único
        /// elemento que destella.
        /// </summary>
        public static bool ReduceFlashing
        {
            get => PlayerPrefs.GetInt(KeyReduceFlash, 0) == 1;
            set { PlayerPrefs.SetInt(KeyReduceFlash, value ? 1 : 0); Notify(); }
        }

        // ── Ciclo ────────────────────────────────────────────────────────────────────────────

        /// <summary>Vuelca al motor lo guardado. Se llama al abrir el menú principal.</summary>
        public static void Apply()
        {
            Screen.fullScreen = Fullscreen;
            QualitySettings.SetQualityLevel(QualityLevel, false);
            AudioListener.volume = MasterVolume;
            Notify();
        }

        public static void Save() => PlayerPrefs.Save();

        private static void Notify() => Changed?.Invoke();
    }
}
