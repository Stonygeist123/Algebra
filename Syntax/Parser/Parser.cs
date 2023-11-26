using Algebra.Miscellaneous;
using Algebra.Syntax.Lexer;

namespace Algebra.Syntax.Parser
{
    internal class Parser
    {
        private readonly Token[] _tokens;
        private int _current = 0;
        private readonly List<Diagnostic> _diagnostics = new();
        private readonly bool _graph;
        private readonly List<string> _symbols = BuiltIns.Constants.Select(kv => kv.Key).ToList();
        public Diagnostic[] Diagnostics => _diagnostics.ToArray();
        public Parser(Token[] tokens, bool graph = true)
        {
            _tokens = tokens.ToArray();
            _graph = graph;
        }

        public Expr Parse() => ParseExpr();
        private Expr ParseExpr(int parentPrecedence = 0)
        {
            Token t = Current;
            Advance();
            switch (t.Kind)
            {
                case TokenKind.Number:
                    return CheckExtension(new LiteralExpr(t), parentPrecedence);
                case TokenKind.Name:
                    if (!_symbols.Contains(t.Lexeme) && !BuiltIns.Fns.ContainsKey(t.Lexeme) && (!_graph || _graph && t.Lexeme != "x"))
                        _diagnostics.Add(new($"Variables are not supported yet.", t.Span));
                    if (Current.Kind == TokenKind.LParen)
                    {
                        Advance();
                        return CheckExtension(GetFunction(t.Lexeme), parentPrecedence);
                    }
                    else
                        return CheckExtension(new NameExpr(t.Lexeme), parentPrecedence);
                case TokenKind.LParen:
                    return CheckExtension(GetGrouping(), parentPrecedence);
                case TokenKind.Pipe:
                    return CheckExtension(GetPipe(), parentPrecedence);
                case TokenKind.Minus:
                case TokenKind.Bang:
                    return CheckExtension(GetUnary(t), parentPrecedence);
                case TokenKind.Sigma:
                    return CheckExtension(GetSigma(), parentPrecedence);
                default:
                    _diagnostics.Add(new($"Unknown expression.", t.Span));
                    return new ErrorExpr();
            };
        }

        private Expr GetFunction(string lexeme)
        {
            Expr arg = ParseExpr();
            Token rParen = Current;
            if (rParen.Kind != TokenKind.RParen)
            {
                _diagnostics.Add(new($"Expected ')'.", rParen.Span));
                return new ErrorExpr();
            }

            Advance();
            return CheckExtension(new FunctionExpr(lexeme, arg));
        }

        private Expr GetSigma()
        {
            if (Current.Kind != TokenKind.LParen)
            {
                _diagnostics.Add(new($"Expected \"(\".", Current.Span));
                return new ErrorExpr();
            }

            Advance();
            Token id = Current;
            if (_symbols.Contains(id.Lexeme))
            {
                _diagnostics.Add(new($"\"{id.Lexeme}\" was already defined.", id.Span));
                return new ErrorExpr();
            }

            Advance();
            if (Current.Kind != TokenKind.Eq)
            {
                _diagnostics.Add(new($"Expected \"=\".", Current.Span));
                return new ErrorExpr();
            }

            Advance();
            Expr start = ParseExpr();
            if (Current.Kind != TokenKind.Comma)
            {
                _diagnostics.Add(new($"Expected \",\".", Current.Span));
                return new ErrorExpr();
            }

            Advance();
            Expr end = ParseExpr();
            if (Current.Kind != TokenKind.Comma)
            {
                _diagnostics.Add(new($"Expected \",\".", Current.Span));
                return new ErrorExpr();
            }

            Advance();
            _symbols.Add(id.Lexeme);
            Expr expr = ParseExpr();
            _symbols.RemoveAt(_symbols.Count - 1);
            Token rParen = Current;
            Advance();
            if (rParen.Kind == TokenKind.RParen)
                return new SigmaExpr(id.Lexeme, start, end, expr);

            _diagnostics.Add(new($"Expected \")\".", rParen.Span));
            return new ErrorExpr();
        }

        private Expr GetGrouping()
        {
            Expr expr = ParseExpr();
            Token rParen = Current;
            Advance();
            if (rParen.Kind == TokenKind.RParen)
                return new GroupingExpr(expr);

            _diagnostics.Add(new($"Expected \")\".", rParen.Span));
            return new ErrorExpr();
        }

        private Expr GetPipe()
        {
            Expr expr = ParseExpr();
            Token rPipe = Current;
            Advance();
            if (rPipe.Kind == TokenKind.Pipe)
                return new AbsExpr(expr);

            _diagnostics.Add(new($"Expected \"|\".", rPipe.Span));
            return new ErrorExpr();
        }

        private Expr GetUnary(Token op)
        {
            Expr expr = ParseExpr();
            return new UnaryExpr(op.Kind, expr);
        }

        private Expr CheckExtension(Expr expr, int parentPrecedence = 0)
        {
            if (IsAtEnd())
                return expr;

            Token t = Current;
            if (OperaterPrecedence(t.Kind) > 0)
            {
                int precedence = OperaterPrecedence(t.Kind);
                while (precedence > 0)
                {
                    if (precedence < parentPrecedence)
                        break;

                    Advance();
                    Expr right = ParseExpr(precedence);
                    expr = new BinaryExpr(expr, t.Kind, right);
                    if (IsAtEnd())
                        return expr;

                    t = Current;
                    precedence = OperaterPrecedence(t.Kind);
                }

                return expr;
            }
            else if (t.Kind == TokenKind.LParen)
            {
                Advance();
                if (expr is NameExpr n && BuiltIns.Fns.Any(fn => fn.Key == n.Name))
                {
                    Expr arg = ParseExpr();
                    Token rParen = Current;
                    if (rParen.Kind != TokenKind.RParen)
                    {
                        _diagnostics.Add(new($"Expected ')'.", t.Span));
                        return new ErrorExpr();
                    }

                    expr = CheckExtension(new FunctionExpr(n.Name, arg));
                }
                else
                {
                    Expr right = ParseExpr();
                    Token rParen = Current;
                    if (Current.Kind != TokenKind.RParen)
                    {
                        _diagnostics.Add(new($"Expected ')'.", t.Span));
                        return new ErrorExpr();
                    }

                    Advance();
                    right = CheckExtension(new GroupingExpr(right), OperaterPrecedence(TokenKind.Power));
                    expr = new BinaryExpr(expr, TokenKind.Star, right);
                }
            }
            else if (t.Kind == TokenKind.Name)
            {
                Advance();
                if ((!_graph || _graph && t.Lexeme != "x") && !_symbols.Contains(t.Lexeme))
                    _diagnostics.Add(new($"Variables are not supported yet.", t.Span));
                return CheckExtension(new BinaryExpr(expr, TokenKind.Star, CheckExtension(new NameExpr(t.Lexeme), OperaterPrecedence(TokenKind.Star))), parentPrecedence);
            }
            else if (expr is NameExpr n && (!_graph || _graph && n.Name != "x") && !_symbols.Contains(n.Name))
                _diagnostics.Add(new($"Variables are not supported yet.", t.Span));

            return expr;
        }

        private bool IsAtEnd() => _current >= _tokens.Length - 1;
        private Token Current => !IsAtEnd() ? _tokens[_current] : _tokens.Last();
        private void Advance() => ++_current;
        public static int OperaterPrecedence(TokenKind kind) => kind switch
        {
            TokenKind.Power => 3,
            TokenKind.Star or TokenKind.Slash => 2,
            TokenKind.Plus or TokenKind.Minus => 1,
            _ => 0
        };
    }
}
