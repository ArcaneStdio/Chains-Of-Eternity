using System;
using UnityEditor;
using UnityEditor.Build;

namespace PurrNet.Editor
{
    public static class EnableLeakDetector
    {
#if PURR_LEAKS_CHECK
        [MenuItem("Tools/PurrNet/Debug/Disable Leak Detection", priority = 200)]
        public static void Uninstall()
        {
            RemoveSymbol("PURR_LEAKS_CHECK");
        }
#else
        [MenuItem("Tools/PurrNet/Debug/Enable Leak Detection", priority = 200)]
        public static void Install()
        {
            AddSymbol("PURR_LEAKS_CHECK");
        }
#endif

#if PURRNET_DEBUG_POOLING
        [MenuItem("Tools/PurrNet/Debug/Disable Pool Debug", priority = 200)]
        public static void UninstallPool()
        {
            RemoveSymbol("PURRNET_DEBUG_POOLING");
        }
#else
        [MenuItem("Tools/PurrNet/Debug/Enable Pool Debug", priority = 200)]
        public static void InstallPool()
        {
            AddSymbol("PURRNET_DEBUG_POOLING");
        }
#endif

        public static void RemoveSymbol(string symbol)
        {
            var activeBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedTarget = NamedBuildTarget.FromBuildTargetGroup(activeBuildTargetGroup);

            var content = PlayerSettings.GetScriptingDefineSymbols(namedTarget);
            int idxOf = content.IndexOf(symbol, StringComparison.Ordinal);
            bool isNextSemicolon = idxOf < content.Length - 1 && content[idxOf + 1] == ';';
            if (isNextSemicolon)
                idxOf++;
            content = content.Remove(idxOf, symbol.Length);
            PlayerSettings.SetScriptingDefineSymbols(namedTarget, content);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void AddSymbol(string symbol)
        {
            var activeBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            var namedTarget = NamedBuildTarget.FromBuildTargetGroup(activeBuildTargetGroup);

            var content = PlayerSettings.GetScriptingDefineSymbols(namedTarget);
            bool needsSemicolon = content.Length > 0 && content[^1] != ';';
            content += needsSemicolon ? ";" : "";
            content += symbol;
            PlayerSettings.SetScriptingDefineSymbols(namedTarget, content);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    public static class InstallUniTask
    {
#if UNITASK_PURRNET_SUPPORT
        [MenuItem("Tools/PurrNet/Packages/Uninstall UniTask", priority = 100)]
        public static void Uninstall()
        {
            if (EditorUtility.DisplayDialog("Uninstall UniTask", "This will remove UniTask from the package manager. Do you want to continue?", "Yes", "No"))
            {
                UnityEditor.PackageManager.Client.Remove("com.cysharp.unitask");
            }
        }
#else
        [MenuItem("Tools/PurrNet/Packages/Install UniTask", priority = 100)]
        public static void Install()
        {
            if (EditorUtility.DisplayDialog("Install UniTask", "This will install UniTask from the package manager. Do you want to continue?", "Yes", "No"))
            {
                UnityEditor.PackageManager.Client.Add("https://github.com/Cysharp/UniTask.git?path=src/UniTask/Assets/Plugins/UniTask");
            }
        }
#endif

#if PURR_BUTTONS
        [MenuItem("Tools/PurrNet/Packages/Disable PurrButtons", priority = 100)]
        public static void UninstallPurrButtons()
        {
            EnableLeakDetector.RemoveSymbol("PURR_BUTTONS");
        }
#else
        [MenuItem("Tools/PurrNet/Packages/Enable PurrButtons", priority = 100)]
        public static void InstallPurrButtons()
        {
            EnableLeakDetector.AddSymbol("PURR_BUTTONS");
        }
#endif

#if PURR_CONTEXT_BUTTONS
        [MenuItem("Tools/PurrNet/Packages/Disable PurrContextButtons", priority = 100)]
        public static void UninstallPURR_CONTEXT_BUTTONS()
        {
            EnableLeakDetector.RemoveSymbol("PURR_CONTEXT_BUTTONS");
        }
#else
        [MenuItem("Tools/PurrNet/Packages/Enable PurrContextButtons", priority = 100)]
        public static void InstallPURR_CONTEXT_BUTTONS()
        {
            EnableLeakDetector.AddSymbol("PURR_CONTEXT_BUTTONS");
        }
#endif

#if PURR_ENDIAN
        [MenuItem("Tools/PurrNet/Packages/Disable Endianness Check", priority = 100)]
        public static void UninstallEndianness()
        {
            EnableLeakDetector.RemoveSymbol("PURR_ENDIAN");
        }
#else
        [MenuItem("Tools/PurrNet/Packages/Enable Endianness Check", priority = 100)]
        public static void InstallEndianness()
        {
            EnableLeakDetector.AddSymbol("PURR_ENDIAN");
        }
#endif
    }
}
