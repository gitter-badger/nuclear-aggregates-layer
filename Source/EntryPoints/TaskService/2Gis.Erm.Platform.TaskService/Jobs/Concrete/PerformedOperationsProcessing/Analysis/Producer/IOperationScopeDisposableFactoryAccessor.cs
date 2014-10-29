namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Producer
{
    /// <summary>
    /// ���������� ������� ������, ������������ ��������, �.�. � ��������� ��������� (��������, ��������� performed operations ��� LOAD ������������),
    /// ���������� ���� �������������� �� ������ lifetime ����� operationscopes, �� � operationscopefactories, ����������� ����� ����� ��������� - 
    /// ������������� perscope ���������, � ����� ������ lifetime, ������ scope ����� ���� ����������
    /// </summary>
    public interface IOperationScopeDisposableFactoryAccessor
    {
        IOperationScopeDisposableFactory Factory { get; }
    }
}