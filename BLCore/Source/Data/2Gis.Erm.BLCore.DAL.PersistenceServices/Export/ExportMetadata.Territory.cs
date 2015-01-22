using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<Territory> Territory = QueryRuleContainer<Territory>.Create(
            () => EntityOperationMapping<Territory>.ForEntity(EntityType.Instance.Territory())
                                                     .Operation<CreateIdentity>()
                                                     .Operation<UpdateIdentity>()
                                                     .Operation<DeactivateIdentity>()
                                                     .Operation<SpecifyAdditionalServicesIdentity>()
                                                     .NonCoupledOperation<ImportTerritoriesIdentity>()
                                                     .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Territory>(ids))));
    }
}
