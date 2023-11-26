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
            for (int i = 0; i < 15; i++)
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

            /*if (MathF.Abs(p1.Y) <= Zero)
                return p1;
            else if (MathF.Abs(p2.Y) <= Zero)
                return p2;

            if (p1.Y <= Zero && p2.Y <= Zero ||
                p1.Y >= Zero && p2.Y >= Zero)
                return null;

            float a = p1.X,
                b = p2.X,
                c = (a + b) / 2;
            float c_ev = fn.Evaluate(symbols, c);
            for (int i = 0; i < 15; ++i)
            {
                float b_ev = fn.Evaluate(symbols, b),
                    a_ev = fn.Evaluate(symbols, a);
                if (c_ev == 0)
                    return new(c, c_ev);

                if (c_ev < 0 && b_ev > 0 || c_ev > 0 && b_ev < 0)
                    a = c;
                else if (c_ev < 0 && a_ev > 0 || c_ev > 0 && a_ev < 0)
                    b = c;

                c = (a + b) / 2;
                c_ev = fn.Evaluate(symbols, c);
            }

            if (MathF.Abs(fn.Evaluate(symbols, c)) <= Zero)
                return new(c, c_ev);
            return null;*/
        }
    }
}