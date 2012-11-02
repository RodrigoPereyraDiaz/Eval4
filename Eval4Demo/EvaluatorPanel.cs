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
    public partial class EvaluatorPanel : UserControl
    {
        public EvaluatorPanel()
        {
            InitializeComponent();
        }

        public PanelLanguage PanelLanguage { get; set; }

        private void EvaluatorPanel_Load(object sender, EventArgs e)
        {
            //switch (PanelLanguage)
            //{
            //    case PanelLanguage.excel:
            //        label1.Text = "Formula evaluator similar to Excel";
            //        break;
            //    case PanelLanguage.vb:
            //        label1.Text = "Formula evaluator similar to Visual Basic";
            //        break;
            //    case PanelLanguage.csharp:
            //        label1.Text = "Formula evaluator similar to C#";
            //        break;
            //    case PanelLanguage.matlab:
            //        label1.Text = "Formula evaluator similar to Matlab";
            //        break;
            //    case PanelLanguage.experiments:
            //        label1.Text = "Formula evaluator similar to Excel";
            //        break;
            //}
        }
    }

    public enum PanelLanguage
    {
        excel,
        vb,
        csharp,
        matlab,
        experiments
    }
}
