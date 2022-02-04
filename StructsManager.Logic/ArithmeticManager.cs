using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StructsManager.Logic
{
    public class ArithmeticManager
    {
        public static ArithmeticManager Instance => new ArithmeticManager();

        public Task<string> GetRpnAsync(string expression)
        {
            return Task.Run(() => GetRpn(expression));
        }

        public Task<double> CalculateAsync(string expression)
        {
            return Task.Run(() => Calculate(expression));
        }
        
        public string GetRpn(string expression)
        {
            var rpn = ParseInRpn(ParseExpression(expression));
            var rpnEnumerable = rpn.Select(element =>
            {
                if (element is Operation operation) return operation.Name;
                return element;
            });
            return string.Join(" ", rpnEnumerable.Reverse());
        }

        public double Calculate(string expression)
        {
            return CalculateRpn(new Stack<object>(ParseInRpn(ParseExpression(expression))));
        }

        private List<object> ParseExpression(string text)
        {
            text = text.Replace(" ", string.Empty);
            text = '⊥' + text + '⊥';
            List<object> expression = new List<object>();
            for (int i = 0; i < text.Length; i++)
            {
                if (char.IsDigit(text[i]) || 
                    ((text[i] == '-' || text[i] == '+') && !char.IsDigit(text[i - 1])) && text[i - 1] != ')')
                    expression.Add(ReadNumber(text, ref i));
                else if (text[i] is '(' or ')' or '⊥') expression.Add(text[i]);
                else if (char.IsWhiteSpace(text[i])) i++;
                else expression.Add(ReadOperation(text, ref i));
            }

            return expression;
        }

        private bool IsPrefix(string text, int i)
        {
            return "-+".Contains(text[i]) && (i < 1 || !char.IsDigit(text[i - 1])) && char.IsDigit(text[i + 1]);
        }

        private object ReadNumber(string text, ref int i)
        {
            var num = new StringBuilder().Append(text[i]);
            while (i < text.Length - 1 && (char.IsDigit(text[i + 1]) || text[i + 1] is ',' or '.'))
            {
                i++;
                num.Append(text[i]);
            }
            return ParseNumber(num.ToString());
        }

        private object ParseNumber(string num)
        {
            if (double.TryParse(num?.Replace('.',','), out var numDouble)) return numDouble;
            throw new ArgumentException();
        }
        
        private Operation ReadOperation(string text, ref int i)
        {
            var op = new StringBuilder().Append(text[i]);
            while (!char.IsDigit(text[i + 1]) && text[i + 1] is not '(')
            {
                if ("+-*/^".Contains(text[i])) break;
                i++;
                op.Append(text[i]);
            }
            return ChooseOp(op.ToString());
        }

        private Operation ChooseOp(string op) => op.ToLower() switch
        {
            "+" => new Plus(),
            "-" => new Minus(),
            "*" => new Mult(),
            "/" => new Div(),
            "^" => new Rank(),
            "%" => new Remainder(),
            "ln" => new Ln(),
            "log" => new Log(),
            "sin" => new Sin(),
            "cos" => new Cos(),
            "tg" => new Tan(),
            "sqrt" => new Sqrt(),
            _ => throw new ArgumentException()
        };

        private static Stack<object> ParseInRpn(List<object> expression)
        {
            var california = new Stack<object>();
            var texas = new Stack<object>();
            texas.Push(expression[0]);
            var i = 1;
            while (true)
            {
                if (expression[i] is double)
                {
                    california.Push(expression[i]);
                    i++;
                }
                else if (expression[i] is '(')
                {
                    texas.Push(expression[i]);
                    i++;
                }
                else if (expression[i] is ')')
                {
                    if (texas.Peek() is Operation)
                    {
                        california.Push(texas.Pop());
                    }
                    else if (texas.Peek() is '(')
                    {
                        texas.Pop();
                        i++;
                    }
                    else throw new SyntaxErrorException("Incorrect expression");
                }
                else if (expression[i] is Operation operation)
                {
                    if (operation.Prior == 1)
                    {
                        texas.Push(expression[i]);
                        i++;
                    }
                    else if (operation.Prior == 2 &&
                             (!(texas.Peek() is Operation) || (texas.Peek() as Operation)?.Prior == 3))
                    {
                        texas.Push(expression[i]);
                        i++;
                    }
                    else if (operation.Prior == 2 || (operation.Prior == 3 && texas.Peek() is Operation))
                    {
                        california.Push(texas.Pop());
                    }
                    else if (operation.Prior == 3)
                    {
                        texas.Push(expression[i]);
                        i++;
                    }
                }
                else if (expression[i] is '⊥')
                {
                    if (texas.Peek() is Operation) california.Push(texas.Pop());
                    else break;
                }
            }

            return california;
        }

        private double CalculateRpn(Stack<object> rpn)
        {
            var calc = new Stack<double>();
            foreach (var element in rpn)
            {
                if (element is double)
                {
                    calc.Push(Convert.ToDouble(element));
                }
                else if (element is Operation operation)
                {
                    if (operation.CountParams == 2)
                    {
                        double[] @params = { calc.Pop(), calc.Pop() };  //x2 , x1
                        calc.Push(operation.Calculate(@params));
                    }
                    else
                    {
                        double[] @params = { calc.Pop() };
                        calc.Push(operation.Calculate(@params));
                    }
                }
            }
            return calc.Pop();
        }
    }
}
