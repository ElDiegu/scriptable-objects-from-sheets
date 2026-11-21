using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace ScriptableObjectsFromSheets.Utils
{
    public static class EnumUtils
    {
        // Cache to avoid scanning the project constantly since it was causing performance issues
        private static readonly Dictionary<string, Type> EnumTypeCache = new Dictionary<string, Type>();

        /// <summary>
        /// Resolves the enum type defined by a script.
        /// </summary>
        /// <returns>The enum type, or null if the script is null or doesn't define an enum.</returns>
        public static Type ResolveEnumType(MonoScript script)
        {
            if (script == null) return null;

            if (EnumTypeCache.TryGetValue(script.name, out Type cached)) return cached;

            Type resolved = FindEnumType(script.name);
            EnumTypeCache[script.name] = resolved;
            return resolved;
        }

        private static Type FindEnumType(string typeName)
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types;
                try { types = assembly.GetTypes(); }
                catch (ReflectionTypeLoadException e) { types = e.Types.Where(t => t != null).ToArray(); }

                foreach (var type in types)
                    if (type.IsEnum && type.Name == typeName) return type;
            }

            return null;
        }
    }
}
