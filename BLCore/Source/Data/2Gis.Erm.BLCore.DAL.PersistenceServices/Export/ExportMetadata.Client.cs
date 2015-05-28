using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
        public static readonly QueryRuleContainer<Client> Client = QueryRuleContainer<Client>.Create(
            () => EntityOperationMapping<Client>.ForEntity(EntityType.Instance.Client())
                                                .Operation<AssignIdentity>()
                                                .Operation<CreateIdentity>()
                                                .Operation<UpdateIdentity>()
                                                .Operation<DisqualifyIdentity>()
                                                .Operation<QualifyIdentity>()
                                                .Operation<MergeIdentity>()
                                                .Use((query, ids) => query.For(Specs.Find.ByIds<Client>(ids))),

            () => EntityOperationMapping<Client>.ForEntity(EntityType.Instance.Firm())
                                                .Operation<ChangeClientIdentity>()
                                                .Operation<UpdateIdentity>()
                                                .Operation<QualifyIdentity>()
                                                .NonCoupledOperation<ImportFirmIdentity>()
                                                .Use((query, ids) => query.For(Specs.Find.ByIds<Firm>(ids))
                                                                            .Where(firm => firm.ClientId != null)
                                                                            .Select(firm => firm.Client)),

            () => EntityOperationMapping<Client>.ForEntity(EntityType.Instance.Contact())
                                                .Operation<CreateIdentity>()
                                                .Operation<UpdateIdentity>()
                                                .Operation<DeleteIdentity>()
                                                .Use((query, ids) => query.For(Specs.Find.ByIds<Contact>(ids))
                                                                            .Select(contact => contact.Client)),

            () => EntityOperationMapping<Client>.ForEntity(EntityType.Instance.LegalPerson())
                                                .Operation<ActivateIdentity>()
                                                .Operation<CreateIdentity>()
                                                .Operation<ChangeClientIdentity>()
                                                .Operation<DeactivateIdentity>()
                                                .Operation<DeleteIdentity>()
                                                .Operation<MergeIdentity>()
                                                .Use((query, ids) => query.For(Specs.Find.ByIds<LegalPerson>(ids))
                                                                            .Where(person => person.ClientId.HasValue)
                                                                            .Select(person => person.Client)));
    }
}
