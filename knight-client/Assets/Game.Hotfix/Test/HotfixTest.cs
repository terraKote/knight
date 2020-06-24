using Knight.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Hotfix
{
    [HotfixBinding]
    public class Test1
    {
        [HotfixBinding("Game", "Test1")]
        public static void TestA()
        {
            Debug.LogError("HHHHHotfix Test1 TestA func...");
        }

        [HotfixBinding("Game", "Test1", 1)]
        public static void TestA(float p1)
        {
            Debug.LogError($"HHHHHotfix Test1 TestA [arg0:{p1}] func...");
        }

        [HotfixBinding("Game", "Test1")]
        public static void TestB(Game.Test1 rThis, int p1, float p2)
        {
            Debug.LogError($"HHHHHotfix Test1 TestB [arg0:{p1}] [arg1:{p2}] func");
            rThis.P = p1 - p2;
            Debug.LogError("TestB result: " + rThis.P);
        }
    }
}