using Algebra.Syntax.Parser;
using SkiaSharp;

namespace Algebra.Algorithms
{
    public static class Roots
    {
        public static SKPoint? CheckRoot(SKPoint p1, SKPoint p2, Expr fn, Dictionary<string, float> symbols)
        {
            if (MathF.Abs(p1.Y) <= 1e-5f)
                return p1;
            else if (MathF.Abs(p2.Y) <= 1e-5f)
                return p2;

            if (p1.Y <= 1e-5f && p2.Y <= 1e-5f ||
                p1.Y >= 1e-5f && p2.Y >= 1e-5f)
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

            if (MathF.Abs(fn.Evaluate(symbols, c)) <= 1e-5f)
                return new(c, c_ev);
            return null;
        }
    }
}