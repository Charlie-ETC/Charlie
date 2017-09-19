using UnityEditor;

public class Builder
{
    public static void Build()
    {
        BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, "UWP",
            BuildTarget.WSAPlayer, BuildOptions.None);
    }
}
