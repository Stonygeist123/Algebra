using MathShit.Miscellaneous;

namespace MathShit.Analysis
{
    public enum TokenKind
    {
        Number, Name,

        Plus, Minus, Star, Slash, Power,
        LParen, RParen,

        Bad, EOF
    }

    public class Token
    {
        public TokenKind Kind { get; }
        public string Lexeme { get; }
        public TextSpan Span { get; }
        public Token(TokenKind kind, string lexeme, TextSpan span)
        {
            Kind = kind;
            Lexeme = lexeme;
            Span = span;
        }
        public override string ToString() => $"Kind:   {Kind}\tLexeme: {Lexeme}\tSpan:   [{Span.Start}..{Span.End}]";
    }
}