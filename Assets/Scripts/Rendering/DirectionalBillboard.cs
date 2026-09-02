using UnityEngine;
using SilentDivide.Player;

namespace SilentDivide.Rendering
{
    /// <summary>
    /// Módulo 4 — Vista del personaje (billboard direccional).
    /// Flowcharts: board de Figma, nodos 50:222 (lógica) y 51:254 (rosa de sectores).
    /// Especificación: docs/tecnico/04-vista-personaje.md
    ///
    /// El personaje no es un modelo 3D: es un plano con un dibujo. Este componente hace dos cosas
    /// para que ese plano se lea como un personaje dentro de un mundo tridimensional:
    ///   1. Lo gira hacia la cámara, para que nunca se vea de canto.
    ///   2. Elige cuál de los CUATRO dibujos mostrar, según hacia dónde camina respecto a la cámara.
    ///
    /// 4 dibujos → 6 direcciones: frontal y trasera se usan tal cual; lateral y ¾ se espejan para
    /// servir a ambos lados. Es lo que reduce a la mitad el arte necesario por personaje y atuendo.
    ///
    /// Corre en LateUpdate DESPUÉS de la cámara (ver ExecutionOrder): el ángulo se mide respecto a
    /// ella.
    /// </summary>
    [DefaultExecutionOrder(ExecutionOrder.Billboard)]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class DirectionalBillboard : MonoBehaviour
    {
        /// <summary>Los cuatro dibujos, en el orden de la rosa de sectores.</summary>
        [System.Serializable]
        public struct SpriteSet
        {
            [Tooltip("Viene hacia la cámara.")]      public Sprite frontal;
            [Tooltip("¾ de sesgo. Se espeja.")]      public Sprite threeQuarter;
            [Tooltip("De perfil. Se espeja.")]       public Sprite lateral;
            [Tooltip("Se aleja de la cámara.")]      public Sprite back;
        }

        [Header("Dibujos")]
        [SerializeField] private SpriteSet sprites;

        [Header("Referencias")]
        [Tooltip("De dónde sale la dirección de avance. Si se deja vacío, se busca en el padre.")]
        [SerializeField] private PlayerMovement movement;

        [Tooltip("Cámara contra la que se mide el sector. Si se deja vacío, se usa Camera.main.")]
        [SerializeField] private Camera viewCamera;

        [Header("Ajuste")]
        [Tooltip("Grados extra que hay que superar para abandonar el sector actual. Evita que un " +
                 "personaje caminando justo sobre una frontera de 60° alterne entre dos dibujos.")]
        [SerializeField, Range(0f, 20f)] private float hysteresis = 5f;

        private const int SectorCount = 6;
        private const float SectorSize = 360f / SectorCount;   // 60°

        private SpriteRenderer spriteRenderer;
        private int currentSector = -1;   // -1 fuerza la primera asignación

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (movement == null) movement = GetComponentInParent<PlayerMovement>();
            if (viewCamera == null) viewCamera = Camera.main;
        }

        /// <summary>
        /// Sustituye los cuatro dibujos en tiempo de ejecución. Lo usa
        /// <see cref="PlaceholderSprites"/> mientras no hay arte final.
        /// </summary>
        public void SetSprites(SpriteSet set)
        {
            sprites = set;
            currentSector = -1;   // fuerza la reasignación en el próximo fotograma
        }

        private void LateUpdate()
        {
            if (viewCamera == null || movement == null) return;

            // ── Girar el plano a la cámara, para no verlo de canto ───────────────────────────
            // Solo en el plano horizontal: inclinar el sprite con la cámara lo haría "acostarse".
            Vector3 toCamera = viewCamera.transform.forward;
            toCamera.y = 0f;
            if (toCamera.sqrMagnitude > 0.0001f)
                transform.rotation = Quaternion.LookRotation(toCamera, Vector3.up);

            // ── Medir hacia dónde camina: el ángulo respecto a la cámara ─────────────────────
            Vector3 facing = movement.FacingDirection;
            facing.y = 0f;
            if (facing.sqrMagnitude < 0.0001f) return;   // sin dirección conocida, se conserva el dibujo

            float signed = Vector3.SignedAngle(toCamera, facing, Vector3.up);
            float angle = (signed + 360f) % 360f;   // 0° = se aleja de la cámara · 180° = viene hacia ella

            // ── Ubicarlo en un sector (6 sectores de 60°) ────────────────────────────────────
            // El desplazamiento de medio sector centra el sector 0 en 0°.
            int sector = Mathf.FloorToInt(((angle + SectorSize * 0.5f) % 360f) / SectorSize);

            // ── ¿Cambió de sector desde el anterior? ─────────────────────────────────────────
            // No es una optimización cosmética: reasignar la textura cada fotograma reiniciaría
            // la animación del dibujo en cada fotograma.
            if (currentSector >= 0)
            {
                // Histéresis: hay que alejarse del centro del sector actual más de medio sector
                // más el margen para que el cambio se acepte.
                float distanceFromCurrentCenter =
                    Mathf.Abs(Mathf.DeltaAngle(angle, currentSector * SectorSize));

                if (distanceFromCurrentCenter <= SectorSize * 0.5f + hysteresis) return;
                if (sector == currentSector) return;
            }

            // ── Cambiar el dibujo mostrado: espaldas · perfil · ¾ · frente ───────────────────
            ApplySector(sector);
            currentSector = sector;
        }

        private void ApplySector(int sector)
        {
            // Sector 0 está centrado en 0°, que es el personaje alejándose de la cámara.
            switch (sector)
            {
                case 0: Show(sprites.back,         mirrored: false); break;   //   -30° ..   30°
                case 1: Show(sprites.threeQuarter, mirrored: false); break;   //    30° ..   90°
                case 2: Show(sprites.lateral,      mirrored: false); break;   //    90° ..  150°
                case 3: Show(sprites.frontal,      mirrored: false); break;   //   150° ..  210°
                case 4: Show(sprites.lateral,      mirrored: true);  break;   //   210° ..  270°
                case 5: Show(sprites.threeQuarter, mirrored: true);  break;   //   270° ..  330°
            }
        }

        private void Show(Sprite sprite, bool mirrored)
        {
            if (sprite != null) spriteRenderer.sprite = sprite;

            // El espejado es en X del renderer, NO una rotación de 180°: rotar giraría también el
            // billboard y lo pondría de espaldas a la cámara.
            spriteRenderer.flipX = mirrored;
        }
    }
}
