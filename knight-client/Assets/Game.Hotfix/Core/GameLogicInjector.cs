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

                Debug.LogError("xxxx000: " + rAllTypes[i].FullName);

                var rAllMethodInfos = rType.GetMethods(BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                for (int j = 0; j < rAllMethodInfos.Length; j++)
                {
                    Debug.LogError("xxxx100 " + rAllMethodInfos[j].Name);

                    var rMethodAttr = rAllMethodInfos[j].GetCustomAttributes(typeof(HotfixBindingAttribute), false);
                    if (rMethodAttr == null) continue;
                    //var rHotfixMethodAttr = (HotfixBindingAttribute)rMethodAttr[0];

                    Debug.LogError("xxxx111");

                    //this.EnableMethodHotfixInject(rHotfixMethodAttr.Namespace, rHotfixMethodAttr.ClassName, 
                    //                              rAllMethodInfos[j].Name, rHotfixMethodAttr.OverrideCount);
                }
            }
        }

        public void EnableMethodHotfixInject(string rNamespace, string rClassName, string rMethodName, int nOverrideCount = 0)
        {
            Debug.LogError(rNamespace + "." + rClassName);

            var rClassType = TypeResolveManager.Instance.GetType($"{rNamespace}.{rClassName}");
            if (rClassType == null) return;

            Debug.LogError(rNamespace + "." + rClassName);

            var rOverrideCountStr = nOverrideCount > 0 ? "_" + nOverrideCount : "";
            var rFieldName = "__hotfix_" + rClassName + "_" + rMethodName + rOverrideCountStr + "_enable__";
            var rBindingFlags = BindingFlags.Public | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.Static;

            var rFieldInfo = rClassType.GetField(rFieldName, rBindingFlags);
            rFieldInfo?.SetValue(null, true);
        }
    }
}
