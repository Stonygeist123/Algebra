﻿namespace MathShit
{
    partial class Form1
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
            Btn_Graph = new Button();
            Img_Graph = new PictureBox();
            bindingSource1 = new BindingSource(components);
            ((System.ComponentModel.ISupportInitialize)Img_Graph).BeginInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).BeginInit();
            SuspendLayout();
            // 
            // Txt_Fn
            // 
            Txt_Fn.AcceptsTab = true;
            Txt_Fn.Location = new Point(47, 12);
            Txt_Fn.Name = "Txt_Fn";
            Txt_Fn.Size = new Size(100, 23);
            Txt_Fn.TabIndex = 0;
            Txt_Fn.TextChanged += Txt_Fn_TextChanged;
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
            // Btn_Graph
            // 
            Btn_Graph.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            Btn_Graph.Location = new Point(153, 12);
            Btn_Graph.Name = "Btn_Graph";
            Btn_Graph.Size = new Size(75, 23);
            Btn_Graph.TabIndex = 2;
            Btn_Graph.Text = "Graph";
            Btn_Graph.UseVisualStyleBackColor = true;
            Btn_Graph.Click += Btn_Graph_Click;
            // 
            // Img_Graph
            // 
            Img_Graph.Location = new Point(12, 41);
            Img_Graph.Name = "Img_Graph";
            Img_Graph.Size = new Size(1255, 471);
            Img_Graph.TabIndex = 5;
            Img_Graph.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1279, 524);
            Controls.Add(Img_Graph);
            Controls.Add(Btn_Graph);
            Controls.Add(label1);
            Controls.Add(Txt_Fn);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)Img_Graph).EndInit();
            ((System.ComponentModel.ISupportInitialize)bindingSource1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox Txt_Fn;
        private Label label1;
        private Button Btn_Graph;
        private PictureBox Img_Graph;
        private BindingSource bindingSource1;
    }
}