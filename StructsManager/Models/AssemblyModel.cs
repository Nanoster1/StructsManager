using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StructsManager.Models
{
    public class AssemblyModel
    {
        public AssemblyModel(Assembly assembly, string? name = null)
        {
            Assembly = assembly;
        }
        public string Name => Assembly.GetName().Name ?? nameof(Assembly);
        public Assembly Assembly { get; }
        public List<TypeModel> Types => Assembly.GetTypes().Select(type => new TypeModel(type)).ToList();
    }
}
