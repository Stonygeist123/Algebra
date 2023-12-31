﻿namespace Algebra.Miscellaneous
{
    public readonly struct TextSpan
    {
        public int Start { get; }
        public int Length { get; }
        public readonly int End => Start + Length;
        public TextSpan(int start, int length)
        {
            Start = start;
            Length = length;
        }

        public static TextSpan From(int start, int end) => new(start, end - start + 1);
    }
}