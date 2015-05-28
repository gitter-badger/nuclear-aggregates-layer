using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        public static readonly QueryRuleContainer<DenialReason> DenialReason = QueryRuleContainer<DenialReason>.Create(
            () => EntityOperationMapping<DenialReason>.ForEntity(EntityType.Instance.DenialReason())
                                                      .Operation<CreateIdentity>()
                                                      .Operation<UpdateIdentity>()
                                                      .Operation<DeactivateIdentity>()
                                                      .Use((query, ids) => query.For(Specs.Find.ByIds<DenialReason>(ids))));
    }
}