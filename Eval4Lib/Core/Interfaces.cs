using System;
using System.Collections.Generic;
using System.Linq;

namespace Eval4.Core
{
    public interface IEvaluator
    {
        void SetVariable<T>(string variableName, T variableValue);

        Variable<T> GetVariable<T>(string variableName);

        IHasValue Parse(string formula);
        object Eval(string formula);

        //IHasValue<string> ParseTemplate(string template);
        //string EvalTemplate(string template);
        void AddEnvironmentFunctions(object o);
        void RemoveEnvironmentFunctions(object o);
        
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
        private String mName;
        private IHasValue mHasValue;

        public Dependency(string name, IHasValue value)
        {
            this.mName = name;
            this.mHasValue = value;
        }

        public String Name { get { return mName; } }
        public IHasValue Value { get { return mHasValue; } }

        internal static Dependency[] Group(string groupname, IEnumerable<IHasValue> @params)
        {
            if (@params == null) return new Dependency[] { };
            return @params.Select((p, n) => new Dependency(groupname + (n + 1), p)).ToArray();
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
