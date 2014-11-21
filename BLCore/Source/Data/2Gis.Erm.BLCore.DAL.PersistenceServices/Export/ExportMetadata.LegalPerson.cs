using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPersonProfile;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<LegalPerson> LegalPerson = QueryRuleContainer<LegalPerson>.Create(
            () => EntityOperationMapping<LegalPerson>.ForEntity(EntityName.LegalPerson)
                                                     .Operation<CreateIdentity>()
                                                     .Operation<UpdateIdentity>()
                                                     .Operation<DeactivateIdentity>()
                                                     .Operation<ActivateIdentity>()
                                                     .Operation<DeleteIdentity>()
                                                     .Operation<AssignIdentity>()
                                                     .Operation<ChangeClientIdentity>()
                                                     .NonCoupledOperation<ChangeRequisitesIdentity>()
                                                     .Operation<MergeIdentity>()
                                                     .Use((finder, ids) => finder.Find(Specs.Find.ByIds<LegalPerson>(ids))),

            () => EntityOperationMapping<LegalPerson>.ForEntity(EntityName.LegalPersonProfile)
                                                     .Operation<CreateIdentity>()
                                                     .Operation<UpdateIdentity>()
                                                     .Operation<AssignIdentity>()
                                                     .Operation<DeleteIdentity>()
                                                     .NonCoupledOperation<SetAsMainLegalPersonProfileIdentity>()
                                                     .Use((finder, ids) => finder.Find(Specs.Find.ByIds<LegalPersonProfile>(ids))
                                                                                 .Select(profile => profile.LegalPerson)),

            () => EntityOperationMapping<LegalPerson>.ForEntity(EntityName.Client)
                                                     .Operation<AssignIdentity>()
                                                     .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Client>(ids))
                                                                                 .SelectMany(client => client.LegalPersons)),

            () => EntityOperationMapping<LegalPerson>.ForEntity(EntityName.Account)
                                                     .Operation<CreateIdentity>()
                                                     .Operation<UpdateIdentity>()
                                                     .Operation<DeleteIdentity>()
                                                     .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Account>(ids))
                                                                                 .Select(account => account.LegalPerson)));
    }
}

