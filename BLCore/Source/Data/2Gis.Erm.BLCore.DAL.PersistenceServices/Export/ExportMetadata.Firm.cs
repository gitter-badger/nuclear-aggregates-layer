using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Building;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<Firm> Firm = QueryRuleContainer<Firm>.Create(
            () => EntityOperationMapping<Firm>.ForEntity(EntityName.Firm)
                                                     .Operation<CreateIdentity>()
                                                     .Operation<UpdateIdentity>()
                                                     .Operation<DeleteIdentity>()
                                                     .Operation<AssignIdentity>()
                                                     .Operation<SpecifyAdditionalServicesIdentity>()
                                                     .Operation<ChangeClientIdentity>()
                                                     .Operation<ChangeTerritoryIdentity>()
                                                     .Operation<QualifyIdentity>()
                                                     .Operation<DisqualifyIdentity>()
                                                     .NonCoupledOperation<ImportCardForErmIdentity>()
                                                     .NonCoupledOperation<ImportCardIdentity>()
                                                     .NonCoupledOperation<ImportFirmIdentity>()
                                                     .NonCoupledOperation<ImportFirmPromisingIdentity>()
                                                     .NonCoupledOperation<ImportBuildingIdentity>()// UpdateIdentity/Building
                                                     .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Firm>(ids))));
    }
}
