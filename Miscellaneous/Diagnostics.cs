using System.Collections;

namespace MathShit.Miscellaneous
{
    internal class DiagnosticBag : IEnumerable<Diagnostic>
    {
        private readonly List<Diagnostic> _diagnostics = new();
        public Diagnostic[] Diagnostics { get => _diagnostics.ToArray(); }
        public void Add(Diagnostic diagnostic) => _diagnostics.Add(diagnostic);
        public void Add(string message, TextSpan span) => _diagnostics.Add(new(message, span));
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();
    }

    internal readonly struct Diagnostic
    {
        public Diagnostic(string message, TextSpan span)
        {
            Message = message;
            Span = span;
        }

        public string Message { get; }
        public TextSpan Span { get; }
        public override string ToString() => $"[{Span.Start + 1} - {Span.End + 1}]:\t{Message}";
    }
}
