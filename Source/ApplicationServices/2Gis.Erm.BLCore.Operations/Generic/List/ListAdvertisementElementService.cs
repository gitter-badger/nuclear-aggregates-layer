using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List.DTO;
using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Operations.Generic.List.Infrastructure;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.List
{
    public class ListAdvertisementElementService : ListEntityDtoServiceBase<AdvertisementElement, ListAdvertisementElementDto>
    {
        public ListAdvertisementElementService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListAdvertisementElementDto> GetListData(IQueryable<AdvertisementElement> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .ApplyQuerySettings(querySettings, out count)
                .Select(item => new
                                    {
                                        item.Id,
                                        AdvertisementElementTemplateId = item.AdvertisementElementTemplate.Id,
                                        AdvertisementElementTemplateName = item.AdvertisementElementTemplate.Name,
                                        item.AdvertisementElementTemplate.RestrictionType,
                                        item.AdvertisementElementTemplate.IsRequired
                                    })
                .AsEnumerable()
                .Select(x =>
                        new ListAdvertisementElementDto
                            {
                                Id = x.Id,
                                AdvertisementElementTemplateId = x.AdvertisementElementTemplateId,
                                AdvertisementElementTemplateName = x.AdvertisementElementTemplateName,
                                IsRequired = x.IsRequired,
                                RestrictionType = ((AdvertisementElementRestrictionType)x.RestrictionType).ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo)
                            });
        }
    }
}