using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SOFromSheets.SOBuilder
{
    /// <summary>
    /// Interface required for the SOFromSheets extension to work. Scriptable Objects must inherit from this class to
    /// be imported from the provided Sheet.
    /// </summary>
    /// <typeparam name="T">
    /// The type where this interface is getting implemented. Must be a Scriptable Object class.
    /// </typeparam>
    public class ImportableSO<T> : ScriptableObject where T : ScriptableObject
    {
        public void Initialize(List<string> headers, List<string> data) 
        {   
            MemberInfo[] members = typeof(T)
                .GetMembers(
                    BindingFlags.Public |
                    BindingFlags.NonPublic |
                    BindingFlags.Instance |
                    BindingFlags.DeclaredOnly)
                .Where(member =>
                    member.MemberType is MemberTypes.Field or MemberTypes.Property &&
                    member.GetCustomAttribute<SheetImportedAttribute>() != null
                ).ToArray();
                
            for (int i = 0; i < members.Length; i++) 
            {
                try 
                {
                    string headerName = members[i].GetCustomAttribute<SheetImportedAttribute>().HeaderName;
                    int headerIndex = InitializationUtils.FindMemberIndex(headerName, headers);
                    
                    Debug.Log($"{headerIndex} | {headerName}");
                    
                    if (headerIndex == -1) throw new ArgumentException($"Member {headerName} is not present in sheet headers.");
                    
                    if (members[i].MemberType == MemberTypes.Field) 
                    {
                        object convertedValue = InitializationUtils.TypeConverter(data[headerIndex], ((FieldInfo)members[i]).FieldType); 
                        ((FieldInfo)members[i]).SetValue(this, convertedValue);
                    }   
                    else if (members[i].MemberType == MemberTypes.Property)
                    {
                        object convertedValue = InitializationUtils.TypeConverter(data[headerIndex], ((PropertyInfo)members[i]).PropertyType); 
                        ((PropertyInfo)members[i]).SetValue(this, convertedValue);
                    }
                }
                catch (ArgumentException ex) 
                {
                    Debug.LogError(ex.Message);
                }
            }
        }
    }
}
