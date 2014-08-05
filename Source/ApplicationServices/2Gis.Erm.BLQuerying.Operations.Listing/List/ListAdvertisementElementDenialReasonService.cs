using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLQuerying.Operations.Listing.List
{
    public sealed class ListAdvertisementElementDenialReasonService :
        ListEntityDtoServiceBase<AdvertisementElementDenialReason, ListAdvertisementElementDenialReasonsDto>
    {
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;
        private readonly IUserContext _userContext;

        public ListAdvertisementElementDenialReasonService(
            IFinder finder,
            FilterHelper filterHelper,
            IUserContext userContext)
        {
            _finder = finder;
            _filterHelper = filterHelper;
            _userContext = userContext;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            switch (querySettings.ParentEntityName)
            {
                case EntityName.AdvertisementElement:
                    return ListCheckedReasons(querySettings);
                case EntityName.AdvertisementElementStatus:
                    return ListPossibleReasons(querySettings);
                default:
                    return new RemoteCollection<object>(new object[0], 0);
            }
        }

        private IRemoteCollection ListCheckedReasons(QuerySettings querySettings)
        {
            var query = _finder.FindAll<AdvertisementElementDenialReason>();

            var result = query
                .Select(x => new ListAdvertisementElementDenialReasonsDto
                    {
                        Id = x.Id,
                        AdvertisementElementId = x.AdvertisementElementId,
                        DenialReasonId = x.DenialReasonId,
                        DenialReasonName = x.DenialReason.Name,
                        DenialReasonTypeEnum = (DenialReasonType)x.DenialReason.Type,
                        Comment = x.Comment,
                        Checked = true,
                        IsActive = x.DenialReason.IsActive
                    })
                .QuerySettings(_filterHelper, querySettings)
                .Transform(x =>
                    {
                        x.DenialReasonType = x.DenialReasonTypeEnum.ToStringLocalized(EnumResources.ResourceManager,
                                                                                      _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                        return x;
                    });

            // Хак для тикета ERM-4669. После перевода на Эластик таких проблем не будет.
            if (querySettings.Sort.Count() == 1 && querySettings.Sort.Single().PropertyName == "DenialReasonType")
            {
                var direction = querySettings.Sort.Single().Direction;
                IList<ListAdvertisementElementDenialReasonsDto> sortedResult;
                switch (direction)
                {
                    case SortDirection.Ascending:
                    {
                        sortedResult = result.OrderBy(x => x.DenialReasonName).ToList();
                        break;
                    }

                    case SortDirection.Descending:
                    {
                        sortedResult = result.OrderByDescending(x => x.DenialReasonName).ToList();
                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return new RemoteCollection<ListAdvertisementElementDenialReasonsDto>(sortedResult, result.TotalCount);
            }

            return result;
        }

        private IRemoteCollection ListPossibleReasons(QuerySettings querySettings)
        {
            if (!querySettings.ParentEntityId.HasValue)
            {
                throw new ArgumentException("ParentEntityId must be specified", "querySettings");
            }

            var advertisementElementId = querySettings.ParentEntityId.Value;

            var checkedReasons = _finder.FindAll<AdvertisementElementDenialReason>().Where(x => x.AdvertisementElementId == advertisementElementId);
            return _finder.FindAll<DenialReason>()
                          .GroupJoin(checkedReasons, dr => dr.Id, aedr => aedr.DenialReasonId, (dr, aedr) => new { dr, aedr = aedr.FirstOrDefault() })
                          .Select(x => new ListAdvertisementElementDenialReasonsDto
                              {
                                  Id = x.aedr != null ? x.aedr.Id : 0,
                                  AdvertisementElementId = advertisementElementId,
                                  DenialReasonId = x.dr.Id,
                                  DenialReasonName = x.dr.Name,
                                  DenialReasonTypeEnum = (DenialReasonType)x.dr.Type,
                                  Comment = x.aedr != null ? x.aedr.Comment : string.Empty,
                                  Checked = x.aedr != null,
                                  IsActive = x.dr.IsActive,
                              })
                          .QuerySettings(_filterHelper, querySettings)
                          .Transform(x =>
                              {
                                  x.DenialReasonType = x.DenialReasonTypeEnum.ToStringLocalized(EnumResources.ResourceManager,
                                                                                                _userContext.Profile.UserLocaleInfo.UserCultureInfo);
                                  return x;
                              });
        }
    }
}