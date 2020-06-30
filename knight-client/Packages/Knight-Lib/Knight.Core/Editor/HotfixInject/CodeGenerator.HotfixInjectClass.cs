using Knight.Core.Serializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;

namespace Knight.Core.Editor
{
    public class CodeGenerator_HotfixInjectClass
    {
        public static string    ClassRootPath = "Assets/Game.Hotfix.Template/Generate/";

        public StringBuilder    StringBuilder;
        public string           FilePath;

        public Type             ClassType;
        public string           NameSpace;
        public string           ClassName;

        public CodeGenerator_HotfixInjectClass(Type rClassType)
        {
            this.ClassType = rClassType;
            this.NameSpace = rClassType.Namespace;
            this.ClassName = rClassType.Name;

            this.FilePath = ClassRootPath + this.GetNameSpacePath(this.NameSpace) + this.ClassName + ".cs";
            this.StringBuilder = new StringBuilder();
        }

        public void WriteCode()
        {
            this.WriteHead();
            this.WriteClass();
            this.WriteEnd();
        }

        private void WriteHead()
        {
            this.StringBuilder?
                .A("using Knight.Core;").L(1)
                .A("using System.Collections;").L(1)
                .A("using System.Collections.Generic;").L(1)
                .A("using UnityEngine;").L(1)
                .L(1)
                .A($"namespace {this.NameSpace}.Hotfix").L(1)
                .A("{").L(1)
                .L(1);
        }

        private void WriteClass()
        {
            this.StringBuilder?
                .T(1).A("[HotfixBinding]").L(1)
                .T(1).A($"public class {this.ClassName}").L(1)
                .T(1).A("{").L(1);

            this.WriteConstructors();
            this.WriteMethods();

            this.StringBuilder?
                .T(1).A("}").L(1);
        }

        private void WriteConstructors()
        {
            var rBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var rAllConstructors = new List<ConstructorInfo>(this.ClassType.GetConstructors(rBindingFlags));
            rAllConstructors.Sort((a, b) => a.Name.CompareTo(b.Name));

            for (int i = 0; i < rAllConstructors.Count; i++)
            {
                var rConstructorInfo = rAllConstructors[i];

                var rIgnoreAttr = rConstructorInfo.CustomAttributes.SingleOrDefault(rAttr => rAttr.AttributeType.FullName.Equals("Knight.Core.HotfixIgnoreAttribute"));
                if (rIgnoreAttr != null) continue;

                if (i == 0)
                {
                    this.StringBuilder?.T(2).A($"[HotfixBinding(\"{this.NameSpace}\", \"{this.ClassName}\")]").L(1);
                }
                else
                {
                    this.StringBuilder?.T(2).A($"[HotfixBinding(\"{this.NameSpace}\", \"{this.ClassName}\", {i})]").L(1);
                }

                this.StringBuilder?.T(2).A($"public static void ctor(");

                // 不是静态方法，传入this对象
                var rParamArgs = rConstructorInfo.GetParameters();   // 添加参数
                if (!rConstructorInfo.IsStatic)
                {
                    if (rParamArgs.Length == 0)
                        this.StringBuilder?.A($"{this.NameSpace}.{this.ClassName} rThis");
                    else
                        this.StringBuilder?.A($"{this.NameSpace}.{this.ClassName} rThis, ");
                }
                for (int j = 0; j < rParamArgs.Length; j++)
                {
                    var rArgTypeName = SerializerAssists.GetTypeName(rParamArgs[j].ParameterType);
                    this.StringBuilder?.A(rArgTypeName).S(1).A(rParamArgs[j].Name);
                    if (j < rParamArgs.Length - 1) this.StringBuilder?.A(", ");
                }

                this.StringBuilder?
                    .A(")").L(1)
                    .T(2).A("{").L(1)
                    .T(2).A("}").L(2);
            }
        }

        private void WriteMethods()
        {
            var rBindingFlags = ReflectionAssist.flags_method | BindingFlags.Instance | BindingFlags.Static;
            var rAllMethods = new List<MethodInfo>(this.ClassType.GetMethods(rBindingFlags));
            rAllMethods.Sort((a, b) => a.Name.CompareTo(b.Name));

            var rMethodOverrideCountDict = new Dict<string, int>();
            for (int i = 0; i < rAllMethods.Count; i++)
            {
                var rMethodInfo = rAllMethods[i];
                if (!rMethodInfo.DeclaringType.Equals(this.ClassType)) continue;

                if (!rMethodOverrideCountDict.TryGetValue(rMethodInfo.Name, out int nRef))
                    rMethodOverrideCountDict.Add(rMethodInfo.Name, 0);
                else
                    rMethodOverrideCountDict[rMethodInfo.Name]++;

                var rIgnoreAttr = rMethodInfo.CustomAttributes.SingleOrDefault(rAttr => rAttr.AttributeType.FullName.Equals("Knight.Core.HotfixIgnoreAttribute"));
                if (rIgnoreAttr != null) continue;
                
                var nRefCount = rMethodOverrideCountDict[rMethodInfo.Name];
                if (nRefCount == 0)
                {
                    this.StringBuilder?.T(2).A($"[HotfixBinding(\"{this.NameSpace}\", \"{this.ClassName}\")]").L(1);
                }
                else
                {
                    this.StringBuilder?.T(2).A($"[HotfixBinding(\"{this.NameSpace}\", \"{this.ClassName}\", {nRefCount})]").L(1);
                }

                this.StringBuilder?.T(2).A($"public static void {rMethodInfo.Name}(");

                // 不是静态方法，传入this对象
                var rParamArgs = rMethodInfo.GetParameters();   // 添加参数
                if (!rMethodInfo.IsStatic)
                {
                    if (rParamArgs.Length == 0)
                        this.StringBuilder?.A($"{this.NameSpace}.{this.ClassName} rThis");
                    else
                        this.StringBuilder?.A($"{this.NameSpace}.{this.ClassName} rThis, ");
                }
                for (int j = 0; j < rParamArgs.Length; j++)
                {
                    var rArgTypeName = SerializerAssists.GetTypeName(rParamArgs[j].ParameterType);
                    this.StringBuilder?.A(rArgTypeName).S(1).A(rParamArgs[j].Name);
                    if (j < rParamArgs.Length - 1) this.StringBuilder?.A(", ");
                }

                this.StringBuilder?
                    .A(")").L(1)
                    .T(2).A("{").L(1)
                    .T(2).A("}").L(2);
            }
        }

        private void WriteEnd()
        {
            this.StringBuilder?
                .A("}")
                .L(1);
        }
        
        public void Save()
        {
            UtilTool.WriteAllText(this.FilePath, this.StringBuilder.ToString());
        }

        public string GetNameSpacePath(string rNamespace)
        {
            var rNamespaceSB = new StringBuilder();
            var rNamespaceArray = rNamespace.Split('.');
            for (int i = 0; i < rNamespaceArray.Length; i++)
            {
                rNamespaceSB.Append(rNamespaceArray[i]);
                rNamespaceSB.Append("_");
            }
            return rNamespaceSB.ToString();
        }
    }
}
