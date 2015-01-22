using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Assign
{
    public interface IGroupAssignEntityService
    {
        IEnumerable<AssignResult> Assign(AssignCommonParameter operationParameter, IEnumerable<AssignEntityParameter> operationItemParameters);
    }

    /// TODO {all, 23.07.2013}: пока не расширяем базовый вариант genric assign entityspecific service, а используем отдельный - нужно решить как выстраивать иерархию интерфейсов, с учетом наличия/отсутсвия групповых обработок
    public interface IGroupAssignGenericEntityService<TEntity> : 
        IEntityOperation<TEntity>, IAssignEntityService, // IAssignGenericEntityService<TEntity>, пока не расширяем базовый вариант genric assign entityspecific service, а используем отдельный
        IGroupAssignEntityService
        where TEntity : class, IEntityKey, ICuratedEntity
    {
    }
}