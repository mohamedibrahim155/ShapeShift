using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Scripts.Editor.ScriptableObjectGenerator
{
    [System.Serializable]
    public class FieldDefinition
    {
        public bool isExpanded = true;
        public string fieldName = "";
        public FieldAccessModifier fieldAccessModifier = FieldAccessModifier.Public;
        public SOFieldType fieldType = SOFieldType.String;
        public SOFieldCollectionType collectionType = SOFieldCollectionType.None;

        public bool isCustomClass;
        public Type customClassType;

        public bool isCustomEnum;
        public Type customEnum;

        public FieldDefinition Clone()
        {

            return new FieldDefinition
            {
                isExpanded = this.isExpanded,
                fieldName = this.fieldName,
                fieldType = this.fieldType,
                collectionType = this.collectionType,
                isCustomClass = this.isCustomClass,
                customClassType = this.customClassType,
                isCustomEnum = this.isCustomEnum,
                customEnum = this.customEnum,
            };

        }
    }

    public enum FieldValidationStatus
    {
        Valid,
        Error
    }

    public struct FieldValidationResult
    {
        public FieldValidationStatus Status;
        public string Message;

        public bool IsValid => Status == FieldValidationStatus.Valid;
    }

    public enum SOFieldType
    {
        String,
        Int,
        Float,
        Bool,
        Vector2,
        Vector3,
        Color,
        Enum,
        CustomClass,
        GameObject,
        MonoBehaviour,
        Transform,
        Component,
        Sprite,
        Material,
        AudioClip,
    }


    public enum SOFieldCollectionType
    {
        None,
        List,
        Array,
        Queue,
        Stack,
    }

    public enum FieldAccessModifier
    {
        Public,
        Private,
        Protected,
        Internal
    }
}


