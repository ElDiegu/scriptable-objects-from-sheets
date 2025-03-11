using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SOFromSheets
{
    public class InitializationUtils
    {
        public static void InitializeFields<T>(object caller, List<string> data) where T : ScriptableObject 
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < fields.Length; i++) 
            {
                FieldInfo field = fields[i];

                try 
                {
                    field.SetValue(caller, Convert.ChangeType(data[i], field.FieldType));
                }
                catch (Exception ex) 
                {
                    Debug.LogError($"Failed to initialize the Scriptable Object in {field.Name} with value {data[i]}: {ex.Message}");
                }
            }
        }

        public static void InitializeProperties<T>(object caller, List<string> data) where T : ScriptableObject 
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            Debug.Log(properties.Length);

            for (int i = 0; i < properties.Length; i++) 
            {
                PropertyInfo property = properties[i];

                try 
                {
                    property.SetValue(caller, Convert.ChangeType(data[i], property.PropertyType));
                }
                catch (Exception ex) 
                {
                    Debug.LogError($"Failed to initialize the Scriptable Object in {property.Name} with value {data[i]}: {ex.Message}");
                }
            }
        }
    }
}
