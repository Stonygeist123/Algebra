using MathShit.Miscellaneous;

namespace MathShit.Syntax.Lexer
{
    internal class Lexer
    {
        private readonly string _fn;
        private readonly List<Token> _tokens = new();
        private int _current = 0, _start = 0;
        private readonly DiagnosticBag _diagnostics = new();
        public Diagnostic[] Diagnostics => _diagnostics.ToArray();
        public Lexer(string fn) => _fn = fn;
        public Token[] Lex()
        {
            while (!IsAtEnd())
            {
                GetToken();
                _start = _current;
            }

            _tokens.Add(new(TokenKind.EOF, "", Span));
            return _tokens.ToArray();
        }

        private void GetToken()
        {
            char c = Current;
            Advance();
            TokenKind kind = TokenKind.Bad;
            string lexeme = c.ToString();
            switch (c)
            {
                case '+':
                    kind = TokenKind.Plus;
                    break;
                case '-':
                    kind = TokenKind.Minus;
                    break;
                case '*':
                    kind = TokenKind.Star;
                    break;
                case '/':
                    kind = TokenKind.Slash;
                    break;
                case '^':
                    kind = TokenKind.Power;
                    break;
                case '(':
                    kind = TokenKind.LParen;
                    break;
                case ')':
                    kind = TokenKind.RParen;
                    break;
                case '|':
                    kind = TokenKind.Pipe;
                    break;
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    return;
                default:
                    if (char.IsDigit(c))
                    {
                        GetNumber(c);
                        return;
                    }
                    if (char.IsLetter(c))
                    {
                        GetAlpha(c);
                        return;
                    }
                    else
                        _diagnostics.Add($"Unknown character '{c}'.", Span);
                    break;
            }

            _tokens.Add(new(kind, lexeme, Span));
        }

        private void GetNumber(char c)
        {
            string lexeme = c.ToString();
            while (!IsAtEnd() && char.IsDigit(Current))
            {
                lexeme += Current;
                Advance();
            }

            if (Current == '.')
            {
                lexeme += Current;
                Advance();
                if (!char.IsDigit(Current))
                    _diagnostics.Add($"Malformed number.", Span);

                while (!IsAtEnd() && char.IsDigit(Current))
                {
                    lexeme += Current;
                    Advance();
                }
            }

            _tokens.Add(new(TokenKind.Number, lexeme, Span));
        }

        private void GetAlpha(char c)
        {
            string lexeme = c.ToString();
            while (!IsAtEnd() && char.IsLetter(Current))
            {
                lexeme += Current;
                Advance();
            }

            _tokens.Add(new(TokenKind.Name, lexeme, Span));
        }

        private bool IsAtEnd() => _current >= _fn.Length;
        private char Current => IsAtEnd() ? '\0' : _fn[_current];
        private void Advance() => ++_current;
        private TextSpan Span => new(_start, _current - _start);
    }
}