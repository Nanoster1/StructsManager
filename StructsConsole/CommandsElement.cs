using System.Collections.Generic;

namespace StructsConsole
{
    public class CommandsElement
    {
        public List<string> Operations { get; init; }
        public Queue<object> Arguments { get; init; }
        public string Name { get; init; }
    }
}