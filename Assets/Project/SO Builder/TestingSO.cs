using System.Collections.Generic;
using SOFromSheets.SOBuilder;
using UnityEngine;

namespace SOFromSheets
{
    public class TestingSO : ImportableSO<TestingSO>
    {
        [field: SerializeField]
        public int ID { get; set; }

        [field: SerializeField]
        public string Name { get; set; }

        [field: SerializeField]
        public float Value { get; set; }
    }
}
