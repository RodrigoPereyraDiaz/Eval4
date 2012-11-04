using System;
using System.Collections.Generic;
using System.Text;

namespace Eval4.Core
{
    //public class VariableBag : IVariableBag
    //{
    //    Dictionary<string, VariableBase> mVariables;

    //    public VariableBag(bool caseSensitive)
    //    {
    //        mVariables = new Dictionary<string, VariableBase>(caseSensitive ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase);
    //    }

    //    public IHasValue GetVariable(string varname)
    //    {
    //        VariableBase result;
    //        mVariables.TryGetValue(varname, out result);
    //        return result;
    //    }

    //    public void SetVariable<T>(string varname, T value)
    //    {
    //        VariableBase result;
    //        if (mVariables.TryGetValue(varname, out result))
    //        {
    //            ((Variable<T>)result).Value = value;
    //        }
    //        else
    //        {
    //            Variable<T> newVariable = new Variable<T>(value, varname);
    //            mVariables[varname] = newVariable;
    //        }

    //    }

    //    public void DeleteVariable(string varname)
    //    {
    //        mVariables.Remove(varname);
    //    }

    //}

    public abstract class VariableBase : IHasValue
    {
        protected string mDescription;
        protected string mName;

        public abstract object ObjectValue { get; set;  }
        public abstract Type SystemType { get; }
        public event ValueChangedEventHandler ValueChanged;

        public VariableBase(string description)
        {
            mDescription = description;
        }

        protected void RaiseValueChanged()
        {
            if (ValueChanged != null)
            {
                ValueChanged(this, new System.EventArgs());
            }
        }


        public IEnumerable<Dependency> Dependencies
        {
            get {
                yield break;
            }
        }


        public string ShortName
        {
            get { return "Variable " + mName; }
        }
    }
}
