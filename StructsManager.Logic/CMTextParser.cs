#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StructsManager.Logic
{
    public partial class CommandsManager
    {
        public Task<List<CommandsElement>> ParseTextAsync(string[] text)
        {
            return Task.Run((() => ParseText(text)));
        }
        
        public List<CommandsElement> ParseText(string[] text)
        {
            //Queue1: Add(24) Peek() Print() Peek() Remove() Add(star) Print();
            var result = new List<CommandsElement>();
            var tasks = string.Join(' ', text).SaveOnlyMainSymbolsAndSplit(';');
            foreach (var task in tasks)
            {
                var element = ParseMainTask(task);
                result.Add(element);
            }

            return result;
        }
        
        private CommandsElement ParseMainTask(string task)
        {
            var taskParts = task.SaveOnlyMainSymbolsAndSplit(':');
            
            var varName = taskParts[0];
            var commands = taskParts[1].SaveOnlyMainSymbolsAndSplit(' ');

            var operations = new List<string>();
            var args = new Queue<object>();

            foreach (var command in commands)
            {
                var (opName, opArgs) = ParseCommand(command);
                if (opArgs != null)
                {
                    foreach (var opArg in opArgs)
                    {
                        args.Enqueue(opArg);
                    }
                }
                operations.Add(opName);
            }

            return new CommandsElement()
            {
                Name = varName,
                Arguments = args,
                Operations = operations
            };
        }
        
        private CommandsElement ParseSubTask(string subTask)
        {
            var taskParts = subTask.SaveOnlyMainSymbolsAndSplit('.');
            
            var varName = taskParts[0];
            var command = taskParts[1];

            var (opName, opArgs) = ParseCommand(command);
            return new CommandsElement()
            {
                Name = varName,
                Operations = new() { opName },
                Arguments = opArgs
            };
        }
        private (string, Queue<object>?) ParseCommand(string command)
        {
            var commandName = command;
            Queue<object>? args = null;
            if (command.Contains('('))
            {
                commandName = string.Concat(command.TakeWhile(x => x != '('));
                var argsStr = command.TakeMiddlePart('(', ')').SaveOnlyMainSymbolsAndSplit(',');
                args = new Queue<object>(argsStr.Select(ParseArg).Where(arg => arg != null));
            }
            return (commandName, args);
        }

        private object? ParseArg(string arg)
        {
            if (string.IsNullOrWhiteSpace(arg)) return null;
            if (arg[0] is '\"' && arg[^1] is '\"') return arg.Replace("\"", "");
            if (arg[0] is '\'' && arg[^1] is '\'') return Convert.ToChar(arg.Replace("\'", ""));
            if (arg[0] is '{' && arg[^1] is '}') return ParseArray(arg);
            if (byte.TryParse(arg, out var numInt16)) return numInt16;
            if (int.TryParse(arg, out var numInt32)) return numInt32;
            if (double.TryParse(arg.Replace('.',','), out var numDouble)) return numDouble;
            if (bool.TryParse(arg, out var boolValue)) return boolValue;
            if (CheckOnMathOperations(arg)) return _arithmeticManager.Calculate(arg);
            if (arg.Contains('.')) return ParseSubTask(arg);
            return Variables[arg];
        }
        
        private object[] ParseArray(string str)
        {
            var elements = str.TakeMiddlePart('{', '}')
                .SaveOnlyMainSymbolsAndSplit(',');
            var result = new object?[elements.Length];
            for (var i = 0; i < elements.Length; i++)
            {
                result[i] = ParseArg(elements[i]);
            }

            return result.Where(x => x != null).ToArray();
        }

        private bool CheckOnMathOperations(string arg)
        {
            var arithmeticsOps = new string[]
            {
                "log", "sin", "cos", "tg", "ctg", "ln"
            };
            return "+-/*^".Any(arg.Contains) || arithmeticsOps.Any(arg.Contains);
        }
    }
}