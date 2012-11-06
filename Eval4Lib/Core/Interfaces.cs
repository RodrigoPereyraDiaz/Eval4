using System;
using System.Collections.Generic;

namespace Eval4.Core
{
    //public interface IVariableBag
    //{
    //    IHasValue GetVariable(string varname);
    //}

    public interface IHasValue
    {
        object ObjectValue { get; }
        IDisposable Subscribe(IObserver observer);
        Type SystemType { get; }
        string ShortName { get; }
        IEnumerable<Dependency> Dependencies { get; }
        //ValueChangedEventHandler ValueChanged { get; set; }
    }

    public interface IVariable : IHasValue
    {
        void SetValue(object variableValue);
    }

    public static class StaticIHasValue
    {
        public static IDisposable Subscribe(this IHasValue source, Action action)
        {
            return source.Subscribe(new SmallObserver(source, action));
        }

        public class SmallObserver : IObserver
        {
            private IHasValue mSource;
            private Action mAction;
            
            public SmallObserver(IHasValue source, Action action)
            {
                mSource = source;
                mAction = action;
            }

            public void OnValueChanged()
            {
                mAction();
            }
        }
    }

    public interface IHasValue<T> : IHasValue
    {
        T Value { get; }
    }

    public interface IObserver
    {
        void OnValueChanged();
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

    //    ValueChangedEventHandler ValueChanged { get; set; }
    //    public event ValueChangedEventHandler(object sender, System.EventArgs e);
}
