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

        private CharacterController controller;
        private float verticalVelocity;

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
            }
            else
            {
                verticalVelocity += gravity * Time.deltaTime;   // acumular gravedad: la caída se acelera
            }

            // ── Sumar avance + caída, en una sola instrucción ─────────────────────────────────
            Vector3 displacement = direction * speed;
            displacement.y = verticalVelocity;

            // ── Mover al personaje: una única vez por fotograma ───────────────────────────────
            controller.Move(displacement * Time.deltaTime);
        }

        private void RotateTowards(Vector3 direction)
        {
            Quaternion target = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, target, turnSpeed * Time.deltaTime);
        }
    }
}
