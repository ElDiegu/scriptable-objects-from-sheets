using System;
using UnityEditor;
using UnityEngine;

public static class InterfaceUtils
{
    public static string ChooseOutputPath(string currentPath)
    {
        string path = EditorUtility.OpenFolderPanel("Select output path", currentPath, "");

        if (string.IsNullOrEmpty(path))
        {
            Debug.LogError("Path is empty");
            return currentPath;
        }

        // Check if the selected path is under "Assets" folder
        if (!path.Contains("Assets"))
        {
            Debug.LogError("Path must be under Assets");
            return currentPath;
        }
        
        // Convert absolute path to relative path
        if (path.StartsWith(Application.dataPath)) path = "Assets" + path.Substring(Application.dataPath.Length);
        return path;
    }
}
