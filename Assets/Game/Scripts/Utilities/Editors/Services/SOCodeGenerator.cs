using System.Collections.Generic;
using UnityEngine;

public static class SOCodeGenerator
{

    public static string Generate(
        string className,
        string namespaceName,string menuName,
        FieldDefinition[] fields)
    {
        string usings = GenerateUsings(fields);
        string fieldCode = GenerateFields(fields);

        if (string.IsNullOrWhiteSpace(namespaceName))
        {
            return
$@"{usings}

[CreateAssetMenu(fileName = ""{className}"", menuName = ""{menuName}/{className}"")]
public class {className} : ScriptableObject
{{
{fieldCode}
}}";
        }

        return
$@"{usings}

namespace {namespaceName}
{{
    [CreateAssetMenu(fileName = ""{className}"", menuName = ""{menuName}/{className}"")]
    public class {className} : ScriptableObject
    {{
{fieldCode}
    }}
}}";
    }



    private static string GenerateFields(FieldDefinition[] fields)
    {
        var sb = new System.Text.StringBuilder();

        foreach (var field in fields)
        {
            sb.AppendLine($"        {GetFieldLine(field)}");
        }

        return sb.ToString();
    }
    public static string GetFieldLine(FieldDefinition field)
    {
        string type = ResolveType(field);
        string access = ResolveAccess(field);

        return $"{access} {type} {field.fieldName};";
    }

    private static string ResolveType(FieldDefinition field)
    {
        string baseType = field.fieldType switch
        {
            SOFieldType.String => "string",
            SOFieldType.Int => "int",
            SOFieldType.Float => "float",
            SOFieldType.Bool => "bool",
            SOFieldType.Vector2 => "Vector2",
            SOFieldType.Vector3 => "Vector3",
            SOFieldType.Color => "Color",
            SOFieldType.Sprite => "Sprite",
            SOFieldType.GameObject => "GameObject",
            SOFieldType.AudioClip => "AudioClip",
            SOFieldType.Material => "Material",
            SOFieldType.CustomClass => field.customClassType?.Name ?? "",
            SOFieldType.Enum => field.customEnum?.Name ?? "",
            _ => ""
        };

        return field.collectionType switch
        {
            SOFieldCollectionType.List => $"List<{baseType}>",
            SOFieldCollectionType.Array => $"{baseType}[]",
            SOFieldCollectionType.Stack => $"Stack<{baseType}>",
            SOFieldCollectionType.Queue => $"Queue<{baseType}>",
            _ => baseType
        };
    }

    private static string ResolveAccess(FieldDefinition field)
    {
        return field.fieldAccessModifier switch
        {
            FieldAccessModifier.Public => "public",
            FieldAccessModifier.Private => "private",
            FieldAccessModifier.Protected => "protected",
            FieldAccessModifier.Internal => "internal",
            _ => "public"
        };
    }

    private static string GenerateUsings(FieldDefinition[] fields)
    {
        HashSet<string> usings = new HashSet<string>
    {
        "using UnityEngine;"
    };

        foreach (var field in fields)
        {
            if (field.collectionType != SOFieldCollectionType.None)
                usings.Add("using System.Collections.Generic;");

            if (field.customClassType != null)
                usings.Add($"using {field.customClassType.Namespace};");

            if (field.customEnum != null)
                usings.Add($"using {field.customEnum.Namespace};");
        }

        return string.Join("\n", usings);
    }
}


