namespace DoubleGis.Erm.Platform.API.Core.Globalization
{
    public interface IGlobalizationSettings
    {
        string BasicLanguage { get; }
        string ReserveLanguage { get; }
        BusinessModel BusinessModel { get; }
    }
}
