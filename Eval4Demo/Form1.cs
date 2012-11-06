using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using Eval4;
using Eval4.Core;

namespace Eval4.DemoCSharp
{
    /// <summary>
    /// Summary description for Form1.
    /// </summary>
    public class Form1 : System.Windows.Forms.Form
    {

        // Required by the Windows Form Designer
        private CSharpEvaluator ev;
        private bool mInitializing;
        private Eval4.Core.IHasValue mFormula3;

        // Note that these 3 variables are visible from within the evaluator 
        // without needing any assessor 
        public Eval4.Core.Variable<double> A;
        public Eval4.Core.Variable<double> B;
        public Eval4.Core.Variable<double> C;
        public double[] arr = { 1.2, 3.4, 5.6, 7.8 };

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        internal System.Windows.Forms.TabControl TabControl1;
        internal System.Windows.Forms.TabPage TabHeavier;
        internal System.Windows.Forms.ComboBox ComboBox1;
        internal System.Windows.Forms.Button btnEvaluate2;
        internal System.Windows.Forms.PictureBox PictureBox1;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.TextBox tbExpressionRed;
        internal System.Windows.Forms.TextBox tbExpressionGreen;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox tbExpressionBlue;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.CheckBox cbAuto;

        public double X;
        public double Y;

        private System.Windows.Forms.TabPage tabDynamic;
        internal System.Windows.Forms.Button btnEvaluate3;
        internal System.Windows.Forms.TextBox LogBox3;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.NumericUpDown updownA;
        internal System.Windows.Forms.TextBox tbExpression3;
        internal System.Windows.Forms.NumericUpDown updownB;
        internal System.Windows.Forms.NumericUpDown updownC;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.Label Label8;
        private TabPage tabVB;
        private FormulaTab evaluatorPanel1;
        private TabPage tabCSharp;
        private FormulaTab evaluatorPanel2;
        private TabPage tabMatlab;
        private FormulaTab evaluatorPanel3;
        private TabPage tabExcel;
        private TabPage tabExperiments;
        private FormulaTab evaluatorPanel5;
        private Panel panel3;
        private Label label11;
        private TabControl tabControl2;
        private TabPage tabSheet1;
        private TabPage tabSheet2;
        private TabPage tabSheet3;
        private ExcelSheet excelSheet1;
        private ExcelSheet excelSheet2;
        private ExcelSheet excelSheet3;
        internal System.Windows.Forms.Label lblResults3;
        private IDisposable formula3subscription;

        public Form1()
        {
            mInitializing = true;
            ev = new Eval4.CSharpEvaluator();
            //ev.AddEnvironmentFunctions(this);
            //ev.AddEnvironmentFunctions(new EvalFunctions());
            ev.SetVariable("EvalFunctions", new EvalFunctions());
            // This call is required by the Windows Form Designer.



            // This call is required by the Windows Form Designer.
            InitializeComponent();

            A = new Eval4.Core.Variable<double>((double)updownA.Value, "UpDown A");
            B = new Eval4.Core.Variable<double>((double)updownB.Value, "UpDown B");
            C = new Eval4.Core.Variable<double>((double)updownC.Value, "UpDown C");

            // Add any initialization after the InitializeComponent() call
            mInitializing = false;
            btnEvaluate2_Click(null, null);
            btnEvaluate3_Click(null, null);
        }

        public string Description
        {
            get
            {
                return "This is form1";
            }
        }

        public string Name1
        {
            get
            {
                return this.Name;
            }
        }

        public System.Type SystemType
        {
            get
            {
                return this.GetType();
            }
        }

        public object Value
        {
            get
            {
                return this;
            }
        }

        public Form1 me
        {
            get
            {
                return this;
            }
        }

        public Form1 theForm
        {
            get
            {
                return this;
            }
        }


        // Form overrides dispose to clean up the component list.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
            base.Dispose(disposing);
        }

        [System.Diagnostics.DebuggerStepThrough()]
        private void InitializeComponent()
        {
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.tabExcel = new System.Windows.Forms.TabPage();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabSheet1 = new System.Windows.Forms.TabPage();
            this.tabSheet2 = new System.Windows.Forms.TabPage();
            this.tabSheet3 = new System.Windows.Forms.TabPage();
            this.tabVB = new System.Windows.Forms.TabPage();
            this.tabCSharp = new System.Windows.Forms.TabPage();
            this.tabMatlab = new System.Windows.Forms.TabPage();
            this.tabExperiments = new System.Windows.Forms.TabPage();
            this.TabHeavier = new System.Windows.Forms.TabPage();
            this.cbAuto = new System.Windows.Forms.CheckBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.PictureBox1 = new System.Windows.Forms.PictureBox();
            this.tbExpressionRed = new System.Windows.Forms.TextBox();
            this.ComboBox1 = new System.Windows.Forms.ComboBox();
            this.btnEvaluate2 = new System.Windows.Forms.Button();
            this.tbExpressionGreen = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label3 = new System.Windows.Forms.Label();
            this.tbExpressionBlue = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.tabDynamic = new System.Windows.Forms.TabPage();
            this.btnEvaluate3 = new System.Windows.Forms.Button();
            this.LogBox3 = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.updownA = new System.Windows.Forms.NumericUpDown();
            this.tbExpression3 = new System.Windows.Forms.TextBox();
            this.updownB = new System.Windows.Forms.NumericUpDown();
            this.updownC = new System.Windows.Forms.NumericUpDown();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.lblResults3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label11 = new System.Windows.Forms.Label();
            this.excelSheet1 = new Eval4.DemoCSharp.ExcelSheet();
            this.excelSheet2 = new Eval4.DemoCSharp.ExcelSheet();
            this.excelSheet3 = new Eval4.DemoCSharp.ExcelSheet();
            this.evaluatorPanel1 = new Eval4.DemoCSharp.FormulaTab();
            this.evaluatorPanel2 = new Eval4.DemoCSharp.FormulaTab();
            this.evaluatorPanel3 = new Eval4.DemoCSharp.FormulaTab();
            this.evaluatorPanel5 = new Eval4.DemoCSharp.FormulaTab();
            this.TabControl1.SuspendLayout();
            this.tabExcel.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabSheet1.SuspendLayout();
            this.tabSheet2.SuspendLayout();
            this.tabSheet3.SuspendLayout();
            this.tabVB.SuspendLayout();
            this.tabCSharp.SuspendLayout();
            this.tabMatlab.SuspendLayout();
            this.tabExperiments.SuspendLayout();
            this.TabHeavier.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            this.tabDynamic.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updownA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownC)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.tabExcel);
            this.TabControl1.Controls.Add(this.tabVB);
            this.TabControl1.Controls.Add(this.tabCSharp);
            this.TabControl1.Controls.Add(this.tabMatlab);
            this.TabControl1.Controls.Add(this.tabExperiments);
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
            this.tabExcel.Controls.Add(this.tabControl2);
            this.tabExcel.Location = new System.Drawing.Point(4, 22);
            this.tabExcel.Name = "tabExcel";
            this.tabExcel.Size = new System.Drawing.Size(510, 474);
            this.tabExcel.TabIndex = 10;
            this.tabExcel.Text = "Excel";
            this.tabExcel.UseVisualStyleBackColor = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Appearance = System.Windows.Forms.TabAppearance.Buttons;
            this.tabControl2.Controls.Add(this.tabSheet1);
            this.tabControl2.Controls.Add(this.tabSheet2);
            this.tabControl2.Controls.Add(this.tabSheet3);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl2.Location = new System.Drawing.Point(0, 0);
            this.tabControl2.Multiline = true;
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(510, 474);
            this.tabControl2.TabIndex = 0;
            // 
            // tabSheet1
            // 
            this.tabSheet1.Controls.Add(this.excelSheet1);
            this.tabSheet1.Location = new System.Drawing.Point(4, 25);
            this.tabSheet1.Name = "tabSheet1";
            this.tabSheet1.Size = new System.Drawing.Size(502, 445);
            this.tabSheet1.TabIndex = 0;
            this.tabSheet1.Text = "Sheet1";
            this.tabSheet1.UseVisualStyleBackColor = true;
            // 
            // tabSheet2
            // 
            this.tabSheet2.Controls.Add(this.excelSheet2);
            this.tabSheet2.Location = new System.Drawing.Point(4, 25);
            this.tabSheet2.Name = "tabSheet2";
            this.tabSheet2.Size = new System.Drawing.Size(502, 445);
            this.tabSheet2.TabIndex = 1;
            this.tabSheet2.Text = "Sheet2";
            this.tabSheet2.UseVisualStyleBackColor = true;
            // 
            // tabSheet3
            // 
            this.tabSheet3.Controls.Add(this.excelSheet3);
            this.tabSheet3.Location = new System.Drawing.Point(4, 25);
            this.tabSheet3.Name = "tabSheet3";
            this.tabSheet3.Size = new System.Drawing.Size(502, 445);
            this.tabSheet3.TabIndex = 2;
            this.tabSheet3.Text = "Sheet3";
            this.tabSheet3.UseVisualStyleBackColor = true;
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
            // tabMatlab
            // 
            this.tabMatlab.Controls.Add(this.evaluatorPanel3);
            this.tabMatlab.Location = new System.Drawing.Point(4, 22);
            this.tabMatlab.Name = "tabMatlab";
            this.tabMatlab.Size = new System.Drawing.Size(510, 474);
            this.tabMatlab.TabIndex = 9;
            this.tabMatlab.Text = "Matlab";
            this.tabMatlab.UseVisualStyleBackColor = true;
            // 
            // tabExperiments
            // 
            this.tabExperiments.Controls.Add(this.evaluatorPanel5);
            this.tabExperiments.Location = new System.Drawing.Point(4, 22);
            this.tabExperiments.Name = "tabExperiments";
            this.tabExperiments.Size = new System.Drawing.Size(510, 474);
            this.tabExperiments.TabIndex = 11;
            this.tabExperiments.Text = "Experiments";
            this.tabExperiments.UseVisualStyleBackColor = true;
            // 
            // TabHeavier
            // 
            this.TabHeavier.Controls.Add(this.cbAuto);
            this.TabHeavier.Controls.Add(this.Label1);
            this.TabHeavier.Controls.Add(this.PictureBox1);
            this.TabHeavier.Controls.Add(this.tbExpressionRed);
            this.TabHeavier.Controls.Add(this.ComboBox1);
            this.TabHeavier.Controls.Add(this.btnEvaluate2);
            this.TabHeavier.Controls.Add(this.tbExpressionGreen);
            this.TabHeavier.Controls.Add(this.Label2);
            this.TabHeavier.Controls.Add(this.Label3);
            this.TabHeavier.Controls.Add(this.tbExpressionBlue);
            this.TabHeavier.Controls.Add(this.Label4);
            this.TabHeavier.Location = new System.Drawing.Point(4, 22);
            this.TabHeavier.Name = "TabHeavier";
            this.TabHeavier.Size = new System.Drawing.Size(510, 474);
            this.TabHeavier.TabIndex = 1;
            this.TabHeavier.Text = "heavier evaluation";
            // 
            // cbAuto
            // 
            this.cbAuto.Checked = true;
            this.cbAuto.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAuto.Location = new System.Drawing.Point(353, 65);
            this.cbAuto.Name = "cbAuto";
            this.cbAuto.Size = new System.Drawing.Size(64, 24);
            this.cbAuto.TabIndex = 10;
            this.cbAuto.Text = "Auto";
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(28, 107);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(408, 16);
            this.Label1.TabIndex = 9;
            this.Label1.Text = "Label1";
            // 
            // PictureBox1
            // 
            this.PictureBox1.Location = new System.Drawing.Point(89, 167);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(256, 256);
            this.PictureBox1.TabIndex = 8;
            this.PictureBox1.TabStop = false;
            // 
            // tbExpressionRed
            // 
            this.tbExpressionRed.Location = new System.Drawing.Point(65, 33);
            this.tbExpressionRed.Name = "tbExpressionRed";
            this.tbExpressionRed.Size = new System.Drawing.Size(280, 20);
            this.tbExpressionRed.TabIndex = 4;
            this.tbExpressionRed.Text = "X*15";
            this.tbExpressionRed.TextChanged += new System.EventHandler(this.tbExpressionBlue_TextChanged);
            // 
            // ComboBox1
            // 
            this.ComboBox1.Items.AddRange(new object[] {
            "Sample1",
            "Sample2",
            "Sample3",
            "Sample4"});
            this.ComboBox1.Location = new System.Drawing.Point(17, 9);
            this.ComboBox1.Name = "ComboBox1";
            this.ComboBox1.Size = new System.Drawing.Size(408, 21);
            this.ComboBox1.TabIndex = 6;
            this.ComboBox1.Text = "<enter an expression or select a sample>";
            this.ComboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // btnEvaluate2
            // 
            this.btnEvaluate2.Location = new System.Drawing.Point(353, 33);
            this.btnEvaluate2.Name = "btnEvaluate2";
            this.btnEvaluate2.Size = new System.Drawing.Size(72, 23);
            this.btnEvaluate2.TabIndex = 5;
            this.btnEvaluate2.Text = "Evaluate";
            this.btnEvaluate2.Click += new System.EventHandler(this.btnEvaluate2_Click);
            // 
            // tbExpressionGreen
            // 
            this.tbExpressionGreen.Location = new System.Drawing.Point(65, 57);
            this.tbExpressionGreen.Name = "tbExpressionGreen";
            this.tbExpressionGreen.Size = new System.Drawing.Size(280, 20);
            this.tbExpressionGreen.TabIndex = 4;
            this.tbExpressionGreen.Text = "Cos(X*Y*4900)";
            this.tbExpressionGreen.TextChanged += new System.EventHandler(this.tbExpressionBlue_TextChanged);
            // 
            // Label2
            // 
            this.Label2.Location = new System.Drawing.Point(17, 36);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(40, 16);
            this.Label2.TabIndex = 9;
            this.Label2.Text = "Red";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label3
            // 
            this.Label3.Location = new System.Drawing.Point(17, 60);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(40, 16);
            this.Label3.TabIndex = 9;
            this.Label3.Text = "Green";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbExpressionBlue
            // 
            this.tbExpressionBlue.Location = new System.Drawing.Point(65, 81);
            this.tbExpressionBlue.Name = "tbExpressionBlue";
            this.tbExpressionBlue.Size = new System.Drawing.Size(280, 20);
            this.tbExpressionBlue.TabIndex = 4;
            this.tbExpressionBlue.Text = "Y*15";
            this.tbExpressionBlue.TextChanged += new System.EventHandler(this.tbExpressionBlue_TextChanged);
            // 
            // Label4
            // 
            this.Label4.Location = new System.Drawing.Point(17, 84);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(40, 16);
            this.Label4.TabIndex = 9;
            this.Label4.Text = "Blue";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabDynamic
            // 
            this.tabDynamic.Controls.Add(this.btnEvaluate3);
            this.tabDynamic.Controls.Add(this.LogBox3);
            this.tabDynamic.Controls.Add(this.Label5);
            this.tabDynamic.Controls.Add(this.updownA);
            this.tabDynamic.Controls.Add(this.tbExpression3);
            this.tabDynamic.Controls.Add(this.updownB);
            this.tabDynamic.Controls.Add(this.updownC);
            this.tabDynamic.Controls.Add(this.Label6);
            this.tabDynamic.Controls.Add(this.Label7);
            this.tabDynamic.Controls.Add(this.Label12);
            this.tabDynamic.Controls.Add(this.Label8);
            this.tabDynamic.Controls.Add(this.lblResults3);
            this.tabDynamic.Location = new System.Drawing.Point(4, 22);
            this.tabDynamic.Name = "tabDynamic";
            this.tabDynamic.Size = new System.Drawing.Size(510, 474);
            this.tabDynamic.TabIndex = 2;
            this.tabDynamic.Text = "Dynamic Formulas";
            // 
            // btnEvaluate3
            // 
            this.btnEvaluate3.Location = new System.Drawing.Point(324, 9);
            this.btnEvaluate3.Name = "btnEvaluate3";
            this.btnEvaluate3.Size = new System.Drawing.Size(72, 23);
            this.btnEvaluate3.TabIndex = 19;
            this.btnEvaluate3.Text = "Evaluate";
            this.btnEvaluate3.Click += new System.EventHandler(this.btnEvaluate3_Click);
            // 
            // LogBox3
            // 
            this.LogBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogBox3.Location = new System.Drawing.Point(8, 168);
            this.LogBox3.Multiline = true;
            this.LogBox3.Name = "LogBox3";
            this.LogBox3.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.LogBox3.Size = new System.Drawing.Size(489, 295);
            this.LogBox3.TabIndex = 18;
            this.LogBox3.Text = "Notice how the formula is refreshed only when involved variables are modified.\r\n";
            // 
            // Label5
            // 
            this.Label5.Location = new System.Drawing.Point(12, 41);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(88, 20);
            this.Label5.TabIndex = 16;
            this.Label5.Text = "a";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // updownA
            // 
            this.updownA.Location = new System.Drawing.Point(108, 41);
            this.updownA.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updownA.Name = "updownA";
            this.updownA.Size = new System.Drawing.Size(72, 20);
            this.updownA.TabIndex = 11;
            this.updownA.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.updownA.ValueChanged += new System.EventHandler(this.updownA_ValueChanged);
            // 
            // tbExpression3
            // 
            this.tbExpression3.Location = new System.Drawing.Point(108, 9);
            this.tbExpression3.Name = "tbExpression3";
            this.tbExpression3.Size = new System.Drawing.Size(208, 20);
            this.tbExpression3.TabIndex = 8;
            this.tbExpression3.Text = "A+2*B";
            // 
            // updownB
            // 
            this.updownB.Location = new System.Drawing.Point(108, 73);
            this.updownB.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updownB.Name = "updownB";
            this.updownB.Size = new System.Drawing.Size(72, 20);
            this.updownB.TabIndex = 9;
            this.updownB.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.updownB.ValueChanged += new System.EventHandler(this.updownB_ValueChanged);
            // 
            // updownC
            // 
            this.updownC.Location = new System.Drawing.Point(108, 105);
            this.updownC.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updownC.Name = "updownC";
            this.updownC.Size = new System.Drawing.Size(72, 20);
            this.updownC.TabIndex = 10;
            this.updownC.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.updownC.ValueChanged += new System.EventHandler(this.updownC_ValueChanged);
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(12, 73);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(88, 20);
            this.Label6.TabIndex = 17;
            this.Label6.Text = "b";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label7
            // 
            this.Label7.Location = new System.Drawing.Point(12, 105);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(88, 20);
            this.Label7.TabIndex = 13;
            this.Label7.Text = "c";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label12
            // 
            this.Label12.Location = new System.Drawing.Point(12, 9);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(88, 20);
            this.Label12.TabIndex = 12;
            this.Label12.Text = "Formula";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(12, 145);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(88, 20);
            this.Label8.TabIndex = 15;
            this.Label8.Text = "Result";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblResults3
            // 
            this.lblResults3.Location = new System.Drawing.Point(108, 145);
            this.lblResults3.Name = "lblResults3";
            this.lblResults3.Size = new System.Drawing.Size(288, 20);
            this.lblResults3.TabIndex = 14;
            this.lblResults3.Text = "Label5";
            this.lblResults3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            // excelSheet1
            // 
            this.excelSheet1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.excelSheet1.Location = new System.Drawing.Point(0, 0);
            this.excelSheet1.Name = "excelSheet1";
            this.excelSheet1.Size = new System.Drawing.Size(502, 445);
            this.excelSheet1.TabIndex = 1;
            // 
            // excelSheet2
            // 
            this.excelSheet2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.excelSheet2.Location = new System.Drawing.Point(0, 0);
            this.excelSheet2.Name = "excelSheet2";
            this.excelSheet2.Size = new System.Drawing.Size(502, 445);
            this.excelSheet2.TabIndex = 1;
            // 
            // excelSheet3
            // 
            this.excelSheet3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.excelSheet3.Location = new System.Drawing.Point(0, 0);
            this.excelSheet3.Name = "excelSheet3";
            this.excelSheet3.Size = new System.Drawing.Size(502, 445);
            this.excelSheet3.TabIndex = 2;
            // 
            // evaluatorPanel1
            // 
            this.evaluatorPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.evaluatorPanel1.Location = new System.Drawing.Point(0, 0);
            this.evaluatorPanel1.Name = "evaluatorPanel1";
            this.evaluatorPanel1.PanelLanguage = Eval4.DemoCSharp.PanelLanguage.vb;
            this.evaluatorPanel1.Size = new System.Drawing.Size(510, 474);
            this.evaluatorPanel1.TabIndex = 0;
            // 
            // evaluatorPanel2
            // 
            this.evaluatorPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.evaluatorPanel2.Location = new System.Drawing.Point(0, 0);
            this.evaluatorPanel2.Name = "evaluatorPanel2";
            this.evaluatorPanel2.PanelLanguage = Eval4.DemoCSharp.PanelLanguage.csharp;
            this.evaluatorPanel2.Size = new System.Drawing.Size(510, 474);
            this.evaluatorPanel2.TabIndex = 0;
            // 
            // evaluatorPanel3
            // 
            this.evaluatorPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.evaluatorPanel3.Location = new System.Drawing.Point(0, 0);
            this.evaluatorPanel3.Name = "evaluatorPanel3";
            this.evaluatorPanel3.PanelLanguage = Eval4.DemoCSharp.PanelLanguage.matlab;
            this.evaluatorPanel3.Size = new System.Drawing.Size(510, 474);
            this.evaluatorPanel3.TabIndex = 0;
            // 
            // evaluatorPanel5
            // 
            this.evaluatorPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.evaluatorPanel5.Location = new System.Drawing.Point(0, 0);
            this.evaluatorPanel5.Name = "evaluatorPanel5";
            this.evaluatorPanel5.PanelLanguage = Eval4.DemoCSharp.PanelLanguage.experiments;
            this.evaluatorPanel5.Size = new System.Drawing.Size(510, 474);
            this.evaluatorPanel5.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(518, 553);
            this.Controls.Add(this.TabControl1);
            this.Controls.Add(this.panel3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "Form1";
            this.Text = "Expression Evaluator 100% managed .net  (C# Version)";
            this.TabControl1.ResumeLayout(false);
            this.tabExcel.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabSheet1.ResumeLayout(false);
            this.tabSheet2.ResumeLayout(false);
            this.tabSheet3.ResumeLayout(false);
            this.tabVB.ResumeLayout(false);
            this.tabCSharp.ResumeLayout(false);
            this.tabMatlab.ResumeLayout(false);
            this.tabExperiments.ResumeLayout(false);
            this.TabHeavier.ResumeLayout(false);
            this.TabHeavier.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            this.tabDynamic.ResumeLayout(false);
            this.tabDynamic.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updownA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownC)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void tbExpressionBlue_TextChanged(object sender, System.EventArgs e)
        {
            if (mInitializing) return;
            if (cbAuto.Checked)
            {
                btnEvaluate2_Click(sender, e);
            }
        }

        private void cbAuto_CheckedChanged(object sender, System.EventArgs e)
        {
            if (mInitializing) return;
            if (cbAuto.Checked)
            {
                btnEvaluate2_Click(sender, e);
            }
        }

        public static void Main()
        {
            Form1 f = new Form1();
            f.ShowDialog();
        }

        private void btnEvaluate2_Click(object sender, System.EventArgs e)
        {
            Eval4.Core.IHasValue lCodeR = null;
            Eval4.Core.IHasValue lCodeG = null;
            Eval4.Core.IHasValue lCodeB = null;
            try
            {
                ev.AddEnvironmentFunctions(this);
                ev.AddEnvironmentFunctions(new EvalFunctions());
                lCodeR = ev.Parse(tbExpressionRed.Text);
                lCodeG = ev.Parse(tbExpressionGreen.Text);
                lCodeB = ev.Parse(tbExpressionBlue.Text);
            }
            catch (Exception ex)
            {
                Label1.Text = ex.Message;
                return;
            }
            try
            {
                Timer t = new Timer();
                Bitmap bm = (Bitmap)PictureBox1.Image;
                if ((bm == null))
                {
                    bm = new Bitmap(256, 256);
                    PictureBox1.Image = bm;
                }
                double mult = (2
                    * (Math.PI / 256));
                double r = 0;
                double g = 0;
                double b = 0;
                for (int Xi = 0; (Xi <= 255); Xi++)
                {
                    X = ((Xi - 128)
                        * mult);
                    for (int Yi = 0; (Yi <= 255); Yi++)
                    {
                        Y = ((Yi - 128)
                            * mult);
                        try
                        {

                            r = Convert.ToDouble(lCodeR.ObjectValue);
                            g = Convert.ToDouble(lCodeG.ObjectValue);
                            b = Convert.ToDouble(lCodeB.ObjectValue);

                            if (((r < 0)
                                || double.IsNaN(r)))
                            {
                                r = 0;
                            }
                            else if ((r > 1))
                            {
                                r = 1;
                            }
                            if (((g < 0)
                                || double.IsNaN(g)))
                            {
                                g = 0;
                            }
                            else if ((g > 1))
                            {
                                g = 1;
                            }
                            if (((b < 0)
                                || double.IsNaN(b)))
                            {
                                b = 0;
                            }
                            else if ((b > 1))
                            {
                                b = 1;
                            }
                        }
                        catch
                        {
                            //  ignore (same as previous pixel)
                        }
                        bm.SetPixel(Xi, Yi, Color.FromArgb((int)(r * 255), (int)(g * 255), (int)(b * 255)));
                    }
                    if ((Xi & 7) == 7)
                    {
                        PictureBox1.Refresh();
                    }
                }
                Label1.Text = ("196,608 evaluations run in "
                    + (t.ms() + " ms"));
            }
            catch (Exception ex)
            {
                Label1.Text = ex.Message;
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            mInitializing = true;
            switch (ComboBox1.SelectedIndex)
            {
                case 1:
                    tbExpressionRed.Text = "Mod(Round(4*X-Y*2),2)-X";
                    tbExpressionGreen.Text = "Mod(Abs(X+2*Y),0.75)*10+Y/5";
                    tbExpressionBlue.Text = "Round(Sin(Sqrt(X*X+Y*Y))*3/5)+X/3";
                    break;
                case 2:
                    tbExpressionRed.Text = "1-Round(X/Y*0.5)";
                    tbExpressionGreen.Text = "1-Round(Y/X*0.4)";
                    tbExpressionBlue.Text = "Round(Sin(Sqrt(X*X+Y*Y)*10))";
                    break;
                case 3:
                    tbExpressionRed.Text = "Cos(X/2)/2";
                    tbExpressionGreen.Text = "Cos(Y/2)/3";
                    tbExpressionBlue.Text = "Round(Sin(Sqrt(X*X*X+Y*Y)*10))";
                    break;
                case 4:
                    tbExpressionRed.Text = "X*15";
                    tbExpressionGreen.Text = "Cos(X*Y*4900)";
                    tbExpressionBlue.Text = "Y*15";
                    break;
                default:
                    tbExpressionRed.Text = String.Empty;
                    tbExpressionGreen.Text = String.Empty;
                    tbExpressionBlue.Text = String.Empty;
                    break;
            }
            mInitializing = false;
            btnEvaluate2_Click(sender, e);
        }

        //private Core.ValueChangedEventHandler FormulaHandler = null;


        private void btnEvaluate3_Click(object sender, System.EventArgs e)
        {
            try
            {
                // throw new NotImplementedException();
                // if (FormulaHandler != null) mFormula3.ValueChanged -= FormulaHandler;
                if (formula3subscription != null) formula3subscription.Dispose();
                mFormula3 = ev.Parse(tbExpression3.Text);

                formula3subscription = mFormula3.Subscribe(() =>
                {
                    string v = VbEvaluator.ConvertToString(mFormula3.ObjectValue);
                    lblResults3.Text = v;
                    LogBox3.AppendText(System.DateTime.Now.ToLongTimeString() + ": " + v + "\r\n");
                });


                //FormulaHandler = new Eval4.Core.ValueChangedEventHandler(mFormula3_ValueChanged);
                // mFormula3.ValueChanged += FormulaHandler;
                // mFormula3_ValueChanged(null, null);
            }
            catch (Exception ex)
            {
                lblResults3.Text = ex.Message;
            }
        }

        private void updownA_ValueChanged(object sender, System.EventArgs e)
        {
            A.SetValue((double)updownA.Value);
        }

        private void updownB_ValueChanged(object sender, System.EventArgs e)
        {
            B.SetValue((double)updownB.Value);
        }

        private void updownC_ValueChanged(object sender, System.EventArgs e)
        {
            C.SetValue((double)updownC.Value);
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
