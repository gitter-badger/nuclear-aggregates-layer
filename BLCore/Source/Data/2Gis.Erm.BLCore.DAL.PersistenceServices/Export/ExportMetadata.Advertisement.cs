using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Advertisement;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<Advertisement> Advertisement = QueryRuleContainer<Advertisement>.Create(
            () => EntityOperationMapping<Advertisement>.ForEntity(EntityType.Instance.Advertisement())
                                                       .Operation<CreateIdentity>()
                                                       .Operation<UpdateIdentity>()
                                                       .Operation<DeleteIdentity>()
                                                       .NonCoupledOperation<SelectAdvertisementToWhitelistIdentity>()
                                                       .Use((query, ids) => query.For(Specs.Find.ByIds<Advertisement>(ids))),

            () => EntityOperationMapping<Advertisement>.ForEntity(EntityType.Instance.AdvertisementElement())
                                                       .Operation<CreateIdentity>()
                                                       .Operation<UpdateIdentity>()
                                                       .Operation<UploadIdentity>()
                                                       .Use((query, ids) => query.For(Specs.Find.ByIds<AdvertisementElement>(ids))
                                                                                   .Select(element => element.Advertisement)),

            () => EntityOperationMapping<Advertisement>.ForEntity(EntityType.Instance.AdvertisementTemplate())
                                                       .Operation<CreateIdentity>()
                                                       .Operation<DeleteIdentity>()
                                                       .Use((query, ids) => query.For(Specs.Find.ByIds<AdvertisementTemplate>(ids))
                                                                                   .SelectMany(template => template.Advertisements)),

            () => EntityOperationMapping<Advertisement>.ForEntity(EntityType.Instance.AdsTemplatesAdsElementTemplate())
                                                       .Operation<DeleteIdentity>()
                                                       .Operation<CreateIdentity>()
                                                       .Operation<UpdateIdentity>()
                                                       .Use((query, ids) => query.For(Specs.Find.ByIds<AdsTemplatesAdsElementTemplate>(ids))
                                                                                   .SelectMany(template => template.AdvertisementTemplate.Advertisements)),

            () => EntityOperationMapping<Advertisement>.ForEntity(EntityType.Instance.AdvertisementElementStatus())
                                                       .NonCoupledOperation<ChangeAdvertisementElementStatusIdentity>()
                                                       .NonCoupledOperation<ResetAdvertisementElementToDraftIdentity>()
                                                       .NonCoupledOperation<TransferAdvertisementElementToReadyForValidationIdentity>()
                                                       .NonCoupledOperation<ApproveAdvertisementElementIdentity>()
                                                       .NonCoupledOperation<DenyAdvertisementElementIdentity>()
                                                       .Use((query, ids) => query.For(Specs.Find.ByIds<AdvertisementElement>(ids))
                                                                                   .Select(element => element.Advertisement)));
    }
}