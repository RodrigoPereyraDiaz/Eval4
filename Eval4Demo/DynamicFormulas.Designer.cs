namespace Eval4.Demo
{
    partial class DynamicFormulas
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
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
            ((System.ComponentModel.ISupportInitialize)(this.updownA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownC)).BeginInit();
            this.SuspendLayout();
            // 
            // btnEvaluate3
            // 
            this.btnEvaluate3.Location = new System.Drawing.Point(319, 6);
            this.btnEvaluate3.Name = "btnEvaluate3";
            this.btnEvaluate3.Size = new System.Drawing.Size(72, 23);
            this.btnEvaluate3.TabIndex = 31;
            this.btnEvaluate3.Text = "Evaluate";
            this.btnEvaluate3.Click += new System.EventHandler(this.btnEvaluate3_Click);
            // 
            // LogBox3
            // 
            this.LogBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogBox3.Location = new System.Drawing.Point(3, 165);
            this.LogBox3.Multiline = true;
            this.LogBox3.Name = "LogBox3";
            this.LogBox3.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.LogBox3.Size = new System.Drawing.Size(443, 236);
            this.LogBox3.TabIndex = 30;
            this.LogBox3.Text = "Notice how the formula is refreshed only when involved variables are modified.\r\n";
            // 
            // Label5
            // 
            this.Label5.Location = new System.Drawing.Point(7, 38);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(88, 20);
            this.Label5.TabIndex = 28;
            this.Label5.Text = "a";
            this.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // updownA
            // 
            this.updownA.Location = new System.Drawing.Point(103, 38);
            this.updownA.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updownA.Name = "updownA";
            this.updownA.Size = new System.Drawing.Size(72, 20);
            this.updownA.TabIndex = 23;
            this.updownA.Value = new decimal(new int[] {
            23,
            0,
            0,
            0});
            this.updownA.ValueChanged += new System.EventHandler(this.updownA_ValueChanged);
            // 
            // tbExpression3
            // 
            this.tbExpression3.Location = new System.Drawing.Point(103, 6);
            this.tbExpression3.Name = "tbExpression3";
            this.tbExpression3.Size = new System.Drawing.Size(208, 20);
            this.tbExpression3.TabIndex = 20;
            this.tbExpression3.Text = "A+2*B";
            // 
            // updownB
            // 
            this.updownB.Location = new System.Drawing.Point(103, 70);
            this.updownB.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updownB.Name = "updownB";
            this.updownB.Size = new System.Drawing.Size(72, 20);
            this.updownB.TabIndex = 21;
            this.updownB.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.updownB.ValueChanged += new System.EventHandler(this.updownA_ValueChanged);
            // 
            // updownC
            // 
            this.updownC.Location = new System.Drawing.Point(103, 102);
            this.updownC.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updownC.Name = "updownC";
            this.updownC.Size = new System.Drawing.Size(72, 20);
            this.updownC.TabIndex = 22;
            this.updownC.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.updownC.ValueChanged += new System.EventHandler(this.updownA_ValueChanged);
            // 
            // Label6
            // 
            this.Label6.Location = new System.Drawing.Point(7, 70);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(88, 20);
            this.Label6.TabIndex = 29;
            this.Label6.Text = "b";
            this.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label7
            // 
            this.Label7.Location = new System.Drawing.Point(7, 102);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(88, 20);
            this.Label7.TabIndex = 25;
            this.Label7.Text = "c";
            this.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label12
            // 
            this.Label12.Location = new System.Drawing.Point(7, 6);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(88, 20);
            this.Label12.TabIndex = 24;
            this.Label12.Text = "Formula";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label8
            // 
            this.Label8.Location = new System.Drawing.Point(7, 142);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(88, 20);
            this.Label8.TabIndex = 27;
            this.Label8.Text = "Result";
            this.Label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblResults3
            // 
            this.lblResults3.Location = new System.Drawing.Point(103, 142);
            this.lblResults3.Name = "lblResults3";
            this.lblResults3.Size = new System.Drawing.Size(288, 20);
            this.lblResults3.TabIndex = 26;
            this.lblResults3.Text = "Label5";
            this.lblResults3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // DynamicFormulas
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnEvaluate3);
            this.Controls.Add(this.LogBox3);
            this.Controls.Add(this.Label5);
            this.Controls.Add(this.updownA);
            this.Controls.Add(this.tbExpression3);
            this.Controls.Add(this.updownB);
            this.Controls.Add(this.updownC);
            this.Controls.Add(this.Label6);
            this.Controls.Add(this.Label7);
            this.Controls.Add(this.Label12);
            this.Controls.Add(this.Label8);
            this.Controls.Add(this.lblResults3);
            this.Name = "DynamicFormulas";
            this.Size = new System.Drawing.Size(449, 401);
            ((System.ComponentModel.ISupportInitialize)(this.updownA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updownC)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

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
        internal System.Windows.Forms.Label lblResults3;
    }
}
