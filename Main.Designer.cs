namespace Algebra
{
    partial class Main
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
            Btn_Graph = new Button();
            Btn_Calc = new Button();
            SuspendLayout();
            // 
            // Btn_Graph
            // 
            Btn_Graph.BackColor = Color.FromArgb(0, 0, 192);
            Btn_Graph.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
            Btn_Graph.Location = new Point(50, 200);
            Btn_Graph.Name = "Btn_Graph";
            Btn_Graph.Size = new Size(445, 122);
            Btn_Graph.TabIndex = 0;
            Btn_Graph.Text = "Graphing";
            Btn_Graph.UseVisualStyleBackColor = false;
            Btn_Graph.Click += Btn_Graph_Click;
            // 
            // Btn_Calc
            // 
            Btn_Calc.BackColor = Color.FromArgb(0, 0, 192);
            Btn_Calc.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point);
            Btn_Calc.Location = new Point(704, 200);
            Btn_Calc.Name = "Btn_Calc";
            Btn_Calc.Size = new Size(445, 122);
            Btn_Calc.TabIndex = 1;
            Btn_Calc.Text = "Calculator";
            Btn_Calc.UseVisualStyleBackColor = false;
            Btn_Calc.Click += Btn_Calc_Click;
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1279, 524);
            Controls.Add(Btn_Calc);
            Controls.Add(Btn_Graph);
            Name = "Main";
            Text = "Algebra";
            ResumeLayout(false);
        }

        #endregion

        private Button Btn_Graph;
        private Button Btn_Calc;
    }
}