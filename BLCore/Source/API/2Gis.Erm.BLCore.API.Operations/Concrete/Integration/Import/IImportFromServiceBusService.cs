namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import
{
    // FIXME {a.tukaev, 08.05.2014}: �� �������, � ����� ����� ������� ���� ������ ���������. ��� �� ��������?
    public interface IImportFromServiceBusService
    {
        void Import(string flowName);
    }
}