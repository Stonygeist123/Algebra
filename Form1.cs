using MathShit.Analysis;
using MathShit.Miscellaneous;

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
            Txt_Res.ForeColor = Color.Black;
            if (lexer.Diagnostics.Any())
            {
                List<string> lines = new();
                foreach (Diagnostic d in lexer.Diagnostics)
                {
                    Txt_Res.ForeColor = Color.DarkRed;
                    lines.Add(d.ToString());
                    lines.Add("");
                }

                Txt_Res.Lines = lines.ToArray();
            }
            else
            {
                List<string> lines = new();
                foreach (Token t in tokens)
                {
                    lines.Add(t.ToString());
                    lines.Add("");
                }

                Txt_Res.Lines = lines.ToArray();
            }
        }
    }
}