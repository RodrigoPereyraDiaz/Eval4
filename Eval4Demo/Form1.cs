using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Eval4.Core;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Eval4.Demo
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {
        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        internal System.Windows.Forms.TabControl TabControl1;
        internal System.Windows.Forms.TabPage TabHeavier;
        private System.Windows.Forms.TabPage tabDynamic;
        private TabPage tabVB;
        private FormulaTab evaluatorPanel1;
        private TabPage tabCSharp;
        private FormulaTab evaluatorPanel2;
        private TabPage tabMathEval;
        private FormulaTab evaluatorPanel3;
        private TabPage tabExcel;
        private Panel panel3;
        private Label label11;
        private ExcelSheet excelSheet1;
        private BitmapFormula bitmapFormula1;
        private DynamicFormulas dynamicFormulas1;

        public Form1()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
        }

        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tabExcel = new System.Windows.Forms.TabPage();
            this.excelSheet1 = new Eval4.Demo.ExcelSheet();
            this.tabVB = new System.Windows.Forms.TabPage();
            this.evaluatorPanel1 = new Eval4.Demo.FormulaTab();
            this.tabCSharp = new System.Windows.Forms.TabPage();
            this.evaluatorPanel2 = new Eval4.Demo.FormulaTab();
            this.tabMathEval = new System.Windows.Forms.TabPage();
            this.evaluatorPanel3 = new Eval4.Demo.FormulaTab();
            this.TabHeavier = new System.Windows.Forms.TabPage();
            this.tabDynamic = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.bitmapFormula1 = new Eval4.Demo.BitmapFormula();
            this.dynamicFormulas1 = new Eval4.Demo.DynamicFormulas();
            this.TabControl1.SuspendLayout();
            this.tabExcel.SuspendLayout();
            this.tabVB.SuspendLayout();
            this.tabCSharp.SuspendLayout();
            this.tabMathEval.SuspendLayout();
            this.TabHeavier.SuspendLayout();
            this.tabDynamic.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.tabExcel);
            this.TabControl1.Controls.Add(this.tabVB);
            this.TabControl1.Controls.Add(this.tabCSharp);
            this.TabControl1.Controls.Add(this.tabMathEval);
            this.TabControl1.Controls.Add(this.TabHeavier);
            this.TabControl1.Controls.Add(this.tabDynamic);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.HotTrack = true;
            this.TabControl1.Location = new System.Drawing.Point(0, 53);
            this.TabControl1.Multiline = true;
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(518, 500);
            this.TabControl1.TabIndex = 5;
            this.TabControl1.SelectedIndexChanged += new System.EventHandler(this.TabControl1_SelectedIndexChanged);
            // 
            // tabExcel
            // 
            this.tabExcel.Controls.Add(this.excelSheet1);
            this.tabExcel.Location = new System.Drawing.Point(4, 22);
            this.tabExcel.Name = "tabExcel";
            this.tabExcel.Size = new System.Drawing.Size(510, 474);
            this.tabExcel.TabIndex = 10;
            this.tabExcel.Text = "Excel";
            this.tabExcel.UseVisualStyleBackColor = true;
            // 
            // excelSheet1
            // 
            this.excelSheet1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.excelSheet1.Location = new System.Drawing.Point(0, 0);
            this.excelSheet1.Name = "excelSheet1";
            this.excelSheet1.Size = new System.Drawing.Size(510, 474);
            this.excelSheet1.TabIndex = 2;
            // 
            // tabVB
            // 
            this.tabVB.Controls.Add(this.evaluatorPanel1);
            this.tabVB.Location = new System.Drawing.Point(4, 22);
            this.tabVB.Name = "tabVB";
            this.tabVB.Size = new System.Drawing.Size(510, 474);
            this.tabVB.TabIndex = 7;
            this.tabVB.Text = "VB";
            this.tabVB.UseVisualStyleBackColor = true;
            // 
            // evaluatorPanel1
            // 
            this.evaluatorPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.evaluatorPanel1.Location = new System.Drawing.Point(0, 0);
            this.evaluatorPanel1.Name = "evaluatorPanel1";
            this.evaluatorPanel1.PanelLanguage = Eval4.Demo.PanelLanguage.vb;
            this.evaluatorPanel1.Size = new System.Drawing.Size(510, 474);
            this.evaluatorPanel1.TabIndex = 0;
            // 
            // tabCSharp
            // 
            this.tabCSharp.Controls.Add(this.evaluatorPanel2);
            this.tabCSharp.Location = new System.Drawing.Point(4, 22);
            this.tabCSharp.Name = "tabCSharp";
            this.tabCSharp.Size = new System.Drawing.Size(510, 474);
            this.tabCSharp.TabIndex = 8;
            this.tabCSharp.Text = "C#";
            this.tabCSharp.UseVisualStyleBackColor = true;
            // 
            // evaluatorPanel2
            // 
            this.evaluatorPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.evaluatorPanel2.Location = new System.Drawing.Point(0, 0);
            this.evaluatorPanel2.Name = "evaluatorPanel2";
            this.evaluatorPanel2.PanelLanguage = Eval4.Demo.PanelLanguage.csharp;
            this.evaluatorPanel2.Size = new System.Drawing.Size(510, 474);
            this.evaluatorPanel2.TabIndex = 0;
            // 
            // tabMathEval
            // 
            this.tabMathEval.Controls.Add(this.evaluatorPanel3);
            this.tabMathEval.Location = new System.Drawing.Point(4, 22);
            this.tabMathEval.Name = "tabMathEval";
            this.tabMathEval.Size = new System.Drawing.Size(510, 474);
            this.tabMathEval.TabIndex = 9;
            this.tabMathEval.Text = "Math";
            this.tabMathEval.UseVisualStyleBackColor = true;
            // 
            // evaluatorPanel3
            // 
            this.evaluatorPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.evaluatorPanel3.Location = new System.Drawing.Point(0, 0);
            this.evaluatorPanel3.Name = "evaluatorPanel3";
            this.evaluatorPanel3.PanelLanguage = Eval4.Demo.PanelLanguage.mathEval;
            this.evaluatorPanel3.Size = new System.Drawing.Size(510, 474);
            this.evaluatorPanel3.TabIndex = 0;
            // 
            // TabHeavier
            // 
            this.TabHeavier.Controls.Add(this.bitmapFormula1);
            this.TabHeavier.Location = new System.Drawing.Point(4, 22);
            this.TabHeavier.Name = "TabHeavier";
            this.TabHeavier.Size = new System.Drawing.Size(510, 474);
            this.TabHeavier.TabIndex = 1;
            this.TabHeavier.Text = "heavier evaluation";
            // 
            // tabDynamic
            // 
            this.tabDynamic.Controls.Add(this.dynamicFormulas1);
            this.tabDynamic.Location = new System.Drawing.Point(4, 22);
            this.tabDynamic.Name = "tabDynamic";
            this.tabDynamic.Size = new System.Drawing.Size(510, 474);
            this.tabDynamic.TabIndex = 2;
            this.tabDynamic.Text = "Dynamic Formulas";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.Info;
            this.panel3.Controls.Add(this.label11);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(518, 53);
            this.panel3.TabIndex = 9;
            // 
            // label11
            // 
            this.label11.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label11.Location = new System.Drawing.Point(0, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(518, 53);
            this.label11.TabIndex = 0;
            this.label11.Text = "label11";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bitmapFormula1
            // 
            this.bitmapFormula1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bitmapFormula1.Location = new System.Drawing.Point(0, 0);
            this.bitmapFormula1.Name = "bitmapFormula1";
            this.bitmapFormula1.Size = new System.Drawing.Size(510, 474);
            this.bitmapFormula1.TabIndex = 0;
            // 
            // dynamicFormulas1
            // 
            this.dynamicFormulas1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dynamicFormulas1.Location = new System.Drawing.Point(0, 0);
            this.dynamicFormulas1.Name = "dynamicFormulas1";
            this.dynamicFormulas1.Size = new System.Drawing.Size(510, 474);
            this.dynamicFormulas1.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(518, 553);
            this.Controls.Add(this.TabControl1);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "Function Evaluation in C#";
            this.TabControl1.ResumeLayout(false);
            this.tabExcel.ResumeLayout(false);
            this.tabVB.ResumeLayout(false);
            this.tabCSharp.ResumeLayout(false);
            this.tabMathEval.ResumeLayout(false);
            this.TabHeavier.ResumeLayout(false);
            this.tabDynamic.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        public static void Main()
        {
            Form1 f = new Form1();
            f.ShowDialog();
        }

        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabControl1.SelectedTab == tabCSharp)
            {
            }
            else if (TabControl1.SelectedTab == tabCSharp)
            {
            }
            else if (TabControl1.SelectedTab == tabCSharp)
            {
            }
            else if (TabControl1.SelectedTab == tabCSharp)
            {
            }
            else if (TabControl1.SelectedTab == tabCSharp)
            {
            }
            else if (TabControl1.SelectedTab == tabCSharp)
            {
            }
        }

    }

}
