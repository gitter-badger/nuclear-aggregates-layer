using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    public static partial class OperationSecurityRegistry
    {
        private static readonly IOperationAccessRequirement CalculateReleaseWithdrawals =
            AccessRequirementBuilder.ForOperation<ActualizeOrderReleaseWithdrawalsIdentity>(
                x => x.Require(EntityAccessTypes.Create, EntityType.Instance.OrderReleaseTotal())
                      .Require(EntityAccessTypes.Create, EntityType.Instance.ReleasesWithdrawalsPosition())
                      .Require(EntityAccessTypes.Create, EntityType.Instance.ReleaseWithdrawal())
                      .Require(EntityAccessTypes.Update, EntityType.Instance.Order())
                      .Require(EntityAccessTypes.Delete, EntityType.Instance.OrderReleaseTotal())
                      .Require(EntityAccessTypes.Delete, EntityType.Instance.ReleaseWithdrawal())
                      .Require(EntityAccessTypes.Delete, EntityType.Instance.ReleasesWithdrawalsPosition())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.BargainType())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.BranchOffice())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.BranchOfficeOrganizationUnit())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.Category())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.CategoryFirmAddress())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.CategoryOrganizationUnit())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.Firm())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.FirmAddress())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.Lock())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.Order())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.OrderPosition())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.OrderPositionAdvertisement())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.OrderReleaseTotal())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.OrganizationUnit())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.Platform())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.Position())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.PricePosition())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.ReleaseWithdrawal())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.ReleasesWithdrawalsPosition())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.Theme()));

        private static readonly IOperationAccessRequirement CreateOrder = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(
            x => x.Require(EntityAccessTypes.Create, EntityType.Instance.Account())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.Order())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Account())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.BranchOffice())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.BranchOfficeOrganizationUnit())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Deal())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.LegalPerson())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Lock())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Order())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.OrderReleaseTotal())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.OrganizationUnit())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.Deal()));

        private static readonly IOperationAccessRequirement UpdateOrder = AccessRequirementBuilder.ForOperation<UpdateIdentity, Order>(
            x => x.UsesOperation<ActualizeOrderReleaseWithdrawalsIdentity>()
                  .Require(EntityAccessTypes.Create, EntityType.Instance.Account())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationAddress())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationEmail())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationEmailTo())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Account())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.AccountDetail())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.BranchOfficeOrganizationUnit())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Deal())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Firm())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.LegalPerson())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Lock())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Note())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Order())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.OrderPosition())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.OrderReleaseTotal())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.OrganizationUnit())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.ReleaseInfo())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.Deal())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.Order())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.OrderPosition())
                  .Require(FunctionalPrivilegeName.OrderChangeDocumentsDebt));

        private static readonly IOperationAccessRequirement CreateOrderPosition = AccessRequirementBuilder.ForOperation<CreateIdentity, OrderPosition>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.Deal())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.FirmAddress())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Lock())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Order())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.OrderPosition())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.OrderReleaseTotal())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.PricePosition())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Position())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Platform())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.ReleaseWithdrawal())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.ReleasesWithdrawalsPosition())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.OrderPosition())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.OrderPositionAdvertisement())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.ReleaseWithdrawal())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.ReleasesWithdrawalsPosition())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.Deal())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.Order())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.OrderPosition())
                  .Require(EntityAccessTypes.Delete, EntityType.Instance.OrderPositionAdvertisement())
                  .Require(EntityAccessTypes.Delete, EntityType.Instance.OrderReleaseTotal())
                  .Require(EntityAccessTypes.Delete, EntityType.Instance.ReleaseWithdrawal())
                  .Require(EntityAccessTypes.Delete, EntityType.Instance.ReleasesWithdrawalsPosition()));

        private static readonly IOperationAccessRequirement UpdateOrderPosition = AccessRequirementBuilder.ForOperation<UpdateIdentity, OrderPosition>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.Deal())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.FirmAddress())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Lock())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Order())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.OrderPosition())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.OrderReleaseTotal())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.PricePosition())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Position())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Platform())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.ReleaseWithdrawal())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.ReleasesWithdrawalsPosition())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.OrderPosition())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.OrderPositionAdvertisement())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.ReleaseWithdrawal())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.ReleasesWithdrawalsPosition())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.Deal())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.Order())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.OrderPosition())
                  .Require(EntityAccessTypes.Delete, EntityType.Instance.OrderPositionAdvertisement())
                  .Require(EntityAccessTypes.Delete, EntityType.Instance.OrderReleaseTotal())
                  .Require(EntityAccessTypes.Delete, EntityType.Instance.ReleaseWithdrawal())
                  .Require(EntityAccessTypes.Delete, EntityType.Instance.ReleasesWithdrawalsPosition()));


    }
}
