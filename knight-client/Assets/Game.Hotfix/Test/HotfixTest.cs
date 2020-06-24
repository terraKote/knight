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
        public static void TestA(int p1)
        {
            Debug.LogError("HHHHHotfix Test1 TestA [arg0:p1] func...");
        }
    }
}