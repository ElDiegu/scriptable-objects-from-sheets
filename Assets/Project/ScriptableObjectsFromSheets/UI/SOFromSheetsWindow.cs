using System;
using System.Collections.Generic;
using ScriptableObjectsFromSheets.Utils;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.UI
{
    public class SOFromSheetsWindow: EditorWindow
    {
        private const float SidebarWidth = 160f;
        
        private List<ISOFromSheetsTab> _tabs;
        private int _selectedTab;

        [MenuItem("ScriptableObjects From Sheets/User Interface")]
        public static void ShowWindow()
        {
            var window = GetWindow<SOFromSheetsWindow>("SOFromSheets");
            window.minSize = new Vector2(1200, 580);
        }

        private void OnEnable()
        {
            _tabs = new List<ISOFromSheetsTab>()
            {
                new AuthWindow(),
                new ScriptableObjectBuilderWindow(this),
                new ScriptableObjectManagerWindow(this)
            };
            
            foreach(var tab in _tabs) tab.OnEnable();
        }

        private void OnDisable()
        {
            if (_tabs == null) return;
            foreach (var tab in _tabs) tab.OnDisable();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();

            DrawSidebar();

            //UIUtils.DrawVerticalLine(Color.black, 1f, 0);

            var contentRect = new Rect(SidebarWidth, 0, position.width - SidebarWidth - 6, position.height);
            EditorGUILayout.BeginVertical(GUILayout.Width(contentRect.width));
            _tabs[_selectedTab].OnGUI(contentRect);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }
        
        private void DrawSidebar()
        {
            const float rowHeight = 17f;

            EditorGUILayout.BeginVertical(GUILayout.Width(SidebarWidth), GUILayout.ExpandHeight(true));

            // Reserve the sidebar's rect within the horizontal layout group
            Rect sidebarRect = GUILayoutUtility.GetRect(SidebarWidth, position.height, GUILayout.ExpandHeight(true));
            EditorGUI.DrawRect(sidebarRect, UIUtils.BackgroundColor);

            float y = sidebarRect.y + 4;

            for (int i = 0; i < _tabs.Count; i++)
            {
                Rect rowRect = new Rect(sidebarRect.x, y, SidebarWidth, rowHeight);
                y += rowHeight;

                bool selected = i == _selectedTab;
                bool hovered = rowRect.Contains(Event.current.mousePosition);

                if (selected) EditorGUI.DrawRect(rowRect, UIUtils.SelectedColor);
                else if (hovered) EditorGUI.DrawRect(rowRect, UIUtils.HoverColor);

                GUI.Label(rowRect, _tabs[i].TabName, selected ? UIUtils.SelectedSidebarLabel : UIUtils.SidebarLabel);

                if (Event.current.type == EventType.MouseDown && rowRect.Contains(Event.current.mousePosition))
                {
                    _selectedTab = i;
                    GUI.FocusControl(null);
                    Event.current.Use();
                    Repaint();
                }
            }

            if (Event.current.type == EventType.MouseMove) Repaint();

            EditorGUILayout.EndVertical();
        }
    }
}