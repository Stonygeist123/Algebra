using MathShit.Syntax.Lexer;
using MathShit.Syntax.Parser;
using SkiaSharp;

namespace MathShit
{
    public partial class Form1 : Form
    {
        private float _scaleFactor = 40f;
        private Expr? _expr = null, _dx = null;
        private bool _hasError = false;
        private const float increment = .01f, graphWidth = 1.5f;
        public Form1() => InitializeComponent();
        private void Form1_Load(object sender, EventArgs e)
        {
            Img_Graph.SizeMode = PictureBoxSizeMode.Zoom;
            DrawGraph();
        }

        private void Btn_Graph_Click(object sender, EventArgs e)
        {
            Analyse();
            if (!_hasError)
                DrawGraph();
        }

        private void Analyse()
        {
            Lexer lexer = new(Txt_Fn.Text);
            Token[] tokens = lexer.Lex();
            if (!(_hasError = lexer.Diagnostics.Any()))
            {
                Parser parser = new(tokens);
                _expr = parser.Parse().Simplify();
                _dx = _expr.Derivative();
                Txt_Dx.Text = "f'(x) = " + _dx?.ToString() ?? "";
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
            SKPaint paint = new()
            {
                Color = SKColors.Black,
                StrokeWidth = 4,
                IsAntialias = true
            };

            canvas.DrawLine(new(0, h / 2), new(w, h / 2), paint);
            canvas.Save();
            canvas.DrawLine(new(w / 2, 0), new(w / 2, h), paint);
            canvas.Save();

            if (!_hasError && _expr is not null)
            {
                paint = new()
                {
                    Color = SKColors.DarkCyan,
                    StrokeWidth = graphWidth,
                    IsAntialias = true,
                    IsStroke = true,
                    Style = SKPaintStyle.Stroke
                };
                SKPath path = new();
                float i = -w / 2;
                float py = h / 2;
                for (; i < w / 2; i += increment)
                    if (_expr.Evaluate(i) is float v)
                    {
                        py = v;
                        break;
                    }

                path.MoveTo(new SKPoint(_scaleFactor * i + w / 2, -_scaleFactor * py + h / 2));
                for (i = -w / 2; i < w / 2; i += increment)
                {
                    float? p_y = _expr.Evaluate(i), p_y1 = _expr.Evaluate(i + increment);
                    if (p_y is float y && p_y1 is float y1)
                    {
                        float coord_y = -_scaleFactor * y + h / 2;
                        float coord_y1 = -_scaleFactor * y1 + h / 2;
                        if (coord_y == float.NegativeInfinity || coord_y1 == float.NegativeInfinity ||
                            coord_y == float.PositiveInfinity || coord_y1 == float.PositiveInfinity)
                            continue;

                        float x1 = _scaleFactor * i + w / 2;
                        float x2 = _scaleFactor * (i + increment) + w / 2;
                        path.QuadTo(x1, coord_y, x2, coord_y1);
                    }
                }

                canvas.DrawPath(path, paint);
                path.Dispose();

                paint = new()
                {
                    Color = SKColors.Red,
                    StrokeWidth = graphWidth,
                    IsAntialias = true,
                    IsStroke = true,
                    Style = SKPaintStyle.Stroke
                };
                if (_dx is not null)
                {
                    path = new();
                    i = -w / 2;
                    py = h / 2;
                    for (; i < w / 2; i += increment)
                        if (_dx.Evaluate(i) is float v)
                        {
                            py = v;
                            break;
                        }

                    path.MoveTo(new SKPoint(_scaleFactor * i + w / 2, -_scaleFactor * py + h / 2));
                    for (i = -w / 2; i < w / 2; i += increment)
                    {
                        float? p_y = _dx.Evaluate(i), p_y1 = _dx.Evaluate(i + increment);
                        if (p_y is float y && p_y1 is float y1)
                        {
                            float coord_y = -_scaleFactor * y + h / 2;
                            float coord_y1 = -_scaleFactor * y1 + h / 2;
                            if (coord_y == float.NegativeInfinity || coord_y1 == float.NegativeInfinity ||
                                coord_y == float.PositiveInfinity || coord_y1 == float.PositiveInfinity)
                                continue;

                            float x1 = _scaleFactor * i + w / 2;
                            float x2 = _scaleFactor * (i + increment) + w / 2;
                            path.QuadTo(x1, coord_y, x2, coord_y1);
                        }
                    }
                }

                canvas.DrawPath(path, paint);
                path.Dispose();
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