using Knight.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Hotfix]
    public class Test1
    {
        public static bool __hotfix_Game_Test1_TestA_Enable__ = false;

        public static void TestA() 
        {
            if (__hotfix_Game_Test1_TestA_Enable__) 
            {
                HotfixInject.InvokeStatic("Game.Hotfix.Test1", "TestA"); 
                return;
            } 
            Debug.LogError("I am TestA func");  
        }

        public void TestB(int p1)
        {
            if (__hotfix_Game_Test1_TestA_Enable__)
            {
                HotfixInject.InvokeStatic("Game.Hotfix.Test1", "TestA", this, p1);
                return; 
            }
            Debug.LogError("I am TestA func");  
        }
    }
}
