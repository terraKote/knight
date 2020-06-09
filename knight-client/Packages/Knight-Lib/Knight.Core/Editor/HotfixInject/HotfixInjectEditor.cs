using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Knight.Core.Editor
{
    public class HotfixInjectEditor
    {
        private static string mDLLPath = "Library/ScriptAssemblies/Game.dll";

        [MenuItem("Tools/Other/Hotfix Injector")]
        public static void Inject()
        {
            AssemblyDefinition rAssembly = null;
            try
            {
                // 取Assetmbly
                var readerParameters = new ReaderParameters { ReadSymbols = true, ReadWrite = true };
                rAssembly = AssemblyDefinition.ReadAssembly(mDLLPath, readerParameters);

                var rHotfixTypes = rAssembly.MainModule.Types.Where(
                                        rType => rType != null &&
                                        rType.BaseType != null &&
                                        rType.CustomAttributes.Any(rAttr => rAttr.AttributeType.FullName.Equals("Knight.Core.HotfixAttribute")));

                var rHotfixTypeList = new List<TypeDefinition>(rHotfixTypes);
                foreach (var rHotfixType in rHotfixTypeList)
                {
                    Debug.LogError("~~~~~~~~~~~~~~~~~~~" + rHotfixType.FullName);
                    InjectType(rAssembly, rHotfixType);
                }

                var rWriteParameters = new WriterParameters { WriteSymbols = true };
                rAssembly.Write(rWriteParameters);
                rAssembly.Dispose();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            Debug.Log("ViewModel inject success!!!");
        }

        private static void InjectType(AssemblyDefinition rAssembly, TypeDefinition rNeedInjectType)
        {
            if (rNeedInjectType == null) return;

            var rAllMethodList = new List<MethodDefinition>(rNeedInjectType.Methods);
            for (int i = 0; i < rAllMethodList.Count; i++)
            {
                var rFiledName = "__hotfix_" + rNeedInjectType.Name + "_" + rAllMethodList[i].Name.Trim('.') + "_enable__";

                if (!ContainField(rNeedInjectType, rFiledName))
                {
                    var rFieldDefinition = new FieldDefinition(rFiledName, FieldAttributes.Static | FieldAttributes.Public, GetBoolValueTypeReference(rAssembly));
                    rNeedInjectType.Fields.Add(rFieldDefinition);
                    Debug.LogError(rFiledName);
                }
            }
        }

        private static bool ContainField(TypeDefinition rType, string rFieldName)
        {
            var rFindField = rType.Fields.Single(rFiled => rFiled.Name.Equals(rFieldName));
            return rFindField != null;
        }

        private static TypeReference GetBoolValueTypeReference(AssemblyDefinition rAssembly)
        {
            var rHotfixTemplateType = rAssembly.MainModule.Types.SingleOrDefault(
                                        rType => rType != null &&
                                        rType.BaseType != null &&
                                        rType.FullName.Equals("Game.HotfixTemplate"));

            if (rHotfixTemplateType != null)
            {
                var rBoolValueField = rHotfixTemplateType.Fields.SingleOrDefault(
                                        rField => rField.Name.Equals("__Bool_Value_Template__"));
                return rBoolValueField.FieldType;
            }
            return null;
        }
    }
}
