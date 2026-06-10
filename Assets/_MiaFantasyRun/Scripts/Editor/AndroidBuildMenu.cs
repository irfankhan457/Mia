using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MiaFantasyRun.Editor
{
    public static class AndroidBuildMenu
    {
        private const string ProductName = "Mia Fantasy Run";
        private const string CompanyName = "Irfan Games";
        private const string PackageName = "com.irfangames.miafantasyrun";
        private const string OutputFolder = "Builds/Android";
        private static readonly string[] Scenes =
        {
            "Assets/_MiaFantasyRun/Scenes/Splash.unity",
            "Assets/_MiaFantasyRun/Scenes/MainMenu.unity",
            "Assets/_MiaFantasyRun/Scenes/Main.unity"
        };

        [MenuItem("Mia Fantasy Run/Android/Configure Android Player Settings")]
        public static void ConfigureAndroidPlayerSettings()
        {
            PlayerSettings.companyName = CompanyName;
            PlayerSettings.productName = ProductName;
            PlayerSettings.bundleVersion = "1.0.0";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, PackageName);
            PlayerSettings.Android.bundleVersionCode = 1;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel29;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.LandscapeLeft;
            PlayerSettings.allowedAutorotateToLandscapeLeft = true;
            PlayerSettings.allowedAutorotateToLandscapeRight = true;
            PlayerSettings.allowedAutorotateToPortrait = false;
            PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
            PlayerSettings.SplashScreen.showUnityLogo = false;
            PlayerSettings.SplashScreen.show = true;

            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(Scenes[0], true),
                new EditorBuildSettingsScene(Scenes[1], true),
                new EditorBuildSettingsScene(Scenes[2], true)
            };

            AssetDatabase.SaveAssets();
            Debug.Log("Mia Fantasy Run: Android player settings configured.");
        }

        [MenuItem("Mia Fantasy Run/Android/Build Development APK")]
        public static void BuildDevelopmentApk()
        {
            ConfigureDevelopmentApkSettings();
            BuildAndroid("MiaFantasyRun-dev.apk", false, BuildOptions.Development | BuildOptions.AllowDebugging);
        }

        [MenuItem("Mia Fantasy Run/Android/Build Google Play AAB")]
        public static void BuildGooglePlayAab()
        {
            ConfigureReleaseAabSettings();
            BuildAndroid("MiaFantasyRun-release.aab", true, BuildOptions.None);
        }

        private static void ConfigureDevelopmentApkSettings()
        {
            ConfigureAndroidPlayerSettings();
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.Mono2x);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7;
            AssetDatabase.SaveAssets();
            Debug.Log("Mia Fantasy Run: Development APK settings configured.");
        }

        private static void ConfigureReleaseAabSettings()
        {
            ConfigureAndroidPlayerSettings();
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            AssetDatabase.SaveAssets();
            Debug.Log("Mia Fantasy Run: Release AAB settings configured.");
        }

        private static void BuildAndroid(string fileName, bool appBundle, BuildOptions options)
        {
            SceneSetupMenu.CreateAllGameScenes();
            Directory.CreateDirectory(OutputFolder);

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            {
                throw new BuildFailedException("Android Build Support is not installed for this Unity editor.");
            }

            EditorUserBuildSettings.buildAppBundle = appBundle;
            var report = BuildPipeline.BuildPlayer(Scenes, Path.Combine(OutputFolder, fileName), BuildTarget.Android, options);
            if (report.summary.result != BuildResult.Succeeded)
            {
                throw new BuildFailedException($"Android build failed: {report.summary.result}");
            }

            Debug.Log($"Mia Fantasy Run: Android build created at {Path.Combine(OutputFolder, fileName)}");
        }
    }
}
