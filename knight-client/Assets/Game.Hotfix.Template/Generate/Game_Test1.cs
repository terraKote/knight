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
		public static void ctor(Game.Test1 rThis)
		{
		}

		[HotfixBinding("Game", "Test1", 1)]
		public static void ctor(Game.Test1 rThis, float t1)
		{
		}

		[HotfixBinding("Game", "Test1")]
		public static void TestA()
		{
		}

		[HotfixBinding("Game", "Test1", 1)]
		public static void TestA(float p1)
		{
		}

		[HotfixBinding("Game", "Test1")]
		public static void TestB(Game.Test1 rThis, int p1, float p2)
		{
		}

	}
}
