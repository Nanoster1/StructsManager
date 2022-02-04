using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StructsManager.Logic
{
    public partial class CommandsManager
    {
        public Task<IEnumerable<CommandResult>> ActivateCommandsAsync(IEnumerable<CommandsElement> commandsElements)
        {
            return Task.Run(() => ActivateCommands(commandsElements));
        }

        private CommandResult ActivateOneCommand(CommandsElement commandsElement)
        {
            return ActivateCommands(new [] {commandsElement}).First();
        }

        public IEnumerable<CommandResult> ActivateCommands(IEnumerable<CommandsElement> commandsElements)
        {
            var results = new List<CommandResult>();
            foreach (var element in commandsElements)
            {
                var currentStruct = Variables[element.Name];
                foreach (var operation in element.Operations)
                {
                    var type = currentStruct as Type ?? currentStruct.GetType();
                    var op = type.GetMethod(operation) ?? type.GetProperty(operation)?.GetMethod;

                    object result;
                    if (op is null)
                    {
                        result = type.GetField(operation)?.GetValue(currentStruct) ?? throw new NullReferenceException();
                    }
                    else
                    {
                        var parameters = op.GetParameters();
                        var paramsCount = parameters.Length;
                        var finalParameters = new List<object>();
                        for (var i = 0; i < paramsCount; i++)
                        {
                            var parameter = element.Arguments.Dequeue();
                            if (parameter is CommandsElement commandsElement)
                            {
                                parameter = ActivateOneCommand(commandsElement).Result;
                            }
                            else if (parameter is object[] array)
                            {
                                array = array.Select(x =>
                                {
                                    if (x is CommandsElement comElem)
                                        return ActivateOneCommand(comElem).Result;
                                    return x;
                                }).ToArray();
                                
                                var parameterType = parameters[i].ParameterType;
                                var arrType = parameterType.GetElementType() ?? parameterType;
                                var newArr = Array.CreateInstance(arrType, array.Length);
                                
                                for (int j = 0; j < array.Length; j++)
                                {
                                    newArr.SetValue(array[j], j);
                                }
                                
                                parameter = newArr;
                            }

                            finalParameters.Add(parameter);
                        }

                        result = op.Invoke(currentStruct, finalParameters.ToArray());
                    }

                    if (result != null)
                    {
                        results.Add(new CommandResult(operation, result));
                    }
                }
            }
            return results;
        }
    }
}