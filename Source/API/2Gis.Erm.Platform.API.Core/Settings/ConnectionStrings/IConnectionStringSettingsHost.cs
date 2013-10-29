namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    public interface IConnectionStringSettingsHost
    {
        ConnectionStringsSettingsAspect ConnectionStrings { get; }
    }
}