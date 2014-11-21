using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.Platform.API.Core.Operations
{
    /// <summary>
    /// ��������� ��������� ��� ���� ��������������� �������� Erm �����������, ����������� ������� ��� ��������� ������� ��������� ERM  - ��� �������� ��������� ��������� applicationservices,
    /// ��������� ������� ����� ������������ �������� �������
    /// �.�. ���������� ���������� �������� ����� ��������/������������ �� �� ������ ���� ��������, � ����� �� ����������, �������� �������� append.
    /// ������� ����������� ��������: 
    ///  - activate(client)
    ///  - append(Role->User)
    /// </summary>
    public interface IEntityOperation : IOperation
    {
    }

    /// <summary>
    /// ��������� ��������� ��� ��������, ���������� ���������� ������� ���������� ��� ������ ����������� ���� ��������
    /// </summary>
    /// <typeparam name="TEntity">��� �������� ��� ������� ����������� ��������</typeparam>
    public interface IEntityOperation<TEntity> : IEntityOperation
        where TEntity : IEntityKey
    {
    }

    /// <summary>
    /// ��������� ��������� ��� ��������, ���������� ���������� ������� ���������� ��� ���� ���������� ����� ���������
    /// </summary>
    /// <typeparam name="TEntity1">��� �������� 1��, ���������� ��� ������� ����������� ��������</typeparam>
    /// <typeparam name="TEntity2">��� �������� 2��, ���������� ��� ������� ����������� ��������</typeparam>
    public interface IEntityOperation<TEntity1, TEntity2> : IEntityOperation
        where TEntity1 : IEntityKey
        where TEntity2 : IEntityKey
    {
    }
}