using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
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

        [MenuItem("The Silent Divide/Construir escena de prototipo")]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "Construir escena de prototipo",
                    "Se creará una escena nueva con el blockout y los cuatro módulos.\n\n" +
                    "Si la escena actual tiene cambios sin guardar, se te pedirá guardarlos.",
                    "Construir", "Cancelar"))
                return;

            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            Transform player = BuildPlayer();
            BuildGround();
            BuildCamera(player);
            BuildSurveillanceZone();
            BuildCameraZone();
            BuildSuspicionUI(player);
            BuildLandmarks();

            EditorSceneManager.MarkSceneDirty(scene);
            Selection.activeTransform = player;

            Debug.Log("[The Silent Divide] Escena de prototipo construida. " +
                      "Pulsa Play y muévete con WASD: la barra sube dentro del volumen dorado.");
        }

        private static void BuildGround()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Suelo (blockout 100x100)";
            // El plano primitivo de Unity mide 10x10 unidades con escala 1.
            ground.transform.localScale = Vector3.one * (GroundSize / 10f);
        }

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
        private static void BuildLandmarks()
        {
            var parent = new GameObject("Referencias").transform;

            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI * 2f / 8f;
                var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.name = $"Referencia {i}";
                cube.transform.SetParent(parent, false);
                cube.transform.position =
                    new Vector3(Mathf.Cos(angle) * 20f, 1.5f, Mathf.Sin(angle) * 20f);
                cube.transform.localScale = new Vector3(2f, 3f, 2f);
            }
        }
    }
}
