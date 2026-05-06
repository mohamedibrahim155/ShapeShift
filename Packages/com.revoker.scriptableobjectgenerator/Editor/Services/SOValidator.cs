using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor.ScriptableObjectGenerator
{
    public static class SOValidator
    {
        public static bool ValidateAll(
           string className,
           string namespaceName,
           FieldDefinition[] fields,
           out string error)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                error = "Class name is empty";
                return false;
            }

            if (!IsValidIdentifier(className))
            {
                error = "Invalid class name";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(namespaceName) && !IsValidNamespace(namespaceName))
            {
                error = "Invalid namespace";
                return false;
            }

            if (fields == null || fields.Length == 0)
            {
                error = "No fields defined";
                return false;
            }

            HashSet<string> names = new HashSet<string>();

            foreach (var field in fields)
            {
                if (!ValidateField(field, out error))
                    return false;

                if (!names.Add(field.fieldName))
                {
                    error = $"Duplicate field: {field.fieldName}";
                    return false;
                }
            }

            error = null;
            return true;
        }

        public static void CreateFolderIfNeeded(string folderPath, out string error)
        {
            if (ValidateFolder(folderPath, out error))
            {
                return;
            }


            string[] folders = folderPath.Split('/');
            string currentPath = folders[0];

            for (int i = 1; i < folders.Length; i++)
            {
                string nextPath = $"{currentPath}/{folders[i]}";

                if (!ValidateFolder(nextPath, out error))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }

                currentPath = nextPath;
            }

            error = null;
        }

        public static bool ValidateFolder(string folderPath, out string error)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
            {
                error = "Folder path is empty";
                return false;
            }
            if (!UnityEditor.AssetDatabase.IsValidFolder(folderPath))
            {
                error = $"Invalid folder: {folderPath}";
                return false;
            }
            error = null;
            return true;
        }

        public static bool ValidateFile(string filePath, out string error)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                error = "file path is empty";
                return false;
            }
            if (System.IO.File.Exists(filePath))
            {
                error = $"A script with this class name already exists:\n{filePath}";
                return false;
            }
            error = null;
            return true;
        }


        public static bool ValidateField(FieldDefinition field, out string error)
        {
            if (string.IsNullOrWhiteSpace(field.fieldName))
            {
                error = "Field name is empty";
                return false;
            }

            if (!IsValidIdentifier(field.fieldName))
            {
                error = $"Invalid field name: {field.fieldName}";
                return false;
            }

            if (field.fieldType == SOFieldType.CustomClass && field.customClassType == null)
            {
                error = $"Missing custom type for {field.fieldName}";
                return false;
            }

            if (field.fieldType == SOFieldType.Enum && field.customEnum == null)
            {
                error = $"Missing enum type for {field.fieldName}";
                return false;
            }

            error = null;
            return true;
        }

        private static bool IsValidIdentifier(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            if (!(char.IsLetter(name[0]) || name[0] == '_'))
                return false;

            for (int i = 1; i < name.Length; i++)
            {
                if (!(char.IsLetterOrDigit(name[i]) || name[i] == '_'))
                    return false;
            }

            return true;
        }

        private static bool IsValidNamespace(string namespaceName)
        {
            if (string.IsNullOrWhiteSpace(namespaceName))
                return true; // namespace is optional

            string[] parts = namespaceName.Split('.');

            foreach (string part in parts)
            {
                if (!IsValidIdentifier(part))
                    return false;
            }

            return true;
        }
    }
}
