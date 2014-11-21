using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    public static partial class OperationSecurityRegistry
    {
        private static readonly IOperationAccessRequirement CreateAdvertisementElement = AccessRequirementBuilder.ForOperation<CreateIdentity, AdvertisementElement>(
            x => x.Require(EntityAccessTypes.Read, EntityName.AdvertisementElementTemplate)
                  .Require(EntityAccessTypes.Read, EntityName.Advertisement)
                  .Require(EntityAccessTypes.Read, EntityName.Firm)
                  .Require(EntityAccessTypes.Read, EntityName.AdvertisementTemplate)
                  .Require(EntityAccessTypes.Read, EntityName.UserProfile)
                  .Require(EntityAccessTypes.Read, EntityName.File)
                  .Require(EntityAccessTypes.Create, EntityName.AdvertisementElement)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationAddress)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationEmail)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationEmailTo)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationEmailTo)
                  .Require(EntityAccessTypes.Update, EntityName.AdvertisementElement));

        private static readonly IOperationAccessRequirement UpdateAdvertisementElement = AccessRequirementBuilder.ForOperation<UpdateIdentity, AdvertisementElement>(
            x => x.Require(EntityAccessTypes.Read, EntityName.AdvertisementElementTemplate)
                  .Require(EntityAccessTypes.Read, EntityName.Advertisement)
                  .Require(EntityAccessTypes.Read, EntityName.Firm)
                  .Require(EntityAccessTypes.Read, EntityName.AdvertisementTemplate)
                  .Require(EntityAccessTypes.Read, EntityName.UserProfile)
                  .Require(EntityAccessTypes.Read, EntityName.File)
                  .Require(EntityAccessTypes.Create, EntityName.AdvertisementElement)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationAddress)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationEmail)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationEmailTo)
                  .Require(EntityAccessTypes.Create, EntityName.NotificationEmailTo)
                  .Require(EntityAccessTypes.Update, EntityName.AdvertisementElement));

        private static readonly IOperationAccessRequirement CreateAdvertisement = AccessRequirementBuilder.ForOperation<CreateIdentity, Advertisement>(
            x => x.Require(EntityAccessTypes.Read, EntityName.Advertisement)
                  .Require(EntityAccessTypes.Read, EntityName.AdvertisementElementTemplate)
                  .Require(EntityAccessTypes.Read, EntityName.AdsTemplatesAdsElementTemplate)
                  .Require(EntityAccessTypes.Create, EntityName.Advertisement)
                  .Require(EntityAccessTypes.Create, EntityName.AdvertisementElement)
                  .Require(EntityAccessTypes.Update, EntityName.Advertisement));

        private static readonly IOperationAccessRequirement UpdateAdvertisement = AccessRequirementBuilder.ForOperation<UpdateIdentity, Advertisement>(
            x => x.Require(EntityAccessTypes.Read, EntityName.Advertisement)
                  .Require(EntityAccessTypes.Read, EntityName.AdvertisementElementTemplate)
                  .Require(EntityAccessTypes.Read, EntityName.AdsTemplatesAdsElementTemplate)
                  .Require(EntityAccessTypes.Create, EntityName.Advertisement)
                  .Require(EntityAccessTypes.Create, EntityName.AdvertisementElement)
                  .Require(EntityAccessTypes.Update, EntityName.Advertisement));
    }
}
