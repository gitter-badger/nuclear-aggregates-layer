using System;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    public sealed class DirhamInEnglishPluralizer : IWordPluralizer
    {
        public string GetPluralFor(int numberToRead)
        {
            return Math.Abs(numberToRead) == 1 ? "dirham" : "dirhams";
        }
    }
}