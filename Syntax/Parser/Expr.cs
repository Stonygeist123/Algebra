using MathShit.Miscellaneous;
using MathShit.Syntax.Lexer;
using System.Globalization;

namespace MathShit.Syntax.Parser
{
    public abstract class Expr
    {
        public abstract float? Evaluate(float param);
        public abstract Expr? Derivative();
        public abstract Expr Simplify();
        public abstract override string ToString();
    }

    public class ErrorExpr : Expr
    {
        public override float? Evaluate(float _) => 0;
        public override Expr? Derivative() => this;
        public override Expr Simplify() => this;
        public override string ToString() => "?";
    }

    public class LiteralExpr : Expr
    {
        public float Value { get; }
        public LiteralExpr(float value) => Value = value;
        public LiteralExpr(Token token) => Value = float.Parse(token.Lexeme, NumberStyles.Any, CultureInfo.InvariantCulture);
        public override float? Evaluate(float _) => Value;
        public override Expr? Derivative() => new LiteralExpr(0);
        public override Expr Simplify() => this;
        public override string ToString() => Value.ToString();
    }

    public class NameExpr : Expr
    {
        public string Name { get; }
        public bool Param => Name == "x";
        public NameExpr(string name) => Name = name;
        public override float? Evaluate(float param) => Param ? param : BuiltIns.Constants[Name];
        public override Expr? Derivative() => new LiteralExpr(1);
        public override Expr Simplify() => this;
        public override string ToString() => Name;
    }

    public class GroupingExpr : Expr
    {
        public Expr Expr { get; }
        public GroupingExpr(Expr expr) => Expr = expr.Simplify();
        public override float? Evaluate(float param) => Expr.Evaluate(param);
        public override Expr? Derivative() => Expr.Derivative();
        public override Expr Simplify() => new GroupingExpr(Expr.Simplify());
        public override string ToString() => "(" + Expr.ToString() + ")";
    }

    public class AbsExpr : Expr
    {
        public Expr Expr { get; }
        public AbsExpr(Expr expr) => Expr = expr.Simplify();
        public override Expr? Derivative() => new BinaryExpr(Expr.Simplify().Derivative()?.Simplify() ?? new ErrorExpr(), TokenKind.Star, BuiltIns.FnsDerivs("abs", Expr));
        public override float? Evaluate(float param)
        {
            float? v = Expr.Evaluate(param);
            return v is null ? null : MathF.Abs(v.Value);
        }

        public override Expr Simplify() => Expr is LiteralExpr l ? new LiteralExpr(BuiltIns.Fns["abs"](l.Value)) : new AbsExpr(Expr);
        public override string ToString() => "|" + Expr.ToString() + "|";
    }

    public class UnaryExpr : Expr
    {
        public TokenKind Op { get; }
        public Expr Operand { get; }
        public UnaryExpr(TokenKind op, Expr operand)
        {
            Op = op;
            Operand = operand.Simplify();
        }

        public override float? Evaluate(float param) => Op switch
        {
            TokenKind.Minus => -Operand.Evaluate(param),
            _ => null
        };

        public override Expr? Derivative()
        {
            Expr? dx = Operand.Derivative()?.Simplify();
            return dx is null ? null : new UnaryExpr(Op, dx!).Simplify();
        }

        public override Expr Simplify()
        {
            Expr expr = Operand;
            return Op switch
            {
                TokenKind.Minus => expr is LiteralExpr l ? new LiteralExpr(-l.Value) : new UnaryExpr(TokenKind.Minus, expr),
                _ => new ErrorExpr()
            };
        }
        public override string ToString() => "-" + Operand.ToString();
    }

    public class BinaryExpr : Expr
    {
        public Expr Left { get; }
        public TokenKind Op { get; }
        public Expr Right { get; }
        public BinaryExpr(Expr left, TokenKind op, Expr right)
        {
            Left = left.Simplify();
            Op = op;
            Right = right.Simplify();
        }

        public override float? Evaluate(float param)
        {
            float? left = Left.Evaluate(param);
            float? right = Right.Evaluate(param);
            if (left is null || right is null ||
                (Op == TokenKind.Slash && right == 0f) ||
                (Op == TokenKind.Power && left < 0 && 1f / right.Value % 2 == 0))
                return null;

            float? res = Op switch
            {
                TokenKind.Plus => left + right,
                TokenKind.Minus => left - right,
                TokenKind.Star => left * right,
                TokenKind.Slash => left / right,
                TokenKind.Power => MathF.Pow(left ?? 1, right ?? 1),
                _ => null
            };

            return res is float.NegativeInfinity or float.PositiveInfinity ? null : res;
        }

        public override Expr? Derivative()
        {
            Expr? leftDx = Left.Derivative();
            Expr? rightDx = Right.Derivative();
            if (leftDx is null || rightDx is null)
                return null;

            return (Op switch
            {
                TokenKind.Plus => new BinaryExpr(leftDx, TokenKind.Plus, rightDx),
                TokenKind.Minus => new BinaryExpr(leftDx, TokenKind.Minus, rightDx),
                TokenKind.Star => new BinaryExpr(new BinaryExpr(leftDx, TokenKind.Star, Right), TokenKind.Plus, new BinaryExpr(Left, TokenKind.Star, rightDx)),
                TokenKind.Slash => new BinaryExpr(new BinaryExpr(new BinaryExpr(leftDx, TokenKind.Star, Right), TokenKind.Minus, new BinaryExpr(Left, TokenKind.Star, rightDx)), TokenKind.Slash, new BinaryExpr(Right, TokenKind.Power, new LiteralExpr(2))),
                TokenKind.Power => new BinaryExpr(leftDx, TokenKind.Star, Right is LiteralExpr r && r.Value == 1 ? Left : new BinaryExpr(Right, TokenKind.Star, new BinaryExpr(Left, TokenKind.Power, new BinaryExpr(Right, TokenKind.Minus, new LiteralExpr(1))))),
                _ => null
            })?.Simplify();
        }

        public override Expr Simplify()
        {
            float? lV = Left is LiteralExpr l ? l.Value : null; // Left is NameExpr ln && !ln.Param ? ln.Evaluate(1) : null;
            float? rV = Right is LiteralExpr r ? r.Value : null; // Right is NameExpr rn && !rn.Param ? rn.Evaluate(1) : null;
            if (lV is not null && rV is not null)
                return new LiteralExpr(Op switch
                {
                    TokenKind.Plus => lV.Value + rV.Value,
                    TokenKind.Minus => lV.Value - rV.Value,
                    TokenKind.Star => lV.Value * rV.Value,
                    TokenKind.Slash => lV.Value / rV.Value,
                    TokenKind.Power => MathF.Pow(lV.Value, rV.Value),
                    _ => 0f
                });
            else if (Op == TokenKind.Star)
            {
                if (Left is LiteralExpr l1 && l1.Value == 1)
                    return Right;
                else if (Right is LiteralExpr r1 && r1.Value == 1)
                    return Left;
                else if (Left is LiteralExpr l2 && l2.Value == 0)
                    return new LiteralExpr(0);
                else if (Right is LiteralExpr r2 && r2.Value == 0)
                    return new LiteralExpr(0);
            }
            else if (Op == TokenKind.Slash)
            {
                if (Left is LiteralExpr l1 && l1.Value == 0)
                    return new LiteralExpr(0);
                else if (Right is LiteralExpr r1 && r1.Value == 1)
                    return Left;
            }
            else if (Left is BinaryExpr)
                return Right is BinaryExpr
                    ? new BinaryExpr(new GroupingExpr(Left), Op, new GroupingExpr(Right))
                    : new BinaryExpr(new GroupingExpr(Left), Op, Right);
            else if (Right is BinaryExpr)
                return Left is BinaryExpr
                    ? new BinaryExpr(new GroupingExpr(Left), Op, new GroupingExpr(Right))
                    : new BinaryExpr(Left, Op, new GroupingExpr(Right));

            return new BinaryExpr(Left, Op, Right);
        }

        public override string ToString() => Left.ToString() + (Op switch
        {
            TokenKind.Plus => " + ",
            TokenKind.Minus => " - ",
            TokenKind.Star => "*",
            TokenKind.Slash => "/",
            TokenKind.Power => "^",
            _ => ""
        }) + Right.ToString();
    }

    public class FunctionExpr : Expr
    {
        public string Name { get; }
        public Expr Arg { get; }
        Func<float, float> Fn { get; }
        public FunctionExpr(string name, Expr arg)
        {
            Name = name;
            Fn = BuiltIns.Fns[Name];
            Arg = arg.Simplify();
        }

        public FunctionExpr(Func<float, float> fn, string name, Expr arg)
        {
            Name = name;
            Fn = fn;
            Arg = arg;
        }

        public override float? Evaluate(float param) => Fn(param);
        public override Expr? Derivative()
        {
            Expr? arg = Arg.Derivative()?.Simplify();
            return arg is null ? null : new BinaryExpr(arg, TokenKind.Star, BuiltIns.FnsDerivs(Name, Arg)).Simplify();
        }

        public override Expr Simplify() => Arg is LiteralExpr l ? new LiteralExpr(Fn(l.Value)) : (Expr)new FunctionExpr(Fn, Name, Arg);
        public override string ToString() => Name + "(" + Arg.ToString() + ")";
    }
}