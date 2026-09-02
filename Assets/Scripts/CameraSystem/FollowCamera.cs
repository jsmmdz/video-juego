using UnityEngine;

namespace SilentDivide.CameraSystem
{
    /// <summary>
    /// Módulo 3 — Cámara seguidora.
    /// Flowchart: board de Figma, nodo 50:215. Especificación: docs/tecnico/03-camara.md
    ///
    /// LA CÁMARA NUNCA ROTA. Es la regla que define la identidad visual del juego: el ángulo lo
    /// fija el desplazamiento de cada zona y no se modifica en juego. Rotarla destruiría la
    /// composición de "escena enmarcada" del referente y rompería el módulo 4, que asume un eje
    /// estable contra el cual medir la dirección del personaje.
    ///
    /// Se ejecuta en LateUpdate: al final del fotograma, cuando el jugador YA se movió. Si corriera
    /// antes, encuadraría la posición del fotograma anterior y temblaría en movimiento continuo.
    /// </summary>
    [DefaultExecutionOrder(ExecutionOrder.Camera)]
    public sealed class FollowCamera : MonoBehaviour
    {
        [Header("Objetivo")]
        [SerializeField] private Transform target;

        [Header("Encuadre")]
        [Tooltip("Vector desde el jugador hasta la cámara. Lo define cada zona; este es el valor " +
                 "por defecto para las áreas sin CameraZone propia.")]
        [SerializeField] private Vector3 defaultOffset = new Vector3(0f, 12f, -10f);

        [Tooltip("Tiempo de aproximación a la posición deseada. Un seguimiento rígido transmitiría " +
                 "cada micro-corrección del movimiento a la imagen.")]
        [SerializeField, Min(0f)] private float smoothTime = 0.25f;

        private Vector3 currentOffset;
        private Vector3 targetOffset;
        private float offsetTransitionDuration;
        private Vector3 offsetVelocity;
        private Vector3 followVelocity;

        /// <summary>El jugador al que sigue la cámara. Lo consultan las CameraZone.</summary>
        public Transform Target => target;

        private void Awake()
        {
            currentOffset = targetOffset = defaultOffset;
        }

        private void LateUpdate()
        {
            // ── ¿Tiene a quién seguir? ────────────────────────────────────────────────────────
            // Se comprueba primero para no fallar en carga de escena, cambio de nivel o cinemáticas.
            if (target == null) return;

            // Transición del encuadre entre zonas. Con duración 0 el cambio es un corte seco, que
            // es el comportamiento por defecto y el coherente con la idea de escena enmarcada.
            if (offsetTransitionDuration <= 0f)
                currentOffset = targetOffset;
            else
                currentOffset = Vector3.SmoothDamp(
                    currentOffset, targetOffset, ref offsetVelocity, offsetTransitionDuration);

            // ── Calcular posición deseada: jugador + desplazamiento de zona ───────────────────
            Vector3 desiredPosition = target.position + currentOffset;

            // ── Acercar la cámara suavemente, SIN ROTAR ──────────────────────────────────────
            transform.position = Vector3.SmoothDamp(
                transform.position, desiredPosition, ref followVelocity, smoothTime);

            // La rotación no se toca aquí. Nunca.
        }

        /// <summary>Cambia el encuadre al entrar en una zona con desplazamiento propio.</summary>
        public void EnterZone(Vector3 offset, float transitionDuration)
        {
            targetOffset = offset;
            offsetTransitionDuration = transitionDuration;
        }

        /// <summary>Vuelve al encuadre por defecto.</summary>
        public void ResetToDefaultZone(float transitionDuration = 0f)
        {
            EnterZone(defaultOffset, transitionDuration);
        }
    }
}
