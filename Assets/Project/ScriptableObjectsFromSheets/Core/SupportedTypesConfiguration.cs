using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.Core
{
    /// <summary>
    /// Contains the supported data types and their corresponding parsing delegates for inferring data types from string values.
    /// as well as other required details and configurations.
    /// </summary>
    public static class SupportedTypesConfiguration
    {
        private const char VectorSeparator = ';';
        
        public static readonly List<(String type, Func<string, bool> ParseDelegate)> TypeCandidates = new()
        {
            ("int", data => int.TryParse(data, out _)),
            ("long", data => long.TryParse(data, out _)),
            ("float", data => float.TryParse(data, out _)),
            ("double", data => double.TryParse(data, out _)),
            ("bool", data => bool.TryParse(data, out _)),
            ("Vector2", data => TryParseVector(data, 2, out _)),
            ("Vector3", data => TryParseVector(data, 3, out _)),
            ("Color", data => ColorUtility.TryParseHtmlString(data.Trim(), out _)),
            ("string", data => true)
        };

        /// <summary>
        /// Parsers for the types that <see cref="Convert.ChangeType(object, Type)"/> can't handle.
        /// </summary>
        private static readonly Dictionary<Type, Func<string, object>> CustomParsers = new()
        {
            { typeof(Vector2), data => TryParseVector(data, 2, out float[] c) ? new Vector2(c[0], c[1]) : throw new ArgumentException($"'{data}' is not a valid Vector2.") },
            { typeof(Vector3), data => TryParseVector(data, 3, out float[] c) ? new Vector3(c[0], c[1], c[2]) : throw new ArgumentException($"'{data}' is not a valid Vector3.") },
            { typeof(Color), data => ColorUtility.TryParseHtmlString(data.Trim(), out Color color) ? color : throw new ArgumentException($"'{data}' is not a valid Color.") }
        };

        /// <summary>
        /// Tries to convert a string to a type that needs custom parsing (Vector2, Vector3, Color).
        /// </summary>
        /// <returns>True if the type has a custom parser. Throws ArgumentException if the value is malformed.</returns>
        public static bool TryCustomParse(string data, Type type, out object result)
        {
            if (CustomParsers.TryGetValue(type, out var parser))
            {
                result = parser(data);
                return true;
            }

            result = null;
            return false;
        }

        private static bool TryParseVector(string data, int components, out float[] values)
        {
            values = new float[components];

            string[] parts = data.Trim().Trim('(', ')').Split(VectorSeparator);
            if (parts.Length != components) return false;

            for (int i = 0; i < components; i++)
            {
                if (!float.TryParse(parts[i].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out values[i])) return false;
            }

            return true;
        }
        
        /// <summary>
        /// Tries to infer the type of a set of values and return it as a string. Used by the Scriptable Object Builder.
        /// </summary>
        /// <param name="dataSet">Data set ot infer the data from.</param>
        /// <returns></returns>
        public static String InferTypeAsStringFromValues(List<string> dataSet)
        {
            List<string> cleanedDataSet = dataSet.Where(dataEntry => !string.IsNullOrEmpty(dataEntry)).ToList();

            if (cleanedDataSet.Count == 0) return "string";

            foreach (var candidate in TypeCandidates)
            {
                if (cleanedDataSet.All(candidate.ParseDelegate)) return candidate.type;
            }
            
            return "string";
        }

        /// <summary>
        /// Returns a HashSet with all the namespaces required to compile a type.
        /// </summary>
        /// <param name="type">The type for which to return import namespaces.</param>
        /// <returns>A HashSet containing the required namespace strings.</returns>
        public static HashSet<string> ImportsByType(Type type)
        {
            switch (type)
            {
                default:
                    return new HashSet<string>();
            }
        }
        
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