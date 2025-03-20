using System.Text;
using Google.Apis.Sheets.v4;
using SOFromSheets.Controllers;
using SOFromSheets.Integration;
using SOFromSheets.SOBuilder;
using UnityEditor;
using UnityEngine;

namespace SOFromSheets
{
    public class TestingController : MonoBehaviour 
    {
        public string range;
        public string sheetId;
        public string value;
    }

    [CustomEditor(typeof(TestingController))]
    public class TestingControllerEditor : Editor
    {
        public override void OnInspectorGUI() 
        {
            DrawDefaultInspector();

            TestingController testingController = (TestingController)target;

            if (GUILayout.Button("Get Range")) 
            {
                foreach (var row in GoogleSheetsService.GetRange(testingController.sheetId, testingController.range)) 
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var cell in row) sb.Append(cell + "\t");
                    Debug.Log(sb.ToString());
                }
            } 

            if (GUILayout.Button("Set Cell")) GoogleSheetsService.UpdateCell(testingController.sheetId, testingController.range, testingController.value);

            if (GUILayout.Button("Generate SO")) 
                ScriptableObjectManager.GenerateScriptableObjectsFromRange<TestingSO>(testingController.sheetId, testingController.range, "Assets/Project/Resources/ScriptableObjects");
        }
    }
}
