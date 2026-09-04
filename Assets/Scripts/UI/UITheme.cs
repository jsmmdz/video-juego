using UnityEngine;

namespace SilentDivide.UI
{
    /// <summary>
    /// Paleta de la interfaz, tomada del kit de UX-UI del board de Figma.
    /// Centralizada para que menú y HUD compartan los mismos valores: la barra de sospecha y el
    /// overlay de alerta reutilizan los estados de aviso definidos aquí.
    /// </summary>
    public static class UITheme
    {
        // ── Fondo ────────────────────────────────────────────────────────────────────────────
        public static readonly Color Background     = Hex(0x080B14);
        public static readonly Color BackgroundDeep = Hex(0x04060C);

        // ── Botones: estados ─────────────────────────────────────────────────────────────────
        /// <summary>Reposo: contorno claro, sin relleno.</summary>
        public static readonly Color ButtonIdle      = Hex(0xC9CEE4);
        /// <summary>Foco: el contorno y el texto pasan a violeta.</summary>
        public static readonly Color ButtonHover     = Hex(0xA98CF5);
        /// <summary>Pulsado: relleno violeta con borde más claro.</summary>
        public static readonly Color ButtonPressed   = Hex(0xC4AEFF);
        public static readonly Color ButtonPressedFill = Hex(0x4A2E9E, 0.85f);
        /// <summary>Deshabilitado: por ejemplo «Continuar» sin partida guardada.</summary>
        public static readonly Color ButtonDisabled  = Hex(0x3A4056);

        // ── Texto ────────────────────────────────────────────────────────────────────────────
        public static readonly Color TextPrimary   = Hex(0xE8EAF4);
        public static readonly Color TextSecondary = Hex(0x8A90A8);

        // ── Estados de aviso (compartidos con el HUD) ────────────────────────────────────────
        public static readonly Color StateNormal    = Hex(0xC9CEE4);
        public static readonly Color StateSuspicion = Hex(0xD9A227);
        public static readonly Color StateAlert     = Hex(0xE01E1E);
        public static readonly Color StateAlarm     = Hex(0xFF2A2A);

        private static Color Hex(int rgb, float alpha = 1f) => new Color(
            ((rgb >> 16) & 0xFF) / 255f,
            ((rgb >> 8)  & 0xFF) / 255f,
            ( rgb        & 0xFF) / 255f,
            alpha);
    }
}
