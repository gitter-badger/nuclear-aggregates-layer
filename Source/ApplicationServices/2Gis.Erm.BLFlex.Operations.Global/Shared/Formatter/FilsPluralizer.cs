using System;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Shared.Formatter
{
    // Fils - арабские копейки. Множественное число (согласно википедии) - fulūs
    public sealed class FilsPluralizer : IWordPluralizer 
    {
        public string GetPluralFor(int numberToRead)
        {
            return Math.Abs(numberToRead) == 1 ? "fils" : "fulūs";
        }
    }
}