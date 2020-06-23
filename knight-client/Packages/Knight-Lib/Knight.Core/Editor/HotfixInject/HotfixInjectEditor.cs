using Mono.Cecil;
using Mono.Cecil.Cil;
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
                InjectMethod(rAssembly, rNeedInjectType, rAllMethodList[i]);
            }
        }

        private static void InjectMethod(AssemblyDefinition rAssembly, TypeDefinition rNeedInjectType, MethodDefinition rMethodDef)
        {
            var rMethodName = rMethodDef.Name.Trim('.');
            // 略过静态构造函数 
            if (rMethodName.Equals("cctor")) return;

            var rFiledName = "__hotfix_" + rNeedInjectType.Name + "_" + rMethodName + "_enable__";
            
            // 添加Field标记变量
            if (ContainField(rNeedInjectType, rFiledName)) return;
            var rFieldDefinition = new FieldDefinition(rFiledName, FieldAttributes.Static | FieldAttributes.Public, GetBoolValueTypeReference(rAssembly));
            rNeedInjectType.Fields.Add(rFieldDefinition);
            Debug.LogError(rFiledName);
            
            var rFirstIns = rMethodDef.Body.Instructions.First();
            var rWorker = rMethodDef.Body.GetILProcessor();

            // bool _hotfix_Game_Test1_TestA_Enable__ = __hotfix_Game_Test1_TestA_Enable__;
            var rCurrentIns = InsertBefore(rWorker, rFirstIns, rWorker.Create(OpCodes.Nop));
            rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Ldsfld, rFieldDefinition));

            // if (!_hotfix_Game_Test1_TestA_Enable__) False跳转到正常语句
            rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Brfalse_S, rFirstIns));

            // 插入Invoke方法
            var rInvokeMethodName = "InvokeStatic";
            var rHotfixInjectMethod = typeof(HotfixInject).GetMethod(rInvokeMethodName);
            var rHotfixInjectMethodRef = rAssembly.MainModule.ImportReference(rHotfixInjectMethod);
            Debug.LogError(rNeedInjectType.Namespace + "." + rNeedInjectType.Name);
            rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Ldstr, rNeedInjectType.Namespace + ".Hotfix." + rNeedInjectType.Name));
            rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Ldstr, rMethodName));

            // 处理传入参数
            var nParamsCount = rMethodDef.Parameters.Count;
            rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Ldc_I4, rMethodDef.IsStatic ? nParamsCount : nParamsCount + 1));
            rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Newarr, rAssembly.MainModule.ImportReference(typeof(object))));

            // 如果不是静态方法
            if (!rMethodDef.IsStatic)
            {
                rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Dup));
                rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Ldc_I4, 0));
                rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Ldarg_0));
                rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Stelem_Ref));
            }

            for (int nIndex = 0; nIndex < nParamsCount; nIndex++)
            {
                var nArgIndex = rMethodDef.IsStatic ? nIndex : nIndex + 1;

                rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Dup));
                rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Ldc_I4, nArgIndex));
                var rParamType = rMethodDef.Parameters[nIndex].ParameterType;

                var rParamTypeDef = rAssembly.MainModule.GetType(rParamType.FullName);
                rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Ldarg, nArgIndex));
                rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Box, rParamType));
                
                rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Stelem_Ref));
            }
            rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Call, rHotfixInjectMethodRef));

            // 处理返回值
            var rMethodReturnVoid = rMethodDef.ReturnType.FullName.Equals("System.Void");
            var rHotfixInjectMethodReturnVoid = rHotfixInjectMethod.ReturnType.FullName.Equals("System.Void");
            if (!rHotfixInjectMethodReturnVoid)
            {
                if (rMethodReturnVoid)
                    rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Pop));
                else
                    rCurrentIns = InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Unbox_Any, rMethodDef.ReturnType));
            }

            // 添加 return 指令
            InsertAfter(rWorker, rCurrentIns, rWorker.Create(OpCodes.Ret));

            // 重新计算方法的偏移
            ComputeOffsets(rMethodDef.Body); 
        }

        private static bool ContainField(TypeDefinition rType, string rFieldName)
        {
            var rFindField = rType.Fields.SingleOrDefault(rFiled => rFiled.Name.Equals(rFieldName));
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

        private static Instruction InsertBefore(ILProcessor rWorker, Instruction rTarget, Instruction rInstruction)
        {
            rWorker.InsertBefore(rTarget, rInstruction);
            return rInstruction;
        }

        private static Instruction InsertAfter(ILProcessor rWorker, Instruction rTarget, Instruction rInstruction)
        {
            rWorker.InsertAfter(rTarget, rInstruction);
            return rInstruction;
        }

        private static OpCode GetLDArg(int nArgIndex)
        {
            if (nArgIndex == 0) return OpCodes.Ldarg_0;
            else if (nArgIndex == 1) return OpCodes.Ldarg_1;
            else if (nArgIndex == 2) return OpCodes.Ldarg_2;
            else if (nArgIndex == 3) return OpCodes.Ldarg_3;
            return OpCodes.Ldarg;
        }

        private static void ComputeOffsets(MethodBody rBody)
        {
            var nOffset = 0;
            foreach (var rInstruction in rBody.Instructions)
            {
                rInstruction.Offset = nOffset;
                nOffset += rInstruction.GetSize();
            }
        }
    }
}
