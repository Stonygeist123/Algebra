using Algebra.Miscellaneous;
using Algebra.Syntax.Lexer;
using Algebra.Syntax.Parser;

namespace Algebra
{
    public partial class Calculator : Form
    {
        private Expr? _expr = null;
        private bool _hasError = false;
        private Diagnostic[] _diagnostics = Array.Empty<Diagnostic>();
        public Calculator()
        {
            InitializeComponent();
        }

        private void Analyse()
        {
            Lexer lexer = new(Txt_Calc.Text);
            Token[] tokens = lexer.Lex();
            if (!(_hasError = lexer.Diagnostics.Any()))
            {
                Parser parser = new(tokens, false);
                _expr = parser.Parse().Simplify();
                _hasError = parser.Diagnostics.Any();
                if (_hasError)
                    _diagnostics = parser.Diagnostics;
            }
        }

        private void Btn_Calc_Click(object sender, EventArgs e)
        {
            Analyse();
            if (_hasError)
            {
                Diagnostic d = _diagnostics.Last();
                Txt_Output.Text = $"[{d.Span.Start + 1}:{_diagnostics.Last().Span.End + 1}] {_diagnostics.Last().Message}";
            }
            else
                Txt_Output.Text = _expr?.EvaluateD().ToString() ?? string.Empty;
        }
    }
}
