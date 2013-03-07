using System;
using System.Collections.Generic;

namespace Eval4.Core
{
    public interface IEvaluator
    {
        void SetVariable<T>(string variableName, T variableValue);

        Variable<T> GetVariable<T>(string variableName);

        //IHasValue Parse(string formula);
        object Eval(string formula);

        void AddEnvironmentFunctions(object o);
        void RemoveEnvironmentFunctions(object o);

        string ConvertToString(object result);

        IParsedExpr Parse(string formula, Action onValueChanged = null);
    }

    public interface IParsedExpr : IDisposable
    {
        object ObjectValue { get; }

        string Error { get; }
    }

    public interface IHasValue : IDisposable
    {
        object ObjectValue { get; }
        ISubscription Subscribe(IObserver observer, string role);
        Type ValueType { get; }
        string ShortName { get; }
        IEnumerable<ISubscription> Subscriptions { get; }
    }

    public interface IHasValue<T> : IHasValue
    {
        T Value { get; }
    }

    public interface ISubscription : IDisposable
    {
        string Name { get; }
        IHasValue Source { get; }
        IObserver Observer { get; }
    }

    public interface IVariable : IHasValue
    {
        void SetValue(object variableValue);
    }

    public interface IObserver
    {
        void OnValueChanged(IHasValue value);
    }

    public static class IHasValueExtensionMethods
    {
        public static IDisposable Subscribe(this IHasValue source, string role,Action action)
        {
            return source.Subscribe(new SimpleObserver(source, action), role);
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

            public void OnValueChanged(IHasValue value)
            {
                mAction();
            }
        }
    }
}
