using UnityEngine;

namespace SilentDivide.UI
{
    /// <summary>
    /// Paleta de la interfaz, tomada del mockup definitivo de la pantalla de inicio.
    /// Centralizada para que menú y HUD compartan los mismos valores: la barra de sospecha y el
    /// overlay de alerta reutilizan los estados de aviso definidos aquí.
    ///
    /// La pantalla de inicio no tiene color propio: **el color lo pone la ilustración**. La
    /// interfaz encima es un panel oscuro translúcido y texto hueso, y nada más. Por eso aquí casi
    /// todo son grises violáceos: cualquier acento saturado competiría con las lámparas del
    /// escenario, que son lo único cálido del cuadro.
    /// </summary>
    public static class UITheme
    {
        // ── Fondo ────────────────────────────────────────────────────────────────────────────
        /// <summary>Se ve solo antes de que cargue la ilustración, o si falta.</summary>
        public static readonly Color Background     = Hex(0x1A1622);
        public static readonly Color BackgroundDeep = Hex(0x0E0B14);

        /// <summary>
        /// Panel de la columna izquierda: negro violáceo translúcido sobre la ilustración.
        /// Oscurece lo justo para que el texto hueso tenga contraste sin tapar la escena.
        /// </summary>
        public static readonly Color PanelScrim     = Hex(0x14101C, 0.82f);
        /// <summary>Borde derecho del panel, apenas visible: separa sin dibujar una línea dura.</summary>
        public static readonly Color PanelEdge      = Hex(0x000000, 0.35f);

        // ── Botones: estados ─────────────────────────────────────────────────────────────────
        // El mockup solo muestra el estado de reposo. Foco y pulsado se derivan de él subiendo el
        // texto a blanco y encendiendo el filete en el ámbar de las lámparas de la ilustración,
        // que es el único cálido del cuadro. Pendiente de confirmar con diseño.

        /// <summary>Reposo: texto hueso y filete gris tenue debajo.</summary>
        public static readonly Color ButtonIdle       = Hex(0xEDEAE4);
        public static readonly Color RuleIdle         = Hex(0x8A8494, 0.75f);
        /// <summary>Foco: el texto sube a blanco y el filete se enciende en ámbar.</summary>
        public static readonly Color ButtonHover      = Hex(0xFFFFFF);
        public static readonly Color RuleHover        = Hex(0xE8A33D);
        /// <summary>Pulsado: el ámbar se aclara y el filete engorda (lo aplica el botón).</summary>
        public static readonly Color ButtonPressed    = Hex(0xF6D061);
        public static readonly Color RulePressed      = Hex(0xF6D061);
        /// <summary>Deshabilitado: por ejemplo «Continuar» sin partida guardada.</summary>
        public static readonly Color ButtonDisabled   = Hex(0x5A5568);
        public static readonly Color RuleDisabled     = Hex(0x3A3646, 0.75f);

        // ── Texto ────────────────────────────────────────────────────────────────────────────
        /// <summary>Hueso, no blanco puro: es el color del título y de todo el texto de interfaz.</summary>
        public static readonly Color TextPrimary   = Hex(0xEDEAE4);
        public static readonly Color TextSecondary = Hex(0x9A94A6);
        public static readonly Color TextMuted     = Hex(0x5A5568);

        // ── Estados de aviso (compartidos con el HUD) ────────────────────────────────────────
        // El HUD sí necesita color: va sobre el juego, no sobre la ilustración del menú.
        public static readonly Color StateNormal    = Hex(0x9A94A6);
        public static readonly Color StateSuspicion = Hex(0xE8A33D);
        public static readonly Color StateAlert     = Hex(0xE01E1E);
        public static readonly Color StateAlarm     = Hex(0xFF2A2A);

        private static Color Hex(int rgb, float alpha = 1f) => new Color(
            ((rgb >> 16) & 0xFF) / 255f,
            ((rgb >> 8)  & 0xFF) / 255f,
            ( rgb        & 0xFF) / 255f,
            alpha);
    }
}
