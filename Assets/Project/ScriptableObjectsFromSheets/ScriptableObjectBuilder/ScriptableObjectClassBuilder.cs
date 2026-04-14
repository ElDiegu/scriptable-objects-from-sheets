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
            SheetData sheetData = new SheetData(data);

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
            sb.AppendLineWithIndent($"namespace {GetNamespace(outputPath)}", indentLevel);
            sb.AppendLineWithIndent("{", indentLevel);
            indentLevel++;
            
            sb.AppendLineWithIndent(
                $"[CreateAssetMenuAttribute(menuName = \"GeneratedScriptableObjects/{className}\", fileName = \"{className}\")]", indentLevel);
            sb.AppendLineWithIndent("public class " + className + " : ScriptableObject", indentLevel);
            sb.AppendLineWithIndent("{", indentLevel);
            indentLevel++;

            for (int i = 0; i < sheetData.Headers.Count; i++)
            {
                string fieldName = sheetData.Headers[i];
                string fieldType = TypeInference.InferTypeAsStringFromValues(sheetData.Columns[i]).ToString();

                sb.AppendLineWithIndent($"[SheetImported(\"{sheetData.Headers[i]}\")]", indentLevel);
                sb.AppendLineWithIndent($"private {fieldType} {fieldName};", indentLevel);
            }

            indentLevel--;
            sb.AppendLineWithIndent("}", indentLevel);;
            
            indentLevel--;
            sb.AppendLineWithIndent("}", indentLevel);
            
            File.WriteAllText($"{outputPath}/{className}.cs", sb.ToString());
            AssetDatabase.Refresh();
        }

        public static string GetNamespace(string outputPath)
        {
            string rootNamespace = !string.IsNullOrEmpty(EditorSettings.projectGenerationRootNamespace) ? $"{EditorSettings.projectGenerationRootNamespace}." : "";
            return $"{rootNamespace}{outputPath.Replace("/", ".").Replace(" ", "")}";
        }
    }
}