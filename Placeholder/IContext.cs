using System;
using System.Collections.Generic;
using System.Linq;
namespace CorpseLib.Placeholder
{
    public interface IContext
    {
        public bool Call(string functionName, string[] args, out string ret);
        public string GetVariable(string name);
    }
}
