using System;
using System.Collections.Generic;
using System.Linq;
using Project.SO_Builder;
using ScriptableObjectsFromSheets.Utils;
using SOFromSheets.Controllers;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.ScriptableObjectBuilder.Interface
{
    public class ScriptableObjectBuilderWindow : EditorWindow
    {
        // Main variables
        private Color _defaultGUIColor;
        private Color _defaultBackgroundColor;
        private List<string> _availableTypes;
        
        // Spreadsheet variables
        // TODO: Remove default values for debugging
        private static string _spreadsheetUrl = "https://docs.google.com/spreadsheets/d/1NNL6lvA7hHKdwlYU7KT7_4kYi1ZzvXTrEs0onUa-mmw/edit?gid=0#gid=0";
        private string _sheetName = "Sheet1";
        private string _range = "A1:F4";
        private ProcessedSheetData _processedSheetData;
        
        // Class setup variables
        private string _className;
        private static string _classNamespace;
        private static string _outputPath = "Assets/";
        private string _assetMenuPath;
        private string _fileName;
        
        // Class fields variables
        private List<ClassField> _classFields = new List<ClassField>();

        // Preview Variables
        private string _classString;
        private Vector2 _previewScroll;
        private Vector2 _classFieldTableScroll;
        
        [MenuItem("SOFromSheets/ScriptableObject Builder")]
        public static void ShowWindow()
        {
            var window = GetWindow<ScriptableObjectBuilderWindow>("Scriptable Object Builder");
            window.minSize = new Vector2(1200, 580);
        }

        private void OnEnable()
        {
            _className = EditorSettings.projectGenerationRootNamespace;
            _defaultGUIColor = GUI.backgroundColor;
            _defaultBackgroundColor = new Color(0.22f, 0.22f, 0.22f);
            _availableTypes = TypeInference.TypeCandidates.Select(typeCandidate => typeCandidate.type).ToList();
        }

        private void OnGUI()
        {
            DrawHeaderBar();
            WindowUtils.DrawHorizontalLine(Color.dimGray);
            
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical(GUILayout.Width(600), GUILayout.ExpandHeight(true));
            DrawSheetSelection();
            WindowUtils.DrawHorizontalLine(Color.dimGray);
            DrawClassBuilderSetup();
            WindowUtils.DrawHorizontalLine(Color.dimGray);
            DrawClassFieldTable();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));
            DrawClassPreview();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
            
            DrawFooter();
            
            UpdateWindow();
        }
        
        // *------ Drawing Section ------*
        // This section contains the methods that draw different sections of the interface window.
        // It has been decided to separate the drawing login into different methods to avoid excessive code clutter
        // Inside the "OnGui" Unity method. This allows for better debugging and scalability.

        /// <summary>
        /// Draws the header bar.
        /// </summary>
        private void DrawHeaderBar()
        {
            var headerRect = EditorGUILayout.GetControlRect(false, 30);
            EditorGUI.DrawRect(headerRect, Color.gray1);
            
            var iconRect = new Rect(headerRect.x + 5, headerRect.y + 5, 20, 20);
            var labelRect = new Rect(iconRect.xMax + 8, headerRect.y, headerRect.width, headerRect.height);
            
            EditorGUI.DrawRect(iconRect, Color.dodgerBlue);
            GUI.Label(iconRect, EditorGUIUtility.IconContent("ScriptableObject Icon").image, EditorStyles.label);
            GUI.Label(labelRect, "Scriptable Object from Sheets Builder", WindowStyles.Header);
        }

        /// <summary>
        /// Draws the necessary information to query Google Sheets data.
        /// </summary>
        private void DrawSheetSelection()
        {
            EditorGUILayout.LabelField("Sheet Selection", WindowStyles.SectionLabel);
            
            // Spreadsheet URL
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Spreadsheet URL", GUILayout.Width(EditorGUIUtility.labelWidth));
            _spreadsheetUrl = EditorGUILayout.TextField(_spreadsheetUrl, WindowUtils.TextInputLayout);
            EditorGUILayout.EndHorizontal();
            
            // Sheet Name
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sheet Name", GUILayout.Width(EditorGUIUtility.labelWidth));
            _sheetName = EditorGUILayout.TextField(_sheetName, WindowUtils.TextInputLayout);
            EditorGUILayout.EndHorizontal();
            
            // Range
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Range", GUILayout.Width(EditorGUIUtility.labelWidth));
            _range = EditorGUILayout.TextField(_range, WindowUtils.TextInputLayout);
            EditorGUILayout.EndHorizontal();
            
            bool validData = !string.IsNullOrWhiteSpace(_spreadsheetUrl) && !string.IsNullOrWhiteSpace(_sheetName) && !string.IsNullOrWhiteSpace(_range);
            
            // Process sheet data.
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = !SheetDataIsValid() ? Color.firebrick : Color.forestGreen;
            
            if (!SheetDataIsValid()) GUI.enabled = false;
            
            if (GUILayout.Button("Process Sheet", WindowUtils.ClassSetupButtonLayout))
            {
                ProcessSheet();
            }
            
            if (!SheetDataIsValid()) EditorGUILayout.LabelField("Invalid data provided to the sheet processor.");
            
            GUI.enabled = true;
            GUI.backgroundColor = _defaultGUIColor;
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draws all necessary fields for the Scriptable Object class setup such as desired namespace, output path or class name.
        /// </summary>
        private void DrawClassBuilderSetup()
        {
            EditorGUILayout.LabelField("Class Setting", WindowStyles.SectionLabel);
            
            EditorGUI.BeginDisabledGroup(_processedSheetData == null);
            
            // Class Name
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Class Name", GUILayout.Width(EditorGUIUtility.labelWidth));
            _className = EditorGUILayout.TextField(_className, WindowUtils.TextInputLayout);
            EditorGUILayout.EndHorizontal();
            
            // Namespace
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Namespace", GUILayout.Width(EditorGUIUtility.labelWidth));
            _classNamespace = EditorGUILayout.TextField(_classNamespace, WindowUtils.TextInputLayout);
            if (GUILayout.Button("Suggest", WindowUtils.ClassSetupButtonLayout))
            {
                _classNamespace = ScriptableObjectClassBuilder.GetNamespaceFromOutputPath(_outputPath);
            } 
            EditorGUILayout.EndHorizontal();
            
            // Asset Menu and File Name
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Asset Menu", GUILayout.Width(EditorGUIUtility.labelWidth));
            _assetMenuPath = EditorGUILayout.TextField(_assetMenuPath, WindowUtils.TextInputLayout);
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("File name", GUILayout.Width(EditorGUIUtility.labelWidth));;
            _fileName = EditorGUILayout.TextField(_fileName, WindowUtils.TextInputLayout);
            EditorGUILayout.EndHorizontal();
            
            // Output Path
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Output Path", GUILayout.Width(EditorGUIUtility.labelWidth));
            _outputPath = EditorGUILayout.TextField(_outputPath, WindowUtils.TextInputLayout);
            if (GUILayout.Button("Browse", WindowUtils.ClassSetupButtonLayout))
            {
                _outputPath = InterfaceUtils.ChooseOutputPath(_outputPath);
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUI.EndDisabledGroup();
        }

        /// <summary>
        /// Draws a table with generated fields from the processed data.
        /// </summary>
        private void DrawClassFieldTable()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Class Fields", WindowStyles.SectionLabel);
            
            // Add field button should only appear if the sheet has been processed.
            if (_processedSheetData != null)
                if (GUILayout.Button("Add Field", WindowUtils.ClassSetupButtonLayout)) _classFields.Add(new ClassField());
            GUILayout.EndHorizontal();
            
            Rect rowRect = EditorGUILayout.GetControlRect(false, 18);
            EditorGUI.DrawRect(rowRect, _defaultBackgroundColor);

            float tableCursor = rowRect.x + WindowUtils.CellPadding;

            GUI.Label(WindowUtils.TableCellRect(ref tableCursor, WindowUtils.NameColumnWidth, rowRect), "Field Name",
                WindowStyles.TableHeaderLabel);
            GUI.Label(WindowUtils.TableCellRect(ref tableCursor, WindowUtils.TypeColumnWidth, rowRect), "Type",
                WindowStyles.TableHeaderLabel);
            GUI.Label(WindowUtils.TableCellRect(ref tableCursor, WindowUtils.IsListColumnWidth, rowRect), "Is List",
                WindowStyles.TableHeaderLabel);
            GUI.Label(WindowUtils.TableCellRect(ref tableCursor, WindowUtils.SeparatorCharColumnWidth, rowRect),
                "Separator", WindowStyles.TableHeaderLabel);
            
            // Fields table
            
            // During field print we store a list of all the fields where the delete button has been pressed.
            var fieldsToRemove = new List<int>();
            
            _classFieldTableScroll = EditorGUILayout.BeginScrollView(_classFieldTableScroll, GUILayout.ExpandHeight(true));
            
            for (int i = 0; i < _classFields.Count; i++) if (DrawClassFieldConfiguratorRow(_classFields[i], i)) fieldsToRemove.Add(i);
            
            EditorGUILayout.EndScrollView();

            // We then remove stored fields after all fields have been painted so next render iteration the deleted fields
            // won't be painted.
            foreach (int index in fieldsToRemove) _classFields.RemoveAt(index);
        }

        private bool DrawClassFieldConfiguratorRow(ClassField field, int index)
        {
            Rect rowRect = EditorGUILayout.GetControlRect(false, 22);
            
            // Alternate row color
            var backgroundColor = index % 2 == 0 ? Color.gray2 : _defaultBackgroundColor;
            EditorGUI.DrawRect(rowRect, backgroundColor);
            
            float tableCursor = rowRect.x + WindowUtils.CellPadding;
            
            field.Name = EditorGUI.TextField(WindowUtils.TableCellRect(ref tableCursor, WindowUtils.NameColumnWidth, rowRect), field.Name);
            
            field.Type = _availableTypes[
                EditorGUI.Popup(
                    WindowUtils.TableCellRect(ref tableCursor, WindowUtils.TypeColumnWidth, rowRect),
                    _availableTypes.IndexOf(field.Type),
                    _availableTypes.ToArray())
            ];
            
            field.IsList = EditorGUI.Toggle(WindowUtils.TableCellRect(ref tableCursor, WindowUtils.IsListColumnWidth, rowRect), field.IsList);

            if (field.IsList)
            {
                field.ListSeparator = EditorGUI.TextField(WindowUtils.TableCellRect(ref tableCursor, WindowUtils.IsListColumnWidth, rowRect), field.ListSeparator.ToString())[0];
            }
            else
            {
                EditorGUI.DrawRect(WindowUtils.TableCellRect(ref tableCursor, WindowUtils.SeparatorCharColumnWidth, rowRect), backgroundColor);                 
            }

            GUI.color = Color.red;
            bool delete = GUI.Button(WindowUtils.TableCellRect(ref tableCursor, WindowUtils.DeleteButtonColumnWidth, rowRect), "X");
            GUI.color = _defaultGUIColor;

            return delete;
        }

        /// <summary>
        /// Draws a preview of the class that's going to be generated by the extension with real-time modification
        /// of the different fields and parameters of the class.
        /// </summary>
        private void DrawClassPreview()
        {
            EditorGUILayout.LabelField("Class Preview", WindowStyles.SectionLabel);
            
            if (string.IsNullOrWhiteSpace(_classString)) return;

            // GUILayout.ExpandHeight(true) was overflowing and hiding part of the preview, so we need to calculate 
            // the scrollView's height manually.
            float usedHeight = GUILayoutUtility.GetLastRect().yMax;
            float scrollHeight = Mathf.Max(position.height - usedHeight - 100f, 60f);
            
            _previewScroll = EditorGUILayout.BeginScrollView(_previewScroll, GUILayout.Height(scrollHeight));

            GUI.enabled = false;
            EditorGUILayout.TextArea(_classString, GUILayout.ExpandHeight(true));
            GUI.enabled = true;
            
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Draws the footer bar with the button to generate the processed class and other information.
        /// </summary>
        private void DrawFooter()
        {
            GUILayout.FlexibleSpace();
            var footerRect = EditorGUILayout.GetControlRect(false, 30);
            EditorGUI.DrawRect(footerRect, Color.gray1);

            EditorGUI.BeginDisabledGroup(_processedSheetData == null);
            
            var generateClassButtonRect = new Rect(footerRect.xMax - 105, footerRect.y + 5, 100, 20);
            GUI.backgroundColor = Color.dodgerBlue;
            if (GUI.Button(generateClassButtonRect, "Generate Class")) ScriptableObjectClassBuilder.GenerateScriptableObjectClass(_classString, _outputPath, _className);
            GUI.backgroundColor = _defaultGUIColor;;
            
            EditorGUI.EndDisabledGroup();
        }
        
        // *------ Other Methods ------*

        private void ProcessSheet()
        {
            SheetQuery query = new SheetQuery()
            {
                SheetId = SheetQuery.ExtractSheetId(_spreadsheetUrl),
                Range = $"{_sheetName}!{_range}"
            };
            
            _classFields = new List<ClassField>();
            
            try
            {
                var data = GoogleSheetsService.GetRange(query);
                RawSheetData rawSheetData = new RawSheetData(data, query);
                _processedSheetData = new ProcessedSheetData(rawSheetData);
                
                _className = _processedSheetData.ClassName;
                _assetMenuPath = $"ScriptableObjects/{_className}";
                _fileName = $"New {_className}";
                
                foreach (string field in _processedSheetData.Fields.Keys)
                    _classFields.Add(new ClassField(field, _processedSheetData.Fields[field]));
            }
            catch (Exception e)
            {
                Debug.LogError($"There was an error while trying to get the data from the sheet: {e.Message}");
            }
        }

        private bool SheetDataIsValid()
        {
            return _spreadsheetUrl.Contains("https://docs.google.com/spreadsheets/d/") &&
                   !string.IsNullOrWhiteSpace(_sheetName) && _range.Contains(":");
        }

        private void UpdateWindow()
        {
            if (_processedSheetData != null)
                _classString = ScriptableObjectClassBuilder.BuildScriptableObjectString(_classFields, _className, _classNamespace, _fileName, _assetMenuPath);
            
            Repaint();
        }
    }
}