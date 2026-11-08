using System;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.Core
{
    /// <summary>
    /// Editor-only EditorGUI controllers for the types listed in <see cref="SupportedTypesConfiguration"/>.
    /// </summary>
    public static class SupportedTypesControllers
    {
        /// <summary>
        /// Draws the EditorGUI controller for the provided type
        /// </summary>
        /// <param name="rect">Rect to draw the controller in</param>
        /// <param name="type">Type of the controller</param>
        /// <param name="value">Default value shown in the controller</param>
        /// <returns></returns>
        public static object TypeController(Rect rect, Type type, object value)
        {
            if (type == typeof(bool)) return EditorGUI.Toggle(rect, (bool)value);
            if (type == typeof(int)) return EditorGUI.IntField(rect, (int)value);
            if (type == typeof(long)) return EditorGUI.LongField(rect, (long)value);
            if (type == typeof(float)) return EditorGUI.FloatField(rect, (float)value);
            if (type == typeof(double)) return EditorGUI.DoubleField(rect, (double)value);
            if (type == typeof(string)) return EditorGUI.TextField(rect, (string)value);
            if (type == typeof(Vector2)) return EditorGUI.Vector2Field(rect, GUIContent.none, (Vector2)value);
            if (type == typeof(Vector3)) return EditorGUI.Vector3Field(rect, GUIContent.none, (Vector3)value);
            if (type == typeof(Color)) return EditorGUI.ColorField(rect, (Color)value);

            return null;
        }
    }
}
