using Algebra.Miscellaneous;
using Algebra.Syntax.Lexer;
using System.Globalization;

namespace Algebra.Syntax.Parser
{
    public abstract class Expr
    {
        public abstract float Evaluate(float param);
        public abstract double EvaluateD();
        public abstract Expr? Derivative();
        public abstract Expr Simplify();
        public abstract override string ToString();
    }

    public class ErrorExpr : Expr
    {
        public override float Evaluate(float _) => 0;
        public override double EvaluateD() => 0;
        public override Expr? Derivative() => this;
        public override Expr Simplify() => this;
        public override string ToString() => "?";
    }

    public class LiteralExpr : Expr
    {
        public double Value { get; }
        public float Value_f => (float)Value;
        public LiteralExpr(double value) => Value = (float)value;
        public LiteralExpr(Token token) => Value = float.Parse(token.Lexeme, NumberStyles.Any, CultureInfo.InvariantCulture);
        public override float Evaluate(float _) => Value_f;
        public override double EvaluateD() => Value;
        public override Expr? Derivative() => new LiteralExpr(0);
        public override Expr Simplify() => this;
        public override string ToString() => Value.ToString();
    }

    public class NameExpr : Expr
    {
        public string Name { get; }
        public bool Param => Name == "x";
        public NameExpr(string name) => Name = name;
        public override float Evaluate(float param) => Param ? param : (float)BuiltIns.Constants[Name];
        public override double EvaluateD() => BuiltIns.Constants[Name];
        public override Expr? Derivative() => new LiteralExpr(Param ? 1 : 0);
        public override Expr Simplify() => this;
        public override string ToString() => Name;
    }

    public class GroupingExpr : Expr
    {
        public Expr Expr { get; }
        public GroupingExpr(Expr expr) => Expr = expr.Simplify();
        public override float Evaluate(float param) => Expr.Evaluate(param);
        public override double EvaluateD() => Expr.EvaluateD();
        public override Expr? Derivative() => Expr.Derivative();
        public override Expr Simplify() => new GroupingExpr(Expr.Simplify());
        public override string ToString() => "(" + Expr.ToString() + ")";
    }

    public class AbsExpr : Expr
    {
        public Expr Expr { get; }
        public AbsExpr(Expr expr) => Expr = expr.Simplify();
        public override Expr? Derivative() => new BinaryExpr(Expr.Simplify().Derivative()?.Simplify() ?? new ErrorExpr(), TokenKind.Star, BuiltIns.FnsDerivs("abs", Expr));
        public override double EvaluateD() => Math.Abs(Expr.EvaluateD());
        public override float Evaluate(float param)
        {
            float v = Expr.Evaluate(param);
            return float.IsNaN(v) ? float.NaN : MathF.Abs(v);
        }

        public override Expr Simplify() => Expr is LiteralExpr l ? new LiteralExpr((float)BuiltIns.Fns["abs"](l.Value_f)) : new AbsExpr(Expr);
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

        public override double EvaluateD() => Op switch
        {
            TokenKind.Minus => -Operand.EvaluateD(),
            _ => double.NaN
        };

        public override float Evaluate(float param) => Op switch
        {
            TokenKind.Minus => -Operand.Evaluate(param),
            _ => float.NaN
        };

        public override Expr? Derivative()
        {
            Expr? dx = Operand.Derivative();
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

        public override double EvaluateD()
        {
            double left = Left.EvaluateD();
            double right = Right.EvaluateD();
            if (double.IsNaN(left) || double.IsNaN(right) ||
                (Op == TokenKind.Slash && right == 0f) ||
                (Op == TokenKind.Power && left < 0 && right < 1 && right > 0 && 1f / right % 2 == 0))
                return double.NaN;

            return Op switch
            {
                TokenKind.Plus => left + right,
                TokenKind.Minus => left - right,
                TokenKind.Star => left * right,
                TokenKind.Slash => left / right,
                TokenKind.Power => left < 0 && right < 1 && right > 0 && 1f / right % 2 == 0 ? -Math.Pow(-left, right) : Math.Pow(left, right),
                _ => double.NaN
            };
        }

        public override float Evaluate(float param)
        {
            float left = Left.Evaluate(param);
            float right = Right.Evaluate(param);
            if (float.IsNaN(left) || float.IsNaN(right) ||
                (Op == TokenKind.Slash && right == 0f) ||
                (Op == TokenKind.Power && left < 0 && right < 1 && right > 0 && 1f / right % 2 == 0))
                return float.NaN;

            return Op switch
            {
                TokenKind.Plus => left + right,
                TokenKind.Minus => left - right,
                TokenKind.Star => left * right,
                TokenKind.Slash => left / right,
                TokenKind.Power => left < 0 && right < 1 && right > 0 && 1f / right % 2 == 0 ? -MathF.Pow(-left, right) : MathF.Pow(left, right),
                _ => float.NaN
            };
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
                TokenKind.Power => Left is LiteralExpr ? new BinaryExpr(rightDx, TokenKind.Star, new BinaryExpr(this, TokenKind.Star, new FunctionExpr("ln", Left))) : new BinaryExpr(leftDx, TokenKind.Star, Right is LiteralExpr r && r.Value == 1 ? Left : new BinaryExpr(Right, TokenKind.Star, new BinaryExpr(Left, TokenKind.Power, new BinaryExpr(Right, TokenKind.Minus, new LiteralExpr(1))))),
                _ => null
            })?.Simplify();
        }

        public override Expr Simplify()
        {
            if (Left is LiteralExpr l && Right is LiteralExpr r)
                return new LiteralExpr(Op switch
                {
                    TokenKind.Plus => l.Value + r.Value,
                    TokenKind.Minus => l.Value - r.Value,
                    TokenKind.Star => l.Value * r.Value,
                    TokenKind.Slash => l.Value / r.Value,
                    TokenKind.Power => MathF.Pow(l.Value_f, r.Value_f),
                    _ => float.NaN
                });
            else if ((Left is LiteralExpr || Right is LiteralExpr) && Op == TokenKind.Plus || Op == TokenKind.Minus)
            {
                if (Left is LiteralExpr l1 && l1.Value == 0)
                    return Right;
                else if (Right is LiteralExpr r1 && r1.Value == 0)
                    return Left;
            }
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
                else if (Left is LiteralExpr l3 && Right is BinaryExpr br && br.Op == TokenKind.Star)
                {
                    if (br.Left is LiteralExpr l4)
                        return new BinaryExpr(new LiteralExpr(l3.Value * l4.Value), TokenKind.Star, br.Right);
                    else if (br.Right is LiteralExpr l5)
                        return new BinaryExpr(new LiteralExpr(l3.Value * l5.Value), TokenKind.Star, br.Left);
                    return new BinaryExpr(Left, Op, new GroupingExpr(Right));
                }
                else if (Right is LiteralExpr l4 && Left is BinaryExpr bl && bl.Op == TokenKind.Star)
                {
                    if (bl.Left is LiteralExpr l5)
                        return new BinaryExpr(new LiteralExpr(l4.Value * l5.Value), TokenKind.Star, bl.Right);
                    else if (bl.Right is LiteralExpr l6)
                        return new BinaryExpr(new LiteralExpr(l4.Value * l6.Value), TokenKind.Star, bl.Left);
                    return new BinaryExpr(new GroupingExpr(Left), Op, Right);
                }
            }
            else if (Op == TokenKind.Slash)
            {
                if (Left is LiteralExpr l1 && l1.Value == 0)
                    return new LiteralExpr(0);
                else if (Left is LiteralExpr && Right is BinaryExpr b && b.Op == TokenKind.Slash && b.Left is LiteralExpr l2 && l2.Value == 1)
                    return new BinaryExpr(Left, TokenKind.Slash, b.Right).Simplify();
                else if (Right is LiteralExpr r1 && r1.Value == 1)
                    return Left;
                else if (Right is LiteralExpr && Left is BinaryExpr b1 && b1.Op == TokenKind.Slash && b1.Left is LiteralExpr l3 && l3.Value == 1)
                    return new BinaryExpr(Right, TokenKind.Slash, b1.Right).Simplify();
            }
            else if (Op == TokenKind.Power && Right is LiteralExpr l1 && l1.Value == 1)
                return Left;
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

        public override string ToString() => Left.ToString() + (Op == TokenKind.Star && (Right is NameExpr && Left is LiteralExpr || Right is GroupingExpr) ? "" : Op switch
        {
            TokenKind.Plus => " + ",
            TokenKind.Minus => " - ",
            TokenKind.Star => "*",
            TokenKind.Slash => "/",
            TokenKind.Power => "^",
            _ => ""
        }) + (Op == TokenKind.Power ? "(" + Right.ToString() + ")" : Right.ToString());
    }

    public class FunctionExpr : Expr
    {
        public string Name { get; }
        public Expr Arg { get; }
        Func<double, double> Fn { get; }
        public FunctionExpr(string name, Expr arg)
        {
            Name = name;
            Fn = BuiltIns.Fns[Name];
            Arg = arg.Simplify();
        }

        public override double EvaluateD() => Fn(Arg.EvaluateD());
        public override float Evaluate(float param) => (float)Fn(Arg.Evaluate(param));
        public override Expr? Derivative()
        {
            Expr? arg = Arg.Derivative()?.Simplify();
            return arg is null ? null : new BinaryExpr(arg, TokenKind.Star, BuiltIns.FnsDerivs(Name, Arg)).Simplify();
        }

        public override Expr Simplify() => Arg is LiteralExpr l ? new LiteralExpr(Fn(l.Value)) : (Expr)new FunctionExpr(Name, Arg);
        public override string ToString() => Name + "(" + Arg.ToString() + ")";
    }
}