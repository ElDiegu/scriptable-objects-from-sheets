using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.Utils
{
    /// <summary>
    /// Stores all styles required to draw the different sections of the interface windows.
    /// </summary>
    public static class WindowStyles
    {
        public static GUIStyle Header = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 15,
            alignment = TextAnchor.MiddleLeft,
            normal = { textColor = Color.white }
        };

        public static readonly GUIStyle SectionLabel = new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 13,
            normal = { textColor = Color.white }
        };

        public static readonly GUIStyle NoteLabel = new GUIStyle(EditorStyles.label)
        {
            fontSize = 10,
            normal = { textColor = Color.slateGray }
        };

        public static readonly GUIStyle TableHeaderLabel = new GUIStyle(EditorStyles.miniLabel)
        {
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.gray6 }
        };
    }
}