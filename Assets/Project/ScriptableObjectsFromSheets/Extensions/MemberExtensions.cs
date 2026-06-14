using System;
using System.Reflection;

namespace ScriptableObjectsFromSheets.Extensions
{
    public static class MemberExtensions
    {
        public static Type GetMemberType(this MemberInfo memberInfo)
        {
            return memberInfo is FieldInfo ? ((FieldInfo)memberInfo).FieldType : ((PropertyInfo)memberInfo).PropertyType;
        }
    }
}