using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    public static partial class OperationSecurityRegistry
    {
        private static readonly IOperationAccessRequirement CreateLegalPersonProfile = AccessRequirementBuilder.ForOperation<CreateIdentity, LegalPersonProfile>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.LegalPerson())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.LegalPersonProfile())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.LegalPersonProfile())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.LegalPersonProfile()));

        private static readonly IOperationAccessRequirement UpdateLegalPersonProfile = AccessRequirementBuilder.ForOperation<UpdateIdentity, LegalPersonProfile>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.LegalPerson())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.LegalPersonProfile())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.LegalPersonProfile())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.LegalPersonProfile()));

        private static readonly IOperationAccessRequirement ChangeRequisites = AccessRequirementBuilder.ForOperation<ChangeRequisitesIdentity>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.LegalPerson())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.LegalPerson())
                  .Require(FunctionalPrivilegeName.LegalPersonChangeRequisites));

        private static readonly IOperationAccessRequirement CreateLegalPerson = AccessRequirementBuilder.ForOperation<CreateIdentity, LegalPerson>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.LegalPerson())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.LegalPerson()));

        private static readonly IOperationAccessRequirement UpdateLegalPerson = AccessRequirementBuilder.ForOperation<UpdateIdentity, LegalPerson>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.LegalPerson())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.LegalPerson()));
    }
}
