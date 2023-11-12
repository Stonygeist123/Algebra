using MathShit.Analysis;
using MathShit.Analysis.Lexer;
using MathShit.Analysis.Parser;
using SkiaSharp;

namespace MathShit
{
    public partial class Form1 : Form
    {
        private float scaleFactor = 1f;
        private Expr? expr = null;
        public Form1() => InitializeComponent();
        private void Form1_Load(object sender, EventArgs e) => DrawGraph();
        private void Txt_Fn_TextChanged(object sender, EventArgs e) => Txt_Fn.Text = " " + Txt_Fn.Text.Trim();
        private void Btn_Graph_Click(object sender, EventArgs e)
        {
            Lexer lexer = new(Txt_Fn.Text);
            Token[] tokens = lexer.Lex();
            if (!lexer.Diagnostics.Any())
            {
                Parser parser = new(tokens);
                expr = parser.Parse();
                DrawGraph();
            }
        }

        private void DrawGraph()
        {
            int w = Width - (int)MathF.Floor(Width * .04f), h = Height - 2 * Txt_Fn.Height;
            Img_Graph.Width = w;
            Img_Graph.Height = h;
            Img_Graph.Location = new Point((int)MathF.Floor(Width * .02f), 2 * Txt_Fn.Height);

            SKImageInfo imgInfo = new(w, h);
            using SKSurface surface = SKSurface.Create(imgInfo);
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

            if (expr is not null)
            {
                paint = new()
                {
                    Color = SKColors.DarkCyan,
                    StrokeWidth = 1.5f,
                    IsAntialias = true,
                    IsStroke = true,
                    Style = SKPaintStyle.Stroke
                };
                SKPath path = new();
                float i = -w / 2;
                path.MoveTo(scaleFactor * i + w / 2, -scaleFactor * expr.Evaluate(i) + h / 2);
                const float increment = .01f;
                List<SKPoint> points = new();
                for (; i < w / 2; i += increment)
                {
                    float x1 = scaleFactor * i + w / 2;
                    float y1 = -scaleFactor * expr.Evaluate(i) + h / 2;
                    points.Add(new(x1, y1));
                }

                path.AddPoly(points.ToArray());
                canvas.DrawPath(path, paint);
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
            base.OnMouseWheel(e);
            if (e.Delta > 0)
                scaleFactor *= 2;
            else
                scaleFactor /= 2;
            DrawGraph();
        }
    }
}