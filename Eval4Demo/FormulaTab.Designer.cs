namespace Eval4.Demo
{
    partial class FormulaTab
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
            this.components = new System.ComponentModel.Container();
            this.cbSamples = new System.Windows.Forms.ComboBox();
            this.btnEvaluate = new System.Windows.Forms.Button();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // cbSamples
            // 
            this.cbSamples.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSamples.Location = new System.Drawing.Point(3, 3);
            this.cbSamples.Name = "cbSamples";
            this.cbSamples.Size = new System.Drawing.Size(325, 21);
            this.cbSamples.TabIndex = 7;
            this.cbSamples.Text = "<enter an expression or select a sample>";
            this.cbSamples.SelectedIndexChanged += new System.EventHandler(this.cbSamples_SelectedIndexChanged);
            this.cbSamples.TextChanged += new System.EventHandler(this.cbSamples_TextChanged);
            // 
            // btnEvaluate
            // 
            this.btnEvaluate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEvaluate.Location = new System.Drawing.Point(339, 3);
            this.btnEvaluate.Name = "btnEvaluate";
            this.btnEvaluate.Size = new System.Drawing.Size(72, 23);
            this.btnEvaluate.TabIndex = 6;
            this.btnEvaluate.Text = "Evaluate";
            this.btnEvaluate.Click += new System.EventHandler(this.btnEvaluate_Click);
            // 
            // TextBox2
            // 
            this.TextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox2.Location = new System.Drawing.Point(3, 43);
            this.TextBox2.Multiline = true;
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox2.Size = new System.Drawing.Size(408, 345);
            this.TextBox2.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "label1";
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // FormulaTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbSamples);
            this.Controls.Add(this.btnEvaluate);
            this.Controls.Add(this.TextBox2);
            this.Name = "FormulaTab";
            this.Size = new System.Drawing.Size(417, 394);
            this.Load += new System.EventHandler(this.EvaluatorPanel_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ComboBox cbSamples;
        internal System.Windows.Forms.Button btnEvaluate;
        internal System.Windows.Forms.TextBox TextBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}
