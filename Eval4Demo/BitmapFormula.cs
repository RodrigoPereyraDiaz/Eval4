using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace Eval4.Demo
{
    public partial class BitmapFormula : UserControl
    {
        private MathEvaluator ev;
        private Bitmap bm;
        private byte[] rgbValues;
                
        private bool mInitializing;

        public BitmapFormula()
        {
            InitializeComponent();
            mInitializing = true;
            ev = new MathEvaluator();
            ev.AddEnvironmentFunctions(this);
            ev.SetVariable("EvalFunctions", new EvalFunctions());
            mInitializing = false;
        }

        private void tbExpressionRed_TextChanged(object sender, EventArgs e)
        {
            if (mInitializing) return;
            btnEvaluate2_Click(sender, e);
        }

        private void btnEvaluate2_Click(object sender, EventArgs e)
        {
            Eval4.Core.IParsedExpr lCodeR = null;
            Eval4.Core.IParsedExpr lCodeG = null;
            Eval4.Core.IParsedExpr lCodeB = null;
            ev.AddEnvironmentFunctions(typeof(Math));
            ev.AddEnvironmentFunctions(new EvalFunctions());
            var vX = ev.SetVariable("X", 0.0);
            var vY = ev.SetVariable("Y", 0.0);
            var vZ = ev.SetVariable("Z", ((double)trackBar1.Value) / trackBar1.Maximum);
            BitmapData bmpData = null;

            try
            {
                lCodeR = ev.Parse(tbExpressionRed.Text);
                errorProvider1.SetError(tbExpressionRed, lCodeR.Error);
                lCodeG = ev.Parse(tbExpressionGreen.Text);
                errorProvider1.SetError(tbExpressionGreen, lCodeG.Error);
                lCodeB = ev.Parse(tbExpressionBlue.Text);
                errorProvider1.SetError(tbExpressionBlue, lCodeB.Error);
                
                PictureBox1.Image = null;
                //PictureBox1.Refresh();
                //Bitmap bm = (Bitmap)PictureBox1.Image;
                if ((bm == null))
                {
                    bm = new Bitmap(256, 256, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                    PictureBox1.Image = bm;
                }
                double mult = (2
                    * (System.Math.PI / 256));
                //double r = 0;
                //double g = 0;
                //double b = 0;
                bmpData = bm.LockBits(new Rectangle(0, 0, 256, 256), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                IntPtr ptr = bmpData.Scan0;
                // Get the address of the first line.


                int bytes = Math.Abs(bmpData.Stride) * bm.Height;
                if (rgbValues == null)
                {
                    rgbValues = new byte[bytes];
                }

                int rgbValuesIndex = 0;
                Stopwatch sw = Stopwatch.StartNew();

                for (int Xi = 0; Xi <= 255; Xi++)
                {
                    vX.SetValue((Xi - 128) * mult);
                    for (int Yi = 0; Yi <= 255; Yi++)
                    {
                        vY.SetValue((Yi - 128) * mult);

                        rgbValues[rgbValuesIndex++] = ZeroTo255(lCodeR.ObjectValue);
                        rgbValues[rgbValuesIndex++] = ZeroTo255(lCodeG.ObjectValue);
                        rgbValues[rgbValuesIndex++] = ZeroTo255(lCodeB.ObjectValue);
                    }
                }

                Label1.Text = ("196,608 evaluations run in " + (sw.ElapsedMilliseconds + " ms"));

                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);


            }
            catch (Exception ex)
            {
                Label1.Text = ex.Message;
            }
            finally
            {
                if (bmpData != null) bm.UnlockBits(bmpData);
                if (lCodeR != null) lCodeR.Dispose();
                if (lCodeG != null) lCodeG.Dispose();
                if (lCodeB != null) lCodeB.Dispose();
                PictureBox1.Image = bm;
                using (var gr = PictureBox1.CreateGraphics())
                {
                    gr.DrawImageUnscaled(bm, 0, 0);
                }
            }
        }

        private static byte ZeroTo255(object o)
        {
            if (o is double)
            {
                // I want Sin to go from color 0 to 255
                // -1 = 0
                // 1 = 255
                double r = ((double)o);
                return (byte)(127.5 + r * 127.5);
            }
            else return 0;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            mInitializing = true;
            switch (ComboBox1.SelectedIndex)
            {
                case 0:
                    tbExpressionRed.Text = "X*15";
                    tbExpressionGreen.Text = "Cos(X*Y*4900)";
                    tbExpressionBlue.Text = "Y*15";
                    break;
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
                default:
                    break;
            }
            mInitializing = false;
            btnEvaluate2_Click(sender, e);
        }

        private void BitmapFormula_Load(object sender, EventArgs e)
        {
            btnEvaluate2_Click(null, null);
            ComboBox1.SelectedIndex = 0;
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            if (mInitializing) return;
            btnEvaluate2_Click(null, null);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            btnEvaluate2_Click(null, null);
        }
    }
}
