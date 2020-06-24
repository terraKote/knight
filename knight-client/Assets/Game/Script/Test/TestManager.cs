using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game
{
    public class TestManager
    {
        public static void Test()
        {
            Test1.TestA();
            Test1.TestA(300.0f);

            var rTest1 = new Test1();
            rTest1.TestB(100, 205.0232f);
        }
    }
}
