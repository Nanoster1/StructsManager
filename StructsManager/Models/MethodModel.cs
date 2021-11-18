using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StructsManager.Models
{
    public class MethodModel
    {
        public MethodModel(MethodInfo method)
        {
            Method = method;
        }

        public string Name => Method.Name;
        public MethodInfo Method { get; }
        public string ModelInfo => 
            $"{Method.ReturnType.Name} {Method.Name}({string.Join(", ", Method.GetParameters().Select(param => $"{param.ParameterType.Name}"))})";
    }
}
