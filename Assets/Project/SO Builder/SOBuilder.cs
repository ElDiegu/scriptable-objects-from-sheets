using System.Collections.Generic;
using SOFromSheets.Controllers;
using SOFromSheets.Integration;
using UnityEditor;
using UnityEngine;

namespace SOFromSheets.SOBuilder
{
    public class ScriptableObjectManager
    {
        public static void GenerateScriptableObjects<T>(List<List<string>> data, string path) where T : ImportableSO<T>
        {
            for (int i = 1; i < data.Count; i ++) 
            {
                T asset = ScriptableObject.CreateInstance<T>();
                asset.Initialize(data[0], data[i]);

                AssetDatabase.CreateAsset(asset, path + $"/{typeof(T).ToString().Split('.')[1]}_{i}.asset");
                AssetDatabase.SaveAssets();
            }
        }

        public static void GenerateScriptableObjectsFromRange<T>(string sheetId, string range, string path) where T : ImportableSO<T>
        {
            var data = GoogleSheetsService.GetRange(sheetId, range);

            GenerateScriptableObjects<T>(data, path);
        }
    }
}
