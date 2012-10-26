using System;
using System.Collections.Generic;

namespace Eval4
{

    public class Variable<T> : Core.Variable, Core.IHasValue<T>
    {
        private T mValue;

        public override Type SystemType
        {
            get { return typeof(T); }
        }

        public override object ObjectValue
        {
            get { return mValue; }
        }


        public Variable(T originalValue, string description = null)
            : base(description)
        {
            mValue = originalValue;
        }

        public T Value
        {
            get { return mValue; }
            set
            {
                this.mValue = value;
                mValue = value;
                base.RaiseValueChanged();
            }
        }
    }


}
