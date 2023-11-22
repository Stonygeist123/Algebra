namespace Algebra.Miscellaneous
{
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
