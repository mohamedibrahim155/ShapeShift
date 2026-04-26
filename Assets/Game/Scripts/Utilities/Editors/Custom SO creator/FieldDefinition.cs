using System;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

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

    public bool isCustomInterface;
    public Type customInterfaceType;

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
            isCustomInterface = this.isCustomInterface,
            customInterfaceType = this.customInterfaceType,
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
    Interface,
    GameObject,
    MonoBehaviour,
    Transform,
    Component,
    Sprite,
    Material,
    AudioClip,
}

public enum SOFieldTypeCategory
{
    Primitive,
    UnityObject,
    CustomClass,
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


