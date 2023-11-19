﻿using MathShit.Miscellaneous;
using MathShit.Syntax.Lexer;
using System.Globalization;

namespace MathShit.Syntax.Parser
{
    public abstract class Expr
    {
        public abstract float? Evaluate(float param);
        public abstract Expr? Derivative();
        public abstract Expr Simplify();
    }

    public class ErrorExpr : Expr
    {
        public override float? Evaluate(float _) => 0;
        public override Expr? Derivative() => this;
        public override Expr Simplify() => this;
    }

    public class LiteralExpr : Expr
    {
        public float Value { get; }
        public LiteralExpr(float value) => Value = value;
        public LiteralExpr(Token token) => Value = float.Parse(token.Lexeme, NumberStyles.Any, CultureInfo.InvariantCulture);
        public override float? Evaluate(float _) => Value;
        public override Expr? Derivative() => new LiteralExpr(0);
        public override Expr Simplify() => this;
    }

    public class NameExpr : Expr
    {
        public string Name { get; }
        public bool Param => Name == "x";
        public NameExpr(string name) => Name = name;
        public override float? Evaluate(float param) => Param ? param : BuiltIns.Constants[Name];
        public override Expr? Derivative() => new LiteralExpr(1);
        public override Expr Simplify() => this;
    }

    public class GroupingExpr : Expr
    {
        public Expr Expr { get; }
        public GroupingExpr(Expr expr) => Expr = expr;
        public override float? Evaluate(float param) => Expr.Evaluate(param);
        public override Expr? Derivative() => Expr.Simplify().Derivative()?.Simplify();
        public override Expr Simplify() => Expr.Simplify();
    }

    public class AbsExpr : Expr
    {
        public Expr Expr { get; }
        public AbsExpr(Expr expr) => Expr = expr;
        public override Expr? Derivative() => BuiltIns.FnsDerivs("abs", Expr);
        public override float? Evaluate(float param)
        {
            float? v = Expr.Evaluate(param);
            return v is null ? null : MathF.Abs(v.Value);
        }

        public override Expr Simplify()
        {
            Expr expr = Expr.Simplify();
            return expr is LiteralExpr l ? new LiteralExpr(BuiltIns.Fns["abs"](l.Value)) : new AbsExpr(expr);
        }
    }

    public class UnaryExpr : Expr
    {
        public TokenKind Op { get; }
        public Expr Operand { get; }
        public UnaryExpr(TokenKind op, Expr operand)
        {
            Op = op;
            Operand = operand;
        }

        public override float? Evaluate(float param) => Op switch
        {
            TokenKind.Minus => -Operand.Evaluate(param),
            _ => null
        };

        public override Expr? Derivative()
        {
            Expr? dx = Operand.Simplify().Derivative()?.Simplify();
            return dx is null ? null : new UnaryExpr(Op, dx!).Simplify();
        }

        public override Expr Simplify()
        {
            Expr expr = Operand.Simplify();
            return Op switch
            {
                TokenKind.Minus => expr is LiteralExpr l ? new LiteralExpr(-l.Value) : new UnaryExpr(TokenKind.Minus, expr),
                _ => new ErrorExpr()
            };
        }
    }

    public class BinaryExpr : Expr
    {
        public Expr Left { get; }
        public TokenKind? Op { get; }
        public Expr Right { get; }
        public BinaryExpr(Expr left, TokenKind? op, Expr right)
        {
            Left = left;
            Op = op;
            Right = right;
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
                null or TokenKind.Star => left * right,
                TokenKind.Slash => left / right,
                TokenKind.Power => MathF.Pow(left ?? 1, right ?? 1),
                _ => null
            };

            return res is float.NegativeInfinity or float.PositiveInfinity ? null : res;
        }

        public override Expr? Derivative()
        {
            Expr? leftDx = Left.Simplify().Derivative()?.Simplify();
            Expr? rightDx = Right.Simplify().Derivative()?.Simplify();
            if (leftDx is null || rightDx is null)
                return null;

            return (Op switch
            {
                TokenKind.Plus => new BinaryExpr(leftDx, TokenKind.Plus, rightDx),
                TokenKind.Minus => new BinaryExpr(leftDx, TokenKind.Minus, rightDx),
                null or TokenKind.Star => new BinaryExpr(new BinaryExpr(leftDx, TokenKind.Star, Right), TokenKind.Plus, new BinaryExpr(Left, TokenKind.Star, rightDx)),
                TokenKind.Slash => new BinaryExpr(new BinaryExpr(new BinaryExpr(leftDx, TokenKind.Star, Right), TokenKind.Minus, new BinaryExpr(Left, TokenKind.Star, rightDx)), TokenKind.Slash, new BinaryExpr(Right, TokenKind.Power, new LiteralExpr(2))),
                TokenKind.Power => Right is LiteralExpr l && l.Value == 1 ? Right : new BinaryExpr(Right, TokenKind.Star, new BinaryExpr(Left, TokenKind.Power, new BinaryExpr(Right, TokenKind.Minus, new LiteralExpr(1)))),
                _ => null
            })?.Simplify();
        }

        public override Expr Simplify()
        {
            Expr left = Left.Simplify();
            Expr right = Right.Simplify();
            if (left is LiteralExpr l && right is LiteralExpr r)
                return new LiteralExpr(Op switch
                {
                    TokenKind.Plus => l.Value + r.Value,
                    TokenKind.Minus => l.Value - r.Value,
                    null or TokenKind.Star => l.Value * r.Value,
                    TokenKind.Slash => l.Value / r.Value,
                    TokenKind.Power => MathF.Pow(l.Value, r.Value),
                    _ => 0f
                });
            else if (Op == TokenKind.Star)
            {
                if (left is LiteralExpr l1 && l1.Value == 1)
                    return right;
                else if (right is LiteralExpr r1 && r1.Value == 1)
                    return left;
                else if (left is LiteralExpr l2 && l2.Value == 0)
                    return new LiteralExpr(0);
                else if (right is LiteralExpr r2 && r2.Value == 0)
                    return new LiteralExpr(0);
            }
            else if (Op == TokenKind.Slash)
            {
                if (left is LiteralExpr l1 && l1.Value == 0)
                    return new LiteralExpr(0);
                else if (right is LiteralExpr r1 && r1.Value == 1)
                    return left;
            }

            return new BinaryExpr(left, Op, right);
        }
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
            Arg = arg;
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
            Expr? arg = Arg.Simplify().Derivative()?.Simplify();
            return arg is null ? null : new BinaryExpr(arg, TokenKind.Star, BuiltIns.FnsDerivs(Name, Arg)).Simplify();
        }

        public override Expr Simplify()
        {
            Expr arg = Arg.Simplify();
            return arg is LiteralExpr l ? new LiteralExpr(Fn(l.Value)) : (Expr)new FunctionExpr(Fn, Name, arg);
        }
    }
}