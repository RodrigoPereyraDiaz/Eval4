using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Eval4.Demo
{
    public partial class DynamicFormulas : UserControl
    {
        // Note that these 3 variables are visible from within the evaluator 
        // without needing any assessor 
        public Eval4.Core.Variable<double> A;
        public Eval4.Core.Variable<double> B;
        public Eval4.Core.Variable<double> C;
        // Required by the Windows Form Designer
        private Eval4.Core.IParsedExpr mFormula3;
        private MathEvaluator ev;

        public DynamicFormulas()
        {
            InitializeComponent();

            A = new Eval4.Core.Variable<double>((double)updownA.Value, "UpDown A");
            B = new Eval4.Core.Variable<double>((double)updownB.Value, "UpDown B");
            C = new Eval4.Core.Variable<double>((double)updownC.Value, "UpDown C");
            ev = new MathEvaluator();
            ev.AddEnvironmentFunctions(this);
            ev.SetVariable("EvalFunctions", new EvalFunctions());

        }

        private void btnEvaluate3_Click(object sender, EventArgs e)
        {
            try
            {
                // throw new NotImplementedException();
                // if (FormulaHandler != null) mFormula3.ValueChanged -= FormulaHandler;
                //if (formula3subscription != null) formula3subscription.Dispose();
                using (mFormula3 = ev.Parse(tbExpression3.Text))
                {
                    string v = ev.ConvertToString(mFormula3.ObjectValue);
                    lblResults3.Text = v;
                    LogBox3.AppendText(System.DateTime.Now.ToLongTimeString() + ": " + v + "\r\n");
                }


                //FormulaHandler = new Eval4.Core.ValueChangedEventHandler(mFormula3_ValueChanged);
                // mFormula3.ValueChanged += FormulaHandler;
                // mFormula3_ValueChanged(null, null);
            }
            catch (Exception ex)
            {
                lblResults3.Text = ex.Message;
            }
        }

        private void updownA_ValueChanged(object sender, EventArgs e)
        {
            A.SetValue((double)updownA.Value);
            B.SetValue((double)updownB.Value);
            C.SetValue((double)updownC.Value);
        }
    }
}
