namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import
{
    // FIXME {a.tukaev, 08.05.2014}: Не понятно, к какой части системы этот сервис относится. Это же операция?
    public interface IImportFromServiceBusService
    {
        void Import(string flowName);
    }
}