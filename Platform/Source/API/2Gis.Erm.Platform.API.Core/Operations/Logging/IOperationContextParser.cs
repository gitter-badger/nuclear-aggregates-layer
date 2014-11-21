namespace DoubleGis.Erm.Platform.API.Core.Operations.Logging
{
    public interface IOperationContextParser
    {
        bool TryParse(string context, out EntityChangesContext changesContext, out string report);
    }
}