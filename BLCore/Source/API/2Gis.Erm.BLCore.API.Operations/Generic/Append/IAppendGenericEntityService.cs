using DoubleGis.Erm.Platform.API.Core.Operations;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Append
{
    public interface IAppendGenericEntityService<TAppended, TParent> : IEntityOperation<TAppended, TParent>, IAppendEntityService
        where TParent : class, IEntityKey 
        where TAppended : class, IEntityKey
    {
    }
}