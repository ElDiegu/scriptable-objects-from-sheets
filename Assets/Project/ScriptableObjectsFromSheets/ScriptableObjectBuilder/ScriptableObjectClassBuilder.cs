using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ScriptableObjectsFromSheets.Extensions;
using UnityEditor;

namespace ScriptableObjectsFromSheets.ScriptableObjectBuilder
{
    public static class ScriptableObjectClassBuilder
    {
        public static string BuildScriptableObjectString(List<ClassField> classFields, string className, string classNamespace, string fileName, string assetMenuPath)
        {
            StringBuilder sb = new StringBuilder();

            int indentLevel = 0;
            
            // File headers like imports and class declaration.
            sb.AppendLineWithIndent("using UnityEngine;", indentLevel);
            sb.AppendLineWithIndent("using System.Collections;", indentLevel);
            sb.AppendLineWithIndent("using System.Collections.Generic;", indentLevel);
            sb.AppendLineWithIndent("using System.Linq;", indentLevel);
            sb.AppendLineWithIndent("using ScriptableObjectsFromSheets.Core;", indentLevel);
            sb.AppendLine();
            
            // Namespace
            sb.AppendLineWithIndent($"namespace {classNamespace}", indentLevel);
            sb.AppendLineWithIndent("{", indentLevel);
            indentLevel++;
            
            sb.AppendLineWithIndent(
                $"[CreateAssetMenuAttribute(menuName = \"{assetMenuPath}\", fileName = \"{fileName}\")]", indentLevel);
            sb.AppendLineWithIndent($"public class {className} : ImportableSO<{className}>", indentLevel);
            sb.AppendLineWithIndent("{", indentLevel);
            indentLevel++;

            foreach (ClassField field in classFields)
            {
                if (field.IsSheetImported) sb.AppendLineWithIndent(field.AttributeDeclaration, indentLevel);
                sb.AppendLineWithIndent(field.FieldDeclaration, indentLevel);
            }

            indentLevel--;
            sb.AppendLineWithIndent("}", indentLevel);;
            
            indentLevel--;
            sb.AppendLineWithIndent("}", indentLevel);

            return sb.ToString();
        }

        public static void GenerateScriptableObjectClass(string classString, string outputPath, string className)
        {
            File.WriteAllText($"{outputPath}/{className}.cs", classString);
            AssetDatabase.Refresh();
        }

        public static string GetNamespaceFromOutputPath(string outputPath)
        {
            string truncatedOutputPath = outputPath.Replace("Assets/", "");
            string rootNamespace = !string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace) ? $"{EditorSettings.projectGenerationRootNamespace}." : "";
            return $"{rootNamespace}{truncatedOutputPath.Replace("/", ".").Replace(" ", "")}";
        }
    }

    /// <summary>
    /// This class is used to store basic information of a basic class field, allowing for an easy modification and management
    /// </summary>
    public class ClassField
    {
        public string Name;
        public string Type;
        public bool IsList;
        public char ListSeparator;
        public bool IsSheetImported;
        private readonly string _headerName;

        public ClassField(string headerName, string type)
        {
            Name = SanitizeName(headerName);
            _headerName = headerName;
            Type = type;
            IsList = false;
            ListSeparator = ',';
            IsSheetImported = true;       
        }

        public ClassField()
        {
            Name = "NewField";
            Type = "string";
            IsList = false;
            ListSeparator = ',';     
            IsSheetImported = true;     
        }
        
        public string FieldDeclaration => $"public {(IsList ? $"List<{Type}>" : Type)} {Name};";
        public string AttributeDeclaration => $"[SheetImported(\"{EscapeForStringLiteral(_headerName ?? Name)}\"{(IsList ? $", \"{ListSeparator}\"" : "")})]";

        /// <summary>
        /// Clears header names from spaces and special characters to obtain a valid field name
        /// </summary>
        /// <param name="header">Header cell of the sheet range</param>
        /// <returns>A sanitized string valid for field declaration</returns>
        public static string SanitizeName(string header)
        {
            string name = new string((header ?? "").Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());

            if (name.Length == 0) return "Field";
            return char.IsDigit(name[0]) ? $"_{name}" : name;
        }

        private static string EscapeForStringLiteral(string value) => value.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }
}