using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StructsManager.Models
{
    public class TypeModel
    {
        public TypeModel(Type type)
        {
            Type = type;
        }

        public string Name => Type.Name;
        public Type Type { get; }
        public List<MethodModel> Methods => Type.GetMethods().Select(method => new MethodModel(method)).ToList();
    }
}
