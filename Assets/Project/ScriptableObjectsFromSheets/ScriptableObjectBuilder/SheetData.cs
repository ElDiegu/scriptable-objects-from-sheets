using System.Collections.Generic;
using System.Linq;
using ScriptableObjectsFromSheets.Utils;

namespace Project.SO_Builder
{
    public struct SheetData
    {
        public List<string> Headers;
        public List<List<string>> Rows;
        public List<List<string>> Columns;

        public SheetData(List<List<string>> data)
        {
            Headers = data[0];
            Rows = data.Select(row => row).Skip(1).ToList();
            Columns = DataUtils.Transpose2DMatrix(Rows);
        }
    }

    public struct SheetQuery
    {
        public string Range;
        public string SheetId;
        public string Value;
    }
}