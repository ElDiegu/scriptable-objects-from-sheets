using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptableObjectsFromSheets.Core;
using ScriptableObjectsFromSheets.Extensions;
using ScriptableObjectsFromSheets.Utils;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.ScriptableObjectGenerator.UI
{
    public class ScriptableObjectManagerWindow : EditorWindow
    {
        // Main variables
        private Color _defaultGUIColor;
        private Color _defaultBackgroundColor;
        private ScriptableObjectManagerTable _dataTable;
        
        // Scriptable Object settings variables
        private string _folderPath = "Assets/Project/Resources/Scriptable Objects";
        private MonoScript _targetClass;
        private Type _targetType;
        private List<ScriptableObject> _scriptableObjectInstances = new List<ScriptableObject>();
        private HashSet<ScriptableObject> _dirtyScriptableObjects = new HashSet<ScriptableObject>(); // We need to use a hash because we don't want duplicated SOs
        
        // Google Sheets settings
        private static string _spreadsheetUrl = "https://docs.google.com/spreadsheets/d/1NNL6lvA7hHKdwlYU7KT7_4kYi1ZzvXTrEs0onUa-mmw/edit?gid=0#gid=0";
        private string _sheetName = "Sheet1";
        private string _range = "A1:F4";
        private ProcessedSheetData _processedSheetData;
        private bool _sheetFoldout;

        private List<MemberInfo> _classMembers = new List<MemberInfo>();
        
        [MenuItem("SOFromSheets/ScriptableObject Manager")]
        public static void ShowWindow()
        {
            var window = GetWindow<ScriptableObjectManagerWindow>("Scriptable Object Manager");
            window.minSize = new Vector2(1200, 580);
        }

        private void OnEnable()
        {
            _defaultGUIColor = GUI.backgroundColor;
        }

        private void OnDisable()
        {
            SaveScriptableObjectChanges();
        }

        private void OnGUI()
        {
            UIUtils.DrawHeaderBar("Scriptable Object from Sheets Manager");
            
            DrawScriptableObjectSelection();
            
            if (_targetClass != null) DrawSheetSelectionFoldout();

            if (_dataTable != null) DrawScriptableObjectInstances();
            
            DrawFooter();
        }
        
        #region Drawing Section

        private void DrawScriptableObjectSelection()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            EditorGUILayout.LabelField("Path", GUILayout.Width(40));
            _folderPath = EditorGUILayout.TextField(_folderPath, EditorStyles.toolbarTextField, GUILayout.Width(200));
            if (GUILayout.Button("Browse", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                _folderPath = PathUtils.ChoosePath(_folderPath);
            }
            
            EditorGUILayout.LabelField("Type", GUILayout.Width(40));
            _targetClass = (MonoScript) EditorGUILayout.ObjectField(_targetClass, typeof(MonoScript), true, GUILayout.Width(200));
            if (_targetClass != null) _targetType = _targetClass.GetClass();

            if (GUILayout.Button("Load", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                LoadScriptableObjectInstances();
            }
            // Fill the rest of the space with empty rect to accomodate toolbar style
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawSheetSelectionFoldout()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            _sheetFoldout = EditorGUILayout.Foldout(_sheetFoldout, "Sheet Selection", true);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            if (_sheetFoldout)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                // Spreadsheet URL
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Spreadsheet URL", GUILayout.Width(EditorGUIUtility.labelWidth));
                _spreadsheetUrl = EditorGUILayout.TextField(_spreadsheetUrl, UIUtils.TextInputLayout);
                EditorGUILayout.EndHorizontal();
            
                // Sheet Name
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Sheet Name", GUILayout.Width(EditorGUIUtility.labelWidth));
                _sheetName = EditorGUILayout.TextField(_sheetName, UIUtils.TextInputLayout);
                EditorGUILayout.EndHorizontal();
            
                // Range
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Range", GUILayout.Width(EditorGUIUtility.labelWidth));
                _range = EditorGUILayout.TextField(_range, UIUtils.TextInputLayout);
                EditorGUILayout.EndHorizontal();
            
                // Process sheet data.
                EditorGUILayout.BeginHorizontal();
                bool validSheetData = UIUtils.SheetDataIsValid(_spreadsheetUrl, _sheetName, _range);
                
                GUI.backgroundColor = !validSheetData ? Color.firebrick : Color.green;
            
                if (!validSheetData) GUI.enabled = false;
            
                if (GUILayout.Button("Generate Scriptable Objects", UIUtils.ClassSetupButtonLayout))
                {
                    ImportScriptableObjectInstances();
                    LoadScriptableObjectInstances();
                }
            
                if (!validSheetData) EditorGUILayout.LabelField("Invalid data provided to the sheet processor.");
            
                GUI.enabled = true;
                GUI.backgroundColor = _defaultGUIColor;
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.GetControlRect(false, 10);
            }
        }

        private void DrawScriptableObjectInstances()
        {
            _dataTable.DrawHeaders(GUILayoutUtility.GetLastRect().yMax, position.width, _classMembers.Select(m => m.Name).ToArray(), _classMembers);
            
            var availableHeight = position.height - GUILayoutUtility.GetLastRect().yMax - 30; // Minus 30 to account for the footer bar
            
            Rect bodyRect = new Rect(0, GUILayoutUtility.GetLastRect().yMax, position.width, availableHeight);

            _dataTable.DrawTableBody(bodyRect, _scriptableObjectInstances, _classMembers, ref _dirtyScriptableObjects);;
        }
        
        private void DrawFooter()
        {
            GUILayout.FlexibleSpace();
            var footerRect = EditorGUILayout.GetControlRect(false, 30);
            EditorGUI.DrawRect(footerRect, Color.gray1);

            EditorGUI.BeginDisabledGroup(_dirtyScriptableObjects.Count == 0);;
            
            var generateClassButtonRect = new Rect(footerRect.xMax - 105, footerRect.y + 5, 100, 20);
            GUI.backgroundColor = Color.dodgerBlue;
            if (GUI.Button(generateClassButtonRect, "Save Changes")) SaveScriptableObjectChanges();
            GUI.backgroundColor = _defaultGUIColor;;
            
            EditorGUI.EndDisabledGroup();
        }
        
        #endregion

        private void ImportScriptableObjectInstances()
        {
            SheetQuery query = new SheetQuery()
            {
                SheetId = SheetQuery.ExtractSheetId(_spreadsheetUrl),
                Range = $"{_sheetName}!{_range}"
            };
            
            try
            {
                // Since type arguments must be compile-tyme constant for the code to compile properly, we have to use
                // reflection when calling our ScriptableObject generator.
                
                MethodInfo method = typeof(ScriptableObjectInstanceGenerator)
                    .GetMethod(nameof(ScriptableObjectInstanceGenerator.GenerateScriptableObjectsFromRange))
                    ?.MakeGenericMethod(_targetType);
                    
                method.Invoke(null, new object[] {query, _folderPath});
                
                Repaint();
            }
            catch (Exception e)
            {
                Debug.LogError($"There was an error while trying to get the data from the sheet: {e.Message}");
            }
        }

        private void LoadScriptableObjectInstances()
        {
            _scriptableObjectInstances = new List<ScriptableObject>();
            _classMembers = new List<MemberInfo>();
            
            // We use the AssetDatabase to find all GUIDs of the desired ScriptableObjects
            var assetGuids = AssetDatabase.FindAssets($"t:{_targetType}", new string[] {_folderPath});

            // Then we use the obtained GUIDs to load the ScriptableObjects
            foreach (var guid in assetGuids)
            {
                var asset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), _targetType);
                if (asset != null) _scriptableObjectInstances.Add(asset as ScriptableObject);
            }
            
            BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            foreach (var field in _targetType.GetFields(flags))
            {
                // Avoid unchangeable fields
                if (field.IsLiteral || field.IsInitOnly) continue;
                _classMembers.Add(field);
            }

            foreach (var property in _targetType.GetProperties(flags))
            {
                if (!property.CanRead && !property.CanWrite) continue;
                _classMembers.Add(property);
            }
            
            _dataTable = new ScriptableObjectManagerTable(_scriptableObjectInstances.Count, 8f,
                _classMembers.Select(m => m.GetMemberType()).ToArray(), 22);
        }

        private void SaveScriptableObjectChanges()
        {
            foreach (var so in _dirtyScriptableObjects)
            {
                EditorUtility.SetDirty(so);
            }
            
            AssetDatabase.SaveAssets();
            _dirtyScriptableObjects.Clear();
            Repaint();
        }
    }
}