using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SOFromSheets.SOBuilder
{
    /// <summary>
    /// Interface required for the SOFromSheets extension to work. This Interface must be added to those Scriptable Object classes
    /// that will be imported through the extension.
    /// </summary>
    /// <typeparam name="T">The type where this interface is getting implemented. Must be an Scriptable Object class.</typeparam>
    public interface IImportableSO<T> where T : ScriptableObject
    {
        public void Initialize(List<string> data) 
        {
            Debug.Log($"initializing Scriptable Object of type {typeof(T).Name}");

            InitializationUtils.InitializeProperties<T>(this, data);
        }
    }
}
