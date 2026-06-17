using System.Text;

namespace ScriptableObjectsFromSheets.Extensions
{
    public static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineWithIndent(this StringBuilder sb, string line, int indentLevel)
        {
            for (int i = 0; i < indentLevel; i++) sb.Append('\t');
            sb.AppendLine(line);
            return sb;
        }
    }
}