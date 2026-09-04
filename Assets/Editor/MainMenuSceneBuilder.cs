using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SilentDivide.UI;

namespace SilentDivide.EditorTools
{
    /// <summary>
    /// Construye la escena de la pantalla de inicio a partir del mockup definitivo del kit de UX-UI.
    /// Menú: The Silent Divide ▸ Construir menú principal
    ///
    /// Igual que el constructor del prototipo: la escena se genera, no se versiona, para no
    /// arrastrar conflictos de YAML de Unity.
    ///
    /// **Las medidas son las del mockup**, expresadas como fracción de la pantalla y no en píxeles
    /// sueltos: la ilustración manda la composición, así que la columna tiene que caer siempre en
    /// el mismo sitio del cuadro sea cual sea la resolución.
    /// </summary>
    public static class MainMenuSceneBuilder
    {
        // Resolución de referencia. Las fracciones de abajo se miden sobre el mockup (700 × 419).
        private const float RefWidth  = 1920f;
        private const float RefHeight = 1080f;

        // ── Composición, en fracciones del mockup ────────────────────────────────────────────
        private const float PanelWidthFrac  = 0.271f;   // ancho del velo oscuro de la izquierda
        private const float ColumnLeftFrac  = 0.111f;   // margen izquierdo del título y los filetes
        private const float ColumnRightFrac = 0.238f;   // donde termina el filete bajo cada opción

        private const float TitleTopFrac  = 0.100f;     // borde superior de «THE»
        private const float TitleLineFrac = 0.112f;     // salto entre las tres líneas del título

        private const float FirstOptionFrac = 0.588f;   // borde superior de «Jugar»
        private const float OptionStepFrac  = 0.093f;   // salto entre opciones

        private const float MoonXFrac = 0.977f;         // el círculo claro de la esquina superior
        private const float MoonYFrac = 0.041f;
        private const float MoonSizeFrac = 0.020f;

        private static float ColumnWidth => (ColumnRightFrac - ColumnLeftFrac) * RefWidth;
        private static float ColumnLeft  => ColumnLeftFrac * RefWidth;

        // Carpeta de la ilustración de fondo. Se coge la primera imagen que haya dentro, sea cual
        // sea su nombre y su formato: pedir un nombre exacto es una fuente de fallos silenciosos.
        private const string BackdropFolder = "Assets/Art/UI/Menu";

        [MenuItem("The Silent Divide/Construir menú principal", priority = 21)]
        public static void Build()
        {
            if (!EditorUtility.DisplayDialog(
                    "Construir pantalla de inicio",
                    "Se creará una escena nueva con el menú, y se guardará en " +
                    SceneCatalog.MenuScenePath + ".\n\n" +
                    "Si la escena actual tiene cambios sin guardar, se te pedirá guardarlos.",
                    "Construir", "Cancelar"))
                return;

            Scene scene = BuildScene();
            SceneCatalog.SaveAndRegister(scene, SceneCatalog.MenuScenePath);

            Debug.Log("[The Silent Divide] Pantalla de inicio construida y guardada en " +
                      SceneCatalog.MenuScenePath + ". Flechas o ratón para navegar, Enter para " +
                      "confirmar, Escape para salir de Ajustes.");
        }

        /// <summary>
        /// Monta la escena sin preguntar ni guardarla. Lo usa <see cref="SceneCatalog.BuildAll"/>,
        /// que encadena las dos escenas y no puede lanzar un diálogo por cada una.
        /// </summary>
        internal static Scene BuildScene()
        {
            var scene = EditorSceneManager.NewScene(
                NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // La cámara solo pinta el hueco: el menú es Screen Space Overlay.
            var camera = Camera.main;
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = UITheme.BackgroundDeep;
            }

            GameObject canvasGo = BuildCanvas();
            BuildBackdrop(canvasGo.transform);
            BuildMoon(canvasGo.transform);

            GameObject menuScreen = BuildMenuScreen(canvasGo.transform, out MenuButton play,
                                                    out MenuButton settings, out List<MenuEntry> menuItems);
            SettingsPanel settingsPanel = BuildSettingsScreen(canvasGo.transform);

            // El escalado de texto de accesibilidad recorre todo el canvas, Ajustes incluido.
            canvasGo.AddComponent<TextScaler>();

            var controller = canvasGo.AddComponent<MainMenuController>();
            var so = new SerializedObject(controller);
            SerializedProperty list = so.FindProperty("items");
            list.arraySize = menuItems.Count;
            for (int i = 0; i < menuItems.Count; i++)
                list.GetArrayElementAtIndex(i).objectReferenceValue = menuItems[i];

            so.FindProperty("playButton").objectReferenceValue     = play;
            so.FindProperty("settingsButton").objectReferenceValue = settings;
            so.FindProperty("rootScreen").objectReferenceValue     = menuScreen;
            so.FindProperty("settingsPanel").objectReferenceValue  = settingsPanel;
            so.ApplyModifiedPropertiesWithoutUndo();

            EditorSceneManager.MarkSceneDirty(scene);

            return scene;
        }

        // ── Armazón ──────────────────────────────────────────────────────────────────────────

        private static GameObject BuildCanvas()
        {
            var go = new GameObject("Canvas - Inicio",
                typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));

            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(RefWidth, RefHeight);
            scaler.matchWidthOrHeight = 0.5f;

            if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                new GameObject("EventSystem",
                    typeof(UnityEngine.EventSystems.EventSystem),
                    typeof(UnityEngine.EventSystems.StandaloneInputModule));
            }

            return go;
        }

        /// <summary>
        /// Ilustración a pantalla completa. Se recorta en vez de deformarse
        /// (<c>EnvelopeParent</c>): el callejón tiene que seguir leyéndose en 16:10 y en 21:9.
        /// </summary>
        private static void BuildBackdrop(Transform parent)
        {
            GameObject go = NewGraphic(parent, "Ilustración", out Image image);
            Stretch(go.GetComponent<RectTransform>());
            image.raycastTarget = false;

            Sprite sprite = LoadBackdrop();
            if (sprite == null)
            {
                image.color = UITheme.Background;
                Debug.LogWarning($"[The Silent Divide] No hay ninguna imagen en '{BackdropFolder}'. " +
                                 "El menú se construye sobre color plano; deja ahí el PNG del " +
                                 "callejón y vuelve a construir la escena.");
                return;
            }

            image.sprite = sprite;
            image.color  = Color.white;

            var fitter = go.AddComponent<AspectRatioFitter>();
            fitter.aspectMode  = AspectRatioFitter.AspectMode.EnvelopeParent;
            fitter.aspectRatio = sprite.rect.width / sprite.rect.height;
        }

        /// <summary>
        /// Primera imagen de la carpeta de fondo, importada como Sprite.
        ///
        /// Unity importa las imágenes como Texture por defecto, y una Texture no se puede asignar a
        /// un <c>Image</c> de interfaz. Antes eso obligaba a cambiarlo a mano en el inspector y, si
        /// se olvidaba, el constructor decía que el archivo faltaba aunque estuviera ahí. Ahora se
        /// corrige el importador y se reimporta.
        /// </summary>
        private static Sprite LoadBackdrop()
        {
            if (!AssetDatabase.IsValidFolder(BackdropFolder)) return null;

            string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { BackdropFolder });
            if (guids.Length == 0) return null;

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);

            if (AssetImporter.GetAtPath(path) is TextureImporter importer &&
                importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.SaveAndReimport();
                Debug.Log($"[The Silent Divide] '{path}' se ha reimportado como Sprite para poder " +
                          "usarlo de fondo.");
            }

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        /// <summary>Círculo claro de la esquina superior derecha del mockup. Es decorativo.</summary>
        private static void BuildMoon(Transform parent)
        {
            GameObject go = NewGraphic(parent, "Círculo", out Image image);
            image.sprite = CircleSprite();
            image.color  = UITheme.TextPrimary;
            image.raycastTarget = false;

            float size = MoonSizeFrac * RefWidth;
            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot     = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(size, size);
            rect.anchoredPosition = new Vector2(MoonXFrac * RefWidth, -MoonYFrac * RefHeight);
        }

        /// <summary>Velo oscuro de la columna, con el borde derecho difuminado como en el mockup.</summary>
        private static void BuildScrim(Transform parent, float widthFrac)
        {
            GameObject go = NewGraphic(parent, "Velo", out Image image);
            image.color = UITheme.PanelScrim;
            image.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(widthFrac, 1f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;

            // Degradado de salida: sin él, el velo corta la ilustración con una línea recta.
            GameObject fadeGo = NewGraphic(parent, "Velo - degradado", out Image fade);
            fade.sprite = HorizontalFadeSprite();
            fade.color  = UITheme.PanelScrim;
            fade.raycastTarget = false;

            var fadeRect = fadeGo.GetComponent<RectTransform>();
            fadeRect.anchorMin = new Vector2(widthFrac, 0f);
            fadeRect.anchorMax = new Vector2(widthFrac, 1f);
            fadeRect.pivot     = new Vector2(0f, 0.5f);
            fadeRect.sizeDelta = new Vector2(0.06f * RefWidth, 0f);
            fadeRect.anchoredPosition = Vector2.zero;
        }

        // ── Pantalla de inicio ───────────────────────────────────────────────────────────────

        private static GameObject BuildMenuScreen(
            Transform parent, out MenuButton play, out MenuButton settings, out List<MenuEntry> items)
        {
            var screen = new GameObject("Pantalla - Inicio", typeof(RectTransform));
            screen.transform.SetParent(parent, false);
            Stretch(screen.GetComponent<RectTransform>());

            BuildScrim(screen.transform, PanelWidthFrac);
            BuildTitle(screen.transform);

            items = new List<MenuEntry>();
            play     = BuildOptionRow(screen.transform, "Jugar",   0, items);
            settings = BuildOptionRow(screen.transform, "Ajustes", 1, items);

            return screen;
        }

        /// <summary>
        /// «THE / SILENT / DIVIDE» en tres líneas, alineadas a la izquierda de la columna.
        /// Son tres etiquetas y no un texto de tres renglones porque «THE» tiene otro cuerpo y otro
        /// espaciado, y el interlineado del mockup es más cerrado que el que da TextMeshPro solo.
        ///
        /// El título **desborda** el velo a propósito: en el mockup «SILENT» y «DIVIDE» se salen
        /// sobre la ilustración.
        /// </summary>
        private static void BuildTitle(Transform parent)
        {
            TitleLine(parent, "Título - THE",    "THE",     78f, 26f, 0);
            TitleLine(parent, "Título - SILENT", "SILENT", 104f,  8f, 1);
            TitleLine(parent, "Título - DIVIDE", "DIVIDE", 104f,  8f, 2);
        }

        private static void TitleLine(
            Transform parent, string name, string text, float size, float tracking, int line)
        {
            TextMeshProUGUI label = NewLabel(parent, name, text, size, tracking,
                                             TextAlignmentOptions.Left);
            label.color = UITheme.TextPrimary;

            var rect = label.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot     = new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(RefWidth * 0.5f, size * 1.25f);
            rect.anchoredPosition = new Vector2(
                ColumnLeft, -(TitleTopFrac + line * TitleLineFrac) * RefHeight);
        }

        /// <summary>Rótulo centrado en la columna con el filete debajo, a todo el ancho de esta.</summary>
        private static MenuButton BuildOptionRow(
            Transform parent, string text, int index, List<MenuEntry> collected)
        {
            const float labelHeight = 46f;
            const float ruleGap     = 12f;
            const float ruleHeight  = 1.5f;
            float rowHeight = labelHeight + ruleGap + ruleHeight;

            var go = new GameObject($"Opción - {text}", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot     = new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(ColumnWidth, rowHeight);
            rect.anchoredPosition = new Vector2(
                ColumnLeft, -(FirstOptionFrac + index * OptionStepFrac) * RefHeight);

            // Zona sensible: cubre la fila entera, para que el ratón no tenga que acertar el texto.
            GameObject hitGo = NewGraphic(go.transform, "Zona sensible", out Image hit);
            Stretch(hitGo.GetComponent<RectTransform>());
            hit.color = new Color(0f, 0f, 0f, 0f);
            hit.raycastTarget = true;

            TextMeshProUGUI label = NewLabel(go.transform, "Etiqueta", text, 40f, 4f,
                                             TextAlignmentOptions.Center);
            var labelRect = label.rectTransform;
            labelRect.anchorMin = new Vector2(0f, 1f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.pivot     = new Vector2(0.5f, 1f);
            labelRect.sizeDelta = new Vector2(0f, labelHeight);
            labelRect.anchoredPosition = Vector2.zero;

            Image rule = BuildRule(go.transform, ruleHeight, out Image highlight);

            var button = go.AddComponent<MenuButton>();
            var so = new SerializedObject(button);
            so.FindProperty("label").objectReferenceValue   = label;
            so.FindProperty("rule").objectReferenceValue      = rule;
            so.FindProperty("highlight").objectReferenceValue = highlight;
            so.FindProperty("hitArea").objectReferenceValue = hit;
            so.FindProperty("ruleThickness").floatValue     = ruleHeight;
            so.ApplyModifiedPropertiesWithoutUndo();

            collected.Add(button);
            return button;
        }

        /// <summary>
        /// Filete anclado abajo, a todo el ancho de la fila, más el filete de realce que lo barre
        /// al ganar el foco. El realce nace con ancho cero, así que en reposo no se ve.
        /// </summary>
        private static Image BuildRule(Transform parent, float thickness, out Image highlight)
        {
            GameObject go = NewGraphic(parent, "Filete", out Image rule);
            rule.color = UITheme.RuleIdle;
            rule.raycastTarget = false;

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot     = new Vector2(0.5f, 0f);
            rect.sizeDelta = new Vector2(0f, thickness);
            rect.anchoredPosition = Vector2.zero;

            // Hijo del filete: hereda su alto, así engorda con él al pulsar sin más cuentas.
            GameObject highlightGo = NewGraphic(go.transform, "Filete - realce", out highlight);
            highlight.color = UITheme.RuleHover;
            highlight.raycastTarget = false;

            var highlightRect = highlightGo.GetComponent<RectTransform>();
            highlightRect.anchorMin = new Vector2(0f, 0f);
            highlightRect.anchorMax = new Vector2(0f, 1f);
            highlightRect.offsetMin = Vector2.zero;
            highlightRect.offsetMax = Vector2.zero;

            return rule;
        }

        // ── Pantalla de Ajustes ──────────────────────────────────────────────────────────────

        /// <summary>
        /// Ajustes de Sistema, en la misma escena y con el mismo lenguaje visual. El velo es más
        /// ancho porque cada fila lleva nombre y valor, y en la columna estrecha del menú no caben.
        /// </summary>
        private static SettingsPanel BuildSettingsScreen(Transform parent)
        {
            const float panelWidthFrac = 0.50f;
            float rowWidth = 0.30f * RefWidth;

            var screen = new GameObject("Pantalla - Ajustes", typeof(RectTransform));
            screen.transform.SetParent(parent, false);
            Stretch(screen.GetComponent<RectTransform>());

            BuildScrim(screen.transform, panelWidthFrac);

            TextMeshProUGUI heading = NewLabel(screen.transform, "Título - Ajustes", "AJUSTES",
                                               64f, 14f, TextAlignmentOptions.Left);
            heading.color = UITheme.TextPrimary;
            var headingRect = heading.rectTransform;
            headingRect.anchorMin = headingRect.anchorMax = new Vector2(0f, 1f);
            headingRect.pivot     = new Vector2(0f, 1f);
            headingRect.sizeDelta = new Vector2(rowWidth, 80f);
            headingRect.anchoredPosition = new Vector2(ColumnLeft, -TitleTopFrac * RefHeight);

            // Cursor vertical: cada fila y cada encabezado se coloca debajo del anterior.
            // Las trece filas tienen que caber en la altura de referencia sin desplazamiento:
            // una lista de Ajustes que hay que arrastrar para ver «Volver» es un callejón sin
            // salida con el teclado. Por eso el paso es ajustado y está medido, no tanteado.
            float y = -(TitleTopFrac * RefHeight + 100f);

            var items = new List<MenuEntry>();

            Section(screen.transform, "GRÁFICOS", rowWidth, ref y);
            MenuOption fullscreen = OptionRow(screen.transform, rowWidth, ref y, items);
            MenuOption quality    = OptionRow(screen.transform, rowWidth, ref y, items);

            Section(screen.transform, "AUDIO", rowWidth, ref y);
            MenuSlider master  = SliderRow(screen.transform, rowWidth, ref y, items);
            MenuSlider music   = SliderRow(screen.transform, rowWidth, ref y, items);
            MenuSlider effects = SliderRow(screen.transform, rowWidth, ref y, items);

            Section(screen.transform, "CONTROLES", rowWidth, ref y);
            // De solo lectura: reasignar teclas no está en el prototipo, pero el jugador que abre
            // Ajustes buscando los controles tiene que encontrarlos.
            ReadOnlyRow(screen.transform, "Moverse",   "W A S D  ·  Flechas", rowWidth, ref y);
            ReadOnlyRow(screen.transform, "Confirmar", "Enter  ·  Espacio",   rowWidth, ref y);
            ReadOnlyRow(screen.transform, "Volver",    "Escape",              rowWidth, ref y);

            Section(screen.transform, "ACCESIBILIDAD", rowWidth, ref y);
            MenuOption textScale   = OptionRow(screen.transform, rowWidth, ref y, items);
            MenuOption reduceFlash = OptionRow(screen.transform, rowWidth, ref y, items);

            y -= 30f;
            MenuButton back = BuildBackButton(screen.transform, rowWidth, y, items);

            var panel = screen.AddComponent<SettingsPanel>();
            var so = new SerializedObject(panel);
            SerializedProperty list = so.FindProperty("items");
            list.arraySize = items.Count;
            for (int i = 0; i < items.Count; i++)
                list.GetArrayElementAtIndex(i).objectReferenceValue = items[i];

            so.FindProperty("fullscreenOption").objectReferenceValue  = fullscreen;
            so.FindProperty("qualityOption").objectReferenceValue     = quality;
            so.FindProperty("masterSlider").objectReferenceValue      = master;
            so.FindProperty("musicSlider").objectReferenceValue       = music;
            so.FindProperty("effectsSlider").objectReferenceValue     = effects;
            so.FindProperty("textScaleOption").objectReferenceValue   = textScale;
            so.FindProperty("reduceFlashOption").objectReferenceValue = reduceFlash;
            so.FindProperty("backButton").objectReferenceValue        = back;
            so.ApplyModifiedPropertiesWithoutUndo();

            // Empieza oculta: si no, al construir la escena aparece encima del menú en el editor.
            screen.SetActive(false);

            return panel;
        }

        private static void Section(Transform parent, string text, float width, ref float y)
        {
            y -= 20f;   // aire extra sobre cada encabezado

            TextMeshProUGUI label = NewLabel(parent, $"Sección - {text}", text, 22f, 10f,
                                             TextAlignmentOptions.Left);
            label.color = UITheme.TextSecondary;

            var rect = label.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot     = new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(width, 28f);
            rect.anchoredPosition = new Vector2(ColumnLeft, y);

            y -= 34f;
        }

        /// <summary>Armazón común de las filas de Ajustes: nombre a la izquierda, filete debajo.</summary>
        private static GameObject SettingsRow(
            Transform parent, string name, float width, ref float y,
            out Image hit, out TextMeshProUGUI nameLabel, out Image rule, out Image highlight)
        {
            const float rowHeight = 44f;

            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot     = new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(width, rowHeight);
            rect.anchoredPosition = new Vector2(ColumnLeft, y);

            GameObject hitGo = NewGraphic(go.transform, "Zona sensible", out hit);
            Stretch(hitGo.GetComponent<RectTransform>());
            hit.color = new Color(0f, 0f, 0f, 0f);

            nameLabel = NewLabel(go.transform, "Nombre", string.Empty, 24f, 3f,
                                 TextAlignmentOptions.Left);
            var nameRect = nameLabel.rectTransform;
            nameRect.anchorMin = new Vector2(0f, 0f);
            nameRect.anchorMax = new Vector2(0.5f, 1f);
            nameRect.offsetMin = nameRect.offsetMax = Vector2.zero;

            rule = BuildRule(go.transform, 1.5f, out highlight);

            y -= rowHeight + 10f;
            return go;
        }

        private static TextMeshProUGUI ValueLabel(Transform parent)
        {
            TextMeshProUGUI label = NewLabel(parent, "Valor", string.Empty, 24f, 3f,
                                             TextAlignmentOptions.Right);
            var rect = label.rectTransform;
            rect.anchorMin = new Vector2(0.5f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            return label;
        }

        private static MenuOption OptionRow(
            Transform parent, float width, ref float y, List<MenuEntry> collected)
        {
            GameObject go = SettingsRow(parent, "Fila - Opción", width, ref y,
                                        out Image hit, out TextMeshProUGUI nameLabel, out Image rule, out Image highlight);
            TextMeshProUGUI value = ValueLabel(go.transform);

            var option = go.AddComponent<MenuOption>();
            var so = new SerializedObject(option);
            so.FindProperty("nameLabel").objectReferenceValue  = nameLabel;
            so.FindProperty("valueLabel").objectReferenceValue = value;
            so.FindProperty("rule").objectReferenceValue       = rule;
            so.FindProperty("highlight").objectReferenceValue  = highlight;
            so.FindProperty("hitArea").objectReferenceValue    = hit;
            so.ApplyModifiedPropertiesWithoutUndo();

            collected.Add(option);
            return option;
        }

        private static MenuSlider SliderRow(
            Transform parent, float width, ref float y, List<MenuEntry> collected)
        {
            GameObject go = SettingsRow(parent, "Fila - Barra", width, ref y,
                                        out Image hit, out TextMeshProUGUI nameLabel, out Image rule, out Image highlight);

            // Canal y relleno ocupan el tercio central; el porcentaje va pegado a la derecha.
            GameObject trackGo = NewGraphic(go.transform, "Canal", out Image track);
            var trackRect = trackGo.GetComponent<RectTransform>();
            trackRect.anchorMin = new Vector2(0.50f, 0.5f);
            trackRect.anchorMax = new Vector2(0.85f, 0.5f);
            trackRect.pivot     = new Vector2(0.5f, 0.5f);
            trackRect.sizeDelta = new Vector2(0f, 4f);
            trackRect.anchoredPosition = Vector2.zero;
            track.color = UITheme.TextMuted;
            track.raycastTarget = false;

            GameObject fillGo = NewGraphic(trackGo.transform, "Relleno", out Image fill);
            Stretch(fillGo.GetComponent<RectTransform>());
            fill.color = UITheme.RuleIdle;
            fill.raycastTarget = false;

            TextMeshProUGUI value = NewLabel(go.transform, "Valor", string.Empty, 22f, 2f,
                                             TextAlignmentOptions.Right);
            var valueRect = value.rectTransform;
            valueRect.anchorMin = new Vector2(0.86f, 0f);
            valueRect.anchorMax = new Vector2(1f, 1f);
            valueRect.offsetMin = valueRect.offsetMax = Vector2.zero;

            var slider = go.AddComponent<MenuSlider>();
            var so = new SerializedObject(slider);
            so.FindProperty("nameLabel").objectReferenceValue  = nameLabel;
            so.FindProperty("valueLabel").objectReferenceValue = value;
            so.FindProperty("track").objectReferenceValue      = track;
            so.FindProperty("fill").objectReferenceValue       = fill;
            so.FindProperty("rule").objectReferenceValue       = rule;
            so.FindProperty("highlight").objectReferenceValue  = highlight;
            so.FindProperty("hitArea").objectReferenceValue    = hit;
            so.ApplyModifiedPropertiesWithoutUndo();

            collected.Add(slider);
            return slider;
        }

        private static void ReadOnlyRow(
            Transform parent, string name, string value, float width, ref float y)
        {
            GameObject go = SettingsRow(parent, $"Fila - {name}", width, ref y,
                                        out Image hit, out TextMeshProUGUI nameLabel, out Image rule, out Image _);

            hit.raycastTarget = false;   // no es seleccionable, no debe robar el foco al ratón
            nameLabel.text  = name;
            nameLabel.color = UITheme.TextSecondary;
            rule.color = UITheme.RuleDisabled;

            TextMeshProUGUI valueLabel = ValueLabel(go.transform);
            valueLabel.text  = value;
            valueLabel.color = UITheme.TextSecondary;
        }

        private static MenuButton BuildBackButton(
            Transform parent, float width, float y, List<MenuEntry> collected)
        {
            const float labelHeight = 36f;
            const float ruleHeight  = 1.5f;

            var go = new GameObject("Opción - Volver", typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var rect = go.GetComponent<RectTransform>();
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot     = new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(ColumnWidth, labelHeight + 12f + ruleHeight);
            rect.anchoredPosition = new Vector2(ColumnLeft, y);

            GameObject hitGo = NewGraphic(go.transform, "Zona sensible", out Image hit);
            Stretch(hitGo.GetComponent<RectTransform>());
            hit.color = new Color(0f, 0f, 0f, 0f);

            TextMeshProUGUI label = NewLabel(go.transform, "Etiqueta", "Volver", 30f, 4f,
                                             TextAlignmentOptions.Center);
            var labelRect = label.rectTransform;
            labelRect.anchorMin = new Vector2(0f, 1f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.pivot     = new Vector2(0.5f, 1f);
            labelRect.sizeDelta = new Vector2(0f, labelHeight);
            labelRect.anchoredPosition = Vector2.zero;

            Image rule = BuildRule(go.transform, ruleHeight, out Image highlight);

            var button = go.AddComponent<MenuButton>();
            var so = new SerializedObject(button);
            so.FindProperty("label").objectReferenceValue   = label;
            so.FindProperty("rule").objectReferenceValue      = rule;
            so.FindProperty("highlight").objectReferenceValue = highlight;
            so.FindProperty("hitArea").objectReferenceValue = hit;
            so.FindProperty("ruleThickness").floatValue     = ruleHeight;
            so.ApplyModifiedPropertiesWithoutUndo();

            collected.Add(button);
            return button;
        }

        // ── Utilidades ───────────────────────────────────────────────────────────────────────

        private static GameObject NewGraphic(Transform parent, string name, out Image image)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            go.transform.SetParent(parent, false);
            image = go.GetComponent<Image>();
            return go;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        /// <summary>
        /// Etiqueta con espaciado entre letras: el kit lo usa de forma marcada en todo el texto de
        /// interfaz, y es lo que le da su carácter.
        ///
        /// La tipografía real es **Dune Rise** (título y botones). Mientras el archivo no esté en el
        /// proyecto, TextMeshPro usa su fuente por defecto y el título no se verá como el mockup.
        /// </summary>
        private static TextMeshProUGUI NewLabel(
            Transform parent, string name, string text, float size, float tracking,
            TextAlignmentOptions alignment)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);

            var label = go.AddComponent<TextMeshProUGUI>();
            label.text = text;
            label.fontSize = size;
            label.characterSpacing = tracking;
            label.alignment = alignment;
            label.color = UITheme.TextPrimary;
            label.raycastTarget = false;   // que no robe los eventos de ratón a la fila

            return label;
        }

        // ── Sprites generados ────────────────────────────────────────────────────────────────
        // Se generan en código para no depender de arte: son dos formas triviales y así el menú se
        // construye igual en un clon recién bajado del repositorio.

        private static Sprite circleSprite;
        private static Sprite fadeSprite;

        private static Sprite CircleSprite()
        {
            if (circleSprite != null) return circleSprite;

            const int size = 128;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave,
            };

            float radius = size * 0.5f;
            var pixels = new Color[size * size];

            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f),
                                                  new Vector2(radius, radius));
                // Un píxel de transición: sin él el borde queda dentado al escalar.
                float alpha = Mathf.Clamp01(radius - distance);
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }

            texture.SetPixels(pixels);
            texture.Apply();

            circleSprite = Sprite.Create(texture, new Rect(0f, 0f, size, size),
                                         new Vector2(0.5f, 0.5f));
            circleSprite.hideFlags = HideFlags.DontSave;
            return circleSprite;
        }

        /// <summary>Degradado horizontal de opaco a transparente, para el borde del velo.</summary>
        private static Sprite HorizontalFadeSprite()
        {
            if (fadeSprite != null) return fadeSprite;

            const int width = 64;
            var texture = new Texture2D(width, 1, TextureFormat.RGBA32, false)
            {
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                hideFlags = HideFlags.DontSave,
            };

            for (int x = 0; x < width; x++)
            {
                float t = x / (float)(width - 1);
                // Curva cuadrática: la caída lineal deja una banda visible donde termina el velo.
                texture.SetPixel(x, 0, new Color(1f, 1f, 1f, (1f - t) * (1f - t)));
            }

            texture.Apply();

            fadeSprite = Sprite.Create(texture, new Rect(0f, 0f, width, 1f), new Vector2(0f, 0.5f));
            fadeSprite.hideFlags = HideFlags.DontSave;
            return fadeSprite;
        }
    }
}
