using NuClear.Model.Common.Entities.Aspects;

namespace NuClear.Aggregates
{
    /// <summary>
    /// ��������� ��������� ��� ������������ (��������������, �������������) ������������ ���������� aggregate root
    /// ����������� ������� aggregate root ��� ��������. 
    /// �.�. ��������� ���� ��������� ������ ������ ������� ���������� ������������� ����������� (���� ����� ����� ����), 
    /// ������� ��������� �������� �������� ��� �������� � ����� ���������, ����� aggregate root
    /// </summary>
    public interface IAggregateRootRepository<TAggregateRoot> : IAggregateRepository
        where TAggregateRoot : class, IEntity, IEntityKey
    {
    }
}