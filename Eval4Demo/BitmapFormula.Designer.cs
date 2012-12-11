namespace Eval4.Demo
{
    partial class BitmapFormula
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
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(9, 145);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(408, 16);
            this.Label1.TabIndex = 17;
            this.Label1.Text = "Label1";
            // 
            // PictureBox1
            // 
            this.PictureBox1.Location = new System.Drawing.Point(51, 164);
            this.PictureBox1.Name = "PictureBox1";
            this.PictureBox1.Size = new System.Drawing.Size(256, 256);
            this.PictureBox1.TabIndex = 16;
            this.PictureBox1.TabStop = false;
            // 
            // tbExpressionRed
            // 
            this.tbExpressionRed.Location = new System.Drawing.Point(51, 27);
            this.tbExpressionRed.Name = "tbExpressionRed";
            this.tbExpressionRed.Size = new System.Drawing.Size(272, 20);
            this.tbExpressionRed.TabIndex = 11;
            this.tbExpressionRed.Text = "X*15";
            this.tbExpressionRed.TextChanged += new System.EventHandler(this.tbExpressionRed_TextChanged);
            // 
            // ComboBox1
            // 
            this.ComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ComboBox1.Items.AddRange(new object[] {
            "Sample1",
            "Sample2",
            "Sample3",
            "Sample4"});
            this.ComboBox1.Location = new System.Drawing.Point(3, 3);
            this.ComboBox1.Name = "ComboBox1";
            this.ComboBox1.Size = new System.Drawing.Size(408, 21);
            this.ComboBox1.TabIndex = 15;
            this.ComboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // btnEvaluate2
            // 
            this.btnEvaluate2.Location = new System.Drawing.Point(339, 27);
            this.btnEvaluate2.Name = "btnEvaluate2";
            this.btnEvaluate2.Size = new System.Drawing.Size(72, 23);
            this.btnEvaluate2.TabIndex = 14;
            this.btnEvaluate2.Text = "Evaluate";
            this.btnEvaluate2.Click += new System.EventHandler(this.btnEvaluate2_Click);
            // 
            // tbExpressionGreen
            // 
            this.tbExpressionGreen.Location = new System.Drawing.Point(51, 51);
            this.tbExpressionGreen.Name = "tbExpressionGreen";
            this.tbExpressionGreen.Size = new System.Drawing.Size(272, 20);
            this.tbExpressionGreen.TabIndex = 12;
            this.tbExpressionGreen.Text = "Cos(X*Y*4900)";
            this.tbExpressionGreen.TextChanged += new System.EventHandler(this.tbExpressionRed_TextChanged);
            // 
            // Label2
            // 
            this.Label2.Location = new System.Drawing.Point(3, 30);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(40, 16);
            this.Label2.TabIndex = 18;
            this.Label2.Text = "Red";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Label3
            // 
            this.Label3.Location = new System.Drawing.Point(3, 54);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(40, 16);
            this.Label3.TabIndex = 19;
            this.Label3.Text = "Green";
            this.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbExpressionBlue
            // 
            this.tbExpressionBlue.Location = new System.Drawing.Point(51, 75);
            this.tbExpressionBlue.Name = "tbExpressionBlue";
            this.tbExpressionBlue.Size = new System.Drawing.Size(272, 20);
            this.tbExpressionBlue.TabIndex = 13;
            this.tbExpressionBlue.Text = "Y*15";
            this.tbExpressionBlue.TextChanged += new System.EventHandler(this.tbExpressionRed_TextChanged);
            // 
            // Label4
            // 
            this.Label4.Location = new System.Drawing.Point(3, 78);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(40, 16);
            this.Label4.TabIndex = 20;
            this.Label4.Text = "Blue";
            this.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // trackBar1
            // 
            this.trackBar1.LargeChange = 50;
            this.trackBar1.Location = new System.Drawing.Point(51, 101);
            this.trackBar1.Maximum = 1000;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(272, 45);
            this.trackBar1.TabIndex = 22;
            this.trackBar1.TickFrequency = 50;
            this.trackBar1.ValueChanged += new System.EventHandler(this.trackBar1_ValueChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(3, 101);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 16);
            this.label5.TabIndex = 23;
            this.label5.Text = "Z";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(339, 56);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(80, 17);
            this.checkBox1.TabIndex = 24;
            this.checkBox1.Text = "checkBox1";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // timer1
            // 
            this.timer1.Interval = 40;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // BitmapFormula
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.PictureBox1);
            this.Controls.Add(this.tbExpressionRed);
            this.Controls.Add(this.ComboBox1);
            this.Controls.Add(this.btnEvaluate2);
            this.Controls.Add(this.tbExpressionGreen);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.tbExpressionBlue);
            this.Controls.Add(this.Label4);
            this.Name = "BitmapFormula";
            this.Size = new System.Drawing.Size(417, 426);
            this.Load += new System.EventHandler(this.BitmapFormula_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.PictureBox PictureBox1;
        internal System.Windows.Forms.TextBox tbExpressionRed;
        internal System.Windows.Forms.ComboBox ComboBox1;
        internal System.Windows.Forms.Button btnEvaluate2;
        internal System.Windows.Forms.TextBox tbExpressionGreen;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.TextBox tbExpressionBlue;
        internal System.Windows.Forms.Label Label4;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        internal System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Timer timer1;
    }
}
