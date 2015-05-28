using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
    public static partial class ExportMetadata
    {
        public static readonly QueryRuleContainer<PricePosition> PricePosition = QueryRuleContainer<PricePosition>.Create(
            () => EntityOperationMapping<PricePosition>.ForEntity(EntityType.Instance.PricePosition())
                                                       .Operation<CreateIdentity>()
                                                       .Operation<UpdateIdentity>()
                                                       .Operation<DeactivateIdentity>()
                                                       .Operation<ActivateIdentity>()
                                                       .Operation<DeleteIdentity>()
                                                       .NonCoupledOperation<CopyPricePositionIdentity>()
                                                       .Use((query, ids) => query.For(Specs.Find.ByIds<PricePosition>(ids))));
    }
}