using System;
using UnityEngine;

namespace ScriptableObjectsFromSheets.ScriptableObjectManager.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SheetImportedAttribute : Attribute
    {
        /// <summary>
        /// This name will be used to initialize the variable. It must match with the value's column first cell.
        /// </summary>
        public string HeaderName { get; }
        
        /// <summary>
        /// Stores the declared separator for List types. @ is default.
        /// </summary>
        public char Separator { get; }
        
        public SheetImportedAttribute(string headerName, char separator = '@') 
        {
            HeaderName = headerName;
            Separator = separator;
        }
    }
}
