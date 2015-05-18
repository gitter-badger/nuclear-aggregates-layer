using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Core.Metadata.Security
{
    public static partial class OperationSecurityRegistry
    {
        private static readonly IOperationAccessRequirement CreateAdvertisementElement = AccessRequirementBuilder.ForOperation<CreateIdentity, AdvertisementElement>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.AdvertisementElementTemplate())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Advertisement())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Firm())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.AdvertisementTemplate())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.UserProfile())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.File())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.AdvertisementElement())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationAddress())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationEmail())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationEmailTo())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationEmailTo())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.AdvertisementElement()));

        private static readonly IOperationAccessRequirement UpdateAdvertisementElement = AccessRequirementBuilder.ForOperation<UpdateIdentity, AdvertisementElement>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.AdvertisementElementTemplate())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Advertisement())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.Firm())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.AdvertisementTemplate())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.UserProfile())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.File())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.AdvertisementElement())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationAddress())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationEmail())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationEmailTo())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.NotificationEmailTo())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.AdvertisementElement()));

        private static readonly IOperationAccessRequirement CreateAdvertisement = AccessRequirementBuilder.ForOperation<CreateIdentity, Advertisement>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.Advertisement())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.AdvertisementElementTemplate())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.AdsTemplatesAdsElementTemplate())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.Advertisement())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.AdvertisementElement())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.Advertisement()));

        private static readonly IOperationAccessRequirement UpdateAdvertisement = AccessRequirementBuilder.ForOperation<UpdateIdentity, Advertisement>(
            x => x.Require(EntityAccessTypes.Read, EntityType.Instance.Advertisement())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.AdvertisementElementTemplate())
                  .Require(EntityAccessTypes.Read, EntityType.Instance.AdsTemplatesAdsElementTemplate())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.Advertisement())
                  .Require(EntityAccessTypes.Create, EntityType.Instance.AdvertisementElement())
                  .Require(EntityAccessTypes.Update, EntityType.Instance.Advertisement()));
    }
}
