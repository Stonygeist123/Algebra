using MathShit.Miscellaneous;

namespace MathShit.Analysis.Parser
{
    internal class Parser
    {
        private readonly Token[] _tokens;
        private int _current = 0;
        private readonly DiagnosticBag _diagnostics = new();
        public Diagnostic[] Diagnostics => _diagnostics.ToArray();
        public Parser(Token[] tokens) => _tokens = tokens.Take(tokens.Length - 1).ToArray();
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
                    if (t.Lexeme != "x")
                        _diagnostics.Add($"Constant variables are not supported yet.", t.Span);
                    return CheckExtension(new NameExpr(t, t.Lexeme == "x"), parentPrecedence);

                case TokenKind.LParen:
                    return CheckExtension(GetGrouping(t), parentPrecedence);
                case TokenKind.Minus:
                    return CheckExtension(GetUnary(t), parentPrecedence);
                default:
                    _diagnostics.Add($"Unknown expression.", t.Span);
                    return new ErrorExpr(t);
            };
        }

        private Expr GetGrouping(Token lParen)
        {
            Expr expr = ParseExpr();
            Token rParen = Current;
            Advance();
            return rParen.Kind == TokenKind.RParen ? new GroupingExpr(lParen, expr, rParen) : new ErrorExpr(lParen);
        }

        private Expr GetUnary(Token minus)
        {
            Expr expr = ParseExpr();
            return new UnaryExpr(minus, expr);
        }

        private Expr CheckExtension(Expr expr, int parentPrecedence = 0)
        {
            if (_current >= _tokens.Length)
                return expr;

            Token t = Current;
            if (OperaterPrecedence(t.Kind) > 0)
            {
                Advance();
                int precedence = OperaterPrecedence(t.Kind);
                do
                {
                    if (precedence < parentPrecedence)
                        break;

                    Expr right = ParseExpr(precedence);
                    expr = new BinaryExpr(expr, t, right);
                    if (_current >= _tokens.Length)
                        return expr;

                    t = Current;
                    precedence = OperaterPrecedence(t.Kind);
                } while (precedence > 0);

                expr = CheckExtension(expr, parentPrecedence);
            }

            return expr;
        }

        private Token Current => _tokens[_current];
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
