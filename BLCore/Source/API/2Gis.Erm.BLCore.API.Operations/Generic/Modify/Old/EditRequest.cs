using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.Old
{
    public class EditRequest<TEntity> : Request
        where TEntity : IEntityKey
    {
        public TEntity Entity { get; set; }
    }
}
