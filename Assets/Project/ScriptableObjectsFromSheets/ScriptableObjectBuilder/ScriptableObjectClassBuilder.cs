using System.Collections.Generic;
using System.IO;
using System.Text;
using Project.SO_Builder;
using ScriptableObjectsFromSheets.Utils;
using SOFromSheets.Controllers;
using SOFromSheets.Extensions;
using UnityEditor;

namespace ScriptableObjectsFromSheets.ScriptableObjectBuilder
{
    public static class ScriptableObjectClassBuilder
    {
        public static void BuildScriptableObjectClass(string className, string outputPath, SheetQuery query)
        {
            var data = GoogleSheetsService.GetRange(query);
            RawSheetData rawSheetData = new RawSheetData(data, query);

            StringBuilder sb = new StringBuilder();

            int indentLevel = 0;
            
            // File headers like imports and class declaration.
            sb.AppendLineWithIndent("using UnityEngine;", indentLevel);
            sb.AppendLineWithIndent("using System.Collections;", indentLevel);
            sb.AppendLineWithIndent("using System.Collections.Generic;", indentLevel);
            sb.AppendLineWithIndent("using System.Linq;", indentLevel);
            sb.AppendLineWithIndent("using SOFromSheets.SOBuilder;", indentLevel);
            sb.AppendLine();
            
            // Namespace derived from folder hierarchy
            sb.AppendLineWithIndent($"namespace {GetNamespaceFromOutputPath(outputPath)}", indentLevel);
            sb.AppendLineWithIndent("{", indentLevel);
            indentLevel++;
            
            sb.AppendLineWithIndent(
                $"[CreateAssetMenuAttribute(menuName = \"GeneratedScriptableObjects/{className}\", fileName = \"{className}\")]", indentLevel);
            sb.AppendLineWithIndent("public class " + className + " : ScriptableObject", indentLevel);
            sb.AppendLineWithIndent("{", indentLevel);
            indentLevel++;

            for (int i = 0; i < rawSheetData.Headers.Count; i++)
            {
                string fieldName = rawSheetData.Headers[i];
                string fieldType = TypeInference.InferTypeAsStringFromValues(rawSheetData.Columns[i]).ToString();

                sb.AppendLineWithIndent($"[SheetImported(\"{rawSheetData.Headers[i]}\")]", indentLevel);
                sb.AppendLineWithIndent($"private {fieldType} {fieldName};", indentLevel);
            }

            indentLevel--;
            sb.AppendLineWithIndent("}", indentLevel);;
            
            indentLevel--;
            sb.AppendLineWithIndent("}", indentLevel);
            
            File.WriteAllText($"{outputPath}/{className}.cs", sb.ToString());
            AssetDatabase.Refresh();
        }

        public static string BuildScriptableObjectString(List<ClassField> classFields, string className, string classNamespace, string fileName, string assetMenuPath)
        {
            StringBuilder sb = new StringBuilder();

            int indentLevel = 0;
            
            // File headers like imports and class declaration.
            sb.AppendLineWithIndent("using UnityEngine;", indentLevel);
            sb.AppendLineWithIndent("using System.Collections;", indentLevel);
            sb.AppendLineWithIndent("using System.Collections.Generic;", indentLevel);
            sb.AppendLineWithIndent("using System.Linq;", indentLevel);
            sb.AppendLineWithIndent("using SOFromSheets.SOBuilder;", indentLevel);
            sb.AppendLine();
            
            // Namespace
            sb.AppendLineWithIndent($"namespace {classNamespace}", indentLevel);
            sb.AppendLineWithIndent("{", indentLevel);
            indentLevel++;
            
            sb.AppendLineWithIndent(
                $"[CreateAssetMenuAttribute(menuName = \"{assetMenuPath}\", fileName = \"{fileName}\")]", indentLevel);
            sb.AppendLineWithIndent($"public class {className} : ScriptableObject", indentLevel);
            sb.AppendLineWithIndent("{", indentLevel);
            indentLevel++;

            foreach (ClassField field in classFields)
            {
                sb.AppendLineWithIndent($"[SheetImported(\"{field.Name}\")]", indentLevel);
                sb.AppendLineWithIndent($"{field.ToString()}", indentLevel);
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
    /// This class is used to store basic information of a basic class field, allowing for an easy modification and management.
    /// </summary>
    public class ClassField
    {
        public string Name;
        public string Type;
        public bool IsList;
        public char ListSeparator;

        public ClassField(string name, string type)
        {
            Name = name;
            Type = type;
            IsList = false;
            ListSeparator = ',';
        }

        public ClassField()
        {
            Name = "NewField";
            Type = "string";
            IsList = false;
            ListSeparator = ',';       
        }

        public override string ToString() => $"public {(IsList ? $"List<{Type}>" : Type)} {Name};";
    }
}