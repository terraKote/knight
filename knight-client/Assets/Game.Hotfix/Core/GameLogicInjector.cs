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

namespace Game
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
            this.EnableMethodHotfixInject("Game", "Test1", "TestA");
            this.EnableMethodHotfixInject("Game", "Test1", "TestB");
        }

        public void EnableMethodHotfixInject(string rNamespace, string rClassName, string rMethodName)
        {
            var rClassType = TypeResolveManager.Instance.GetType($"{rNamespace}.{rClassName}");
            if (rClassType == null) return;

            var rFieldName = "__hotfix_" + rClassName + "_" + rMethodName + "_enable__";
            var rBindingFlags = BindingFlags.Public | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.Static;

            var rFieldInfo = rClassType.GetField(rFieldName, rBindingFlags);
            rFieldInfo?.SetValue(null, true);
        }
    }
}
