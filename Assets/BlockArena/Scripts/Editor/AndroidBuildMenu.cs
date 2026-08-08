using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AndroidBuildMenu
{
    private const string OutputDirectory = "Builds/Android";
    private const string OutputPath = OutputDirectory + "/BlockArena.apk";
    private const string ReleaseOutputPath =
        OutputDirectory + "/BlockArena.aab";
    private const string AppIconPath =
        "Assets/BlockArena/Art/UI/BlockArenaAppIcon.png";

    [MenuItem("Block Arena/Build Android Test APK")]
    public static void BuildTestApk()
    {
        Directory.CreateDirectory(OutputDirectory);

        PlayerSettings.companyName = "Block Arena";
        PlayerSettings.productName = "Block Arena";
        PlayerSettings.SetApplicationIdentifier(
            NamedBuildTarget.Android,
            "com.blockarena.game"
        );
        PlayerSettings.Android.targetArchitectures =
            AndroidArchitecture.ARM64;
        PlayerSettings.Android.bundleVersionCode = 6;
        ConfigureAppIcon();
        SetLegacyInputManagerOnly();
        EditorUserBuildSettings.buildAppBundle = false;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = EnabledScenes(),
            locationPathName = OutputPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.Development
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new BuildFailedException(
                $"Android build failed: {report.summary.result}"
            );
        }

        Debug.Log(
            $"Android test APK ready: {Path.GetFullPath(OutputPath)}"
        );
        EditorUtility.RevealInFinder(OutputPath);
    }

    [MenuItem("Block Arena/Build Android Release AAB")]
    public static void BuildReleaseAab()
    {
        if (!PlayerSettings.Android.useCustomKeystore)
        {
            throw new BuildFailedException(
                "Configure a custom Android keystore in Player Settings " +
                "before building the release AAB."
            );
        }

        Directory.CreateDirectory(OutputDirectory);

        PlayerSettings.companyName = "Block Arena";
        PlayerSettings.productName = "Block Arena";
        PlayerSettings.bundleVersion = "1.0.0";
        PlayerSettings.SetApplicationIdentifier(
            NamedBuildTarget.Android,
            "com.blockarena.game"
        );
        PlayerSettings.Android.bundleVersionCode = 6;
        PlayerSettings.Android.targetArchitectures =
            AndroidArchitecture.ARM64;
        ConfigureAppIcon();
        SetLegacyInputManagerOnly();
        EditorUserBuildSettings.buildAppBundle = true;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = EnabledScenes(),
            locationPathName = ReleaseOutputPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
            options = BuildOptions.None
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        if (report.summary.result != BuildResult.Succeeded)
        {
            throw new BuildFailedException(
                $"Android release build failed: {report.summary.result}"
            );
        }

        Debug.Log(
            $"Android release AAB ready: " +
            $"{Path.GetFullPath(ReleaseOutputPath)}"
        );
        EditorUtility.RevealInFinder(ReleaseOutputPath);
    }

    private static string[] EnabledScenes()
    {
        return System.Array.ConvertAll(
            System.Array.FindAll(
                EditorBuildSettings.scenes,
                scene => scene.enabled
            ),
            scene => scene.path
        );
    }

    private static void SetLegacyInputManagerOnly()
    {
        Object[] settingsAssets = AssetDatabase.LoadAllAssetsAtPath(
            "ProjectSettings/ProjectSettings.asset"
        );
        if (settingsAssets.Length == 0)
        {
            throw new BuildFailedException("PlayerSettings could not be loaded.");
        }

        SerializedObject settings = new SerializedObject(settingsAssets[0]);
        SerializedProperty inputHandler = settings.FindProperty(
            "activeInputHandler"
        );
        if (inputHandler == null)
        {
            throw new BuildFailedException(
                "Active Input Handling setting could not be found."
            );
        }

        // 0 = Input Manager (Old), which is the API used by BoardManager.
        inputHandler.intValue = 0;
        settings.ApplyModifiedPropertiesWithoutUndo();
        AssetDatabase.SaveAssets();
    }

    private static void ConfigureAppIcon()
    {
        Texture2D icon = AssetDatabase.LoadAssetAtPath<Texture2D>(
            AppIconPath
        );
        if (icon == null)
        {
            throw new BuildFailedException(
                $"App icon could not be loaded at {AppIconPath}."
            );
        }

        PlayerSettings.SetIcons(
            NamedBuildTarget.Android,
            new[] { icon },
            IconKind.Application
        );
    }
}
