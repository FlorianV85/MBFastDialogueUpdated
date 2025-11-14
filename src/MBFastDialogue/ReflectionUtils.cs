using System;
using System.Collections.Generic;
using System.Reflection;

namespace MBFastDialogue
{
    public static class ReflectionUtils
    {
        private static readonly Dictionary<string, FieldInfo> _fieldCache = new Dictionary<string, FieldInfo>();
        private static readonly Dictionary<string, MethodInfo> _methodCache = new Dictionary<string, MethodInfo>();
        private static readonly object _cacheLock = new object();
        
        public static T ForceGet<T>(object obj, string fieldName)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var cacheKey = $"{obj.GetType().FullName}.{fieldName}";
            if (!_fieldCache.TryGetValue(cacheKey, out var field))
            {
                lock (_cacheLock)
                {
                    if (!_fieldCache.TryGetValue(cacheKey, out field))
                    {
                        field = FindField(obj.GetType(), fieldName);
                        if (field == null)
                        {
                            throw new MissingFieldException(
                                $"Field '{fieldName}' not found on type '{obj.GetType().FullName}'");
                            _fieldCache[cacheKey] = field;
                        }
                    }
                }
            }
            /*FieldInfo? field = null;
            var baseType = obj.GetType();
            while (field == null)
            {
                field = baseType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
                baseType = baseType.BaseType;
                if (baseType == null) break;
            }*/
            return (T)field.GetValue(obj);
        }

        public static T ForceCall<T>(object obj, string methodName, object[] args)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));
            var cacheKey = $"{obj.GetType().FullName}.{methodName}";
            if (!_methodCache.TryGetValue(cacheKey, out var method))
            {
                lock (_cacheLock)
                {
                    if (!_methodCache.TryGetValue(cacheKey, out method))
                    {
                        method = FindMethod(obj.GetType(), methodName);
                        if (method == null)
                        {
                            throw new MissingMethodException($"Method '{methodName}' not found on type '{obj.GetType().FullName}'");
                        }
                        _methodCache[cacheKey] = method;
                    }
                }
            }
            /*MethodInfo? method = null;
            var baseType = obj.GetType();
            while (method == null)
            {
                method = baseType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
                baseType = baseType.BaseType;
                if (baseType == null) break;
            }*/
            if (method.ReturnType == typeof(void))
            {
                method.Invoke(obj, args);
                return default!;
            }
            return (T)method.Invoke(obj, args);
        }

        private static FieldInfo FindField(Type type, string fieldName)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            while (type != null)
            {
                var field = type.GetField(fieldName, flags);
                if (field != null) return field;
                type = type.BaseType;
            }
            return null;
        }
        
        private static MethodInfo FindMethod(Type type, string methodName)
        {
            const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
            while (type != null)
            {
                var method = type.GetMethod(methodName, flags);
                if (method != null) return method;
                type = type.BaseType;
            }
            return null;
        }
        
        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                _fieldCache.Clear();
                _methodCache.Clear();
            }
        }
    }
}