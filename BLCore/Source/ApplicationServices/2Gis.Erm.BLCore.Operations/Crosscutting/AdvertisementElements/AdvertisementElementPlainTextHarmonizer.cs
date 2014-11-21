using System;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting.AdvertisementElements
{
    public sealed class AdvertisementElementPlainTextHarmonizer : IAdvertisementElementPlainTextHarmonizer
    {
        public string Process(string rawPlainText)
        {
            // cleanup linebreaks (Export does not recognize linebreaks)
            return !string.IsNullOrEmpty(rawPlainText)
                       ? rawPlainText.Replace(Environment.NewLine, "\n")
                                     .Replace((char)160, ' ')
                       : rawPlainText;
        }
    }
}