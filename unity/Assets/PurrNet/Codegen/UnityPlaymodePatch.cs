#if UNITY_MONO_CECIL && UNITY_PLAYMODE
using Mono.Cecil;
using Mono.Cecil.Cil;
using PurrNet.Editor;

namespace PurrNet.Codegen
{
    public static class UnityPlaymodePatch
    {
        public static void Patch(TypeDefinition topViewWindow)
        {
            var onGUIMethod = new MethodDefinition("OnGUI",
                MethodAttributes.Public | MethodAttributes.HideBySig,
                topViewWindow.Module.TypeSystem.Void);

            topViewWindow.Methods.Add(onGUIMethod);
            var processor = onGUIMethod.Body.GetILProcessor();

            var unityProxyType = topViewWindow.Module.GetTypeReference(typeof(PlayModePatch)).Import(topViewWindow.Module);
            var methodRef = unityProxyType.GetMethod("OnGUI").Import(topViewWindow.Module);

            processor.Emit(OpCodes.Ldarg_0);
            processor.Emit(OpCodes.Call, methodRef);
            processor.Emit(OpCodes.Ret);
        }
    }
}

#endif
