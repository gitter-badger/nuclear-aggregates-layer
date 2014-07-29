using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<Client> Client = QueryRuleContainer<Client>.Create(
            () => EntityOperationMapping<Client>.ForEntity(EntityName.Client)
                                                .Operation<AssignIdentity>()
                                                .Operation<CreateIdentity>()
                                                .Operation<UpdateIdentity>()
                                                .Operation<DisqualifyIdentity>()
                                                .Operation<QualifyIdentity>()
                                                .Operation<MergeIdentity>()
                                                .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Client>(ids))),

            () => EntityOperationMapping<Client>.ForEntity(EntityName.Firm)
                                                .Operation<ChangeClientIdentity>()
                                                .Operation<UpdateIdentity>()
                                                .Operation<QualifyIdentity>()
                                                .NonCoupledOperation<ImportFirmIdentity>()
                                                .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Firm>(ids))
                                                                            .Where(firm => firm.ClientId != null)
                                                                            .Select(firm => firm.Client)),

            () => EntityOperationMapping<Client>.ForEntity(EntityName.Contact)
                                                .Operation<CreateIdentity>()
                                                .Operation<UpdateIdentity>()
                                                .Operation<DeleteIdentity>()
                                                .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Contact>(ids))
                                                                            .Select(contact => contact.Client)),

            () => EntityOperationMapping<Client>.ForEntity(EntityName.LegalPerson)
                                                .Operation<ActivateIdentity>()
                                                .Operation<CreateIdentity>()
                                                .Operation<ChangeClientIdentity>()
                                                .Operation<DeactivateIdentity>()
                                                .Operation<DeleteIdentity>()
                                                .Operation<MergeIdentity>()
                                                .Use((finder, ids) => finder.Find(Specs.Find.ByIds<LegalPerson>(ids))
                                                                            .Where(person => person.ClientId.HasValue)
                                                                            .Select(person => person.Client)));
    }
}
