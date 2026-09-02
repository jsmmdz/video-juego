using UnityEngine;

namespace SilentDivide.Rendering
{
    /// <summary>
    /// Genera cuatro rectángulos de color distinto y los inyecta en el
    /// <see cref="DirectionalBillboard"/>, para poder verificar el módulo completo sin arte final.
    ///
    /// El prototipo se arma con primitivas mientras llegan los assets de ilustración: mientras este
    /// componente esté activo, el color del plano cambia al cruzar cada sector, que es exactamente
    /// lo que hay que poder ver para validar el módulo.
    ///
    /// Se elimina cuando entren los sprites reales de Nero (4 dibujos × 2 atuendos).
    /// </summary>
    [RequireComponent(typeof(DirectionalBillboard))]
    public sealed class PlaceholderSprites : MonoBehaviour
    {
        [SerializeField] private Color frontal      = new Color(0.93f, 0.85f, 0.45f);  // amarillo
        [SerializeField] private Color threeQuarter = new Color(0.51f, 0.78f, 0.92f);  // celeste
        [SerializeField] private Color lateral      = new Color(0.55f, 0.75f, 0.45f);  // verde
        [SerializeField] private Color back         = new Color(0.45f, 0.45f, 0.52f);  // gris

        private void Awake()
        {
            var billboard = GetComponent<DirectionalBillboard>();
            billboard.SetSprites(new DirectionalBillboard.SpriteSet
            {
                frontal      = SolidSprite(frontal),
                threeQuarter = SolidSprite(threeQuarter),
                lateral      = SolidSprite(lateral),
                back         = SolidSprite(back),
            });
        }

        /// <summary>
        /// Sprite de un solo color. Se dibuja asimétrico a propósito —una muesca en el borde
        /// derecho— para que el espejado de los sectores laterales y ¾ sea visible en pruebas.
        /// </summary>
        private static Sprite SolidSprite(Color color)
        {
            const int width = 32, height = 64;
            var texture = new Texture2D(width, height) { filterMode = FilterMode.Point };

            for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                bool notch = x > width - 8 && y > height / 2 - 4 && y < height / 2 + 4;
                texture.SetPixel(x, y, notch ? Color.black : color);
            }

            texture.Apply();
            return Sprite.Create(
                texture, new Rect(0, 0, width, height), new Vector2(0.5f, 0f), pixelsPerUnit: 32f);
        }
    }
}
