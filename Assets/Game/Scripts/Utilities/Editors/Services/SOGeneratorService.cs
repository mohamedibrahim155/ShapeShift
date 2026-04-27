using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Scripts.Editor.ScriptableObjectGenerator
{
    internal class SOGeneratorService
    {
        public static SOCreateResult Create(SORequestData request)
        {
            if (!SOValidator.ValidateAll(
              request.ClassName,
              request.NamespaceName,
              request.FieldDefinitions,
              out string error))
            {
                return new SOCreateResult
                {
                    Success = false,
                    Message = error
                };
            }


            if (!SOValidator.ValidateFolder(request.ScriptFolder, out string folderError) && !string.IsNullOrEmpty(folderError))
            {
                return SOCreateResult.Fail(folderError);
            }

            if (!SOValidator.ValidateFolder(request.AssetFolder, out string assetFolderError) && !string.IsNullOrEmpty(assetFolderError))
            {
                return SOCreateResult.Fail(assetFolderError);
            }


            string scriptPath = $"{request.ScriptFolder}/{request.ClassName}.cs";

            if (!SOValidator.ValidateFile(scriptPath, out string fileError))
            {
                return SOCreateResult.Fail(fileError);
            }

            string scriptContent = SOCodeGeneratorService.Generate(
                request.ClassName,
                request.NamespaceName,
                request.MenuName,
                request.FieldDefinitions
            );


            System.IO.File.WriteAllText(scriptPath, scriptContent);


            PendingSOData pendingData = new PendingSOData(
                request.ClassName, 
                request.NamespaceName, 
                request.AssetFolder);

            EditorPrefs.SetString(SOGeneratorSettings.PendingSODataKey,
                JsonUtility.ToJson(pendingData));

            AssetDatabase.Refresh();

            return SOCreateResult.Ok($"Created script: {scriptPath}");

        }
    }
}
