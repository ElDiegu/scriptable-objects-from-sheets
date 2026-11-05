using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.Core
{
    /// <summary>
    /// Persistent sheet selection (spreadsheet URL, sheet name and range) shared by every tab of the extension.
    /// Saved per user and project in UserSettings/, so it survives domain reloads and editor restarts.
    /// </summary>
    [FilePath("UserSettings/ScriptableObjectsFromSheets/SheetSelection.asset", FilePathAttribute.Location.ProjectFolder)]
    public class SheetSelectionSettings : ScriptableSingleton<SheetSelectionSettings>
    {
        [SerializeField] private string spreadsheetUrl = "";
        [SerializeField] private string sheetName = "";
        [SerializeField] private string range = "";
        [SerializeField] private string targetScriptGuid = "";

        public static string SpreadsheetUrl
        {
            get => instance.spreadsheetUrl;
            set => Set(ref instance.spreadsheetUrl, value);
        }

        public static string SheetName
        {
            get => instance.sheetName;
            set => Set(ref instance.sheetName, value);
        }

        public static string Range
        {
            get => instance.range;
            set => Set(ref instance.range, value);
        }
        
        public static MonoScript TargetScript
        {
            get
            {
                string path = AssetDatabase.GUIDToAssetPath(instance.targetScriptGuid);
                return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            }
            set
            {
                string path = value != null ? AssetDatabase.GetAssetPath(value) : "";
                Set(ref instance.targetScriptGuid, string.IsNullOrEmpty(path) ? "" : AssetDatabase.AssetPathToGUID(path));
            }
        }

        private static void Set(ref string field, string value)
        {
            if (field == value) return;

            field = value;
            instance.Save(true);
        }
    }
}
