using UnityEngine;

namespace SilentDivide.Player
{
    /// <summary>
    /// Módulo 1 — Movimiento del jugador.
    /// Flowchart: board de Figma, nodo 50:170. Especificación: docs/tecnico/01-movimiento.md
    ///
    /// Las dos decisiones del diagrama NO son alternativas entre sí: la primera define si hay
    /// dirección horizontal, la segunda define cómo se calcula la caída. Sus resultados se suman
    /// en una sola instrucción de movimiento que se aplica UNA ÚNICA VEZ por fotograma — por eso
    /// el personaje sigue cayendo aunque esté quieto.
    ///
    /// **El salto es una extensión, no está en el flowchart.** Lo pide el roadmap para la
    /// verticalidad de Umbria (decisiones abiertas #6). Encaja sin tocar la estructura del
    /// diagrama porque solo cambia el valor de la velocidad vertical: el resto —una sola llamada
    /// de movimiento, el empuje al suelo distinto de cero— se mantiene igual.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerMovement : MonoBehaviour
    {
        [Header("Avance")]
        [SerializeField, Min(0f)] private float speed = 4f;
        [SerializeField, Min(0f)] private float turnSpeed = 720f;

        [Header("Caída")]
        [Tooltip("Aceleración de caída. Negativa.")]
        [SerializeField] private float gravity = -20f;

        [Tooltip("Empuje hacia abajo al estar apoyado. NO debe ser cero: es lo que mantiene al " +
                 "personaje pegado al suelo y hace que isGrounded siga dando verdadero en " +
                 "fotogramas sucesivos. Con cero, despega en rampas y bajadas.")]
        [SerializeField] private float groundedPull = -2f;

        [Header("Salto")]
        [Tooltip("Altura máxima del salto, en metros. La velocidad inicial se deduce de ella y de " +
                 "la gravedad, así que se puede retocar la gravedad sin recalcular el salto.")]
        [SerializeField, Min(0f)] private float jumpHeight = 1.6f;

        [Tooltip("Margen tras dejar el suelo en el que todavía se admite el salto. Sin él, saltar " +
                 "justo al pisar el borde de una plataforma se pierde, y se lee como que el juego " +
                 "no responde en vez de como un error propio.")]
        [SerializeField, Min(0f)] private float coyoteTime = 0.12f;

        private CharacterController controller;
        private float verticalVelocity;

        /// <summary>Tiempo desde el último fotograma apoyado. Es lo que mide el margen de salto.</summary>
        private float timeSinceGrounded;

        /// <summary>Cierto mientras el personaje no está apoyado. Lo usará el sistema de sigilo.</summary>
        public bool IsAirborne { get; private set; }

        /// <summary>
        /// Última dirección de avance conocida. Se conserva cuando no hay entrada: el personaje
        /// no vuelve a una orientación por defecto, se queda mirando a donde iba.
        /// El módulo 4 (vista del personaje) depende de esto para elegir el dibujo.
        /// </summary>
        public Vector3 FacingDirection { get; private set; } = Vector3.forward;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            FacingDirection = transform.forward;
        }

        private void Update()
        {
            // ── Leer entrada del teclado (horizontal + profundidad) ───────────────────────────
            float horizontal = Input.GetAxisRaw("Horizontal");   // A / D
            float depth      = Input.GetAxisRaw("Vertical");     // W / S

            // Una sola lectura por fotograma: GetButtonDown solo es cierto en el fotograma de la
            // pulsación, así que consultarlo dos veces daría resultados distintos.
            bool jumpPressed = Input.GetButtonDown("Jump");       // Espacio

            // ── Construir vector dirección ────────────────────────────────────────────────────
            Vector3 direction = new Vector3(horizontal, 0f, depth);

            // ── ¿Hay entrada? ─────────────────────────────────────────────────────────────────
            // Normalizamos SOLO dentro de esta rama: normalizar el vector cero es indefinido.
            if (direction.sqrMagnitude > 0.0001f)
            {
                direction.Normalize();
                RotateTowards(direction);
                FacingDirection = direction;
            }
            else
            {
                // Sin entrada: dirección = 0 y el personaje conserva su orientación.
                direction = Vector3.zero;
            }

            // ── ¿Tocando el piso? ─────────────────────────────────────────────────────────────
            // Rama independiente de la anterior: solo decide cómo se calcula la caída.
            if (controller.isGrounded)
            {
                verticalVelocity = groundedPull;          // reiniciar la caída (valor mínimo hacia abajo)
                timeSinceGrounded = 0f;
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;   // acumular gravedad: la caída se acelera
                timeSinceGrounded += Time.deltaTime;
            }

            IsAirborne = !controller.isGrounded;

            // ── ¿Salta? ───────────────────────────────────────────────────────────────────────
            // Va DESPUÉS de la rama anterior a propósito: si fuese antes, el bloque de arriba
            // machacaría el impulso con groundedPull en el mismo fotograma y el salto no saldría.
            if (jumpPressed && timeSinceGrounded <= coyoteTime)
            {
                verticalVelocity = JumpVelocity;

                // Consume el margen: sin esto, el mismo apoyo daría un segundo salto en el aire
                // mientras durase el margen.
                timeSinceGrounded = coyoteTime + 1f;
            }

            // ── Sumar avance + caída, en una sola instrucción ─────────────────────────────────
            Vector3 displacement = direction * speed;
            displacement.y = verticalVelocity;

            // ── Mover al personaje: una única vez por fotograma ───────────────────────────────
            controller.Move(displacement * Time.deltaTime);
        }

        /// <summary>
        /// Velocidad inicial para alcanzar <c>jumpHeight</c> con la gravedad actual: v = √(2·g·h).
        /// Se calcula en vez de exponerse directamente porque una velocidad suelta no dice nada,
        /// y al cambiar la gravedad dejaría de corresponder con la altura que se quería.
        /// </summary>
        private float JumpVelocity => gravity < 0f
            ? Mathf.Sqrt(2f * -gravity * jumpHeight)
            : 0f;

        private void RotateTowards(Vector3 direction)
        {
            Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, target, turnSpeed * Time.deltaTime);
        }
    }
}
