using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    public static partial class OperationSecurityRegistry
    {
        private static readonly IOperationAccessRequirement CalculateReleaseWithdrawals =
            AccessRequirementBuilder.ForOperation<ActualizeOrderReleaseWithdrawalsIdentity>(
                x => x.Require(EntityAccessTypes.Create, EntityName.OrderReleaseTotal)
                      .Require(EntityAccessTypes.Create, EntityName.ReleasesWithdrawalsPosition)
                      .Require(EntityAccessTypes.Create, EntityName.ReleaseWithdrawal)
                      .Require(EntityAccessTypes.Update, EntityName.Order)
                      .Require(EntityAccessTypes.Delete, EntityName.OrderReleaseTotal)
                      .Require(EntityAccessTypes.Delete, EntityName.ReleaseWithdrawal)
                      .Require(EntityAccessTypes.Delete, EntityName.ReleasesWithdrawalsPosition)
                      .Require(EntityAccessTypes.Read, EntityName.BargainType)
                      .Require(EntityAccessTypes.Read, EntityName.BranchOffice)
                      .Require(EntityAccessTypes.Read, EntityName.BranchOfficeOrganizationUnit)
                      .Require(EntityAccessTypes.Read, EntityName.Category)
                      .Require(EntityAccessTypes.Read, EntityName.CategoryFirmAddress)
                      .Require(EntityAccessTypes.Read, EntityName.CategoryOrganizationUnit)
                      .Require(EntityAccessTypes.Read, EntityName.Firm)
                      .Require(EntityAccessTypes.Read, EntityName.FirmAddress)
                      .Require(EntityAccessTypes.Read, EntityName.Lock)
                      .Require(EntityAccessTypes.Read, EntityName.Order)
                      .Require(EntityAccessTypes.Read, EntityName.OrderPosition)
                      .Require(EntityAccessTypes.Read, EntityName.OrderPositionAdvertisement)
                      .Require(EntityAccessTypes.Read, EntityName.OrderReleaseTotal)
                      .Require(EntityAccessTypes.Read, EntityName.OrganizationUnit)
                      .Require(EntityAccessTypes.Read, EntityName.Platform)
                      .Require(EntityAccessTypes.Read, EntityName.Position)
                      .Require(EntityAccessTypes.Read, EntityName.PricePosition)
                      .Require(EntityAccessTypes.Read, EntityName.ReleaseWithdrawal)
                      .Require(EntityAccessTypes.Read, EntityName.ReleasesWithdrawalsPosition)
                      .Require(EntityAccessTypes.Read, EntityName.Theme));

        private static readonly IOperationAccessRequirement CreateOrder = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(
            x => x.Require(EntityAccessTypes.Create, EntityName.Account)
                  .Require(EntityAccessTypes.Create, EntityName.Order)
                  .Require(EntityAccessTypes.Read, EntityName.Account)
                  .Require(EntityAccessTypes.Read, EntityName.BranchOffice)
                  .Require(EntityAccessTypes.Read, EntityName.BranchOfficeOrganizationUnit)
                  .Require(EntityAccessTypes.Read, EntityName.Deal)
                  .Require(EntityAccessTypes.Read, EntityName.LegalPerson)
                  .Require(EntityAccessTypes.Read, EntityName.Lock)
                  .Require(EntityAccessTypes.Read, EntityName.Order)
                  .Require(EntityAccessTypes.Read, EntityName.OrderReleaseTotal)
                  .Require(EntityAccessTypes.Read, EntityName.OrganizationUnit)
                  .Require(EntityAccessTypes.Update, EntityName.Deal));

        private static readonly IOperationAccessRequirement UpdateOrder = AccessRequirementBuilder.ForOperation<UpdateIdentity, Order>(
            x => x.UsesOperation<ActualizeOrderReleaseWithdrawalsIdentity>()
                  .Require(EntityAccessTypes.Create, EntityName.Account)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationAddress)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationEmail)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationEmailTo)
                  .Require(EntityAccessTypes.Read, EntityName.Account)
                  .Require(EntityAccessTypes.Read, EntityName.AccountDetail)
                  .Require(EntityAccessTypes.Read, EntityName.BranchOfficeOrganizationUnit)
                  .Require(EntityAccessTypes.Read, EntityName.Deal)
                  .Require(EntityAccessTypes.Read, EntityName.Firm)
                  .Require(EntityAccessTypes.Read, EntityName.LegalPerson)
                  .Require(EntityAccessTypes.Read, EntityName.Lock)
                  .Require(EntityAccessTypes.Read, EntityName.Note)
                  .Require(EntityAccessTypes.Read, EntityName.Order)
                  .Require(EntityAccessTypes.Read, EntityName.OrderPosition)
                  .Require(EntityAccessTypes.Read, EntityName.OrderReleaseTotal)
                  .Require(EntityAccessTypes.Read, EntityName.OrganizationUnit)
                  .Require(EntityAccessTypes.Read, EntityName.ReleaseInfo)
                  .Require(EntityAccessTypes.Update, EntityName.Deal)
                  .Require(EntityAccessTypes.Update, EntityName.Order)
                  .Require(EntityAccessTypes.Update, EntityName.OrderPosition)
                  .Require(FunctionalPrivilegeName.OrderChangeDocumentsDebt));

        private static readonly IOperationAccessRequirement CreateOrderPosition = AccessRequirementBuilder.ForOperation<CreateIdentity, OrderPosition>(
            x => x.Require(EntityAccessTypes.Read, EntityName.Deal)
                  .Require(EntityAccessTypes.Read, EntityName.FirmAddress)
                  .Require(EntityAccessTypes.Read, EntityName.Lock)
                  .Require(EntityAccessTypes.Read, EntityName.Order)
                  .Require(EntityAccessTypes.Read, EntityName.OrderPosition)
                  .Require(EntityAccessTypes.Read, EntityName.OrderReleaseTotal)
                  .Require(EntityAccessTypes.Read, EntityName.PricePosition)
                  .Require(EntityAccessTypes.Read, EntityName.Position)
                  .Require(EntityAccessTypes.Read, EntityName.Platform)
                  .Require(EntityAccessTypes.Read, EntityName.ReleaseWithdrawal)
                  .Require(EntityAccessTypes.Read, EntityName.ReleasesWithdrawalsPosition)
                  .Require(EntityAccessTypes.Create, EntityName.OrderPosition)
                  .Require(EntityAccessTypes.Create, EntityName.OrderPositionAdvertisement)
                  .Require(EntityAccessTypes.Create, EntityName.ReleaseWithdrawal)
                  .Require(EntityAccessTypes.Create, EntityName.ReleasesWithdrawalsPosition)
                  .Require(EntityAccessTypes.Update, EntityName.Deal)
                  .Require(EntityAccessTypes.Update, EntityName.Order)
                  .Require(EntityAccessTypes.Update, EntityName.OrderPosition)
                  .Require(EntityAccessTypes.Delete, EntityName.OrderPositionAdvertisement)
                  .Require(EntityAccessTypes.Delete, EntityName.OrderReleaseTotal)
                  .Require(EntityAccessTypes.Delete, EntityName.ReleaseWithdrawal)
                  .Require(EntityAccessTypes.Delete, EntityName.ReleasesWithdrawalsPosition));

        private static readonly IOperationAccessRequirement UpdateOrderPosition = AccessRequirementBuilder.ForOperation<UpdateIdentity, OrderPosition>(
            x => x.Require(EntityAccessTypes.Read, EntityName.Deal)
                  .Require(EntityAccessTypes.Read, EntityName.FirmAddress)
                  .Require(EntityAccessTypes.Read, EntityName.Lock)
                  .Require(EntityAccessTypes.Read, EntityName.Order)
                  .Require(EntityAccessTypes.Read, EntityName.OrderPosition)
                  .Require(EntityAccessTypes.Read, EntityName.OrderReleaseTotal)
                  .Require(EntityAccessTypes.Read, EntityName.PricePosition)
                  .Require(EntityAccessTypes.Read, EntityName.Position)
                  .Require(EntityAccessTypes.Read, EntityName.Platform)
                  .Require(EntityAccessTypes.Read, EntityName.ReleaseWithdrawal)
                  .Require(EntityAccessTypes.Read, EntityName.ReleasesWithdrawalsPosition)
                  .Require(EntityAccessTypes.Create, EntityName.OrderPosition)
                  .Require(EntityAccessTypes.Create, EntityName.OrderPositionAdvertisement)
                  .Require(EntityAccessTypes.Create, EntityName.ReleaseWithdrawal)
                  .Require(EntityAccessTypes.Create, EntityName.ReleasesWithdrawalsPosition)
                  .Require(EntityAccessTypes.Update, EntityName.Deal)
                  .Require(EntityAccessTypes.Update, EntityName.Order)
                  .Require(EntityAccessTypes.Update, EntityName.OrderPosition)
                  .Require(EntityAccessTypes.Delete, EntityName.OrderPositionAdvertisement)
                  .Require(EntityAccessTypes.Delete, EntityName.OrderReleaseTotal)
                  .Require(EntityAccessTypes.Delete, EntityName.ReleaseWithdrawal)
                  .Require(EntityAccessTypes.Delete, EntityName.ReleasesWithdrawalsPosition));


    }
}
