using MathShit.Analysis;

namespace MathShit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Btn_Graph_Click(object sender, EventArgs e)
        {
            Lexer lexer = new(Txt_Fn.Text);
            List<Token> tokens = lexer.Lex();
            Txt_Res.Text = "";
            if (lexer.Error is null)
                foreach (Token t in tokens)
                    Txt_Res.Text += $"{t}";
            else
                Txt_Res.Text += lexer.Error;
        }
    }
}