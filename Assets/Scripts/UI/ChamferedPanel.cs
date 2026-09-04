using UnityEngine;
using UnityEngine.UI;

namespace SilentDivide.UI
{
    /// <summary>
    /// Rectángulo de esquinas cortadas en diagonal, con relleno y contorno independientes.
    /// Es la forma base de todos los botones del kit de UX-UI.
    ///
    /// Se dibuja por malla en vez de con un sprite 9-slice: el corte es diagonal y un 9-slice lo
    /// deformaría al estirarse. Así el mismo componente sirve para un botón ancho del menú y para
    /// una etiqueta pequeña de interacción, sin necesidad de arte.
    /// </summary>
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class ChamferedPanel : MaskableGraphic
    {
        [Tooltip("Tamaño del corte en diagonal de cada esquina, en píxeles.")]
        [SerializeField, Min(0f)] private float chamfer = 14f;

        [Tooltip("Grosor del contorno. Con cero, el panel es solo relleno.")]
        [SerializeField, Min(0f)] private float borderThickness = 2f;

        [SerializeField] private Color borderColor = Color.white;

        /// <summary>Color del contorno. El relleno usa <see cref="Graphic.color"/>.</summary>
        public Color BorderColor
        {
            get => borderColor;
            set { borderColor = value; SetVerticesDirty(); }
        }

        public float Chamfer
        {
            get => chamfer;
            set { chamfer = Mathf.Max(0f, value); SetVerticesDirty(); }
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();

            Rect r = GetPixelAdjustedRect();
            float maxChamfer = Mathf.Min(r.width, r.height) * 0.5f;
            float c = Mathf.Clamp(chamfer, 0f, maxChamfer);

            Vector2[] outer = BuildPath(r, c);

            // Relleno: abanico de triángulos desde el centro del octógono.
            if (color.a > 0f)
                AddFan(vh, outer, color);

            if (borderThickness <= 0f || borderColor.a <= 0f) return;

            // Contorno: el mismo camino desplazado hacia dentro. Al desplazar un polígono convexo
            // una distancia t, los lados rectos se acercan t y el corte diagonal se reduce
            // t·(2−√2), que es la intersección real de las rectas desplazadas.
            float t = Mathf.Min(borderThickness, Mathf.Min(r.width, r.height) * 0.5f);
            Rect innerRect = new Rect(r.x + t, r.y + t, r.width - 2f * t, r.height - 2f * t);
            float innerChamfer = Mathf.Max(0f, c - t * (2f - Mathf.Sqrt(2f)));

            Vector2[] inner = BuildPath(innerRect, innerChamfer);
            AddRing(vh, outer, inner, borderColor);
        }

        /// <summary>Los ocho vértices del octógono, en sentido antihorario desde abajo-izquierda.</summary>
        private static Vector2[] BuildPath(Rect r, float c)
        {
            float xMin = r.xMin, xMax = r.xMax, yMin = r.yMin, yMax = r.yMax;
            return new[]
            {
                new Vector2(xMin + c, yMin),
                new Vector2(xMax - c, yMin),
                new Vector2(xMax,     yMin + c),
                new Vector2(xMax,     yMax - c),
                new Vector2(xMax - c, yMax),
                new Vector2(xMin + c, yMax),
                new Vector2(xMin,     yMax - c),
                new Vector2(xMin,     yMin + c),
            };
        }

        private static void AddFan(VertexHelper vh, Vector2[] path, Color color)
        {
            int center = vh.currentVertCount;

            Vector2 mid = Vector2.zero;
            for (int i = 0; i < path.Length; i++) mid += path[i];
            mid /= path.Length;

            vh.AddVert(mid, color, Vector2.zero);
            for (int i = 0; i < path.Length; i++)
                vh.AddVert(path[i], color, Vector2.zero);

            for (int i = 0; i < path.Length; i++)
            {
                int a = center + 1 + i;
                int b = center + 1 + (i + 1) % path.Length;
                vh.AddTriangle(center, a, b);
            }
        }

        private static void AddRing(VertexHelper vh, Vector2[] outer, Vector2[] inner, Color color)
        {
            int start = vh.currentVertCount;

            for (int i = 0; i < outer.Length; i++)
            {
                vh.AddVert(outer[i], color, Vector2.zero);
                vh.AddVert(inner[i], color, Vector2.zero);
            }

            for (int i = 0; i < outer.Length; i++)
            {
                int o0 = start + i * 2;
                int i0 = o0 + 1;
                int o1 = start + ((i + 1) % outer.Length) * 2;
                int i1 = o1 + 1;

                vh.AddTriangle(o0, o1, i1);
                vh.AddTriangle(o0, i1, i0);
            }
        }
    }
}
