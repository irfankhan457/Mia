#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MiaFantasyRun.Editor
{
    [InitializeOnLoad]
    public static class AutoRunStarterScene
    {
        private static bool hasScheduled;

        static AutoRunStarterScene()
        {
            EditorApplication.update += TryRun;
        }

        private static void TryRun()
        {
            var markerPath = GetMarkerPath();
            if (hasScheduled || EditorApplication.isCompiling || EditorApplication.isUpdating || !File.Exists(markerPath))
            {
                return;
            }

            hasScheduled = true;
            File.Delete(markerPath);
            EditorApplication.update -= TryRun;
            EditorApplication.delayCall += Run;
        }

        private static string GetMarkerPath()
        {
            var projectRoot = Directory.GetParent(Application.dataPath)?.FullName ?? Directory.GetCurrentDirectory();
            return Path.Combine(projectRoot, ".mia_autorun.marker");
        }

        private static void Run()
        {
            if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += Run;
                return;
            }

            SceneSetupMenu.CreateStarterScene();

            EditorApplication.delayCall += () => EditorApplication.isPlaying = true;
        }

        public static void RunNow()
        {
            Run();
        }

        public static void RunSplash()
        {
            if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.isPlaying = false;
                EditorApplication.delayCall += RunSplash;
                return;
            }

            EditorSceneManager.OpenScene("Assets/_MiaFantasyRun/Scenes/Splash.unity");
            EditorApplication.delayCall += () => EditorApplication.isPlaying = true;
        }
    }
}
#endif
