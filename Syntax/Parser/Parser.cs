using MathShit.Miscellaneous;
using MathShit.Syntax.Lexer;

namespace MathShit.Syntax.Parser
{
    internal class Parser
    {
        private readonly Token[] _tokens;
        private int _current = 0;
        private readonly DiagnosticBag _diagnostics = new();
        public Diagnostic[] Diagnostics => _diagnostics.ToArray();
        public Parser(Token[] tokens) => _tokens = tokens.ToArray();
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
                    if (t.Lexeme != "x" && !BuiltIns.Constants.Any(c => c.Key == t.Lexeme) && !BuiltIns.Fns.ContainsKey(t.Lexeme))
                        _diagnostics.Add($"Constant variables are not supported yet.", t.Span);
                    return CheckExtension(new NameExpr(t.Lexeme), parentPrecedence);
                case TokenKind.LParen:
                    return CheckExtension(GetGrouping(), parentPrecedence);
                case TokenKind.Pipe:
                    return CheckExtension(GetPipe(), parentPrecedence);
                case TokenKind.Minus:
                    return CheckExtension(GetUnary(t), parentPrecedence);
                default:
                    _diagnostics.Add($"Unknown expression.", t.Span);
                    return new ErrorExpr();
            };
        }

        private Expr GetGrouping()
        {
            Expr expr = ParseExpr();
            Token rParen = Current;
            Advance();
            if (rParen.Kind == TokenKind.RParen)
                return new GroupingExpr(expr);

            _diagnostics.Add($"Expected \")\".", rParen.Span);
            return new ErrorExpr();
        }

        private Expr GetPipe()
        {
            Expr expr = ParseExpr();
            Token rPipe = Current;
            Advance();
            if (rPipe.Kind == TokenKind.Pipe)
                return new AbsExpr(expr);

            _diagnostics.Add($"Expected \"|\".", rPipe.Span);
            return new ErrorExpr();
        }

        private Expr GetUnary(Token minus)
        {
            Expr expr = ParseExpr();
            return new UnaryExpr(minus.Kind, expr);
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
                        _diagnostics.Add($"Expected ')'.", t.Span);
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
                        _diagnostics.Add($"Expected ')'.", t.Span);
                        return new ErrorExpr();
                    }

                    Advance();
                    right = CheckExtension(new GroupingExpr(right), OperaterPrecedence(TokenKind.Power));
                    expr = new BinaryExpr(expr, null, right);
                }
            }
            else if (t.Kind == TokenKind.Name)
            {
                Advance();
                if (t.Lexeme != "x" && !BuiltIns.Constants.Any(c => c.Key == t.Lexeme))
                    _diagnostics.Add($"Constant variables are not supported yet.", t.Span);
                return CheckExtension(new BinaryExpr(expr, null, CheckExtension(new NameExpr(t.Lexeme), OperaterPrecedence(TokenKind.Star))));
            }
            else if (expr is NameExpr n && n.Name != "x" && !BuiltIns.Constants.Any(c => c.Key == n.Name))
                _diagnostics.Add($"Constant variables are not supported yet.", t.Span);

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
