using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Aggregates
{
    /// <summary>
    /// ��������� ��������� ��� ������������ (��������������, �������������) ������������ ������ ��������� �������� ��������
    /// �.�. ��������� ���� ��������� ������ ���������� ������������ ����� ��������� �������� �������� ��� �����-�� ���������� �������� - ��������� ����� ��������
    /// </summary>
    public interface IAggregatePartRepository<TAggregateRoot> : IAggregateRepository
        where TAggregateRoot : class, IEntity, IEntityKey
    {
    }
}