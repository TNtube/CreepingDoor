using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Ionic.Zip;

public class Patch_cudnn64_7_Runner : AssetPostprocessor
{
    struct Patchs
    {
        public bool isValid;
        public string patch_src;
        public string patch_dest;
    }

    static void OnPostprocessAllAssets(string[] _, string[] __, string[] ___, string[] ____, bool _____)
    {
        // Only check once per Session + Get Patch paths
        if (SessionState.GetBool(nameof(Patch_cudnn64_7_Runner), false)) return;
        Patchs patchs = GetPatchPaths();
        if (!patchs.isValid) return;

        // Get & Check if destination does not already exists
        string destination = Path.Combine(patchs.patch_dest, "cudnn64_7.dll");
        if (File.Exists(destination)) {
            SessionState.SetBool(nameof(Patch_cudnn64_7_Runner), true);
            return;
        }

        // Get src archive parts
        string[] archives = new string[] {
            Path.Combine(patchs.patch_src, "cudnn64_7.zip"),
            Path.Combine(patchs.patch_src, "cudnn64_7.z01"),
            Path.Combine(patchs.patch_src, "cudnn64_7.z02")
        };

        // Check all src archive parts are valids
        foreach (string archive in archives) { 
            if (!File.Exists(archive)) {
                Debug.LogError($"[{nameof(Patch_cudnn64_7_Runner)}] An archive part is missing ! Please fix it\n" + archive + "\n");
                return;
            }
        }
        
        // Extract dll + meta from archive grouped (with overwrite)
        using (ZipFile zip = ZipFile.Read(archives[0])) {
            foreach (ZipEntry entry in zip.Entries) { 
                if (entry.FileName.Contains("cudnn64_7.dll")) {
                    entry.Extract(patchs.patch_dest, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

        // Notify Patch was successfully executed
        Debug.Log($"[{nameof(Patch_cudnn64_7_Runner)}] Patched successfully !");
    }

    static Patchs GetPatchPaths()
    {
        string[] guids = AssetDatabase.FindAssets("t:" + typeof(Patch_cudnn64_7));
        if (guids.Length > 2) {
            Debug.LogError($"[{nameof(Patch_cudnn64_7_Runner)}] Too many '{nameof(Patch_cudnn64_7)}' founded ! Please fix it");
            return new Patchs { isValid = false };
        }

        string[] paths = guids.Select(AssetDatabase.GUIDToAssetPath).ToArray();
        Patch_cudnn64_7[] patchs = paths.Select(AssetDatabase.LoadAssetAtPath<Patch_cudnn64_7>).ToArray();
        Patch_cudnn64_7 patch_src = null, patch_dest = null;
        for (int i = 0; i < patchs.Length; ++i) {
            if (patchs[i].data == Patch_cudnn64_7.Source) patch_src = patchs[i];
            else if (patchs[i].data == Patch_cudnn64_7.Destination) patch_dest = patchs[i];
        }

        string srcpath;
        if (patch_src == null) {
            srcpath = Path.Combine("Assets", nameof(Patch_cudnn64_7) + "_src.asset");
            patch_src = ScriptableObject.CreateInstance<Patch_cudnn64_7>();
            patch_src.data = Patch_cudnn64_7.Source;
            AssetDatabase.CreateAsset(patch_src, srcpath);
            EditorUtility.SetDirty(patch_src);
            AssetDatabase.SaveAssetIfDirty(patch_src);
        } else srcpath = AssetDatabase.GetAssetPath(patch_src);

        string destPath;
        if (patch_dest == null) {
            destPath = Path.Combine("Assets", nameof(Patch_cudnn64_7) + "_dest.asset");
            patch_dest = ScriptableObject.CreateInstance<Patch_cudnn64_7>();
            patch_dest.data = Patch_cudnn64_7.Destination;
            AssetDatabase.CreateAsset(patch_dest, destPath);
            EditorUtility.SetDirty(patch_dest);
            AssetDatabase.SaveAssetIfDirty(patch_dest);
        } else destPath = AssetDatabase.GetAssetPath(patch_dest);

        string appPath = Directory.GetParent(Application.dataPath).FullName;
        return new Patchs { 
            isValid = true,
            patch_src = Path.GetDirectoryName(Path.Combine(appPath, srcpath)),
            patch_dest = Path.GetDirectoryName(Path.Combine(appPath, destPath))
        };
    }
}