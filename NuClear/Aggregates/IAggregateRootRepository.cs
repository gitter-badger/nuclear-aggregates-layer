using NuClear.Model.Common.Entities.Aspects;

namespace NuClear.Aggregates
{
    /// <summary>
    /// ��������� ��������� ��� ������������ (��������������, �������������) ������������ ���������� aggregate root
    /// ����������� ������� aggregate root ��� ��������. 
    /// �.�. ��������� ���� ��������� ������ ������ ������� ���������� ������������� ����������� (���� ����� ����� ����), 
    /// ������� ��������� �������� �������� ��� �������� � ����� ���������, ����� aggregate root
    /// </summary>
    public interface IAggregateRootService<TAggregateRoot> : IAggregateService
        where TAggregateRoot : class, IEntity, IEntityKey
    {
    }
}