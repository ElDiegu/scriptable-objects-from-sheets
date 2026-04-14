using System.Collections.Generic;

namespace ScriptableObjectsFromSheets.Utils
{
    public static class DataUtils
    {
        public static List<List<string>> Transpose2DMatrix(List<List<string>> matrix)
        {
            var transposedMatrix = new List<List<string>>();
            
            for (int i = 0; i < matrix[0].Count; i++)
            {
                transposedMatrix.Add(new List<string>());
                for (int j = 0; j < matrix.Count; j++)
                {
                    transposedMatrix[i].Add(matrix[j][i]);
                }
            }
            
            return transposedMatrix;
        }
    }
}