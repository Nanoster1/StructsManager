#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace StructsConsole
{
    public partial class CommandsManager
    {
        public static readonly CommandsManager Instance = new();

        public Dictionary<string, object> Variables { get; } = new();
        private ArithmeticManager _arithmeticManager = ArithmeticManager.Instance;
    }
}