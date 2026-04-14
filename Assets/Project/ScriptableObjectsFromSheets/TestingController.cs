using System.Text;
using Google.Apis.Sheets.v4;
using Project.SO_Builder;
using ScriptableObjectsFromSheets.ScriptableObjectBuilder;
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

            var query = new SheetQuery()
            {
                Range = testingController.range,
                SheetId = testingController.sheetId,
                Value = testingController.value
            };

            if (GUILayout.Button("Generate Scriptable Objects"))
            {
                ScriptableObjectClassBuilder.BuildScriptableObjectClass("TestSOClass", "Assets/Project/Resources/Scriptable Objects", query);
            }

        }
    }
}
