using System;

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
        //public string Expression { get; }
        //public IEvalValue[] Dependencies { get; }
        //public int Priority { get; }
    }

    public interface IHasValue<T>
    {
        T Value { get; }        
    }

    public delegate void ValueChangedEventHandler(object sender, System.EventArgs e);
        
}
