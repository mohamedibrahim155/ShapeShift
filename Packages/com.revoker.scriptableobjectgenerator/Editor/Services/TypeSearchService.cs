using Scripts.Editor.ScriptableObjectGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;


namespace Scripts.Editor.ScriptableObjectGenerator
{
    public enum TypeFilter
    {
        All,
        UnityTypes,
        Class,
        SerializableTypes,
        Enum,
    }
    public static class TypeSearchService
    {
        private static List<Type> cachedTypes = null;
        
        public static List<Type> GetTypes(string search, TypeFilter filter)
        {
            if (cachedTypes == null)
                cachedTypes = BuildCachedType();

            search = search.ToLower();
            return cachedTypes
                .Where(t => MatchesSearch(t, search))
                .Where(t => MatchFilter(t, filter))
                .ToList();
        }

        private static List<Type> BuildCachedType() 
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly =>
                {
                    try { return assembly.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(IsValidType)
                .OrderBy(t => t.Name)
                .ToList();
        }

        public static bool IsValidType(Type type)
        {
            return (type.IsClass || type.IsEnum)
                    && !type.IsAbstract
                    && !type.IsGenericType
                    && type.IsPublic
                    && !type.Name.StartsWith("<")
                    && !type.Name.Contains("AnonymousType")
                    && !Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute))
                    && (typeof(UnityEngine.Object).IsAssignableFrom(type) || type.IsSerializable);
        }

        private static bool MatchesSearch(Type type, string search)
        {
            return string.IsNullOrEmpty(search) ||
                   type.Name.ToLower().Contains(search);
        }

        private static bool MatchFilter(Type type, TypeFilter filter)
        {
            return filter switch
            {
                TypeFilter.UnityTypes =>
                    typeof(UnityEngine.Object).IsAssignableFrom(type),

                TypeFilter.SerializableTypes =>
                    type.IsSerializable && !type.IsEnum,

                TypeFilter.Class =>
                    type.IsClass,

                TypeFilter.Enum =>
                    type.IsEnum,

                _ => true
            };

        }
    }
}
