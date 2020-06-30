using Knight.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [Hotfix]
    public class Test1
    {
        public float P;

        public Test1()
        {
        }

        public Test1(float t1)
        {
            this.P = t1;
        }

        public static void TestA() 
        {
            Debug.LogError("I am TestA func");
        }

        public static void TestA(float p1)
        {
            Debug.LogError($"I am TestA [arg0:{p1}] func"); 
        }

        [HotfixIgnore]
        public static void TestIgnore()
        {
            Debug.LogError("I am TestIgnore func");
        }

        public void TestB(int p1, float p2)
        {
            Debug.LogError($"I am TestA [arg0:{p1}] [arg1:{p2}] func");
            this.P = p1 + p2;
            Debug.LogError("TestB result: " + this.P);
        }
    }
}
