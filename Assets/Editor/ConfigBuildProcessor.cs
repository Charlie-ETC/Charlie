#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class ConfigBuildProcessor : IPreprocessBuild {
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildTarget target, string path)
    {
        Debug.Log($"ConfigBuildProcessor.OnPreprocessBuild for target ${target} at path ${path}");
    }
}
#endif
