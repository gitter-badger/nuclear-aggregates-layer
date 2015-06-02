using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Delete
{
    public sealed class DeleteBillOperationService : IDeleteGenericEntityService<Bill>
    {
        public DeleteConfirmation Delete(long entityId)
        {
            throw new System.NotSupportedException("Bulk bill delete operation supported only");
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            throw new System.NotSupportedException("Bulk bill delete operation supported only");
        }
    }
}
