namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import
{
    public interface IImportServiceBusDtoServiceFactory
    {
        bool TryGetServiceBusObjectImportService(string flowName, string busObjectType, out IImportServiceBusDtoService service);
    }
}