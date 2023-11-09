using MathShit.Miscellaneous;

namespace MathShit.Analysis
{
    public enum TokenKind
    {
        Number, Parameter,

        Plus, Minus, Star, Slash, LParen, RParen,

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
        public override string ToString() => $"Kind:   {Kind}\nLexeme: {Lexeme}\nSpan:   [{Span.Start}..{Span.End}]";
    }
}