using System;
using System.Collections.Generic;
using System.Text;

namespace Eval4.Core
{
    class VariableBag : IVariableBag
    {
        Dictionary<string, Variable> mVariables;

        public VariableBag(bool caseSensitive)
        {
            mVariables = new Dictionary<string, Variable>(caseSensitive ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase);
        }

        public IEvalValue GetVariable(string varname)
        {
            Variable result;
            mVariables.TryGetValue(varname, out result);
            return result;
        }

        public void SetVariable<T>(string varname, T value)
        {
            Variable result;
            if (mVariables.TryGetValue(varname, out result))
            {
                ((Variable<T>)result).Value = value;
            }
            else
            {
                Variable<T> newVariable = new Variable<T>(value, varname);
                mVariables[varname] = newVariable;
            }

        }

        public void DeleteVariable(string varname)
        {
            mVariables.Remove(varname);
        }

    }

    public abstract class Variable : IEvalValue
    {
        protected string mDescription;
        protected string mName;

        public abstract object ObjectValue { get; }
        public abstract Type SystemType { get; }
        public event ValueChangedEventHandler ValueChanged;


        public string Description
        {
            get { return mDescription; }
            set { mDescription = value; }
        }

        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        public Variable(string description)
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
    }
}
