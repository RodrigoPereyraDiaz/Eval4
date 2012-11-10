using System;
using System.Collections.Generic;

namespace Eval4.Core
{
    public interface IEvaluator
    {
        void SetVariable<T>(string variableName, T variableValue);

        Variable<T> GetVariable<T>(string variableName);

        IHasValue Parse(string formula);
        object Eval(string formula);

        IHasValue<string> ParseTemplate(string template);
        string EvalTemplate(string template);

        string ConvertToString(object result);
    }

    public interface IHasValue
    {
        object ObjectValue { get; }
        IDisposable Subscribe(IObserver observer);
        Type ValueType { get; }
        string ShortName { get; }
        IEnumerable<Dependency> Dependencies { get; }
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

    public static class IHasValueExtensionMethods
    {
        public static IDisposable Subscribe(this IHasValue source, Action action)
        {
            return source.Subscribe(new SimpleObserver(source, action));
        }

        public class SimpleObserver : IObserver
        {
            private IHasValue mSource;
            private Action mAction;

            public SimpleObserver(IHasValue source, Action action)
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
}
