using ScriptableObjectsFromSheets.Core;
using UnityEngine;

namespace ScriptableObjectsFromSheets.Test{
    [CreateAssetMenu(fileName = "TestSO", menuName = "Scriptable Objects/TestSO")]
    public class TestSO : ImportableSO<TestSO>
    {
        [SheetImported("ID")]
        public int ID;
        [SheetImported("Name")]
        public string Name;

        [SheetImported("TestField")] public int TestField;
        [SheetImported("Value")]
        public float Value;
        [SheetImported("Active")]
        public bool Active;

        public string Test2;
        public string Test3;
        public string Test4;
        public string Test5;
        public string Test6;
    }
}

