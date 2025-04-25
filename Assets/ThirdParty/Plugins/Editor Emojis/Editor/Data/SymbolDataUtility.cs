using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Anvil.EditorEmojis
{
    internal static class SymbolDataUtility
    {
        public static SymbolsDataJson LoadSymbolData(string jsonPath)
        {
            if (!File.Exists(jsonPath))
            {
                Debug.LogError($"Icon JSON file not found at path: {jsonPath}");
                return null;
            }

            string jsonText = File.ReadAllText(jsonPath);
            
            var symbolsDataJson = new SymbolsDataJson();
            EditorJsonUtility.FromJsonOverwrite(jsonText, symbolsDataJson);
            return symbolsDataJson;
        }


        public static string GetFilePath(string fileName)
        {
            // find the filename in the project and return it's filepath relative to the project
            string[] allFiles = Directory.GetFiles(Application.dataPath, fileName, SearchOption.AllDirectories);
            if (allFiles.Length > 0)
            {
                string fullPath = allFiles[0];
                int assetsIndex = fullPath.IndexOf("Assets", StringComparison.Ordinal);
                if (assetsIndex >= 0)
                {
                    return fullPath.Substring(assetsIndex);
                }
                return fullPath;
            }
            else
            {
                Debug.LogError($"File not found: {fileName}");
                throw new FileNotFoundException($"File not found: {fileName}");
            }
        }

        public static string GetDirPath(string dirName)
        {
            string[] allDirs = Directory.GetDirectories(Application.dataPath, dirName, SearchOption.AllDirectories);
            if (allDirs.Length > 0)
            {
                string fullPath = allDirs[0];
                int assetsIndex = fullPath.IndexOf("Assets", StringComparison.OrdinalIgnoreCase);
                return assetsIndex >= 0 ? fullPath.Substring(assetsIndex) : fullPath;
            }
            else
            {
                Debug.LogError($"Directory not found: {dirName}");
                throw new DirectoryNotFoundException($"Directory not found: {dirName}");
            }
        }

    }
}