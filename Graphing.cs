using Algebra.Algorithms;
using Algebra.Miscellaneous;
using Algebra.Syntax.Lexer;
using Algebra.Syntax.Parser;
using SkiaSharp;

namespace Algebra
{
    public partial class Graphing : Form
    {
        private float _scaleFactor = 50f;
        private const float _xAxisCount = 7.5f, _yAxisCount = 7.5f;
        private Expr? _expr = null, _dx = null;
        private bool _hasError = false;
        private const float graphWidth = 1.5f;
        private const float increment = .01f;
        public Graphing() => InitializeComponent();
        private void Form1_Load(object sender, EventArgs e) => DrawGraph();
        private void Analyse()
        {
            Lexer lexer = new(Txt_Fn.Text);
            Token[] tokens = lexer.Lex();
            if (!(_hasError = lexer.Diagnostics.Any()))
            {
                Parser parser = new(tokens);
                _expr = parser.Parse().Simplify();
                _dx = _expr.Derivative();
                Txt_Dx.Text = "f'(x) = " + _dx?.ToString() ?? "None";
                _hasError = parser.Diagnostics.Any();
            }
        }

        private void DrawGraph()
        {
            int w = Width - (int)MathF.Floor(Width * .04f), h = Height - 2 * Txt_Fn.Height;
            Img_Graph.Width = w;
            Img_Graph.Height = h;
            Img_Graph.Location = new Point((int)MathF.Floor(Width * .02f), 2 * Txt_Fn.Height);

            SKImageInfo imgInfo = new(w, h);
            SKSurface surface = SKSurface.Create(imgInfo);
            SKCanvas canvas = surface.Canvas;
            canvas.Clear();
            SKPaint pathPaint = new()
            {
                Color = SKColors.Black,
                StrokeWidth = 4,
                IsAntialias = true
            }, textPaint = new()
            {
                Color = SKColors.Black,
                StrokeWidth = 1.25f,
                IsAntialias = true,
                IsStroke = true
            };

            canvas.DrawLine(new(0, h / 2), new(w, h / 2), pathPaint);
            canvas.DrawLine(new(w / 2, 0), new(w / 2, h), pathPaint);

            pathPaint.Color = SKColors.DarkGray;
            pathPaint.StrokeWidth = 2;
            /* x-Axis */
            for (float n = 0; n < w / 2; n += w / 2 / _xAxisCount)
            {
                float x = n + w / 2;
                canvas.DrawLine(new(x, h / 2 + 15), new(x, h / 2 - 15), pathPaint);
                if (n != 0)
                    canvas.DrawText($"{n / _scaleFactor}", new(x + 5, h / 2 - 25), textPaint);
                else
                    canvas.DrawText($"{n / _scaleFactor}", new(x + 5, h / 2 - 5), textPaint);
            }

            for (float n = w / 2 / _xAxisCount; n < w / 2; n += w / 2 / _xAxisCount)
            {
                float x = (w / 2) - n;
                canvas.DrawLine(new(x, h / 2 + 15), new(x, h / 2 - 15), pathPaint);
                canvas.DrawText($"{-(n / _scaleFactor)}", new(x + 5, h / 2 - 25), textPaint);
            }

            /* y-Axis */
            for (float n = 0; n < h / 2; n += h / 2 / _yAxisCount)
            {
                float y = n + h / 2;
                canvas.DrawLine(new(w / 2 - 15, y), new(w / 2 + 15, y), pathPaint);
                if (n != 0)
                    canvas.DrawText($"{-(n / _scaleFactor)}", new(w / 2 + 20, y + 4.5f), textPaint);
            }

            for (float n = h / 2 / _yAxisCount; n < w / 2; n += h / 2 / _yAxisCount)
            {
                float y = (h / 2) - n;
                canvas.DrawLine(new(w / 2 - 15, y), new(w / 2 + 15, y), pathPaint);
                canvas.DrawText($"{n / _scaleFactor}", new(w / 2 + 20, y + 4.5f), textPaint);
            }

            if (!_hasError && _expr is not null)
            {
                pathPaint = new()
                {
                    Color = SKColors.DarkCyan,
                    StrokeWidth = graphWidth,
                    IsAntialias = true,
                    IsStroke = true,
                    Style = SKPaintStyle.Stroke
                };

                Dictionary<string, float> symbols = BuiltIns.Constants;
                SKPath path = new();
                float i = -w / 2;
                float py = h / 2;
                for (; i < w / 2; i += increment)
                    if (_expr.Evaluate(symbols, i) is float v && !float.IsNaN(v) && float.IsFinite(v))
                    {
                        py = v;
                        break;
                    }

                path.MoveTo(new SKPoint(i + w / 2, -py + h / 2));
                List<SKPoint> roots = new();
                for (i = -w / 2; i <= w / 2; i += increment)
                {
                    float? p_y1 = _expr.Evaluate(symbols, i), p_y2 = _expr.Evaluate(symbols, i + increment);
                    if (MathF.Abs(p_y1 ?? 1) <= 1e-3f || MathF.Abs(p_y2 ?? 1) <= 1e-3f)
                    {
                        Console.WriteLine();
                    }

                    if (p_y1 is float y1 && p_y2 is float y2 && !float.IsNaN(y1) && !float.IsNaN(y2) && float.IsFinite(y1) && float.IsFinite(y2))
                    {
                        float coord_y1 = -_scaleFactor * y1 + h / 2;
                        float coord_y2 = -_scaleFactor * y2 + h / 2;
                        if (float.IsInfinity(coord_y1) || float.IsInfinity(coord_y2))
                            continue;

                        float x1 = _scaleFactor * i + w / 2;
                        float x2 = _scaleFactor * (i + increment) + w / 2;
                        path.QuadTo(new(x1, coord_y1), new(x2, coord_y2));

                        SKPoint? p0 = Roots.CheckRoot(new(i, y1), new(i + increment, y2), _expr, symbols);
                        if (p0 is not null)
                            roots.Add(new(_scaleFactor * p0.Value.X + w / 2, -_scaleFactor * p0.Value.Y + h / 2));
                    }
                    else if (path.PointCount > 1)
                    {
                        canvas.DrawPath(path, pathPaint);
                        path.Dispose();
                        path = new();
                    }
                }

                canvas.DrawPath(path, pathPaint);
                path.Dispose();

                SKPaint detailsPaint = new()
                {
                    Color = SKColors.Purple,
                    StrokeWidth = graphWidth,
                    IsAntialias = true,
                    IsStroke = true,
                    Style = SKPaintStyle.Fill
                };

                foreach (SKPoint r in roots)
                    canvas.DrawCircle(r, 5f, detailsPaint);
                if (_dx is not null)
                {
                    Dictionary<SKPoint, bool> extrema = new();
                    pathPaint = new()
                    {
                        Color = SKColors.Red,
                        StrokeWidth = graphWidth,
                        IsAntialias = true,
                        IsStroke = true,
                        Style = SKPaintStyle.Stroke
                    };

                    path = new();
                    i = -w / 2;
                    py = h / 2;
                    for (; i < w / 2; i += increment)
                        if (_dx.Evaluate(symbols, i) is float v && !float.IsNaN(v) && float.IsFinite(v))
                        {
                            py = v;
                            break;
                        }

                    path.MoveTo(new SKPoint(_scaleFactor * i + w / 2, -_scaleFactor * py + h / 2));
                    Expr? dx1 = _dx.Derivative(), dx2 = dx1?.Derivative();
                    for (i = -w / 2; i < w / 2; i += increment)
                    {
                        float? pY1 = _dx.Evaluate(symbols, i), pY2 = _dx.Evaluate(symbols, i + increment);
                        if (pY1 is float y1 && pY2 is float y2 && !float.IsNaN(y1) && !float.IsNaN(y2) && float.IsFinite(y1) && float.IsFinite(y2))
                        {
                            float coord_y1 = -_scaleFactor * y1 + h / 2;
                            float coord_y2 = -_scaleFactor * y2 + h / 2;
                            if (float.IsInfinity(coord_y1) || float.IsInfinity(coord_y2))
                                continue;

                            float x1 = _scaleFactor * i + w / 2;
                            float x2 = _scaleFactor * (i + increment) + w / 2;
                            path.QuadTo(x1, coord_y1, x2, coord_y2);

                            SKPoint? p0 = Roots.CheckRoot(new(i, y1), new(i + increment, y2), _dx, symbols);
                            if (p0 is not null && dx1 is not null)
                            {
                                float v = dx1.Evaluate(symbols, p0.Value.X);
                                float v1 = dx2?.Evaluate(symbols, p0.Value.X) ?? 0f;
                                if (MathF.Abs(v) > 1e-5f && Math.Abs(v1) <= 1e-5f)
                                    extrema.TryAdd(new(_scaleFactor * p0.Value.X + w / 2, -_scaleFactor * _expr.Evaluate(symbols, p0.Value.X) + h / 2), v < 0);
                            }
                        }
                    }

                    canvas.DrawPath(path, pathPaint);
                    path.Dispose();
                    foreach (KeyValuePair<SKPoint, bool> e in extrema)
                    {
                        canvas.DrawCircle(e.Key, 5f, detailsPaint);
                        canvas.DrawText(e.Value ? "HP" : "LP", new(e.Key.X + 15, e.Key.Y - 15), textPaint);
                    }
                }

                canvas.Save();
            }

            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            using MemoryStream mStream = new(data.ToArray());
            Bitmap bm = new(mStream, false);
            Img_Graph.Image = bm;
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            DrawGraph();
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (e.Delta > 0)
                _scaleFactor *= 1.5f;
            else
                _scaleFactor /= 1.5f;
            DrawGraph();
        }

        protected void Txt_Fn_OnKeyDown(object sender, KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Enter)
            {
                Analyse();
                if (!_hasError)
                    DrawGraph();
            }
        }
    }
}