using System;
using System.Collections.Generic;
using System.Reflection;

#if NETFX_CORE
/// <summary>Specifies flags that control binding and the way in which the search for members and types is conducted by reflection.</summary>
[Flags]
public enum BindingFlags
{
    IgnoreCase = 1,
    DeclaredOnly = 2,
    Instance = 4,
    Static = 8,
    Public = 16,
    NonPublic = 32,
    FlattenHierarchy = 64,
}
#endif

public static class WsaReflectionExtensions
{
    public static Type AsType(this Type type)
    {
        return type;
    }
#if !NETFX_CORE
    public static Delegate CreateDelegate(this MethodInfo methodInfo, Type delegateType, object instance)
    {
        return Delegate.CreateDelegate(delegateType, instance, methodInfo);
    }
    public static Assembly GetAssembly(this Type type)
    {
        return type.Assembly;
    }
    public static Type GetTypeInfo(this Type type)
    {
        return type;
    }
    public static string GetDelegateName(this Delegate delegateInstance)
    {
        return delegateInstance.Method.Name;
    }
#else
    public static bool IsAssignableFrom(this Type type, Type other)
    {
        return type.GetTypeInfo().IsAssignableFrom(other.GetTypeInfo());
    }
    public static bool IsAssignableFrom(this Type type, TypeInfo other)
    {
        return type.GetTypeInfo().IsAssignableFrom(other);
    }
    public static Assembly GetAssembly(this Type type)
    {
        return type.GetTypeInfo().Assembly;
    }
    public static bool IsInstanceOfType(this Type type, object obj)
    {
        return obj != null && type.GetTypeInfo().IsAssignableFrom(obj.GetType().GetTypeInfo());
    }
    public static string GetDelegateName(this Delegate delegateInstance)
    {
        return delegateInstance.ToString();
    }
    public static MethodInfo GetMethod(this Type type, string methodName)
    {
        return type.GetTypeInfo().GetDeclaredMethod(methodName);
    }
    public static IEnumerable<FieldInfo> GetFields(this TypeInfo typeInfo)
    {
        return typeInfo.DeclaredFields;
    }
    public static TypeInfo GetTypeInfo(this TypeInfo typeInfo)
    {
        return typeInfo;
    }
    public static IEnumerable<ConstructorInfo> GetConstructors(this TypeInfo typeInfo)
    {
        return typeInfo.DeclaredConstructors;
    }
    public static IEnumerable<MethodInfo> GetMethods(this TypeInfo typeInfo, BindingFlags ignored)
    {
        return typeInfo.DeclaredMethods;
    }
    public static IEnumerable<TypeInfo> GetTypes(this Assembly assembly)
    {
        return assembly.DefinedTypes;
    }
#endif
}
