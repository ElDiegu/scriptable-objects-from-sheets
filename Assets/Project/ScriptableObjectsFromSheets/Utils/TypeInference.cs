using System;
using System.Collections.Generic;
using System.Linq;

namespace ScriptableObjectsFromSheets.Utils
{
    public static class TypeInference
    {
        private static readonly List<(String type, Func<string, bool> ParseDelegate)> TypeCandidates = new()
        {
            ("int", data => int.TryParse(data, out _)),
            ("float", data => float.TryParse(data, out _)),
            ("bool", data => bool.TryParse(data, out _))
        };
        
        /// <summary>
        /// Tries to infer the type of a set of values.
        /// </summary>
        /// <param name="dataSet">Data set ot infer the data from.</param>
        /// <returns></returns>
        public static String InferTypeAsStringFromValues(List<string> dataSet)
        {
            List<string> cleanedDataSet = dataSet.Where(dataEntry => !string.IsNullOrEmpty(dataEntry)).ToList();

            if (cleanedDataSet.Count == 0) return "string";

            foreach (var candidate in TypeCandidates)
            {
                if (cleanedDataSet.All(candidate.ParseDelegate)) return candidate.type;
            }
            
            return "string";
        }
    }
}