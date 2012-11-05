namespace Eval4.DemoCSharp
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
            this.cbSamples = new System.Windows.Forms.ComboBox();
            this.btnEvaluate = new System.Windows.Forms.Button();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cbSamples
            // 
            this.cbSamples.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSamples.Items.AddRange(new object[] {
            "123+345",
            "(2+3)*(4-2)",
            "(1+1)==2?\"T\":\"F\"",
            "Now",
            "Round(Now - Date(2006,1,1))+\" days since new year\"",
            "aNumber*5",
            "arr[1]+arr[2]",
            "theForm.Controls[0].Name",
            "theForm.Left"});
            this.cbSamples.Location = new System.Drawing.Point(3, 3);
            this.cbSamples.Name = "cbSamples";
            this.cbSamples.Size = new System.Drawing.Size(325, 21);
            this.cbSamples.TabIndex = 7;
            this.cbSamples.Text = "<enter an expression or select a sample>";
            // 
            // btnEvaluate
            // 
            this.btnEvaluate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEvaluate.Location = new System.Drawing.Point(339, 3);
            this.btnEvaluate.Name = "btnEvaluate";
            this.btnEvaluate.Size = new System.Drawing.Size(72, 23);
            this.btnEvaluate.TabIndex = 6;
            this.btnEvaluate.Text = "Evaluate";
            // 
            // TextBox2
            // 
            this.TextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBox2.Location = new System.Drawing.Point(3, 32);
            this.TextBox2.Multiline = true;
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.TextBox2.Size = new System.Drawing.Size(408, 356);
            this.TextBox2.TabIndex = 5;
            // 
            // EvaluatorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbSamples);
            this.Controls.Add(this.btnEvaluate);
            this.Controls.Add(this.TextBox2);
            this.Name = "EvaluatorPanel";
            this.Size = new System.Drawing.Size(417, 394);
            this.Load += new System.EventHandler(this.EvaluatorPanel_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.ComboBox cbSamples;
        internal System.Windows.Forms.Button btnEvaluate;
        internal System.Windows.Forms.TextBox TextBox2;
    }
}
