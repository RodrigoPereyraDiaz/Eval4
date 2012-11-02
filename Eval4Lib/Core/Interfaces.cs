using System;
using System.Collections.Generic;

namespace Eval4.Core
{
    public interface IVariableBag
    {
        IHasValue GetVariable(string varname);
    }

    public interface IHasValue
    {
        object ObjectValue { get; }
        event ValueChangedEventHandler ValueChanged;
        Type SystemType { get; }
        string ShortName { get; }
        IEnumerable<Dependency> Dependencies { get; }
    }

    public interface IHasValue<T> : IHasValue
    {
        T Value { get; }
    }

    public class Dependency
    {
        public String Name;
        public IHasValue Expr;

        private static Dependency[] NoDependencies = new Dependency[] { };

        public Dependency(string name, IHasValue value)
        {
            this.Name = name;
            this.Expr = value;
        }
        public static IEnumerable<Dependency> None { get { return NoDependencies; } }
    }

    public delegate void ValueChangedEventHandler(object sender, System.EventArgs e);
}
