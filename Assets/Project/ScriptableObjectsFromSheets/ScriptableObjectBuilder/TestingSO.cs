using System.Collections.Generic;
using SOFromSheets.SOBuilder;
using UnityEngine;

namespace SOFromSheets
{
    public class TestingSO : ImportableSO<TestingSO>
    {
        [field: SerializeField]
        [SheetImported("ID")]
        public int ID { get; set; }

        [field: SerializeField]
        [SheetImported("Name")]
        public string Name { get; set; }

        [field: SerializeField]
        [SheetImported("Value")]
        public float Value { get; set; }

        [SerializeField]
        [SheetImported("TestField")]
        private bool testField;
        
        [field: SerializeField]
        public string NotImported { get; set; }
        
        [field: SerializeField]
        [SheetImported("List")]
        public List<string> ListTry { get; set; }
    }
}
