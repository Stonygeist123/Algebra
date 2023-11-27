using Algebra.Syntax.Parser;
using SkiaSharp;

namespace Algebra.Algorithms
{
    public static class Roots
    {
        public const float Zero = 1e-7f;
        public static SKPoint? CheckRoot(SKPoint p, Expr fn, Expr dx, Dictionary<string, float> symbols)
        {
            if (fn is null || dx is null)
                return null;

            float x = p.X;
            for (int i = 0; i < 50 && !float.IsNaN(x) && float.IsFinite(x); i++)
            {
                float m = dx.Evaluate(symbols, x);
                if (float.IsNaN(m) || !float.IsFinite(m))
                    break;

                float b = fn.Evaluate(symbols, x) - (m * x);
                x = (-b) / m;
            }

            float y = fn.Evaluate(symbols, x);
            if (MathF.Abs(y) <= Zero)
                return new(x, y);
            return null;
        }
    }
}