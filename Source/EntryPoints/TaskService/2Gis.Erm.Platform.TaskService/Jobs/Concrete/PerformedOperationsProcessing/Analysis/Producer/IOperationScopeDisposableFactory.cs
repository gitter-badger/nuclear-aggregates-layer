using System;

using DoubleGis.Erm.Platform.API.Core.Operations.Logging;

namespace DoubleGis.Erm.Platform.TaskService.Jobs.Concrete.PerformedOperationsProcessing.Analysis.Producer
{
    /// <summary>
    /// ���������� ���� ��������� ����� ���������� ����� ������� �������� ��� IOperationScopeFactory - ��� ��� ���������� ��������� dispose,
    /// � ��������� ��������� ��� �����, �.�. � ����� ������ lifetime ��������� ������� perscope ���������, ��� ���� scope ����� ���� ���������
    /// </summary>
    public interface IOperationScopeDisposableFactory : IOperationScopeFactory, IDisposable
    {
    }
}