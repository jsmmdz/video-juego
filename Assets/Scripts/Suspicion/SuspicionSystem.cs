using System.Collections.Generic;
using UnityEngine;

namespace SilentDivide.Suspicion
{
    /// <summary>
    /// Módulo 2 — Sistema de sospecha. ES LA MECÁNICA CENTRAL DEL JUEGO.
    /// Flowchart: board de Figma, nodo 50:202. Especificación: docs/tecnico/02-sospecha.md
    ///
    /// Nero se infiltra en Aurea y hay zonas vigiladas: si se queda en ellas la sospecha sube,
    /// si sale baja, y si llega al máximo lo detectan.
    ///
    /// Va en el mismo GameObject que el jugador. El CharacterController actúa como colisionador,
    /// así que los eventos de trigger de las zonas llegan aquí.
    /// </summary>
    public sealed class SuspicionSystem : MonoBehaviour
    {
        [Header("Curva de sospecha")]
        [Tooltip("Valor que dispara la detección.")]
        [SerializeField, Min(0.01f)] private float maxSuspicion = 100f;

        [Tooltip("Sospecha por segundo DENTRO de zona vigilada.")]
        [SerializeField, Min(0f)] private float riseRate = 25f;

        [Tooltip("Sospecha por segundo FUERA de zona vigilada. La relación entre subida y bajada " +
                 "define el margen de maniobra del jugador: es el primer valor a ajustar en " +
                 "playtesting.")]
        [SerializeField, Min(0f)] private float fallRate = 15f;

        /// <summary>Sospecha actual, entre 0 y <see cref="maxSuspicion"/>.</summary>
        public float Suspicion { get; private set; }

        /// <summary>Sospecha normalizada entre 0 y 1. Es lo que consume la barra de UI.</summary>
        public float NormalizedSuspicion => Suspicion / maxSuspicion;

        /// <summary>
        /// Latch: una vez verdadero NO vuelve solo a falso. El diagrama corta el ciclo en el primer
        /// rombo. Quien lo reinicia es el resto del juego (checkpoint, fin de alerta), no este módulo.
        /// </summary>
        public bool Detected { get; private set; }

        /// <summary>Se dispara una única vez, en el fotograma en que la sospecha llega al máximo.</summary>
        public event System.Action OnDetected;

        private readonly HashSet<SurveillanceZone> occupiedZones = new HashSet<SurveillanceZone>();

        /// <summary>
        /// Único punto de contacto con el "cómo" se determina la vigilancia. Cuando se implementen
        /// los conos de visión de guardias, se sustituye esta implementación y el resto del módulo
        /// no cambia.
        /// </summary>
        private bool IsInsideSurveillanceZone()
        {
            occupiedZones.RemoveWhere(zone => zone == null || !zone.isActiveAndEnabled);
            return occupiedZones.Count > 0;
        }

        private void Update()
        {
            // ── ¿Ya detectado? ────────────────────────────────────────────────────────────────
            // El estado queda congelado: no se recalcula nada más.
            if (Detected) return;

            // ── ¿Dentro de zona vigilada? ─────────────────────────────────────────────────────
            // Sube O baja, nunca las dos cosas en el mismo fotograma.
            // Saturación en ambos extremos: sin el piso en cero, la sospecha se volvería negativa
            // fuera de las zonas y el jugador acumularía un colchón invisible.
            if (IsInsideSurveillanceZone())
            {
                Suspicion = Mathf.Min(Suspicion + riseRate * Time.deltaTime, maxSuspicion);
            }
            else
            {
                Suspicion = Mathf.Max(Suspicion - fallRate * Time.deltaTime, 0f);
            }

            // ── Actualizar barra de UI ────────────────────────────────────────────────────────
            // Siempre, suba o baje, y ANTES de comprobar el máximo: así la barra llega visualmente
            // al tope en el mismo fotograma en que se dispara la detección.
            // (La UI se suscribe leyendo NormalizedSuspicion; ver SuspicionBar.)

            // ── ¿Sospecha ≥ máximo? ───────────────────────────────────────────────────────────
            if (Suspicion >= maxSuspicion)
            {
                Detected = true;
                OnDetected?.Invoke();   // avisa al resto del juego: alarma, bloqueo de accesos, checkpoint
            }
        }

        /// <summary>Reinicio desde el resto del juego (punto de control, fin de la alerta).</summary>
        public void ResetDetection()
        {
            Detected = false;
            Suspicion = 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out SurveillanceZone zone))
                occupiedZones.Add(zone);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out SurveillanceZone zone))
                occupiedZones.Remove(zone);
        }
    }
}
