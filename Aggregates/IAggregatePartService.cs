using System;

using NuClear.Model.Common.Entities.Aspects;

namespace NuClear.Aggregates
{
    /// <summary>
    /// ��������� ��������� ��� ������������ (��������������, �������������) ������������ ������ ��������� �������� ��������
    /// �.�. ��������� ���� ��������� ������ ���������� ������������ ����� ��������� �������� �������� ��� �����-�� ���������� �������� - ��������� ����� ��������
    /// </summary>
    [Obsolete("Use non-generic interface marked with IAggregateSpecificOperation")]
    public interface IAggregatePartService<TAggregateRoot> : IAggregateService
        where TAggregateRoot : class, IEntity, IEntityKey
    {
    }
}