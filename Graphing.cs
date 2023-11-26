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
        private bool _showDeriv = false, _showRoots = false, _showExtrema = false, _showTurnings = false;
        private SKCanvas? _canvas = null;
        private Dictionary<string, float> _symbols = new();
        private readonly Dictionary<SKPoint, bool> _extrema = new();
        private readonly List<SKPoint> _roots = new(), _turningPoints = new();
        private int _w = 0, _h = 0;
        private readonly SKPaint _detailsPaint = new()
        {
            Color = SKColors.Purple,
            StrokeWidth = graphWidth / 2f,
            IsAntialias = true,
            IsStroke = true,
            Style = SKPaintStyle.Fill
        }, _textPaint = new()
        {
            Color = SKColors.Black,
            StrokeWidth = 1.25f,
            IsAntialias = true,
            IsStroke = true
        }, _axisPaint = new()
        {
            Color = SKColors.Black,
            StrokeWidth = 4,
            IsAntialias = true
        }, _scalingPaint = new()
        {
            Color = SKColors.Gray,
            StrokeWidth = 2,
            IsAntialias = true
        }, _graphPaint = new()
        {
            Color = SKColors.DarkCyan,
            StrokeWidth = graphWidth,
            IsAntialias = true,
            IsStroke = true,
            Style = SKPaintStyle.Stroke
        }, _graphDxPaint = new()
        {
            Color = SKColors.Red,
            StrokeWidth = graphWidth,
            IsAntialias = true,
            IsStroke = true,
            Style = SKPaintStyle.Stroke
        };

        public Graphing() => InitializeComponent();
        private void Form1_Load(object sender, EventArgs e) => DrawAxes(Initialize());
        private SKSurface Initialize()
        {
            _w = Width;
            _h = Height - 2 * Txt_Fn.Height;
            Img_Graph.Width = _w;
            Img_Graph.Height = _h;

            SKImageInfo imgInfo = new(_w, _h);
            SKSurface surface = SKSurface.Create(imgInfo);
            _canvas = surface.Canvas;
            return surface;
        }

        private void Analyse()
        {
            Lexer lexer = new(Txt_Fn.Text);
            Token[] tokens = lexer.Lex();
            if (!(_hasError = lexer.Diagnostics.Any()))
            {
                Parser parser = new(tokens);
                _expr = parser.Parse()?.Simplify();
                _dx = _expr?.Derivative()?.Simplify();
                _hasError = parser.Diagnostics.Any();
                if (_hasError)
                    Txt_Dx.Text = "f'(x) = None";
                else
                    Txt_Dx.Text = "f'(x) = " + _dx?.ToString() ?? "None";
            }
        }

        private void DrawGraph()
        {
            SKSurface surface = Initialize();
            DrawAxes(surface);
            if (!_hasError && _expr is not null && _canvas is not null)
            {
                _symbols = BuiltIns.Constants;
                SKPath path = new();
                float i = -_w / 2;
                float py = _h / 2;
                for (; i < _w / 2; i += increment)
                    if (_expr.Evaluate(_symbols, i) is float v && !float.IsNaN(v) && float.IsFinite(v))
                    {
                        py = v;
                        break;
                    }

                path.MoveTo(new SKPoint(i + _w / 2, -py + _h / 2));
                _roots.Clear();
                for (i = -_w / 2; i <= _w / 2; i += increment)
                {
                    float? p_y1 = _expr.Evaluate(_symbols, i), p_y2 = _expr.Evaluate(_symbols, i + increment);
                    if (p_y1 is float y1 && p_y2 is float y2 && !float.IsNaN(y1) && !float.IsNaN(y2) && float.IsFinite(y1) && float.IsFinite(y2))
                    {
                        float coord_y1 = -_scaleFactor * y1 + _h / 2;
                        float coord_y2 = -_scaleFactor * y2 + _h / 2;
                        if (float.IsInfinity(coord_y1) || float.IsInfinity(coord_y2))
                            continue;

                        float x1 = _scaleFactor * i + _w / 2;
                        float x2 = _scaleFactor * (i + increment) + _w / 2;
                        path.QuadTo(new(x1, coord_y1), new(x2, coord_y2));

                        SKPoint? p0 = Roots.CheckRoot(new(i, y1), _expr, _dx!, _symbols);
                        if (p0 is not null)
                            _roots.Add(new(_scaleFactor * p0.Value.X + _w / 2, -_scaleFactor * p0.Value.Y + _h / 2));
                    }
                }

                _canvas.DrawPath(path, _graphPaint);
                path.Dispose();

                _extrema.Clear();
                _turningPoints.Clear();
                if (_dx is not null)
                {
                    DrawDx();
                    Expr? dx1 = _dx.Derivative(), dx2 = dx1?.Derivative();
                    for (i = -_w / 2; i < _w / 2; i += increment)
                    {
                        float y1 = _dx.Evaluate(_symbols, i), y2 = _dx.Evaluate(_symbols, i + increment);
                        if (!float.IsNaN(y1) && !float.IsNaN(y2) && float.IsFinite(y1) && float.IsFinite(y2))
                        {
                            SKPoint? p0 = Roots.CheckRoot(new(i, y1), _dx, dx1!, _symbols);
                            if (p0 is not null && dx1 is not null)
                            {
                                float v = dx1.Evaluate(_symbols, p0.Value.X);
                                if (MathF.Abs(v) > Roots.Zero)
                                    _extrema.TryAdd(new(_scaleFactor * p0.Value.X + _w / 2, -_scaleFactor * _expr.Evaluate(_symbols, p0.Value.X) + _h / 2), v < 0);
                            }

                            if (dx1 is not null && dx2 is not null)
                            {
                                float yDx1 = dx1.Evaluate(_symbols, i);
                                SKPoint? p01 = Roots.CheckRoot(new(i, yDx1), dx1, dx2, _symbols);
                                if (p01 is not null &&
                                    MathF.Abs(dx2.Evaluate(_symbols, p01.Value.X)) > Roots.Zero)
                                    _turningPoints.Add(new(_scaleFactor * p01.Value.X + _w / 2, -_scaleFactor * _expr.Evaluate(_symbols, p01.Value.X) + _h / 2));
                            }
                        }
                    }

                    _canvas.Save();
                }

                DrawExtrema();
                DrawTurnings();
                DrawRoots();
                DrawImage(surface);
            }
        }

        private void DrawImage(SKSurface surface)
        {
            SKImage image = surface.Snapshot();
            SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            MemoryStream mStream = new(data.ToArray());
            Bitmap bm = new(mStream, false);
            Img_Graph.Image = bm;
        }

        private void DrawAxes(SKSurface surface)
        {
            if (_canvas is null)
                return;

            _canvas.DrawLine(new(0, _h / 2), new(_w, _h / 2), _axisPaint);
            _canvas.DrawLine(new(_w / 2, 0), new(_w / 2, _h), _axisPaint);

            _scalingPaint.Color = SKColors.DarkGray;
            _scalingPaint.StrokeWidth = 2;
            /* x-Axis */
            for (float n = 0; n <= _w / 2; n += _w / 2 / _xAxisCount)
            {
                float x = n + _w / 2;
                _canvas.DrawLine(new(x, _h / 2 + 15), new(x, _h / 2 - 15), _scalingPaint);
                if (n != 0)
                    _canvas.DrawText($"{n / _scaleFactor}", new(x + 5, _h / 2 - 25), _textPaint);
                else
                    _canvas.DrawText($"{n / _scaleFactor}", new(x + 5, _h / 2 - 5), _textPaint);
            }

            for (float n = _w / 2 / _xAxisCount; n < _w / 2; n += _w / 2 / _xAxisCount)
            {
                float x = (_w / 2) - n;
                _canvas.DrawLine(new(x, _h / 2 + 15), new(x, _h / 2 - 15), _scalingPaint);
                _canvas.DrawText($"{-(n / _scaleFactor)}", new(x + 5, _h / 2 - 25), _textPaint);
            }

            /* y-Axis */
            for (float n = 0; n < _h / 2; n += _h / 2 / _yAxisCount)
            {
                float y = n + _h / 2;
                _canvas.DrawLine(new(_w / 2 - 15, y), new(_w / 2 + 15, y), _scalingPaint);
                if (n != 0)
                    _canvas.DrawText($"{-(n / _scaleFactor)}", new(_w / 2 + 20, y + 4.5f), _textPaint);
            }

            for (float n = _h / 2 / _yAxisCount; n < _w / 2; n += _h / 2 / _yAxisCount)
            {
                float y = (_h / 2) - n;
                _canvas.DrawLine(new(_w / 2 - 15, y), new(_w / 2 + 15, y), _scalingPaint);
                _canvas.DrawText($"{n / _scaleFactor}", new(_w / 2 + 20, y + 4.5f), _textPaint);
            }

            _canvas.Save();
            using SKImage image = surface.Snapshot();
            using SKData data = image.Encode(SKEncodedImageFormat.Png, 100);
            using MemoryStream mStream = new(data.ToArray());
            Bitmap bm = new(mStream, false);
            Img_Graph.Image = bm;
        }

        private void DrawDx()
        {
            if (_showDeriv && _expr is not null && _dx is not null)
            {
                SKPath path = new();
                float i = -_w / 2;
                float py = _h / 2;
                for (; i < _w / 2; i += increment)
                    if (_dx.Evaluate(_symbols, i) is float v && !float.IsNaN(v) && float.IsFinite(v))
                    {
                        py = v;
                        break;
                    }

                path.MoveTo(new SKPoint(_scaleFactor * i + _w / 2, -_scaleFactor * py + _h / 2));
                for (i = -_w / 2; i < _w / 2; i += increment)
                {
                    float? pY1 = _dx.Evaluate(_symbols, i), pY2 = _dx.Evaluate(_symbols, i + increment);
                    if (pY1 is float y1 && pY2 is float y2 && !float.IsNaN(y1) && !float.IsNaN(y2) && float.IsFinite(y1) && float.IsFinite(y2))
                    {
                        float coord_y1 = -_scaleFactor * y1 + _h / 2;
                        float coord_y2 = -_scaleFactor * y2 + _h / 2;
                        if (float.IsInfinity(coord_y1) || float.IsInfinity(coord_y2))
                            continue;

                        float x1 = _scaleFactor * i + _w / 2;
                        float x2 = _scaleFactor * (i + increment) + _w / 2;
                        path.QuadTo(x1, coord_y1, x2, coord_y2);
                    }
                }

                _canvas?.DrawPath(path, _graphDxPaint);
                path.Dispose();
            }
        }

        private void DrawRoots()
        {
            if (_showRoots && _expr is not null)
                foreach (SKPoint r in _roots)
                    _canvas?.DrawCircle(r, 5f, _detailsPaint);
        }

        private void DrawExtrema()
        {
            if (_showExtrema && _expr is not null)
                foreach (KeyValuePair<SKPoint, bool> e in _extrema)
                {
                    _canvas?.DrawCircle(e.Key, 5f, _detailsPaint);
                    _canvas?.DrawText(e.Value ? "HP" : "LP", new(e.Key.X + 15, e.Key.Y - 15), _textPaint);
                }
        }

        private void DrawTurnings()
        {
            if (_showTurnings && _expr is not null)
                foreach (SKPoint t in _turningPoints)
                {
                    _canvas?.DrawCircle(t, 5f, _detailsPaint);
                    _canvas?.DrawText("TP", new(t.X + 15, t.Y - 15), _textPaint);
                }
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

        private void Checkbox_Derivative_CheckedChanged(object sender, EventArgs e)
        {
            _showDeriv = Checkbox_Derivative.Checked;
            DrawGraph();
        }

        private void Checkbox_Roots_CheckedChanged_1(object sender, EventArgs e)
        {
            _showRoots = Checkbox_Roots.Checked;
            DrawGraph();
        }

        private void Checkbox_Extrema_CheckedChanged(object sender, EventArgs e)
        {
            _showExtrema = Checkbox_Extrema.Checked;
            DrawGraph();
        }

        private void Checkbox_Turnings_CheckedChanged(object sender, EventArgs e)
        {
            _showTurnings = Checkbox_Turnings.Checked;
            DrawGraph();
        }
    }
}