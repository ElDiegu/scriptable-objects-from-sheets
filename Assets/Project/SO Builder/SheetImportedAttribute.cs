using System;
using UnityEngine;

namespace SOFromSheets.SOBuilder
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class SheetImportedAttribute : Attribute
    {
        /// <summary>
        /// This name will be used to intialize the variable. It must match with the value's column first cell.
        /// </summary>
        public string HeaderName { get; }
        
        public SheetImportedAttribute(string headerName) 
        {
            HeaderName = headerName;
        }
    }
}
