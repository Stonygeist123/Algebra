namespace Algebra
{
    partial class Calculator
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Txt_Calc = new TextBox();
            Btn_Calc = new Button();
            Txt_Output = new Label();
            SuspendLayout();
            // 
            // Txt_Calc
            // 
            Txt_Calc.AcceptsTab = true;
            Txt_Calc.Location = new Point(12, 176);
            Txt_Calc.Multiline = true;
            Txt_Calc.Name = "Txt_Calc";
            Txt_Calc.Size = new Size(1255, 175);
            Txt_Calc.TabIndex = 1;
            // 
            // Btn_Calc
            // 
            Btn_Calc.Location = new Point(12, 391);
            Btn_Calc.Name = "Btn_Calc";
            Btn_Calc.Size = new Size(262, 93);
            Btn_Calc.TabIndex = 2;
            Btn_Calc.Text = "Calculate";
            Btn_Calc.UseVisualStyleBackColor = true;
            Btn_Calc.Click += Btn_Calc_Click;
            // 
            // Txt_Output
            // 
            Txt_Output.AutoSize = true;
            Txt_Output.Location = new Point(407, 41);
            Txt_Output.MaximumSize = new Size(400, 100);
            Txt_Output.MinimumSize = new Size(400, 100);
            Txt_Output.Name = "Txt_Output";
            Txt_Output.Size = new Size(400, 100);
            Txt_Output.TabIndex = 3;
            Txt_Output.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // Calculator
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1279, 524);
            Controls.Add(Txt_Output);
            Controls.Add(Btn_Calc);
            Controls.Add(Txt_Calc);
            Name = "Calculator";
            Text = "Algebra - Calculator";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox Txt_Calc;
        private Button Btn_Calc;
        private Label Txt_Output;
    }
}