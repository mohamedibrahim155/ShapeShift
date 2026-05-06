using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Editor.ScriptableObjectGenerator
{
    [System.Serializable]
    public class SORequestData
    {
        public string NamespaceName;
        public string ScriptFolder;
        public string AssetFolder;
        public string ClassName;
        public string MenuName;
        public FieldDefinition[] FieldDefinitions;

        public SORequestData(string className,
            string namespaceName ,
            string menuName, 
            string scriptFolder,
            string assetFolder, 
            FieldDefinition[] fieldDefinitions)
        {
            ClassName = className;
            NamespaceName = namespaceName;
            MenuName = menuName;
            ScriptFolder = scriptFolder;
            AssetFolder = assetFolder;
            FieldDefinitions = fieldDefinitions;
        }
    }

    public struct PendingSOData
    {
        public string ClassName;
        public string NamespaceName;
        public string AssetFolder;

        public PendingSOData(string className, string namespaceName, string assetFolder)
        {
            ClassName = className;
            NamespaceName = namespaceName;
            AssetFolder = assetFolder;
        }
    }

    public struct SOCreateResult
    {
        public bool Success;
        public string Message;

        public static SOCreateResult Fail(string message)
        {
            return new SOCreateResult
            {
                 Success = false,
                 Message = message
            };
        }

        public static SOCreateResult Ok(string message)
        {
            return new SOCreateResult
            {
                Success = true,
                Message = message
            };
        }
    }
}
