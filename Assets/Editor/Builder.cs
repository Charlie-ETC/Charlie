using System.Collections.Generic;
using UnityEditor;

public class Builder
{
    public static void Build()
    {
        List<string> sceneNames = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            sceneNames.Add(scene.path);
        }

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
