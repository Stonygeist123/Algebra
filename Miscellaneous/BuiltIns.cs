using Algebra.Syntax.Lexer;
using Algebra.Syntax.Parser;

namespace Algebra.Miscellaneous
{
    public static class BuiltIns
    {
        public static Dictionary<string, Func<float, float>> Fns => new() {
            { "sin", MathF.Sin },
            { "cos", MathF.Cos },
            { "tan", MathF.Tan },
            { "asin", MathF.Asin },
            { "acos", MathF.Acos },
            { "atan", MathF.Atan },
            { "sec", x => 1 / MathF.Cos(x) },
            { "csc", x => 1 / MathF.Sin(x) },
            { "cot", x => MathF.Cos(x)/  MathF.Sin(x) },
            { "asec", x => MathF.Acos(1 / x) },
            { "acsc", x => MathF.Asin(1 / x) },
            { "acot", x => MathF.Atan(1 / x) },
            { "abs", MathF.Abs },
            { "sgn", x => MathF.Sign(x) },
            { "ln",  MathF.Log }
        };

        public static Expr FnsDerivs(string name, Expr arg)
        {
            return name switch
            {
                "sin" => new FunctionExpr("cos", arg),
                "cos" => new UnaryExpr(TokenKind.Minus, new FunctionExpr("sin", arg)),
                "tan" => new BinaryExpr(new FunctionExpr("sec", arg), TokenKind.Power, new LiteralExpr(2)),
                "asin" => new BinaryExpr(new LiteralExpr(1), TokenKind.Slash, new BinaryExpr(new BinaryExpr(new LiteralExpr(1), TokenKind.Minus, new BinaryExpr(arg, TokenKind.Power, new LiteralExpr(2))), TokenKind.Power, new LiteralExpr(.5f))),
                "acos" => new UnaryExpr(TokenKind.Minus, new BinaryExpr(new LiteralExpr(1), TokenKind.Slash, new BinaryExpr(new BinaryExpr(new LiteralExpr(1), TokenKind.Minus, new BinaryExpr(arg, TokenKind.Power, new LiteralExpr(2))), TokenKind.Power, new LiteralExpr(.5f)))),
                "atan" => new BinaryExpr(new LiteralExpr(1), TokenKind.Slash, new BinaryExpr(new LiteralExpr(1), TokenKind.Plus, new BinaryExpr(arg, TokenKind.Power, new LiteralExpr(2)))),
                "sec" => new BinaryExpr(new FunctionExpr("sec", arg), TokenKind.Star, new FunctionExpr("tan", arg)),
                "csc" => new BinaryExpr(new UnaryExpr(TokenKind.Minus, new FunctionExpr("csc", arg)), TokenKind.Star, new FunctionExpr("cot", arg)),
                "cot" => new UnaryExpr(TokenKind.Minus, new BinaryExpr(new FunctionExpr("csc", arg), TokenKind.Power, new LiteralExpr(2))),
                "asec" => new BinaryExpr(new LiteralExpr(1), TokenKind.Slash, new BinaryExpr(new AbsExpr(arg), TokenKind.Star, new BinaryExpr(new BinaryExpr(new BinaryExpr(arg, TokenKind.Power, new LiteralExpr(2)), TokenKind.Minus, new LiteralExpr(1)), TokenKind.Power, new LiteralExpr(.5f)))),
                "acsc" => new UnaryExpr(TokenKind.Minus, new BinaryExpr(new LiteralExpr(1), TokenKind.Slash, new BinaryExpr(new AbsExpr(arg), TokenKind.Star, new BinaryExpr(new BinaryExpr(new BinaryExpr(arg, TokenKind.Power, new LiteralExpr(2)), TokenKind.Minus, new LiteralExpr(1)), TokenKind.Power, new LiteralExpr(.5f))))),
                "acot" => new UnaryExpr(TokenKind.Minus, new BinaryExpr(new LiteralExpr(1), TokenKind.Slash, new BinaryExpr(new LiteralExpr(1), TokenKind.Plus, new BinaryExpr(arg, TokenKind.Power, new LiteralExpr(2))))),
                "abs" => new FunctionExpr("sgn", arg),
                "sgn" => new LiteralExpr(0),
                "ln" => new BinaryExpr(new LiteralExpr(1), TokenKind.Slash, arg),
                _ => new ErrorExpr()
            };
        }

        public static Dictionary<string, float> Constants => new() {
            { "pi", MathF.PI },
            { "e", MathF.E }
        };
    }
}