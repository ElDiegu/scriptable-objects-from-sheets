using System.Collections.Generic;
using System.Linq;
using ScriptableObjectsFromSheets.Utils;

namespace Project.SO_Builder
{
    /// <summary>
    /// Raw data from the queried sheet range.
    /// </summary>
    public class RawSheetData
    {
        public SheetQuery SourceQuery;
        public readonly string SheetName;
        public readonly List<string> Headers;
        public readonly List<List<string>> Rows;
        public readonly List<List<string>> Columns;

        public RawSheetData(List<List<string>> data, SheetQuery sourceQuery)
        {
            Headers = data[0];
            Rows = data.Select(row => row).Skip(1).ToList();
            Columns = DataUtils.Transpose2DMatrix(Rows);
            SourceQuery = sourceQuery;
            SheetName = SourceQuery.GetSheetName();
        }
    }

    /// <summary>
    /// Processed data. Columns are transformed into fields with a name and a type as strings.
    /// </summary>
    public class ProcessedSheetData
    {
        public string ClassName;
        public Dictionary<string, string> Fields = new Dictionary<string, string>();

        public ProcessedSheetData(RawSheetData rawSheetData)
        {
            ClassName = rawSheetData.SheetName;

            for (int i = 0; i < rawSheetData.Headers.Count; i++)
            {
                Fields.Add(rawSheetData.Headers[i], TypeInference.InferTypeAsStringFromValues(rawSheetData.Columns[i]));
            }
        }
    }

    public struct SheetQuery
    {
        public string Range;
        public string SheetId;
        public string Value;

        public static string ExtractSheetId(string url) => url.Split('/')[5];
        public string GetSheetName() => Range.Split('!')[0];
    }
}