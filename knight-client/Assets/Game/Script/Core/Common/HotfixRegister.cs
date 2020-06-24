using Knight.Framework.Hotfix;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Game
{
    public static class HotfixRegister
    {
        public static void Register()
        {
            HotfixApp_ILRT.OnHotfixRegisterFunc = (rApp) => 
            {
                rApp.DelegateManager.RegisterFunctionDelegate<System.Reflection.MethodInfo, System.Reflection.MethodInfo, System.Int32>();
                rApp.DelegateManager.RegisterDelegateConvertor<System.Comparison<System.Reflection.MethodInfo>>((act) =>
                {
                    return new System.Comparison<System.Reflection.MethodInfo>((x, y) =>
                    {
                        return ((Func<System.Reflection.MethodInfo, System.Reflection.MethodInfo, System.Int32>)act)(x, y);
                    });
                });
            };
        }
    }
}
