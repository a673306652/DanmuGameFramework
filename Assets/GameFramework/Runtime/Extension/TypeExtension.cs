using System;
using System.Reflection;
using UnityEngine;

public static class TypeExtension {

    public static FieldInfo GetFieldEx(this Type self, string fieldName) {
#if NETFX_CORE
        FieldInfo field = self.GetRuntimeField(fieldName);
#else
        FieldInfo field = self.GetField(fieldName);
#endif
        return field;
    }

    public static PropertyInfo GetPropertyEx(this Type self, string propertyName) {
#if NETFX_CORE
        PropertyInfo property = self.GetRuntimeProperty(propertyName);
#else
        PropertyInfo property = self.GetProperty(propertyName);
#endif
        return property;
    }

    public static MethodInfo GetMethodEx(this Type self, string methodName, params Type[] argTypes) {
        MethodInfo method = null;
        if (argTypes.Length > 0) {
#if NETFX_CORE
            method = self.GetRuntimeMethod(methodName, argTypes);
#else
            method = self.GetMethod(methodName, argTypes);
#endif
        } else {
#if NETFX_CORE
            method = self.GetType().GetRuntimeMethod(methodName);
#else
            method = self.GetType().GetMethod(methodName);
#endif
        }
        return method;
    }

}
