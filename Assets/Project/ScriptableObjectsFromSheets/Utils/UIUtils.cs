using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.Utils
{
    public static class UIUtils
    {
        public const float NameColumnWidth = 160f;
        public const float TypeColumnWidth = 130f;
        public const float IsListColumnWidth = 50f;
        public const float SeparatorCharColumnWidth = 50f;
        public const float DeleteButtonColumnWidth = 20f;
        public const float CellPadding = 5f;

        public static Rect TableCellRect(ref float tableCursor, float width, Rect row)
        {
            // We calculate the position of the cell. 
            var rect = new Rect(tableCursor, row.y + 2, width, row.height - 4);
            
            // We move the table cursor forward
            tableCursor += width + CellPadding;
            
            return rect;
        }
        
        public static void DrawHorizontalLine(Color color, float thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            EditorGUI.DrawRect(r, color);
        }
        
        // Layouts
        
        public static readonly GUILayoutOption[] TextInputLayout =
        {
            //GUILayout.MaxWidth(400)
            GUILayout.ExpandWidth(true)
        };

        public static readonly GUILayoutOption[] ClassSetupButtonLayout =
        {
            GUILayout.MinWidth(60),
            GUILayout.ExpandWidth(false)
        };
        
        // Styles
        
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