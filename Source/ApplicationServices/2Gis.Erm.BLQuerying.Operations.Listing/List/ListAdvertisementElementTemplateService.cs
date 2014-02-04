using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Metadata;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementElementTemplateService : ListEntityDtoServiceBase<AdvertisementElementTemplate, ListAdvertisementElementTemplateDto>
    {
        public ListAdvertisementElementTemplateService(
            IQuerySettingsProvider querySettingsProvider, 
            IFinderBaseProvider finderBaseProvider,
            IFinder finder,
            IUserContext userContext)
            : base(querySettingsProvider, finderBaseProvider, finder, userContext)
        {
        }

        protected override IEnumerable<ListAdvertisementElementTemplateDto> GetListData(IQueryable<AdvertisementElementTemplate> query, QuerySettings querySettings, ListFilterManager filterManager, out int count)
        {
            return query
                .ApplyQuerySettings(querySettings, out count)
                .Select(x => new
                    {
                        x.Id,
                        x.Name,
                        RestrictionType = (AdvertisementElementRestrictionType)x.RestrictionType
                    })
                .AsEnumerable()
                .Select(x =>
                        new ListAdvertisementElementTemplateDto
                            { 
                                Id = x.Id, 
                                Name = x.Name,
                                RestrictionType = x.RestrictionType.ToStringLocalized(EnumResources.ResourceManager, UserContext.Profile.UserLocaleInfo.UserCultureInfo) 
                            });
        }
    }
}