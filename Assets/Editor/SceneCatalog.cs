using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SilentDivide.EditorTools
{
    /// <summary>
    /// Deja el proyecto jugable de principio a fin en un solo comando: construye la pantalla de
    /// inicio y la escena de prototipo, las guarda en disco y las registra en Build Settings.
    ///
    /// Existe porque construir las escenas no bastaba. Los constructores las montaban en memoria y
    /// ahí se quedaban: «Jugar» buscaba una escena llamada «Prototipo» que no existía como archivo,
    /// avisaba por consola y no pasaba nada. Para enseñar el recorrido completo —menú, entrar al
    /// escenario, moverse— hacen falta los tres pasos, no solo el primero.
    /// </summary>
    public static class SceneCatalog
    {
        public const string ScenesFolder  = "Assets/Scenes";
        public const string MenuScenePath = ScenesFolder + "/Inicio.unity";
        public const string GameScenePath = ScenesFolder + "/Prototipo.unity";

        [MenuItem("The Silent Divide/Construir todo y dejarlo jugable", priority = 0)]
        public static void BuildAll()
        {
            if (!EditorUtility.DisplayDialog(
                    "Construir todo",
                    "Se construirán las dos escenas —inicio y prototipo—, se guardarán en " +
                    "Assets/Scenes/ y se registrarán en Build Settings.\n\n" +
                    "Si la escena actual tiene cambios sin guardar, se te pedirá guardarlos.",
                    "Construir", "Cancelar"))
                return;

            EnsureScenesFolder();

            // Primero el prototipo y luego el menú, para acabar con el menú abierto: es la escena
            // por la que se empieza a enseñar.
            Scene game = PrototypeSceneBuilder.BuildScene();
            EditorSceneManager.SaveScene(game, GameScenePath);

            Scene menu = MainMenuSceneBuilder.BuildScene();
            EditorSceneManager.SaveScene(menu, MenuScenePath);

            // El orden importa: la escena 0 es la que arranca en una build.
            SetBuildScenes(MenuScenePath, GameScenePath);

            Debug.Log("[The Silent Divide] Todo listo. Pulsa Play sobre la pantalla de inicio: " +
                      "«Jugar» entra al escenario, y dentro te mueves con WASD.");
        }

        /// <summary>Crea <c>Assets/Scenes</c> si no existe. Unity no la trae en un proyecto vacío.</summary>
        public static void EnsureScenesFolder() => EnsureFolder(ScenesFolder);

        /// <summary>
        /// Crea una carpeta del proyecto si falta, con las intermedias que haga falta. Los
        /// constructores generan assets en carpetas que un clon recién bajado no tiene.
        /// </summary>
        public static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;

            Directory.CreateDirectory(path);
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// Deja en Build Settings exactamente estas escenas, en este orden y activadas. Reemplaza
        /// la lista entera en vez de añadir: así volver a ejecutarlo no acumula duplicados ni deja
        /// de primera una escena vieja que haría arrancar la build por el sitio equivocado.
        /// </summary>
        public static void SetBuildScenes(params string[] paths)
        {
            var scenes = new List<EditorBuildSettingsScene>(paths.Length);
            foreach (string path in paths)
                scenes.Add(new EditorBuildSettingsScene(path, true));

            EditorBuildSettings.scenes = scenes.ToArray();
        }

        /// <summary>Guarda la escena abierta y la registra, conservando el resto de la lista.</summary>
        public static void SaveAndRegister(Scene scene, string path)
        {
            EnsureScenesFolder();
            EditorSceneManager.SaveScene(scene, path);

            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (scenes.Exists(s => s.path == path)) return;

            scenes.Add(new EditorBuildSettingsScene(path, true));
            EditorBuildSettings.scenes = scenes.ToArray();
        }
    }
}
