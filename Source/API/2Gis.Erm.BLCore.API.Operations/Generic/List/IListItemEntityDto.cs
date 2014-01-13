using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.List
{
    /// <summary>
    /// ��������� ��������� DTO, ������������� � �������� List, ��� ����������� ���� �������� ERM
    /// </summary>
    public interface IListItemEntityDto<TEntity> : IOperationSpecificEntityDto<TEntity>
        where TEntity : IEntityKey
    {
    }
}