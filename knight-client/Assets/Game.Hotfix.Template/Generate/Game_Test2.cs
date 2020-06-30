using Knight.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Hotfix
{

	[HotfixBinding]
	public class Test2
	{
		[HotfixBinding("Game", "Test2")]
		public static void ctor(Game.Test2 rThis)
		{
		}

		[HotfixBinding("Game", "Test2")]
		public static void TestB(Game.Test2 rThis)
		{
		}

	}
}
