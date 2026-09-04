using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SilentDivide.UI;

namespace SilentDivide.EditorTools
{
    /// <summary>
    /// Construye la escena del menú principal a partir del kit de UX-UI del board.
    /// Menú: The Silent Divide ▸ Construir menú principal
    ///
    /// Igual que el constructor del prototipo: la escena se genera, no se versiona, para no
    /// arrastrar conflictos de YAML de Unity.
    /// </summary>
    public static class MainMenuSceneBuilder
    {
        private const float ButtonWidth  = 420f;
        private const float ButtonHeight = 62f;
        private const float ButtonGap    = 14f;

        [MenuItem("The Silent Divide/Construir menú principal")]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "Construir menú principal",
                    "Se creará una escena nueva con el menú.\n\n" +
                    "Si la escena actual tiene cambios sin guardar, se te pedirá guardarlos.",
                    "Construir", "Cancelar"))
                return;

            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // La cámara solo pinta el fondo: el menú es Screen Space Overlay.
            var camera = Camera.main;
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = UITheme.BackgroundDeep;
            }

            GameObject canvasGo = BuildCanvas();
            BuildBackdrop(canvasGo.transform);
            BuildTitle(canvasGo.transform);

            var buttons = new List<MenuButton>();
            MenuButton newGame  = BuildButton(canvasGo.transform, "NUEVA PARTIDA", 0, buttons);
            MenuButton continueB = BuildButton(canvasGo.transform, "CONTINUAR",    1, buttons);
            MenuButton settings = BuildButton(canvasGo.transform, "AJUSTES",       2, buttons);
            MenuButton quit     = BuildButton(canvasGo.transform, "SALIR",         3, buttons);

            BuildHint(canvasGo.transform);

            var controller = canvasGo.AddComponent<MainMenuController>();
            var so = new SerializedObject(controller);
            SerializedProperty list = so.FindProperty("buttons");
            list.arraySize = buttons.Count;
            for (int i = 0; i < buttons.Count; i++)
                list.GetArrayElementAtIndex(i).objectReferenceValue = buttons[i];

            so.FindProperty("newGameButton").objectReferenceValue  = newGame;
            so.FindProperty("continueButton").objectReferenceValue = continueB;
            so.FindProperty("settingsButton").objectReferenceValue = settings;
            so.FindProperty("quitButton").objectReferenceValue     = quit;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);

            Debug.Log("[The Silent Divide] Menú principal construido. " +
                      "«Continuar» aparece deshabilitado porque no hay partida guardada: es el " +
                      "comportamiento correcto.");
        }

        private static GameObject BuildCanvas()
        {
            var go = new GameObject("Canvas - Menu",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                new GameObject("EventSystem",
                    typeof(UnityEngine.EventSystems.EventSystem),
                    typeof(UnityEngine.EventSystems.StandaloneInputModule));
            }

            return go;
        }

        /// <summary>Fondo plano. Sustituible por la ilustración de la brecha entre mundos.</summary>
        private static void BuildBackdrop(Transform parent)
        {
            var go = new GameObject("Fondo", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;

            go.GetComponent<Image>().color = UITheme.Background;
        }

        private static void BuildTitle(Transform parent)
        {
            TextMeshProUGUI title = NewLabel(parent, "Titulo", "THE SILENT DIVIDE", 64f, 18f);
            title.color = UITheme.TextPrimary;
            title.fontStyle = FontStyles.Bold;

            var rect = title.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot     = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(1000f, 90f);
            rect.anchoredPosition = new Vector2(0f, 250f);
        }

        private static MenuButton BuildButton(
            Transform parent, string text, int index, List<MenuButton> collected)
        {
            var go = new GameObject($"Boton - {text}",
                typeof(RectTransform), typeof(CanvasRenderer), typeof(ChamferedPanel));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot     = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(ButtonWidth, ButtonHeight);
            rect.anchoredPosition = new Vector2(0f, 90f - index * (ButtonHeight + ButtonGap));

            var panel = go.GetComponent<ChamferedPanel>();
            panel.color = Color.clear;
            panel.BorderColor = UITheme.ButtonIdle;

            TextMeshProUGUI label = NewLabel(go.transform, "Etiqueta", text, 26f, 12f);
            var labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            var button = go.AddComponent<MenuButton>();
            var so = new SerializedObject(button);
            so.FindProperty("label").objectReferenceValue = label;
            so.FindProperty("panel").objectReferenceValue = panel;
            so.ApplyModifiedPropertiesWithoutUndo();

            collected.Add(button);
            return button;
        }

        private static void BuildHint(Transform parent)
        {
            TextMeshProUGUI hint = NewLabel(
                parent, "Ayuda", "FLECHAS PARA NAVEGAR   ·   ENTER PARA CONFIRMAR", 16f, 8f);
            hint.color = UITheme.TextSecondary;

            var rect = hint.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(0.5f, 0f);
            rect.pivot     = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(900f, 40f);
            rect.anchoredPosition = new Vector2(0f, 48f);
        }

        /// <summary>
        /// Etiqueta con espaciado entre letras: el kit lo usa de forma marcada en todo el texto de
        /// interfaz, y es lo que le da su carácter.
        /// </summary>
        private static TextMeshProUGUI NewLabel(
            Transform parent, string name, string text, float size, float tracking)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var label = go.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = size;
            label.characterSpacing = tracking;
            label.alignment = TextAlignmentOptions.Center;
            label.color = UITheme.TextPrimary;
            label.raycastTarget = false;   // que no robe los eventos de ratón al botón

            return label;
        }
    }
}
