using System;
using System.Reflection;
using UnityEngine;

public static class ObjectExtension
{

    public static object GetFieldValue(this object self, string fieldName)
    {
        Type type = self.GetType();
        FieldInfo field = type.GetFieldEx(fieldName);
        if (field == null)
        {
            //Debug.LogError("Cannot find field '" + fieldName + "' in '" + type.Name + "' class");
            return default;
        }
        return field.GetValue(self);
    }
    public static T GetFieldValue<T>(this object self, string fieldName)
    {
        Type type = self.GetType();
        FieldInfo field = type.GetFieldEx(fieldName);
        if (field == null)
        {
            //Debug.LogError("Cannot find field '" + fieldName + "' in '" + type.Name + "' class");
            return default(T);
        }
        return (T)field.GetValue(self);
    }

    public static void SetFieldValue<T>(this object self, string fieldName, T fieldValue)
    {
        Type type = self.GetType();
        FieldInfo field = type.GetFieldEx(fieldName);
        if (field == null)
        {
            Debug.LogError("Cannot find field '" + fieldName + "' in '" + type.Name + "' class");
            return;
        }
        field.SetValue(self, fieldValue);
    }

    public static T GetPropertyValue<T>(this object self, string propertyName)
    {
        Type type = self is Type ? self as Type : self.GetType();
        PropertyInfo property = type.GetPropertyEx(propertyName);
        if (property == null)
        {
            Debug.LogError("Cannot find property '" + propertyName + "' in '" + type.Name + "' class");
            return default(T);
        }
        if (!property.CanRead)
        {
            Debug.LogError("Property '" + propertyName + "' is not readable!");
            return default(T);
        }
        return (T)property.GetValue(self is Type ? null : self, null);
    }

    public static void SetPropertyValue<T>(this object self, string propertyName, T propertyValue)
    {
        Type type = self is Type ? self as Type : self.GetType();
        PropertyInfo property = type.GetPropertyEx(propertyName);
        if (property == null)
        {
            Debug.LogError("Cannot find property '" + propertyName + "' in '" + type.Name + "' class");
            return;
        }
        if (!property.CanWrite)
        {
            Debug.LogError("Property '" + propertyName + "' is not writable!");
            return;
        }
        property.SetValue(self is Type ? null : self, propertyValue, null);
    }

    public static T Invoke<T>(this object self, string methodName, params object[] args)
    {
        Type[] types = new Type[args.Length];
        for (int i = 0; i < args.Length; ++i)
        {
            types[i] = args[i].GetType();
        }
        Type type = self is Type ? self as Type : self.GetType();
        MethodInfo method = type.GetMethodEx(methodName, types);
        if (method == null)
        {
            Debug.LogError("Cannot find method '" + methodName + "' in '" + type.Name + "' class");
            return default(T);
        }
        return (T)method.Invoke(self is Type ? null : self, args);
    }
}
