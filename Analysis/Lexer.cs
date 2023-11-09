using MathShit.Miscellaneous;

namespace MathShit.Analysis
{
    internal class Lexer
    {
        private readonly string _fn;
        private readonly List<Token> _tokens = new();
        private int _current = 0, _start;
        private string? _error = null;
        public Lexer(string fn) => _fn = fn;
        public string? Error => _error;

        public List<Token> Lex()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                GetToken();
            }


            _tokens.Add(new(TokenKind.EOF, "", Span));
            return _tokens;
        }

        private void GetToken()
        {
            char c = Current;
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
                case '(':
                    kind = TokenKind.LParen;
                    break;
                case ')':
                    kind = TokenKind.RParen;
                    break;
                case ' ':
                case '\t':
                case '\n':
                case '\r':
                    Advance();
                    return;
                default:
                    if (char.IsDigit(c))
                    {
                        GetNumber();
                        Advance();
                        return;
                    }
                    else
                        _error = $"Unknown character '{c}'.";
                    break;
            }

            Advance();
            _tokens.Add(new(kind, lexeme, Span));
        }

        private void GetNumber()
        {
            string lexeme = "";
            while (!IsAtEnd() && char.IsDigit(Current))
            {
                lexeme += Current;
                Advance();
            }

            _tokens.Add(new(TokenKind.Number, lexeme, Span));
        }

        private bool IsAtEnd() => _current >= _fn.Length;
        private char Current => IsAtEnd() ? '\0' : _fn[_current];
        private void Advance() => ++_current;
        private TextSpan Span => new(_start, _current - _start);
    }
}