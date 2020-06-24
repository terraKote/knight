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

    [AttributeUsage(AttributeTargets.Method)]
    public class HotfixIgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HotfixBindingAttribute : Attribute
    {
        public string   Namespace;
        public string   ClassName;

        public int      OverrideCount = 0;

        public HotfixBindingAttribute(string rNamespace = "", string rClassName = "", int nOverrideCount = 0)
        {
            this.Namespace = rNamespace;
            this.ClassName = rClassName;
            this.OverrideCount = nOverrideCount;
        }
    }
    
    public class HotfixInject
    {
        public static object InvokeStatic(string rTypeName, string rMethodName, params object[] rArgs)
        {
            return TypeResolveManager.Instance.InvokeStatic(rTypeName, rMethodName, rArgs);
        }
    }
}
