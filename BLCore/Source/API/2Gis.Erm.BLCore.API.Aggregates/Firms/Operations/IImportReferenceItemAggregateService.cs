using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using NuClear.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations
{
    public interface IImportReferenceItemAggregateService : IAggregatePartService<Firm>
    {
        void ImportReferenceItems(IEnumerable<ReferenceItemServiceBusDto> referenceItems);
    }
}