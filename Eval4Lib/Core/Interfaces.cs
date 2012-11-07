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

    public interface IVariable : IHasValue
    {
        void SetValue(object variableValue);
    }

    public interface IObserver
    {
        void OnValueChanged();
    }

    public class Dependency
    {
        public String Name;
        public IHasValue Expr;

        public Dependency(string name, IHasValue value)
        {
            this.Name = name;
            this.Expr = value;
        }
    }

}
