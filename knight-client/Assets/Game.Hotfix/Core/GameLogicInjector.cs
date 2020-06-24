//======================================================================
//        Copyright (C) 2015-2020 Winddy He. All rights reserved
//        Email: hgplan@126.com
//======================================================================
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;
using Knight.Framework.Hotfix;
using UnityEngine.UI;
using Knight.Core;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Knight.Hotfix.Core
{
    public class GameLogicInjector
    {
#pragma warning disable 1998
        public async Task Initialize()
        {
            this.InitializeInjector();
        }
#pragma warning restore 1998
        
        private void InitializeInjector()
        {
            var rAllTypes = TypeResolveManager.Instance.GetTypes("Game.Hotfix");
            for (int i = 0; i < rAllTypes.Length; i++)
            {
                var rType = rAllTypes[i];
                var rHotfixBindingAttrs = rType.GetCustomAttributes(typeof(HotfixBindingAttribute), false);
                if (rHotfixBindingAttrs == null || rHotfixBindingAttrs.Length == 0) continue;

                var rAllMethodInfos = new List<MethodInfo>(rType.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static));
                rAllMethodInfos.Sort((a, b) => a.Name.CompareTo(b.Name));
                for (int j = 0; j < rAllMethodInfos.Count; j++)
                {
                    var rMethodAttrs = rAllMethodInfos[j].GetCustomAttributes(false);
                    if (rMethodAttrs == null || rMethodAttrs.Length == 0) continue;
                    var rHotfixMethodAttr = (HotfixBindingAttribute)rMethodAttrs[0];
                    
                    this.EnableMethodHotfixInject(rHotfixMethodAttr.Namespace, rHotfixMethodAttr.ClassName,
                                                  rAllMethodInfos[j].Name, rHotfixMethodAttr.OverrideCount);
                }
            }
        }

        public void EnableMethodHotfixInject(string rNamespace, string rClassName, string rMethodName, int nOverrideCount = 0)
        {
            var rClassType = TypeResolveManager.Instance.GetType($"{rNamespace}.{rClassName}");
            if (rClassType == null) return;

            var rOverrideCountStr = nOverrideCount > 0 ? "_" + nOverrideCount : "";
            var rFieldName = "__hotfix_" + rClassName + "_" + rMethodName + rOverrideCountStr + "_enable__";
            var rBindingFlags = BindingFlags.Public | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.Static;

            var rFieldInfo = rClassType.GetField(rFieldName, rBindingFlags);
            if (rFieldInfo != null)
            {
                rFieldInfo.SetValue(null, true);
                Debug.Log($"Injected: [ {rFieldName} ] ");
            }
        }
    }
}
