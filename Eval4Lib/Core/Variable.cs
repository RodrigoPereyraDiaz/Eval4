using System;
using System.Collections.Generic;
using System.Text;

namespace Eval4.Core
{
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
