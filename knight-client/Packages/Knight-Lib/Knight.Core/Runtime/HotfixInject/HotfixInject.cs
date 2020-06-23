using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Knight.Core
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HotfixAttribute : Attribute
    {
    }

    public class HotfixInject
    {
        public static object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            return TypeResolveManager.Instance.InvokeStatic(rTypeName, rMethodName, rArgs);
        }
    }
}
