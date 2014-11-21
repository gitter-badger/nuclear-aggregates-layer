using System;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations
{
    [Obsolete("IImportCardRelationForErmService is a new version of this operation")]
    public interface IImportCardRelationService : IImportServiceBusDtoService<CardRelationServiceBusDto>, IOperation<ImportCardRelationIdentity>
    {
    }
}