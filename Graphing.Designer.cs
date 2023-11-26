namespace Algebra
{
    partial class Graphing
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Txt_Fn = new TextBox();
            label1 = new Label();
            Img_Graph = new PictureBox();
            bindingSource1 = new BindingSource(components);
            Txt_Dx = new Label();
            Checkbox_Derivative = new CheckBox();
            Checkbox_Roots = new CheckBox();
            Checkbox_Extrema = new CheckBox();
            Checkbox_Turnings = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)Img_Graph).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            SuspendLayout();
            // 
            // Txt_Fn
            // 
            Txt_Fn.AcceptsTab = true;
            Txt_Fn.Location = new Point(47, 12);
            Txt_Fn.Name = "Txt_Fn";
            Txt_Fn.Size = new Size(563, 23);
            Txt_Fn.TabIndex = 0;
            Txt_Fn.KeyDown += Txt_Fn_OnKeyDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 15);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 1;
            label1.Text = "f(x) = ";
            // 
            // Img_Graph
            // 
            Img_Graph.Location = new Point(0, 34);
            Img_Graph.Name = "Img_Graph";
            Img_Graph.Size = new Size(1279, 471);
            Img_Graph.TabIndex = 5;
            Img_Graph.TabStop = false;
            // 
            // Txt_Dx
            // 
            Txt_Dx.AutoSize = true;
            Txt_Dx.Location = new Point(731, 16);
            Txt_Dx.Name = "Txt_Dx";
            Txt_Dx.Size = new Size(42, 15);
            Txt_Dx.TabIndex = 6;
            Txt_Dx.Text = "f'(x) = ";
            // 
            // Checkbox_Derivative
            // 
            Checkbox_Derivative.AutoSize = true;
            Checkbox_Derivative.Location = new Point(0, 41);
            Checkbox_Derivative.Name = "Checkbox_Derivative";
            Checkbox_Derivative.Size = new Size(78, 19);
            Checkbox_Derivative.TabIndex = 7;
            Checkbox_Derivative.Text = "Derivative";
            Checkbox_Derivative.UseVisualStyleBackColor = true;
            Checkbox_Derivative.CheckedChanged += Checkbox_Derivative_CheckedChanged;
            // 
            // Checkbox_Roots
            // 
            Checkbox_Roots.AutoSize = true;
            Checkbox_Roots.Location = new Point(0, 66);
            Checkbox_Roots.Name = "Checkbox_Roots";
            Checkbox_Roots.Size = new Size(56, 19);
            Checkbox_Roots.TabIndex = 8;
            Checkbox_Roots.Text = "Roots";
            Checkbox_Roots.UseVisualStyleBackColor = true;
            Checkbox_Roots.CheckedChanged += Checkbox_Roots_CheckedChanged_1;
            // 
            // Checkbox_Extrema
            // 
            Checkbox_Extrema.AutoSize = true;
            Checkbox_Extrema.Location = new Point(0, 91);
            Checkbox_Extrema.Name = "Checkbox_Extrema";
            Checkbox_Extrema.Size = new Size(69, 19);
            Checkbox_Extrema.TabIndex = 9;
            Checkbox_Extrema.Text = "Extrema";
            Checkbox_Extrema.UseVisualStyleBackColor = true;
            Checkbox_Extrema.CheckedChanged += Checkbox_Extrema_CheckedChanged;
            // 
            // Checkbox_Turnings
            // 
            Checkbox_Turnings.AutoSize = true;
            Checkbox_Turnings.Location = new Point(0, 116);
            Checkbox_Turnings.Name = "Checkbox_Turnings";
            Checkbox_Turnings.Size = new Size(72, 19);
            Checkbox_Turnings.TabIndex = 10;
            Checkbox_Turnings.Text = "Turnings";
            Checkbox_Turnings.UseVisualStyleBackColor = true;
            Checkbox_Turnings.CheckedChanged += Checkbox_Turnings_CheckedChanged;
            // 
            // Graphing
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1279, 524);
            Controls.Add(Checkbox_Turnings);
            Controls.Add(Checkbox_Extrema);
            Controls.Add(Checkbox_Roots);
            Controls.Add(Checkbox_Derivative);
            Controls.Add(Txt_Dx);
            Controls.Add(Img_Graph);
            Controls.Add(label1);
            Controls.Add(Txt_Fn);
            Name = "Graphing";
            Text = "Algebra";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)Img_Graph).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox Txt_Fn;
        private Label label1;
        private PictureBox Img_Graph;
        private BindingSource bindingSource1;
        private Label Txt_Dx;
        private CheckBox Checkbox_Derivative;
        private CheckBox Checkbox_Roots;
        private CheckBox Checkbox_Extrema;
        private CheckBox Checkbox_Turnings;
    }
}