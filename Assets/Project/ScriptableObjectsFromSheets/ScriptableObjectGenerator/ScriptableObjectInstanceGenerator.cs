using System.Collections.Generic;
using Project.SO_Builder;
using SOFromSheets.Controllers;
using SOFromSheets.Integration;
using UnityEditor;
using UnityEngine;

namespace ScriptableObjectsFromSheets.ScriptableObjectManager
{
    public static class ScriptableObjectInstanceGenerator
    {
        public static void GenerateScriptableObjects<T>(List<List<string>> data, string path) where T : ImportableSO<T>
        {
            for (int i = 1; i < data.Count; i ++) 
            {
                T asset = ScriptableObject.CreateInstance<T>();
                asset.Initialize(data[0], data[i]);

                Debug.Log(typeof(T));

                AssetDatabase.CreateAsset(asset, path + $"/{typeof(T).ToString().Split('.')[1]}_{i}.asset");
                AssetDatabase.SaveAssets();
            }
        }

        public static void GenerateScriptableObjectsFromRange<T>(SheetQuery query, string outputPath) where T : ImportableSO<T>
        {
            var data = GoogleSheetsService.GetRange(query);

            GenerateScriptableObjects<T>(data, outputPath);
        }
    }
}
