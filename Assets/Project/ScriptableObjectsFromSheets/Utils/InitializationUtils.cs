using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjectsFromSheets.Utils
{
    public static class InitializationUtils
    {
        public static int FindMemberIndex(string name, List<string> headers) => headers.IndexOf(name);
        
        public static object TypeConverter(string value, Type conversionType, char separator) 
        {
            if (conversionType == typeof(bool)) 
            {
                Debug.Log("bool");
                if (int.TryParse(value, out int intValue)) return Convert.ChangeType(intValue, conversionType);
            }
            
            if (conversionType != typeof(string) && conversionType.GetInterface("IEnumerable`1") != null) 
            {
                Type enumerableType = conversionType.GetInterface("IEnumerable`1").GetGenericArguments()[0];
                
                // Initialize a List of the desired type to store all elements from the Enumerable
                IList elementList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(enumerableType));
                foreach (string element in value.Split(separator)) elementList.Add(TypeConverter(element, enumerableType, separator));
                
                return Activator.CreateInstance(conversionType, elementList);
            }
            
            Debug.Log($"Converting {value} to {conversionType}");
            return Convert.ChangeType(value, conversionType);
        }
    }
}
