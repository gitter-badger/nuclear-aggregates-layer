using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.Model.Aggregates
{
    /// <summary>
    /// ��������� ��������� ��� ������������ (��������������, �������������) ������������ ������ ��������� �������� ��������
    /// �.�. ��������� ���� ��������� ������ ���������� ������������ ����� ��������� �������� �������� ��� �����-�� ���������� �������� - ��������� ����� ��������
    /// </summary>
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IAggregatePartRepository<TAggregateRoot> : IAggregateRepository
        where TAggregateRoot : class, IEntity, IEntityKey
    {
    }
}