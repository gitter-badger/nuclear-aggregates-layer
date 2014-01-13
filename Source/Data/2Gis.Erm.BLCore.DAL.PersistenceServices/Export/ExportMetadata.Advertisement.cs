using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Advertisement;

namespace DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export
{
    public static partial class ExportMetadata
    {
        [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1115:ParameterMustFollowComma", Justification = "Reviewed. Suppression is OK here.")]
        public static readonly QueryRuleContainer<Advertisement> Advertisement = QueryRuleContainer<Advertisement>.Create(
            () => EntityOperationMapping<Advertisement>.ForEntity(EntityName.Advertisement)
                                                       .Operation<CreateIdentity>()
                                                       .Operation<UpdateIdentity>()
                                                       .Operation<DeleteIdentity>()
                                                       .NonCoupledOperation<SelectAdvertisementToWhitelistIdentity>()
                                                       .Use((finder, ids) => finder.Find(Specs.Find.ByIds<Advertisement>(ids))),

            () => EntityOperationMapping<Advertisement>.ForEntity(EntityName.AdvertisementElement)
                                                       .Operation<CreateIdentity>()
                                                       .Operation<UpdateIdentity>()
                                                       .Use((finder, ids) => finder.Find(Specs.Find.ByIds<AdvertisementElement>(ids))
                                                                                   .Select(element => element.Advertisement)),

            () => EntityOperationMapping<Advertisement>.ForEntity(EntityName.File)
                                                       .Operation<UploadIdentity>()
                                                       .Use((finder, ids) => finder.Find<AdvertisementElement>(element => element.FileId.HasValue &&
                                                                                                                          ids.Contains(element.FileId.Value))
                                                                                   .Select(element => element.Advertisement)),

            () => EntityOperationMapping<Advertisement>.ForEntity(EntityName.AdvertisementTemplate)
                                                       .Operation<CreateIdentity>()
                                                       .Operation<DeleteIdentity>()
                                                       .Use((finder, ids) => finder.Find(Specs.Find.ByIds<AdvertisementTemplate>(ids))
                                                                                   .SelectMany(template => template.Advertisements)),

            () => EntityOperationMapping<Advertisement>.ForEntity(EntityName.AdsTemplatesAdsElementTemplate)
                                                       .Operation<DeleteIdentity>()
                                                       .Operation<CreateIdentity>()
                                                       .Operation<UpdateIdentity>()
                                                       .Use((finder, ids) => finder.Find(Specs.Find.ByIds<AdsTemplatesAdsElementTemplate>(ids))
                                                                                   .SelectMany(template => template.AdvertisementTemplate.Advertisements)));
    }
}
