using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public static class AndroidTestBuild
{
    private const string OutputPath = "Builds/Android/BlockArena-Test.apk";
    private const string RequestPath =
        "ProjectSettings/BuildAndroidTest.request";

    [InitializeOnLoadMethod]
    private static void BuildWhenRequested()
    {
        if (!File.Exists(RequestPath))
        {
            return;
        }

        File.Delete(RequestPath);
        EditorApplication.delayCall += AndroidBuildMenu.BuildTestApk;
    }

    public static void BuildApk()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(OutputPath));

        EditorUserBuildSettings.SwitchActiveBuildTarget(
            BuildTargetGroup.Android,
            BuildTarget.Android
        );
        EditorUserBuildSettings.buildAppBundle = false;
        PlayerSettings.Android.useCustomKeystore = false;

        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = new[]
            {
                "Assets/BlockArena/Scenes/MainMenu.unity",
                "Assets/BlockArena/Scenes/Game.unity"
            },
            locationPathName = OutputPath,
            target = BuildTarget.Android,
            options = BuildOptions.Development
        };

        BuildReport report = BuildPipeline.BuildPlayer(options);
        BuildSummary summary = report.summary;
        if (summary.result != BuildResult.Succeeded)
        {
            throw new Exception(
                $"Android APK build failed: {summary.result}, " +
                $"errors={summary.totalErrors}"
            );
        }

        Debug.Log($"APK hazır: {Path.GetFullPath(OutputPath)} ({summary.totalSize} bytes)");
    }
}
