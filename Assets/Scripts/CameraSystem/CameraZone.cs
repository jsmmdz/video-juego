using UnityEngine;

namespace SilentDivide.CameraSystem
{
    /// <summary>
    /// Zona con encuadre propio. El desplazamiento es por zona y no global: permite un plano más
    /// cerrado en un callejón de Umbria y uno más abierto en la plaza de Aurea, sin que la cámara
    /// gire jamás durante el juego.
    ///
    /// Las dos perspectivas del pitch —tercera persona y vista central— se consiguen como valores
    /// distintos de este mismo parámetro, no como dos sistemas de cámara.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class CameraZone : MonoBehaviour
    {
        [Tooltip("Vector desde el jugador hasta la cámara mientras se está en esta zona.")]
        [SerializeField] private Vector3 offset = new Vector3(0f, 12f, -10f);

        [Tooltip("Segundos de transición al entrar. 0 = corte seco (por defecto).")]
        [SerializeField, Min(0f)] private float transitionDuration = 0f;

        [SerializeField] private FollowCamera followCamera;

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void Awake()
        {
            if (followCamera == null)
                followCamera = FindFirstObjectByType<FollowCamera>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (followCamera == null || followCamera.Target == null) return;
            if (other.transform != followCamera.Target) return;

            followCamera.EnterZone(offset, transitionDuration);
        }

        private void OnDrawGizmosSelected()
        {
            Collider col = GetComponent<Collider>();
            if (col == null) return;

            Gizmos.color = new Color(0.31f, 0.78f, 0.92f, 0.8f);
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
            Gizmos.DrawLine(col.bounds.center, col.bounds.center + offset);
            Gizmos.DrawWireSphere(col.bounds.center + offset, 0.5f);
        }
    }
}
