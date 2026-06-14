using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptableObjectsFromSheets.ScriptableObjectManager.Attributes;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.Utils
{
    public class ScriptableObjectManagerTable
    {
        public float CellPadding { get; set; }
        public List<float> ColumnWidths { get; set; } = new List<float>();
        public float RowHeight { get; set; }
        private int _rowNumber;
        private Rect _tablePointer;
        private const float MinColumnWidth = 50f;
        private Vector2 _bodyScroll;
        
        // Column width handler
        private const float HandlerWidth = 10f; // Width of the handler to resize columns
        private const float SeparatorWidth = 2f;
        private int _selectedColumn = -1;
        private float _dragStartingPosition; // We only save the "X" value of the position as we don't care about vertical dragging
        private float _originalColumnWidth;

        public ScriptableObjectManagerTable(int rowNumber, float cellPadding, Type[] types, float rowHeight)
        {
            _rowNumber = rowNumber;
            CellPadding = cellPadding + HandlerWidth;
            RowHeight = rowHeight;
            foreach (Type type in types) ColumnWidths.Add(MinWidthByType(type));
        }

        public void DrawHeaders(float headerPointer, float windowWidth, string[] headers, List<MemberInfo> members)
        {
            var totalWidth = Mathf.Max(ColumnWidths.Sum() + CellPadding * (ColumnWidths.Count - 1), windowWidth);
            
            Rect rowRect = new Rect(0, headerPointer, totalWidth, RowHeight);
            EditorGUI.DrawRect(rowRect, Color.gray2);

            float cellPointer = rowRect.x;
            
            for (int i = 0; i < headers.Length; i++)
            {
                if (members[i].HasAttribute<SheetImportedAttribute>()) headers[i] = $"{headers[i]}📥";
                
                // We get the width of the header label to adjust column size to it
                float labelWidth = GUI.skin.label.CalcSize(new GUIContent(headers[i])).x;
                ColumnWidths[i] = Mathf.Max(labelWidth, ColumnWidths[i]);
                
                Rect headerCellRect = GetCellRect(ref cellPointer, ColumnWidths[i], rowRect);
                GUI.Label(headerCellRect, headers[i], UIUtils.TableHeaderLabel);
                
                cellPointer += CellPadding;
                
                DrawWidthHandler(rowRect, cellPointer, i);
            }
        }

        public void DrawTableBody(Rect bodyRect, List<ScriptableObject> scriptableObjectInstances, List<MemberInfo> members, ref HashSet<ScriptableObject> dirtyScriptableObjects)
        {
            var totalWidth = Mathf.Max(ColumnWidths.Sum() + CellPadding * (ColumnWidths.Count - 1), bodyRect.width);
            
            Rect viewRect = new Rect(0, 0, ColumnWidths.Sum() + CellPadding * (ColumnWidths.Count - 1), RowHeight * scriptableObjectInstances.Count);
            _bodyScroll = GUI.BeginScrollView(bodyRect, _bodyScroll, viewRect);
            
            for (int i = 0; i < scriptableObjectInstances.Count; i++)
            {
                // Row background
                Rect rowRect = new Rect(0, (i + 1) * RowHeight, totalWidth, RowHeight);
                
                EditorGUI.BeginChangeCheck();
                DrawRow(scriptableObjectInstances[i], members, rowRect, i);
                if (EditorGUI.EndChangeCheck())
                {
                    dirtyScriptableObjects.Add(scriptableObjectInstances[i]);
                }
            }
            
            GUI.EndScrollView();
        }
        
        public void DrawRow(ScriptableObject so, List<MemberInfo> members, Rect rowRect, int rowIndex)
        {
            EditorGUI.DrawRect(rowRect, rowIndex % 2 == 0 ? Color.gray4 : Color.gray3);
            
            float cellPointer = rowRect.x;

            for (int i = 0; i < members.Count; i++)
            {
                DrawCell(GetCellRect(ref cellPointer, ColumnWidths[i], rowRect), so, members[i]);
                
                cellPointer += CellPadding;
                
                Rect separator = new Rect(cellPointer - CellPadding / 2f, rowRect.y, SeparatorWidth, rowRect.height);
                EditorGUI.DrawRect(separator, UIUtils.DefaultGUIColor);
            }
        }
        
        private void DrawCell(Rect cellRect, ScriptableObject so, MemberInfo member)
        {
            // We extract the type from the member
            Type type = member is FieldInfo ? ((FieldInfo)member).FieldType : ((PropertyInfo)member).PropertyType; 
            object value = member is FieldInfo ? ((FieldInfo)member).GetValue(so) : ((PropertyInfo)member).GetValue(so);
            
            object newValue = DrawController(cellRect, type, value);

            if (!Equals(value, newValue))
            {
                if (member is FieldInfo) ((FieldInfo)member).SetValue(so, newValue);
                else if (member is PropertyInfo) ((PropertyInfo)member).SetValue(so, newValue);
            }
        }
        
        private Rect GetCellRect(ref float cellPointer, float width, Rect row)
        {
            Rect cellRect = new Rect(cellPointer, row.y, width, row.height);
            cellPointer += width;
            return cellRect;
        }

        private void DrawWidthHandler(Rect rowRect, float cellPointer, int columnIndex)
        {
            Event e = Event.current;
                
            // Clickable handler to drag and resize columns
            Rect handler = new Rect(cellPointer - CellPadding / 2f - HandlerWidth / 2f, rowRect.y, HandlerWidth, RowHeight * (_rowNumber + 1));
            // EditorGUI.DrawRect(handler, Color.black);
                
            // Change mouse icon when hovering
            EditorGUIUtility.AddCursorRect(handler, MouseCursor.ResizeHorizontal);
                
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0 && handler.Contains(e.mousePosition))
                    {
                        _selectedColumn = columnIndex;
                        _dragStartingPosition = e.mousePosition.x;
                        _originalColumnWidth = ColumnWidths[columnIndex];
                        e.Use();
                    }

                    break;
                case EventType.MouseDrag:
                    if (_selectedColumn == columnIndex)
                    {
                        float delta = e.mousePosition.x - _dragStartingPosition;
                        ColumnWidths[columnIndex] = Mathf.Max(_originalColumnWidth + delta, MinColumnWidth);
                        e.Use();
                    }

                    break;
                case EventType.MouseUp:
                    if (_selectedColumn == columnIndex)
                    {
                        _selectedColumn = -1;
                        e.Use();
                    }

                    break;
            }
        }

        private object DrawController(Rect rect, Type type, object value)
        {
            if (type == typeof(bool)) return EditorGUI.Toggle(rect, (bool)value);
            if (type == typeof(int)) return EditorGUI.IntField(rect, (int)value);
            if (type == typeof(long)) return EditorGUI.LongField(rect, (long)value);
            if (type == typeof(float)) return EditorGUI.FloatField(rect, (float)value);
            if (type == typeof(double)) return EditorGUI.DoubleField(rect, (double)value);
            if (type == typeof(string)) return EditorGUI.TextField(rect, (string)value);

            return null;
        }

        private float MinWidthByType(Type type)
        {
            if (type == typeof(bool)) return 50f;
            if (type == typeof(int)) return 50f;
            if (type == typeof(long)) return 50f;
            if (type == typeof(float)) return 50f;
            if (type == typeof(double)) return 50f;
            if (type == typeof(string)) return 150f;

            return 50f;
        }
    }
}