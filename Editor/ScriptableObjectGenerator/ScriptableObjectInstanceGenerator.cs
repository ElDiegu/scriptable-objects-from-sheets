using System;
using System.Collections.Generic;
using System.Reflection;
using ScriptableObjectsFromSheets.APIIntegration;
using ScriptableObjectsFromSheets.Core;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.ScriptableObjectGenerator
{
    public static class ScriptableObjectInstanceGenerator
    {
        private static void GenerateScriptableObjects<T>(List<List<string>> data, string path) where T : ImportableSO<T>
        {
            for (int i = 1; i < data.Count; i ++) 
            {
                T asset = ScriptableObject.CreateInstance<T>();
                asset.Initialize(data[0], data[i]);

                AssetDatabase.CreateAsset(asset, path + $"/{typeof(T).ToString().Split('.')[1]}_{i}.asset");
                AssetDatabase.SaveAssets();
            }
        }

        public static void GenerateScriptableObjectsFromRange<T>(SheetQuery query, string outputPath) where T : ImportableSO<T>
        {
            var data = GoogleSheetsService.GetRange(query);
            if (data == null) return;

            GenerateScriptableObjects<T>(data, outputPath);
        }

        public static void GenerateScriptableObjectsFromQuery(SheetQuery query, string outputPath, Type type)
        {
            
            MethodInfo method = typeof(ScriptableObjectInstanceGenerator)
                .GetMethod(nameof(ScriptableObjectInstanceGenerator.GenerateScriptableObjectsFromRange))
                ?.MakeGenericMethod(type);
                    
            method.Invoke(null, new object[] {query, outputPath});
        }
    }
}
