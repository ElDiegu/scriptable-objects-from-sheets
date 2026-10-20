using ScriptableObjectsFromSheets.APIIntegration;
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
        
        public static void DrawVerticalLine(Color color, float thickness = 1, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(padding + thickness));
            r.width = thickness;
            r.x += padding / 2;
            EditorGUI.DrawRect(r, color);
        }
        
        // Colors 
        
        public static Color DefaultGUIColor =  new Color(0.22f, 0.22f, 0.22f);
        
        public static readonly Color SelectedColor = EditorGUIUtility.isProSkin
            ? new Color32(44, 93, 135, 255)
            : new Color32(58, 114, 176, 255);

        public static readonly Color HoverColor = EditorGUIUtility.isProSkin
            ? new Color32(64, 64, 64, 255)
            : new Color32(216, 216, 216, 255);

        public static readonly Color BackgroundColor = EditorGUIUtility.isProSkin
            ? new Color32(38, 38, 38, 255)
            : new Color32(200, 200, 200, 255);
        
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
        
        public static readonly GUIStyle SidebarLabel = new GUIStyle(EditorStyles.label)
        {
            padding = new RectOffset(16, 0, 0, 0),
            alignment = TextAnchor.MiddleLeft,
            fontSize = 12,
        };

        public static readonly GUIStyle SelectedSidebarLabel = new GUIStyle(SidebarLabel)
        {
            normal = { textColor = Color.white },
            fontStyle = FontStyle.Normal
        };
        
        // Helper Methods
        
        public static bool SheetDataIsValid(string sheetUrl, string sheetName, string range)
        {
            return sheetUrl.Contains("https://docs.google.com/spreadsheets/d/") &&
                   !string.IsNullOrWhiteSpace(sheetName) && range.Contains(":");
        }
        
        // Reused drawing methods
        
        public static void DrawHeaderBar(string label)
        {
            var headerRect = EditorGUILayout.GetControlRect(false, 30);
            EditorGUI.DrawRect(headerRect, Color.gray1);
            
            var iconRect = new Rect(headerRect.x + 5, headerRect.y + 5, 20, 20);
            var labelRect = new Rect(iconRect.xMax + 8, headerRect.y, headerRect.width, headerRect.height);
            
            EditorGUI.DrawRect(iconRect, GoogleSheetsService._serviceInitialized ? Color.dodgerBlue : Color.softRed);
            GUI.Label(iconRect, EditorGUIUtility.IconContent("ScriptableObject Icon").image, EditorStyles.label);
            GUI.Label(labelRect, label, Header);
        }
    }
}