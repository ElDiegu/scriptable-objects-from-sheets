using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.Utils
{
    public static class WindowUtils
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
    }
}