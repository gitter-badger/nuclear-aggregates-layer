using DoubleGis.Erm.Platform.Common.Crosscutting;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting.AdvertisementElements
{
    /// <summary>
    /// Привести значение аттрибута plain Text в соответсвие с требованиями конечных продуктов cleanup linebreaks (Export does not recognize linebreaks)
    /// </summary>
    public interface IAdvertisementElementPlainTextHarmonizer : IInvariantSafeCrosscuttingService
    {   //
        string Process(string rawPlainText);
    }
}
