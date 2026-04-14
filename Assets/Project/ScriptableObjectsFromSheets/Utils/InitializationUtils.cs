using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SOFromSheets
{
    public class InitializationUtils
    {        
        // TODO: Let the user choose the separator string through interface windows
        public static readonly string separatorString = "@"; 
        public static int FindMemberIndex(string name, List<string> headers) => headers.IndexOf(name);
        
        public static object TypeConverter(string value, Type conversionType) 
        {
            if (conversionType == typeof(bool)) 
            {
                Debug.Log("bool");
                if (int.TryParse(value, out int intValue)) return Convert.ChangeType(intValue, conversionType);
            }
            
            if (conversionType != typeof(string) && conversionType.GetInterface("IEnumerable`1") != null) 
            {
                Type enumerableType = conversionType.GetInterface("IEnumerable`1").GetGenericArguments()[0];
                
                // Initialize a List of the desired type to store all elements from the Enumerable as desired type
                IList elementList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(enumerableType));
                foreach (string element in value.Split(separatorString)) elementList.Add(TypeConverter(element, enumerableType));
                
                return Activator.CreateInstance(conversionType, elementList);
            }
            
            Debug.Log($"Converting {value} to {conversionType}");
            return Convert.ChangeType(value, conversionType);
        }
    }
}
