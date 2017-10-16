using System.Collections.Generic;
using UnityEditor;

public class Builder
{
    public static void Build()
    {
        // Fixes "Plugin 'VuforiaWrapper.dll' is used from several locations".
        //
        // Vuforia updates some plugin settings in Vuforia/Editor/Scripts/ExtensionImport.cs.
        // Running this method beforehand will give the editor script an
        // opportunity to run.
        EditorApplication.update();

        List<string> sceneNames = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            sceneNames.Add(scene.path);
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WSA, BuildTarget.WSAPlayer);
        EditorUserBuildSettings.wsaSubtarget = WSASubtarget.HoloLens;
        BuildPlayerOptions options = new BuildPlayerOptions
        {
            scenes = sceneNames.ToArray(),
            target = BuildTarget.WSAPlayer,
            locationPathName = "UWP",
            options = BuildOptions.None,
            targetGroup = BuildTargetGroup.WSA
        };
        BuildPipeline.BuildPlayer(options);
    }
}
