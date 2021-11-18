using System;

namespace StructsConsole
{
    internal abstract class Operation
    {
        public abstract string Name { get; }
        public abstract int CountParams { get; }
        public abstract double Calculate(double[] @params);
        public abstract byte Prior { get; }
    }
    internal class Plus : Operation
    {
        public override string Name => "+";
        public override int CountParams => 2;
        public override double Calculate(double[] @params) { return @params[1] + @params[0]; }
        public override byte Prior => 3;
    }
    internal class Minus : Operation
    {
        public override string Name => "-";
        public override int CountParams => 2;
        public override double Calculate(double[] @params) { return @params[1] - @params[0]; }
        public override byte Prior => 3;
    }

    internal class Mult : Operation
    {
        public override string Name => "*";
        public override int CountParams => 2;
        public override double Calculate(double[] @params) { return @params[1] * @params[0]; }
        public override byte Prior => 2;
    }
    internal class Div : Operation
    {
        public override string Name => "/";
        public override int CountParams => 2;
        public override double Calculate(double[] @params) { return @params[1] / @params[0]; }
        public override byte Prior => 2;
    }
    internal class Remainder : Operation
    {
        public override string Name => "%";
        public override int CountParams => 2;
        public override double Calculate(double[] @params) { return @params[1] % @params[0]; }
        public override byte Prior => 2;
    }
    internal class Log : Operation
    {
        public override string Name => "log";
        public override int CountParams => 2;
        public override double Calculate(double[] @params) { return Math.Log(@params[0], @params[1]); }                    //params 1 = основание
        public override byte Prior => 1;
    }
    internal class Ln : Operation
    {
        public override string Name => "ln";
        public override int CountParams => 1;
        public override double Calculate(double[] @params) { return Math.Log(@params[0]); }                    //params 1 = основание
        public override byte Prior => 1;
    }
    internal class Sin : Operation
    {
        public override string Name => "sin";
        public override int CountParams => 1;
        public override double Calculate(double[] @params) { return Math.Sin(@params[0]); }
        public override byte Prior => 1;
    }
    internal class Cos : Operation
    {
        public override string Name => "cos";
        public override int CountParams => 1;
        public override double Calculate(double[] @params) { return Math.Cos(@params[0]); }
        public override byte Prior => 1;
    }
    internal class Tan : Operation
    {
        public override string Name => "tg";
        public override int CountParams => 1;
        public override double Calculate(double[] @params) { return Math.Tan(@params[0]); }
        public override byte Prior => 1;
    }
    internal class Rank: Operation
    {
        public override string Name => "^";
        public override int CountParams => 2;
        public override double Calculate(double[] @params) { return Math.Pow(@params[1], @params[0]); }
        public override byte Prior => 1;
    }
    internal class Sqrt : Operation
    {
        public override string Name => "sqrt";
        public override int CountParams => 1;
        public override double Calculate(double[] @params) { return Math.Sqrt(@params[0]); }
        public override byte Prior => 1;
    }
}
