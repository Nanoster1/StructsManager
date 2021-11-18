using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructsManager.Models
{
    public class Variable: ReactiveObject
    {
        public string Name { get; init; }
        public object Data { get; init; }
    }
}
