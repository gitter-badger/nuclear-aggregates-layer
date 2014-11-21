using System;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Delete;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.Delete
{
    // FIXME {y.baranihin, 09.09.2014}: Удалить. Есть разные сущности, для которых не применима операция удаления,
    // например, PerformedBusinessOperations - но для них подобные заглушки не генерируем.
    public sealed class DeleteLegalPersonOperationService : IDeleteGenericEntityService<LegalPerson>
    {
        public DeleteConfirmation Delete(long entityId)
        {
            throw new NotSupportedException();
        }

        public DeleteConfirmationInfo GetConfirmation(long entityId)
        {
            throw new NotSupportedException();
        }
    }
}