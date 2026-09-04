using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SilentDivide.Player;
using SilentDivide.Suspicion;
using SilentDivide.CameraSystem;
using SilentDivide.Rendering;
using SilentDivide.UI;

namespace SilentDivide.EditorTools
{
    /// <summary>
    /// Construye la escena de prototipo con primitivas: blockout plano, jugador cápsula con
    /// billboard de colores, cámara de ángulo fijo, una zona vigilada y la barra de sospecha.
    ///
    /// Menú: The Silent Divide ▸ Construir escena de prototipo
    ///
    /// Sirve para verificar los cuatro módulos sin ningún arte final. Se puede volver a ejecutar
    /// las veces que haga falta: crea una escena nueva cada vez.
    /// </summary>
    public static class PrototypeSceneBuilder
    {
        // Escala real según docs/diseno/escenarios.md: los escenarios son de 100 × 100 m.
        private const float GroundSize = 100f;

        [MenuItem("The Silent Divide/Construir escena de prototipo", priority = 20)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "Construir escena de prototipo",
                    "Se creará una escena nueva con el blockout y los cuatro módulos, y se " +
                    "guardará en " + SceneCatalog.GameScenePath + ".\n\n" +
                    "Si la escena actual tiene cambios sin guardar, se te pedirá guardarlos.",
                    "Construir", "Cancelar"))
                return;

            Scene scene = BuildScene();
            SceneCatalog.SaveAndRegister(scene, SceneCatalog.GameScenePath);

            Debug.Log("[The Silent Divide] Escena de prototipo construida y guardada en " +
                      SceneCatalog.GameScenePath + ". Pulsa Play y muévete con WASD: la barra sube " +
                      "dentro del volumen dorado.");
        }

        /// <summary>
        /// Monta la escena sin preguntar ni guardarla. Lo usa <see cref="SceneCatalog.BuildAll"/>,
        /// que encadena las dos escenas y no puede lanzar un diálogo por cada una.
        /// </summary>
        internal static Scene BuildScene()
        {
            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            Transform player = BuildPlayer();
            BuildGround();
            BuildCamera(player);
            BuildSurveillanceZone();
            BuildCameraZone();
            BuildSuspicionUI(player);
            BuildLandmarks();
            BuildPlatforms();

            EditorSceneManager.MarkSceneDirty(scene);
            Selection.activeTransform = player;

            return scene;
        }

        /// <summary>
        /// Suelo del blockout, con un damero de 2 × 2 m en tonos de Umbria.
        ///
        /// No es decoración: sobre un plano de un solo color no se puede juzgar la velocidad de
        /// caminata ni si la cámara sigue con retraso, porque no hay nada que se desplace bajo los
        /// pies. Y con la cuadrícula a escala conocida se mide de un vistazo cuánto avanza el
        /// personaje por segundo y cuánto cubre un salto.
        /// </summary>
        private static void BuildGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Suelo (blockout 100x100)";
            // El plano primitivo de Unity mide 10x10 unidades con escala 1.
            ground.transform.localScale = Vector3.one * (GroundSize / 10f);

            PaintCheckered(ground, "Suelo-Umbria", Hex(0x3B3A36), Hex(0x4A413E),
                           GroundSize / TileSize);
        }

        /// <summary>Lado de cada casilla del damero del suelo, en metros.</summary>
        private const float TileSize = 2f;

        private static Transform BuildPlayer()
        {
            var player = new GameObject("Nero");

            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.3f;
            controller.center = new Vector3(0f, 0.9f, 0f);

            player.AddComponent<PlayerMovement>();
            player.AddComponent<SuspicionSystem>();
            player.transform.position = new Vector3(0f, 0.1f, 0f);

            // El plano con el dibujo, hijo del jugador: se orienta a la cámara por su cuenta.
            var billboard = new GameObject("Billboard");
            billboard.transform.SetParent(player.transform, false);
            billboard.transform.localPosition = Vector3.zero;

            var renderer = billboard.AddComponent<SpriteRenderer>();
            renderer.sortingOrder = 10;
            billboard.AddComponent<DirectionalBillboard>();
            billboard.AddComponent<PlaceholderSprites>();

            return player.transform;
        }

        private static void BuildCamera(Transform target)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                var go = new GameObject("Main Camera") { tag = "MainCamera" };
                camera = go.AddComponent<Camera>();
                go.AddComponent<AudioListener>();
            }

            // Ángulo fijo, tipo escena enmarcada. La cámara NUNCA rota durante el juego.
            camera.transform.position = target.position + new Vector3(0f, 12f, -10f);
            camera.transform.rotation = Quaternion.Euler(45f, 0f, 0f);

            var follow = camera.gameObject.AddComponent<FollowCamera>();
            var so = new SerializedObject(follow);
            so.FindProperty("target").objectReferenceValue = target;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void BuildSurveillanceZone()
        {
            var zone = new GameObject("Zona vigilada");
            zone.transform.position = new Vector3(12f, 2f, 8f);

            var box = zone.AddComponent<BoxCollider>();
            box.size = new Vector3(16f, 4f, 12f);
            box.isTrigger = true;

            zone.AddComponent<SurveillanceZone>();

            MarkOnGround(zone.transform, box, "Zona-vigilada", Hex(0xB08D57));
        }

        /// <summary>
        /// Dibuja la huella de un volumen en el suelo.
        ///
        /// Los gizmos de las zonas **solo se ven en la vista de escena**. Jugando, la barra de
        /// sospecha subía y la cámara cambiaba de plano sin que nada en pantalla explicara por qué.
        /// Esta marca no participa en la lógica: la detección sigue siendo del BoxCollider.
        /// </summary>
        private static void MarkOnGround(Transform zone, BoxCollider box, string name, Color color)
        {
            var marker = GameObject.CreatePrimitive(PrimitiveType.Plane);
            marker.name = $"Marca — {name}";
            marker.transform.SetParent(zone, false);

            // Dos centímetros sobre el suelo: a ras de cero, los dos planos parpadean al competir
            // por el mismo píxel de profundidad.
            marker.transform.localPosition = new Vector3(0f, -box.size.y * 0.5f + 0.02f, 0f);
            // El plano primitivo mide 10 × 10 unidades, de ahí la división.
            marker.transform.localScale = new Vector3(box.size.x / 10f, 1f, box.size.z / 10f);

            // Sin colisión: es una marca, no un suelo. Con ella, el personaje caminaría dos
            // centímetros más alto justo al entrar en la zona.
            Object.DestroyImmediate(marker.GetComponent<Collider>());

            Paint(marker, name, color);
        }

        private static void BuildCameraZone()
        {
            var zone = new GameObject("Zona de camara - plano cerrado");
            zone.transform.position = new Vector3(-18f, 2f, 0f);

            var box = zone.AddComponent<BoxCollider>();
            box.size = new Vector3(14f, 4f, 14f);
            box.isTrigger = true;

            var cameraZone = zone.AddComponent<CameraZone>();
            var so = new SerializedObject(cameraZone);
            // Plano más cerrado, como un callejón de Umbria. Mismo ángulo, distinta distancia.
            so.FindProperty("offset").vector3Value = new Vector3(0f, 7f, -6f);
            so.ApplyModifiedPropertiesWithoutUndo();

            MarkOnGround(zone.transform, box, "Zona-de-camara", Hex(0x4A4A7E));
        }

        private static void BuildSuspicionUI(Transform player)
        {
            var canvasGo = new GameObject("Canvas",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.GetComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;

            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                new GameObject("EventSystem",
                    typeof(UnityEngine.EventSystems.EventSystem),
                    typeof(UnityEngine.EventSystems.StandaloneInputModule));
            }

            var background = NewUIImage("Barra de sospecha", canvasGo.transform,
                new Color(0.11f, 0.10f, 0.09f, 0.85f));
            var bgRect = background.rectTransform;
            bgRect.anchorMin = new Vector2(0.5f, 1f);
            bgRect.anchorMax = new Vector2(0.5f, 1f);
            bgRect.pivot     = new Vector2(0.5f, 1f);
            bgRect.anchoredPosition = new Vector2(0f, -32f);
            bgRect.sizeDelta = new Vector2(360f, 22f);

            var fill = NewUIImage("Relleno", background.transform,
                new Color(0.51f, 0.78f, 0.92f));
            var fillRect = fill.rectTransform;
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = new Vector2(3f, 3f);
            fillRect.offsetMax = new Vector2(-3f, -3f);
            fill.type = Image.Type.Filled;
            fill.fillMethod = Image.FillMethod.Horizontal;
            fill.fillAmount = 0f;

            var bar = canvasGo.AddComponent<SuspicionBar>();
            var so = new SerializedObject(bar);
            so.FindProperty("suspicionSystem").objectReferenceValue =
                player.GetComponent<SuspicionSystem>();
            so.FindProperty("fill").objectReferenceValue = fill;
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static Image NewUIImage(string name, Transform parent, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            var image = go.GetComponent<Image>();
            image.color = color;
            return image;
        }

        /// <summary>Cubos de referencia, para tener contra qué juzgar el movimiento y la cámara.</summary>
        /// <summary>
        /// Ocho cubos en círculo alrededor del punto de salida. No son decoración: sin nada contra
        /// lo que comparar, ni el movimiento ni la cámara se pueden juzgar —un plano vacío se ve
        /// igual moviéndose que quieto—.
        ///
        /// Cada uno lleva **su propio color**, de las paletas de `docs/diseno/paletas.md`. Con los
        /// ocho grises era imposible saber hacia dónde miraba la cámara al girar; con colores, cada
        /// dirección tiene una referencia reconocible de un vistazo.
        ///
        /// El recorrido va de los grises de Umbria a los cálidos de Aurea, así que el círculo
        /// también sirve de muestrario de las dos paletas.
        /// </summary>
        private static void BuildLandmarks()
        {
            var parent = new GameObject("Referencias").transform;

            (string name, Color color)[] palette =
            {
                ("Umbria - Negro",     Hex(0x1D1918)),
                ("Umbria - Sombra",    Hex(0x3B3A36)),
                ("Umbria - Madera",    Hex(0xB2A392)),
                ("Umbria - Gris",      Hex(0x83838D)),
                ("Umbria - Neblina",   Hex(0xC4CBD8)),
                ("Aurea - Verde agua", Hex(0x83D1C4)),
                ("Aurea - Cielo",      Hex(0x50C6EB)),
                ("Aurea - Sol",        Hex(0xF6D061)),
            };

            for (int i = 0; i < palette.Length; i++)
            {
                float angle = i * Mathf.PI * 2f / palette.Length;
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = $"Referencia {i} — {palette[i].name}";
                cube.transform.SetParent(parent, false);
                cube.transform.position =
                    new Vector3(Mathf.Cos(angle) * 20f, 1.5f, Mathf.Sin(angle) * 20f);
                cube.transform.localScale = new Vector3(2f, 3f, 2f);

                Paint(cube, palette[i].name, palette[i].color);
            }
        }

        /// <summary>
        /// Escalera de bloques y una plataforma alta, para poder probar el salto.
        ///
        /// Cada escalón sube 0,8 m, la mitad de la altura de salto por defecto. Está medido así a
        /// propósito: con un margen de 2× se sube la escalera entera sin pelearse con el borde, que
        /// es lo que hay que poder enseñar. Si alguien baja `jumpHeight` por debajo de 0,8 la
        /// escalera deja de subirse, y eso es justo la señal de que el salto se quedó corto.
        /// </summary>
        private static void BuildPlatforms()
        {
            var parent = new GameObject("Plataformas").transform;

            const float step = 0.8f;
            Color stone = Hex(0x5A6065);
            Color wood  = Hex(0x514435);

            for (int i = 0; i < 4; i++)
            {
                var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.name = $"Escalón {i + 1}";
                block.transform.SetParent(parent, false);

                float height = step * (i + 1);
                // Cada escalón nace en el suelo y crece hacia arriba: así el bloque se apoya en
                // vez de flotar, y la cara superior queda justo a la altura del escalón.
                block.transform.position = new Vector3(8f + i * 3f, height * 0.5f, -6f);
                block.transform.localScale = new Vector3(2.5f, height, 2.5f);

                Paint(block, "Piedra", stone);
            }

            var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = "Plataforma alta";
            platform.transform.SetParent(parent, false);
            platform.transform.position = new Vector3(22f, 1.9f, -6f);
            platform.transform.localScale = new Vector3(6f, 3.8f, 6f);

            Paint(platform, "Madera", wood);
        }

        // ── Materiales ───────────────────────────────────────────────────────────────────────

        private const string MaterialsFolder = "Assets/Art/Materials";

        /// <summary>
        /// Aplica un material de color plano, guardándolo como asset y reutilizándolo entre
        /// objetos y entre reconstrucciones.
        ///
        /// Los materiales tienen que ser assets y no objetos sueltos: un material creado en
        /// memoria y asignado a un objeto de escena se pierde al recargar, y los cubos aparecen en
        /// magenta la siguiente vez que se abre la escena.
        /// </summary>
        private static void Paint(GameObject target, string materialName, Color color)
        {
            var renderer = target.GetComponent<Renderer>();
            if (renderer == null) return;

            SceneCatalog.EnsureFolder(MaterialsFolder);

            string safeName = materialName.Replace(" - ", "-").Replace(" ", "-");
            string path = $"{MaterialsFolder}/{safeName}.mat";

            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                // El shader se toma del material por defecto de la primitiva en vez de buscarlo
                // por nombre: «Standard» solo existe en Built-in, y el proyecto puede migrar a URP
                // sin que esto tenga que enterarse.
                material = new Material(renderer.sharedMaterial.shader);
                material.color = color;
                AssetDatabase.CreateAsset(material, path);
            }
            else
            {
                material.color = color;
                EditorUtility.SetDirty(material);
            }

            renderer.sharedMaterial = material;
        }

        /// <summary>
        /// Material de damero de dos colores. La textura son 2 × 2 píxeles repetidos por la
        /// superficie: una imagen mínima con filtrado por punto da un damero perfecto a cualquier
        /// tamaño, sin arte y sin peso.
        /// </summary>
        private static void PaintCheckered(
            GameObject target, string materialName, Color a, Color b, float tiles)
        {
            var renderer = target.GetComponent<Renderer>();
            if (renderer == null) return;

            SceneCatalog.EnsureFolder(MaterialsFolder);

            string texturePath  = $"{MaterialsFolder}/{materialName}.asset";
            string materialPath = $"{MaterialsFolder}/{materialName}.mat";

            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (texture == null)
            {
                texture = new Texture2D(2, 2, TextureFormat.RGBA32, false)
                {
                    // Por punto y no bilineal: con filtrado suave, dos píxeles estirados por cien
                    // metros dan un degradado, no un damero.
                    filterMode = FilterMode.Point,
                    wrapMode = TextureWrapMode.Repeat,
                };
                AssetDatabase.CreateAsset(texture, texturePath);
            }

            texture.SetPixels(new[] { a, b, b, a });
            texture.Apply();
            EditorUtility.SetDirty(texture);

            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                material = new Material(renderer.sharedMaterial.shader);
                AssetDatabase.CreateAsset(material, materialPath);
            }

            material.color = Color.white;   // el color lo pone la textura, no el tinte
            material.mainTexture = texture;
            // La textura tiene dos casillas por lado, así que media repetición por casilla.
            material.mainTextureScale = Vector2.one * (tiles * 0.5f);
            EditorUtility.SetDirty(material);

            renderer.sharedMaterial = material;
        }

        private static Color Hex(int rgb) => new Color(
            ((rgb >> 16) & 0xFF) / 255f,
            ((rgb >> 8)  & 0xFF) / 255f,
            ( rgb        & 0xFF) / 255f);
    }
}
