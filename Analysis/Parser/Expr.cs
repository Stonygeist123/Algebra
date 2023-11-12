using MathShit.Miscellaneous;
using System.Globalization;

namespace MathShit.Analysis.Parser
{
    public enum ExprKind
    {
        Literal, Name, Binary, Unary
    }

    public abstract class Expr
    {
        public Expr(TextSpan span) => Span = span;
        public TextSpan Span { get; }
        public abstract ExprKind Kind { get; }
        public abstract float Evaluate(float param);
    }

    public class ErrorExpr : Expr
    {
        public Token Token { get; }
        public override ExprKind Kind => ExprKind.Literal;
        public ErrorExpr(Token token) : base(token.Span) => Token = token;
        public override float Evaluate(float _) => 0;
    }

    public class LiteralExpr : Expr
    {
        public Token Token { get; }
        public override ExprKind Kind => ExprKind.Literal;
        public LiteralExpr(Token token) : base(token.Span) => Token = token;
        public override float Evaluate(float _) => float.Parse(Token.Lexeme, NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    public class NameExpr : Expr
    {
        public Token Token { get; }
        public bool Param { get; }
        public override ExprKind Kind => ExprKind.Name;
        public NameExpr(Token token, bool param) : base(token.Span)
        {
            Token = token;
            Param = param;
        }
        public override float Evaluate(float param) => param;
    }

    public class GroupingExpr : Expr
    {
        public override ExprKind Kind => ExprKind.Name;
        public Token LParen { get; }
        public Expr Expr { get; }
        public Token RParen { get; }

        public GroupingExpr(Token lParen, Expr expr, Token rParen) : base(TextSpan.From(lParen.Span.Start, rParen.Span.End))
        {
            LParen = lParen;
            Expr = expr;
            RParen = rParen;
        }
        public override float Evaluate(float param) => Expr.Evaluate(param);
    }

    public class BinaryExpr : Expr
    {
        public Expr Left { get; }
        public Token Op { get; }
        public Expr Right { get; }
        public override ExprKind Kind => ExprKind.Literal;
        public BinaryExpr(Expr left, Token op, Expr right) : base(TextSpan.From(left.Span.Start, right.Span.End))
        {
            Left = left;
            Op = op;
            Right = right;
        }
        public override float Evaluate(float param) => Op.Kind switch
        {
            TokenKind.Plus => Left.Evaluate(param) + Right.Evaluate(param),
            TokenKind.Minus => Left.Evaluate(param) - Right.Evaluate(param),
            TokenKind.Star => Left.Evaluate(param) * Right.Evaluate(param),
            TokenKind.Slash => Left.Evaluate(param) / Right.Evaluate(param),
            TokenKind.Power => MathF.Pow(Left.Evaluate(param), Right.Evaluate(param)),
            _ => throw new NotImplementedException()
        };
    }

    public class UnaryExpr : Expr
    {
        public Token Op { get; }
        public Expr Operand { get; }
        public override ExprKind Kind => ExprKind.Literal;
        public UnaryExpr(Token op, Expr operand) : base(TextSpan.From(op.Span.Start, operand.Span.End))
        {
            Op = op;
            Operand = operand;
        }
        public override float Evaluate(float param) => -Operand.Evaluate(param);
    }
}