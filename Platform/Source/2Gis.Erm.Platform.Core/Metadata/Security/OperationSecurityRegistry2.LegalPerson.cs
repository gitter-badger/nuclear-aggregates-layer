using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    public static partial class OperationSecurityRegistry
    {
        private static readonly IOperationAccessRequirement CreateLegalPersonProfile = AccessRequirementBuilder.ForOperation<CreateIdentity, LegalPersonProfile>(
            x => x.Require(EntityAccessTypes.Read, EntityName.LegalPerson)
                  .Require(EntityAccessTypes.Read, EntityName.LegalPersonProfile)
                  .Require(EntityAccessTypes.Create, EntityName.LegalPersonProfile)
                  .Require(EntityAccessTypes.Update, EntityName.LegalPersonProfile));

        private static readonly IOperationAccessRequirement UpdateLegalPersonProfile = AccessRequirementBuilder.ForOperation<UpdateIdentity, LegalPersonProfile>(
            x => x.Require(EntityAccessTypes.Read, EntityName.LegalPerson)
                  .Require(EntityAccessTypes.Read, EntityName.LegalPersonProfile)
                  .Require(EntityAccessTypes.Create, EntityName.LegalPersonProfile)
                  .Require(EntityAccessTypes.Update, EntityName.LegalPersonProfile));

        private static readonly IOperationAccessRequirement ChangeRequisites = AccessRequirementBuilder.ForOperation<ChangeRequisitesIdentity>(
            x => x.Require(EntityAccessTypes.Read, EntityName.LegalPerson)
                  .Require(EntityAccessTypes.Update, EntityName.LegalPerson)
                  .Require(FunctionalPrivilegeName.LegalPersonChangeRequisites));

        private static readonly IOperationAccessRequirement CreateLegalPerson = AccessRequirementBuilder.ForOperation<CreateIdentity, LegalPerson>(
            x => x.Require(EntityAccessTypes.Read, EntityName.LegalPerson)
                  .Require(EntityAccessTypes.Update, EntityName.LegalPerson));

        private static readonly IOperationAccessRequirement UpdateLegalPerson = AccessRequirementBuilder.ForOperation<UpdateIdentity, LegalPerson>(
            x => x.Require(EntityAccessTypes.Read, EntityName.LegalPerson)
                  .Require(EntityAccessTypes.Update, EntityName.LegalPerson));
    }
}
