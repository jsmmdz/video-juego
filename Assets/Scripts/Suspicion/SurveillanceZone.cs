using UnityEngine;

namespace SilentDivide.Suspicion
{
    /// <summary>
    /// Marca un volumen como zona vigilada. En el prototipo son triggers colocados a mano sobre
    /// el blockout; más adelante los sustituirán conos de visión de guardias y alcance de cámaras.
    ///
    /// Este componente no contiene lógica: solo identifica el volumen. Quien lleva la cuenta es
    /// <see cref="SuspicionSystem"/>, para que toda la mecánica viva en un único sitio.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public sealed class SurveillanceZone : MonoBehaviour
    {
        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnDrawGizmos()
        {
            Collider col = GetComponent<Collider>();
            if (col == null) return;

            Gizmos.color = new Color(0.85f, 0.72f, 0.42f, 0.25f);
            Bounds b = col.bounds;
            Gizmos.DrawCube(b.center, b.size);
            Gizmos.color = new Color(0.85f, 0.72f, 0.42f, 0.9f);
            Gizmos.DrawWireCube(b.center, b.size);
        }
    }
}
