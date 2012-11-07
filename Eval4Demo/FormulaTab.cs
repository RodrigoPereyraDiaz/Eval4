using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Eval4.DemoCSharp
{
    public partial class FormulaTab : UserControl
    {
        Core.Evaluator ev;

        public FormulaTab()
        {
            InitializeComponent();
        }

        public PanelLanguage PanelLanguage { get; set; }

        private void EvaluatorPanel_Load(object sender, EventArgs e)
        {
            var items = new List<object>();

            switch (PanelLanguage)
            {
                case PanelLanguage.vb:
                    ev = new VbEvaluator();
                    items.Add("1+2*3");
                    break;
                case PanelLanguage.csharp:
                    ev = new CSharpEvaluator();
                    //label1.Text = "Formula evaluator similar to C#";
                    break;
                case PanelLanguage.mathEval:
                    ev = new MathEvaluator();
                    //label1.Text = "Formula evaluator similar to Matlab";
                    break;
                case PanelLanguage.experiments:
                    ev = null;
                    //label1.Text = "Formula evaluator similar to Excel";
                    break;
            }
            this.cbSamples.Items.AddRange(items.ToArray());
        }

        private void btnEvaluate_Click(object sender, EventArgs e)
        {
            try
            {
                var res = ev.Eval(cbSamples.Text);
                TextBox2.AppendText(cbSamples.Text + Environment.NewLine);
                TextBox2.AppendText(res.ToString() + Environment.NewLine);
            }
            catch (Exception ex)
            {
                TextBox2.AppendText(ex.Message + Environment.NewLine);
            }
        }

        private void cbSamples_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnEvaluate_Click(sender, e);
        }
    }

    public enum PanelLanguage
    {
        excel,
        vb,
        csharp,
        mathEval,
        experiments
    }
}
